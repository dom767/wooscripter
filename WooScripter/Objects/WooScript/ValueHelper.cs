using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class ValueHelper
    {
        public static void Parse(ref string text, ref Number num1, ref Number num2, ref Number num3, ref int argCount, ref Log log)
        {
/*            num1 = new Number();
            num1.Parse(ParseUtils.GetToken(ref text), log);
            argCount = 1;
            int pos = text.IndexOf(',');
            if (text.IndexOf(',') >= 0)
            {
                num2 = new Number();
                num2.Parse(ParseUtils.GetToken(ref text), log);
                argCount++;
            }
            if (text.IndexOf(',') >= 0)
            {
                num3 = new Number();
                num3.Parse(ParseUtils.GetToken(ref text), log);
                argCount++;
            }
            */
        }

        public enum ModType
        {
            Set,
            Add,
            Sub,
            Mul,
            Div
        }

        public static void SetValue(string value, ref WooState state)
        {
            ModValue(value, ModType.Set, ref state);
        }

        public static void ModValue(string value, ModType modType, ref WooState state)
        {
            /*Vector3 vec = new Vector3(0, 0, 0);
            if (_Value.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                vec.x = _Number.GetNewNumber(state._X, state._Random);
            if (_Value.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                vec.y = _Number.GetNewNumber(state._Y, state._Random);
            if (_Value.Equals("z", StringComparison.InvariantCultureIgnoreCase))
                vec.z = _Number.GetNewNumber(state._Z, state._Random);

            vec.x = vec.x * state._Scale.x;
            vec.y = vec.y * state._Scale.y;
            vec.z = vec.z * state._Scale.z;
            
            vec.Mul(state._Rotation);
            state._X += vec.x;
            state._Y += vec.y;
            state._Z += vec.z;

            if (_Value.Equals("rx", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rX, state._Random);
                state._rX += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(delta * 2 * 3.141592 / 360, 0, 0);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            }
            if (_Value.Equals("ry", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rY, state._Random);
                state._rY += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(0, delta * 2 * 3.141592 / 360, 0);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            } 
            if (_Value.Equals("rz", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rZ, state._Random);
                state._rZ += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(0, 0, delta * 2 * 3.141592 / 360);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            }

            if (_Value.Equals("diff", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Diff._Red += _Number.GetNewNumber(state._Diff._Red, state._Random);
                state._Diff._Green += _Number2.GetNewNumber(state._Diff._Green, state._Random);
                state._Diff._Blue += _Number3.GetNewNumber(state._Diff._Blue, state._Random);
            }
            if (_Value.Equals("diffR", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Red += _Number.GetNewNumber(state._Diff._Red, state._Random);
            if (_Value.Equals("diffG", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Green += _Number.GetNewNumber(state._Diff._Green, state._Random);
            if (_Value.Equals("diffB", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Blue += _Number.GetNewNumber(state._Diff._Blue, state._Random);

            if (_Value.Equals("refl", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Refl._Red += _Number.GetNewNumber(state._Refl._Red, state._Random);
                state._Refl._Green += _Number2.GetNewNumber(state._Refl._Green, state._Random);
                state._Refl._Blue += _Number3.GetNewNumber(state._Refl._Blue, state._Random);
            }
            if (_Value.Equals("reflR", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Red += _Number.GetNewNumber(state._Refl._Red, state._Random);
            if (_Value.Equals("reflG", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Green += _Number.GetNewNumber(state._Refl._Green, state._Random);
            if (_Value.Equals("reflB", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Blue += _Number.GetNewNumber(state._Refl._Blue, state._Random);

            if (_Value.Equals("emi", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Emi._Red += _Number.GetNewNumber(state._Emi._Red, state._Random);
                state._Emi._Green += _Number2.GetNewNumber(state._Emi._Green, state._Random);
                state._Emi._Blue += _Number3.GetNewNumber(state._Emi._Blue, state._Random);
            }
            if (_Value.Equals("emiR", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Red += _Number.GetNewNumber(state._Emi._Red, state._Random);
            if (_Value.Equals("emiG", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Green += _Number.GetNewNumber(state._Emi._Green, state._Random);
            if (_Value.Equals("emiB", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Blue += _Number.GetNewNumber(state._Emi._Blue, state._Random);

            if (_Value.Equals("spec", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Spec._Red += _Number.GetNewNumber(state._Spec._Red, state._Random);
                state._Spec._Green += _Number2.GetNewNumber(state._Spec._Green, state._Random);
                state._Spec._Blue += _Number3.GetNewNumber(state._Spec._Blue, state._Random);
            }
            if (_Value.Equals("specR", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Red += _Number.GetNewNumber(state._Spec._Red, state._Random);
            if (_Value.Equals("specG", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Green += _Number.GetNewNumber(state._Spec._Green, state._Random);
            if (_Value.Equals("specB", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Blue += _Number.GetNewNumber(state._Spec._Blue, state._Random);

            if (_Value.Equals("power", StringComparison.InvariantCultureIgnoreCase))
                state._Power += (float)_Number.GetNewNumber((double)state._Power, state._Random);

            if (_Value.Equals("scale", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Scale.x += _Number.GetNewNumber(state._Scale.x, state._Random);
                state._Scale.y += _Number2.GetNewNumber(state._Scale.y, state._Random);
                state._Scale.z += _Number3.GetNewNumber(state._Scale.z, state._Random);
            }
            if (_Value.Equals("scaleX", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.x += _Number.GetNewNumber(state._Scale.x, state._Random);
            if (_Value.Equals("scaleY", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.y += _Number.GetNewNumber(state._Scale.y, state._Random);
            if (_Value.Equals("scaleZ", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.z += _Number.GetNewNumber(state._Scale.z, state._Random);

            if (_Value.Equals("recursions", StringComparison.InvariantCultureIgnoreCase))
                state._Recursions += (int)(0.5+_Number.GetNewNumber((double)state._Recursions, state._Random));
      */  }
        /*
        var GetValue(ref WooState state, string value)
        {
            if (value.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                return state._X;
            if (_Value.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                vec.y = _Number.GetNewNumber(state._Y, state._Random);
            if (_Value.Equals("z", StringComparison.InvariantCultureIgnoreCase))
                vec.z = _Number.GetNewNumber(state._Z, state._Random);
            if (_Value.Equals("rx", StringComparison.InvariantCultureIgnoreCase))
        }
        
        SetValue(string value)
        {
        }*/
    }
}
