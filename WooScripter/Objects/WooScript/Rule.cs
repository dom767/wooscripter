using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class Rule
    {
        public string _Name;
        RuleBlock block = new RuleBlock();

        public Rule(string name)
        {
            _Name = name;
        }

        public void Parse(ref string[] program)
        {
            block.Parse(ref program);
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
            ret._RefractiveIndex = (float)state._RefractiveIndex;
            ret._SpecularPower = (float)state._Power;
            ret._Shininess = (float)state._Gloss;
            ret._MaterialFunction = state._MaterialFunction;
            return ret;
        }
    }
}
