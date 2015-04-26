using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class MengerPatternFunction : NullFunction
    {
        int[] _Pattern = new int [27];

        public void Parse(ref string[] program)
        {
            string _PatternStr = ParseUtils.GetToken(ref program);
            if (_PatternStr.Length!=27)
                throw new ParseException("argument must have 27 digits mengerpattern(101101...)");

            for (int i = 0; i < 27; i++)
            {
                if (_PatternStr[i] == '1')
                    _Pattern[i] = 1;
                else
                    _Pattern[i] = 0;
            }
        }

        public void Execute(ref WooState state)
        {
            state._MengerPattern = _Pattern;
        }

        public string GetSymbol()
        {
            return "mengerpattern";
        }

        public Function CreateNew()
        {
            return new MengerPatternFunction();
        }
    };

    class MengerRule : Rule
    {
        public MengerRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "menger - Create a menger 3x3x3 fractal";
        }

        public override bool CanRecurse()
        {
            return false;
        }

        public override void Execute(ref WooState state)
        {
            if (state._Objects > 0)
            {
                state._Objects--;
                Vector3 val = new Vector3(0.0, 0.5, 0.0);
                val.y *= state._Scale.y;
                val.Mul(state._Rotation);

                Menger newMenger = new Menger(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z),
                    state._Scale,
                    state._Rotation,
                    state._MengerIterations,
                    state._MengerPattern);
                newMenger._Material = GenerateMaterial(state);
                newMenger.CreateElement(state._Preview, state._Parent);
            }
        }
    }
}
