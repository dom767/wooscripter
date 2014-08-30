using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class DistanceFunctionFunction : NullFunction
    {
        string _DistanceFunction;
        bool _ShaderMode;

        public void Parse(ref string[] program)
        {
            _DistanceFunction = ParseUtils.GetToken(ref program);
            if (WooScript.IsShader(_DistanceFunction))
            {
                _ShaderMode = true;
            }
            else
            {
                ShaderScript.ValidateEstimator(_DistanceFunction, 1);
                _ShaderMode = false;
            }
        }

        public void Execute(ref WooState state)
        {
            if (_ShaderMode)
            {
                state._DistanceFunction = state.GetShader(_DistanceFunction).Evaluate(state);
            }
            else
            {
                state._DistanceFunction = "set(distance, " + _DistanceFunction + ")";
            }
        }

        public string GetSymbol()
        {
            return "distancefunction";
        }

        public Function CreateNew()
        {
            return new DistanceFunctionFunction();
        }
    };
    
    class DistanceRule : Rule
    {
        public DistanceRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "distance - Create a distance estimation object";
        }

        public override bool CanRecurse()
        {
            return false;
        }

        public override void Execute(ref WooState state)
        {
            Vector3 val = new Vector3(0.0, 0.5, 0.0);
            val.y *= state._Scale.y;
            val.Mul(state._Rotation);

            Distance newDistance = new Distance(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z),
                state._Scale * 0.5,
                state._Rotation,
                state._DistanceFunction,
                state._DistanceMinimum,
                state._DistanceScale,
                state._DistanceOffset,
                state._DistanceIterations,
                state._StepSize);
            newDistance._Material = GenerateMaterial(state);
            newDistance.CreateElement(state._Preview, state._Parent);
        }
    };
}
