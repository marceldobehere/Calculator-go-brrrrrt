using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public partial class Solver
    {
        private static Dictionary<string, EquationToken> variables;
        private static Dictionary<string, EquationToken> constants;

        public static void ClearVars()
        {
            variables.Clear();
            variables.Add("", new EmptyToken());
        }

        public static void InitConsts()
        {
            constants = new Dictionary<string, EquationToken>()
            {
                {"PI", new NumberToken(Math.PI) },
            };
        }

        public static void SetVar(string varname, EquationToken val)
        {
            if (constants.ContainsKey(varname))
                return;

            if (!variables.ContainsKey(varname))
                variables.Add(varname, val);
            else
                variables[varname] = val;
        }

        public static EquationToken GetVar(string varname)
        {
            if (varname.Equals("ans"))
            {
                if (results[results.Count - 1] is NumberToken)
                    return results[results.Count - 1];
                if (results[results.Count - 1] is VariableToken)
                    if (((VariableToken)results[results.Count - 1]).varname.Equals("ans"))
                        return new ErrorToken("Self reference");
                return CalculateTokens(new List<EquationToken>() { results[results.Count - 1] });
            }

            if (constants.ContainsKey(varname))
                return constants[varname];

            if (!variables.ContainsKey(varname))
                return new NumberToken(0);
            else
                return variables[varname];
        }
    }
}