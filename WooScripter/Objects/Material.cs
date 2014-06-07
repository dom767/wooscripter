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
            _DiffuseFunction = "";
            _SpecularFunction = "";
            _EmissiveFunction = "";
            _ReflectiveFunction = "";
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
            _DiffuseFunction = rhs._DiffuseFunction;
            _SpecularFunction = rhs._SpecularFunction;
            _EmissiveFunction = rhs._EmissiveFunction;
            _ReflectiveFunction = rhs._ReflectiveFunction;
        }

        public XElement CreateElement(bool preview)
        {
            XElement mat;
            if (preview)
            {
                ColourFunction black = new CFConstant(0, 0, 0);
                mat = new XElement("MATERIAL",
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
                mat = new XElement("MATERIAL",
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

            if (_DiffuseFunction.Length > 0) mat.Add(new XAttribute("diffuseFunction", _DiffuseFunction));
            if (_SpecularFunction.Length > 0) mat.Add(new XAttribute("specularFunction", _SpecularFunction));
            if (_EmissiveFunction.Length > 0) mat.Add(new XAttribute("emissiveFunction", _EmissiveFunction));
            if (_ReflectiveFunction.Length > 0) mat.Add(new XAttribute("reflectiveFunction", _ReflectiveFunction));

            return mat;
        }

        public string _DiffuseFunction;
        public string _SpecularFunction;
        public string _EmissiveFunction;
        public string _ReflectiveFunction;
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
