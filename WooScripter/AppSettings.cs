using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WooScripter
{
    public class AppSettings
    {
        public Vector3 _CameraFrom;
        public Vector3 _CameraTo;
        public double _FOV;
        public double _ApertureSize;
        public double _Spherical;

        public AppSettings()
        {
            _CameraFrom = new Vector3(-2, 4, -4);
            _CameraTo = new Vector3(0, 0, 0);
            _FOV = 40;
            _ApertureSize = 0.1;
            _Spherical = 0.0;
        }

        public void Save(string filename, Camera camera)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                try
                {
                    _CameraFrom = camera._Position;
                    _CameraTo = camera._Target;
                    _FOV = camera._FOV;
                    _ApertureSize = camera._ApertureSize;
                    _Spherical = camera._Spherical;

                    XmlSerializer xmls = new XmlSerializer(typeof(AppSettings));
                    xmls.Serialize(sw, this);
                    sw.Close();
                }
                catch (Exception e)
                {
                    // lets not get overexcited...
                }
            }
        }

        public static AppSettings Load(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        XmlSerializer xmls = new XmlSerializer(typeof(AppSettings));
                        return xmls.Deserialize(sr) as AppSettings;
                        sr.Close();
                    }
                }
                catch (Exception)
                {
                    //ho-hum
                }
            }
            
            return new AppSettings();
        }
    }
}
