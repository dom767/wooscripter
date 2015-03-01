using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Light
    {
        public void CreateElement(XElement parent)
        {
            _LightInstance.CreateElement(parent, _Position);
        }

        public Light()
        {
            _Position = new Vector3(0, 20, 0);
            _LightInstance = new PointLight(new Colour(100.1f, 100.1f, 100.1f), new Vector3(0,20,0));
        }

        public Vector3 _Position;
        public InternalLight _LightInstance;
        
        public Light Clone()
        {
            Light clone = new Light();
            clone._LightInstance = _LightInstance.Clone();
            return clone;
        }
    }
    
    [Serializable]
    public abstract class InternalLight
    {
        public abstract void CreateElement(XElement parent, Vector3 position);
        public abstract InternalLight Clone();
        public abstract Colour GetColour();
    }
    
    [Serializable]
    public class PointLight : InternalLight
    {
        public Colour _Colour;
        public Vector3 _Position;

        public PointLight()
        {
            _Colour = new Colour();
            _Position = new Vector3();
        }

        public PointLight(Colour colour, Vector3 position)
        {
            _Colour = colour;
            _Position = position;
        }

        public override void CreateElement(XElement parent, Vector3 position)
        {
            parent.Add(new XElement("LIGHT",
                new XAttribute("type", 0),
                new XAttribute("position", position),
                new XAttribute("colour", _Colour)));
        }

        public override Colour GetColour()
        {
            return _Colour;
        }

        public override InternalLight Clone()
        {
            return new PointLight(_Colour, _Position);
        }
    }

    [Serializable]
    public class WorldLight : InternalLight
    {
        public Colour _Colour;
        public int _Samples;

        public WorldLight()
        {
            _Colour = new Colour();
            _Samples = 1;
        }

        public WorldLight(Colour colour, int samples)
        {
            _Colour = colour;
            _Samples = samples;
        }

        public override void CreateElement(XElement parent, Vector3 position)
        {
            parent.Add(new XElement("LIGHT",
                new XAttribute("type", 1),
                new XAttribute("colour", _Colour),
                new XAttribute("samples", _Samples)));
        }

        public override Colour GetColour()
        {
            return _Colour;
        }

        public override InternalLight Clone()
        {
            return new WorldLight(_Colour, _Samples);
        }
    }

    [Serializable]
    public class AmbientLight : InternalLight
    {
        public Colour _Colour;

        public AmbientLight()
        {
            _Colour = new Colour();
        }

        public AmbientLight(Colour colour)
        {
            _Colour = colour;
        }

        public override void CreateElement(XElement parent, Vector3 position)
        {
            parent.Add(new XElement("LIGHT",
                new XAttribute("type", 2),
                new XAttribute("colour", _Colour)));
        }

        public override Colour GetColour()
        {
            return _Colour;
        }

        public override InternalLight Clone()
        {
            return new AmbientLight(_Colour);
        }
    }

    [Serializable]
    public class DirectionalLight : InternalLight
    {
        public Colour _Colour;
        public Vector3 _Direction;
        public float _Area;
        public int _Samples;

        public DirectionalLight()
        {
            _Colour = new Colour();
            _Direction = new Vector3();
            _Area = 0.0f;
            _Samples = 1;
        }

        public DirectionalLight(Colour colour, Vector3 direction, float area, int samples)
        {
            _Colour = colour;
            _Direction = direction;
            _Area = area;
            _Samples = samples;
        }

        public override void CreateElement(XElement parent, Vector3 position)
        {
            parent.Add(new XElement("LIGHT",
                new XAttribute("type", 3),
                new XAttribute("colour", _Colour),
                new XAttribute("direction", _Direction),
                new XAttribute("area", _Area),
                new XAttribute("samples", _Samples)));
        }

        public override Colour GetColour()
        {
            return _Colour;
        }

        public override InternalLight Clone()
        {
            return new DirectionalLight(_Colour, _Direction, _Area, _Samples);
        }
    }

    [Serializable]
    public class SphereLight : InternalLight
    {
        public Colour _Colour;
        public Vector3 _Position;
        public float _Radius;
        public int _Samples;

        public SphereLight()
        {
            _Colour = new Colour();
            _Position = new Vector3();
            _Radius = 1;
            _Samples = 1;
        }

        public SphereLight(Colour colour, Vector3 position, float radius, int samples)
        {
            _Colour = colour;
            _Position = position;
            _Radius = radius;
            _Samples = samples;
        }

        public override void CreateElement(XElement parent, Vector3 position)
        {
            Material mat = new Material();
            mat._DiffuseColour = new CFConstant(0, 0, 0);
            mat._SpecularColour = new CFConstant(0, 0, 0);
            mat._Reflectivity = new CFConstant(0, 0, 0);
            float attenuation = 1;
            mat._EmissiveColour = new CFConstant(_Colour._Red * attenuation, _Colour._Green * attenuation, _Colour._Blue * attenuation);

            Sphere sphere = new Sphere();
            sphere._Position = position;
            sphere._Scale = new Vector3(_Radius, _Radius, _Radius);
            sphere._Material = mat;
            sphere._IgnoreLighting = true;
            sphere.CreateElement(false, parent);

            parent.Add(new XElement("LIGHT",
                new XAttribute("type", 4),
                new XAttribute("colour", _Colour),
                new XAttribute("position", _Position),
                new XAttribute("radius", _Radius),
                new XAttribute("samples", _Samples)));
        }

        public override Colour GetColour()
        {
            return _Colour;
        }

        public override InternalLight Clone()
        {
            return new SphereLight(_Colour, _Position, _Radius, _Samples);
        }
    }
}
