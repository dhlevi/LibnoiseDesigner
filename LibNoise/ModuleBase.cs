using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LibNoise
{
    /// <summary>
    /// Defines a collection of quality modes.
    /// </summary>
    public enum QualityMode { Low, Medium, High }

    /// <summary>
    /// Base class for noise modules.
    /// </summary>
    public abstract class ModuleBase : IDisposable
    {
        private ModuleBase[] _modules;
        
        /// <summary>
        /// Initializes a new instance of a Base Module.
        /// </summary>
        /// <param name="count">The number of connecting source modules.</param>
        protected ModuleBase(int count)
        {
            if (count > 0) _modules = new ModuleBase[count];
        }

        /// <summary>
        /// Gets or sets a source module by index.
        /// </summary>
        /// <param name="index">The index of the source module to aquire.</param>
        /// <returns>The requested source module.</returns>
        public virtual ModuleBase this[int index]
        {
            get
            {
                if (index < 0 || index >= _modules.Length) throw new ArgumentOutOfRangeException("Index out of valid module range");
                if (_modules[index] == null) throw new ArgumentNullException("Desired element is null");
                
                return _modules[index];
            }
            set
            {
                if (index < 0 || index >= _modules.Length) throw new ArgumentOutOfRangeException("Index out of valid module range");
                if (value == null) throw new ArgumentNullException("Value should not be null");
                
                _modules[index] = value;
            }
        }

        [Category("Module Info")]
        [DisplayName("Modules")]
        [Description("The current module collection")]
        [Browsable(false)]
        public ModuleBase[] Modules
        {
            get { return _modules; }
        }

        /// <summary>
        /// Gets the number of source modules required by this noise module.
        /// </summary>
        [Category("Module Info")]
        [DisplayName("Total Modules")]
        [Description("The number of modules that can be attached to this module")]
        [Browsable(false)]
        public int SourceModuleCount
        {
            get { return (_modules == null) ? 0 : _modules.Length; }
        }

        /// <summary>
        /// Returns a supplied description by any inheriting class (For UI use mainly)
        /// </summary>
        /// <returns></returns>
        public abstract string GetDescription();

        /// <summary>
        /// Returns the output value for the given input coordinates.
        /// </summary>
        /// <param name="x">The input coordinate on the x-axis.</param>
        /// <param name="y">The input coordinate on the y-axis.</param>
        /// <param name="z">The input coordinate on the z-axis.</param>
        /// <returns>The resulting output value.</returns>
        public abstract double GetValue(double x, double y, double z, int scale);

        /// <summary>
        /// Returns the output value for the given input coordinates.
        /// </summary>
        /// <param name="coordinate">The input coordinate.</param>
        /// <returns>The resulting output value.</returns>
        public double GetValue(Vector3 coordinate, int scale)
        {
            return GetValue(coordinate.x, coordinate.y, coordinate.z, scale);
        }

        /// <summary>
        /// Returns the output value for the given input coordinates.
        /// </summary>
        /// <param name="coordinate">The input coordinate.</param>
        /// <returns>The resulting output value.</returns>
        public double GetValue(ref Vector3 coordinate, int scale)
        {
            return GetValue(coordinate.x, coordinate.y, coordinate.z, scale);
        }

        [XmlIgnore]
        private bool _disposed;

        /// <summary>
        /// Gets a value whether the object is disposed.
        /// </summary>
        [Category("Module Info")]
        [DisplayName("IsDisposed")]
        [Description("Informs if this module has been disposed, rendering it unusable.")]
        [Browsable(false)]
        public bool IsDisposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Immediately releases the unmanaged resources used by this object.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = Disposing();
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Immediately releases the unmanaged resources used by this object.
        /// </summary>
        /// <returns>True if the object is completely disposed.</returns>
        protected virtual bool Disposing()
        {
            if (_modules != null)
            {
                for (var i = 0; i < _modules.Length; i++)
                {
                    _modules[i].Dispose();
                    _modules[i] = null;
                }
                _modules = null;
            }
            return true;
        }
    }
}