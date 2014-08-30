using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class IfStatement : Statement
    {
        public class IfConditionBlock
        {
            public ConditionalExpression _Condition;
            public RuleBlock _Block;
            public void Parse(ref string[] program)
            {
                string ifstring = ParseUtils.GetToken(ref program);
                if (!ifstring.Equals("if", StringComparison.Ordinal))
                    throw new ParseException("If Statement should always start with if, found " + ifstring + " instead.");

                string openbracket = ParseUtils.GetToken(ref program);
                if (!openbracket.Equals("(", StringComparison.Ordinal))
                    throw new ParseException("if statement must be followed by a condition in brackets, found " + openbracket + " instead.");
                _Condition = ConditionBuilder.Parse(ref program);
                string closebracket = ParseUtils.GetToken(ref program);
                if (!closebracket.Equals(")", StringComparison.Ordinal))
                    throw new ParseException("if statement found a conditional expression without a closing bracket, found " + closebracket + " instead.");

                _Block = new RuleBlock();
                _Block.Parse(ref program);
            }
        };

        public List<IfConditionBlock> _IfBlock = new List<IfConditionBlock>();
        public RuleBlock _ElseBlock;
        public void Parse(ref string[] program)
        {
            IfConditionBlock ifConditionBlock = new IfConditionBlock();
            ifConditionBlock.Parse(ref program);
            _IfBlock.Add(ifConditionBlock);

            string token = ParseUtils.PeekToken(program);
            while (token.Equals("else", StringComparison.Ordinal))
            {
                string elsestring = ParseUtils.GetToken(ref program);
                if (!elsestring.Equals("else", StringComparison.Ordinal))
                    throw new ParseException("missing else statement, found " + elsestring + " instead.");

                string iftoken = ParseUtils.PeekToken(program);
                if (iftoken.Equals("if", StringComparison.Ordinal))
                {
                    IfConditionBlock ifCondBlock = new IfConditionBlock();
                    ifCondBlock.Parse(ref program);
                    _IfBlock.Add(ifCondBlock);
                }
                else
                {
                    _ElseBlock = new RuleBlock();
                    _ElseBlock.Parse(ref program);
                    return; // no more blocks allowed...
                }
                token = ParseUtils.PeekToken(program);
            }
        }

        public void Execute(ref WooState state)
        {
            for (int i = 0; i < _IfBlock.Count; i++)
            {
                if (_IfBlock[i]._Condition.Evaluate(ref state))
                {
                    _IfBlock[i]._Block.Execute(ref state);
                    return;
                }
            }

            if (_ElseBlock != null)
            {
                _ElseBlock.Execute(ref state);
            }
        }
    }
}
