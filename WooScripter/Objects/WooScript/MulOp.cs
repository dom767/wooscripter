using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class MulOp : Op
    {
        public double EvaluateFloat(double arg1, double arg2)
        {
            return arg1 * arg2;
        }

        public Vector3 EvaluateVector(Vector3 arg1, Vector3 arg2)
        {
            return new Vector3(arg1.x * arg2.x,
                arg1.y * arg2.y,
                arg1.z * arg2.z);
        }

        public string GetSymbol()
        {
            return "*";
        }

        public Op CreateNew()
        {
            return new MulOp();
        }

        public int GetPrecedence()
        {
            return 3;
        }
    }
}
