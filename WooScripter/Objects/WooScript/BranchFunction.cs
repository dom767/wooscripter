using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class BranchFunction : NullFunction
    {
        public List<string> _Rule = new List<string>();
        public List<Expression> _Weight = new List<Expression>();
        public void Parse(ref string[] program)
        {
            while (program[0].IndexOf(',') >= 0)
            {
                string rulename = ParseUtils.GetToken(ref program);
                _Rule.Add(rulename);

                string comma = ParseUtils.GetToken(ref program);
                if (!comma.Equals(",", StringComparison.Ordinal))
                    throw new ParseException("Expected \",\"");

                WooScript._Log.AddMsg("Rule1 : " + rulename);

                Expression expression = ExpressionBuilder.Parse(ref program);
                if (expression.GetExpressionType() != VarType.varFloat)
                    throw new ParseException("Failed to convert weighting parameter to repeat method to float");

                _Weight.Add(expression);
                WooScript._Log.AddMsg("Weight : " + expression);

                comma = ParseUtils.PeekToken(program);
                if (comma.Equals(",", StringComparison.Ordinal))
                    comma = ParseUtils.GetToken(ref program);
            }
        }
        public void Execute(ref WooState state)
        {
            double rand = state._Random.NextDouble();
            double totalWeight = 0;
            foreach (Expression e in _Weight) totalWeight += e.EvaluateFloat(ref state);
            rand *= totalWeight;
            double currentWeight = 0;
            int i = 0;
            while (currentWeight < rand)
            {
                currentWeight += _Weight[i++].EvaluateFloat(ref state);
            }
            if (state._Recursions > 0 || !state.GetRule(_Rule[i - 1]).CanRecurse())
            {
                state._Recursions--;
                WooState newState = state.Clone();
                state.GetRule(_Rule[i - 1]).Execute(ref newState);
                state._Recursions++;
            }
        }

        public string GetSymbol()
        {
            return "branch";
        }

        public Function CreateNew()
        {
            return new BranchFunction();
        }
    }
}
