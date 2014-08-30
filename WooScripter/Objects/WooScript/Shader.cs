using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml;

namespace WooScripter.Objects.WooScript
{
    public class ScriptElement
    {
        public SEType _Type;
        public string _String;
        public List<ScriptElement> _Arguments = new List<ScriptElement>();

        public string Evaluate(WooState state)
        {
            string ret;
            ret = _String;
            if (_Arguments.Count() > 0)
            {
                ret += "(";
                for (int i =0; i<_Arguments.Count(); i++)
                {
                    ret += _Arguments[i].Evaluate(state);
                    if (i<_Arguments.Count()-1) ret += ",";
                }
                ret += ")";
            }
            return ret;
        }
    };

    public enum SEType
    {
        FloatVar,
        VectorVar,
        Num,
        Func
    };

    public class ShaderScript
    {
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

        public class FuncVar
        {
            public void Read(XmlReader reader)
            {
                if (reader.Name == "FLOATVAR") _VarType = 1;
                else _VarType = 0;

                reader.MoveToNextAttribute();
                if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "name")
                    _Name = reader.Value;
            }
            public int _VarType;
            public string _Name;
        };

        public static bool IsFloatVar(string token)
        {
            foreach (FuncVar fv in _FuncVar)
            {
                if (fv._Name.Equals(token, StringComparison.Ordinal)
                    && fv._VarType == 1)
                    return true;
            }
            return false;
        }

        public static bool IsVectorVar(string token)
        {
            foreach (FuncVar fv in _FuncVar)
            {
                if (fv._Name.Equals(token, StringComparison.Ordinal)
                    && fv._VarType == 0)
                    return true;
            }
            return false;
        }

        public static bool IsFloatNum(string token)
        {
            double val;
            return double.TryParse(token, out val);
        }

