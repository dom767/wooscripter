using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using WooScripter.Objects.WooScript;

namespace WooScripter.Objects
{
    class Fractal : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;
        public double _DistanceMinimum;
        public double _DistanceScale;
        public Vector3 _DistanceOffset;
        public int _DistanceIterations;
        public Vector3 _DistanceExtents;
        public double _StepSize;
        public List<FractalIteration> _FractalIterations;

        public Fractal(Vector3 centre,
            Vector3 scale,
            Matrix3 rotation,
            double distanceMinimum,
            double distanceScale,
            Vector3 distanceOffset,
            int distanceIterations,
            Vector3 distanceExtents,
            double stepSize,
            List<FractalIteration> fractalIterations)
        {
            _Material = new Material();
            _Position = new Vector3();
            _Scale = new Vector3();
            _Rotation = new Matrix3();
            _Position = centre;
            _Scale = scale;
            _Rotation = rotation;
            _DistanceMinimum = distanceMinimum;
            _DistanceScale = distanceScale;
            _DistanceOffset = distanceOffset;
            _DistanceIterations = distanceIterations;
            _DistanceExtents = distanceExtents;
            _StepSize = stepSize;
            _FractalIterations = fractalIterations;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            XElement ret = new XElement("OBJECT",
                new XAttribute("type", 8),
                new XAttribute("minimumdistance", _DistanceMinimum),
                new XAttribute("distancescale", _DistanceScale),
                new XAttribute("distanceoffset", _DistanceOffset),
                new XAttribute("distanceiterations", _DistanceIterations),
                new XAttribute("distanceextents", _DistanceExtents),
                new XAttribute("stepsize", _StepSize),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", false));
            ret.Add(_Material.CreateElement(preview));
            for (int i = 0; i < _FractalIterations.Count(); i++)
            {
                ret.Add(_FractalIterations[i].CreateElement());
            }
            parent.Add(ret);
        }
    }
}
