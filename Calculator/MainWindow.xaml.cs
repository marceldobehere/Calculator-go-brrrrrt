using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using Path = System.IO.Path;

namespace Calculator
{
    // TODO: 
    // Implement Session Saving and Loading using recursive object serialization.
    // Implement History
    // Implement Customizability
    // Add Wolfram alpha API support


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


        private bool deg = false;
        public bool Deg
        {
            get
            {
                return deg;
            }
            set
            {
                deg = value;
                if (value)
                {
                    Button_deg_rad.Content = "DEG";
                    Solver.multiplier = Math.PI / 180.0;
                }
                else
                {
                    Button_deg_rad.Content = "RAD";
                    Solver.multiplier = 1;
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            Solver.Init();
            Input = "";
            Result = "";
            Deg = true;
            TextVSKeyboard = true;
            this.InputBindings.Add(new InputBinding(TestCmd, new KeyGesture(Key.V, ModifierKeys.Alt)));

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {

                string newname = $"{Directory.GetParent(args[1])}\\{Path.GetFileNameWithoutExtension(args[1])} - CALCULATED{Path.GetExtension(args[1])}";
                //MessageBox.Show($"AA:\n{args[1]}\nAB:\n{newname}");


                StreamReader reader = new StreamReader(args[1]);
                StreamWriter writer = new StreamWriter(newname);

                while(!reader.EndOfStream)
                    writer.WriteLine(Solver.Solve(reader.ReadLine()).ToString());

                reader.Close();
                writer.Close();
                Close();
            }
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
            string name = ((Button)sender).Name;
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
            else if (name.Equals("Button_textvsinput"))
            {
                TextVSKeyboard = !TextVSKeyboard;
            }
            else if (name.Equals("Button_del_var"))
            {
                Solver.ClearVars();
                Result = "Cleared Variables";
            }
            else if (name.Equals("Button_deg_rad"))
            {
                Deg = !Deg;
            }
            else if (name.Equals("Button_s_save"))
            {

            }
            else if (name.Equals("Button_s_load"))
            {

            }
            //Button_s_load

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
                Solver.Init();
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
                Input += "->";
            }
            else if (tempdata.Equals("?"))
            {
                Input += "?";
            }



            //Input += tempdata;;
        }

        private void SolveCurrent()
        {
            Solver.EquationToken temp = Solver.Solve(Input);
            Solver.results.Add(temp);
            Result = temp.ToString();
        }



       
    }
}
