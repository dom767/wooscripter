using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class CallFunction : NullFunction
    {
        public string _Callee;
        public void Parse(ref string[] program)
        {
            _Callee = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Callee : " + _Callee);
        }

        public void Execute(ref WooState state)
        {
            if (state._Recursions > 0 || !state.GetRule(_Callee).CanRecurse())
            {
                state._Recursions--;
//                WooState newState = state.Clone();
                state.GetRule(_Callee).Execute(ref state);
                state._Recursions++;
            }
        }

        public string GetSymbol()
        {
            return "call";
        }

        public Function CreateNew()
        {
            return new CallFunction();
        }
    }
}
