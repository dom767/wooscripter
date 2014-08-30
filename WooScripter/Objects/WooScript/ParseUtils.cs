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
            char[] whitespace = new char[] { ' ' };
            char[] specialChars = new char[] { ',', '(', ')', '{', '}' };
            char[] opChars = new char[] { '/', '*', '<', '>', '|', '&', '-', '+', '=' };

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
            int tokenOp = lines[0].IndexOfAny(opChars);

            if (tokenEnd == -1) tokenEnd = lines[0].Length;
            if (tokenSpecial == -1) tokenSpecial = lines[0].Length;
            if (tokenOp == -1) tokenOp = lines[0].Length;

            int tokenquote = lines[0].IndexOf("\"");
            if (tokenquote == 0)
            {
                tokenEnd = 1+lines[0].IndexOf("\"", 1);
            }
            else
            {
                if (tokenSpecial == 0)
                {
                    tokenEnd = 1;
                }
                else if (tokenOp == 0)
                {
                    int length = 0;
                    for (int i = 0; i < WooScript.GetNumOps(); i++)
                    {
                        string opName = WooScript.GetOp(i);
                        if (lines[0].IndexOf(opName) == 0)
                        {
                            if (opName.Length > length)
                                length = opName.Length;
                        }
                    }
                    if (lines[0].IndexOf("<") == 0
                        || lines[0].IndexOf(">") == 0)
                        length = 1;
                    if (lines[0].IndexOf("!=") == 0)
                        length = 2;
                    // nah
                    tokenEnd = length;
                }
                else
                {
                    if (tokenSpecial < tokenEnd) tokenEnd = tokenSpecial;
                    if (tokenOp < tokenEnd) tokenEnd = tokenOp;
                }
            }

            if (tokenEnd == lines[0].Length)
            {
                token = lines[0];
                lines[0] = "";
            }
            else
            {
                token = lines[0].Substring(0, tokenEnd);
                lines[0] = lines[0].Substring(tokenEnd);
            }

            if (tokenquote == 0)
                token = token.Substring(1, token.Length - 2);

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
