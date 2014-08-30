using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class CallStatement : Statement
    {
        public string _RuleName;

        public void Parse(ref string[] program)
        {
            _RuleName = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Call Rule : " + _RuleName);

            string openbracket = ParseUtils.PeekToken(program);
            if (openbracket.Equals("(", StringComparison.Ordinal))
            {
                // arguments not supported for now
                openbracket = ParseUtils.GetToken(ref program);

                string closebracket = ParseUtils.GetToken(ref program);
                if (!closebracket.Equals(")", StringComparison.Ordinal))
                    throw new ParseException("rules not currently supported with arguments, missing ), found " + closebracket + " instead.");
            }
        }

        public void Execute(ref WooState state)
        {
            if (state._Recursions > 0 || !state.GetRule(_RuleName).CanRecurse())
            {
                state._Recursions--;
                state.GetRule(_RuleName).Execute(ref state);
                state._Recursions++;
            }
        }
    }
}
