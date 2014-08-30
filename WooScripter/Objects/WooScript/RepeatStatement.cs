using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class RepeatStatement : Statement
    {
        public Expression _Expression;
        public RuleBlock _RepeatBlock;

        public void Parse(ref string[] program)
        {
            string repeatstring = ParseUtils.GetToken(ref program);
            if (!repeatstring.Equals("repeat", StringComparison.Ordinal))
                throw new ParseException("repeat Statement should always start with repeat, found " + repeatstring + " instead.");
            
            string openbracket = ParseUtils.GetToken(ref program);
            if (!openbracket.Equals("(", StringComparison.Ordinal))
                throw new ParseException("if statement must be followed by a condition in brackets, found " + openbracket + " instead.");
            
            _Expression = ExpressionBuilder.Parse(ref program);
            if (_Expression.GetExpressionType() != VarType.varFloat)
                throw new ParseException("repeat statement requires a float parameter");
            
            string closebracket = ParseUtils.GetToken(ref program);            
            if (!closebracket.Equals(")", StringComparison.Ordinal))
                throw new ParseException("if statement found a conditional expression without a closing bracket, found " + closebracket + " instead.");

            _RepeatBlock = new RuleBlock();
            _RepeatBlock.Parse(ref program);
        }

        public void Execute(ref WooState state)
        {
            int cycles = 0;
            for (int i = 0; i < _Expression.EvaluateFloat(ref state); i++)
            {
                _RepeatBlock.Execute(ref state);
                if (cycles++ > 256)
                    return;
            }
        }
    }

}
