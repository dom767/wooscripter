using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Camera
    {
        public Vector3 _Position;
        public Vector3 _Target;
        public double _FOV;
        public bool _DOFEnabled;
        public bool _AAEnabled;
        public double _FocusDepth;
        public double _ApertureSize;
        public int _MinSamples;
        public int _MaxSamples;

        public Camera(Vector3 position, Vector3 target, double fov)
        {
            _Position = position;
            _Target = target;
            _FOV = fov;
            _DOFEnabled = false;
            _AAEnabled = false;
            _FocusDepth = 1;
            _ApertureSize = 1;
            _MinSamples = 1;
            _MaxSamples = 1;
        }

        public XElement CreateElement()
        {
            return new XElement("CAMERA",
                new XAttribute("from", _Position),
                new XAttribute("target", _Target),
                new XAttribute("up", new Vector3(0,1,0)),
                new XAttribute("fov", _FOV),
                new XAttribute("dofEnabled", _DOFEnabled),
                new XAttribute("aaEnabled", _AAEnabled),
                new XAttribute("focusDepth", _FocusDepth),
                new XAttribute("apertureSize", _ApertureSize),
                new XAttribute("minSamples", _MinSamples),
                new XAttribute("maxSamples", _MaxSamples));
        }
    }
}
