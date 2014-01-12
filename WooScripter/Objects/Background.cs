using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    [Serializable]
    public class Background
    {
        public Background()
        {
            _Simple = true;
            _BackgroundColour = new Colour(0, 0, 0);
        }

        public void CreateElement(XElement parent)
        {
            parent.Add(new XElement("BACKGROUND",
                new XAttribute("type", 0),
                new XAttribute("backgroundColour", _BackgroundColour)));
        }

        public Colour _BackgroundColour;
        public bool _Simple;
    }
}
