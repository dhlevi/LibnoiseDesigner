using LibNoise;
using NetworkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace WorldForge.LibnoiseDesigner.Viewer
{
    public class ValidateDiagram
    {
        public static bool Validate(List<NodeViewModel> nodes)
        {
            bool result = true;

            result = ValidateNoiseGeneration(nodes);
            if(result) result = ValidateUniqueNames(nodes);
            
            return result;
        }

        public static bool ValidateNoiseGeneration(List<NodeViewModel> nodes)
        {
            bool result = true;

            //final returns a usable result
            ModuleBase finalModule = nodes.First(n => n.Module.ModuleType.Equals("Final")).Module.LibnoiseModule;

            try
            {
                double val = finalModule.GetValue(0, 0, 0, 0);
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public static bool ValidateUniqueNames(List<NodeViewModel> nodes)
        {
            bool result = true;

            // names are all unique
            var q = nodes.GroupBy(x => x.Module.ID)
            .Select(x => new
            {
                Count = x.Count(),
                Name = x.Key,
                ID = x.First().Module.ID
            })
            .OrderByDescending(x => x.Count);

            if (q.Count(x => x.Count > 1) != 0) result = false;

            return result;
        } 
    }
}
