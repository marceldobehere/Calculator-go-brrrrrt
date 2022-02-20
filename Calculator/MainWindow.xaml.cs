using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Calculator
{



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string __input;
        public string Input
        {
            get
            {
                __input = Textbox.Text;
                return __input;
            }
            set
            {
                __input = value;
                Textbox.Text = __input;
            }
        }

        string __result;
        public string Result
        {
            get
            {
                return __result;
            }
            set
            {
                __result = value;
                ResultBox.Text = __result;
            }
        }

        private bool _textvskeyboard;
        public bool TextVSKeyboard
        {
            get
            {
                return _textvskeyboard;
            }
            set
            {
                _textvskeyboard = value;
                if (value)
                {
                    Button_textvsinput.Content = "Textbox";
                    Textbox.IsReadOnly = false;

                }
                else
                {
                    Button_textvsinput.Content = "Keyboard";
                    Textbox.IsReadOnly = true;


                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            Input = "";
            Result = "";
            TextVSKeyboard = true;
            this.InputBindings.Add(new InputBinding(TestCmd, new KeyGesture(Key.V, ModifierKeys.Alt)));
        }


        private TestCMD _TestCmd;
        public ICommand TestCmd
        {
            get
            {
                if (_TestCmd == null)
                    _TestCmd = new TestCMD(PasteHandler);
                return _TestCmd;
            }
        }

        class TestCMD : ICommand
        {
            #region ICommand Members  
            private Action TestCaller;
            public TestCMD(Action Temp)
            {
                TestCaller = Temp;
            }


            public bool CanExecute(object parameter)
            {
                //MessageBox.Show("Bru 1");
                return true;
            }
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                TestCaller.Invoke();
            }
            #endregion
        }



        public void PasteHandler()
        {
            if (!TextVSKeyboard)
                Input += Clipboard.GetText(TextDataFormat.Text);
        }




        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
        public void MenuButtons(object sender, RoutedEventArgs args)
        {
            string menudata = (string)((Button)sender).Content;
            if (menudata.Equals("x"))
                Close();
            else if (menudata.Equals("-"))
                this.WindowState = System.Windows.WindowState.Minimized;
            else if (menudata.Equals("+"))
            {
                if (this.WindowState == System.Windows.WindowState.Normal)
                    this.WindowState = System.Windows.WindowState.Maximized;
                else
                    this.WindowState = System.Windows.WindowState.Normal;
            }
            else if (((Button)sender).Name.Equals("Button_textvsinput"))
            {
                TextVSKeyboard = !TextVSKeyboard;
            }

        }



        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                ButtonInput("=");
            else if (e.Key == Key.Escape)
                ButtonInput("CE");

            if (TextVSKeyboard)
                return;

            if (e.Key == Key.Add)
                Input += "+";
            else if (e.Key == Key.Subtract)
                Input += "-";
            else if (e.Key == Key.Multiply)
                Input += "*";
            else if (e.Key == Key.Divide)
                Input += "/";
            else if (e.Key == Key.Back)
                ButtonInput("<");
            else if (e.Key == Key.Space)
                Input += " ";



            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Input += Clipboard.GetText(TextDataFormat.Text);
                return;
            }

            else if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                    Input += "0";
                else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                    Input += "1";
                else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                    Input += "2";
                else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                    Input += "3";
                else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                    Input += "4";
                else if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                    Input += "5";
                else if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                    Input += "6";
                else if (e.Key == Key.D7 || e.Key == Key.NumPad7)
                    Input += "7";
                else if (e.Key == Key.D8 || e.Key == Key.NumPad8)
                    Input += "8";
                else if (e.Key == Key.D9 || e.Key == Key.NumPad9)
                    Input += "9";
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                    Input += (char)('a' + (e.Key - 44));


                if (e.Key == Key.OemPlus)
                    Input += "+";
                else if (e.Key == Key.OemMinus)
                    Input += "-";
                else if (e.Key == Key.OemComma)
                    Input += ",";
                else if (e.Key == Key.OemPeriod)
                    Input += ".";
            }
            else
            {
                if (e.Key == Key.OemPlus)
                    Input += "*";
                else if (e.Key == Key.D7)
                    Input += "/";
                else if (e.Key == Key.D0)
                    ButtonInput("=");
                else if (e.Key == Key.D8)
                    Input += "(";
                else if (e.Key == Key.D9)
                    Input += ")";
                else if (e.Key == Key.D5)
                    Input += "%";
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                    Input += (char)('A' + (e.Key - 44));
            }




            //if (e.Key == Key.Subtract)
            //{
            //    // Do something
            //}
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)((Button)e.Source).Content);
        }

        private void ButtonInput(string tempdata)
        {
            if ("0123456789.,()+-*/".Contains(tempdata))
            {
                Input += tempdata;
                return;
            }

            if (tempdata.Equals("CE"))
            {
                Input = "";
                Result = "";
            }
            else if (tempdata.Equals("<"))
            {
                if (Input.Length > 0)
                    Input = Input.Substring(0, Input.Length - 1);
            }
            else if (tempdata.Equals(">"))
            {
                Input = "";
                Result = "";
            }
            else if (tempdata.Equals("="))
            {
                SolveCurrent();
            }
            else if (tempdata.Equals("ans"))
            {
                Input = "";
            }
            else if (tempdata.Equals("->"))
            {
                Input = "";
            }
            else if (tempdata.Equals("?"))
            {
                Input += "?";
            }



            //Input += tempdata;;
        }

        private void SolveCurrent()
        {
            Result = Solve(Input);
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
        private static string opchars = "+-*/%^";
        private static string infuncchars = "(),";
        private static string funcorvarchars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string Solve(string equation)
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
                        tokens.Add(new NumberToken(double.Parse(tempstr)));
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

            {
                EquationToken tok = CalculateTokens(tokens);

                if (tok != null)
                    return tok.ToString();
                return "";
            }
        }

        private EquationToken CalculateTokens(List<EquationToken> tokens)
        {
            //{
            //    string data = $"Calculating Tokens: ";

            //    foreach (EquationToken tok in tokens)
            //        data += $"{tok.ToString()} ";


            //    //MessageBox.Show(data);
            //    AllocConsole();
            //    Console.WriteLine(data + "\n");
            //}



            tokens = new List<EquationToken>(tokens);

            bool change = true;

            while (change)
            {
                change = true;
                while (change)
                {
                    change = false;
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i] is BracketToken)
                        {
                            EquationToken temp = CalculateTokens(((BracketToken)tokens[i]).data.ToList());
                            if (!temp.Equals(tokens[i]))
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
                                    arrtoksog = new EquationToken[1] {temptok}; 




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
                            EquationToken temp = CallFunction(((FunctionToken)tokens[i]).name, ((FunctionToken)tokens[i]).args);
                            Console.WriteLine($"{temp.ToString()} == {tokens[i].ToString()} ? {temp.Equals(tokens[i])}");
                            if (!temp.Equals(tokens[i]))
                            {
                                tokens[i] = temp;
                                change = true;
                            }
                        }
                        else if (tokens[i] is VariableToken)
                        {
                            EquationToken temp = GetVar(((VariableToken)tokens[i]).varname);
                            if (!temp.Equals(tokens[i]))
                            {
                                tokens[i] = temp;
                                change = true;
                            }
                        }
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
                                    if (!(tokens[i - 1] is NumberToken && tokens[i + 1] is NumberToken))
                                        break;
                                    if (op == OperatorToken.Operation.PLUS)
                                    {
                                        tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value + ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                    if (op == OperatorToken.Operation.MINUS)
                                    {
                                        tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value - ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                    if (op == OperatorToken.Operation.MULTIPLY)
                                    {
                                        tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value * ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                    if (op == OperatorToken.Operation.POWER)
                                    {
                                        tokens[i - 1] = new NumberToken(Math.Pow(((NumberToken)tokens[i - 1]).value, ((NumberToken)tokens[i + 1]).value));
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                    if (op == OperatorToken.Operation.DIVIDE)
                                    {
                                        tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value / ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                    if (op == OperatorToken.Operation.MOD)
                                    {
                                        tokens[i - 1] = new NumberToken(((NumberToken)tokens[i - 1]).value % ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveRange(i, 2);
                                        i = 0;
                                        change = true;
                                    }
                                }
                                else if (i < tokens.Count - 1)
                                {
                                    if (!(tokens[i + 1] is NumberToken))
                                        break;
                                    if (op == OperatorToken.Operation.MINUS)
                                    {
                                        tokens[i + 1] = new NumberToken(0 - ((NumberToken)tokens[i + 1]).value);
                                        tokens.RemoveAt(i);
                                        i = 0;
                                        change = true;
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }





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
                return null;
        }


        private EquationToken CallFunction(string name, EquationToken[] args)
        {
            return new NumberToken(0);
        }

        private EquationToken GetVar(string name)
        {
            return new NumberToken(0);
        }


        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        private List<EquationToken> CorrectTokens(List<EquationToken> tokens)
        {
            tokens = new List<EquationToken>(tokens);
            if (tokens.Count == 0)
                return new List<EquationToken>() { new EquationToken() };

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

                        //int level = 0;
                        //int endi = i;
                        //List<EquationToken> toks = new List<EquationToken>();
                        //for (int i2 = i + 1; i2 < tokens.Count; i2++)
                        //{
                        //    if (tokens[i2] is FunctionSplitterToken)
                        //    {
                        //        FunctionSplitterToken.FunctionSplitterType temptype = ((FunctionSplitterToken)tokens[i2]).type;
                        //        if (temptype == FunctionSplitterToken.FunctionSplitterType.Comma)
                        //        {
                        //            toks.Add(tokens[i2]);
                        //        }
                        //        else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Open)
                        //        {
                        //            toks.Add(tokens[i2]);
                        //            level++;
                        //        }
                        //        else if (temptype == FunctionSplitterToken.FunctionSplitterType.Bracket_Close)
                        //        {
                        //            if (level == 0)
                        //            {
                        //                endi = i2;
                        //                break;
                        //            }
                        //            else
                        //            {
                        //                toks.Add(tokens[i2]);
                        //                level--;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        toks.Add(tokens[i2]);
                        //    }
                        //}
                        //tokens.RemoveRange(i + 1, endi - i);

                        //tokens[i] = new BracketToken(CorrectTokens(toks).ToArray());




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




        private TokenMode GetTokenModeFromChar(char val)
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

        public class EquationToken
        {
            public override string ToString()
            {
                return "";
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


                if (x is NumberToken && y is NumberToken)
                    if (((NumberToken)x).value == ((NumberToken)y).value)
                        return true;
                if (x is OperatorToken && y is OperatorToken)
                    if (((OperatorToken)x).op == ((OperatorToken)y).op)
                        return true;
                if (x is FunctionSplitterToken && y is FunctionSplitterToken)
                    if (((FunctionSplitterToken)x).type == ((FunctionSplitterToken)y).type)
                        return true;
                if (x is VariableToken && y is VariableToken)
                    if (((VariableToken)x).varname == ((VariableToken)y).varname)
                        return true;
                if (x is VariableorFunctionToken && y is VariableorFunctionToken)
                    if (((VariableorFunctionToken)x).name == ((VariableorFunctionToken)y).name)
                        return true;

                
                if (x is BracketToken && y is BracketToken)
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

               
                if (x is FunctionToken && y is FunctionToken)
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



                return false;
            }
        }

        public class NumberToken : EquationToken
        {
            public double value;
            public override string ToString()
            {
                //return $"<Value: {value}>";
                return $"{value}";
            }
            public NumberToken(double value)
            {
                this.value = value;
            }
        }

        public class VariableToken : EquationToken
        {
            public string varname;
            public override string ToString()
            {
                return $"<Variable: {varname}>";
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
