using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class ParseUtils
    {
        public static string GetToken(ref string[] lines)
        {
            char[] whitespace = new char[] { ' ', ',' };
            char[] specialChars = new char[] { '(', ')', '{', '}' };
            do
            {
                if (lines[0].Length == 0 || lines[0].IndexOf("//") == 0)
                {
                    lines = lines.Where((val, idx) => idx != 0).ToArray();
                    if (lines.Length == 0)
                    {
                        return "";
                    }
                }
                lines[0] = lines[0].TrimStart(whitespace);
            }
            while (lines[0].Length==0 || lines[0].IndexOf("//")==0);

            string token;

            int tokenEnd = lines[0].IndexOfAny(whitespace);
            int tokenSpecial = lines[0].IndexOfAny(specialChars);
            if (tokenSpecial != 0)
            {
                if (tokenEnd == -1)
                    tokenEnd = tokenSpecial;
                else if ((tokenEnd > tokenSpecial) && (tokenSpecial!=-1))
                    tokenEnd = tokenSpecial;
            }
            else
            {
                tokenEnd = 1;
            }

            if (tokenEnd == -1)
            {
                token = lines[0];
                lines[0] = "";
            }
            else
            {
                token = lines[0].Substring(0, tokenEnd);
                lines[0] = lines[0].Substring(tokenEnd);
            }

            return token;
        }

        public static string PeekToken(string[] lines)
        {
            string[] tempLines = lines.Clone() as string[];
            string token = ParseUtils.GetToken(ref tempLines);
            return token;
        }
    }
}
