using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace WooScripter.Objects.WooScript
{/*
    class AdjustOp : Op
    {
        public string _Value;
        public Number _Number;
        public Number _Number2;
        public Number _Number3;
        public int _ArgCount;

        public void Parse(ref string[] program)
        {/*
            _Value = ParseUtils.GetToken(ref data);
            log.AddMsg("Adjusting value : " + _Value);
            _Number = new Number();
            _Number.Parse(ParseUtils.GetToken(ref data), log);
            _ArgCount = 1;
            int pos = data.IndexOf(',');
            if (data.IndexOf(',') >= 0)
            {
                _Number2 = new Number();
                _Number2.Parse(ParseUtils.GetToken(ref data), log);
                _ArgCount++;
            }
            if (data.IndexOf(',') >= 0)
            {
                _Number3 = new Number();
                _Number3.Parse(ParseUtils.GetToken(ref data), log);
                _ArgCount++;
            }*//*
        }
        
        public void Execute(ref WooState state)
        {/*
            Vector3 vec = new Vector3(0, 0, 0);
            if (_Value.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                vec.x = _Number.GetNewNumber(state._X, state._Random);
            if (_Value.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                vec.y = _Number.GetNewNumber(state._Y, state._Random);
            if (_Value.Equals("z", StringComparison.InvariantCultureIgnoreCase))
                vec.z = _Number.GetNewNumber(state._Z, state._Random);

            vec.x = vec.x * state._Scale.x;
            vec.y = vec.y * state._Scale.y;
            vec.z = vec.z * state._Scale.z;
            
            vec.Mul(state._Rotation);
            state._X += vec.x;
            state._Y += vec.y;
            state._Z += vec.z;

            if (_Value.Equals("rx", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rX, state._Random);
                state._rX += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(delta * 2 * 3.141592 / 360, 0, 0);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            }
            if (_Value.Equals("ry", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rY, state._Random);
                state._rY += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(0, delta * 2 * 3.141592 / 360, 0);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            } 
            if (_Value.Equals("rz", StringComparison.InvariantCultureIgnoreCase))
            {
                double delta = _Number.GetNewNumber(state._rZ, state._Random);
                state._rZ += delta;
                Matrix3 rotMatrix = new Matrix3();
                rotMatrix.MakeFromRPY(0, 0, delta * 2 * 3.141592 / 360);
                rotMatrix.Mul(state._Rotation);
                state._Rotation = rotMatrix;
            }

            if (_Value.Equals("diff", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Diff._Red += _Number.GetNewNumber(state._Diff._Red, state._Random);
                state._Diff._Green += _Number2.GetNewNumber(state._Diff._Green, state._Random);
                state._Diff._Blue += _Number3.GetNewNumber(state._Diff._Blue, state._Random);
            }
            if (_Value.Equals("diffR", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Red += _Number.GetNewNumber(state._Diff._Red, state._Random);
            if (_Value.Equals("diffG", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Green += _Number.GetNewNumber(state._Diff._Green, state._Random);
            if (_Value.Equals("diffB", StringComparison.InvariantCultureIgnoreCase))
                state._Diff._Blue += _Number.GetNewNumber(state._Diff._Blue, state._Random);

            if (_Value.Equals("refl", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Refl._Red += _Number.GetNewNumber(state._Refl._Red, state._Random);
                state._Refl._Green += _Number2.GetNewNumber(state._Refl._Green, state._Random);
                state._Refl._Blue += _Number3.GetNewNumber(state._Refl._Blue, state._Random);
            }
            if (_Value.Equals("reflR", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Red += _Number.GetNewNumber(state._Refl._Red, state._Random);
            if (_Value.Equals("reflG", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Green += _Number.GetNewNumber(state._Refl._Green, state._Random);
            if (_Value.Equals("reflB", StringComparison.InvariantCultureIgnoreCase))
                state._Refl._Blue += _Number.GetNewNumber(state._Refl._Blue, state._Random);

            if (_Value.Equals("emi", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Emi._Red += _Number.GetNewNumber(state._Emi._Red, state._Random);
                state._Emi._Green += _Number2.GetNewNumber(state._Emi._Green, state._Random);
                state._Emi._Blue += _Number3.GetNewNumber(state._Emi._Blue, state._Random);
            }
            if (_Value.Equals("emiR", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Red += _Number.GetNewNumber(state._Emi._Red, state._Random);
            if (_Value.Equals("emiG", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Green += _Number.GetNewNumber(state._Emi._Green, state._Random);
            if (_Value.Equals("emiB", StringComparison.InvariantCultureIgnoreCase))
                state._Emi._Blue += _Number.GetNewNumber(state._Emi._Blue, state._Random);

            if (_Value.Equals("spec", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Spec._Red += _Number.GetNewNumber(state._Spec._Red, state._Random);
                state._Spec._Green += _Number2.GetNewNumber(state._Spec._Green, state._Random);
                state._Spec._Blue += _Number3.GetNewNumber(state._Spec._Blue, state._Random);
            }
            if (_Value.Equals("specR", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Red += _Number.GetNewNumber(state._Spec._Red, state._Random);
            if (_Value.Equals("specG", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Green += _Number.GetNewNumber(state._Spec._Green, state._Random);
            if (_Value.Equals("specB", StringComparison.InvariantCultureIgnoreCase))
                state._Spec._Blue += _Number.GetNewNumber(state._Spec._Blue, state._Random);

            if (_Value.Equals("power", StringComparison.InvariantCultureIgnoreCase))
                state._Power += (float)_Number.GetNewNumber((double)state._Power, state._Random);

            if (_Value.Equals("scale", StringComparison.InvariantCultureIgnoreCase))
            {
                state._Scale.x += _Number.GetNewNumber(state._Scale.x, state._Random);
                state._Scale.y += _Number2.GetNewNumber(state._Scale.y, state._Random);
                state._Scale.z += _Number3.GetNewNumber(state._Scale.z, state._Random);
            }
            if (_Value.Equals("scaleX", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.x += _Number.GetNewNumber(state._Scale.x, state._Random);
            if (_Value.Equals("scaleY", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.y += _Number.GetNewNumber(state._Scale.y, state._Random);
            if (_Value.Equals("scaleZ", StringComparison.InvariantCultureIgnoreCase))
                state._Scale.z += _Number.GetNewNumber(state._Scale.z, state._Random);

            if (_Value.Equals("recursions", StringComparison.InvariantCultureIgnoreCase))
                state._Recursions += (int)(0.5+_Number.GetNewNumber((double)state._Recursions, state._Random));
        *//*}
    }
    */



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
        }

        public void Reset()
        {
            _Rules.Clear();
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

        public bool Parse(ref string Log)
        {
            Reset();

            try
            {
                string programData = _Program.Clone() as string;

                string[] seperator = new string[] {"\r\n"};
                string[] programLines = programData.Split(seperator, StringSplitOptions.None);
                
                ParseProgram(ref programLines);

                _ParseSuccessful = true;
                _Log.AddMsg("Parsing Successful");
                Log = _Log.GetLog();
            }
            catch (ParseException e)
            {
                _ParseSuccessful = false;
                _Log.AddMsg("*ERROR* Parse exception : " + e._WooMessage);
                Log = _Log.GetLog();
                return false;
            }
            return true;
        }

        public void CreateElement(bool preview, XElement parent)
        {
            if (!_ParseSuccessful)
                return;

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
    }
}
