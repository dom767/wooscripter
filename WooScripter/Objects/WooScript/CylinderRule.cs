using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class CylinderRule : Rule
    {
        public CylinderRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "cylinder - Create a cylinder";
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

            Cylinder newCylinder = new Cylinder(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z), state._Scale * 0.5, state._Rotation);
            newCylinder._Material = GenerateMaterial(state);
            newCylinder.CreateElement(state._Preview, state._Parent);
        }
    }
}
