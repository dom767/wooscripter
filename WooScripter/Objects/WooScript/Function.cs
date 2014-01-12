using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public interface Function
    {
        void Parse(ref string[] program);
        string GetSymbol();
        Function CreateNew();
    }

    public interface NullFunction : Function
    {
        void Execute(ref WooState state);
    }

    public interface VecFunction : Function
    {
        Vector3 EvaluateVector(ref WooState state);
    }

    public interface FloatFunction : Function
    {
        double EvaluateFloat(ref WooState state);
    }

    public class VectorConstructorFunction : VecFunction
    {
        Expression _XExpr;
        Expression _YExpr;
        Expression _ZExpr;

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);
            if (!openbrace.Equals("(", StringComparison.Ordinal))
                throw new ParseException("Expected \"(\" at start of function parameters");

            _XExpr = ExpressionBuilder.Parse(ref program);
            if (_XExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("parameter one to vec() is not a float");

            _YExpr = ExpressionBuilder.Parse(ref program);
            if (_YExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("parameter two to vec() is not a float");

            _ZExpr = ExpressionBuilder.Parse(ref program);
            if (_ZExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("parameter three to vec() is not a float");

            string closebrace = ParseUtils.GetToken(ref program);
            if (!closebrace.Equals(")", StringComparison.Ordinal))
                throw new ParseException("Expected \")\" at end of function parameters");
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            return new Vector3(_XExpr.EvaluateFloat(ref state),
                _YExpr.EvaluateFloat(ref state),
                _ZExpr.EvaluateFloat(ref state));
        }

        public string GetSymbol()
        {
            return "vec";
        }

        public Function CreateNew()
        {
            return new VectorConstructorFunction();
        }
    }

    public class VectorNormaliseFunction : VecFunction
    {
        Expression _Arg;

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);
            if (!openbrace.Equals("(", StringComparison.Ordinal))
                throw new ParseException("Expected \"(\" at start of function parameters");

            _Arg = ExpressionBuilder.Parse(ref program);
            if (_Arg.GetExpressionType() != VarType.varVector)
                throw new ParseException("parameter to normalise() is not a vector");
            
            string closebrace = ParseUtils.GetToken(ref program);
            if (!closebrace.Equals(")", StringComparison.Ordinal))
                throw new ParseException("Expected \")\" at end of function parameters");
        }

        public Vector3 EvaluateVector(ref WooState state)
        {
            Vector3 arg = _Arg.EvaluateVector(ref state);
            arg.Normalise();
            return arg;
        }

        public string GetSymbol()
        {
            return "normalise";
        }

        public Function CreateNew()
        {
            return new VectorNormaliseFunction();
        }
    }
}
