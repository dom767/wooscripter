using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class RuleBlock
    {
        List<Statement> _Statements = new List<Statement>();

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);

            string nexttoken = ParseUtils.PeekToken(program);

            while (nexttoken != "}")
            {
                TokenType type = WooScript.GetTokenType(nexttoken);
                if (nexttoken.Equals("if", StringComparison.Ordinal))
                {
                    IfStatement ifStatement = new IfStatement();
                    ifStatement.Parse(ref program);
                    _Statements.Add(ifStatement);
                }
                else if (nexttoken.Equals("repeat", StringComparison.Ordinal))
                {
                    RepeatStatement repeatStatement = new RepeatStatement();
                    repeatStatement.Parse(ref program);
                    _Statements.Add(repeatStatement);
                }
                else if (nexttoken.Equals("{", StringComparison.Ordinal))
                {
                    ScopeStatement scopeStatement = new ScopeStatement();
                    scopeStatement.Parse(ref program);
                    _Statements.Add(scopeStatement);
                }
                else if (type == TokenType.rule)
                {
                    CallStatement callStatement = new CallStatement();
                    callStatement.Parse(ref program);
                    _Statements.Add(callStatement);
                }
                else if (type == TokenType.nullFunction)
                {
                    NullStatement nullStatement = new NullStatement();
                    nullStatement.Parse(ref program);
                    _Statements.Add(nullStatement);
                }
                else if ((type == TokenType.vecVar) || (type == TokenType.floatVar))
                {
                    VarStatement varStatement = new VarStatement();
                    varStatement.Parse(ref program);
                    _Statements.Add(varStatement);
                }
                else
                {
                    throw new ParseException("Unexpected token \"" + nexttoken + "\" at start of expression");
                }

                nexttoken = ParseUtils.PeekToken(program);
            }

            string closebrace = ParseUtils.GetToken(ref program);

        }

        public void Execute(ref WooState state)
        {
            foreach (Statement s in _Statements)
            {
                s.Execute(ref state);
            }
        }
    }
}
