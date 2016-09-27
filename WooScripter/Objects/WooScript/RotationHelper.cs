using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class ResetRotation : NullFunction
    {
        public void Parse(ref string[] program)
        {
        }

        public void Execute(ref WooState state)
        {
            state._Rotation.MakeIdentity();
        }

        public string GetSymbol()
        {
            return "resetrotation";
        }

        public Function CreateNew()
        {
            return new ResetRotation();
        }
    }

    class PointAt : NullFunction
    {
        Expression _At;
        Expression _Up;

        public void Parse(ref string[] program)
        {
            _At = ExpressionBuilder.Parse(ref program);
            if (_At.GetExpressionType() != VarType.varVector)
                throw new ParseException("_At must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Up = ExpressionBuilder.Parse(ref program);
            if (_Up.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Up must be of type vector");
        }

        public void Execute(ref WooState state)
        {
            state._Rotation.PointAt(state._Position, _At.EvaluateVector(ref state), _Up.EvaluateVector(ref state));
        }

        public string GetSymbol()
        {
            return "pointat";
        }

        public Function CreateNew()
        {
            return new PointAt();
        }
    }

}
