using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    interface Statement
    {
        void Parse(ref string[] program);
        void Execute(ref WooState state);
    };

    public class NullStatement : Statement
    {
        NullFunction _NullFunction;

        public void Parse(ref string[] program)
        {
            string token = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Found \"" + token + "\" function (null return)");
            WooScript._Log.Indent();
            _NullFunction = WooScript.GetNullFunction(token);
            string openCurl = ParseUtils.GetToken(ref program);
            if (!openCurl.Equals("(", StringComparison.Ordinal))
                new ParseException("Expected \"(\" not \"" + openCurl + "\"");
            _NullFunction.Parse(ref program);
            string closeCurl = ParseUtils.GetToken(ref program);
            if (!closeCurl.Equals(")", StringComparison.Ordinal))
                new ParseException("Expected \")\" not \"" + closeCurl + "\"");
            WooScript._Log.UnIndent();
        }
        public void Execute(ref WooState state)
        {
            _NullFunction.Execute(ref state);
        }
    };

    public class VarStatement : Statement
    {
        string _Var;
        VarType _ReturnType;
        AssignOp _AssignOp;
        Expression _Expression;

        public void Parse(ref string[] program)
        {
            string token = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Found function, target \"" + token + "\"");
            WooScript._Log.Indent();
            if (WooScript.IsFloatVariable(token))
            {
                _ReturnType = VarType.varFloat;
                _Var = token;
            }
            else if (WooScript.IsVecVariable(token))
            {
                _ReturnType = VarType.varVector;
                _Var = token;
            }
            else
            {
                throw new ParseException("Expected \"" + token + "\" to be a float or vector variable");
            }

            string assignOp = ParseUtils.GetToken(ref program);
            _AssignOp = WooScript.GetAssignOp(assignOp);

            _Expression = ExpressionBuilder.Parse(ref program);

            if (_ReturnType == VarType.varVector
                && (_Expression.GetExpressionType() != VarType.varVector))
                throw new ParseException("Target token is \"" + token + "\" which is a vector, expression isn't...");

            if (_ReturnType == VarType.varFloat
                && (_Expression.GetExpressionType() != VarType.varFloat))
                throw new ParseException("Target token is \"" + token + "\" which is a float, expression isn't...");
        }
        public void Execute(ref WooState state)
        {
            if (_ReturnType == VarType.varFloat)
            {
                _AssignOp.ExecuteFloat(ref state, _Var, _Expression);
            }
            else if (_ReturnType == VarType.varVector)
            {
                _AssignOp.ExecuteVector(ref state, _Var, _Expression);
            }
        }
    };
}
