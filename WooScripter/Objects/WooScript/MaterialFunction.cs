using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class MaterialFunctionFunction : NullFunction
    {
        string _MaterialFunction;

        public void Parse(ref string[] program)
        {
            _MaterialFunction = ParseUtils.GetToken(ref program);
            if (!WooScript.IsShader(_MaterialFunction))
            {
                throw new ParseException("materialfunction call expected a shader, sadly " + _MaterialFunction + " isn't one...");
            }
        }

        public void Execute(ref WooState state)
        {
            state._MaterialFunction = state.GetShader(_MaterialFunction).Evaluate(state);
        }

        public string GetSymbol()
        {
            return "materialfunction";
        }

        public Function CreateNew()
        {
            return new MaterialFunctionFunction();
        }
    };

}
