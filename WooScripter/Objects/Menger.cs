using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter.Objects
{
    public class Menger : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;
        public int _Iterations;
        public int[] _Pattern = new int[27];

        public Menger(Vector3 centre, Vector3 scale, Matrix3 rotation, int iterations, int[] pattern)
        {
            _Material = new Material();
            _Position = new Vector3();
            _Scale = new Vector3();
            _Rotation = new Matrix3();
            _Position = centre;
            _Scale = scale;
            _Rotation = rotation;
            _Iterations = iterations;
            for(int i=0;i<27;i++) _Pattern[i] = pattern[i];
        }

        public void CreateElement(bool preview, XElement parent)
        {
            string patternStr = "";
            for (int i=0; i<27; i++)
            {
                patternStr += (_Pattern[i]>0?"1" : "0");
                if (i!=26) patternStr+=", ";
            }

            XElement ret = new XElement("OBJECT",
                new XAttribute("type", 4),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", false),
                new XAttribute("pattern", patternStr),
                new XAttribute("iterations", _Iterations),
                _Material.CreateElement(preview));
            parent.Add(ret);
        }
    }
}
