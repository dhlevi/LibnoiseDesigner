﻿using LibNoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorldForge.LibnoiseDesigner.Viewer
{
    public class LoadedModule
    {
        public string ID { get; set; }
        public ModuleBase Module { get; set; }
        public List<string> Links { get; private set; }
        public System.Windows.Point Location { get; set; }

        public LoadedModule() { }
        
        public LoadedModule(string id, ModuleBase module)
        {
            this.ID = id;
            this.Module = module;
            Links = new List<string>();
        }

        public LoadedModule(string id, ModuleBase module, System.Windows.Point location)
        {
            this.ID = id;
            this.Module = module;
            this.Location = location;
            Links = new List<string>();
        }

        public LoadedModule(string id, ModuleBase module, System.Windows.Point location, List<string> links)
        {
            this.ID = id;
            this.Module = module;
            this.Location = location;
            this.Links = links;
        }
    }
}
