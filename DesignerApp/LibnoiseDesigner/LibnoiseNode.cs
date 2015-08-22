using LibNoise;
using System;

namespace WorldForge.LibnoiseDesigner
{
    public class LibnoiseNode
    {
        public ModuleBase LibnoiseModule { get; set; }
        public string ID { get; set; }
        public LibnoiseNode[] Inputs { get; set; }

        public LibnoiseNode(ModuleBase sourceModule)
        {
            LibnoiseModule = sourceModule;
            ID = Guid.NewGuid().ToString();
            Inputs = new LibnoiseNode[5];
        }

        public LibnoiseNode(ModuleBase sourceModule, string ID)
        {
            LibnoiseModule = sourceModule;
            this.ID = ID;
        }

        public string ModuleType
        {
            get
            {
                return LibnoiseModule != null ? LibnoiseModule.GetType().Name : "";
            }
        }
    }
}
