using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class SphereLightRule : Rule
    {
        public SphereLightRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "sphereLight - Create a spherical light (used as light source)";
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
            Vector3 pos = new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z);

            SphereLight newLight = new SphereLight(state._Diff,
                pos,
                (float)(state._Scale.y * 0.5),
                2);
            newLight.CreateElement(state._Parent, pos);
        }
    }
}
