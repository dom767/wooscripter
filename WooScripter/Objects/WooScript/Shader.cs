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
        public bool _Reorderable;
        public CodeBlock _Codeblock;

        public string Evaluate(WooState state)
        {
            string ret;

            if (_Codeblock != null)
            {
                ret = "{";
                ret += _Codeblock.Evaluate(state);
                ret += "}";
                return ret;
            }

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
        CodeBlock,
        Num,
        Null
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
            public SEType _Type;
        }
        public class DistFunc
        {
            public void Read(XmlReader reader)
            {
                if (reader.Name == "FLOATFUNC") _ReturnType = SEType.FloatVar;
                else if (reader.Name == "VECFUNC") _ReturnType = SEType.VectorVar;
                else _ReturnType = SEType.Null;

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
                                    param._Type = SEType.VectorVar;
                                    break;
                                case "float":
                                    param._Type = SEType.FloatVar;
                                    break;
                            }
                        }

                        _DistParams.Add(param);
                    }
                }

            }
            public SEType _ReturnType;
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

        public static bool IsNullFunction(string token)
        {
            foreach (DistFunc df in _DistFunc)
            {
                if (df._Name.Equals(token, StringComparison.Ordinal)
                    && df._ReturnType == SEType.Null)
                {
                    return true;
                }
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

        public static DistFunc GetFunction(string token)
        {
            foreach (DistFunc df in _DistFunc)
            {
                if (df._Name.Equals(token, StringComparison.Ordinal))
                {
                    return df;
                }
            }
            throw new ParseException("Missing distance function looking up \"" + token + "\"");
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

        public static bool IsOperator(string token)
        {
            if (token.Equals("+", StringComparison.Ordinal))
                return true;
            if (token.Equals("-", StringComparison.Ordinal))
                return true;
            if (token.Equals("/", StringComparison.Ordinal))
                return true;
            if (token.Equals("*", StringComparison.Ordinal))
                return true;
            if (token.Equals("%", StringComparison.Ordinal))
                return true;
            return false;
        }

        public static int GetPrecedence(string token)
        {
            if (token.Equals("add", StringComparison.Ordinal))
                return 5;
            if (token.Equals("sub", StringComparison.Ordinal))
                return 4;
            if (token.Equals("div", StringComparison.Ordinal))
                return 2;
            if (token.Equals("mul", StringComparison.Ordinal))
                return 3;
            if (token.Equals("mod", StringComparison.Ordinal))
                return 1;
            return -1;
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

        public static void ValidateFunction(ScriptElement func)
        {
            bool foundFunction = false;
            bool validNumParams = false;
            bool validTypeParams = false;
            foreach (DistFunc df in _DistFunc)
            {
                if (df._Name.Equals(func._String, StringComparison.Ordinal))
                {
                    foundFunction = true;
                    if (df._DistParams.Count == func._Arguments.Count)
                    {
                        validNumParams = true;
                        bool functionParamsValid = true;
                        for (int i = 0; i < df._DistParams.Count; i++)
                        {
                            if (df._DistParams.ElementAt(i)._Type != func._Arguments.ElementAt(i)._Type)
                                functionParamsValid = false;
                        }
                        if (functionParamsValid)
                        {
                            validTypeParams = true;
                            func._Type = df._ReturnType;
                        }
                    }
                }
            }
            if (!foundFunction)
                throw new ParseException("Missing distance function looking up \"" + func._String + "\"");
            if (!validNumParams)
                throw new ParseException("No matching version of function \"" + func._String + "\" found expecting " + func._Arguments.Count + " parameters");
            if (!validTypeParams)
                throw new ParseException("Type mismatch on args for function \"" + func._String + "\"");
        }

        public static ScriptElement ParseExpression(ref string[] program)
        {
            ScriptElement ret = new ScriptElement();
            string token = ParseUtils.PeekToken(program);

            ret._Reorderable = false;

            if (token.Equals("-", StringComparison.Ordinal))
            {
                token = ParseUtils.GetToken(ref program);
                token += ParseUtils.PeekToken(program);
            }
            
            if (token.Equals("(", StringComparison.Ordinal))
            {
                token = ParseUtils.GetToken(ref program);
                ret = ParseExpression(ref program);
                ret._Reorderable = false;

                token = ParseUtils.GetToken(ref program);
                if (!token.Equals(")", StringComparison.Ordinal))
                    throw new ParseException("Missing closebracket on bracketed expression, found" + token);
            }
            else if (IsFloatVar(token))
            {
                token = ParseUtils.GetToken(ref program);
                ret._Type = SEType.FloatVar;
                ret._String = token;
            }
            else if (IsVectorVar(token))
            {
                token = ParseUtils.GetToken(ref program);
                ret._Type = SEType.VectorVar;
                ret._String = token;
            }
            else if (IsFloatNum(token))
            {
                string chuck = ParseUtils.GetToken(ref program);
                ret._Type = SEType.FloatVar;
                ret._String = token;
            }
            else if (token.Equals("{"))
            {
                ret._Type = SEType.CodeBlock;
                ret._Codeblock = new CodeBlock();
                ret._Codeblock.Parse(ref program);
            }
            else if (token.IndexOf('.') > -1)
            {
                token = ParseUtils.GetToken(ref program);
                int dotPosition = token.IndexOf('.');

                ret._Type = SEType.VectorVar;
                ret._String = token.Substring(0, dotPosition);

                ScriptElement getter = new ScriptElement();

                string index = token.Substring(dotPosition + 1);

                if (index.Equals("x", StringComparison.Ordinal))
                    getter._String = "getx";
                else if (index.Equals("y", StringComparison.Ordinal))
                    getter._String = "gety";
                else if (index.Equals("z", StringComparison.Ordinal))
                    getter._String = "getz";
                else
                    throw new ParseException("Invalid subindex " + index);

                getter._Arguments.Add(ret);
                getter._Type = SEType.FloatVar;
                ret = getter;
            }
            else if (token.Equals("repeat", StringComparison.Ordinal))
            {
                token = ParseUtils.GetToken(ref program);
                ret._String = token;

                string openBracket = ParseUtils.PeekToken(program);
                if (openBracket.Equals("(", StringComparison.Ordinal))
                {
                    openBracket = ParseUtils.GetToken(ref program);

                    ret._Arguments.Add(ParseExpression(ref program));

                    string closeBracket = ParseUtils.GetToken(ref program);
                    if (!closeBracket.Equals(")", StringComparison.Ordinal))
                        throw new ParseException("Expected close bracket, found \"" + token + "\"");
                }
                else
                {
                    throw new ParseException("repeat not followed by number, found \"" + token + "\", usage : repeat(x){}");
                }

                ret._Arguments.Add(ParseExpression(ref program));
            }
            else if (IsFunction(token))
            {
                token = ParseUtils.GetToken(ref program);
                ret._String = token;

                string openBracket = ParseUtils.PeekToken(program);
                if (openBracket.Equals("(", StringComparison.Ordinal))
                {
                    openBracket = ParseUtils.GetToken(ref program);

                    ret._Arguments.Add(ParseExpression(ref program));

                    string comma = ParseUtils.PeekToken(program);
                    while (comma.Equals(",", StringComparison.Ordinal))
                    {
                        comma = ParseUtils.GetToken(ref program);
                        ret._Arguments.Add(ParseExpression(ref program));
                        comma = ParseUtils.PeekToken(program);
                    }

                    string closeBracket = ParseUtils.GetToken(ref program);
                    if (!closeBracket.Equals(")", StringComparison.Ordinal))
                        throw new ParseException("Expected close bracket, found \"" + token + "\"");
                }

                ValidateFunction(ret);
            }
            else
            {
                throw new ParseException("Unrecognised token found \"" + token + "\"");
            }

            string opCode = ParseUtils.PeekToken(program);
            if (IsOperator(opCode))
            {
                opCode = ParseUtils.GetToken(ref program);
                ScriptElement opElement = new ScriptElement();

                if (opCode.Equals("+", StringComparison.Ordinal))
                    opElement._String = "add";
                if (opCode.Equals("-", StringComparison.Ordinal))
                    opElement._String = "sub";
                if (opCode.Equals("*", StringComparison.Ordinal))
                    opElement._String = "mul";
                if (opCode.Equals("/", StringComparison.Ordinal))
                    opElement._String = "div";
                if (opCode.Equals("%", StringComparison.Ordinal))
                    opElement._String = "mod";

                opElement._Type = ret._Type;
                opElement._Arguments.Add(ret);

                ScriptElement arg2 = ParseExpression(ref program);
                opElement._Arguments.Add(arg2);

                if (arg2._Reorderable)
                {
                    if (GetPrecedence(opElement._String) < GetPrecedence(arg2._String))
                    {
                        ScriptElement Arg1Arg1 = ret;
                        ScriptElement Arg2Arg1 = arg2._Arguments.ElementAt(0);
                        ScriptElement Arg2Arg2 = arg2._Arguments.ElementAt(1);

                        arg2._Arguments.Clear();
                        arg2._Arguments.Add(opElement);
                        arg2._Arguments.Add(Arg2Arg2);
                        opElement._Arguments.Clear();
                        opElement._Arguments.Add(Arg1Arg1);
                        opElement._Arguments.Add(Arg2Arg1);
                            
                        opElement = arg2;
                    }
                }

                ret = opElement;
                ret._Reorderable = true;

                ValidateFunction(opElement);
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
                            if (reader.NodeType == XmlNodeType.Element && (reader.Name == "FLOATFUNC" || reader.Name == "VECFUNC" || reader.Name == "NULLFUNC"))
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
        /*
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
        }*/
/*
        public static bool ValidateEstimator(string estimator, int type)
        {
            string[] lines = new string[1];
            lines[0] = estimator;

            ValidateType(ref lines, type);

            return true;
        }*/
    };

    public class ShaderStatement
    {
        string _Target;
        string _AssignOp;
        ScriptElement _Argument;
        bool _IsNull = true;

        public void Parse(ref string[] program)
        {
            _Target = ParseUtils.PeekToken(program);
            bool isFloatVar = ShaderScript.IsFloatVar(_Target);
            bool isVectorVar = ShaderScript.IsVectorVar(_Target);
            bool isNullFunc = ShaderScript.IsNullFunction(_Target);

            if (!isFloatVar && !isVectorVar && !isNullFunc)
                throw new ParseException("Expected \"" + _Target + "\" to be a float or vector variable or a non-return function");

            if (isFloatVar || isVectorVar)
            {
                _Target = ParseUtils.GetToken(ref program);
                _IsNull = false;
                WooScript._Log.AddMsg("Found target variable \"" + _Target + "\"");
                WooScript._Log.Indent();

                _AssignOp = ParseUtils.GetToken(ref program);

                if (!ShaderScript.IsAssignOp(_AssignOp))
                    throw new ParseException("Expected \"" + _AssignOp + "\" to be an assignment operation");
            }

            _Argument = ShaderScript.ParseExpression(ref program);
        }

        public string Evaluate(WooState state)
        {
            if (_IsNull)
                return _Argument.Evaluate(state);
            else
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
            }

            throw new EvaluateException("Failed to evaluate shader statement");
        }
    }

    public class CodeBlock
    {
        List<ShaderStatement> _Statements = new List<ShaderStatement>();

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);
            if (!openbrace.Equals("{"))
                throw new ParseException("Found \""+openbrace+"\", but expected \"{\".");

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

    public class Shader
    {
        public string _Name;
        CodeBlock _CodeBlock = new CodeBlock();

        public Shader(string name)
        {
            _Name = name;
        }

        public void Parse(ref string[] program)
        {
            _CodeBlock.Parse(ref program);
        }

        public string Evaluate(WooState state)
        {
            return _CodeBlock.Evaluate(state);
        }
    }
}
