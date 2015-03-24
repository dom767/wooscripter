using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Scene
    {
        public Scene(Camera camera)
        {
            _Camera = camera;
        }
        Camera _Camera;
        public bool _PathTracer = false;
        public bool _Caustics = false;
        List<RenderObject> _RenderObjects = new List<RenderObject>();
        List<Light> _Lights = new List<Light>();
        Background _Background = new Background();
        public int _Recursions = 3;

        public XElement CreateElement(bool preview)
        {
            XElement ret = new XElement("SCENE");

            if (_PathTracer && !preview)
                ret.Add(new XAttribute("pathTracer", 1));
            else
                ret.Add(new XAttribute("pathTracer", 0));

            if (_Caustics && !preview)
                ret.Add(new XAttribute("caustics", true));
            else
                ret.Add(new XAttribute("caustics", false));

            ret.Add(new XAttribute("recursions", _Recursions));

            foreach (Light light in _Lights)
            {
                light.CreateElement(ret);
            }

            foreach (RenderObject renderObject in _RenderObjects)
            {
                renderObject.CreateElement(preview, ret);
            }

//            _Background.CreateElement(ret);

            return ret;
        }

        public void AddLight(Light light)
        {
            _Lights.Add(light);
        }

        public void AddRenderObject(RenderObject renderObject)
        {
            _RenderObjects.Add(renderObject);
        }

        public int GetNumLights()
        {
            return _Lights.Count;
        }

        public int GetNumRenderObjects()
        {
            return _RenderObjects.Count;
        }

        public Light GetLight(int num)
        {
            return _Lights[num];
        }

        public RenderObject GetRenderObject(int num)
        {
            return _RenderObjects[num];
        }

        public Background GetBackground()
        {
            return _Background;
        }

        public void Clear()
        {
            _RenderObjects.Clear();
            _Lights.Clear();
            _Background = new Background();
        }
    }
}
