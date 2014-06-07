using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public interface Op
    {
        double EvaluateFloat(double arg1, double arg2);
        Vector3 EvaluateVector(Vector3 arg1, Vector3 arg2);
        string GetSymbol();
        int GetPrecedence();
        Op CreateNew();
    }

    public interface AssignOp
    {
        void ExecuteFloat(ref WooState state, string floatVar, Expression floatExpression);
        void ExecuteVector(ref WooState state, string vectorVar, Expression floatExpression);
        string GetSymbol();
        AssignOp CreateNew();
    }
}
