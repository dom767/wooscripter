using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class BoxRule : Rule
    {
        public BoxRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "box - Create a cube";
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

            Cube newCube = new Cube(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z), state._Scale, state._Rotation);
            newCube._Material = GenerateMaterial(state);
            newCube.CreateElement(state._Preview, state._Parent);
        }
    }
}
