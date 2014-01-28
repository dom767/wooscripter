using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class BranchFunction : NullFunction
    {
        public List<string> _Rule = new List<string>();
        public List<float> _Weight = new List<float>();
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
                string weightstr = ParseUtils.GetToken(ref program);
                float weight;
                try
                {
                    weight = float.Parse(weightstr);
                }
                catch (FormatException /*e*/)
                {
                    throw new ParseException("Failed to convert second parameter to Repeat method : " + weightstr);
                }
                _Weight.Add(weight);
                WooScript._Log.AddMsg("Weight : " + weight);

                comma = ParseUtils.GetToken(ref program);
                if (!comma.Equals(",", StringComparison.Ordinal))
                    throw new ParseException("Expected \",\"");
            }
        }
        public void Execute(ref WooState state)
        {
            double rand = state._Random.NextDouble();
            double totalWeight = 0;
            foreach (float f in _Weight) totalWeight += f;
            rand *= totalWeight;
            double currentWeight = 0;
            int i = 0;
            while (currentWeight < rand)
            {
                currentWeight += _Weight[i++];
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
