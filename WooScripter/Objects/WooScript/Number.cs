using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class Number
    {
        enum RangeTypeT
        {
            Continuous, Binary, Single
        }

        RangeTypeT _RangeType;
        float val1, val2;
        
        public void Parse(string data, Log log)
        {
            if (data.IndexOf(':') > 0)
            {
                _RangeType = RangeTypeT.Continuous;
                log.AddMsg("Number type : Continuous range");
                int opPos = data.IndexOf(':');
                val1 = float.Parse(data.Substring(0, opPos), CultureInfo.InvariantCulture);
                val2 = float.Parse(data.Substring(opPos + 1), CultureInfo.InvariantCulture);
                log.AddMsg("Val1 : " + val1.ToString(CultureInfo.InvariantCulture));
                log.AddMsg("Val2 : " + val2.ToString(CultureInfo.InvariantCulture));
            }
            else if (data.IndexOf('|') > 0)
            {
                _RangeType = RangeTypeT.Binary;
                log.AddMsg("Number type : Binary Option");
                int opPos = data.IndexOf('|');
                val1 = float.Parse(data.Substring(0, opPos), CultureInfo.InvariantCulture);
                val2 = float.Parse(data.Substring(opPos + 1), CultureInfo.InvariantCulture);
                log.AddMsg("Val1 : " + val1.ToString(CultureInfo.InvariantCulture));
                log.AddMsg("Val2 : " + val2.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _RangeType = RangeTypeT.Single;
                log.AddMsg("Number type : Single Value");
                val1 = float.Parse(data, CultureInfo.InvariantCulture);
                log.AddMsg("Val1 : " + val1.ToString(CultureInfo.InvariantCulture));
            }
        }

        public double GetNumber(Random rand)
        {
            double output = 0;
            double randVal = rand.NextDouble();

            switch (_RangeType)
            {
                case RangeTypeT.Single:
                    output = val1;
                    break;
                case RangeTypeT.Binary:
                    if (randVal > 0.5)
                        output = val1;
                    else
                        output = val2;
                    break;
                case RangeTypeT.Continuous:
                    output = ((val2 - val1) * randVal) + val1;
                    break;
            }

            return output;
        }
    }
}
