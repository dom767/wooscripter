using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class CircleRule : Rule
    {
        public CircleRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "circle - Create a circle";
        }

        public override bool CanRecurse()
        {
            return false;
        }

        public override void Execute(ref WooState state)
        {
            Circle newCircle = new Circle(new Vector3(state._Position.x, state._Position.y, state._Position.z), state._Scale*0.5, state._Rotation);
            newCircle._Material = GenerateMaterial(state);
            newCircle.CreateElement(state._Preview, state._Parent);
        }
    }
}
