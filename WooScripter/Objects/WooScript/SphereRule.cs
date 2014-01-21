using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class SphereRule : Rule
    {
        public SphereRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "sphere - Create a sphere";
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

            Sphere newSphere = new Sphere(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z),
                new Vector3(state._Scale.x * 0.5, state._Scale.y * 0.5, state._Scale.z * 0.5), state._Rotation);
            newSphere._Material = GenerateMaterial(state);
            newSphere.CreateElement(state._Preview, state._Parent);
        }
    }
}
