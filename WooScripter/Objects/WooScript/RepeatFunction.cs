using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class RepeatFunction : NullFunction
    {
        public string _Callee;
        public int _Repeats;
        public void Parse(ref string[] program)
        {
            _Callee = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Callee : " + _Callee);

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            string token = ParseUtils.GetToken(ref program);
            try
            {
                _Repeats = int.Parse(token);
            }
            catch (FormatException /*e*/)
            {
                throw new ParseException("Failed to convert second parameter to Repeat method : " + token);
            }
            WooScript._Log.AddMsg("Repeats : " + _Repeats.ToString());
        }

        public void Execute(ref WooState state)
        {
            //WooState newState = state.Clone();
            if (state._Recursions > 0 || !state.GetRule(_Callee).CanRecurse())
            {
                state._Recursions--;
                for (int i = 0; i < _Repeats; i++)
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
