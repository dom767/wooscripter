using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class CallStatement : Statement
    {
        public RulePrototype _RulePrototype;
        public List<Expression> _ArgValue = new List<Expression>();

        public void Parse(ref string[] program)
        {
            string ruleName = ParseUtils.GetToken(ref program);
            _RulePrototype = WooScript.GetRulePrototype(ruleName);
            WooScript._Log.AddMsg("Call Rule : " + ruleName);

            string openbracket = ParseUtils.PeekToken(program);
            if (openbracket.Equals("(", StringComparison.Ordinal))
            {
                openbracket = ParseUtils.GetToken(ref program);
                
                for (int i=0; i<_RulePrototype._Args.Count(); i++)
                {
                    _ArgValue.Add(ExpressionBuilder.Parse(ref program));
                    if (i+1<_RulePrototype._Args.Count())
                    {
                        string comma = ParseUtils.GetToken(ref program);
                        if (!comma.Equals(",", StringComparison.Ordinal))
                            throw new ParseException("Found "+comma+" but expected another argument after a \",\"");
                    }
                }

                string closebracket = ParseUtils.GetToken(ref program);
                if (!closebracket.Equals(")", StringComparison.Ordinal))
                    throw new ParseException("rules not currently supported with arguments, missing ), found " + closebracket + " instead.");
            }
        }

        public void Execute(ref WooState state)
        {
            if (state._Recursions > 0 || !state.GetRule(_RulePrototype._Name).CanRecurse())
            {
                state._Recursions--;
                for (int i=0; i<_ArgValue.Count(); i++)
                {
                    if (_RulePrototype._Args[i]._Type == VarType.varVector)
                        state.AddVector(_RulePrototype._Args[i]._Name, _ArgValue[i].EvaluateVector(ref state));
                    else if (_RulePrototype._Args[i]._Type == VarType.varFloat)
                        state.AddFloat(_RulePrototype._Args[i]._Name, _ArgValue[i].EvaluateFloat(ref state));
                }
                state.GetRule(_RulePrototype._Name).Execute(ref state);
                for (int i = 0; i < _ArgValue.Count(); i++)
                {
                    if (_RulePrototype._Args[i]._Type == VarType.varVector)
                        state.RemoveVector(_RulePrototype._Args[i]._Name);
                    else if (_RulePrototype._Args[i]._Type == VarType.varFloat)
                        state.RemoveFloat(_RulePrototype._Args[i]._Name);
                }
                state._Recursions++;
            }
        }
    }
}
