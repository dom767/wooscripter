using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class ExpressionBuilder
    {
        public static Expression Parse(ref string[] program)
        {
            Expression ret;

            string token1 = ParseUtils.PeekToken(program);
            TokenType type1 = WooScript.GetTokenType(token1);
            if (token1.Equals("(", StringComparison.Ordinal))
            {
                ret = new Brackets();
                ret.Parse(ref program);
            }
            else if (type1 == TokenType.floatNum)
            {
                ret = new FloatNumber();
                ret.Parse(ref program);
            }
            else if (type1 == TokenType.floatVar
                || type1 == TokenType.vecVar)
            {
                ret = new Variable();
                ret.Parse(ref program);
            }
            else if (type1 == TokenType.vecFunction)
            {
                ret = new VecFunctionExpr();
                ret.Parse(ref program);
            }
            else if (type1 == TokenType.floatFunction)
            {
                ret = new FloatFunctionExpr();
                ret.Parse(ref program);
            }
            else
                throw new ParseException("Unrecognised expression \"" + token1 + "\"");
/*            if (type1 == TokenType.floatFunction)
            {
                ret = new FloatFunction();
                ret.Parse(ref program);
            }
*/
            string token2 = ParseUtils.PeekToken(program);
            TokenType type2 = WooScript.GetTokenType(token2);
            if (type2 == TokenType.Op)
            {
                FloatOperation flop = new FloatOperation();
                flop.Parse(ref program);
                flop._Argument1 = ret;
                flop._Argument2 = ExpressionBuilder.Parse(ref program);

                if (flop._Argument1.GetExpressionType() != flop._Argument2.GetExpressionType())
                    throw new ParseException("Mismatch argument types on operation");

                ret = flop;
            }

            return ret;
        }
    };

    public interface Expression
    {
        void Parse(ref string[] program);
        VarType GetExpressionType();
        double EvaluateFloat(ref WooState state);
        Vector3 EvaluateVector(ref WooState state);
    }

    public class Brackets : Expression
    {
        Expression _Expression;

        public void Parse(ref string[] program)
        {
            string token1 = ParseUtils.GetToken(ref program);
            if (!token1.Equals("(", StringComparison.Ordinal))
                throw new ParseException("Expected \"(\" in Brackets");

            _Expression = ExpressionBuilder.Parse(ref program);
            
            string token2 = ParseUtils.GetToken(ref program);
            if (!token2.Equals(")", StringComparison.Ordinal))
                throw new ParseException("Expected \")\" in Brackets");
        }

        public VarType GetExpressionType()
        {
            return _Expression.GetExpressionType();
        }

        public double EvaluateFloat(ref WooState state)
        {
            return _Expression.EvaluateFloat(ref state);
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            return _Expression.EvaluateVector(ref state);
        }
    };

    public class FloatOperation : Expression
    {
        public Expression _Argument1;
        public Expression _Argument2;
        VarType _Type;
        public Op _Op;

        public void Parse(ref string[] program)
        {
            string op = ParseUtils.GetToken(ref program);
            _Op = WooScript.GetOp(op);
        }

        public VarType GetExpressionType()
        {
            return _Argument1.GetExpressionType();
        }

        public double EvaluateFloat(ref WooState state)
        {
            return _Op.EvaluateFloat(_Argument1.EvaluateFloat(ref state), _Argument2.EvaluateFloat(ref state));
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            return _Op.EvaluateVector(_Argument1.EvaluateVector(ref state), _Argument2.EvaluateVector(ref state));
        }
    };

    public class Variable : Expression
    {
        string _Variable;
        VarType _Type;
        public void Parse(ref string[] program)
        {
            _Variable = ParseUtils.GetToken(ref program);
            TokenType tokenType = WooScript.GetTokenType(_Variable);
            if (tokenType == TokenType.floatVar)
            {
                _Type = VarType.varFloat;
            }
            else if (tokenType == TokenType.vecVar)
            {
                _Type = VarType.varVector;
            }
            else
                throw new ParseException("Unknown type for variable \"" + _Variable + "\"");
        }

        public VarType GetExpressionType()
        {
            return _Type;
        }

        public double EvaluateFloat(ref WooState state)
        {
            if (_Type== VarType.varFloat)
                return state.GetValueFloat(_Variable);
            else
                throw new EvaluateException("Vector variables can't evaluate to floats.\n");
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            if (_Type == VarType.varVector)
                return state.GetValueVector(_Variable);
            else
                throw new EvaluateException("Float variables can't evaluate to vectors.\n");
        }
    }

    public class FloatNumber : Expression
    {
        enum RangeTypeT
        {
            Continuous, Binary, Single
        }

        RangeTypeT _RangeType;
        float val1, val2;

        public VarType GetExpressionType()
        {
            return VarType.varFloat;
        }

        public void Parse(ref string[] program)
        {
            string data = ParseUtils.GetToken(ref program);
            if (data.IndexOf(':') > 0)
            {
                _RangeType = RangeTypeT.Continuous;
                WooScript._Log.AddMsg("Number type : Continuous range");
                int opPos = data.IndexOf(':');
                val1 = float.Parse(data.Substring(0, opPos));
                val2 = float.Parse(data.Substring(opPos + 1));
                WooScript._Log.AddMsg("Val1 : " + val1.ToString());
                WooScript._Log.AddMsg("Val2 : " + val2.ToString());
            }
            else if (data.IndexOf('|') > 0)
            {
                _RangeType = RangeTypeT.Binary;
                WooScript._Log.AddMsg("Number type : Binary Option");
                int opPos = data.IndexOf('|');
                val1 = float.Parse(data.Substring(0, opPos));
                val2 = float.Parse(data.Substring(opPos + 1));
                WooScript._Log.AddMsg("Val1 : " + val1.ToString());
                WooScript._Log.AddMsg("Val2 : " + val2.ToString());
            }
            else
            {
                _RangeType = RangeTypeT.Single;
                WooScript._Log.AddMsg("Number type : Single Value");
                val1 = float.Parse(data);
                WooScript._Log.AddMsg("Val1 : " + val1.ToString());
            }
        }

        public double EvaluateFloat(ref WooState state)
        {
            double output = 0;
            double randVal = state._Random.NextDouble();

            switch (_RangeType)
            {
                case RangeTypeT.Single:
                    output = val1;
                    break;
                case RangeTypeT.Binary:
                    if (randVal > 0.5)
                        output = val1;
                    else
                        output = val2;
                    break;
                case RangeTypeT.Continuous:
                    output = ((val2 - val1) * randVal) + val1;
                    break;
            }

            return output;
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            throw new EvaluateException("Float numbers can't evaluate to vectors.\n");
        }
    }

    public class VecFunctionExpr : Expression
    {
        public VecFunction _Function;

        public void Parse(ref string[] program)
        {
            string vecFunctionName = ParseUtils.GetToken(ref program);
            _Function = WooScript.GetVecFunction(vecFunctionName);
            _Function.Parse(ref program);
        }

        public VarType GetExpressionType()
        {
            return VarType.varVector;
        }

        public double EvaluateFloat(ref WooState state)
        {
            throw new EvaluateException("Vec function can't evaluate to a float");
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            return _Function.EvaluateVector(ref state);
        }
    };

    public class FloatFunctionExpr : Expression
    {
        public FloatFunction _Function;

        public void Parse(ref string[] program)
        {
            string floatFunctionName = ParseUtils.GetToken(ref program);
            _Function = WooScript.GetFloatFunction(floatFunctionName);
            _Function.Parse(ref program);
        }

        public VarType GetExpressionType()
        {
            return VarType.varFloat;
        }

        public double EvaluateFloat(ref WooState state)
        {
            return _Function.EvaluateFloat(ref state);
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            throw new EvaluateException("Float function can't evaluate to a vector"); 
        }
    };
}
