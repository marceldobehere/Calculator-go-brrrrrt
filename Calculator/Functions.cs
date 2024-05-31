using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (string aaa in new string[] {"sin", "cos", "tan", "log_two", "asin", "acos", "atan", "abs", "acosh", "atanh", "cbrt", "ceiling", "cosh", "exp", "floor", "log", "log_ten", "round", "sign", "sinh", "sqrt", "√", "tanh", "fact"})
                FunctionArgs.Add(aaa, new Type[] { typeof(NumberToken) });

            // max min
            FunctionArgs.Add("max", new Type[] { typeof(NumberToken), typeof(NumberToken) });
            FunctionArgs.Add("min", new Type[] { typeof(NumberToken), typeof(NumberToken) });

            FunctionArgs.Add("get", new Type[] { typeof(BracketToken), typeof(NumberToken) });
            FunctionArgs.Add("set", new Type[] { typeof(BracketToken), typeof(NumberToken), typeof(EquationToken) });

            // binomial (n, k, p)
            FunctionArgs.Add("binomial", new Type[] { typeof(NumberToken), typeof(NumberToken), typeof(NumberToken) });

            // range
            FunctionArgs.Add("range", new Type[] { typeof(NumberToken), typeof(NumberToken), typeof(NumberToken) });
        }

        private static double fact(int num)
        {
            double res = 1;
            for (int i = 1; i <= num; i++)
                res *= i;
            return res;
        }

        public static double getnCk(long n, long k)
        {
            //double buffern = n * Math.Log(n) - n;
            //double bufferk = k * Math.Log(k) - k;
            //double bufferkn = Math.Abs(n - k) * Math.Log(Math.Abs(n - k)) - Math.Abs(n - k);

            //return Math.Exp(buffern) / (Math.Exp(bufferk) * Math.Exp(bufferkn));
            if (k > n) return 0;
            if (n == k) return 1; // only one way to chose when n == k
            if (k > n - k) k = n - k; // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
            double c = 1;
            for (long i = 1; i <= k; i++)
            {
                c *= n--;
                c /= i;
            }
            return c;
        }

        // (->p) (fact(5)/(fact(p)*fact(5-p)))*(0.6^p)*(0.4^(5-p))
        private static double binomial(int n, int k, double p)
        {
            //return (fact(n) / (fact(k) * fact(n-k))) * Math.Pow(p, k) * Math.Pow((1-p), (n-k));
            return getnCk(n, k) * Math.Pow(p, k) * Math.Pow((1 - p), (n - k));
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
                "sqrt" => new NumberToken(Math.Sqrt(((NumberToken)args[0]).value)),
                "fact" => new NumberToken(fact((int)((NumberToken)args[0]).value)),

                "√" => new NumberToken(Math.Sqrt(((NumberToken)args[0]).value)),


                _          => new EmptyToken()
            };
            if (!(tempToken is EmptyToken))
                return tempToken;


            if (name.Equals("min"))
                return new NumberToken(Math.Min(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));

            if (name.Equals("max"))
                return new NumberToken(Math.Max(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));

            if (name.Equals("binomial"))
                return new NumberToken(binomial((int)((NumberToken)args[0]).value, (int)(((NumberToken)args[1]).value), ((NumberToken)args[2]).value));


            if (name.Equals("pow"))
                return new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));

            if (name.Equals("range"))
            {
                double start = ((NumberToken)args[0]).value;
                double stop = ((NumberToken)args[1]).value;
                double step = ((NumberToken)args[2]).value;

                List<EquationToken> vals = new List<EquationToken>();
                if (step == 0)
                    ;
                else if (step > 0)
                    for (double i = start; i <= stop; i += step)
                        vals.Add(new NumberToken(i));
                else
                    for (double i = start; i >= stop; i += step)
                        vals.Add(new NumberToken(i));

                return new BracketToken(vals.ToArray());
            }

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
