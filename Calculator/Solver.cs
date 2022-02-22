using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Calculator
{
    public partial class Solver
    {
        public static List<EquationToken> results;
        public static void Init()
        {
            variables = new Dictionary<string, EquationToken>();
            results = new List<EquationToken>() { new EmptyToken() };
            InitFunctionArgs();
            ClearVars();
        }

        enum TokenMode
        {
            Unknown,
            Number,
            Operator,
            InFunction,
            FunctionOrVariable
        }
        private static string numchars = "0123456789.";
        private static string opchars = "+-*/%^<>$";
        private static string infuncchars = "(),";
        private static string funcorvarchars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static EquationToken Solve(string equation)
        {
            List<EquationToken> tokens = new List<EquationToken>();
            string tempstr = "";
            TokenMode tempmode = TokenMode.Unknown;
            for (int i = 0; i < equation.Length; i++)
            {
                TokenMode tempmode2 = GetTokenModeFromChar(equation[i]);
                if (tempmode2 == TokenMode.Unknown)
                {
                    if (tempmode != TokenMode.Unknown)
                    {
                        if (tempmode == TokenMode.Number)
                            tokens.Add(new NumberToken(double.Parse(tempstr)));
                        else if (tempmode == TokenMode.FunctionOrVariable)
                            tokens.Add(new VariableorFunctionToken(tempstr));
                        else if (tempmode == TokenMode.InFunction)
                            tokens.Add(new FunctionSplitterToken(FunctionSplitterToken.GetFunctionSplitterTypeFromStr(tempstr)));
                        else if (tempmode == TokenMode.Operator)
                            tokens.Add(new OperatorToken(OperatorToken.GetOperationFromStr(tempstr)));

                        tempstr = "";
                        tempmode = TokenMode.Unknown;
                    }
                }

                if (tempmode == TokenMode.Unknown)
                {
                    tempstr = equation[i].ToString();
                    tempmode = tempmode2;
                    continue;
                }

                if (tempmode == tempmode2 && tempmode != TokenMode.InFunction)
                {
                    tempstr += equation[i];
                }
                else
                {
                    if (tempmode == TokenMode.Number)
                    {
                        if (double.TryParse(tempstr, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double temp1))
                            tokens.Add(new NumberToken(temp1));
                    }
                    else if (tempmode == TokenMode.FunctionOrVariable)
                        tokens.Add(new VariableorFunctionToken(tempstr));
                    else if (tempmode == TokenMode.InFunction)
                        tokens.Add(new FunctionSplitterToken(FunctionSplitterToken.GetFunctionSplitterTypeFromStr(tempstr)));
                    else if (tempmode == TokenMode.Operator)
                        tokens.Add(new OperatorToken(OperatorToken.GetOperationFromStr(tempstr)));

                    tempstr = "";
                    tempmode = TokenMode.Unknown;
                    i--;
                }

            }
            if (tempmode != TokenMode.Unknown)
            {
                if (tempmode == TokenMode.Number)
                {
                    if (double.TryParse(tempstr, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double temp1))
                        tokens.Add(new NumberToken(temp1));
                }
                else if (tempmode == TokenMode.FunctionOrVariable)
                    tokens.Add(new VariableorFunctionToken(tempstr));
                else if (tempmode == TokenMode.InFunction)
                    tokens.Add(new FunctionSplitterToken(FunctionSplitterToken.GetFunctionSplitterTypeFromStr(tempstr)));
                else if (tempmode == TokenMode.Operator)
                    tokens.Add(new OperatorToken(OperatorToken.GetOperationFromStr(tempstr)));

                tempstr = "";
                tempmode = TokenMode.Unknown;
            }


            {
                int layer = 0;
                for (int i = 0; i < tokens.Count; i++)
                    if (tokens[i] is FunctionSplitterToken)
                    {
                        if (((FunctionSplitterToken)tokens[i]).type == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                            layer++;
                        else if (((FunctionSplitterToken)tokens[i]).type == FunctionSplitterToken.FunctionSplitterType.Bracket_Close)
                            layer--;
                    }

                if (layer != 0)
                {
                    if (layer > 0)
                    {
                        for (int i = 0; i < layer; i++)
                            tokens.Add(new FunctionSplitterToken(FunctionSplitterToken.GetFunctionSplitterTypeFromStr(")")));
                    }
                    else
                    {
                        layer = -layer;
                        for (int i = 0; i < layer; i++)
                            tokens.Insert(0, new FunctionSplitterToken(FunctionSplitterToken.GetFunctionSplitterTypeFromStr("(")));
                    }
                }
            }




            tokens = CorrectTokens(tokens);




            //{
            //    string data = $"EQU:{equation}\nTOKENS: ";

            //    foreach (EquationToken tok in tokens)
            //        data += $"{tok.ToString()} ";


            //    //MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //}

            //EquationToken tok = CalculateTokens(tokens);
            //if (tok is NumberToken)
            //{
            //    return ((NumberToken)CalculateTokens(tokens)).value.ToString(CultureInfo.InvariantCulture);
            //}
            //else
            //{
            //    EquationToken
            //    string data = $"";

            //    for(int i = 0; i < tok.)
            //        data += $"{tok.ToString()} ";
            //}

            calccounter = 0;
            return CalculateTokens(tokens);
            
        }
        private static int calccounter = 0;

        private static EquationToken CalculateTokens(List<EquationToken> tokens)
        {
            if (calccounter != -999)
            {
                if (calccounter == -1000)
                    return new ErrorToken("OP Cancelled!");
                calccounter++;

                if (calccounter > 10000)
                {
                    MessageBoxResult tempres = MessageBox.Show($"Bro you are doing some intense calculations.\nWanna Cancel that?", "CALCULATION 2000", MessageBoxButton.YesNoCancel);
                    if (tempres.Equals(MessageBoxResult.Yes))
                        calccounter = -1000;
                    else if (tempres.Equals(MessageBoxResult.No))
                        calccounter = -999;
                    else
                        calccounter = 0;
                }
            }
            //{
            //    string data = $"Calculating Tokens: ";

            //    foreach (EquationToken tok in tokens)
            //        data += $"{tok.ToString()} ";


            //    //MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //}



            tokens = CloneTokenList(tokens);

            bool change = true;

            while (change)
            {
                change = false;
                foreach (OperatorToken.Operation currentop in new OperatorToken.Operation[] { OperatorToken.Operation.VAL, OperatorToken.Operation.SET })
                {

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i] is OperatorToken)
                        {
                            OperatorToken.Operation op = ((OperatorToken)tokens[i]).op;
                            if (op == currentop)
                            {
                                if (i > 0 && i < tokens.Count - 1)
                                {
                                    if (tokens[i - 1] is NumberToken && tokens[i + 1] is NumberToken)
                                    {

                                    }
                                    else
                                    {
                                        if (op == OperatorToken.Operation.SET)
                                        {
                                            if (tokens[i + 1] is VariableToken)
                                            {
                                                SetVar(((VariableToken)tokens[i + 1]).varname, tokens[i - 1]);
                                                tokens.RemoveRange(i - 1, 3);
                                                i = -1;
                                                change = true;
                                            }
                                            else
                                                return new ErrorToken("SET Operator received incorrect arguments");

                                        }
                                    }

                                }
                                else if (i < tokens.Count - 1)
                                {
                                    if (op == OperatorToken.Operation.VAL)
                                    {
                                        if (tokens[i + 1] is BracketToken)
                                            tokens[i + 1] = CalculateTokens(((BracketToken)tokens[i + 1]).data.ToList());
                                        else
                                            tokens[i + 1] = CalculateTokens(new List<EquationToken>() { tokens[i + 1] });

                                        if (tokens[i + 1] is VariableToken)
                                            tokens[i + 1] = GetVar(((VariableToken)tokens[i + 1]).varname);

                                        tokens.RemoveAt(i);
                                        i = -1;
                                        change = true;
                                    }

                                }
                            }
                        }
                    }
                }

                if (change)
                    continue;

                change = true;
                while (change)
                {
                    change = false;

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i] is BracketToken)
                        {
                            EquationToken temp = CalculateTokens(((BracketToken)tokens[i]).data.ToList());
                            if (temp == null && tokens[i] != null)
                            {
                                tokens[i] = temp;
                                change = true;
                            }
                            else if (!temp.Equals(tokens[i]))
                            {
                                tokens[i] = temp;
                                change = true;
                            }
                            else
                            {
                                List<EquationToken> toksold = tokens;
                                tokens = new List<EquationToken>();

                                {
                                    bool found = false;
                                    for (int i3 = 0; i3 < toksold.Count; i3++)
                                    {
                                        if (toksold[i3] is OperatorToken)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        tokens = toksold;
                                        tokens[i] = CalculateTokens(((BracketToken)toksold[i]).data.ToList());
                                        continue;
                                    }

                                }


                                EquationToken temptok = CalculateTokens(((BracketToken)toksold[i]).data.ToList());
                                EquationToken[] arrtoksog;
                                if (temptok is BracketToken)
                                    arrtoksog = ((BracketToken)temptok).data;
                                else
                                    arrtoksog = new EquationToken[1] { temptok };




                                EquationToken[] arrtoks = new EquationToken[arrtoksog.Length];

                                for (int i2 = 0; i2 < arrtoks.Length; i2++)
                                {
                                    toksold[i] = arrtoksog[i2];

                                    arrtoks[i2] = CalculateTokens(toksold);
                                }

                                if (arrtoks.Length > 1)
                                    tokens.Add(new BracketToken(arrtoks));
                                else
                                    tokens.Add(arrtoks[0]);


                                if (toksold.Count != tokens.Count)
                                {
                                    change = true;
                                    //i = -1;
                                    continue;
                                }

                                for (int i2 = 0; i2 < tokens.Count; i2++)
                                {
                                    if (!tokens[i2].Equals(toksold[i2]))
                                    {
                                        change = true;
                                        //i = -1;
                                        continue;
                                    }
                                }
                            }

                        }
                        else if (tokens[i] is FunctionToken)
                        {
                            EquationToken[] temparr = ((FunctionToken)tokens[i]).args;
                            for (int i2 = 0; i2 < temparr.Length; i2++)
                            {
                                temparr[i2] = CalculateTokens(new List<EquationToken>() { temparr[i2] });
                            }
                            ((FunctionToken)tokens[i]).args = temparr;

                            EquationToken temp = CallFunction(((FunctionToken)tokens[i]).name, ((FunctionToken)tokens[i]).args);
                            //Console.WriteLine($"{temp.ToString()} == {tokens[i].ToString()} ? {temp.Equals(tokens[i])}");
                            if (!temp.Equals(tokens[i]))
                            {
                                tokens[i] = temp;
                                change = true;
                            }
                        }
                        //else if (tokens[i] is VariableToken)
                        //{
                        //    EquationToken temp = GetVar(((VariableToken)tokens[i]).varname);
                        //    if (!temp.Equals(tokens[i]))
                        //    {
                        //        tokens[i] = temp;
                        //        change = true;
                        //    }
                        //}
                    }
                }

                change = false;

                foreach (OperatorToken.Operation currentop in new OperatorToken.Operation[] { OperatorToken.Operation.MOD, OperatorToken.Operation.POWER, OperatorToken.Operation.MULTIPLY, OperatorToken.Operation.DIVIDE, OperatorToken.Operation.PLUS, OperatorToken.Operation.MINUS })
                {

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i] is OperatorToken)
                        {
                            OperatorToken.Operation op = ((OperatorToken)tokens[i]).op;
                            if (op == currentop)
                            {
                                if (i > 0 && i < tokens.Count - 1)
                                {
                                    if (tokens[i - 1] is VariableToken)
                                        tokens[i - 1] = GetVar(((VariableToken)tokens[i - 1]).varname);
                                    if (tokens[i + 1] is VariableToken)
                                        tokens[i + 1] = GetVar(((VariableToken)tokens[i + 1]).varname);

                                    if (tokens[i - 1] is NumberToken && tokens[i + 1] is NumberToken)
                                    {
                                        if (op == OperatorToken.Operation.PLUS)
                                        {
                                            tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value + ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                        if (op == OperatorToken.Operation.MINUS)
                                        {
                                            tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value - ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                        if (op == OperatorToken.Operation.MULTIPLY)
                                        {
                                            tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value * ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                        if (op == OperatorToken.Operation.POWER)
                                        {
                                            tokens[i - 1] = new NumberToken(Math.Pow(((NumberToken)tokens[i - 1]).value, ((NumberToken)tokens[i + 1]).value));
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                        if (op == OperatorToken.Operation.DIVIDE)
                                        {
                                            tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value / ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                        if (op == OperatorToken.Operation.MOD)
                                        {
                                            tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value % ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveRange(i, 2);
                                            i = -1;
                                            change = true;
                                        }
                                    }
                                    else
                                    {
                                        if (op == OperatorToken.Operation.SET)
                                        {
                                            if (tokens[i + 1] is VariableToken)
                                            {
                                                SetVar(((VariableToken)tokens[i + 1]).varname, tokens[i - 1]);
                                                tokens.RemoveRange(i - 1, 3);
                                                i = -1;
                                                change = true;
                                            }
                                        }
                                    }

                                }
                                else if (i < tokens.Count - 1)
                                {
                                    if (tokens[i + 1] is NumberToken)
                                    {
                                        if (op == OperatorToken.Operation.MINUS)
                                        {
                                            tokens[i + 1] = new NumberToken(0 - ((NumberToken)tokens[i + 1]).value);
                                            tokens.RemoveAt(i);
                                            i = -1;
                                            change = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //{
            //    string data = $"Equals Tokens: ";

            //    foreach (EquationToken tok in tokens)
            //        data += $"{tok.ToString()} ";


            //    //MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //    Console.WriteLine();
            //}


            //for (int i = 0; i < tokens.Count; i++)
            //    if (tokens[i] is VariableToken)
            //        tokens[i] = GetVar(((VariableToken)tokens[i]).varname);
                
            


            if (tokens.Count > 0)
            {
                if (tokens.Count > 1)
                {
                    return new BracketToken(tokens.ToArray());
                }
                else
                    return tokens[0];
            }
            else
                return new EmptyToken();
        }




        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        private static List<EquationToken> CorrectTokens(List<EquationToken> tokens)
        {
            tokens = CloneTokenList(tokens);
            if (tokens.Count == 0)
                return new List<EquationToken>() { new EmptyToken() };

            //{
            //    string data = $"Correcting Tokens: ";

            //    foreach (EquationToken tok in tokens)
            //        data += $"{tok.ToString()} ";

            //    MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //}

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is FunctionSplitterToken)
                {
                    if (((FunctionSplitterToken)tokens[i]).type == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                    {
                        if (i > 0)
                            if ((tokens[i - 1] is VariableorFunctionToken))
                                continue;

                        List<List<EquationToken>> toks = new List<List<EquationToken>>();
                        toks.Add(new List<EquationToken>());
                        int level = 0;
                        int endi = i;
                        for (int i2 = i + 1; i2 < tokens.Count; i2++)
                        {
                            if (tokens[i2] is FunctionSplitterToken)
                            {
                                FunctionSplitterToken.FunctionSplitterType temptype = ((FunctionSplitterToken)tokens[i2]).type;
                                if (temptype == FunctionSplitterToken.FunctionSplitterType.Comma)
                                {
                                    if (level == 0)
                                        toks.Add(new List<EquationToken>());
                                    else
                                        toks.Last().Add(tokens[i2]);
                                }
                                else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                                {
                                    toks.Last().Add(tokens[i2]);
                                    level++;
                                }
                                else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Close)
                                {
                                    if (level == 0)
                                    {
                                        endi = i2;
                                        break;
                                    }
                                    else
                                    {
                                        toks.Last().Add(tokens[i2]);
                                        level--;
                                    }
                                }

                            }
                            else
                            {
                                toks.Last().Add(tokens[i2]);
                            }
                        }

                        tokens.RemoveRange(i + 1, endi - i);


                        List<EquationToken> toks2 = new List<EquationToken>();
                        for (int i2 = 0; i2 < toks.Count; i2++)
                        {
                            List<EquationToken> toks3 = CorrectTokens(toks[i2]);
                            if (toks3.Count > 1)
                                toks2.Add(new BracketToken(toks3.ToArray()));
                            else
                                toks2.Add(toks3[0]);
                        }


                        if (toks2.Count == 0)
                        {
                            tokens.RemoveAt(i);
                            i--;
                        }
                        else
                            tokens[i] = new BracketToken(toks2.ToArray());
                    }
                }
            }
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is VariableorFunctionToken)
                {
                    if ((i + 1) < tokens.Count)
                    {
                        if (tokens[i + 1] is FunctionSplitterToken)
                        {
                            if (((FunctionSplitterToken)tokens[i + 1]).type == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                            {
                                List<List<EquationToken>> toks = new List<List<EquationToken>>();
                                toks.Add(new List<EquationToken>());
                                int level = 0;
                                int endi = i;
                                for (int i2 = i + 2; i2 < tokens.Count; i2++)
                                {
                                    if (tokens[i2] is FunctionSplitterToken)
                                    {
                                        FunctionSplitterToken.FunctionSplitterType temptype = ((FunctionSplitterToken)tokens[i2]).type;
                                        if (temptype == FunctionSplitterToken.FunctionSplitterType.Comma)
                                        {
                                            if (level == 0)
                                                toks.Add(new List<EquationToken>());
                                            else
                                                toks.Last().Add(tokens[i2]);
                                        }
                                        else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                                        {
                                            toks.Last().Add(tokens[i2]);
                                            level++;
                                        }
                                        else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Close)
                                        {
                                            if (level == 0)
                                            {
                                                endi = i2;
                                                break;
                                            }
                                            else
                                            {
                                                toks.Last().Add(tokens[i2]);
                                                level--;
                                            }
                                        }

                                        // CHECK FOR recursive BRACKETS LIKE function(1, (1 + 3), sin(2));
                                        // I think it should work, just that all brackets get saved as functionsplitter
                                    }
                                    else
                                    {
                                        toks.Last().Add(tokens[i2]);
                                    }
                                }

                                tokens.RemoveRange(i + 1, endi - i);


                                List<EquationToken> toks2 = new List<EquationToken>();
                                for (int i2 = 0; i2 < toks.Count; i2++)
                                {
                                    List<EquationToken> toks3 = CorrectTokens(toks[i2]);
                                    if (toks3.Count > 1)
                                        toks2.Add(new BracketToken(toks3.ToArray()));
                                    else
                                        toks2.Add(toks3[0]);
                                }


                                tokens[i] = new FunctionToken(((VariableorFunctionToken)tokens[i]).name, toks2.ToArray());
                            }
                            else
                            {
                                tokens[i] = new VariableToken(((VariableorFunctionToken)tokens[i]).name);
                            }
                        }
                        else
                        {
                            tokens[i] = new VariableToken(((VariableorFunctionToken)tokens[i]).name);
                        }
                    }
                    else
                    {
                        tokens[i] = new VariableToken(((VariableorFunctionToken)tokens[i]).name);
                    }
                }
            }






            return tokens;
        }




        private static TokenMode GetTokenModeFromChar(char val)
        {
            if (numchars.Contains(val))
                return TokenMode.Number;
            else if (opchars.Contains(val))
                return TokenMode.Operator;
            else if (funcorvarchars.Contains(val))
                return TokenMode.FunctionOrVariable;
            else if (infuncchars.Contains(val))
                return TokenMode.InFunction;
            else
                return TokenMode.Unknown;
        }


    }
}
