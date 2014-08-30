using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class ScopeStatement : Statement
    {
        public RuleBlock _RuleBlock;

        public void Parse(ref string[] program)
        {
            _RuleBlock = new RuleBlock();
            _RuleBlock.Parse(ref program);
        }

        public void Execute(ref WooState state)
        {
            WooState newState = state.Clone();
            _RuleBlock.Execute(ref newState);
        }
    }
}
