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
    public sealed class StrategyProperties : Form
    {
        private int leftPanelsWidth;
        private int rightPanelsWidth;

        public StrategyProperties()
        {
            PnlAveraging = new FancyPanel(Language.T("Handling of Additional Entry Signals"),
                                          LayoutColors.ColorSlotCaptionBackAveraging);
            PnlAmounts = new FancyPanel(Language.T("Trading Size"), LayoutColors.ColorSlotCaptionBackAveraging);
            PnlProtection = new FancyPanel(Language.T("Permanent Protection"),
                                           LayoutColors.ColorSlotCaptionBackAveraging);
            BalanceChart = new SmallBalanceChart();

            LblPercent1 = new Label();
            LblPercent2 = new Label();
            LblPercent3 = new Label();

            LblSameDirAction = new Label();
            LblOppDirAction = new Label();

            CbxSameDirAction = new ComboBox();
            CbxOppDirAction = new ComboBox();
            FancyNudMaxOpenLots = new FancyNud();
            RbConstantUnits = new RadioButton();
            RbVariableUnits = new RadioButton();
            FancyNudEntryLots = new FancyNud();
            FancyNudAddingLots = new FancyNud();
            FancyNudReducingLots = new FancyNud();
            LblMaxOpenLots = new Label();
            LblEntryLots = new Label();
            LblAddingLots = new Label();
            LblReducingLots = new Label();
            CbxUseMartingale = new CheckBox();
            FancyNudMartingaleMultiplier = new FancyNud();

            ChbPermaSL = new CheckBox();
            CbxPermaSLType = new ComboBox();
            FancyNudPermaSL = new FancyNud();
            ChbPermaTP = new CheckBox();
            CbxPermaTPType = new ComboBox();
            FancyNudPermaTP = new FancyNud();
            ChbBreakEven = new CheckBox();
            FancyNudBreakEven = new FancyNud();

            BtnAccept = new Button();
            BtnDefault = new Button();
            BtnCancel = new Button();

            ColorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Strategy Properties");

            PnlAveraging.Parent = this;
            PnlAmounts.Parent = this;
            PnlProtection.Parent = this;
            BalanceChart.Parent = this;
            BalanceChart.SetChartData();

            var toolTip = new ToolTip();

            // Label Same dir action
            LblSameDirAction.Parent = PnlAveraging;
            LblSameDirAction.ForeColor = ColorText;
            LblSameDirAction.BackColor = Color.Transparent;
            LblSameDirAction.AutoSize = true;
            LblSameDirAction.Text = Language.T("Next same direction signal behavior");

            // Label Opposite dir action
            LblOppDirAction.Parent = PnlAveraging;
            LblOppDirAction.ForeColor = ColorText;
            LblOppDirAction.BackColor = Color.Transparent;
            LblOppDirAction.AutoSize = true;
            LblOppDirAction.Text = Language.T("Next opposite direction signal behavior");

            // ComboBox SameDirAction
            CbxSameDirAction.Parent = PnlAveraging;
            CbxSameDirAction.Name = "cbxSameDirAction";
            CbxSameDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            var sameItems = new[] {Language.T("Nothing"), Language.T("Winner"), Language.T("Add")};
            foreach (string item in sameItems)
                CbxSameDirAction.Items.Add(item);
            CbxSameDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxSameDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Winner - adds to a winning position.") + Environment.NewLine +
                               Language.T("Add - adds to all positions."));

            // ComboBox OppDirAction
            CbxOppDirAction.Parent = PnlAveraging;
            CbxOppDirAction.Name = "cbxOppDirAction";
            CbxOppDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            var oppItems = new[]
                {Language.T("Nothing"), Language.T("Reduce"), Language.T("Close"), Language.T("Reverse")};
            foreach (string item in oppItems)
                CbxOppDirAction.Items.Add(item);
            CbxOppDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxOppDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Reduce - reduces or closes a position.") + Environment.NewLine +
                               Language.T("Close - closes the position.") + Environment.NewLine +
                               Language.T("Reverse - reverses the position."));

            // Label MaxOpen Lots
            LblMaxOpenLots.Parent = PnlAmounts;
            LblMaxOpenLots.ForeColor = ColorText;
            LblMaxOpenLots.BackColor = Color.Transparent;
            LblMaxOpenLots.AutoSize = true;
            LblMaxOpenLots.Text = Language.T("Maximum number of open lots");

            // NumericUpDown MaxOpen Lots
            FancyNudMaxOpenLots.Parent = PnlAmounts;
            FancyNudMaxOpenLots.Name = "nudMaxOpenLots";
            FancyNudMaxOpenLots.BeginInit();
            FancyNudMaxOpenLots.Minimum = 0.01M;
            FancyNudMaxOpenLots.Maximum = 100;
            FancyNudMaxOpenLots.Increment = 0.01M;
            FancyNudMaxOpenLots.Value = 20;
            FancyNudMaxOpenLots.DecimalPlaces = 2;
            FancyNudMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            FancyNudMaxOpenLots.EndInit();

            // Radio Button Constant Units
            RbConstantUnits.Parent = PnlAmounts;
            RbConstantUnits.ForeColor = ColorText;
            RbConstantUnits.BackColor = Color.Transparent;
            RbConstantUnits.Checked = true;
            RbConstantUnits.AutoSize = true;
            RbConstantUnits.Name = "rbConstantUnits";
            RbConstantUnits.Text = Language.T("Trade a constant number of lots");

            // Radio Button Variable Units
            RbVariableUnits.Parent = PnlAmounts;
            RbVariableUnits.ForeColor = ColorText;
            RbVariableUnits.BackColor = Color.Transparent;
            RbVariableUnits.Checked = false;
            RbVariableUnits.AutoSize = false;
            RbVariableUnits.Name = "rbVariableUnits";
            RbVariableUnits.Text =
                Language.T(
                    "Trade percent of your account. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            LblEntryLots.Parent = PnlAmounts;
            LblEntryLots.ForeColor = ColorText;
            LblEntryLots.BackColor = Color.Transparent;
            LblEntryLots.AutoSize = true;
            LblEntryLots.Text = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            FancyNudEntryLots.Parent = PnlAmounts;
            FancyNudEntryLots.Name = "nudEntryLots";
            FancyNudEntryLots.BeginInit();
            FancyNudEntryLots.Minimum = 0.01M;
            FancyNudEntryLots.Maximum = 100;
            FancyNudEntryLots.Increment = 0.01M;
            FancyNudEntryLots.Value = 1;
            FancyNudEntryLots.DecimalPlaces = 2;
            FancyNudEntryLots.TextAlign = HorizontalAlignment.Center;
            FancyNudEntryLots.EndInit();

            // Label Entry Lots %
            LblPercent1.Parent = PnlAmounts;
            LblPercent1.ForeColor = ColorText;
            LblPercent1.BackColor = Color.Transparent;

            // Label Adding Lots
            LblAddingLots.Parent = PnlAmounts;
            LblAddingLots.ForeColor = ColorText;
            LblAddingLots.BackColor = Color.Transparent;
            LblAddingLots.AutoSize = true;
            LblAddingLots.Text = Language.T("In case of addition - number of lots to add");

            // NumericUpDown Adding Lots
            FancyNudAddingLots.Parent = PnlAmounts;
            FancyNudAddingLots.Name = "nudAddingLots";
            FancyNudAddingLots.BeginInit();
            FancyNudAddingLots.Minimum = 0.01M;
            FancyNudAddingLots.Maximum = 100;
            FancyNudAddingLots.Increment = 0.01M;
            FancyNudAddingLots.Value = 1;
            FancyNudAddingLots.DecimalPlaces = 2;
            FancyNudAddingLots.TextAlign = HorizontalAlignment.Center;
            FancyNudAddingLots.EndInit();

            // Label Adding Lots %
            LblPercent2.Parent = PnlAmounts;
            LblPercent2.ForeColor = ColorText;
            LblPercent2.BackColor = Color.Transparent;

            // Label Reducing Lots
            LblReducingLots.Parent = PnlAmounts;
            LblReducingLots.ForeColor = ColorText;
            LblReducingLots.BackColor = Color.Transparent;
            LblReducingLots.AutoSize = true;
            LblReducingLots.Text = Language.T("In case of reduction - number of lots to close");

            // NumericUpDown Reducing Lots
            FancyNudReducingLots.Parent = PnlAmounts;
            FancyNudReducingLots.Name = "nudReducingLots";
            FancyNudReducingLots.BeginInit();
            FancyNudReducingLots.Minimum = 0.01M;
            FancyNudReducingLots.Maximum = 100;
            FancyNudReducingLots.Increment = 0.01m;
            FancyNudReducingLots.DecimalPlaces = 2;
            FancyNudReducingLots.Value = 1;
            FancyNudReducingLots.TextAlign = HorizontalAlignment.Center;
            FancyNudReducingLots.EndInit();

            // CheckBox Use Martingale
            CbxUseMartingale.Name = "cbxUseMartingale";
            CbxUseMartingale.Parent = PnlAmounts;
            CbxUseMartingale.ForeColor = ColorText;
            CbxUseMartingale.BackColor = Color.Transparent;
            CbxUseMartingale.AutoCheck = true;
            CbxUseMartingale.AutoSize = true;
            CbxUseMartingale.Checked = false;
            CbxUseMartingale.Text = Language.T("Martingale money management multiplier");

            // NumericUpDown Martingale Multiplier
            FancyNudMartingaleMultiplier.Parent = PnlAmounts;
            FancyNudMartingaleMultiplier.Name = "nudMartingaleMultiplier";
            FancyNudMartingaleMultiplier.BeginInit();
            FancyNudMartingaleMultiplier.Minimum = 0.01M;
            FancyNudMartingaleMultiplier.Maximum = 10;
            FancyNudMartingaleMultiplier.Increment = 0.01m;
            FancyNudMartingaleMultiplier.DecimalPlaces = 2;
            FancyNudMartingaleMultiplier.Value = 2;
            FancyNudMartingaleMultiplier.TextAlign = HorizontalAlignment.Center;
            FancyNudMartingaleMultiplier.EndInit();


            // Label Reducing Lots %
            LblPercent3.Parent = PnlAmounts;
            LblPercent3.ForeColor = ColorText;
            LblPercent3.BackColor = Color.Transparent;

            // CheckBox Permanent Stop Loss
            ChbPermaSL.Name = "chbPermaSL";
            ChbPermaSL.Parent = PnlProtection;
            ChbPermaSL.ForeColor = ColorText;
            ChbPermaSL.BackColor = Color.Transparent;
            ChbPermaSL.AutoCheck = true;
            ChbPermaSL.AutoSize = true;
            ChbPermaSL.Checked = false;
            ChbPermaSL.Text = Language.T("Permanent Stop Loss");

            // ComboBox cbxPermaSLType
            CbxPermaSLType.Parent = PnlProtection;
            CbxPermaSLType.Name = "cbxPermaSLType";
            CbxPermaSLType.Visible = false;
            CbxPermaSLType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaSLType.Items.Add(Language.T("Relative"));
            CbxPermaSLType.Items.Add(Language.T("Absolute"));
            CbxPermaSLType.SelectedIndex = 0;

            // NumericUpDown Permanent S/L
            FancyNudPermaSL.Name = "nudPermaSL";
            FancyNudPermaSL.Parent = PnlProtection;
            FancyNudPermaSL.BeginInit();
            FancyNudPermaSL.Minimum = 5;
            FancyNudPermaSL.Maximum = 5000;
            FancyNudPermaSL.Increment = 1;
            FancyNudPermaSL.Value = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            FancyNudPermaSL.TextAlign = HorizontalAlignment.Center;
            FancyNudPermaSL.EndInit();

            // CheckBox Permanent Take Profit
            ChbPermaTP.Name = "chbPermaTP";
            ChbPermaTP.Parent = PnlProtection;
            ChbPermaTP.ForeColor = ColorText;
            ChbPermaTP.BackColor = Color.Transparent;
            ChbPermaTP.AutoCheck = true;
            ChbPermaTP.AutoSize = true;
            ChbPermaTP.Checked = false;
            ChbPermaTP.Text = Language.T("Permanent Take Profit");

            // ComboBox cbxPermaTPType
            CbxPermaTPType.Parent = PnlProtection;
            CbxPermaTPType.Name = "cbxPermaTPType";
            CbxPermaTPType.Visible = false;
            CbxPermaTPType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaTPType.Items.Add(Language.T("Relative"));
            CbxPermaTPType.Items.Add(Language.T("Absolute"));
            CbxPermaTPType.SelectedIndex = 0;

            // NumericUpDown Permanent Take Profit
            FancyNudPermaTP.Parent = PnlProtection;
            FancyNudPermaTP.Name = "nudPermaTP";
            FancyNudPermaTP.BeginInit();
            FancyNudPermaTP.Minimum = 5;
            FancyNudPermaTP.Maximum = 5000;
            FancyNudPermaTP.Increment = 1;
            FancyNudPermaTP.Value = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            FancyNudPermaTP.TextAlign = HorizontalAlignment.Center;
            FancyNudPermaTP.EndInit();

            // CheckBox Break Even
            ChbBreakEven.Name = "chbBreakEven";
            ChbBreakEven.Parent = PnlProtection;
            ChbBreakEven.ForeColor = ColorText;
            ChbBreakEven.BackColor = Color.Transparent;
            ChbBreakEven.AutoCheck = true;
            ChbBreakEven.AutoSize = true;
            ChbBreakEven.Checked = false;
            ChbBreakEven.Text = Language.T("Break Even");

            // NumericUpDown Break Even
            FancyNudBreakEven.Parent = PnlProtection;
            FancyNudBreakEven.Name = "nudBreakEven";
            FancyNudBreakEven.BeginInit();
            FancyNudBreakEven.Minimum = 5;
            FancyNudBreakEven.Maximum = 5000;
            FancyNudBreakEven.Increment = 1;
            FancyNudBreakEven.Value = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            FancyNudBreakEven.TextAlign = HorizontalAlignment.Center;
            FancyNudBreakEven.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "btnDefault";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Name = "btnCancel";
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "btnAccept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnDefault { get; set; }
        private ComboBox CbxOppDirAction { get; set; }
        private ComboBox CbxPermaSLType { get; set; }
        private ComboBox CbxPermaTPType { get; set; }
        private ComboBox CbxSameDirAction { get; set; }
        private CheckBox CbxUseMartingale { get; set; }
        private CheckBox ChbBreakEven { get; set; }
        private CheckBox ChbPermaSL { get; set; }
        private CheckBox ChbPermaTP { get; set; }
        private Color ColorText { get; set; }
        private Label LblAddingLots { get; set; }
        private Label LblEntryLots { get; set; }
        private Label LblMaxOpenLots { get; set; }
        private Label LblOppDirAction { get; set; }
        private Label LblPercent1 { get; set; }
        private Label LblPercent2 { get; set; }
        private Label LblPercent3 { get; set; }
        private Label LblReducingLots { get; set; }
        private Label LblSameDirAction { get; set; }
        private FancyNud FancyNudAddingLots { get; set; }
        private FancyNud FancyNudBreakEven { get; set; }
        private FancyNud FancyNudEntryLots { get; set; }
        private FancyNud FancyNudMartingaleMultiplier { get; set; }
        private FancyNud FancyNudMaxOpenLots { get; set; }
        private FancyNud FancyNudPermaSL { get; set; }
        private FancyNud FancyNudPermaTP { get; set; }
        private FancyNud FancyNudReducingLots { get; set; }
        private FancyPanel PnlAmounts { get; set; }
        private FancyPanel PnlAveraging { get; set; }
        private FancyPanel PnlProtection { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private RadioButton RbConstantUnits { get; set; }
        private RadioButton RbVariableUnits { get; set; }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetFormSize();
            SetParams();
            BtnAccept.Focus();
        }

        private void SetFormSize()
        {
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            const int leftComboBxWith = 80;
            const int rightComboBxWith = 95;
            const int nudWidth = 60;
            const int lblPercentWidth = 15;
            const int border = 2;

            leftPanelsWidth = 3*buttonWidth + 2*btnHrzSpace;
            rightPanelsWidth = 3*buttonWidth + 2*btnHrzSpace;

            if (leftPanelsWidth < space + LblSameDirAction.Width + space + leftComboBxWith + space)
                leftPanelsWidth = space + LblSameDirAction.Width + space + leftComboBxWith + space;

            if (leftPanelsWidth < space + LblOppDirAction.Width + space + leftComboBxWith + space)
                leftPanelsWidth = space + LblOppDirAction.Width + space + leftComboBxWith + space;

            if (leftPanelsWidth < space + LblMaxOpenLots.Width + space + nudWidth + space)
                leftPanelsWidth = space + LblMaxOpenLots.Width + space + nudWidth + space;

            RbVariableUnits.Width = leftPanelsWidth - 2*space;
            Graphics g = CreateGraphics();
            while (g.MeasureString(RbVariableUnits.Text, RbVariableUnits.Font, RbVariableUnits.Width - 10).Height >
                   3*RbVariableUnits.Font.Height)
                RbVariableUnits.Width++;
            g.Dispose();
            if (leftPanelsWidth < space + RbVariableUnits.Width + space)
                leftPanelsWidth = space + RbVariableUnits.Width + space;

            if (leftPanelsWidth < space + LblEntryLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + LblEntryLots.Width + space + lblPercentWidth + nudWidth + space;

            if (leftPanelsWidth < space + LblAddingLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + LblAddingLots.Width + space + lblPercentWidth + nudWidth + space;

            if (leftPanelsWidth < space + LblReducingLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + LblReducingLots.Width + space + lblPercentWidth + nudWidth + space;

            int maxRightCheckBoxWidth = Math.Max(ChbPermaSL.Width, ChbPermaTP.Width);
            int requiredRightPanelWidth = border + space + maxRightCheckBoxWidth + space + rightComboBxWith + space +
                                          nudWidth + space + border;
            if (rightPanelsWidth < requiredRightPanelWidth)
                rightPanelsWidth = requiredRightPanelWidth;

            ClientSize = new Size(space + leftPanelsWidth + space + rightPanelsWidth + space, 390);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int buttonWidth = ((rightPanelsWidth - 2*btnHrzSpace)/3);
            int space = btnHrzSpace;
            const int border = 2;
            const int leftComboBxWith = 80;
            const int rightComboBxWith = 95;
            const int nudWidth = 60;
            const int lblPercentWidth = 15;

            // pnlAveraging
            PnlAveraging.Size = new Size(leftPanelsWidth, 84);
            PnlAveraging.Location = new Point(space, space);

            // pnlAmounts
            PnlAmounts.Size = new Size(leftPanelsWidth, 252);
            PnlAmounts.Location = new Point(space, PnlAveraging.Bottom + space);

            // pnlProtection
            PnlProtection.Size = new Size(rightPanelsWidth, 110);
            PnlProtection.Location = new Point(PnlAveraging.Right + space, PnlAveraging.Top);

            // Averaging
            int comboBxLeft = PnlAveraging.ClientSize.Width - leftComboBxWith - space - border;

            CbxSameDirAction.Width = leftComboBxWith;
            LblSameDirAction.Location = new Point(space, space + 25);
            CbxSameDirAction.Location = new Point(comboBxLeft, space + 21);

            CbxOppDirAction.Width = leftComboBxWith;
            LblOppDirAction.Location = new Point(space, buttonHeight + 2*space + 23);
            CbxOppDirAction.Location = new Point(comboBxLeft, buttonHeight + 2*space + 19);

            // Amounts
            int nudLeft = leftPanelsWidth - nudWidth - space - border;

            LblMaxOpenLots.Location = new Point(space, 0*buttonHeight + space + 25);
            FancyNudMaxOpenLots.Size = new Size(nudWidth, buttonHeight);
            FancyNudMaxOpenLots.Location = new Point(nudLeft, 0*buttonHeight + space + 22);

            RbConstantUnits.Location = new Point(space + 3, 55);
            RbVariableUnits.Location = new Point(space + 3, 79);
            RbVariableUnits.Size = new Size(leftPanelsWidth - 2*space, 2*buttonHeight);

            LblEntryLots.Location = new Point(btnHrzSpace, 139);
            FancyNudEntryLots.Size = new Size(nudWidth, buttonHeight);
            FancyNudEntryLots.Location = new Point(nudLeft, 137);
            LblPercent1.Width = lblPercentWidth;
            LblPercent1.Location = new Point(FancyNudEntryLots.Left - lblPercentWidth, LblEntryLots.Top);

            LblAddingLots.Location = new Point(btnHrzSpace, 167);
            FancyNudAddingLots.Size = new Size(nudWidth, buttonHeight);
            FancyNudAddingLots.Location = new Point(nudLeft, 165);
            LblPercent2.Width = lblPercentWidth;
            LblPercent2.Location = new Point(FancyNudAddingLots.Left - lblPercentWidth, LblAddingLots.Top);

            LblReducingLots.Location = new Point(btnHrzSpace, 195);
            FancyNudReducingLots.Size = new Size(nudWidth, buttonHeight);
            FancyNudReducingLots.Location = new Point(nudLeft, 193);
            LblPercent3.Width = lblPercentWidth;
            LblPercent3.Location = new Point(FancyNudReducingLots.Left - lblPercentWidth, LblReducingLots.Top);

            CbxUseMartingale.Location = new Point(btnHrzSpace + 2, 223);
            FancyNudMartingaleMultiplier.Size = new Size(nudWidth, buttonHeight);
            FancyNudMartingaleMultiplier.Location = new Point(nudLeft, 221);

            nudLeft = rightPanelsWidth - nudWidth - btnHrzSpace - border;
            comboBxLeft = nudLeft - space - rightComboBxWith;

            // Permanent Stop Loss
            ChbPermaSL.Location = new Point(border + space, 0*buttonHeight + 1*space + 24);
            CbxPermaSLType.Width = rightComboBxWith;
            CbxPermaSLType.Location = new Point(comboBxLeft, 0*buttonHeight + 1*space + 21);
            FancyNudPermaSL.Size = new Size(nudWidth, buttonHeight);
            FancyNudPermaSL.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 22);

            // Permanent Take Profit
            ChbPermaTP.Location = new Point(border + space, 1*buttonHeight + 2*space + 22);
            FancyNudPermaTP.Size = new Size(nudWidth, buttonHeight);
            CbxPermaTPType.Width = rightComboBxWith;
            CbxPermaTPType.Location = new Point(comboBxLeft, 1*buttonHeight + 2*space + 19);
            FancyNudPermaTP.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 20);

            // Break Even
            ChbBreakEven.Location = new Point(border + space, 2*buttonHeight + 3*space + 20);
            FancyNudBreakEven.Size = new Size(nudWidth, buttonHeight);
            FancyNudBreakEven.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 18);

            BalanceChart.Size = new Size(rightPanelsWidth, PnlAmounts.Bottom - PnlProtection.Bottom - space);
            BalanceChart.Location = new Point(PnlAveraging.Right + space, PnlProtection.Bottom + space);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            int buttonTop = ClientSize.Height - buttonHeight - btnVertSpace;

            BtnAccept.Location = new Point(PnlProtection.Left, buttonTop);
            BtnDefault.Location = new Point((PnlProtection.Left + PnlProtection.Right - buttonWidth)/2, buttonTop);
            BtnCancel.Location = new Point(PnlProtection.Right - buttonWidth, buttonTop);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        private void BtnDefaultClick(object sender, EventArgs e)
        {
            Data.Strategy.SameSignalAction = SameDirSignalAction.Nothing;

            Data.Strategy.OppSignalAction = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName ==
                                            "Close and Reverse"
                                                ? OppositeDirSignalAction.Reverse
                                                : OppositeDirSignalAction.Nothing;

            Data.Strategy.UsePermanentSL = false;
            Data.Strategy.PermanentSL = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            Data.Strategy.PermanentSLType = PermanentProtectionType.Relative;
            Data.Strategy.UsePermanentTP = false;
            Data.Strategy.PermanentTP = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            Data.Strategy.PermanentTPType = PermanentProtectionType.Relative;
            Data.Strategy.UseBreakEven = false;
            Data.Strategy.BreakEven = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            Data.Strategy.UseAccountPercentEntry = false;
            Data.Strategy.MaxOpenLots = 20;
            Data.Strategy.EntryLots = 1;
            Data.Strategy.AddingLots = 1;
            Data.Strategy.ReducingLots = 1;
            Data.Strategy.UseMartingale = false;
            Data.Strategy.MartingaleMultiplier = 2;

            SetParams();
            CalculateStrategy();
            UpdateChart();
        }

        private void ParamChanged(object sender, EventArgs e)
        {
            FancyNudPermaSL.Enabled = ChbPermaSL.Checked;
            FancyNudPermaTP.Enabled = ChbPermaTP.Checked;
            FancyNudBreakEven.Enabled = ChbBreakEven.Checked;
            CbxPermaSLType.Enabled = ChbPermaSL.Checked;
            CbxPermaTPType.Enabled = ChbPermaTP.Checked;
            FancyNudMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;

            if (!RbVariableUnits.Checked)
                FancyNudEntryLots.Value = Math.Min(FancyNudEntryLots.Value, FancyNudMaxOpenLots.Value);

            Data.Strategy.SameSignalAction = (SameDirSignalAction) CbxSameDirAction.SelectedIndex;
            Data.Strategy.OppSignalAction = (OppositeDirSignalAction) CbxOppDirAction.SelectedIndex;
            Data.Strategy.UseAccountPercentEntry = RbVariableUnits.Checked;
            Data.Strategy.MaxOpenLots = (double) FancyNudMaxOpenLots.Value;
            Data.Strategy.EntryLots = (double) FancyNudEntryLots.Value;
            Data.Strategy.AddingLots = (double) FancyNudAddingLots.Value;
            Data.Strategy.ReducingLots = (double) FancyNudReducingLots.Value;
            Data.Strategy.UsePermanentSL = ChbPermaSL.Checked;
            Data.Strategy.UsePermanentTP = ChbPermaTP.Checked;
            Data.Strategy.UseBreakEven = ChbBreakEven.Checked;
            Data.Strategy.PermanentSLType = (PermanentProtectionType) CbxPermaSLType.SelectedIndex;
            Data.Strategy.PermanentTPType = (PermanentProtectionType) CbxPermaTPType.SelectedIndex;
            Data.Strategy.PermanentSL = (int) FancyNudPermaSL.Value;
            Data.Strategy.PermanentTP = (int) FancyNudPermaTP.Value;
            Data.Strategy.BreakEven = (int) FancyNudBreakEven.Value;
            Data.Strategy.UseMartingale = CbxUseMartingale.Checked;
            Data.Strategy.MartingaleMultiplier = (double) FancyNudMartingaleMultiplier.Value;

            SetLabelPercent();
            CalculateStrategy();
            UpdateChart();
        }

        private void SetParams()
        {
            RemoveParamEventHandlers();

            CbxSameDirAction.SelectedIndex = (int) Data.Strategy.SameSignalAction;
            CbxOppDirAction.SelectedIndex = (int) Data.Strategy.OppSignalAction;
            CbxOppDirAction.Enabled = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName != "Close and Reverse";

            RbConstantUnits.Checked = !Data.Strategy.UseAccountPercentEntry;
            RbVariableUnits.Checked = Data.Strategy.UseAccountPercentEntry;

            FancyNudMaxOpenLots.Value = (decimal) Data.Strategy.MaxOpenLots;

            if (!RbVariableUnits.Checked)
                FancyNudEntryLots.Value = (decimal) Math.Min(Data.Strategy.EntryLots, Data.Strategy.MaxOpenLots);
            else
                FancyNudEntryLots.Value = (decimal) Data.Strategy.EntryLots;

            FancyNudAddingLots.Value = (decimal) Data.Strategy.AddingLots;
            FancyNudReducingLots.Value = (decimal) Data.Strategy.ReducingLots;

            CbxUseMartingale.Checked = Data.Strategy.UseMartingale;
            FancyNudMartingaleMultiplier.Value = (decimal) Data.Strategy.MartingaleMultiplier;
            FancyNudMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;

            ChbPermaSL.Checked = Data.Strategy.UsePermanentSL;
            FancyNudPermaSL.Value = Data.Strategy.PermanentSL;
            FancyNudPermaSL.Enabled = Data.Strategy.UsePermanentSL;
            CbxPermaSLType.Enabled = Data.Strategy.UsePermanentSL;
            CbxPermaSLType.SelectedIndex = (int) Data.Strategy.PermanentSLType;

            ChbPermaTP.Checked = Data.Strategy.UsePermanentTP;
            FancyNudPermaTP.Value = Data.Strategy.PermanentTP;
            FancyNudPermaTP.Enabled = Data.Strategy.UsePermanentTP;
            CbxPermaTPType.Enabled = Data.Strategy.UsePermanentTP;
            CbxPermaTPType.SelectedIndex = (int) Data.Strategy.PermanentTPType;

            ChbBreakEven.Checked = Data.Strategy.UseBreakEven;
            FancyNudBreakEven.Value = Data.Strategy.BreakEven;
            FancyNudBreakEven.Enabled = Data.Strategy.UseBreakEven;

            SetParamEventHandlers();
            SetLabelPercent();
        }

        private void SetParamEventHandlers()
        {
            CbxSameDirAction.SelectedIndexChanged += ParamChanged;
            CbxOppDirAction.SelectedIndexChanged += ParamChanged;
            RbConstantUnits.CheckedChanged += ParamChanged;
            RbVariableUnits.CheckedChanged += ParamChanged;
            FancyNudMaxOpenLots.ValueChanged += ParamChanged;
            FancyNudEntryLots.ValueChanged += ParamChanged;
            FancyNudAddingLots.ValueChanged += ParamChanged;
            FancyNudReducingLots.ValueChanged += ParamChanged;
            ChbPermaSL.CheckedChanged += ParamChanged;
            CbxPermaSLType.SelectedIndexChanged += ParamChanged;
            FancyNudPermaSL.ValueChanged += ParamChanged;
            ChbPermaTP.CheckedChanged += ParamChanged;
            CbxPermaTPType.SelectedIndexChanged += ParamChanged;
            FancyNudPermaTP.ValueChanged += ParamChanged;
            FancyNudBreakEven.ValueChanged += ParamChanged;
            ChbBreakEven.CheckedChanged += ParamChanged;
            CbxUseMartingale.CheckedChanged += ParamChanged;
            FancyNudMartingaleMultiplier.ValueChanged += ParamChanged;
        }

        private void RemoveParamEventHandlers()
        {
            CbxSameDirAction.SelectedIndexChanged -= ParamChanged;
            CbxOppDirAction.SelectedIndexChanged -= ParamChanged;
            RbConstantUnits.CheckedChanged -= ParamChanged;
            RbVariableUnits.CheckedChanged -= ParamChanged;
            FancyNudMaxOpenLots.ValueChanged -= ParamChanged;
            FancyNudEntryLots.ValueChanged -= ParamChanged;
            FancyNudAddingLots.ValueChanged -= ParamChanged;
            FancyNudReducingLots.ValueChanged -= ParamChanged;
            ChbPermaSL.CheckedChanged -= ParamChanged;
            CbxPermaSLType.SelectedIndexChanged -= ParamChanged;
            FancyNudPermaSL.ValueChanged -= ParamChanged;
            ChbPermaTP.CheckedChanged -= ParamChanged;
            CbxPermaTPType.SelectedIndexChanged -= ParamChanged;
            FancyNudPermaTP.ValueChanged -= ParamChanged;
            FancyNudBreakEven.ValueChanged -= ParamChanged;
            ChbBreakEven.CheckedChanged -= ParamChanged;
            CbxUseMartingale.CheckedChanged -= ParamChanged;
            FancyNudMartingaleMultiplier.ValueChanged -= ParamChanged;
        }

        private void SetLabelPercent()
        {
            string text = Data.Strategy.UseAccountPercentEntry ? "%" : "";
            LblPercent1.Text = text;
            LblPercent2.Text = text;
            LblPercent3.Text = text;
        }

        private void CalculateStrategy()
        {
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
        }

        private void UpdateChart()
        {
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }
    }
}