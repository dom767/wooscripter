using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class RepeatFunction : NullFunction
    {
        public string _Callee;
        public Expression _Expr;
        public void Parse(ref string[] program)
        {
            _Callee = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Callee : " + _Callee);

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Expr = ExpressionBuilder.Parse(ref program);
            if (_Expr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("Failed to convert repeat number to float");

            WooScript._Log.AddMsg("Repeats : " + _Expr.ToString());
        }

        public void Execute(ref WooState state)
        {
            //WooState newState = state.Clone();
            if (state._Recursions > 0 || !state.GetRule(_Callee).CanRecurse())
            {
                state._Recursions--;
                for (int i = 0; i < (int)(_Expr.EvaluateFloat(ref state)+0.5); i++)
                {
                    state.GetRule(_Callee).Execute(ref state);
                }
                state._Recursions++;
            }
        }

        public string GetSymbol()
        {
            return "repeat";
        }

        public Function CreateNew()
        {
            return new RepeatFunction();
        }
    }
}
