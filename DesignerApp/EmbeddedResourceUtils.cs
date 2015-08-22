using System.IO;
using System.Reflection;

namespace Utils
{
    public class EmbeddedResourceUtils
    {
        /// <summary>
        /// Takes the full name of a resource and loads it in to a stream.
        /// </summary>
        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Get the list of all emdedded resources in the assembly.
        /// </summary>
        /// <returns>An array of fully qualified resource names</returns>
        public static string[] GetEmbeddedResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
    }
}
