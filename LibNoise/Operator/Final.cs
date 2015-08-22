using System;

namespace LibNoise.Operator
{
    public class Final : Cache
    {
        public override string GetDescription()
        {
            return "Functionally, the Final Component is a Cache. This components only exists for the designer, and acts as the final node for generating the LibNoise values. You cannot delete the Final node, and only values which are attached to this node will be processed by World Forge, or saved when a LibNoise document is written to disk.";
        }
    }
}
