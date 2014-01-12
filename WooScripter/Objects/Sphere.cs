using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Sphere : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;
        public bool _IgnoreLighting;

        public Sphere(Vector3 position, Vector3 scale, Matrix3 rotation)
        {
            _Position = position;
            _Scale = scale;
            _Rotation = rotation;
            _Material = new Material();
            _IgnoreLighting = false;
        }

        public Sphere()
        {
            _Position = new Vector3(0, 0, 0);
            _Scale = new Vector3(1, 1, 1);
            _Rotation = new Matrix3();
            _Rotation.MakeIdentity();
            _Material = new Material();
            _IgnoreLighting = false;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            parent.Add(new XElement("OBJECT",
                new XAttribute("type", 0),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", _IgnoreLighting),
                _Material.CreateElement(preview)));
        }
    }
}
