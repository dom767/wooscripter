using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class SetAddOp : AssignOp
    {
        public void ExecuteFloat(ref WooState state, string varName, Expression expression)
        {
            double value = expression.EvaluateFloat(ref state);
            double current = state.GetValueFloat(varName);
            state.SetValue(varName, value + current);
        }

        public void ExecuteVector(ref WooState state, string varName, Expression expression)
        {
            Vector3 value = expression.EvaluateVector(ref state);
            Vector3 current = state.GetValueVector(varName);
            current.x += value.x;
            current.y += value.y;
            current.z += value.z;
            state.SetValue(varName, current);
        }

        public string GetSymbol()
        {
            return "+=";
        }

        public AssignOp CreateNew()
        {
            return new SetAddOp();
        }
    }
}
