using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public partial class Solver
    {
        private static Dictionary<string, EquationToken> variables;

        public static void ClearVars()
        {
            variables.Clear();
        }

        public static void SetVar(string varname, EquationToken val)
        {
            if (!variables.ContainsKey(varname))
                variables.Add(varname, val);
            else
                variables[varname] = val;
        }

        public static EquationToken GetVar(string varname)
        {
            if (!variables.ContainsKey(varname))
                return new NumberToken(0);
            else
                return variables[varname];
        }
    }
}