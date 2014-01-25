using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows;

namespace WooScripter.Objects.WooScript
{
    public enum VarType
    {
        varFloat,
        varVector,
        varUnknown
    }

    public enum TokenType
    {
        assignOp,
        Op,
        vecFunction,
        floatFunction,
        nullFunction,
        vecVar,
        floatVar,
        floatNum,
        unknown
    }

    [Serializable]
    public class WooScript : RenderObject
    {
        public string _Program;
        public List<Rule> _Rules = new List<Rule>();
        public static List<Op> _Operators = new List<Op>();
        public static List<AssignOp> _AssignOperators = new List<AssignOp>();
        public static List<string> _VecVariables = new List<string>();
        public static List<string> _FloatVariables = new List<string>();
        public static List<NullFunction> _NullFunctions = new List<NullFunction>();
        public static List<VecFunction> _VecFunctions = new List<VecFunction>();
        public static List<FloatFunction> _FloatFunctions = new List<FloatFunction>();
        public static Log _Log = new Log();
        public float _Width = 10;
        public float _Height = 10;

        // Save the program using a configurable location
        public void SaveUserInput(string subfolder)
        {
            // create folder if it don't exist like
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Scripts" + "\\" + subfolder;
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = subfolder + "script"; // Default file name
            dlg.DefaultExt = ".woo"; // Default file extension
            dlg.Filter = "WooScript|*.woo"; // Filter files by extension
            dlg.InitialDirectory = store;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName;
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(_Program);
                sw.Close();
            }
        }

        // name is the name of the file, no path, NO EXTENSION
        public void Save(string subfolder, string name)
        {
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Scripts";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            store = store + "\\" + subfolder;
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            string filename = store + "\\" + name + ".woo";
            StreamWriter sw = new StreamWriter(filename);
            sw.Write(_Program);
            sw.Close();
        }

        public void LoadUserInput(string subfolder)
        {
            string store = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WooScripter\\Scripts\\" + subfolder;

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = subfolder + "script"; // Default file name
            dlg.DefaultExt = ".woo"; // Default file extension
            dlg.Filter = "WooScript|*.woo"; // Filter files by extension
            dlg.InitialDirectory = store;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // get name of file
            if (result == true)
            {
                string filename = dlg.FileName;
                StreamReader sr = new StreamReader(filename);
                _Program = sr.ReadToEnd();
            }
        }

        public void Load(string subfolder, string name)
        {
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Scripts\\"+ subfolder + "\\" + name + ".woo";
            if (System.IO.File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                _Program = sr.ReadToEnd();
                sr.Close();
            }
        }

        public WooScript()
        {
        }

