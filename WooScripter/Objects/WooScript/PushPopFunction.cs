using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class PushFunction : NullFunction
    {
        public void Parse(ref string[] program)
        {
        }
        
        public void Execute(ref WooState state)
        {
            state._PreviousState = state.Clone();
        }

        public string GetSymbol()
        {
            return "push";
        }

        public Function CreateNew()
        {
            return new PushFunction();
        }
    }

    class PopFunction : NullFunction
    {
        public void Parse(ref string[] program)
        {
        }
        
        public void Execute(ref WooState state)
        {
            if (state._PreviousState != null)
                state = state._PreviousState;
        }

        public string GetSymbol()
        {
            return "pop";
        }

        public Function CreateNew()
        {
            return new PopFunction();
        }
    }
}
