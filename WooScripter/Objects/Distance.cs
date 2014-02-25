using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using WooScripter.Objects.WooScript;

namespace WooScripter.Objects
{
    class Distance : RenderObject
    {
        public Vector3 _Position;
        public Vector3 _Scale;
        public Matrix3 _Rotation;
        public Material _Material;
        public string _DistanceFunction;
        public double _DistanceMinimum;
        public double _DistanceScale;
        public Vector3 _DistanceOffset;
        public int _DistanceIterations;

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDistanceSchemaLength();
        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDistanceSchema(StringBuilder sb);

        public class DistParam
        {
            public int _Index;
            public string _Name;
            public int _Type;
        }
        public class DistFunc
        {
            public void Read(XmlReader reader)
            {
                if (reader.Name == "FLOATFUNC") _ReturnType = 1;
                else _ReturnType = 0;

                reader.MoveToNextAttribute();
                if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "name")
                    _Name = reader.Value;

                _DistParams = new List<DistParam>();
                while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PARAM")
                    {
                        DistParam param = new DistParam();

                        // index, name, type
                        reader.MoveToNextAttribute();
                        if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "index")
                            param._Index = int.Parse(reader.Value);

                        reader.MoveToNextAttribute();
                        if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "name")
                            param._Name = reader.Value;

                        reader.MoveToNextAttribute();
                        if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "type")
                        {
                            switch (reader.Value)
                            {
                                case "vec":
                                    param._Type = 0;
                                    break;
                                case "float":
                                    param._Type = 1;
                                    break;
                                case "rawfloat":
                                    param._Type = 2;
                                    break;
                            }
                        }

                        _DistParams.Add(param);
                    }
                }

            }
            public int _ReturnType;
            public string _Name;
            public List<DistParam> _DistParams;
        };
        public static List<DistFunc> _DistFunc = new List<DistFunc>();

        public static string GetHelpText()
        {
            string ret = "";

            for (int i = 0; i < _DistFunc.Count(); i++)
            {
                ret += _DistFunc[i]._Name;
                ret += "(";
                for (int p = 0; p < _DistFunc[i]._DistParams.Count(); p++)
                {
                    ret += _DistFunc[i]._DistParams[p]._Name;
                    if (p != _DistFunc[i]._DistParams.Count() - 1)
                        ret += ", ";
                }
                ret += ")" + System.Environment.NewLine;
            }

            return ret;
        }

        public static void ReadDistanceSchema()
        {
            // Get the Schema for distance functions from the DLL
            int schemaLength = GetDistanceSchemaLength();
            StringBuilder sb = new StringBuilder(schemaLength);
            GetDistanceSchema(sb);

            using (XmlReader reader = XmlReader.Create(new StringReader(sb.ToString())))
            {
                while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "DISTANCESCHEMA")
                    {
                        while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                        {
                            if (reader.NodeType == XmlNodeType.Element && (reader.Name == "FLOATFUNC" || reader.Name == "VECFUNC"))
                            {
                                DistFunc newFunc = new DistFunc();
                                newFunc.Read(reader);
                                _DistFunc.Add(newFunc);
                            }
                        }
                    }
                }
            }
        }

        public static void ValidateType(ref string[] lines, int type)
        {
            string token = ParseUtils.GetToken(ref lines);

            for (int i = 0; i < _DistFunc.Count(); i++)
            {
                if (token == _DistFunc[i]._Name)
                {
                    if (_DistFunc[i]._DistParams.Count() > 0)
                    {
                        if (ParseUtils.GetToken(ref lines) != "(")
                            throw new ParseException("Missing open bracket on function " + token);
                        for (int p = 0; p < _DistFunc[i]._DistParams.Count(); p++)
                        {
                            ValidateType(ref lines, _DistFunc[i]._DistParams[p]._Type);
                            if (p<_DistFunc[i]._DistParams.Count()-1)
                                if (ParseUtils.GetToken(ref lines) != ",")
                                    throw new ParseException("Missing comma on function " + token);
                        }
//                        if (type != _DistFunc[i]._ReturnType)
  //                          throw new ParseException("Unexpected type, token : " + token);
                        if (ParseUtils.GetToken(ref lines) != ")")
                            throw new ParseException("Missing close bracket on function " + token);
                    }

                    if (_DistFunc[i]._ReturnType != type)
                        throw new ParseException("Unexpected type, token : " + token);

                    return;
                }
            }

            float rawfloat;
            if (float.TryParse(token, out rawfloat) == true)
            {
                if (!(type == 2 || type == 1))
                    throw new ParseException("Unexpected type, token : " + token);

                return;
            }

            throw new ParseException("Unrecognised token : " + token);
        }

        public static bool ValidateEstimator(string estimator)
        {
            string[] lines = new string[1];
            lines[0] = estimator;

            ValidateType(ref lines, 1);

            return true;
        }

        public Distance(Vector3 centre,
            Vector3 scale,
            Matrix3 rotation,
            string distanceFunction,
            double distanceMinimum,
            double distanceScale,
            Vector3 distanceOffset,
            int distanceIterations)
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
            _DistanceIterations = distanceIterations;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            XElement ret = new XElement("OBJECT",
                new XAttribute("type", 6),
                new XAttribute("distancefunction", _DistanceFunction),
                new XAttribute("minimumdistance", _DistanceMinimum),
                new XAttribute("distancescale", _DistanceScale),
                new XAttribute("distanceoffset", _DistanceOffset),
                new XAttribute("distanceiterations", _DistanceIterations),
                new XAttribute("position", _Position),
                new XAttribute("scale", _Scale),
                new XAttribute("rotation", _Rotation),
                new XAttribute("ignoreWhileLighting", false),
                _Material.CreateElement(preview));
            parent.Add(ret);
        }
    }
}
