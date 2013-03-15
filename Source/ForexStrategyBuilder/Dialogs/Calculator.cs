//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    public sealed class Calculator : Form
    {
        private Regex expression;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Calculator()
        {
            // Test Box Input
            TbxInput = new TextBox {Parent = this, Location = Point.Empty, Size = new Size(190, 20)};
            TbxInput.KeyUp += TbxInputKeyUp;

            // The Form
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = TbxInput.Size;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Opacity = 0.8;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = Language.T("Calculator") + " (F1 - " + Language.T("Help") + ")";
            TopMost = true;

            SetPatterns();
        }

        private TextBox TbxInput { get; set; }

        /// <summary>
        ///     Sets the patterns
        /// </summary>
        private void SetPatterns()
        {
            const string patternNumber = @"(?<numb>\-?\d+([\.,]\d+)?(E[\+\-]\d{1,2})?)";
            const string patternOperator = @"(?<operator>[\+\-\*/\^])";
            const string patternLast = @"(?<last>[^\d\.,E])";

            string operation = string.Format(
                @"{0}\s*{1}\s*{2}\s*{3}",
                patternNumber.Replace("numb", "arg1"),
                patternOperator,
                patternNumber.Replace("numb", "arg2"),
                patternLast);

            expression = new Regex(operation, RegexOptions.Compiled);
        }

        /// <summary>
        ///     Catches the hot keys
        /// </summary>
        private void TbxInputKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ParseInput(TbxInput.Text + "=");
            else if (e.KeyCode == Keys.Escape)
                TbxInput.Clear();
            else if (e.KeyCode == Keys.F1)
                MessageBox.Show(
                    Language.T("Write the mathematical expression in the box.") + Environment.NewLine +
                    Language.T("Enter the first number, the operator and the second number.") + Environment.NewLine +
                    Language.T("To see the result press a key or continue with the next operation.") +
                    Environment.NewLine + Environment.NewLine +
                    Language.T("Addition") + ": 12.34 + 8.8 =" + Environment.NewLine +
                    Language.T("Power") + ": -5.3 ^ 2 =" + Environment.NewLine +
                    Language.T("Percent") + ": 2.2 * 125 %" + Environment.NewLine + Environment.NewLine +
                    Language.T("Operations") + ": + - * / ^ %" + Environment.NewLine + Environment.NewLine +
                    Language.T("Hot keys") + ":" + Environment.NewLine + "   F1 - " +
                    Language.T("Help") + Environment.NewLine + "   Esc - " +
                    Language.T("Clear") + Environment.NewLine + "   F11 / F12 - " +
                    Language.T("Opacity") + Environment.NewLine + "   (Alt + F4) - " +
                    Language.T("Exit"),
                    Language.T("Calculator Help"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else if (e.KeyCode == Keys.F11 && Opacity > 0.4f)
                Opacity -= 0.1f;
            else if (e.KeyCode == Keys.F12 && Opacity < 1.0f)
                Opacity += 0.1f;
            else
                ParseInput(TbxInput.Text);
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void ParseInput(string input)
        {
            string dcmlSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);
            Match match = expression.Match(input);
            double result = double.NaN;

            if (match.Success)
            {
                double arg1 = double.Parse(match.Groups["arg1"].Value);
                double arg2 = double.Parse(match.Groups["arg2"].Value);
                string optr = match.Groups["operator"].Value;
                string last = match.Groups["last"].Value;

                // Addition
                if (optr == "+")
                    result = arg1 + arg2;

                    // Subtraction
                else if (optr == "-")
                    result = arg1 - arg2;

                    // Multiplication
                else if (optr == "*" && last != "%")
                    result = arg1*arg2;

                    // Division
                else if (optr == @"/")
                {
                    if (Math.Abs(arg2 - 0) > 0.000000001) result = arg1/arg2;
                    else if (arg1 > 0) result = double.PositiveInfinity;
                    else if (arg1 < 0) result = double.NegativeInfinity;
                }

                    // Percent
                else if (optr == "*" && last == "%")
                {
                    result = arg1*arg2/100;
                    last = "=";
                }

                    // Power
                else if (optr == "^")
                    result = Math.Pow(arg1, arg2);

                if (Regex.IsMatch(last, @"[\+\-\*/\^]"))
                    last = " " + last + " ";
                else
                    last = " ";

                TbxInput.Clear();
                TbxInput.AppendText(result.ToString(CultureInfo.InvariantCulture) + last);
            }
        }
    }
}