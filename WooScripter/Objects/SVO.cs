using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooScripter.Objects.WooScript
{
    public class SVO : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;
        public string _DistanceFunction;
        public double _DistanceMinimum;
        public double _DistanceScale;
        public Vector3 _DistanceOffset;
        public double _StepSize;
        public int _Depth;

        public SVO(Vector3 centre,
            Vector3 scale,
            Matrix3 rotation,
            string distanceFunction,
            double distanceMinimum,
            double distanceScale,
            Vector3 distanceOffset,
            double stepSize,
            int depth)
        {
            _Material = new Material();
            _Position = new Vector3();
            _Scale = new Vector3();
            _Rotation = new Matrix3();
            _Position = centre;
            _Scale = scale;
            _Rotation = rotation;
            _DistanceFunction = distanceFunction;
            _DistanceMinimum = distanceMinimum;
            _DistanceScale = distanceScale;
            _DistanceOffset = distanceOffset;
            _StepSize = stepSize;
            _Depth = depth;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            XElement ret = new XElement("OBJECT",
                new XAttribute("type", 7),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", false),
                new XAttribute("distancefunction", _DistanceFunction),
                new XAttribute("minimumdistance", _DistanceMinimum),
                new XAttribute("distancescale", _DistanceScale),
                new XAttribute("distanceoffset", _DistanceOffset),
                new XAttribute("stepsize", _StepSize),
                new XAttribute("depth", _Depth),
                _Material.CreateElement(preview));
            parent.Add(ret);
        }
    }
}
