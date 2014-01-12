using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter
{
    public interface RenderObject
    {
        void CreateElement(bool preview, XElement parent);
    }
}
