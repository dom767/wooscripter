using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooScripter.Objects.WooScript
{
    public class WooState
    {
        public Vector3 _Position = new Vector3(0, 0, 0);
        public double _rX = 0;
        public double _rY = 0;
        public double _rZ = 0;
        public Colour _Diff = new Colour(1, 1, 1);
        public Colour _Refl = new Colour(0.3, 0.3, 0.3);
        public Colour _Emi = new Colour(0, 0, 0);
        public Colour _Spec = new Colour(0.4, 0.4, 0.4);
        public double _Power = 8;
        public double _Gloss = 1;
        public Vector3 _Scale = new Vector3(1, 1, 1);
        public Vector3 _v0 = new Vector3(0, 0, 0);
        public Vector3 _v1 = new Vector3(0, 0, 0);
        public Vector3 _v2 = new Vector3(0, 0, 0);
        public Vector3 _v3 = new Vector3(0, 0, 0);
        public Matrix3 _Rotation;
        public List<Rule> _Rules;
        public int _Recursions = 10;
        public WooState _PreviousState;
        public bool _Preview = false;
        public int _MengerIterations = 4;
        public int[] _MengerPattern = new int[27];
        public string _DistanceFunction = "sphere(pos, vec(0,0,0), 1)";
        public double _DistanceMinimum = 0.01f;
        public double _DistanceScale = 1.0f;
        public Vector3 _DistanceOffset = new Vector3(0,0,0);
        public int _DistanceIterations = 200;

        public WooState()
        {
            _Rotation = new Matrix3();
            _Rotation.MakeIdentity();
            int idx = 0;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;

            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 1;

            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 0;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
            _MengerPattern[idx++] = 1;
        }
        public Rule GetRule(string name)
        {
            foreach (Rule r in _Rules)
            {
                if (r._Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return r;
                }
            }
            throw new EvaluateException("Tried to call rule " + name + ", but it doesn't exist...");
        }
        public Random _Random;
        public XElement _Parent;
        public WooState Clone()
        {
            WooState clone = new WooState();
            clone._Position = this._Position.Clone();
            clone._rX = this._rX;
            clone._rY = this._rY;
            clone._rZ = this._rZ;
            clone._Diff = this._Diff.Clone();
            clone._Refl = this._Refl.Clone();
            clone._Emi = this._Emi.Clone();
            clone._Spec = this._Spec.Clone();
            clone._v0 = this._v0.Clone();
            clone._v1 = this._v1.Clone();
            clone._v2 = this._v2.Clone();
            clone._v3 = this._v3.Clone();
            clone._Power = this._Power;
            clone._Gloss = this._Gloss;
            clone._Rules = this._Rules;
            clone._Random = this._Random;
            clone._Parent = this._Parent;
            clone._Rotation = this._Rotation.Clone();
            clone._Scale = new Vector3(this._Scale);
            clone._Recursions = this._Recursions;
            clone._PreviousState = this._PreviousState;
            clone._Preview = this._Preview;
            clone._MengerIterations = this._MengerIterations;
            for (int i = 0; i < 27; i++)
                clone._MengerPattern[i] = this._MengerPattern[i];
            clone._DistanceFunction = this._DistanceFunction;
            clone._DistanceMinimum = this._DistanceMinimum;
            clone._DistanceScale = this._DistanceScale;
            clone._DistanceOffset = this._DistanceOffset.Clone();
            clone._DistanceIterations = this._DistanceIterations;
            return clone;
        }
        void SetSelectedValue(ref Vector3 target, string selector, double value)
        {
            if (selector.Equals("x", StringComparison.Ordinal)
                || selector.Equals("r", StringComparison.Ordinal))
            {
                target.x = value;
            }
            if (selector.Equals("y", StringComparison.Ordinal)
                || selector.Equals("g", StringComparison.Ordinal))
            {
                target.y = value;
            }
            if (selector.Equals("z", StringComparison.Ordinal)
                || selector.Equals("b", StringComparison.Ordinal))
            {
                target.z = value;
            }
        }
        double GetSelectedValueFloat(Vector3 target, string selector)
        {
            if (selector.Equals("x", StringComparison.Ordinal)
                            || selector.Equals("r", StringComparison.Ordinal))
            {
                return target.x;
            }
            if (selector.Equals("y", StringComparison.Ordinal)
                || selector.Equals("g", StringComparison.Ordinal))
            {
                return target.y;
            }
            if (selector.Equals("z", StringComparison.Ordinal)
                || selector.Equals("b", StringComparison.Ordinal))
            {
                return target.z;
            }
            throw new ParseException("Selector \"" + selector + "\" doesn't match any value.");
        }
        public double GetValueFloat(string target)
        {
            int dotIndex = target.IndexOf('.');
            if (dotIndex >= 0)
            {
                // target is a vector
                string varname = target.Substring(0, dotIndex);
                string selector = target.Substring(dotIndex + 1);
                return GetSelectedValueFloat(GetValueVector(varname), selector);
            }
            else
            {
                if (target.Equals("rx", StringComparison.Ordinal))
                    return _rX;
                if (target.Equals("ry", StringComparison.Ordinal))
                    return _rY;
                if (target.Equals("rz", StringComparison.Ordinal))
                    return _rZ;
                if (target.Equals("power", StringComparison.Ordinal))
                    return _Power;
                if (target.Equals("gloss", StringComparison.Ordinal))
                    return _Gloss;
                if (target.Equals("recursions", StringComparison.Ordinal))
                    return _Recursions;
                if (target.Equals("mengeriterations", StringComparison.Ordinal))
                    return _MengerIterations;
                if (target.Equals("distanceminimum", StringComparison.Ordinal))
                    return _DistanceMinimum;
                if (target.Equals("distanceiterations", StringComparison.Ordinal))
                    return _DistanceIterations;
                if (target.Equals("distancescale", StringComparison.Ordinal))
                    return _DistanceScale;
            }
            throw new ParseException("no matching target for \"" + target + "\"");
        }

        public Vector3 GetValueVector(string target)
        {
            int dotIndex = target.IndexOf('.');
            if (dotIndex >= 0)
            {
                // target is a vector
                string varname = target.Substring(0, dotIndex);
                string selector = target.Substring(dotIndex + 1);
                if (varname.Equals("pos", StringComparison.Ordinal))
                    return _Position;// GetSelectedValueVector(_Position, selector);
            }
            else
            {
                if (target.Equals("pos", StringComparison.Ordinal))
                    return new Vector3(_Position);
                if (target.Equals("scale", StringComparison.Ordinal))
                    return new Vector3(_Scale);
                if (target.Equals("diff", StringComparison.Ordinal))
                    return new Vector3(_Diff);
                if (target.Equals("refl", StringComparison.Ordinal))
                    return new Vector3(_Refl);
                if (target.Equals("emi", StringComparison.Ordinal))
                    return new Vector3(_Emi);
                if (target.Equals("spec", StringComparison.Ordinal))
                    return new Vector3(_Spec);
                if (target.Equals("v0", StringComparison.Ordinal))
                    return new Vector3(_v0);
                if (target.Equals("v1", StringComparison.Ordinal))
                    return new Vector3(_v1);
                if (target.Equals("v2", StringComparison.Ordinal))
                    return new Vector3(_v2);
                if (target.Equals("v3", StringComparison.Ordinal))
                    return new Vector3(_v3);
                if (target.Equals("distanceoffset", StringComparison.Ordinal))
                    return new Vector3(_DistanceOffset);
            }
            throw new ParseException("no matching target for \"" + target + "\"");
        }

        private void SetValueInternal(Vector3 vec, string varname, string selector, double value, bool overrideState)
        {
            Vector3 newval = new Vector3(vec);
            SetSelectedValue(ref newval, selector, value);
            SetValue3arg(varname, newval, overrideState);
        }

        public void SetValue(string target, double value)
        {
            SetValue3arg(target, value, false);
        }

        public void SetValueOverride(string target, double value)
        {
            SetValue3arg(target, value, true);
        }

        private void SetValue3arg(string target, double value, bool overrideState)
        {
            int dotindex = target.IndexOf('.');
            if (dotindex >= 0)
            {
                // target is a vector
                string varname = target.Substring(0, dotindex);
                string selector = target.Substring(dotindex + 1);
                if (varname.Equals("pos", StringComparison.Ordinal))
                    SetValueInternal(_Position, varname, selector, value, overrideState);
                else if (varname.Equals("scale", StringComparison.Ordinal))
                    SetValueInternal(_Scale, varname, selector, value, overrideState);
                else if (varname.Equals("diff", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_Diff), varname, selector, value, overrideState);
                else if (varname.Equals("refl", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_Refl), varname, selector, value, overrideState);
                else if (varname.Equals("emi", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_Emi), varname, selector, value, overrideState);
                else if (varname.Equals("spec", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_Spec), varname, selector, value, overrideState);
                else if (varname.Equals("v0", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_v0), varname, selector, value, overrideState);
                else if (varname.Equals("v1", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_v1), varname, selector, value, overrideState);
                else if (varname.Equals("v2", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_v2), varname, selector, value, overrideState);
                else if (varname.Equals("v3", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_v3), varname, selector, value, overrideState);
                else if (varname.Equals("distanceoffset", StringComparison.Ordinal))
                    SetValueInternal(new Vector3(_DistanceOffset), varname, selector, value, overrideState);
            }
            else
            {
                if (target.Equals("rx", StringComparison.Ordinal))
                {
                    double delta = value - _rX;
                    _rX = value;
                    Matrix3 rotMatrix = new Matrix3();
                    rotMatrix.MakeFromRPY(delta * 2 * 3.141592 / 360, 0, 0);
                    rotMatrix.Mul(_Rotation);
                    _Rotation = rotMatrix;
                }
                if (target.Equals("ry", StringComparison.Ordinal))
                {
                    double delta = value - _rY;
                    _rY = value;
                    Matrix3 rotMatrix = new Matrix3();
                    rotMatrix.MakeFromRPY(0, delta * 2 * 3.141592 / 360, 0);
                    rotMatrix.Mul(_Rotation);
                    _Rotation = rotMatrix;
                }
                if (target.Equals("rz", StringComparison.Ordinal))
                {
                    double delta = value - _rZ;
                    _rZ = value;
                    Matrix3 rotMatrix = new Matrix3();
                    rotMatrix.MakeFromRPY(0, 0, delta * 2 * 3.141592 / 360);
                    rotMatrix.Mul(_Rotation);
                    _Rotation = rotMatrix;
                }
                if (target.Equals("power", StringComparison.Ordinal))
                {
                    _Power = value;
                }
                if (target.Equals("gloss", StringComparison.Ordinal))
                {
                    _Gloss = value;
                }
                if (target.Equals("recursions", StringComparison.Ordinal))
                {
                    _Recursions = (int)(value + 0.5);
                }
                if (target.Equals("mengeriterations", StringComparison.Ordinal))
                {
                    _MengerIterations = (int)(value + 0.5);
                }
                if (target.Equals("distanceminimum", StringComparison.Ordinal))
                {
                    _DistanceMinimum = value;
                }
                if (target.Equals("distancescale", StringComparison.Ordinal))
                {
                    _DistanceScale = value;
                }
                if (target.Equals("distanceiterations", StringComparison.Ordinal))
                {
                    _DistanceIterations = (int)(value+0.5);
                }
            }
        }
        public void SetValue(string target, Vector3 arg)
        {
            SetValue3arg(target, arg, false);
        }
        public void SetValueOverride(string target, Vector3 arg)
        {
            SetValue3arg(target, arg, true);
        }
        private void SetValue3arg(string target, Vector3 arg, bool overrideState)
        {
            string varname = target;
            int dotindex = target.IndexOf('.');
            if (dotindex >= 0)
            {
                // target is a vector
                varname = target.Substring(0, dotindex);
                string selector = target.Substring(dotindex + 1);
            }

            if (varname.Equals("pos", StringComparison.Ordinal))
            {
                if (overrideState)
                {
                    _Position = arg;
                }
                else
                {
                    Vector3 newval = arg;
                    Vector3 vec = new Vector3();
                    vec.x = newval.x - _Position.x;
                    vec.y = newval.y - _Position.y;
                    vec.z = newval.z - _Position.z;
                    vec.x = vec.x * _Scale.x;
                    vec.y = vec.y * _Scale.y;
                    vec.z = vec.z * _Scale.z;
                    vec.Mul(_Rotation);
                    _Position.x += vec.x;
                    _Position.y += vec.y;
                    _Position.z += vec.z;
                }
            }

            if (varname.Equals("scale", StringComparison.Ordinal))
            {
                _Scale.x = arg.x;
                _Scale.y = arg.y;
                _Scale.z = arg.z;
            }

            if (varname.Equals("diff", StringComparison.Ordinal))
            {
                _Diff._Red = arg.x;
                _Diff._Green = arg.y;
                _Diff._Blue = arg.z;
            }

            if (varname.Equals("refl", StringComparison.Ordinal))
            {
                _Refl._Red = arg.x;
                _Refl._Green = arg.y;
                _Refl._Blue = arg.z;
            }

            if (varname.Equals("emi", StringComparison.Ordinal))
            {
                _Emi._Red = arg.x;
                _Emi._Green = arg.y;
                _Emi._Blue = arg.z;
            }

            if (varname.Equals("spec", StringComparison.Ordinal))
            {
                _Spec._Red = arg.x;
                _Spec._Green = arg.y;
                _Spec._Blue = arg.z;
            }
            if (varname.Equals("v0", StringComparison.Ordinal))
            {
                _v0.x = arg.x;
                _v0.y = arg.y;
                _v0.z = arg.z;
            }
            if (varname.Equals("v1", StringComparison.Ordinal))
            {
                _v1.x = arg.x;
                _v1.y = arg.y;
                _v1.z = arg.z;
            }
            if (varname.Equals("v2", StringComparison.Ordinal))
            {
                _v2.x = arg.x;
                _v2.y = arg.y;
                _v2.z = arg.z;
            }
            if (varname.Equals("v3", StringComparison.Ordinal))
            {
                _v3.x = arg.x;
                _v3.y = arg.y;
                _v3.z = arg.z;
            }
            if (varname.Equals("distanceoffset", StringComparison.Ordinal))
            {
                _DistanceOffset.x = arg.x;
                _DistanceOffset.y = arg.y;
                _DistanceOffset.z = arg.z;
            }
        }
    };
}
