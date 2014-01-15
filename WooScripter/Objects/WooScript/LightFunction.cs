using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    class DirectionalLightFunction : NullFunction
    {
        Expression _ColourExpr;
        Expression _DirectionExpr;
        Expression _AreaExpr;
        Expression _SamplesExpr;

        public void Parse(ref string[] program)
        {
            _ColourExpr = ExpressionBuilder.Parse(ref program);
            if (_ColourExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed directional light colour");

            _DirectionExpr = ExpressionBuilder.Parse(ref program);
            if (_DirectionExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed directional light direction");

            _AreaExpr = ExpressionBuilder.Parse(ref program);
            if (_AreaExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("malformed directional light area");

            _SamplesExpr = ExpressionBuilder.Parse(ref program);
            if (_SamplesExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("malformed directional light samples");
        }

        public void Execute(ref WooState state)
        {
            Vector3 colourVec = _ColourExpr.EvaluateVector(ref state);
            Colour colour = new Colour(colourVec.x, colourVec.y, colourVec.z);
            colour.Clamp(0, 1);

            Vector3 direction = _DirectionExpr.EvaluateVector(ref state);

            double area = _AreaExpr.EvaluateFloat(ref state);
            if (area < 0) area = 0;
            if (area > 0.99) area = 0.99;

            double samples = _SamplesExpr.EvaluateFloat(ref state);
            if (samples < 1) samples = 1;

            DirectionalLight directionalLight = new DirectionalLight(colour, direction, (float)area, (int)samples);
            directionalLight.CreateElement(state._Parent, new Vector3(0,0,0));
        }

        public string GetSymbol()
        {
            return "directionalLight";
        }

        public Function CreateNew()
        {
            return new DirectionalLightFunction();
        }
    }

    class PointLightFunction : NullFunction
    {
        Expression _ColourExpr;
        Expression _PositionExpr;

        public void Parse(ref string[] program)
        {
            _ColourExpr = ExpressionBuilder.Parse(ref program);
            if (_ColourExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed point light colour");

            _PositionExpr = ExpressionBuilder.Parse(ref program);
            if (_PositionExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed point light position");
        }

        public void Execute(ref WooState state)
        {
            Vector3 colourVec = _ColourExpr.EvaluateVector(ref state);
            Colour colour = new Colour(colourVec.x, colourVec.y, colourVec.z);

            Vector3 position = _PositionExpr.EvaluateVector(ref state);

            PointLight pointLight = new PointLight(colour, position);
            pointLight.CreateElement(state._Parent, position);
        }

        public string GetSymbol()
        {
            return "pointLight";
        }

        public Function CreateNew()
        {
            return new PointLightFunction();
        }
    }

    class AmbientLightFunction : NullFunction
    {
        Expression _ColourExpr;

        public void Parse(ref string[] program)
        {
            _ColourExpr = ExpressionBuilder.Parse(ref program);
            if (_ColourExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed ambient light colour");
        }

        public void Execute(ref WooState state)
        {
            Vector3 colourVec = _ColourExpr.EvaluateVector(ref state);
            Colour colour = new Colour(colourVec.x, colourVec.y, colourVec.z);

            AmbientLight ambientLight = new AmbientLight(colour);
            ambientLight.CreateElement(state._Parent, new Vector3(0,0,0));
        }

        public string GetSymbol()
        {
            return "ambientLight";
        }

        public Function CreateNew()
        {
            return new AmbientLightFunction();
        }
    }

    class WorldLightFunction : NullFunction
    {
        Expression _ColourExpr;
        Expression _SamplesExpr;

        public void Parse(ref string[] program)
        {
            _ColourExpr = ExpressionBuilder.Parse(ref program);
            if (_ColourExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed world light colour");

            _SamplesExpr = ExpressionBuilder.Parse(ref program);
            if (_SamplesExpr.GetExpressionType() != VarType.varFloat)
                throw new ParseException("malformed world light samples");
        }

        public void Execute(ref WooState state)
        {
            Vector3 colourVec = _ColourExpr.EvaluateVector(ref state);
            Colour colour = new Colour(colourVec.x, colourVec.y, colourVec.z);

            double samples = _SamplesExpr.EvaluateFloat(ref state);
            if (samples < 1) samples = 1;

            WorldLight worldLight = new WorldLight(colour, (int)samples);
            worldLight.CreateElement(state._Parent, new Vector3(0, 0, 0));
        }

        public string GetSymbol()
        {
            return "worldLight";
        }

        public Function CreateNew()
        {
            return new WorldLightFunction();
        }
    }

    class BackgroundFunction : NullFunction
    {
        Expression _ColourExpr;

        public void Parse(ref string[] program)
        {
            _ColourExpr = ExpressionBuilder.Parse(ref program);
            if (_ColourExpr.GetExpressionType() != VarType.varVector)
                throw new ParseException("malformed background function");
        }

        public void Execute(ref WooState state)
        {
            Vector3 colourVec = _ColourExpr.EvaluateVector(ref state);
            Colour colour = new Colour(colourVec.x, colourVec.y, colourVec.z);

            Background background = new Background();
            background._BackgroundColour = colour;
            background._Simple = true;
            background.CreateElement(state._Parent);
        }

        public string GetSymbol()
        {
            return "background";
        }

        public Function CreateNew()
        {
            return new BackgroundFunction();
        }
    }
}
