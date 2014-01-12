using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WooScripter
{
    [Serializable]
    public class Material
    {
        public Material()
        {
            _SpecularPower = 5;
            _Opacity = 1.0f;
            _RefractiveIndex = 1.0f;
            _Density = 1.0f;
            _Shininess = 1.0f;
            _DiffuseColour = new CFConstant(1.0f, 0.5f, 0.0f);
            _SpecularColour = new CFConstant(1.0f, 1.0f, 1.0f);
            _EmissiveColour = new CFConstant(0.0f, 0.0f, 0.0f);
            _Reflectivity = new CFConstant(0.3f, 0.3f, 0.3f);
            _AbsorptionColour = new CFConstant(0.0f, 0.0f, 0.0f);
        }

        public Material(Material rhs)
        {
            _SpecularPower = rhs._SpecularPower;
            _Opacity = rhs._Opacity;
            _RefractiveIndex = rhs._RefractiveIndex;
            _Density = rhs._Density;
            _Shininess = rhs._Shininess;
            _DiffuseColour = rhs._DiffuseColour.Clone();
            _SpecularColour = rhs._SpecularColour.Clone();
            _EmissiveColour = rhs._EmissiveColour.Clone();
            _Reflectivity = rhs._Reflectivity.Clone();
            _AbsorptionColour = rhs._AbsorptionColour.Clone();
        }

        public XElement CreateElement(bool preview)
        {
            if (preview)
            {
                ColourFunction black = new CFConstant(0, 0, 0);
                return new XElement("MATERIAL",
                    new XAttribute("specularPower", 0),
                    new XAttribute("opacity", 1),
                    new XAttribute("density", 1),
                    new XAttribute("shininess", 0),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    black.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    black.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }
            else
            {
                return new XElement("MATERIAL",
                    new XAttribute("specularPower", _SpecularPower),
                    new XAttribute("opacity", _Opacity),
                    new XAttribute("density", _Density),
                    new XAttribute("shininess", _Shininess),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    _SpecularColour.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    _Reflectivity.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }
        }
        
        public ColourFunction _DiffuseColour;
        public ColourFunction _SpecularColour;
        public ColourFunction _EmissiveColour;
        public ColourFunction _Reflectivity;
        public ColourFunction _AbsorptionColour;
        public float _SpecularPower;
        public float _Opacity;
        public float _RefractiveIndex;
        public float _Density;
        public float _Shininess;
    }
}
