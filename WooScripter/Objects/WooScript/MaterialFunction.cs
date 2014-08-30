using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class DiffuseFunctionFunction : NullFunction
    {
        string _DiffuseFunction;

        public void Parse(ref string[] program)
        {
            _DiffuseFunction = ParseUtils.GetToken(ref program);
            ShaderScript.ValidateEstimator(_DiffuseFunction, 0);
        }

        public void Execute(ref WooState state)
        {
            state._DiffuseFunction = _DiffuseFunction;
        }

        public string GetSymbol()
        {
            return "diffusefunction";
        }

        public Function CreateNew()
        {
            return new DiffuseFunctionFunction();
        }
    };

    class SpecularFunctionFunction : NullFunction
    {
        string _SpecularFunction;

        public void Parse(ref string[] program)
        {
            _SpecularFunction = ParseUtils.GetToken(ref program);
            ShaderScript.ValidateEstimator(_SpecularFunction, 0);
        }

        public void Execute(ref WooState state)
        {
            state._SpecularFunction = _SpecularFunction;
        }

        public string GetSymbol()
        {
            return "specularfunction";
        }

        public Function CreateNew()
        {
            return new SpecularFunctionFunction();
        }
    };

    class EmissiveFunctionFunction : NullFunction
    {
        string _EmissiveFunction;

        public void Parse(ref string[] program)
        {
            _EmissiveFunction = ParseUtils.GetToken(ref program);
            ShaderScript.ValidateEstimator(_EmissiveFunction, 0);
        }

        public void Execute(ref WooState state)
        {
            state._EmissiveFunction = _EmissiveFunction;
        }

        public string GetSymbol()
        {
            return "emissivefunction";
        }

        public Function CreateNew()
        {
            return new EmissiveFunctionFunction();
        }
    };

    class ReflectiveFunctionFunction : NullFunction
    {
        string _ReflectiveFunction;

        public void Parse(ref string[] program)
        {
            _ReflectiveFunction = ParseUtils.GetToken(ref program);
            ShaderScript.ValidateEstimator(_ReflectiveFunction, 0);
        }

        public void Execute(ref WooState state)
        {
            state._ReflectiveFunction = _ReflectiveFunction;
        }

        public string GetSymbol()
        {
            return "reflectivefunction";
        }

        public Function CreateNew()
        {
            return new ReflectiveFunctionFunction();
        }
    };
}
