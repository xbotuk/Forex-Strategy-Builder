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
    ///     Account Settings
    /// </summary>
    public sealed class AccountSettings : Form
    {
        private readonly Color colorText;
        private string accountCurrency;
        private int initialAccount;
        private int leverage;
        private double rateToEUR;
        private double rateToUSD;

        /// <summary>
        ///     Constructor
        /// </summary>
        public AccountSettings()
        {
            PnlBase = new FancyPanel();

            LblAccountCurrency = new Label();
            LblInitialAccount = new Label();
            LblLeverage = new Label();
            LblExchangeRate = new Label();
            LblExchangeRateInfo = new Label();

            CbxAccountCurrency = new ComboBox();
            NudInitialAccount = new NumericUpDown();
            CbxLeverage = new ComboBox();
            NudExchangeRate = new NumericUpDown();
            TbxExchangeRate = new TextBox();

            BtnDefault = new Button();
            BtnCancel = new Button();
            BtnAccept = new Button();

            colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Account Settings");

            // pnlBase
            PnlBase.Parent = this;

            // Label Account Currency
            LblAccountCurrency.Parent = PnlBase;
            LblAccountCurrency.ForeColor = colorText;
            LblAccountCurrency.BackColor = Color.Transparent;
            LblAccountCurrency.Text = Language.T("Account currency");
            LblAccountCurrency.AutoSize = true;

            // Label Initial Account
            LblInitialAccount.Parent = PnlBase;
            LblInitialAccount.ForeColor = colorText;
            LblInitialAccount.BackColor = Color.Transparent;
            LblInitialAccount.Text = Language.T("Initial account");
            LblInitialAccount.AutoSize = true;

            // Label Leverage
            LblLeverage.Parent = PnlBase;
            LblLeverage.ForeColor = colorText;
            LblLeverage.BackColor = Color.Transparent;
            LblLeverage.Text = Language.T("Leverage");
            LblLeverage.AutoSize = true;

            // Label Exchange Rate
            LblExchangeRate.Parent = PnlBase;
            LblExchangeRate.ForeColor = colorText;
            LblExchangeRate.BackColor = Color.Transparent;
            LblExchangeRate.Text = Language.T("Account exchange rate");
            LblExchangeRate.AutoSize = true;

            // Label Exchange Rate Info
            LblExchangeRateInfo.Parent = PnlBase;
            LblExchangeRateInfo.ForeColor = colorText;
            LblExchangeRateInfo.BackColor = Color.Transparent;
            LblExchangeRateInfo.Text =
                Language.T(
                    "Forex Strategy Builder uses the account exchange rate to calculate the trading statistics in your account currency.") +
                " " +
                Language.T(
                    "When your account currency does not take part in the trading couple the account exchange rate is a fixed figure.");

            // ComboBox Account Currency
            CbxAccountCurrency.Parent = PnlBase;
            CbxAccountCurrency.Name = "cbxAccountCurrency";
            CbxAccountCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxAccountCurrency.Items.AddRange(new object[] {"USD", "EUR"});
            CbxAccountCurrency.SelectedIndex = 0;

            // NumericUpDown Initial Account
            NudInitialAccount.Parent = PnlBase;
            NudInitialAccount.Name = "nudInitialAccount";
            NudInitialAccount.BeginInit();
            NudInitialAccount.Minimum = 100;
            NudInitialAccount.Maximum = 100000;
            NudInitialAccount.Increment = 1000;
            NudInitialAccount.Value = initialAccount;
            NudInitialAccount.EndInit();

            // ComboBox Leverage
            CbxLeverage.Parent = PnlBase;
            CbxLeverage.Name = "cbxLeverage";
            CbxLeverage.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxLeverage.Items.AddRange(new object[]
                {"1/1", "1/10", "1/20", "1/30", "1/50", "1/100", "1/200", "1/300", "1/400", "1/500"});
            CbxLeverage.SelectedIndex = 5;

            // tbxExchangeRate
            TbxExchangeRate.Parent = PnlBase;
            TbxExchangeRate.BackColor = LayoutColors.ColorControlBack;
            TbxExchangeRate.ForeColor = colorText;
            TbxExchangeRate.ReadOnly = true;
            TbxExchangeRate.Visible = false;
            TbxExchangeRate.Text = Language.T("Deal price");

            // NumericUpDown Exchange Rate
            NudExchangeRate.BeginInit();
            NudExchangeRate.Parent = PnlBase;
            NudExchangeRate.Name = "nudExchangeRate";
            NudExchangeRate.Minimum = 0;
            NudExchangeRate.Maximum = 100000;
            NudExchangeRate.Increment = 0.0001M;
            NudExchangeRate.DecimalPlaces = 4;
            NudExchangeRate.Value = 1;
            NudExchangeRate.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "Default";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

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

        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnDefault { get; set; }
        private ComboBox CbxAccountCurrency { get; set; }
        private ComboBox CbxLeverage { get; set; }
        private Label LblAccountCurrency { get; set; }
        private Label LblExchangeRate { get; set; }
        private Label LblExchangeRateInfo { get; set; }
        private Label LblInitialAccount { get; set; }
        private Label LblLeverage { get; set; }

        private NumericUpDown NudExchangeRate { get; set; }
        private NumericUpDown NudInitialAccount { get; set; }
        private FancyPanel PnlBase { get; set; }
        private TextBox TbxExchangeRate { get; set; }

        /// <summary>
        ///     Account Currency
        /// </summary>
        public string AccountCurrency
        {
            get { return accountCurrency; }
            set { accountCurrency = value; }
        }

        /// <summary>
        ///     Initial Account
        /// </summary>
        public int InitialAccount
        {
            get { return initialAccount; }
            set { initialAccount = value; }
        }

        /// <summary>
        ///     Leverage
        /// </summary>
        public int Leverage
        {
            get { return leverage; }
            set { leverage = value; }
        }

        /// <summary>
        ///     Exchange Rate to USD
        /// </summary>
        public double RateToUSD
        {
            get { return rateToUSD; }
            set { rateToUSD = value; }
        }

        /// <summary>
        ///     Exchange Rate to EUR
        /// </summary>
        public double RateToEur
        {
            get { return rateToEUR; }
            set { rateToEUR = value; }
        }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);

            var height = (int)(260 * Data.VDpiScale);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, height);

            CbxAccountCurrency.SelectedIndexChanged += ParamChanged;
            NudInitialAccount.ValueChanged += ParamChanged;
            CbxLeverage.SelectedIndexChanged += ParamChanged;
            NudExchangeRate.ValueChanged += ParamChanged;

            BtnAccept.Focus();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            int nudWidth = buttonWidth - space - 1;
            const int border = 2;

            // pnlBase
            PnlBase.Size = new Size(ClientSize.Width - 2*space,
                                    ClientSize.Height - 2*btnVertSpace - buttonHeight - space);
            PnlBase.Location = new Point(space, space);

            // Labels
            LblAccountCurrency.Location = new Point(border + btnHrzSpace, 0*buttonHeight + 1*space + 8);
            LblInitialAccount.Location = new Point(border + btnHrzSpace, 1*buttonHeight + 2*space + 6);
            LblLeverage.Location = new Point(border + btnHrzSpace, 2*buttonHeight + 3*space + 8);
            LblExchangeRate.Location = new Point(border + btnHrzSpace, 3*buttonHeight + 4*space + 7);
            LblExchangeRateInfo.Location = new Point(border + btnHrzSpace, 4*buttonHeight + 5*space + 8);
            LblExchangeRateInfo.Size = new Size(PnlBase.ClientSize.Width - 2*space - 2*border,
                                                PnlBase.ClientSize.Height - border - space - LblExchangeRateInfo.Top);

            // Params
            int nudLeft = PnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            CbxAccountCurrency.Size = new Size(nudWidth, buttonHeight);
            CbxAccountCurrency.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 4);
            NudInitialAccount.Size = new Size(nudWidth, buttonHeight);
            NudInitialAccount.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 4);
            CbxLeverage.Size = new Size(nudWidth, buttonHeight);
            CbxLeverage.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 4);
            NudExchangeRate.Size = new Size(nudWidth, buttonHeight);
            NudExchangeRate.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 4);
            TbxExchangeRate.Size = new Size(nudWidth, buttonHeight);
            TbxExchangeRate.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 4);

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                            ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnDefault.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Sets the controls' text
        /// </summary>
        public void SetParams()
        {
            // ComboBox Account Currency
            CbxAccountCurrency.SelectedItem = accountCurrency;

            // NumericUpDown Initial Account
            NudInitialAccount.Value = initialAccount;

            // ComboBox Leverage
            CbxLeverage.SelectedItem = "1/" + leverage;

            SetAcountExchangeRate();
        }

        /// <summary>
        ///     Calculates the account exchange rate.
        /// </summary>
        private void SetAcountExchangeRate()
        {
            LblExchangeRate.Text = Language.T("Account exchange rate");

            if (Data.InstrProperties.PriceIn == CbxAccountCurrency.Text)
            {
                TbxExchangeRate.Text = Language.T("Not used");
                TbxExchangeRate.Visible = true;
                NudExchangeRate.Value = 1;
                NudExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    rateToUSD = 1;
                else if (CbxAccountCurrency.Text == "EUR")
                    rateToEUR = 1;
            }
            else if (Data.InstrProperties.InstrType == InstrumetType.Forex &&
                     Data.InstrProperties.Symbol.StartsWith(CbxAccountCurrency.Text))
            {
                TbxExchangeRate.Text = Language.T("Deal price");
                TbxExchangeRate.Visible = true;
                NudExchangeRate.Value = 0;
                NudExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    rateToUSD = 0;
                else if (CbxAccountCurrency.Text == "EUR")
                    rateToEUR = 0;
            }
            else
            {
                LblExchangeRate.Text += " " + CbxAccountCurrency.Text + Data.InstrProperties.PriceIn;
                TbxExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    NudExchangeRate.Value = (decimal) rateToUSD;
                else if (CbxAccountCurrency.Text == "EUR")
                    NudExchangeRate.Value = (decimal) rateToEUR;
                NudExchangeRate.Visible = true;
            }
        }

        /// <summary>
        ///     Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            string name = ((Control) sender).Name;

            // ComboBox Account Currency
            if (name == "cbxAccountCurrency")
            {
                accountCurrency = CbxAccountCurrency.Text;
                SetAcountExchangeRate();
            }

            // NumericUpDown Initial Account
            if (name == "nudInitialAccount")
            {
                initialAccount = (int) NudInitialAccount.Value;
            }

            // ComboBox Leverage
            if (name == "cbxLeverage")
            {
                leverage = int.Parse(CbxLeverage.Text.Substring(2));
            }

            // NumericUpDown Exchange Rate
            if (name == "nudExchangeRate")
            {
                if (CbxAccountCurrency.Text == "USD")
                    rateToUSD = (double) NudExchangeRate.Value;
                else if (CbxAccountCurrency.Text == "EUR")
                    rateToEUR = (double) NudExchangeRate.Value;
            }
        }

        /// <summary>
        ///     Button Default Click
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            accountCurrency = "USD";
            initialAccount = 10000;
            leverage = 100;

            SetParams();
        }
    }
}