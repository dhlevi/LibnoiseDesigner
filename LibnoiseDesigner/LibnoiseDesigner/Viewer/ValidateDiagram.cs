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
        /// <summary>
        /// Validate a set of Libnoise nodes to ensure they can generate without error, and
        /// that their names are unique
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>

        public static bool Validate(List<NodeViewModel> nodes)
        {   
            return ValidateNoiseGeneration(nodes) && ValidateUniqueNames(nodes);
        }

        /// <summary>
        /// Validate the LibNoise nodes will generate a result
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validate that the libnoise nodes contain only unique names
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
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
