using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SciCalk.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _inputText = "";
        private string _calculatedResult = "0";

        public event PropertyChangedEventHandler PropertyChanged;

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
                AutoCalculate();
            }
        }

        public string CalculatedResult
        {
            get => _calculatedResult;
            set
            {
                _calculatedResult = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public ICommand NumberInputCommand => new Command<string>((num) =>
        {
            InputText += num;
        });

        public ICommand MathOperatorCommand => new Command<string>((op) =>
        {
            InputText += $" {ConvertMathOp(op)} ";
        });

        public ICommand BackspaceCommand => new Command(() =>
        {
            if (!string.IsNullOrEmpty(InputText))
                InputText = InputText[..^1];
        });

        public ICommand ResetCommand => new Command(() =>
        {
            InputText = "";
            CalculatedResult = "0";
        });

        public ICommand RegionOparatorCommand => new Command<string>((op) =>
        {
            InputText += op;
        });

        public ICommand SientificOperatorCommand => new Command<string>((op) =>
        {
            InputText += $"{op}(";
        });

        public ICommand CalculateCommand => new Command(() =>
        {
            string result = Evaluate(InputText);

            CalculatedResult = result;

            InputText = result;
        });


        private void AutoCalculate()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                CalculatedResult = "0";
                return;
            }

            try
            {
                CalculatedResult = Evaluate(InputText);
            }
            catch
            {
                CalculatedResult = "";
            }
        }


        private string Evaluate(string expr)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expr))
                    return "0";

                expr = expr.Replace("×", "*").Replace("÷", "/");

                double result = EvaluateExpression(expr);

                return double.IsNaN(result) ? "Error" : result.ToString();
            }
            catch
            {
                return "Error";
            }
        }

        private double EvaluateExpression(string expr)
        {
            expr = expr.Trim();

            while (expr.Contains("("))
            {
                int close = expr.IndexOf(")");
                int open = expr.LastIndexOf("(", close);

                string inside = expr.Substring(open + 1, close - open - 1);

                double val = SolveSegment(inside);

                expr = expr.Substring(0, open) + val + expr.Substring(close + 1);
            }

            return SolveSegment(expr);
        }

        private double SolveSegment(string seg)
        {
            seg = seg.Trim();

            var funcMatch = Regex.Match(seg, @"^([A-Za-z]+)\s*(.*)$");

            if (funcMatch.Success && FunctionExists(funcMatch.Groups[1].Value))
                return EvaluateFunction(funcMatch.Groups[1].Value, funcMatch.Groups[2].Value);

            return SolveBasicMath(seg);
        }

        private bool FunctionExists(string f)
        {
            string[] funcs =
            {
        "SIN","COS","TAN","ASIN","ACOS","ATAN",
        "LOG","LOG10","EXP",
        "SQRT","ABS",
        "MEAN","VAR","STD","POW","MOD","FACT"
    };

            return funcs.Contains(f.ToUpper());
        }


        private double EvaluateFunction(string func, string argsRaw)
        {
            func = func.ToUpper();

            argsRaw = argsRaw.Trim();
            if (argsRaw.StartsWith("(") && argsRaw.EndsWith(")"))
                argsRaw = argsRaw.Substring(1, argsRaw.Length - 2);

            var args = SmartSplitArgs(argsRaw);

            double[] vals = args.Select(a => EvaluateExpression(a)).ToArray();

            return func switch
            {
                "SIN" => Math.Sin(ToRad(vals[0])),
                "COS" => Math.Cos(ToRad(vals[0])),
                "TAN" => Math.Tan(ToRad(vals[0])),

                "ASIN" => ToDeg(Math.Asin(vals[0])),
                "ACOS" => ToDeg(Math.Acos(vals[0])),
                "ATAN" => ToDeg(Math.Atan(vals[0])),

                "LOG" => Math.Log(vals[0]),
                "LOG10" => Math.Log10(vals[0]),
                "EXP" => Math.Exp(vals[0]),

                "SQRT" => Math.Sqrt(vals[0]),
                "ABS" => Math.Abs(vals[0]),

                "POW" => Math.Pow(vals[0], vals[1]),
                "MOD" => vals[0] % vals[1],

                "FACT" => Factorial((int)vals[0]),

                "MEAN" => vals.Average(),
                "VAR" => vals.Select(v => Math.Pow(v - vals.Average(), 2)).Average(),
                "STD" => Math.Sqrt(vals.Select(v => Math.Pow(v - vals.Average(), 2)).Average()),

                _ => throw new Exception("Invalid Function")
            };
        }

        private List<string> SmartSplitArgs(string args)
        {
            List<string> list = new();
            int depth = 0;
            int lastSplit = 0;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '(') depth++;
                else if (args[i] == ')') depth--;
                else if (args[i] == ',' && depth == 0)
                {
                    list.Add(args.Substring(lastSplit, i - lastSplit));
                    lastSplit = i + 1;
                }
            }

            list.Add(args.Substring(lastSplit));

            return list;
        }

        private double SolveBasicMath(string expression)
        {
            var dt = new DataTable();
            return Convert.ToDouble(dt.Compute(expression, ""));
        }

        private double ToRad(double d) => d * Math.PI / 180;
        private double ToDeg(double r) => r * 180 / Math.PI;

        private int Factorial(int n)
        {
            if (n < 0) return 0;
            int f = 1;
            for (int i = 1; i <= n; i++) f *= i;
            return f;
        }

        private string ConvertMathOp(string op)
        {
            return op switch
            {
                "×" => "*",
                "÷" => "/",
                _ => op
            };
        }

        private string PreprocessExpression(string exp)
        {
            exp = exp.Replace("×", "*").Replace("÷", "/");

            return exp;
        }        

        private double Variance(double[] nums)
        {
            double avg = nums.Average();
            return nums.Select(x => (x - avg) * (x - avg)).Average();
        }

        private double Std(double[] nums)
        {
            return Math.Sqrt(Variance(nums));
        }
    }
}
