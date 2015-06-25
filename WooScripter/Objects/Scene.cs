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
        public bool _Shadows = true;
        List<RenderObject> _RenderObjects = new List<RenderObject>();
        List<Light> _Lights = new List<Light>();
        Background _Background = new Background();
        public int _Recursions = 3;

        public XElement CreateElement(bool preview, bool simpleLighting)
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

            if (_Shadows || !preview)
                ret.Add(new XAttribute("shadows", true));
            else
                ret.Add(new XAttribute("shadows", false));

            ret.Add(new XAttribute("recursions", preview ? 1 : _Recursions));

            if (simpleLighting)
            {
                DirectionalLight directional = new DirectionalLight(new Colour(1, 1, 1), new Vector3(-1, 1, -1), 0.0f, 1);
                directional.CreateElement(ret, new Vector3(-1,1,-1));
            }
            else
            {
                foreach (Light light in _Lights)
                {
                    light.CreateElement(ret);
                }
            }

            for (int i=0; i<(simpleLighting?2:3); i++)
            {
                _RenderObjects[i].CreateElement(preview, ret);
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
