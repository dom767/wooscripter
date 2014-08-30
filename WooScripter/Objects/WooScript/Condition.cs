using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class ConditionBuilder
    {
        public static ConditionalExpression Parse(ref string[] program)
        {
            ConditionalExpression ret = null;
            Expression expr = null;

            string token1 = ParseUtils.PeekToken(program);
            TokenType type1 = WooScript.GetTokenType(token1);
            if (token1.Equals("(", StringComparison.Ordinal))
            {
                ret = new ConditionalBrackets();
                (ret as ConditionalBrackets).Parse(ref program);
            }
            else if (type1 == TokenType.UnaryBooleanOp)
            {
                ret = WooScript.GetUnaryBooleanOp(token1);
                (ret as UnaryBooleanOp).Parse(ref program);
            }
            else if (type1 == TokenType.floatVar
                || type1 == TokenType.floatNum
                || type1 == TokenType.floatFunction)
            {
                expr = ExpressionBuilder.Parse(ref program);
            }
            else
                throw new ParseException("Unrecognised expression \"" + token1 + "\"");

            string token2 = ParseUtils.GetToken(ref program);
            TokenType type2 = WooScript.GetTokenType(token2);
            if (type2 == TokenType.ConditionalOp)
            {
                if (type1 != TokenType.floatNum && type1 != TokenType.floatVar && type1 != TokenType.floatFunction)
                    throw new ParseException("Conditional operation only takes floating point parameters");

                ConditionalOp condOp = WooScript.GetConditionalOp(token2);
                condOp._Arg1 = expr;
                Expression arg2 = ExpressionBuilder.Parse(ref program);

                if (arg2.GetExpressionType() != VarType.varFloat)
                    throw new ParseException("Conditional operation only takes floating point parameters");

                condOp._Arg2 = arg2;

                // no need for precedence check on conditional operator
                ret = condOp;

                string token3 = ParseUtils.PeekToken(program);
                TokenType type3 = WooScript.GetTokenType(token3);
                if (type3 == TokenType.BooleanOp)
                {
                    BooleanOp boolOp = WooScript.GetBooleanOp(token3);
                    boolOp._Arg1 = condOp;

                    ConditionalExpression condArg2 = ConditionBuilder.Parse(ref program);
                    boolOp._Arg2 = condArg2;

                    // operator precedence check
                    if (condArg2 is BooleanOp)
                    {
                        if ((condArg2 as BooleanOp).GetPrecedence() < boolOp.GetPrecedence())
                        {
                            // shuffle args
                            boolOp._Arg2 = (condArg2 as BooleanOp)._Arg1;
                            (condArg2 as BooleanOp)._Arg1 = boolOp;
                            boolOp = (condArg2 as BooleanOp);
                        }
                    }

                    ret = boolOp;
                }
            }

            if (ret == null)
                throw new ParseException("Malformed conditional expression, expected conditional operation");
            return ret;
        }
    }

    public interface ConditionalExpression
    {
//        void Parse(ref string[] program);
        bool Evaluate(ref WooState state);
    }

    public class ConditionalBrackets : ConditionalExpression
    {
        ConditionalExpression _ConditionalExpression;
        public void Parse(ref string[] program)
        {
            _ConditionalExpression = ConditionBuilder.Parse(ref program);
        }
        public bool Evaluate(ref WooState state)
        {
            return _ConditionalExpression.Evaluate(ref state);
        }
    }

    public abstract class ConditionalOp : ConditionalExpression
    {
        public Expression _Arg1;
        public Expression _Arg2;
//        public void Parse(ref string[] program) { }
        public abstract bool Evaluate(ref WooState state);
        public abstract string GetSymbol();
        public abstract ConditionalOp CreateNew();
    }

    public class LessOp : ConditionalOp
    {
        public override bool Evaluate(ref WooState state)
        {
            return _Arg1.EvaluateFloat(ref state) < _Arg2.EvaluateFloat(ref state);
        }
        public override string GetSymbol()
        {
            return "<";
        }
        public override ConditionalOp CreateNew()
        {
            return new LessOp();
        }
    };

    public class GreaterOp : ConditionalOp
    {
        public override bool Evaluate(ref WooState state)
        {
            return _Arg1.EvaluateFloat(ref state) > _Arg2.EvaluateFloat(ref state);
        }
        public override string GetSymbol()
        {
            return ">";
        }
        public override ConditionalOp CreateNew()
        {
            return new GreaterOp();
        }
    };

    public abstract class BooleanOp : ConditionalExpression
    {
        public ConditionalExpression _Arg1;
        public ConditionalExpression _Arg2;
        public abstract bool Evaluate(ref WooState state);
        public abstract string GetSymbol();
        public abstract int GetPrecedence();
        public abstract BooleanOp CreateNew();
    }

    public class AndOp : BooleanOp
    {
        public override bool Evaluate(ref WooState state)
        {
            return _Arg1.Evaluate(ref state) & _Arg2.Evaluate(ref state);
        }
        public override string GetSymbol()
        {
            return "&";
        }
        public override int GetPrecedence()
        {
            return 2;
        }
        public override BooleanOp CreateNew()
        {
            return new AndOp();
        }
    }

    public class OrOp : BooleanOp
    {
        public override bool Evaluate(ref WooState state)
        {
            return _Arg1.Evaluate(ref state) | _Arg2.Evaluate(ref state);
        }
        public override string GetSymbol()
        {
            return "|";
        }
        public override int GetPrecedence()
        {
            return 1;
        }
        public override BooleanOp CreateNew()
        {
            return new OrOp();
        }
    }

    public interface UnaryBooleanOp : ConditionalExpression
    {
        void Parse(ref string[] program);
        bool Evaluate(ref WooState state);
        string GetSymbol();
        int GetPrecedence();
        UnaryBooleanOp CreateNew();
    }

    public class NotOp : UnaryBooleanOp
    {
        ConditionalExpression _Arg1;
        public void Parse(ref string[] program)
        {
            _Arg1 = ConditionBuilder.Parse(ref program);
        }
        public bool Evaluate(ref WooState state)
        {
            return !(_Arg1.Evaluate(ref state));
        }
        public string GetSymbol()
        {
            return "!";
        }
        public int GetPrecedence()
        {
            return 3;
        }
        public UnaryBooleanOp CreateNew()
        {
            return new NotOp();
        }
    }
}
