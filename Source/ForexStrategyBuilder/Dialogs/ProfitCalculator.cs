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
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Profit Calculator
    /// </summary>
    public sealed class ProfitCalculator : Form
    {
        private readonly Color colorText;

        private string symbol;

        /// <summary>
        ///     Constructor
        /// </summary>
        public ProfitCalculator()
        {
            PnlInput = new FancyPanel(Language.T("Input Values"));
            PnlOutput = new FancyPanel(Language.T("Output Values"));

            AlblInputNames = new Label[6];
            AlblOutputNames = new Label[8];
            AlblOutputValues = new Label[8];

            LblLotSize = new Label();
            CbxDirection = new ComboBox();
            NudLots = new NumericUpDown();
            NudEntryPrice = new NumericUpDown();
            NudExitPrice = new NumericUpDown();
            NudDays = new NumericUpDown();

            colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("Profit Calculator");

            // Input
            PnlInput.Parent = this;

            // Output
            PnlOutput.Parent = this;

            // Input Names
            var asInputNames = new[]
                {
                    Data.InstrProperties.Symbol,
                    Language.T("Direction"),
                    Language.T("Number of lots"),
                    Language.T("Entry price"),
                    Language.T("Exit price"),
                    Language.T("Days rollover")
                };

            int number = 0;
            foreach (string name in asInputNames)
            {
                AlblInputNames[number] = new Label
                    {
                        Parent = PnlInput,
                        ForeColor = colorText,
                        BackColor = Color.Transparent,
                        AutoSize = true,
                        Text = name
                    };
                number++;
            }

            // Label Lot size
            LblLotSize.Parent = PnlInput;
            LblLotSize.ForeColor = colorText;
            LblLotSize.BackColor = Color.Transparent;

            // ComboBox SameDirAction
            CbxDirection.Parent = PnlInput;
            CbxDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxDirection.Items.AddRange(new object[] {Language.T("Long"), Language.T("Short")});
            CbxDirection.SelectedIndex = 0;

            // Lots
            NudLots.Parent = PnlInput;
            NudLots.BeginInit();
            NudLots.Minimum = 0.01M;
            NudLots.Maximum = 100;
            NudLots.Increment = 0.01M;
            NudLots.DecimalPlaces = 2;
            NudLots.Value = (decimal) Data.Strategy.EntryLots;
            NudLots.EndInit();

            // NumericUpDown Entry Price
            NudEntryPrice.Parent = PnlInput;

            // NumericUpDown Exit Price
            NudExitPrice.Parent = PnlInput;

            // NumericUpDown Reducing Lots
            NudDays.Parent = PnlInput;
            NudDays.BeginInit();
            NudDays.Minimum = 0;
            NudDays.Maximum = 1000;
            NudDays.Increment = 1;
            NudDays.Value = 1;
            NudDays.EndInit();

            // Output Names
            var asOutputNames = new[]
                {
                    Language.T("Required margin"),
                    Language.T("Gross profit"),
                    Language.T("Spread"),
                    Language.T("Entry commission"),
                    Language.T("Exit commission"),
                    Language.T("Rollover"),
                    Language.T("Slippage"),
                    Language.T("Net profit")
                };

            number = 0;
            foreach (string name in asOutputNames)
            {
                AlblOutputNames[number] = new Label
                    {
                        Parent = PnlOutput,
                        ForeColor = colorText,
                        BackColor = Color.Transparent,
                        AutoSize = true,
                        Text = name
                    };

                AlblOutputValues[number] = new Label
                    {
                        Parent = PnlOutput,
                        ForeColor = colorText,
                        BackColor = Color.Transparent,
                        AutoSize = true
                    };

                number++;
            }

            AlblOutputNames[number - 1].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            AlblOutputValues[number - 1].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            Timer = new Timer {Interval = 2000};
            Timer.Tick += TimerTick;
            Timer.Start();
        }

        private FancyPanel PnlInput { get; set; }
        private FancyPanel PnlOutput { get; set; }

        private Label[] AlblInputNames { get; set; }
        private Label[] AlblOutputNames { get; set; }
        private Label[] AlblOutputValues { get; set; }

        private Label LblLotSize { get; set; }
        private ComboBox CbxDirection { get; set; }
        private NumericUpDown NudLots { get; set; }
        private NumericUpDown NudEntryPrice { get; set; }
        private NumericUpDown NudExitPrice { get; set; }
        private NumericUpDown NudDays { get; set; }

        private Timer Timer { get; set; }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CbxDirection.SelectedIndexChanged += ParamChanged;
            NudLots.ValueChanged += ParamChanged;
            NudEntryPrice.ValueChanged += ParamChanged;
            NudExitPrice.ValueChanged += ParamChanged;
            NudDays.ValueChanged += ParamChanged;

            var width = (int)(270 * Data.HDpiScale);
            var height = (int)(405 * Data.VDpiScale);
            ClientSize = new Size(width, height);

            InitParams();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int border = btnHrzSpace;
            var width = (int)(100 * Data.HDpiScale);

            // pnlInput
            PnlInput.Size = new Size(ClientSize.Width - 2*border, (int) (190 * Data.VDpiScale));
            PnlInput.Location = new Point(border, border);

            int left = PnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            LblLotSize.Width = width;
            CbxDirection.Width = width;
            NudLots.Width = width;
            NudEntryPrice.Width = width;
            NudExitPrice.Width = width;
            NudDays.Width = width;

            int shift = 22;
            int vertSpace = 2;
            LblLotSize.Location = new Point(left, 0*buttonHeight + 1*vertSpace + shift - 0);
            CbxDirection.Location = new Point(left, 1*buttonHeight + 2*vertSpace + shift - 4);
            NudLots.Location = new Point(left, 2*buttonHeight + 3*vertSpace + shift - 4);
            NudEntryPrice.Location = new Point(left, 3*buttonHeight + 4*vertSpace + shift - 4);
            NudExitPrice.Location = new Point(left, 4*buttonHeight + 5*vertSpace + shift - 4);
            NudDays.Location = new Point(left, 5*buttonHeight + 6*vertSpace + shift - 4);

            int numb = 0;
            foreach (Label lbl in AlblInputNames)
            {
                lbl.Location = new Point(border, numb*buttonHeight + (numb + 1)*vertSpace + shift);
                numb++;
            }

            // pnlOutput
            PnlOutput.Size = new Size(ClientSize.Width - 2 * border, (int) (200 * Data.VDpiScale));
            PnlOutput.Location = new Point(border, PnlInput.Bottom + border);

            shift = 24;
            vertSpace = -4;
            numb = 0;
            foreach (Label lbl in AlblOutputNames)
            {
                lbl.Location = new Point(border, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }

            numb = 0;
            foreach (Label lbl in AlblOutputValues)
            {
                lbl.Location = new Point(left, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }
        }

        /// <summary>
        ///     Perform periodical action.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            if (symbol == Data.InstrProperties.Symbol) return;
            InitParams();
            InitParams();
        }

        /// <summary>
        ///     Sets the initial params.
        /// </summary>
        private void InitParams()
        {
            symbol = Data.InstrProperties.Symbol;

            AlblInputNames[0].Text = symbol;
            LblLotSize.Text = Data.InstrProperties.LotSize.ToString(CultureInfo.InvariantCulture);

            // NumericUpDown Entry Price
            NudEntryPrice.BeginInit();
            NudEntryPrice.DecimalPlaces = Data.InstrProperties.Digits;
            NudEntryPrice.Minimum = (decimal) (Data.MinPrice*0.7);
            NudEntryPrice.Maximum = (decimal) (Data.MaxPrice*1.3);
            NudEntryPrice.Increment = (decimal) Data.InstrProperties.Point;
            NudEntryPrice.Value = (decimal) Data.Close[Data.Bars - 1];
            NudEntryPrice.EndInit();

            // NumericUpDown Exit Price
            NudExitPrice.BeginInit();
            NudExitPrice.DecimalPlaces = Data.InstrProperties.Digits;
            NudExitPrice.Minimum = (decimal) (Data.MinPrice*0.7);
            NudExitPrice.Maximum = (decimal) (Data.MaxPrice*1.3);
            NudExitPrice.Increment = (decimal) Data.InstrProperties.Point;
            NudExitPrice.Value = (decimal) (Data.Close[Data.Bars - 1] + 100*Data.InstrProperties.Point);
            NudExitPrice.EndInit();

            Calculate();
        }

        /// <summary>
        ///     Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        ///     Calculates the result
        /// </summary>
        private void Calculate()
        {
            bool isLong = (CbxDirection.SelectedIndex == 0);
            PosDirection posDir = isLong ? PosDirection.Long : PosDirection.Short;
            int lotSize = Data.InstrProperties.LotSize;
            var lots = (double) NudLots.Value;
            var entryPrice = (double) NudEntryPrice.Value;
            var exitPrice = (double) NudExitPrice.Value;
            var daysRollover = (int) NudDays.Value;
            double point = Data.InstrProperties.Point;
            string unit = " " + Configs.AccountCurrency;
            double entryValue = lots*lotSize*entryPrice;
            double exitValue = lots*lotSize*exitPrice;

            // Required margin
            double requiredMargin = (lots*lotSize/Configs.Leverage)*
                                    (entryPrice/Backtester.AccountExchangeRate(entryPrice));
            AlblOutputValues[0].Text = requiredMargin.ToString("F2") + unit;

            // Gross Profit
            double grossProfit = (isLong ? exitValue - entryValue : entryValue - exitValue)/
                                 Backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[1].Text = grossProfit.ToString("F2") + unit;

            // Spread
            double spread = Data.InstrProperties.Spread*point*lots*lotSize/Backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[2].Text = spread.ToString("F2") + unit;

            // Entry Commission
            double entryCommission = Backtester.CommissionInMoney(lots, entryPrice, false);
            AlblOutputValues[3].Text = entryCommission.ToString("F2") + unit;

            // Exit Commission
            double exitCommission = Backtester.CommissionInMoney(lots, exitPrice, true);
            AlblOutputValues[4].Text = exitCommission.ToString("F2") + unit;

            // Rollover
            double rollover = Backtester.RolloverInMoney(posDir, lots, daysRollover, exitPrice);
            AlblOutputValues[5].Text = rollover.ToString("F2") + unit;

            // Slippage
            double slippage = Data.InstrProperties.Slippage*point*lots*lotSize/Backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[6].Text = slippage.ToString("F2") + unit;

            // Net Profit
            double netProfit = grossProfit - entryCommission - exitCommission - rollover - slippage;
            AlblOutputValues[7].Text = netProfit.ToString("F2") + unit;
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}