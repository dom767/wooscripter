using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class SetOp : AssignOp
    {
        public void ExecuteFloat(ref WooState state, string varName, Expression expression)
        {
            double value = expression.EvaluateFloat(ref state);
            //            state.GetValue(floatVar);
            state.SetValueOverride(varName, value);
        }

        public void ExecuteVector(ref WooState state, string varName, Expression expression)
        {
            Vector3 value = expression.EvaluateVector(ref state);
            //            state.GetValue(floatVar);
            state.SetValueOverride(varName, value);
        }

        public string GetSymbol()
        {
            return "=";
        }

        public AssignOp CreateNew()
        {
            return new SetOp();
        }
    }
}
