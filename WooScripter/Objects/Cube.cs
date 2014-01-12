using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Cube : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;

        public Cube(Vector3 centre, Vector3 scale, Matrix3 rotation)
        {
            _Material = new Material();
            _Position = new Vector3();
            _Scale = new Vector3();
            _Rotation = new Matrix3();
            _Position = centre;
            _Scale = scale;
            _Rotation = rotation;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            XElement ret = new XElement("OBJECT",
                new XAttribute("type", 2),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", false),
                _Material.CreateElement(preview));
            parent.Add(ret);
        }
    }
}
