using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Calculator
{
    public partial class Solver
    {
        public static List<EquationToken> CloneTokenList(List<EquationToken> og)
        {
            List<EquationToken> newlist = new List<EquationToken>(og.Count);

            foreach (EquationToken x in og)
                newlist.Add(x.Clone());

            return newlist;
        }

        public class EquationToken
        {
            public override string ToString()
            {
                return "<Default>";
            }
            public EquationToken Clone()
            {
                return (EquationToken)this.MemberwiseClone();
            }
            public override bool Equals(object obj)
            {
                return EqualsA((EquationToken)this, (EquationToken)obj);
            }

            private bool EqualsA(EquationToken x, EquationToken y)
            {
                //Console.WriteLine("Bruh");
                if (x == null || y == null)
                    return false;

                if (!x.GetType().FullName.Equals(y.GetType().FullName))
                    return false;



                if (x is NumberToken)
                {
                    if (((NumberToken)x).value != ((NumberToken)y).value)
                        return false;
                }
                else if (x is OperatorToken)
                {
                    if (((OperatorToken)x).op != ((OperatorToken)y).op)
                        return false;
                }
                else if (x is FunctionSplitterToken)
                {
                    if (((FunctionSplitterToken)x).type != ((FunctionSplitterToken)y).type)
                        return false;
                }
                else if (x is ErrorToken)
                {
                    if (!((ErrorToken)x).message.Equals(((ErrorToken)y).message))
                        return false;
                }
                else if (x is VariableToken)
                {
                    if (((VariableToken)x).varname != ((VariableToken)y).varname)
                        return false;
                }
                else if (x is VariableorFunctionToken)
                {
                    if (((VariableorFunctionToken)x).name != ((VariableorFunctionToken)y).name)
                        return false;
                }


                else if (x is BracketToken)
                {
                    EquationToken[] xarr = ((BracketToken)x).data;
                    EquationToken[] yarr = ((BracketToken)y).data;
                    if (xarr.Length != yarr.Length)
                        return false;

                    for (int i = 0; i < xarr.Length; i++)
                        if (!xarr[i].Equals(yarr[i]))
                            return false;

                    return true;
                }


                else if (x is FunctionToken)
                {
                    if (!((FunctionToken)x).name.Equals(((FunctionToken)y).name))
                        return false;

                    EquationToken[] xarr = ((FunctionToken)x).args;
                    EquationToken[] yarr = ((FunctionToken)y).args;
                    if (xarr.Length != yarr.Length)
                        return false;

                    for (int i = 0; i < xarr.Length; i++)
                        if (!xarr[i].Equals(yarr[i]))
                            return false;

                    return true;
                }



                return true;
            }
        }

        public class NumberToken : EquationToken
        {
            public double value;
            public override string ToString()
            {
                //return $"<Value: {value}>";
                return $"{value.ToString(CultureInfo.InvariantCulture)}";
            }
            public NumberToken(double value)
            {
                this.value = value;
            }
        }

        public class ErrorToken : EquationToken
        {
            public string message;
            public override string ToString()
            {
                return $"<ERROR: {message}>";
            }
            public ErrorToken(string message)
            {
                this.message = message;
            }
        }

        public class EmptyToken : EquationToken
        {
            public override string ToString()
            {
                return "";
            }
        }

        public class VariableToken : EquationToken
        {
            public string varname;
            public override string ToString()
            {
                return $"<Var \"{varname}\": {GetVar(varname).ToString()}>";
            }
            public VariableToken(string varname)
            {
                this.varname = varname;
            }
        }

        public class OperatorToken : EquationToken
        {
            public enum Operation
            {
                PLUS,
                MINUS,
                MULTIPLY,
                DIVIDE,
                MOD,
                POWER,
                SET,
                VAL,
                UNKNOWN
            }

            public static Operation GetOperationFromStr(string chr)
            {
                if (chr.Equals("+"))
                    return Operation.PLUS;
                else if (chr.Equals("-"))
                    return Operation.MINUS;
                else if (chr.Equals("*"))
                    return Operation.MULTIPLY;
                else if (chr.Equals("/"))
                    return Operation.DIVIDE;
                else if (chr.Equals("%"))
                    return Operation.MOD;
                else if (chr.Equals("**") || chr.Equals("^"))
                    return Operation.POWER;
                else if (chr.Equals("->"))
                    return Operation.SET;
                else if (chr.Equals("$"))
                    return Operation.VAL;



                return Operation.UNKNOWN;
            }

            public override string ToString()
            {
                return $"<Operator: {op}>";
            }

            public Operation op;
            public OperatorToken(Operation op)
            {
                this.op = op;
            }
        }

        public class VariableorFunctionToken : EquationToken
        {
            public string name;
            public override string ToString()
            {
                return $"<Var/Function: {name}>";
            }
            public VariableorFunctionToken(string name)
            {
                this.name = name;
            }
        }

        public class FunctionSplitterToken : EquationToken
        {
            public enum FunctionSplitterType
            {
                Bracket_Open,
                Bracket_Close,
                Comma,
                Unknown
            }

            public static FunctionSplitterType GetFunctionSplitterTypeFromStr(string chr)
            {
                if (chr.Equals("("))
                    return FunctionSplitterType.Bracket_Open;
                else if (chr.Equals(")"))
                    return FunctionSplitterType.Bracket_Close;
                else if (chr.Equals(","))
                    return FunctionSplitterType.Comma;



                return FunctionSplitterType.Unknown;
            }
            public override string ToString()
            {
                return $"<{type}>";
            }
            public FunctionSplitterType type;
            public FunctionSplitterToken(FunctionSplitterType type)
            {
                this.type = type;
            }
        }

        public class FunctionToken : EquationToken
        {
            public string name;
            public EquationToken[] args;
            public FunctionToken(string name, EquationToken arg)
            {
                this.name = name;
                this.args = new EquationToken[1] { arg };
            }
            public FunctionToken(string name, EquationToken[] args)
            {
                this.name = name;
                this.args = args;
            }
            public override string ToString()
            {
                string aaa = $"<Function \"{name}\" ARGS: ";

                for (int i = 0; i < args.Length - 1; i++)
                    aaa += $"{args[i].ToString()}, ";
                if (args.Length > 0)
                    aaa += $"{args[args.Length - 1].ToString()}";

                aaa += ">";

                return aaa;
            }
        }

        public class BracketToken : EquationToken
        {

            public EquationToken[] data;
            public BracketToken(EquationToken arg)
            {
                this.data = new EquationToken[1] { arg };
            }
            public BracketToken(EquationToken[] args)
            {
                this.data = args;
            }
            public override string ToString()
            {
                //string aaa = $"<Array: ";

                //for (int i = 0; i < data.Length - 1; i++)
                //    aaa += $"{data[i].ToString()}, ";
                //if (data.Length > 0)
                //    aaa += $"{data[data.Length - 1].ToString()}";

                //aaa += ">";

                string aaa = $"(";

                for (int i = 0; i < data.Length - 1; i++)
                    aaa += $"{data[i].ToString()}, ";
                if (data.Length > 0)
                    aaa += $"{data[data.Length - 1].ToString()}";

                aaa += ")";



                return aaa;
            }
        }

    }
}
