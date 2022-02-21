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
            FunctionArgs.Add("Pow", new Type[] {typeof(NumberToken), typeof(NumberToken) });
        }


        private static EquationToken CallFunction(string name, EquationToken[] args)
        {
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
                        }
                        else
                            same = false;
                        
                    }

                if (!same)
                    return new ErrorToken($"Function Arguments for \"{name}\" don't match.");
            }


            if (name.Equals("Pow"))
            {
                return new NumberToken(Math.Pow(((NumberToken)args[0]).value, ((NumberToken)args[1]).value));
            }

            //(3->x)+Pow(2,x)

            return new NumberToken(0);
        }

    }
}
