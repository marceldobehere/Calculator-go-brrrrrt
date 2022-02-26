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
            FunctionArgs.Add("sin", new Type[] { typeof(NumberToken) });
            FunctionArgs.Add("cos", new Type[] { typeof(NumberToken) });
            FunctionArgs.Add("tan", new Type[] { typeof(NumberToken) });
            FunctionArgs.Add("asin", new Type[] { typeof(NumberToken) });
            FunctionArgs.Add("acos", new Type[] { typeof(NumberToken) });
            FunctionArgs.Add("atan", new Type[] { typeof(NumberToken) });
        }


        private static EquationToken CallFunction(string name, EquationToken[] args)
        {
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
                    return new ErrorToken($"Function Arguments for \"{name}\" don't match.");

                bool same = true;
                for (int i = 0; i < temp.Length; i++)
                    if (!(args[i].GetType().Equals(temp[i])))
                    {
                        if ((args[i] is VariableToken) && (temp[i].Equals(typeof(NumberToken))))
                        {
                            args[i] = GetVar(((VariableToken)args[i]).varname);
                            i = 0;
                        }
                        else
                            same = false;
                    }

                if (!same)
                    return new ErrorToken($"Function Arguments for \"{name}\" don't match.");
            }


            EquationToken tempToken = name switch
            {
                "pow" => new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value)),
                "sin" => new NumberToken(Math.Sin((((NumberToken)args[0]).value) * multiplier)),
                "cos" => new NumberToken(Math.Cos((((NumberToken)args[0]).value) * multiplier)),
                "tan" => new NumberToken(Math.Tan((((NumberToken)args[0]).value) * multiplier)),
                "asin" => new NumberToken(Math.Asin(((NumberToken)args[0]).value) / multiplier),
                "acos" => new NumberToken(Math.Acos(((NumberToken)args[0]).value) / multiplier),
                "atan" => new NumberToken(Math.Atan(((NumberToken)args[0]).value) / multiplier),


                _ => new EmptyToken()
            };
            if (!(tempToken is EmptyToken))
                return tempToken;



            

            if (name.Equals("pow"))
                return new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));




            return new NumberToken(0);
        }

    }
}