        public static bool IsFunction(string token)
        {
            foreach (DistFunc df in _DistFunc)
            {
                if (df._Name.Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAssignOp(string token)
        {
            if (token.Equals("=", StringComparison.Ordinal))
                return true;
            if (token.Equals("*=", StringComparison.Ordinal))
                return true;
            if (token.Equals("/=", StringComparison.Ordinal))
                return true;
            if (token.Equals("-=", StringComparison.Ordinal))
                return true;
            if (token.Equals("+=", StringComparison.Ordinal))
                return true;
            return false;
        }

        public static List<DistFunc> _DistFunc = new List<DistFunc>();
        public static List<FuncVar> _FuncVar = new List<FuncVar>();

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

            ret += System.Environment.NewLine + "Shader Variables : " + System.Environment.NewLine;

            for (int i = 0; i < _FuncVar.Count(); i++)
            {
                ret += _FuncVar[i]._Name;
                ret += " : ";
                ret += _FuncVar[i]._VarType == 1 ? "float" : "vector";
                ret += System.Environment.NewLine;
            }

            return ret;
        }

        public static ScriptElement ParseFunction(ref string[] program)
        {
            ScriptElement ret = new ScriptElement();
            string token = ParseUtils.GetToken(ref program);
            if (token.Equals("-", StringComparison.Ordinal))
            {
                token += ParseUtils.GetToken(ref program);
            } 
            
            if (IsFloatVar(token))
            {
                ret._Type = SEType.FloatVar;
                ret._String = token;
            }
            else if (IsVectorVar(token))
            {
                ret._Type = SEType.VectorVar;
                ret._String = token;
            }
            else if (IsFloatNum(token))
            {
                ret._Type = SEType.Num;
                ret._String = token;
            }
            else if (IsFunction(token))
            {
                ret._Type = SEType.Func;
                ret._String = token;

                string openBracket = ParseUtils.PeekToken(program);
                if (openBracket.Equals("(", StringComparison.Ordinal))
                {
                    openBracket = ParseUtils.GetToken(ref program);

                    ret._Arguments.Add(ParseFunction(ref program));

                    string comma = ParseUtils.PeekToken(program);
                    while (comma.Equals(",", StringComparison.Ordinal))
                    {
                        comma = ParseUtils.GetToken(ref program);
                        ret._Arguments.Add(ParseFunction(ref program));
                        comma = ParseUtils.PeekToken(program);
                    }

                    string closeBracket = ParseUtils.GetToken(ref program);
                    if (!closeBracket.Equals(")", StringComparison.Ordinal))
                        throw new ParseException("Expected close bracket, found \"" + token + "\"");
                }
            }
            else
            {
                throw new ParseException("Unrecognised token found \"" + token + "\"");
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
                            if (reader.NodeType == XmlNodeType.Element && (reader.Name == "FLOATVAR" || reader.Name == "VECVAR"))
                            {
                                FuncVar newVar = new FuncVar();
                                newVar.Read(reader);
                                _FuncVar.Add(newVar);
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
                            if (p < _DistFunc[i]._DistParams.Count() - 1)
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

            if (token == "-")
                token = "-" + ParseUtils.GetToken(ref lines);
            float rawfloat;
            if (float.TryParse(token, out rawfloat) == true)
            {
                if (!(type == 2 || type == 1))
                    throw new ParseException("Unexpected type, token : " + token);

                return;
            }

            throw new ParseException("Unrecognised token : " + token);
        }

        public static bool ValidateEstimator(string estimator, int type)
        {
            string[] lines = new string[1];
            lines[0] = estimator;

            ValidateType(ref lines, type);

            return true;
        }
    };

    public class ShaderStatement
    {
        string _Target;
        string _AssignOp;
        ScriptElement _Argument;

        public void Parse(ref string[] program)
        {
            _Target = ParseUtils.GetToken(ref program);

            if (!ShaderScript.IsFloatVar(_Target) && !ShaderScript.IsVectorVar(_Target))
                throw new ParseException("Expected \"" + _Target + "\" to be a float or vector variable");
            WooScript._Log.AddMsg("Found target variable \"" + _Target + "\"");
            WooScript._Log.Indent();

            _AssignOp = ParseUtils.GetToken(ref program);

            if (!ShaderScript.IsAssignOp(_AssignOp))
                throw new ParseException("Expected \"" + _AssignOp + "\" to be an assignment operation");

            _Argument = ShaderScript.ParseFunction(ref program);
        }

        public string Evaluate(WooState state)
        {
            if (_AssignOp.Equals("=", StringComparison.Ordinal))
                return "set(" + _Target + ", " + _Argument.Evaluate(state) + ")";
            if (_AssignOp.Equals("*=", StringComparison.Ordinal))
                return "set(" + _Target + ", mul(" + _Target + ", " + _Argument.Evaluate(state) + "))";
            if (_AssignOp.Equals("/=", StringComparison.Ordinal))
                return "set(" + _Target + ", div(" + _Target + ", " + _Argument.Evaluate(state) + "))";
            if (_AssignOp.Equals("-=", StringComparison.Ordinal))
                return "set(" + _Target + ", sub(" + _Target + ", " + _Argument.Evaluate(state) + "))";
            if (_AssignOp.Equals("+=", StringComparison.Ordinal))
                return "set(" + _Target + ", add(" + _Target + ", " + _Argument.Evaluate(state) + "))";

            throw new EvaluateException("Failed to evaluate shader statement");
        }
    }

    public class Shader
    {
        public string _Name;
        List<ShaderStatement> _Statements = new List<ShaderStatement>();

        public Shader(string name)
        {
            _Name = name;
        }

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);

            string nexttoken = ParseUtils.PeekToken(program);

            while (nexttoken != "}")
            {
                ShaderStatement shaderStatement = new ShaderStatement();
                shaderStatement.Parse(ref program);
                _Statements.Add(shaderStatement);

                nexttoken = ParseUtils.PeekToken(program);
            }

            string closebrace = ParseUtils.GetToken(ref program);
        }

        public string Evaluate(WooState state)
        {
            string ret = "";
            foreach (ShaderStatement statement in _Statements)
            {
                ret += statement.Evaluate(state); 
            }
            return ret;
        }
    }
}
