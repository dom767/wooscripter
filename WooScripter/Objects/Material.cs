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
            _TintDensity = 0.1f;
            _Shininess = 1.0f;
            _DiffuseColour = new CFConstant(1.0f, 0.5f, 0.0f);
            _SpecularColour = new CFConstant(1.0f, 1.0f, 1.0f);
            _EmissiveColour = new CFConstant(0.0f, 0.0f, 0.0f);
            _Reflectivity = new CFConstant(0.3f, 0.3f, 0.3f);
            _AbsorptionColour = new CFConstant(0.0f, 0.0f, 0.0f);
            _MaterialFunction = "";
        }

        public Material(Material rhs)
        {
            _SpecularPower = rhs._SpecularPower;
            _Opacity = rhs._Opacity;
            _RefractiveIndex = rhs._RefractiveIndex;
            _Density = rhs._Density;
            _TintDensity = rhs._TintDensity;
            _Shininess = rhs._Shininess;
            _DiffuseColour = rhs._DiffuseColour.Clone();
            _SpecularColour = rhs._SpecularColour.Clone();
            _EmissiveColour = rhs._EmissiveColour.Clone();
            _Reflectivity = rhs._Reflectivity.Clone();
            _AbsorptionColour = rhs._AbsorptionColour.Clone();
            _MaterialFunction = rhs._MaterialFunction;
        }

        public XElement CreateElement(bool preview)
        {
            XElement mat;
            if (preview)
            {
                ColourFunction black = new CFConstant(0, 0, 0);
                mat = new XElement("MATERIAL",
                    new XAttribute("specularPower", 10),
                    new XAttribute("opacity", _Opacity),
                    new XAttribute("density", _Density),
                    new XAttribute("tintdensity", _TintDensity),
                    new XAttribute("shininess", 1),
                    new XAttribute("refractiveIndex", _RefractiveIndex),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    black.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    black.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }
            else
            {
                mat = new XElement("MATERIAL",
                    new XAttribute("specularPower", _SpecularPower),
                    new XAttribute("opacity", _Opacity),
                    new XAttribute("density", _Density),
                    new XAttribute("tintdensity", _TintDensity),
                    new XAttribute("shininess", _Shininess),
                    new XAttribute("refractiveIndex", _RefractiveIndex),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    _SpecularColour.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    _Reflectivity.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }

            if (_MaterialFunction.Length > 0) mat.Add(new XAttribute("materialFunction", _MaterialFunction));

            return mat;
        }

        public string _MaterialFunction;
        public ColourFunction _DiffuseColour;
        public ColourFunction _SpecularColour;
        public ColourFunction _EmissiveColour;
        public ColourFunction _Reflectivity;
        public ColourFunction _AbsorptionColour;
        public float _SpecularPower;
        public float _Opacity;
        public float _RefractiveIndex;
        public float _Density;
        public float _TintDensity;
        public float _Shininess;
    }
}
