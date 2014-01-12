using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class FinalCallFunction : NullFunction
    {
        public string _Callee;
        public void Parse(ref string[] program)
        {
            _Callee = ParseUtils.GetToken(ref program);
            WooScript._Log.AddMsg("Callee : " + _Callee);
        }
        
        public void Execute(ref WooState state)
        {
            if (state._Recursions == 0)
            {
                state._Recursions--;
                WooState newState = state.Clone();
                state.GetRule(_Callee).Execute(ref newState);
                state._Recursions++;
            }
        }

        public string GetSymbol()
        {
            return "finalcall";
        }

        public Function CreateNew()
        {
            return new FinalCallFunction();
        }
    }
}
