using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public partial class Solver
    {
        private static Dictionary<string, Type[]> FunctionArgs;

        private static void InitFunctionArgs()
        {
            FunctionArgs = new Dictionary<string, Type[]>();
            FunctionArgs.Add("pow", new Type[] { typeof(NumberToken), typeof(NumberToken) });

            foreach (string aaa in new string[] {"sin", "cos", "tan", "log_two", "asin", "acos", "atan", "abs", "acosh", "atanh", "cbrt", "ceiling", "cosh", "exp", "floor", "log", "log_ten", "round", "sign", "sinh", "sqrt", "tanh"})
                FunctionArgs.Add(aaa, new Type[] { typeof(NumberToken) });

            // max min
            FunctionArgs.Add("max", new Type[] { typeof(NumberToken), typeof(NumberToken) });
            FunctionArgs.Add("min", new Type[] { typeof(NumberToken), typeof(NumberToken) });

            FunctionArgs.Add("get", new Type[] { typeof(BracketToken), typeof(NumberToken) });
            FunctionArgs.Add("set", new Type[] { typeof(BracketToken), typeof(NumberToken), typeof(EquationToken) });
        }


        private static EquationToken CallFunction(string name, EquationToken[] args)
        {
            args = (EquationToken[])args.Clone();
            name = name.ToLower();
            //{
            //    string data = $"Function  \"{name}\" called with args: ";

            //    foreach (EquationToken tok in args)
            //        data += $"{tok.ToString()} ";


            //    //MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //    Console.WriteLine();
            //}

            if (!FunctionArgs.ContainsKey(name))
                return new ErrorToken($"Function \"{name}\" not found.");
            else
            {
                Type[] temp = FunctionArgs[name];

                if (args.Length != temp.Length)
                    return new ErrorToken($"Function Arguments for \"{name}\" don't match (Length different).");

                List<int> diffindexes = new List<int>();
                for (int i = 0; i < temp.Length; i++)
                    if (!(args[i].GetType().Equals(temp[i])) && !(temp[i].Equals(typeof(EquationToken))))
                    {
                        if ((args[i] is VariableToken) && (temp[i].Equals(typeof(NumberToken))))
                        {
                            args[i] = GetVar(((VariableToken)args[i]).varname);
                            i = -1;
                        }
                        else
                            diffindexes.Add(i);
                    }

                if (diffindexes.Count > 0)
                {
                    string a = "";

                    int last = diffindexes[diffindexes.Count - 1];
                    diffindexes.RemoveAt(diffindexes.Count - 1);

                    foreach (int index in diffindexes)
                        a += $"At {index}: {args[index].GetType()} is not {temp[index]}, ";
                    a += $"At {last}: {args[last].GetType()} is not {temp[last]}";

                    return new ErrorToken($"Function Arguments for \"{name}\" don't match. ({a})");
                }
            }



            // { "round", "sign", "sinh", "sqrt", "tanh"}
            // max, min
            EquationToken tempToken = name switch
            {
                "pow"      => new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value)),
                "sin"      => new NumberToken(Math.Sin((((NumberToken)args[0]).value) * multiplier)),
                "cos"      => new NumberToken(Math.Cos((((NumberToken)args[0]).value) * multiplier)),
                "tan"      => new NumberToken(Math.Tan((((NumberToken)args[0]).value) * multiplier)),
                "asin"     => new NumberToken(Math.Asin(((NumberToken)args[0]).value) / multiplier),
                "acos"     => new NumberToken(Math.Acos(((NumberToken)args[0]).value) / multiplier),
                "atan"     => new NumberToken(Math.Atan(((NumberToken)args[0]).value) / multiplier),

                "sinh"     => new NumberToken(Math.Sinh((((NumberToken)args[0]).value) * multiplier)),
                "cosh"     => new NumberToken(Math.Cosh((((NumberToken)args[0]).value) * multiplier)),
                "tanh"     => new NumberToken(Math.Tanh((((NumberToken)args[0]).value) * multiplier)),
                "asinh"    => new NumberToken(Math.Asinh(((NumberToken)args[0]).value) / multiplier),
                "acosh"    => new NumberToken(Math.Acosh(((NumberToken)args[0]).value) / multiplier),
                "atanh"    => new NumberToken(Math.Atanh(((NumberToken)args[0]).value) / multiplier),

                "abs"      => new NumberToken(Math.Abs(((NumberToken)args[0]).value)),
                "cbrt"     => new NumberToken(Math.Cbrt(((NumberToken)args[0]).value)),

                "ceiling"  => new NumberToken(Math.Ceiling(((NumberToken)args[0]).value)),
                "floor"    => new NumberToken(Math.Floor(((NumberToken)args[0]).value)),
                "exp"      => new NumberToken(Math.Exp(((NumberToken)args[0]).value)),

                "log"      => new NumberToken(Math.Log(((NumberToken)args[0]).value)),
                "log_ten"  => new NumberToken(Math.Log10(((NumberToken)args[0]).value)),
                "log_two"  => new NumberToken(Math.Log2(((NumberToken)args[0]).value)),

                "round"    => new NumberToken(Math.Round(((NumberToken)args[0]).value)),
                "sign"     => new NumberToken(Math.Sign(((NumberToken)args[0]).value)),
                "sqrt"     => new NumberToken(Math.Sqrt(((NumberToken)args[0]).value)),


                _          => new EmptyToken()
            };
            if (!(tempToken is EmptyToken))
                return tempToken;





            if (name.Equals("pow"))
                return new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));

            if (name.Equals("get"))
            {
                int index = (int)((NumberToken)args[1]).value;
                if (index >= 0 && index < ((BracketToken)args[0]).data.Length)
                    return ((BracketToken)args[0]).data[index];
                else
                    return new ErrorToken($"Index {index} is out of bounds.");
            }

            if (name.Equals("set"))
            {
                int index = (int)((NumberToken)args[1]).value;
                if (index >= 0 && index < ((BracketToken)args[0]).data.Length)
                {
                    ((BracketToken)args[0]).data[index] = args[2];
                    return args[0];
                }
                else
                    return new ErrorToken($"Index {index} is out of bounds.");
            }



            return new NumberToken(0);
        }

    }
}
