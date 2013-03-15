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
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Market Settings
    /// </summary>
    public sealed class TradingCharges : Form
    {
        private readonly Color colorText;
        private bool editInstrument;

        /// <summary>
        ///     Constructor
        /// </summary>
        public TradingCharges()
        {
            PnlBase = new FancyPanel();

            LblSpread = new Label();
            LblSwapLong = new Label();
            LblSwapShort = new Label();
            LblCommission = new Label();
            LblSlippage = new Label();

            NudSpread = new NumericUpDown();
            NudSwapLong = new NumericUpDown();
            NudSwapShort = new NumericUpDown();
            NudCommission = new NumericUpDown();
            NudSlippage = new NumericUpDown();

            BtnEditInstrument = new Button();
            BtnAccept = new Button();
            BtnCancel = new Button();

            colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Trading Charges") + " - " + Data.Symbol;

            var toolTip = new ToolTip();


            // pnlBase
            PnlBase.Parent = this;

            // Label Spread
            LblSpread.Parent = PnlBase;
            LblSpread.ForeColor = colorText;
            LblSpread.BackColor = Color.Transparent;
            LblSpread.AutoSize = true;
            LblSpread.Text = Language.T("Spread") + " [" + Language.T("pips") + "]";

            // Label Swap Long
            LblSwapLong.Parent = PnlBase;
            LblSwapLong.ForeColor = colorText;
            LblSwapLong.BackColor = Color.Transparent;
            LblSwapLong.AutoSize = true;
            LblSwapLong.Text = Language.T("Swap number for a long position rollover") + " [" +
                               (Data.InstrProperties.SwapType == CommissionType.money
                                    ? Data.InstrProperties.PriceIn
                                    : Language.T(Data.InstrProperties.SwapType.ToString())) + "]" +
                               Environment.NewLine +
                               "(" + Language.T("A positive value decreases your profit.") + ")";

            // Label Swap Short
            LblSwapShort.Parent = PnlBase;
            LblSwapShort.ForeColor = colorText;
            LblSwapShort.BackColor = Color.Transparent;
            LblSwapShort.AutoSize = true;
            LblSwapShort.Text = Language.T("Swap number for a short position rollover") + " [" +
                                (Data.InstrProperties.SwapType == CommissionType.money
                                     ? Data.InstrProperties.PriceIn
                                     : Language.T(Data.InstrProperties.SwapType.ToString())) + "]" +
                                Environment.NewLine +
                                "(" + Language.T("A negative value decreases your profit.") + ")";

            // Label Commission
            LblCommission.Parent = PnlBase;
            LblCommission.ForeColor = colorText;
            LblCommission.BackColor = Color.Transparent;
            LblCommission.AutoSize = true;
            LblCommission.Text = Language.T("Commission in") + " " +
                                 Data.InstrProperties.CommissionTypeToString + " " +
                                 Data.InstrProperties.CommissionScopeToString + " " +
                                 Data.InstrProperties.CommissionTimeToString +
                                 (Data.InstrProperties.CommissionType == CommissionType.money
                                      ? " [" + Data.InstrProperties.PriceIn + "]"
                                      : "");

            // Label Slippage
            LblSlippage.Parent = PnlBase;
            LblSlippage.ForeColor = colorText;
            LblSlippage.BackColor = Color.Transparent;
            LblSlippage.AutoSize = true;
            LblSlippage.Text = Language.T("Slippage") + " [" + Language.T("pips") + "]";

            // NumericUpDown Spread
            NudSpread.BeginInit();
            NudSpread.Parent = PnlBase;
            NudSpread.Name = Language.T("Spread");
            NudSpread.TextAlign = HorizontalAlignment.Center;
            NudSpread.Minimum = 0;
            NudSpread.Maximum = 500;
            NudSpread.Increment = 0.01M;
            NudSpread.DecimalPlaces = 2;
            NudSpread.Value = 4;
            NudSpread.EndInit();
            toolTip.SetToolTip(NudSpread, Language.T("Difference between Bid and Ask prices."));

            // NumericUpDown Swap Long
            NudSwapLong.BeginInit();
            NudSwapLong.Parent = PnlBase;
            NudSwapLong.Name = "SwapLong";
            NudSwapLong.TextAlign = HorizontalAlignment.Center;
            NudSwapLong.Minimum = -500;
            NudSwapLong.Maximum = 500;
            NudSwapLong.Increment = 0.01M;
            NudSwapLong.DecimalPlaces = 2;
            NudSwapLong.Value = 1;
            NudSwapLong.EndInit();
            toolTip.SetToolTip(NudSwapLong,
                               Language.T(
                                   "A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Swap Short
            NudSwapShort.BeginInit();
            NudSwapShort.Parent = PnlBase;
            NudSwapShort.Name = "SwapShort";
            NudSwapShort.TextAlign = HorizontalAlignment.Center;
            NudSwapShort.Minimum = -500;
            NudSwapShort.Maximum = 500;
            NudSwapShort.Increment = 0.01M;
            NudSwapShort.DecimalPlaces = 2;
            NudSwapShort.Value = -1;
            NudSwapShort.EndInit();
            toolTip.SetToolTip(NudSwapShort,
                               Language.T(
                                   "A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Commission
            NudCommission.BeginInit();
            NudCommission.Parent = PnlBase;
            NudCommission.Name = Language.T("Commission");
            NudCommission.TextAlign = HorizontalAlignment.Center;
            NudCommission.Minimum = -500;
            NudCommission.Maximum = 500;
            NudCommission.Increment = 0.01M;
            NudCommission.DecimalPlaces = 2;
            NudCommission.Value = 0;
            NudCommission.EndInit();

            // NumericUpDown Slippage
            NudSlippage.BeginInit();
            NudSlippage.Parent = PnlBase;
            NudSlippage.Name = "Slippage";
            NudSlippage.TextAlign = HorizontalAlignment.Center;
            NudSlippage.Minimum = 0;
            NudSlippage.Maximum = 200;
            NudSlippage.Increment = 1;
            NudSlippage.Value = 0;
            NudSlippage.EndInit();
            toolTip.SetToolTip(NudSlippage, Language.T("Number of pips you lose due to an inaccurate order execution."));

            //Button btnEditInstrument
            BtnEditInstrument.Parent = this;
            BtnEditInstrument.Name = "EditInstrument";
            BtnEditInstrument.Text = Language.T("More");
            BtnEditInstrument.Click += BtnEditInstrumentClick;
            BtnEditInstrument.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlBase { get; set; }

        private Label LblSpread { get; set; }
        private Label LblSwapLong { get; set; }
        private Label LblSwapShort { get; set; }
        private Label LblCommission { get; set; }
        private Label LblSlippage { get; set; }

        private NumericUpDown NudSpread { get; set; }
        private NumericUpDown NudSwapLong { get; set; }
        private NumericUpDown NudSwapShort { get; set; }
        private NumericUpDown NudCommission { get; set; }
        private NumericUpDown NudSlippage { get; set; }

        private Button BtnEditInstrument { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }


        /// <summary>
        ///     Spread
        /// </summary>
        public double Spread
        {
            get { return (double) NudSpread.Value; }
            set { NudSpread.Value = (decimal) value; }
        }

        /// <summary>
        ///     Swap Long
        /// </summary>
        public double SwapLong
        {
            get { return (double) NudSwapLong.Value; }
            set { NudSwapLong.Value = (decimal) value; }
        }

        /// <summary>
        ///     Swap Short
        /// </summary>
        public double SwapShort
        {
            get { return (double) NudSwapShort.Value; }
            set { NudSwapShort.Value = (decimal) value; }
        }

        /// <summary>
        ///     Commission
        /// </summary>
        public double Commission
        {
            get { return (double) NudCommission.Value; }
            set { NudCommission.Value = (decimal) value; }
        }

        /// <summary>
        ///     Slippage
        /// </summary>
        public int Slippage
        {
            get { return (int) NudSlippage.Value; }
            set { NudSlippage.Value = value; }
        }

        /// <summary>
        ///     Whether to edit the instrument
        /// </summary>
        public bool EditInstrument
        {
            get { return editInstrument; }
        }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(350, 208);

            BtnAccept.Focus();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            const int border = 2;
            const int nudWidth = 70;

            // pnlBase
            PnlBase.Size = new Size(ClientSize.Width - 2*space,
                                    ClientSize.Height - 2*btnVertSpace - buttonHeight - space);
            PnlBase.Location = new Point(space, space);

            // Labels
            LblSpread.Location = new Point(btnHrzSpace + border, 0*buttonHeight + 1*space + 8);
            LblSwapLong.Location = new Point(btnHrzSpace + border, 1*buttonHeight + 2*space + 2);
            LblSwapShort.Location = new Point(btnHrzSpace + border, 2*buttonHeight + 3*space + 2);
            LblCommission.Location = new Point(btnHrzSpace + border, 3*buttonHeight + 4*space + 8);
            LblSlippage.Location = new Point(btnHrzSpace + border, 4*buttonHeight + 5*space + 8);

            // FancyNud Parameters
            int nudLeft = PnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            NudSpread.Size = new Size(nudWidth, buttonHeight);
            NudSpread.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 6);
            NudSwapLong.Size = new Size(nudWidth, buttonHeight);
            NudSwapLong.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 6);
            NudSwapShort.Size = new Size(nudWidth, buttonHeight);
            NudSwapShort.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 6);
            NudCommission.Size = new Size(nudWidth, buttonHeight);
            NudCommission.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 6);
            NudSlippage.Size = new Size(nudWidth, buttonHeight);
            NudSlippage.Location = new Point(nudLeft, 4*buttonHeight + 5*space + 6);

            // Button btnEditInstrument
            BtnEditInstrument.Size = new Size(buttonWidth, buttonHeight);
            BtnEditInstrument.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            //btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            //btnDefault.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Resize if necessary
            int iMaxLblRight = LblSpread.Right;
            if (LblSwapLong.Right > iMaxLblRight) iMaxLblRight = LblSwapLong.Right;
            if (LblSwapShort.Right > iMaxLblRight) iMaxLblRight = LblSwapShort.Right;
            if (LblCommission.Right > iMaxLblRight) iMaxLblRight = LblCommission.Right;
            if (LblSlippage.Right > iMaxLblRight) iMaxLblRight = LblSlippage.Right;

            if (nudLeft - iMaxLblRight < btnVertSpace)
                Width += btnVertSpace - nudLeft + iMaxLblRight;
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Shows Instrument Editor
        /// </summary>
        private void BtnEditInstrumentClick(object sender, EventArgs e)
        {
            editInstrument = true;
            Close();
        }
    }
}