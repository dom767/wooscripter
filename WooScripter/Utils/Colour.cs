using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter
{
    [Serializable]
    public class Colour
    {
        public double _Red;
        public double _Green;
        public double _Blue;

        public Colour()
        {
        }

        public Colour(Colour rhs)
        {
            _Red = rhs._Red;
            _Green = rhs._Green;
            _Blue = rhs._Blue;
        }

        public Colour(double red, double green, double blue)
        {
            _Red = red;
            _Green = green;
            _Blue = blue;
        }
        
        public override string ToString()
        {
            return _Red.ToString() + ", " + _Green.ToString() + ", " + _Blue.ToString();
        }

        public Colour Clone()
        {
            Colour ret = new Colour(_Red, _Green, _Blue);
            return ret;
        }
        // http://www.cs.rit.edu/~ncs/color/t_convert.html
       // public

        public void Clamp(double min, double max)
        {
            if (_Red < min) _Red = min;
            if (_Green < min) _Green = min;
            if (_Blue < min) _Blue = min;

            if (_Red > max) _Red = max;
            if (_Green > max) _Green = max;
            if (_Blue > max) _Blue = max;
        }
    }
}
