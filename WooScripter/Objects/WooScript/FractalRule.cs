using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooScripter.Objects.WooScript
{
    public abstract class FractalIteration
    {
        public abstract XElement CreateElement();
    };

    class TetraFractalIteration : FractalIteration
    {
        Vector3 _Rotate1;
        Vector3 _Rotate2;
        double _Scale;
        Vector3 _Offset;
        public TetraFractalIteration(Vector3 rotate1, Vector3 rotate2, double scale, Vector3 offset)
        {
            _Rotate1 = rotate1; _Rotate2 = rotate2; _Scale = scale; _Offset = offset;
        }

        override public XElement CreateElement()
        {
            return new XElement("ITERATION",
                new XAttribute("type", 0),
                new XAttribute("rotate1", _Rotate1),
                new XAttribute("rotate2", _Rotate2),
                new XAttribute("scale", _Scale),
                new XAttribute("offset", _Offset));
        }
    };

    class CuboidFractalIteration : FractalIteration
    {
        Vector3 _Rotate1;
        Vector3 _Rotate2;
        double _Scale;
        Vector3 _Offset;
        public CuboidFractalIteration(Vector3 rotate1, Vector3 rotate2, double scale, Vector3 offset)
        {
            _Rotate1 = rotate1; _Rotate2 = rotate2; _Scale = scale; _Offset = offset;
        }

        override public XElement CreateElement()
        {
            return new XElement("ITERATION",
                new XAttribute("type", 1),
                new XAttribute("rotate1", _Rotate1),
                new XAttribute("rotate2", _Rotate2),
                new XAttribute("scale", _Scale),
                new XAttribute("offset", _Offset));
        }
    };

    class MengerFractalIteration : FractalIteration
    {
        Vector3 _Rotate1;
        Vector3 _Rotate2;
        double _Scale;
        Vector3 _Offset;
        public MengerFractalIteration(Vector3 rotate1, Vector3 rotate2, double scale, Vector3 offset)
        {
            _Rotate1 = rotate1; _Rotate2 = rotate2; _Scale = scale; _Offset = offset;
        }

        override public XElement CreateElement()
        {
            return new XElement("ITERATION",
                new XAttribute("type", 2),
                new XAttribute("rotate1", _Rotate1),
                new XAttribute("rotate2", _Rotate2),
                new XAttribute("scale", _Scale),
                new XAttribute("offset", _Offset));
        }
    };

    class MandelBoxFractalIteration : FractalIteration
    {
        Vector3 _Rotate1;
        double _Scale;
        public MandelBoxFractalIteration(Vector3 rotate1, double scale)
        {
            _Rotate1 = rotate1; _Scale = scale;
        }

        override public XElement CreateElement()
        {
            return new XElement("ITERATION",
                new XAttribute("type", 3),
                new XAttribute("rotate1", _Rotate1),
                new XAttribute("scale", _Scale));
        }
    };

    class MandelBulbFractalIteration : FractalIteration
    {
        Vector3 _Rotate1;
        double _Scale;
        public MandelBulbFractalIteration(Vector3 rotate1, double scale)
        {
            _Rotate1 = rotate1; _Scale = scale;
        }

        override public XElement CreateElement()
        {
            return new XElement("ITERATION",
                new XAttribute("type", 4),
                new XAttribute("rotate1", _Rotate1),
                new XAttribute("scale", _Scale));
        }
    };

    class FractalReset : NullFunction
    {
        public void Parse(ref string[] program)
        {
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Clear();
        }

        public string GetSymbol()
        {
            return "fractal_reset";
        }

        public Function CreateNew()
        {
            return new FractalReset();
        }
    }

    class FractalTetra : NullFunction
    {
        Expression _Rotate1;
        Expression _Rotate2;
        Expression _Scale;
        Expression _Offset;

        public void Parse(ref string[] program)
        {
            _Rotate1 = ExpressionBuilder.Parse(ref program);
            if (_Rotate1.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate1 must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Rotate2 = ExpressionBuilder.Parse(ref program);
            if (_Rotate2.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate2 must be of type vector");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Scale = ExpressionBuilder.Parse(ref program);
            if (_Scale.GetExpressionType() != VarType.varFloat)
                throw new ParseException("_Scale must be of type float");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Offset = ExpressionBuilder.Parse(ref program);
            if (_Offset.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Offset must be of type vector");
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Add(new TetraFractalIteration(_Rotate1.EvaluateVector(ref state), _Rotate2.EvaluateVector(ref state), _Scale.EvaluateFloat(ref state), _Offset.EvaluateVector(ref state)));
        }

        public string GetSymbol()
        {
            return "fractal_tetra";
        }

        public Function CreateNew()
        {
            return new FractalTetra();
        }
    }

    class FractalCuboid : NullFunction
    {
        Expression _Rotate1;
        Expression _Rotate2;
        Expression _Scale;
        Expression _Offset;

        public void Parse(ref string[] program)
        {
            _Rotate1 = ExpressionBuilder.Parse(ref program);
            if (_Rotate1.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate1 must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Rotate2 = ExpressionBuilder.Parse(ref program);
            if (_Rotate2.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate2 must be of type vector");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Scale = ExpressionBuilder.Parse(ref program);
            if (_Scale.GetExpressionType() != VarType.varFloat)
                throw new ParseException("_Scale must be of type float");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Offset = ExpressionBuilder.Parse(ref program);
            if (_Offset.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Offset must be of type vector");
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Add(new CuboidFractalIteration(_Rotate1.EvaluateVector(ref state), _Rotate2.EvaluateVector(ref state), _Scale.EvaluateFloat(ref state), _Offset.EvaluateVector(ref state)));
        }

        public string GetSymbol()
        {
            return "fractal_cuboid";
        }

        public Function CreateNew()
        {
            return new FractalCuboid();
        }
    }

    class FractalMenger : NullFunction
    {
        Expression _Rotate1;
        Expression _Rotate2;
        Expression _Scale;
        Expression _Offset;

        public void Parse(ref string[] program)
        {
            _Rotate1 = ExpressionBuilder.Parse(ref program);
            if (_Rotate1.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate1 must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Rotate2 = ExpressionBuilder.Parse(ref program);
            if (_Rotate2.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate2 must be of type vector");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Scale = ExpressionBuilder.Parse(ref program);
            if (_Scale.GetExpressionType() != VarType.varFloat)
                throw new ParseException("_Scale must be of type float");

            comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Offset = ExpressionBuilder.Parse(ref program);
            if (_Offset.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Offset must be of type vector");
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Add(new MengerFractalIteration(_Rotate1.EvaluateVector(ref state), _Rotate2.EvaluateVector(ref state), _Scale.EvaluateFloat(ref state), _Offset.EvaluateVector(ref state)));
        }

        public string GetSymbol()
        {
            return "fractal_menger";
        }

        public Function CreateNew()
        {
            return new FractalMenger();
        }
    }

    class FractalMandelBox : NullFunction
    {
        Expression _Rotate1;
        Expression _Scale;

        public void Parse(ref string[] program)
        {
            _Rotate1 = ExpressionBuilder.Parse(ref program);
            if (_Rotate1.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate1 must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Scale = ExpressionBuilder.Parse(ref program);
            if (_Scale.GetExpressionType() != VarType.varFloat)
                throw new ParseException("_Scale must be of type float");
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Add(new MandelBoxFractalIteration(_Rotate1.EvaluateVector(ref state), _Scale.EvaluateFloat(ref state)));
        }

        public string GetSymbol()
        {
            return "fractal_mandelbox";
        }

        public Function CreateNew()
        {
            return new FractalMandelBox();
        }
    }

    class FractalMandelBulb : NullFunction
    {
        Expression _Rotate1;
        Expression _Scale;

        public void Parse(ref string[] program)
        {
            _Rotate1 = ExpressionBuilder.Parse(ref program);
            if (_Rotate1.GetExpressionType() != VarType.varVector)
                throw new ParseException("_Rotate1 must be of type vector");

            string comma = ParseUtils.GetToken(ref program);
            if (!comma.Equals(",", StringComparison.Ordinal))
                throw new ParseException("Expected \",\"");

            _Scale = ExpressionBuilder.Parse(ref program);
            if (_Scale.GetExpressionType() != VarType.varFloat)
                throw new ParseException("_Scale must be of type float");
        }

        public void Execute(ref WooState state)
        {
            state._FractalIterations.Add(new MandelBulbFractalIteration(_Rotate1.EvaluateVector(ref state), _Scale.EvaluateFloat(ref state)));
        }

        public string GetSymbol()
        {
            return "fractal_mandelbulb";
        }

        public Function CreateNew()
        {
            return new FractalMandelBulb();
        }
    }

    class FractalRule : Rule
    {
        public FractalRule(string name)
            : base(name)
        {
        }

        public override string GetHelpText()
        {
            return "fractal - Create an iterative fractal object";
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

                Fractal newFractal = new Fractal(new Vector3(state._Position.x + val.x, state._Position.y + val.y, state._Position.z + val.z),
                    state._Scale * 0.5,
                    state._Rotation,
                    state._DistanceMinimum,
                    state._DistanceScale,
                    state._DistanceOffset,
                    state._DistanceIterations,
                    state._DistanceExtents,
                    state._StepSize,
                    state._FractalIterations);
                newFractal._Material = GenerateMaterial(state);
                newFractal.CreateElement(state._Preview, state._Parent);
            }
        }
    };
}
