using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class Argument
    {
        public VarType _Type;
        public string _Name;

        public void Add()
        {
            if (_Type == VarType.varFloat)
            {
                WooScript.AddFloatVariable(_Name);
            }
            else if (_Type == VarType.varVector)
            {
                WooScript.AddVecVariable(_Name);
            }
        }

        public void Remove()
        {
            if (_Type == VarType.varFloat)
            {
                WooScript.RemoveFloatVariable(_Name);
            }
            else if (_Type == VarType.varVector)
            {
                WooScript.RemoveVecVariable(_Name);
            }
        }
    }
    public class Rule
    {
        public string _Name;
        RuleBlock block = new RuleBlock();
        List<Argument> _Arguments = new List<Argument>();

        public Rule(string name)
        {
            _Name = name;
        }

        public void Parse(ref string[] program)
        {
            string token = ParseUtils.PeekToken(program);
            if (token.Equals("(", StringComparison.Ordinal))
            {
                token = ParseUtils.GetToken(ref program);

                do
                {
                    Argument arg = new Argument();
                    token = ParseUtils.GetToken(ref program);
                    if (token.Equals("float", StringComparison.Ordinal))
                    {
                        arg._Type = VarType.varFloat;
                    }
                    else if (token.Equals("vec", StringComparison.Ordinal))
                    {
                        arg._Type = VarType.varVector;
                    }
                    else
                    {
                        throw new ParseException("Expected type of parameter (float OR vec), not" + token);
                    }

                    arg._Name = ParseUtils.GetToken(ref program);
                    WooScript.ValidateName(arg._Name);

                    _Arguments.Add(arg);
                    token = ParseUtils.GetToken(ref program);
                }
                while (token.Equals(",", StringComparison.Ordinal));

                if (!token.Equals(")", StringComparison.Ordinal))
                {
                    throw new ParseException("Expected \")\" but found \""+token+"\" instead. :(");
                }
            }

            foreach (Argument a in _Arguments) a.Add();
            block.Parse(ref program);
            foreach (Argument a in _Arguments) a.Remove();
        }

        public virtual bool CanRecurse()
        {
            return true;
        }

        public virtual void Execute(ref WooState state)
        {
            block.Execute(ref state);
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
            CFConstant absorption = ret._AbsorptionColour as CFConstant;
            absorption._Colour = state._Abs;
            ret._Opacity = (float)state._Opacity;
            ret._Density = (float)state._Density;
            ret._TintDensity = (float)state._TintDensity;
            ret._RefractiveIndex = (float)state._RefractiveIndex;
            ret._SpecularPower = (float)state._Power;
            ret._Shininess = (float)state._Gloss;
            ret._MaterialFunction = state._MaterialFunction;
            return ret;
        }
    }
}
