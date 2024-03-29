﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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

        [Serializable]
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

        //[DataContract(IsReference = true)]
        public class NumberToken : EquationToken
        {
            [DataMember]
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

        //[DataContract(IsReference = true)]
        public class ErrorToken : EquationToken
        {
            [DataMember]
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

        //[DataContract(IsReference = true)]
        public class EmptyToken : EquationToken
        {
            public override string ToString()
            {
                return "";
            }
        }

        [DataContract(IsReference = true)]
        public class VariableToken : EquationToken
        {
            [DataMember]
            public string varname;
            public static int maxcounter = 0;
            public override string ToString()
            {
                if (maxcounter > 20)
                    return $"TOO MANY OBJECTS TO DISPLAY";
                
                maxcounter++;
                string temp = ToString(this, 0);
                maxcounter--;
                return temp;
            }
            public static string ToString(VariableToken tok, int counter)
            {
                if (counter > 20)
                    return $"TOO MANY OBJECTS TO DISPLAY";

                

                EquationToken temp = GetVar(tok.varname);
                if (temp is VariableToken)
                    return $"<Var \"{tok.varname}\": {VariableToken.ToString(((VariableToken)temp), counter + 1)}>";

                return $"<Var \"{tok.varname}\": {temp.ToString()}>";
            }
            public VariableToken(string varname)
            {
                this.varname = varname;
            }
        }

        //[DataContract(IsReference = true)]
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

            [DataMember]
            public Operation op;
            public OperatorToken(Operation op)
            {
                this.op = op;
            }
        }

        //[DataContract(IsReference = true)]
        public class VariableorFunctionToken : EquationToken
        {
            [DataMember]
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

        [DataContract(IsReference = true)]
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
            [DataMember]
            public FunctionSplitterType type;
            public FunctionSplitterToken(FunctionSplitterType type)
            {
                this.type = type;
            }
        }

        [DataContract(IsReference = true)]
        public class FunctionToken : EquationToken
        {
            [DataMember]
            public string name;
            [DataMember]
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

        [DataContract(IsReference = true)]
        public class BracketToken : EquationToken
        {
            [DataMember]
            public EquationToken[] data;
            public BracketToken(EquationToken arg)
            {
                this.data = new EquationToken[1] { arg };
            }
            public BracketToken(EquationToken[] args)
            {
                this.data = args;
            }

            public static int maxcounter = 0;
            public override string ToString()
            {
                if (maxcounter > 10)
                    return $"TOO MANY OBJECTS TO DISPLAY";

                maxcounter++;
                string temp = ToString(this, 0);
                maxcounter--;
                return temp;
            }
            public static string ToString(BracketToken tok, int counter)
            {
                if (counter > 10)
                    return $"TOO MANY OBJECTS TO DISPLAY";


                string aaa = $"(";

                for (int i = 0; i < tok.data.Length - 1; i++)
                    if (tok.data[i] is BracketToken)
                        aaa += $"{ToString((BracketToken)tok.data[i], counter+1)}, "; 
                    else
                        aaa += $"{tok.data[i].ToString()}, ";


                if (tok.data.Length > 0)
                {
                    if (tok.data[tok.data.Length - 1] is BracketToken)
                        aaa += $"{ToString((BracketToken)tok.data[tok.data.Length - 1], counter + 1)}";
                    else
                        aaa += $"{tok.data[tok.data.Length - 1].ToString()}";
                }

                aaa += ")";



                return aaa;



                //EquationToken temp = GetVar(tok.varname);
                //if (temp is VariableToken)
                //    return $"<Var \"{tok.varname}\": {VariableToken.ToString(((VariableToken)temp), counter + 1)}>";

                //return $"<Var \"{tok.varname}\": {temp.ToString()}>";
            }

        }

    }
}
