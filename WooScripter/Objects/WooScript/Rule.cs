using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class Rule
    {
        public string _Name;
        List<Statement> _Statements = new List<Statement>();

        public Rule(string name)
        {
            _Name = name;
        }

        public void Parse(ref string[] program)
        {
            string openbrace = ParseUtils.GetToken(ref program);

            string nexttoken = ParseUtils.PeekToken(program);

            while (nexttoken != "}")
            {
                TokenType type = WooScript.GetTokenType(nexttoken);
                if (type == TokenType.nullFunction)
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

            /*            Op op;

                        string opId = ParseUtils.GetToken(ref program);

                        while (opId.Length > 0)
                        {
                            int start = program.IndexOf('(');
                            int end = program.IndexOf(')');
                            if (end < start) throw new ParseException("Error in parser : " + program);
                            string method = program.Substring(start + 1, end - (start + 1));
                            if (string.Compare(opId, "call", true) == 0)
                            {
                                log.AddMsg("Found call operation");
                                op = new CallOp();
                                log.Indent();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else if (string.Compare(opId, "finalcall", true) == 0)
                            {
                                log.AddMsg("Found finalcall operation");
                                op = new FinalCallOp();
                                log.Indent();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else if (string.Compare(opId, "repeat", true) == 0)
                            {
                                log.AddMsg("Found repeat operation");
                                op = new RepeatOp();
                                log.Indent();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);                    
                            }
                            else if (string.Compare(opId, "branch", true) == 0)
                            {
                                log.AddMsg("Found branch operation");
                                op = new BranchOp();
                                log.Indent();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else if (string.Compare(opId, "adjust", true) == 0)
                            {
                                log.AddMsg("Found adjust operation");
                                log.Indent();
                                op = new AdjustOp();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else if (string.Compare(opId, "push", true) == 0)
                            {
                                log.AddMsg("Found push operation");
                                log.Indent();
                                op = new PushOp();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else if (string.Compare(opId, "pop", true) == 0)
                            {
                                log.AddMsg("Found pop operation");
                                log.Indent();
                                op = new PopOp();
                                op.Parse(method, log);
                                log.UnIndent();
                                _Ops.Add(op);
                            }
                            else
                            {
                                throw new ParseException("Unrecognised method name : " + opId);
                            }
                            data = data.Substring(end+1);
                            opId = ParseUtils.GetToken(ref data);
                        }
             */
        }

        public virtual bool CanRecurse()
        {
            return true;
        }

        public virtual void Execute(ref WooState state)
        {
            foreach (Statement s in _Statements)
            {
                s.Execute(ref state);
            }
        }

        public virtual string GetHelpText()
        {
            return _Name;
        }

        protected Material GenerateMaterial(WooState state)
        {
            Material ret = new Material();
            CFConstant diffuse = ret._DiffuseColour as CFConstant;
            diffuse._Colour = state._Diff;
            CFConstant reflectivity = ret._Reflectivity as CFConstant;
            reflectivity._Colour = state._Refl;
            CFConstant emissive = ret._EmissiveColour as CFConstant;
            emissive._Colour = state._Emi;
            CFConstant specular = ret._SpecularColour as CFConstant;
            specular._Colour = state._Spec;
            ret._SpecularPower = (float)state._Power;
            ret._Shininess = (float)state._Gloss;
            ret._DiffuseFunction = state._DiffuseFunction;
            ret._SpecularFunction = state._SpecularFunction;
            ret._EmissiveFunction = state._EmissiveFunction;
            ret._ReflectiveFunction = state._ReflectiveFunction;
            return ret;
        }
    }
}
