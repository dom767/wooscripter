using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class SVORule : Rule
    {
        public SVORule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "svo - Create a sparse voxel octree";
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

                SVO newSVO = new SVO(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z),
                    state._Scale,
                    state._Rotation,
                    state._DistanceFunction,
                    state._DistanceMinimum,
                    state._DistanceScale,
                    state._DistanceOffset,
                    state._StepSize,
                    state._Depth);
                newSVO._Material = GenerateMaterial(state);
                newSVO.CreateElement(state._Preview, state._Parent);
            }
        }
    }
}
