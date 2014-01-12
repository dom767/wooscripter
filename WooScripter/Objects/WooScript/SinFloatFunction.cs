﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class SinFloatFunction : FloatFunction
    {
        Expression _Expr;

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);
            if (!openbrace.Equals("(", StringComparison.Ordinal))
                throw new ParseException("Expected \"(\" at start of function parameters");

            _Expr = ExpressionBuilder.Parse(ref program);
            if (_Expr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("parameter one to sinf() is not a float");

            string closebrace = ParseUtils.GetToken(ref program);
            if (!closebrace.Equals(")", StringComparison.Ordinal))
                throw new ParseException("Expected \")\" at end of function parameters");
        }

        public double EvaluateFloat(ref WooState state)
        {
            return Math.Sin(2 * Math.PI * _Expr.EvaluateFloat(ref state) / 360);
        }

        public string GetSymbol()
        {
            return "sinf";
        }

        public Function CreateNew()
        {
            return new SinFloatFunction();
        }
    }
}