        public static bool IsAssignOp(string token)
        {
            foreach(AssignOp assignOp in _AssignOperators)
            {
                if (assignOp.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsOp(string token)
        {
            foreach (Op op in _Operators)
            {
                if (op.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNullFunction(string token)
        {
            foreach (Function nf in _NullFunctions)
            {
                if (nf.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsVecVariable(string token)
        {
            int dotPosition = token.IndexOf('.');
            if (dotPosition > -1)
            {
                string vecname = token.Substring(0, dotPosition);
                string selector = token.Substring(dotPosition + 1);
                foreach (string vv in _VecVariables)
                {
                    if (vv.Equals(vecname, StringComparison.Ordinal))
                    {
                        if (selector.Length==3)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (string vv in _VecVariables)
                {
                    if (vv.Equals(token, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsFloatVariable(string token)
        {
            // plain float variable (i.e. "power")
            foreach (string fv in _FloatVariables)
            {
                if (fv.Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            int dotPosition = token.IndexOf('.');
            if (dotPosition > -1)
            {
                string vecname = token.Substring(0, dotPosition);
                string selector = token.Substring(dotPosition + 1);
                foreach (string vv in _VecVariables)
                {
                    if (vv.Equals(vecname, StringComparison.Ordinal))
                    {
                        if (selector.Equals("x", StringComparison.Ordinal)
                            || selector.Equals("y", StringComparison.Ordinal)
                            || selector.Equals("z", StringComparison.Ordinal)
                            || selector.Equals("r", StringComparison.Ordinal)
                            || selector.Equals("g", StringComparison.Ordinal)
                            || selector.Equals("b", StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsFloatNumber(string token)
        {
            double result;
            int sepindex = token.IndexOf(':');
            if (sepindex == -1)
                sepindex = token.IndexOf('|');
            if (sepindex != -1)
            {
                string num1, num2;
                num1 = token.Substring(0, sepindex);
                num2 = token.Substring(sepindex + 1);
                bool success = double.TryParse(num1, out result);
                if (success)
                    success = double.TryParse(num1, out result);
                if (success)
                    return true;

            }
            else
            {
                bool success = double.TryParse(token, out result);
                if (success)
                    return true;
            }

            return false;
        }

        public static bool IsVecFunction(string token)
        {
            foreach (VecFunction vf in _VecFunctions)
            {
                if (vf.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsFloatFunction(string token)
        {
            foreach (FloatFunction ff in _FloatFunctions)
            {
                if (ff.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static TokenType GetTokenType(string token)
        {
            TokenType ret = TokenType.unknown;
            int matches=0;

            if (IsNullFunction(token))
            {
                matches++;
                ret = TokenType.nullFunction;
            }
            if (IsFloatVariable(token))
            {
                matches++;
                ret = TokenType.floatVar;
            }
            if (IsVecVariable(token))
            {
                matches++;
                ret = TokenType.vecVar;
            }
            if (IsOp(token))
            {
                matches++;
                ret = TokenType.Op;
            }
            if (IsFloatNumber(token))
            {
                matches++;
                ret = TokenType.floatNum;
            }
            if (IsVecFunction(token))
            {
                matches++;
                ret = TokenType.vecFunction;
            }
            if (IsFloatFunction(token))
            {
                matches++;
                ret = TokenType.floatFunction;
            }
            /*            if (IsAssignOp(token))
                        {
                            matches++;
                            ret = TokenType.assignOp;
                        }
                        if (IsVecVariable(token))
                        {
                            matches++;
                            ret = TokenType.vecVar;
                        }
                        if (IsFloatFunction(token))
                        {
                            matches++;
                            ret = TokenType.floatFunction;
                        }
            */
            if (matches > 1)
                throw new ParseException("Name clash for token \"" + token + "\"");

            return ret;
        }

        static void AddOperators()
        {
            _AssignOperators.Add(new SetDivOp());
            _AssignOperators.Add(new SetMulOp());
            _AssignOperators.Add(new SetAddOp());
            _AssignOperators.Add(new SetSubOp());
            _AssignOperators.Add(new SetOp());

            _Operators.Add(new DivOp());
            _Operators.Add(new MulOp());
            _Operators.Add(new AddOp());
            _Operators.Add(new SubOp());
        }

        static void AddVariables()
        {
            _VecVariables.Add("pos");
            _VecVariables.Add("diff");
            _VecVariables.Add("refl");
            _VecVariables.Add("emi");
            _VecVariables.Add("spec");
            _VecVariables.Add("scale");
            _VecVariables.Add("v0");
            _VecVariables.Add("v1");
            _VecVariables.Add("v2");
            _VecVariables.Add("v3");

            _FloatVariables.Add("gloss");
            _FloatVariables.Add("power");
            _FloatVariables.Add("recursions");
            _FloatVariables.Add("rx");
            _FloatVariables.Add("ry");
            _FloatVariables.Add("rz");
        }

        public static NullFunction GetNullFunction(string name)
        {
            foreach (Function fun in _NullFunctions)
            {
                if (fun.GetSymbol().Equals(name, StringComparison.Ordinal))
                {
                    return fun.CreateNew() as NullFunction;
                }
            }
            
            throw new ParseException("No matching function found \"" + name + "\"");
        }

        public static FloatFunction GetFloatFunction(string name)
        {
            foreach (Function fun in _FloatFunctions)
            {
                if (fun.GetSymbol().Equals(name, StringComparison.Ordinal))
                {
                    return fun.CreateNew() as FloatFunction;
                }
            }

            throw new ParseException("No matching function found \"" + name + "\"");
        }

        public static VecFunction GetVecFunction(string name)
        {
            foreach (Function fun in _VecFunctions)
            {
                if (fun.GetSymbol().Equals(name, StringComparison.Ordinal))
                {
                    return fun.CreateNew() as VecFunction;
                }
            }

            throw new ParseException("No matching function found \"" + name + "\"");
        }

        public static AssignOp GetAssignOp(string token)
        {
            foreach (AssignOp assignOp in _AssignOperators)
            {
                if (assignOp.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return assignOp.CreateNew() as AssignOp;
                }
            }

            throw new ParseException("No matching assign operation found \"" + token + "\"");
        }

        public static Op GetOp(string token)
        {
            foreach (Op Op in _Operators)
            {
                if (Op.GetSymbol().Equals(token, StringComparison.Ordinal))
                {
                    return Op.CreateNew() as Op;
                }
            }

            throw new ParseException("No matching assign operation found \"" + token + "\"");
        }

        static void AddFunctions()
        {
            _NullFunctions.Add(new CallFunction());
            _NullFunctions.Add(new RepeatFunction());
            _NullFunctions.Add(new BranchFunction());
            _NullFunctions.Add(new FinalCallFunction());
            _NullFunctions.Add(new PushFunction());
            _NullFunctions.Add(new PopFunction());
            _NullFunctions.Add(new DirectionalLightFunction());
            _NullFunctions.Add(new PointLightFunction());
            _NullFunctions.Add(new WorldLightFunction());
            _NullFunctions.Add(new AmbientLightFunction());
            _NullFunctions.Add(new BackgroundFunction());

            _VecFunctions.Add(new VectorConstructorFunction());
            _VecFunctions.Add(new VectorNormaliseFunction());

            _FloatFunctions.Add(new CosFloatFunction());

/*            _NullFunctions.Add("final");
            _NullFunctions.Add("repeat");
            _NullFunctions.Add("branch");
            _NullFunctions.Add("push");
            _NullFunctions.Add("pop");

            _FloatFunctions.Add("cosf");
            _FloatFunctions.Add("sinf");
            _FloatFunctions.Add("clampf");

            _VecFunctions.Add("cos");
            _VecFunctions.Add("sin");
            _VecFunctions.Add("clamp");
*/        }

        void AddStandardRules()
        {
            Rule boxRule = new BoxRule("box");
            _Rules.Add(boxRule);
            Rule sphereRule = new SphereRule("sphere");
            _Rules.Add(sphereRule);
            Rule circleRule = new CircleRule("circle");
            _Rules.Add(circleRule);
            Rule sphereLightRule = new SphereLightRule("spherelight");
            _Rules.Add(sphereLightRule);
            Rule cylinderRule = new CylinderRule("cylinder");
            _Rules.Add(cylinderRule);
        }

        public string GetHelpText()
        {
            AddStandardRules();

            StringBuilder strbuilder = new StringBuilder();

            strbuilder.AppendLine("Rules : ");
            foreach (Rule rule in _Rules)
            {
                strbuilder.AppendLine(rule.GetHelpText());
            }
            strbuilder.AppendLine("");

            strbuilder.AppendLine("Vector variables : ");
            foreach (string vv in _VecVariables)
            {
                strbuilder.AppendLine(vv);
            }
            strbuilder.AppendLine("");

            strbuilder.AppendLine("Float variables : ");
            foreach (string fv in _FloatVariables)
            {
                strbuilder.AppendLine(fv);
            }
            strbuilder.AppendLine("");

            strbuilder.AppendLine("Functions (null return) : ");
            foreach (NullFunction nf in _NullFunctions)
            {
                strbuilder.AppendLine(nf.GetSymbol());
            }
            strbuilder.AppendLine("");

            strbuilder.AppendLine("Functions (float return) : ");
            foreach (FloatFunction ff in _FloatFunctions)
            {
                strbuilder.AppendLine(ff.GetSymbol());
            }
            strbuilder.AppendLine("");

            strbuilder.AppendLine("Functions (vector return) : ");
            foreach (VecFunction vf in _VecFunctions)
            {
                strbuilder.AppendLine(vf.GetSymbol());
            }

            return strbuilder.ToString();
        }

        public void Reset()
        {
            _Rules.Clear();
            _Operators.Clear();
            _AssignOperators.Clear();
            _VecVariables.Clear();
            _FloatVariables.Clear();
            _NullFunctions.Clear();
            _VecFunctions.Clear();
            _FloatFunctions.Clear();
            AddStandardRules();
            AddOperators();
            AddVariables();
            AddFunctions();
            _Log.Clear();
        }

        public void ParseProgram(ref string[] program)
        {
            string token;

            do
            {
                token = ParseUtils.GetToken(ref program);

                if (string.Compare(token, "rule", true) == 0)
                {
                    string rulename = ParseUtils.GetToken(ref program);
                    _Log.AddMsg("Found rule " + rulename);
                    Rule newRule = new Rule(rulename);
                    _Log.Indent();
                    newRule.Parse(ref program);
                    _Log.UnIndent();
                    _Rules.Add(newRule);
                }
                else
                {
                    if (token.Length>0)
                        throw new ParseException("Global statements must start with \"rule\".");
                }
            }
            while (token.Length > 0);

            // rule rulename { statement* }
            // statement = floatvar aop floatrhs | vecvar aop vecexpr | nfun ( args )
            // args = (rulename|floatexpr|vecexpr)*
            // floatexpr = ( floatexpr ) | num op floatexpr | ffun(args) op floatexpr | num | ffun(args) | floatvar
            // num = 0.x | vec.sel
            // vecexpr = Vec(floatexpr, floatexpr, floatexpr) | vec op vecexpr | vfun ( args )
            // vec = vec(x,y,z) | vec(x,y,z).rep
        }

        bool _ParseSuccessful = false;

        public bool Parse(ref string Log, ref string Error)
        {
            Reset();

            string programData = _Program.Clone() as string;

            string[] seperator = new string[] {"\r\n"};
            string[] programLines = programData.Split(seperator, StringSplitOptions.None);

            int initialLines = programLines.Length;

            try
            {
                ParseProgram(ref programLines);

                _ParseSuccessful = true;
                _Log.AddMsg("Parsing Successful");
                Log = _Log.GetLog();
            }
            catch (ParseException e)
            {
                _ParseSuccessful = false;
                Error = "*ERROR* Line : " + (initialLines - programLines.Length) + " : " + e._WooMessage;
                return false;
            }
            return true;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            if (!_ParseSuccessful)
                return;

            try
            {
                foreach (Rule r in _Rules)
                {
                    if (r._Name.Equals("main", StringComparison.InvariantCultureIgnoreCase))
                    {
                        WooState state = new WooState();
                        state._Rules = _Rules;
                        state._Random = new Random(10);//_Seed);
                        state._Parent = parent;
                        state._Preview = preview;
                        r.Execute(ref state);
                    }
                }
            }
            catch (EvaluateException e)
            {
                MessageBox.Show(e._WooMessage);
            }
        }
    }
}
