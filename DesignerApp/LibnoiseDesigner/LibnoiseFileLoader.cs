using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using NetworkModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Utils;
using WorldForge.LibnoiseDesigner.Viewer;

namespace WorldForge.LibnoiseDesigner
{
    public class LibnoiseFileUtils
    {
        public static StringBuilder ExportToClass(ImpObservableCollection<object> nodes)
        {
            StringBuilder sb = new StringBuilder();
            string final = "";

            sb.AppendLine("using System;");
            sb.AppendLine("using LibNoise;");
            sb.AppendLine("using LibNoise.Generator;");
            sb.AppendLine("using LibNoise.Operator;");
            sb.AppendLine("");
            sb.AppendLine("namespace Libnoise.CustomModule");
            sb.AppendLine("{");
            sb.AppendLine("    public class LibnoiseModule");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ModuleBase CustomNoiseModule()");
            sb.AppendLine("        {");

            foreach (Object o in nodes)
            {
                NodeViewModel nvm = (NodeViewModel)o;

                // create obeject instantiations
                switch (nvm.Module.ModuleType)
                {
                    case "Billow":
                        sb.AppendLine("            Billow " + nvm.Module.ID + " = new Billow();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Billow)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Lacunarity = " + ((Billow)nvm.Module.LibnoiseModule).Lacunarity.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".OctaveCount = " + ((Billow)nvm.Module.LibnoiseModule).OctaveCount.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Persistence = " + ((Billow)nvm.Module.LibnoiseModule).Persistence.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Quality = QualityMode." + ((Billow)nvm.Module.LibnoiseModule).Quality.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Seed = " + ((Billow)nvm.Module.LibnoiseModule).Seed.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Checker":
                        sb.AppendLine("            Checker " + nvm.Module.ID + " = new Checker();");
                        sb.AppendLine("");
                        break;
                    case "Const":
                        sb.AppendLine("            Const " + nvm.Module.ID + " = new Const();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Value = " + ((Const)nvm.Module.LibnoiseModule).Value.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Cylinders":
                        sb.AppendLine("            Cylinders " + nvm.Module.ID + " = new Cylinders();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Cylinders)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Perlin":
                        sb.AppendLine("            Perlin " + nvm.Module.ID + " = new Perlin();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Perlin)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Lacunarity = " + ((Perlin)nvm.Module.LibnoiseModule).Lacunarity.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".OctaveCount = " + ((Perlin)nvm.Module.LibnoiseModule).OctaveCount.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Persistence = " + ((Perlin)nvm.Module.LibnoiseModule).Persistence.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Quality = QualityMode." + ((Perlin)nvm.Module.LibnoiseModule).Quality.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Seed = " + ((Perlin)nvm.Module.LibnoiseModule).Seed.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "RidgedMultifractal":
                        sb.AppendLine("            RidgedMultifractal " + nvm.Module.ID + " = new RidgedMultifractal();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((RidgedMultifractal)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Lacunarity = " + ((RidgedMultifractal)nvm.Module.LibnoiseModule).Lacunarity.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".OctaveCount = " + ((RidgedMultifractal)nvm.Module.LibnoiseModule).OctaveCount.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Quality = QualityMode." + ((RidgedMultifractal)nvm.Module.LibnoiseModule).Quality.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Seed = " + ((RidgedMultifractal)nvm.Module.LibnoiseModule).Seed.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Spheres":
                        sb.AppendLine("            Spheres " + nvm.Module.ID + " = new Spheres();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Spheres)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Voronoi":
                        sb.AppendLine("            Voronoi " + nvm.Module.ID + " = new Voronoi();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Voronoi)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Displacement = " + ((Voronoi)nvm.Module.LibnoiseModule).Displacement.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".UseDistance = " + ((Voronoi)nvm.Module.LibnoiseModule).UseDistance.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Seed = " + ((Voronoi)nvm.Module.LibnoiseModule).Seed.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Abs":
                        sb.AppendLine("            Abs " + nvm.Module.ID + " = new Abs();");
                        sb.AppendLine("");
                        break;
                    case "Add":
                        sb.AppendLine("            Add " + nvm.Module.ID + " = new Add();");
                        sb.AppendLine("");
                        break;
                    case "Blend":
                        sb.AppendLine("            Blend " + nvm.Module.ID + " = new Blend();");
                        sb.AppendLine("");
                        break;
                    case "Cache":
                        sb.AppendLine("            Cache " + nvm.Module.ID + " = new Cache();");
                        sb.AppendLine("");
                        break;
                    case "Clamp":
                        sb.AppendLine("            Clamp " + nvm.Module.ID + " = new Clamp();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Maximum = " + ((Clamp)nvm.Module.LibnoiseModule).Maximum.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Minimum = " + ((Clamp)nvm.Module.LibnoiseModule).Minimum.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Curve":
                        sb.AppendLine("            Curve " + nvm.Module.ID + " = new Curve();");

                        foreach (CurveControlPoint cp in ((Curve)nvm.Module.LibnoiseModule).ControlPoints)
                        {
                            sb.AppendLine("            " + nvm.Module.ID + ".Add(" + cp.X + ", " + cp.Y + ");");
                        }

                        sb.AppendLine("");
                        break;
                    case "Displace":
                        sb.AppendLine("            Displace " + nvm.Module.ID + " = new Displace();");
                        sb.AppendLine("");
                        break;
                    case "Exponent":
                        sb.AppendLine("            Exponent " + nvm.Module.ID + " = new Exponent();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Value = " + ((Exponent)nvm.Module.LibnoiseModule).Value.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Invert":
                        sb.AppendLine("            Invert " + nvm.Module.ID + " = new Invert();");
                        sb.AppendLine("");
                        break;
                    case "Max":
                        sb.AppendLine("            Max " + nvm.Module.ID + " = new Max();");
                        sb.AppendLine("");
                        break;
                    case "Min":
                        sb.AppendLine("            Min " + nvm.Module.ID + " = new Min();");
                        sb.AppendLine("");
                        break;
                    case "Multiply":
                        sb.AppendLine("            Multiply " + nvm.Module.ID + " = new Multiply();");
                        sb.AppendLine("");
                        break;
                    case "Power":
                        sb.AppendLine("            Power " + nvm.Module.ID + " = new Power();");
                        sb.AppendLine("");
                        break;
                    case "Rotate":
                        sb.AppendLine("            Rotate " + nvm.Module.ID + " = new Rotate();");
                        sb.AppendLine("            " + nvm.Module.ID + ".X = " + ((Rotate)nvm.Module.LibnoiseModule).X.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Y = " + ((Rotate)nvm.Module.LibnoiseModule).Y.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Z = " + ((Rotate)nvm.Module.LibnoiseModule).Z.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Scale":
                        sb.AppendLine("            Scale " + nvm.Module.ID + " = new Scale();");
                        sb.AppendLine("            " + nvm.Module.ID + ".X = " + ((Scale)nvm.Module.LibnoiseModule).X.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Y = " + ((Scale)nvm.Module.LibnoiseModule).Y.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Z = " + ((Scale)nvm.Module.LibnoiseModule).Z.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "ScaleBias":
                        sb.AppendLine("            ScaleBias " + nvm.Module.ID + " = new ScaleBias();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Scale = " + ((ScaleBias)nvm.Module.LibnoiseModule).Scale.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Bias = " + ((ScaleBias)nvm.Module.LibnoiseModule).Bias.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Select":
                        sb.AppendLine("            Select " + nvm.Module.ID + " = new Select();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Minimum = " + ((Select)nvm.Module.LibnoiseModule).Minimum.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Maximum = " + ((Select)nvm.Module.LibnoiseModule).Maximum.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".FallOff = " + ((Select)nvm.Module.LibnoiseModule).FallOff.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Subtract":
                        sb.AppendLine("            Subtract " + nvm.Module.ID + " = new Subtract();");
                        sb.AppendLine("");
                        break;
                    case "Terrace":
                        sb.AppendLine("            Terrace " + nvm.Module.ID + " = new Terrace();");

                        foreach (double cp in ((Terrace)nvm.Module.LibnoiseModule).ControlPoints)
                        {
                            sb.AppendLine("            " + nvm.Module.ID + ".Add(" + cp + ");");
                        }

                        sb.AppendLine("");
                        break;
                    case "Translate":
                        sb.AppendLine("            Translate " + nvm.Module.ID + " = new Translate();");
                        sb.AppendLine("            " + nvm.Module.ID + ".X = " + ((Scale)nvm.Module.LibnoiseModule).X.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Y = " + ((Scale)nvm.Module.LibnoiseModule).Y.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Z = " + ((Scale)nvm.Module.LibnoiseModule).Z.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Turbulence":
                        sb.AppendLine("            Turbulence " + nvm.Module.ID + " = new Turbulence();");
                        sb.AppendLine("            " + nvm.Module.ID + ".Frequency = " + ((Turbulence)nvm.Module.LibnoiseModule).Frequency.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Power = " + ((Turbulence)nvm.Module.LibnoiseModule).Power.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Roughness = " + ((Turbulence)nvm.Module.LibnoiseModule).Roughness.ToString() + ";");
                        sb.AppendLine("            " + nvm.Module.ID + ".Seed = " + ((Turbulence)nvm.Module.LibnoiseModule).Seed.ToString() + ";");
                        sb.AppendLine("");
                        break;
                    case "Final":
                        final = nvm.Module.ID;
                        sb.AppendLine("            Cache " + nvm.Module.ID + " = new Cache();");
                        sb.AppendLine("");
                        break;
                    default:
                        break;
                }
            }

            // its pretty ugly, but now that we're 100% sure all of the objects have been instantiated, we can
            // assign the module links

            foreach (Object o in nodes)
            {
                NodeViewModel nvm = (NodeViewModel)o;

                if (nvm.Module.Inputs[0] != null) sb.AppendLine("            " + nvm.Module.ID + ".Modules[0] = " + nvm.Module.Inputs[0].ID + ";");
                if (nvm.Module.Inputs[1] != null) sb.AppendLine("            " + nvm.Module.ID + ".Modules[1] = " + nvm.Module.Inputs[1].ID + ";");
                if (nvm.Module.Inputs[2] != null) sb.AppendLine("            " + nvm.Module.ID + ".Modules[2] = " + nvm.Module.Inputs[2].ID + ";");
                if (nvm.Module.Inputs[3] != null) sb.AppendLine("            " + nvm.Module.ID + ".Modules[3] = " + nvm.Module.Inputs[3].ID + ";");
                if (nvm.Module.Inputs[4] != null) sb.AppendLine("            " + nvm.Module.ID + ".Modules[4] = " + nvm.Module.Inputs[4].ID + ";");
                sb.AppendLine("");
            }

            sb.AppendLine("            return " + final + ";");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb;
        }

        public static XmlDocument DiagramToXML(ImpObservableCollection<object> nodes)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = (XmlElement)doc.AppendChild(doc.CreateElement("LibNoise"));

            foreach (Object o in nodes)
            {
                NodeViewModel nvm = (NodeViewModel)o;

                XmlElement module = doc.CreateElement("Module");
                module.SetAttribute("type", nvm.Module.ModuleType);
                module.SetAttribute("guid", nvm.Module.ID);
                module.SetAttribute("position", nvm.X + "," + nvm.Y);

                switch (nvm.Module.ModuleType)
                {
                    case "Billow":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Billow)nvm.Module.LibnoiseModule).Frequency.ToString();
                        module.AppendChild(doc.CreateElement("Lacunarity")).InnerText = ((Billow)nvm.Module.LibnoiseModule).Lacunarity.ToString();
                        module.AppendChild(doc.CreateElement("OctaveCount")).InnerText = ((Billow)nvm.Module.LibnoiseModule).OctaveCount.ToString();
                        module.AppendChild(doc.CreateElement("Persistence")).InnerText = ((Billow)nvm.Module.LibnoiseModule).Persistence.ToString();
                        module.AppendChild(doc.CreateElement("Quality")).InnerText = ((Billow)nvm.Module.LibnoiseModule).Quality.ToString();
                        module.AppendChild(doc.CreateElement("Seed")).InnerText = ((Billow)nvm.Module.LibnoiseModule).Seed.ToString();
                        break;
                    case "Checker":
                        break;
                    case "Const":
                        module.AppendChild(doc.CreateElement("Value")).InnerText = ((Const)nvm.Module.LibnoiseModule).Value.ToString();
                        break;
                    case "Cylinders":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Cylinders)nvm.Module.LibnoiseModule).Frequency.ToString();
                        break;
                    case "Perlin":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).Frequency.ToString();
                        module.AppendChild(doc.CreateElement("Lacunarity")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).Lacunarity.ToString();
                        module.AppendChild(doc.CreateElement("OctaveCount")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).OctaveCount.ToString();
                        module.AppendChild(doc.CreateElement("Persistence")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).Persistence.ToString();
                        module.AppendChild(doc.CreateElement("Quality")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).Quality.ToString();
                        module.AppendChild(doc.CreateElement("Seed")).InnerText = ((Perlin)nvm.Module.LibnoiseModule).Seed.ToString();
                        break;
                    case "RidgedMultifractal":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((RidgedMultifractal)nvm.Module.LibnoiseModule).Frequency.ToString();
                        module.AppendChild(doc.CreateElement("Lacunarity")).InnerText = ((RidgedMultifractal)nvm.Module.LibnoiseModule).Lacunarity.ToString();
                        module.AppendChild(doc.CreateElement("OctaveCount")).InnerText = ((RidgedMultifractal)nvm.Module.LibnoiseModule).OctaveCount.ToString();
                        module.AppendChild(doc.CreateElement("Quality")).InnerText = ((RidgedMultifractal)nvm.Module.LibnoiseModule).Quality.ToString();
                        module.AppendChild(doc.CreateElement("Seed")).InnerText = ((RidgedMultifractal)nvm.Module.LibnoiseModule).Seed.ToString();
                        break;
                    case "Spheres":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Spheres)nvm.Module.LibnoiseModule).Frequency.ToString();
                        break;
                    case "Voronoi":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Voronoi)nvm.Module.LibnoiseModule).Frequency.ToString();
                        module.AppendChild(doc.CreateElement("Displacement")).InnerText = ((Voronoi)nvm.Module.LibnoiseModule).Displacement.ToString();
                        module.AppendChild(doc.CreateElement("UseDistance")).InnerText = ((Voronoi)nvm.Module.LibnoiseModule).UseDistance.ToString();
                        module.AppendChild(doc.CreateElement("Seed")).InnerText = ((Voronoi)nvm.Module.LibnoiseModule).Seed.ToString();
                        break;
                    case "Abs":
                        XmlNode absInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        absInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Add":
                        XmlNode addInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        addInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        addInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Blend":
                        XmlNode blendInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        blendInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        blendInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        blendInputs.AppendChild(doc.CreateElement("Operator")).InnerText = nvm.Module.Inputs[2].ID;
                        break;
                    case "Cache":
                        XmlNode cacheInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        cacheInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Clamp":
                        module.AppendChild(doc.CreateElement("Maximum")).InnerText = ((Clamp)nvm.Module.LibnoiseModule).Maximum.ToString();
                        module.AppendChild(doc.CreateElement("Minimum")).InnerText = ((Clamp)nvm.Module.LibnoiseModule).Minimum.ToString();
                        XmlNode clampInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        clampInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Curve":
                        XmlNode controlPointInputs = module.AppendChild(doc.CreateElement("ControlPoints"));
                        foreach (CurveControlPoint cp in ((Curve)nvm.Module.LibnoiseModule).ControlPoints)
                        {
                            controlPointInputs.AppendChild(doc.CreateElement("ControlPoint")).InnerText = cp.X + "," + cp.Y;
                        }
                        XmlNode curveInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        curveInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Displace":
                        XmlNode displaceInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        displaceInputs.AppendChild(doc.CreateElement("Primary")).InnerText = nvm.Module.Inputs[0].ID;
                        displaceInputs.AppendChild(doc.CreateElement("X")).InnerText = nvm.Module.Inputs[1].ID;
                        displaceInputs.AppendChild(doc.CreateElement("Y")).InnerText = nvm.Module.Inputs[2].ID;
                        displaceInputs.AppendChild(doc.CreateElement("Z")).InnerText = nvm.Module.Inputs[3].ID;
                        break;
                    case "Exponent":
                        module.AppendChild(doc.CreateElement("Value")).InnerText = ((Exponent)nvm.Module.LibnoiseModule).Value.ToString();
                        XmlNode expInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        expInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Invert":
                        XmlNode invertInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        invertInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Max":
                        XmlNode maxInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        maxInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        maxInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Min":
                        XmlNode minInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        minInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        minInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Multiply":
                        XmlNode multiplyInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        multiplyInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        multiplyInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Power":
                        XmlNode powerInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        powerInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        powerInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Rotate":
                        module.AppendChild(doc.CreateElement("X")).InnerText = ((Rotate)nvm.Module.LibnoiseModule).X.ToString();
                        module.AppendChild(doc.CreateElement("Y")).InnerText = ((Rotate)nvm.Module.LibnoiseModule).Y.ToString();
                        module.AppendChild(doc.CreateElement("Z")).InnerText = ((Rotate)nvm.Module.LibnoiseModule).Z.ToString();
                        XmlNode rotateInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        rotateInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Scale":
                        module.AppendChild(doc.CreateElement("X")).InnerText = ((Scale)nvm.Module.LibnoiseModule).X.ToString();
                        module.AppendChild(doc.CreateElement("Y")).InnerText = ((Scale)nvm.Module.LibnoiseModule).Y.ToString();
                        module.AppendChild(doc.CreateElement("Z")).InnerText = ((Scale)nvm.Module.LibnoiseModule).Z.ToString();
                        XmlNode scaleInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        scaleInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "ScaleBias":
                        module.AppendChild(doc.CreateElement("Scale")).InnerText = ((ScaleBias)nvm.Module.LibnoiseModule).Scale.ToString();
                        module.AppendChild(doc.CreateElement("Bias")).InnerText = ((ScaleBias)nvm.Module.LibnoiseModule).Bias.ToString();
                        XmlNode scaleBiasInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        scaleBiasInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Select":
                        module.AppendChild(doc.CreateElement("Minimum")).InnerText = ((Select)nvm.Module.LibnoiseModule).Minimum.ToString();
                        module.AppendChild(doc.CreateElement("Maximum")).InnerText = ((Select)nvm.Module.LibnoiseModule).Maximum.ToString();
                        module.AppendChild(doc.CreateElement("FallOff")).InnerText = ((Select)nvm.Module.LibnoiseModule).FallOff.ToString();
                        XmlNode selectInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        selectInputs.AppendChild(doc.CreateElement("Primary")).InnerText = nvm.Module.Inputs[0].ID;
                        selectInputs.AppendChild(doc.CreateElement("Secondary")).InnerText = nvm.Module.Inputs[1].ID;
                        selectInputs.AppendChild(doc.CreateElement("Controller")).InnerText = nvm.Module.Inputs[2].ID;
                        break;
                    case "Subtract":
                        XmlNode subtractInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        subtractInputs.AppendChild(doc.CreateElement("Left")).InnerText = nvm.Module.Inputs[0].ID;
                        subtractInputs.AppendChild(doc.CreateElement("Right")).InnerText = nvm.Module.Inputs[1].ID;
                        break;
                    case "Terrace":
                        XmlNode terraceControlPointInputs = module.AppendChild(doc.CreateElement("ControlPoints"));
                        foreach (double cp in ((Terrace)nvm.Module.LibnoiseModule).ControlPoints)
                        {
                            terraceControlPointInputs.AppendChild(doc.CreateElement("ControlPoint")).InnerText = cp.ToString();
                        }
                        XmlNode terraceInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        terraceInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Translate":
                        module.AppendChild(doc.CreateElement("X")).InnerText = ((Translate)nvm.Module.LibnoiseModule).X.ToString();
                        module.AppendChild(doc.CreateElement("Y")).InnerText = ((Translate)nvm.Module.LibnoiseModule).Y.ToString();
                        module.AppendChild(doc.CreateElement("Z")).InnerText = ((Translate)nvm.Module.LibnoiseModule).Z.ToString();
                        XmlNode translateInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        translateInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Turbulence":
                        module.AppendChild(doc.CreateElement("Frequency")).InnerText = ((Turbulence)nvm.Module.LibnoiseModule).Frequency.ToString();
                        module.AppendChild(doc.CreateElement("Power")).InnerText = ((Turbulence)nvm.Module.LibnoiseModule).Power.ToString();
                        module.AppendChild(doc.CreateElement("Roughness")).InnerText = ((Turbulence)nvm.Module.LibnoiseModule).Roughness.ToString();
                        module.AppendChild(doc.CreateElement("Seed")).InnerText = ((Turbulence)nvm.Module.LibnoiseModule).Seed.ToString();
                        XmlNode turbInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        turbInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    case "Final":
                        XmlNode finalInputs = module.AppendChild(doc.CreateElement("ModuleInputs"));
                        finalInputs.AppendChild(doc.CreateElement("Input")).InnerText = nvm.Module.Inputs[0].ID;
                        break;
                    default:
                        break;
                }
                root.AppendChild(module);
            }

            return doc;
        }

        public static ImageSource LoadDefaultTileImage()
        {
            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = EmbeddedResourceUtils.GetEmbeddedResourceStream("WorldForge.Resources.temp_tile.png");
            imageSource.EndInit();

            return imageSource;
        }

        public static void LoadDefaultXml(DesignerViewer LibnoiseDesigner)
        {
            Stream defaultXml = EmbeddedResourceUtils.GetEmbeddedResourceStream("WorldForge.Resources.default.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(defaultXml);
            LoadLibnoiseXml(doc, LibnoiseDesigner);
        }

        public static void LoadDetailedXml(DesignerViewer LibnoiseDesigner)
        {
            Stream detailedXml = EmbeddedResourceUtils.GetEmbeddedResourceStream("WorldForge.Resources.detailed.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(detailedXml);
            LoadLibnoiseXml(doc, LibnoiseDesigner);
        }

        public static void LoadLibnoiseXml(XmlDocument doc, DesignerViewer LibnoiseDesigner)
        {
            XmlNodeList moduleList = doc.GetElementsByTagName("Module");
            List<LoadedModule> loadedModules = GetModules(moduleList);

            LibnoiseDesigner.ViewModel.DeleteAllNodes();

            //loop through modules, create nodes
            foreach (LoadedModule lm in loadedModules)
            {
                NodeViewModel node = LibnoiseDesigner.ViewModel.CreateNode(lm.Module, lm.Location, false);
                node.Module.ID = lm.ID;
            }

            foreach (LoadedModule lm in loadedModules)
            {
                int count = 0;
                NodeViewModel destinationNodeModel = LibnoiseDesigner.ViewModel.Network.Nodes.First(n => n.Module.ID.Equals(lm.ID));
                foreach (string link in lm.Links)
                {
                    NodeViewModel sourceNodeModel = LibnoiseDesigner.ViewModel.Network.Nodes.First(n => n.Module.ID.Equals(link));

                    ConnectionViewModel cvm = new ConnectionViewModel();
                    cvm.DestConnector = destinationNodeModel.InputConnectors[count];
                    cvm.SourceConnector = sourceNodeModel.OutputConnectors[0]; // only ever one output connector in our Libnoise model

                    LibnoiseNode sourceNode = sourceNodeModel.Module;
                    LibnoiseNode destinationNode = destinationNodeModel.Module;
                    ModuleBase sourceModule = sourceNode.LibnoiseModule;
                    ModuleBase destinationModule = destinationNode.LibnoiseModule;

                    destinationModule.Modules[count] = sourceModule;
                    destinationNode.Inputs[count] = sourceNode;

                    LibnoiseDesigner.ViewModel.Network.Connections.Add(cvm);

                    count++;
                }
            }
        }

        public static ModuleBase LoadLibnoiseXml(XmlDocument doc)
        {
            XmlNodeList moduleList = doc.GetElementsByTagName("Module");
            List<LoadedModule> loadedModules = GetModules(moduleList);

            // for each loaded module from XML, create a linked module, and write the "Final" back

            //loop through modules, create nodes
            ModuleBase finalModule = null;
            foreach (LoadedModule lm in loadedModules)
            {
                if (lm.Module.GetType() == typeof(Final)) finalModule = lm.Module;

                for (int i = 0; i < lm.Links.Count; i++)
                {
                    lm.Module.Modules[i] = loadedModules.First(m => m.ID.Equals(lm.Links[i])).Module;
                }
            }

            return finalModule;
        }

        private static List<LoadedModule> GetModules(XmlNodeList moduleList)
        {
            List<LoadedModule> loadedModules = new List<LoadedModule>();

            foreach (XmlNode node in moduleList)
            {
                string id = node.Attributes["guid"].Value;
                Point position = new Point(ParseDouble(node.Attributes["position"].Value.Split(',')[0]), ParseDouble(node.Attributes["position"].Value.Split(',')[1]));
                ModuleBase module = null;

                List<string> links = new List<string>();

                switch (node.Attributes["type"].Value)
                {
                    case "Billow":
                        Billow billow = new Billow();
                        billow.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        billow.Lacunarity = ParseDouble(node.SelectSingleNode("Lacunarity").InnerText);
                        billow.OctaveCount = int.Parse(node.SelectSingleNode("OctaveCount").InnerText);
                        billow.Persistence = ParseDouble(node.SelectSingleNode("Persistence").InnerText);
                        billow.Quality = (QualityMode)Enum.Parse(typeof(QualityMode), node.SelectSingleNode("Quality").InnerText);
                        billow.Seed = int.Parse(node.SelectSingleNode("Seed").InnerText);
                        module = billow;
                        break;
                    case "Checker":
                        module = new Checker();
                        break;
                    case "Const":
                        Const con = new Const();
                        con.Value = ParseDouble(node.SelectSingleNode("Value").InnerText);
                        module = con;
                        break;
                    case "Cylinders":
                        Cylinders cylinder = new Cylinders();
                        cylinder.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        module = cylinder;
                        break;
                    case "Perlin":
                        Perlin perlin = new Perlin();
                        perlin.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        perlin.Lacunarity = ParseDouble(node.SelectSingleNode("Lacunarity").InnerText);
                        perlin.OctaveCount = int.Parse(node.SelectSingleNode("OctaveCount").InnerText);
                        perlin.Persistence = ParseDouble(node.SelectSingleNode("Persistence").InnerText);
                        perlin.Quality = (QualityMode)Enum.Parse(typeof(QualityMode), node.SelectSingleNode("Quality").InnerText);
                        perlin.Seed = int.Parse(node.SelectSingleNode("Seed").InnerText);
                        module = perlin;
                        break;
                    case "RidgedMultifractal":
                        RidgedMultifractal ridgedMF = new RidgedMultifractal();
                        ridgedMF.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        ridgedMF.Lacunarity = ParseDouble(node.SelectSingleNode("Lacunarity").InnerText);
                        ridgedMF.OctaveCount = int.Parse(node.SelectSingleNode("OctaveCount").InnerText);
                        ridgedMF.Quality = (QualityMode)Enum.Parse(typeof(QualityMode), node.SelectSingleNode("Quality").InnerText);
                        ridgedMF.Seed = int.Parse(node.SelectSingleNode("Seed").InnerText);
                        module = ridgedMF;
                        break;
                    case "Spheres":
                        Spheres spheres = new Spheres();
                        spheres.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        module = spheres;
                        break;
                    case "Voronoi":
                        Voronoi voronoi = new Voronoi();
                        voronoi.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        voronoi.Displacement = ParseDouble(node.SelectSingleNode("Displacement").InnerText);
                        voronoi.UseDistance = bool.Parse(node.SelectSingleNode("UseDistance").InnerText);
                        voronoi.Seed = int.Parse(node.SelectSingleNode("Seed").InnerText);
                        module = voronoi;
                        break;
                    case "Abs":
                        module = new Abs();
                        XmlNode absInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(absInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Add":
                        module = new Add();
                        XmlNode addInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(addInputs.SelectSingleNode("Left").InnerText);
                        links.Add(addInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Blend":
                        module = new Blend();
                        XmlNode blendInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(blendInputs.SelectSingleNode("Left").InnerText);
                        links.Add(blendInputs.SelectSingleNode("Right").InnerText);
                        links.Add(blendInputs.SelectSingleNode("Operator").InnerText);
                        break;
                    case "Cache":
                        module = new Cache();
                        XmlNode cacheInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(cacheInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Clamp":
                        Clamp clamp = new Clamp();
                        clamp.Maximum = ParseDouble(node.SelectSingleNode("Maximum").InnerText);
                        clamp.Minimum = ParseDouble(node.SelectSingleNode("Minimum").InnerText);
                        module = clamp;

                        XmlNode clampInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(clampInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Curve":
                        Curve curve = new Curve();
                        module = curve;

                        foreach (XmlNode cpNode in node.SelectSingleNode("ControlPoints").ChildNodes)
                        {
                            double x = ParseDouble(cpNode.InnerText.Split(',')[0]);
                            double y = ParseDouble(cpNode.InnerText.Split(',')[1]);
                            curve.Add(x, y);
                        }

                        XmlNode curveInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(curveInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Displace":
                        module = new Displace();
                        XmlNode displaceInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(displaceInputs.SelectSingleNode("Primary").InnerText);
                        links.Add(displaceInputs.SelectSingleNode("X").InnerText);
                        links.Add(displaceInputs.SelectSingleNode("Y").InnerText);
                        links.Add(displaceInputs.SelectSingleNode("Z").InnerText);
                        break;
                    case "Exponent":
                        Exponent exponent = new Exponent();
                        exponent.Value = ParseDouble(node.SelectSingleNode("Value").InnerText);
                        module = exponent;

                        XmlNode exponentInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(exponentInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Invert":
                        module = new Invert();
                        XmlNode invertInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(invertInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Max":
                        module = new Max();
                        XmlNode maxInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(maxInputs.SelectSingleNode("Left").InnerText);
                        links.Add(maxInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Min":
                        module = new Min();
                        XmlNode minInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(minInputs.SelectSingleNode("Left").InnerText);
                        links.Add(minInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Multiply":
                        module = new Multiply();
                        XmlNode multiplyInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(multiplyInputs.SelectSingleNode("Left").InnerText);
                        links.Add(multiplyInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Power":
                        module = new Power();
                        XmlNode powerInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(powerInputs.SelectSingleNode("Left").InnerText);
                        links.Add(powerInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Rotate":
                        Rotate rotate = new Rotate();
                        rotate.X = ParseDouble(node.SelectSingleNode("X").InnerText);
                        rotate.Y = ParseDouble(node.SelectSingleNode("Y").InnerText);
                        rotate.Z = ParseDouble(node.SelectSingleNode("Z").InnerText);
                        module = rotate;

                        XmlNode rotateInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(rotateInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Scale":
                        Scale scale = new Scale();
                        scale.X = ParseDouble(node.SelectSingleNode("X").InnerText);
                        scale.Y = ParseDouble(node.SelectSingleNode("Y").InnerText);
                        scale.Z = ParseDouble(node.SelectSingleNode("Z").InnerText);
                        module = scale;

                        XmlNode scaleInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(scaleInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "ScaleBias":
                        ScaleBias scaleBias = new ScaleBias();
                        scaleBias.Scale = ParseDouble(node.SelectSingleNode("Scale").InnerText);
                        scaleBias.Bias = ParseDouble(node.SelectSingleNode("Bias").InnerText);
                        module = scaleBias;

                        XmlNode scaleBiasInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(scaleBiasInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Select":
                        Select select = new Select();
                        select.Minimum = ParseDouble(node.SelectSingleNode("Minimum").InnerText);
                        select.Maximum = ParseDouble(node.SelectSingleNode("Maximum").InnerText);
                        select.FallOff = ParseDouble(node.SelectSingleNode("FallOff").InnerText);
                        module = select;

                        XmlNode selectInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(selectInputs.SelectSingleNode("Primary").InnerText);
                        links.Add(selectInputs.SelectSingleNode("Secondary").InnerText);
                        links.Add(selectInputs.SelectSingleNode("Controller").InnerText);
                        break;
                    case "Subtract":
                        module = new Subtract();
                        XmlNode subtractInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(subtractInputs.SelectSingleNode("Left").InnerText);
                        links.Add(subtractInputs.SelectSingleNode("Right").InnerText);
                        break;
                    case "Terrace":
                        Terrace terrace = new Terrace();
                        module = terrace;

                        foreach (XmlNode cpNode in node.SelectSingleNode("ControlPoints").ChildNodes)
                        {
                            terrace.Add(ParseDouble(cpNode.InnerText));
                        }

                        XmlNode terraceInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(terraceInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Translate":
                        Translate translate = new Translate();
                        translate.X = ParseDouble(node.SelectSingleNode("X").InnerText);
                        translate.Y = ParseDouble(node.SelectSingleNode("Y").InnerText);
                        translate.Z = ParseDouble(node.SelectSingleNode("Z").InnerText);
                        module = translate;

                        XmlNode translateInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(translateInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Turbulence":
                        Turbulence turbulence = new Turbulence();
                        turbulence.Frequency = ParseDouble(node.SelectSingleNode("Frequency").InnerText);
                        turbulence.Power = ParseDouble(node.SelectSingleNode("Power").InnerText);
                        turbulence.Roughness = int.Parse(node.SelectSingleNode("Roughness").InnerText);
                        turbulence.Seed = int.Parse(node.SelectSingleNode("Seed").InnerText);
                        module = turbulence;

                        XmlNode turbulenceInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(turbulenceInputs.SelectSingleNode("Input").InnerText);
                        break;
                    case "Final":
                        module = new Final();
                        XmlNode finalInputs = node.SelectSingleNode("ModuleInputs");
                        links.Add(finalInputs.SelectSingleNode("Input").InnerText);
                        break;
                    default:
                        break;
                }

                LoadedModule lm = new LoadedModule(id, module, position, links);
                loadedModules.Add(lm);
            }

            return loadedModules;
        }

        private static double ParseDouble(string val)
        {
            return Double.Parse(val, CultureInfo.InvariantCulture);
        }
    }
}
