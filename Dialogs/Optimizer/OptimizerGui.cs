// Strategy Optimizer - GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer - GUI
    /// </summary>
    public partial class Optimizer : Form
    {
        /// <summary>
        /// Initializes main controls of optimizer.
        /// </summary>
        void InitializeControls()
        {
            pnlParamsBase     = new Panel();
            pnlParamsBase2    = new Panel();
            pnlCaptions       = new Panel();
            pnlParams         = new Panel();
            pnlLimitations    = new FancyPanel(Language.T("Limitations"));
            pnlSettings       = new FancyPanel(Language.T("Settings"));
            scrollBar         = new VScrollBar();
            smallBalanceChart = new SmallBalanceChart();
            progressBar       = new ProgressBar();
            btnOptimize       = new Button();
            btnAccept         = new Button();
            btnCancel         = new Button();

            lblNoParams = new Label();

            fontIndicator = new Font(Font.FontFamily, 11);
            colorText     = LayoutColors.ColorControlText;

            // pnlParamsBase
            pnlParamsBase.Parent    = this;
            pnlParamsBase.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase.Paint    += new PaintEventHandler(PnlParamsBase_Paint);

            // pnlCaptions
            pnlCaptions.Parent    = pnlParamsBase;
            pnlCaptions.Dock      = DockStyle.Top;
            pnlCaptions.BackColor = LayoutColors.ColorCaptionBack;
            pnlCaptions.ForeColor = LayoutColors.ColorCaptionText;
            pnlCaptions.Paint    += new PaintEventHandler(PnlCaptions_Paint);

            // pnlParamsBase2
            pnlParamsBase2.Parent    = pnlParamsBase;
            pnlParamsBase2.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase2.Resize   += new EventHandler(PnlParamsBase2_Resize);

            // VScrollBar
            scrollBar.Parent        = pnlParamsBase2;
            scrollBar.Dock          = DockStyle.Right;
            scrollBar.TabStop       = true;
            scrollBar.ValueChanged += new EventHandler(ScrollBar_ValueChanged);
            scrollBar.MouseWheel   += new MouseEventHandler(ScrollBar_MouseWheel);

            // pnlParams
            pnlParams.Parent    = pnlParamsBase2;
            pnlParams.BackColor = LayoutColors.ColorControlBack;

            // lblNoParams
            lblNoParams.Parent   = pnlParams;
            lblNoParams.Text     = Language.T("There are no parameters suitable for optimization.");
            lblNoParams.AutoSize = true;
            lblNoParams.Visible  = false;

            // Panel Limitations
            pnlLimitations.Parent  = this;
            pnlLimitations.Visible = false;

            // Panel Settings
            pnlSettings.Parent  = this;
            pnlSettings.Visible = false;

            // smallBalanceChart
            smallBalanceChart.Parent    = this;
            smallBalanceChart.BackColor = LayoutColors.ColorControlBack;
            smallBalanceChart.SetChartData();

            // ProgressBar
            progressBar.Parent  = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step    = 1;

            // Button Optimize
            btnOptimize.Parent   = this;
            btnOptimize.Name     = "btnOptimize";
            btnOptimize.Text     = Language.T("Optimize");
            btnOptimize.TabIndex = 0;
            btnOptimize.Click   += new EventHandler(BtnOptimize_Click);
            btnOptimize.UseVisualStyleBackColor = true;

            // Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "btnAccept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.TabIndex     = 1;
            btnAccept.Enabled      = false;
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            // Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.TabIndex     = 2;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(BgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);
            
        }

        /// <summary>
        /// Sets controls in panel Limitations
        /// </summary>
        void SetPanelLimitations()
        {
            chbAmbiguousBars = new CheckBox();
            chbAmbiguousBars.Parent    = pnlLimitations;
            chbAmbiguousBars.ForeColor = colorText;
            chbAmbiguousBars.BackColor = Color.Transparent;
            chbAmbiguousBars.Text      = Language.T("Maximum number of ambiguous bars");
            chbAmbiguousBars.Checked   = false;
            chbAmbiguousBars.AutoSize  = true;

            nudAmbiguousBars = new NumericUpDown();
            nudAmbiguousBars.Parent    = pnlLimitations;
            nudAmbiguousBars.TextAlign = HorizontalAlignment.Center;
            nudAmbiguousBars.BeginInit();
            nudAmbiguousBars.Minimum   = 0;
            nudAmbiguousBars.Maximum   = 100;
            nudAmbiguousBars.Increment = 1;
            nudAmbiguousBars.Value     = 10;
            nudAmbiguousBars.EndInit();

            chbMaxDrawdown = new CheckBox();
            chbMaxDrawdown.Parent    = pnlLimitations;
            chbMaxDrawdown.ForeColor = colorText;
            chbMaxDrawdown.BackColor = Color.Transparent;
            chbMaxDrawdown.Checked   = false;
            chbMaxDrawdown.Text      = Language.T("Maximum equity drawdown") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            chbMaxDrawdown.AutoSize  = true;

            nudMaxDrawdown = new NumericUpDown();
            nudMaxDrawdown.Parent    = pnlLimitations;
            nudMaxDrawdown.TextAlign = HorizontalAlignment.Center;
            nudMaxDrawdown.BeginInit();
            nudMaxDrawdown.Minimum   = 0;
            nudMaxDrawdown.Maximum   = Configs.InitialAccount;
            nudMaxDrawdown.Increment = 10;
            nudMaxDrawdown.Value     = Configs.InitialAccount / 4;
            nudMaxDrawdown.EndInit();

            chbEquityPercent = new CheckBox();
            chbEquityPercent.Parent    = pnlLimitations;
            chbEquityPercent.ForeColor = colorText;
            chbEquityPercent.BackColor = Color.Transparent;
            chbEquityPercent.Text      = Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency + "]";
            chbEquityPercent.Checked   = false;
            chbEquityPercent.AutoSize  = true;

            nudEquityPercent = new NumericUpDown();
            nudEquityPercent.Parent    = pnlLimitations;
            nudEquityPercent.TextAlign = HorizontalAlignment.Center;
            nudEquityPercent.BeginInit();
            nudEquityPercent.Minimum   = 1;
            nudEquityPercent.Maximum   = 100;
            nudEquityPercent.Increment = 1;
            nudEquityPercent.Value     = 25;
            nudEquityPercent.EndInit();

            chbMinTrades = new CheckBox();
            chbMinTrades.Parent    = pnlLimitations;
            chbMinTrades.ForeColor = colorText;
            chbMinTrades.BackColor = Color.Transparent;
            chbMinTrades.Text      = Language.T("Minimum number of trades");
            chbMinTrades.Checked   = true;
            chbMinTrades.AutoSize  = true;

            nudMinTrades = new NumericUpDown();
            nudMinTrades.Parent    = pnlLimitations;
            nudMinTrades.TextAlign = HorizontalAlignment.Center;
            nudMinTrades.BeginInit();
            nudMinTrades.Minimum   = 10;
            nudMinTrades.Maximum   = 1000;
            nudMinTrades.Increment = 10;
            nudMinTrades.Value     = 100;
            nudMinTrades.EndInit();

            chbMaxTrades = new CheckBox();
            chbMaxTrades.Parent    = pnlLimitations;
            chbMaxTrades.ForeColor = colorText;
            chbMaxTrades.BackColor = Color.Transparent;
            chbMaxTrades.Text      = Language.T("Maximum number of trades");
            chbMaxTrades.Checked   = false;
            chbMaxTrades.AutoSize  = true;

            nudMaxTrades = new NumericUpDown();
            nudMaxTrades.Parent    = pnlLimitations;
            nudMaxTrades.TextAlign = HorizontalAlignment.Center;
            nudMaxTrades.BeginInit();
            nudMaxTrades.Minimum   = 10;
            nudMaxTrades.Maximum   = 10000;
            nudMaxTrades.Increment = 10;
            nudMaxTrades.Value     = 1000;
            nudMaxTrades.EndInit();

            chbWinLossRatio = new CheckBox();
            chbWinLossRatio.Parent    = pnlLimitations;
            chbWinLossRatio.ForeColor = colorText;
            chbWinLossRatio.BackColor = Color.Transparent;
            chbWinLossRatio.Text      = Language.T("Minimum win / loss trades ratio");
            chbWinLossRatio.Checked   = false;
            chbWinLossRatio.AutoSize  = true;

            nudWinLossRatio = new NumericUpDown();
            nudWinLossRatio.Parent    = pnlLimitations;
            nudWinLossRatio.TextAlign = HorizontalAlignment.Center;
            nudWinLossRatio.BeginInit();
            nudWinLossRatio.Minimum       = 0.10M;
            nudWinLossRatio.Maximum       = 1;
            nudWinLossRatio.Increment     = 0.01M;
            nudWinLossRatio.Value         = 0.30M;
            nudWinLossRatio.DecimalPlaces = 2;
            nudWinLossRatio.EndInit();

            chbOOSPatternFilter = new CheckBox();
            chbOOSPatternFilter.Parent    = pnlLimitations;
            chbOOSPatternFilter.ForeColor = colorText;
            chbOOSPatternFilter.BackColor = Color.Transparent;
            chbOOSPatternFilter.Text      = Language.T("Filter bad OOS performance");
            chbOOSPatternFilter.Checked   = false;
            chbOOSPatternFilter.AutoSize  = true;

            nudOOSPatternPercent = new NumericUpDown();
            nudOOSPatternPercent.Parent    = pnlLimitations;
            nudOOSPatternPercent.TextAlign = HorizontalAlignment.Center;
            nudOOSPatternPercent.BeginInit();
            nudOOSPatternPercent.Minimum = 1;
            nudOOSPatternPercent.Maximum = 50;
            nudOOSPatternPercent.Value   = 20;
            nudOOSPatternPercent.EndInit();
            toolTip.SetToolTip(nudOOSPatternPercent, Language.T("Deviation percent."));

            chbSmoothBalanceLines = new CheckBox();
            chbSmoothBalanceLines.Parent    = pnlLimitations;
            chbSmoothBalanceLines.ForeColor = colorText;
            chbSmoothBalanceLines.BackColor = Color.Transparent;
            chbSmoothBalanceLines.Text      = Language.T("Filter non-linear balance pattern");
            chbSmoothBalanceLines.Checked   = false;
            chbSmoothBalanceLines.AutoSize  = true;

            nudSmoothBalancePercent = new NumericUpDown();
            nudSmoothBalancePercent.Parent    = pnlLimitations;
            nudSmoothBalancePercent.TextAlign = HorizontalAlignment.Center;
            nudSmoothBalancePercent.BeginInit();
            nudSmoothBalancePercent.Minimum = 1;
            nudSmoothBalancePercent.Maximum = 50;
            nudSmoothBalancePercent.Value   = 20;
            nudSmoothBalancePercent.EndInit();
            toolTip.SetToolTip(nudSmoothBalancePercent, Language.T("Deviation percent."));

            nudSmoothBalanceCheckPoints = new NumericUpDown();
            nudSmoothBalanceCheckPoints.Parent    = pnlLimitations;
            nudSmoothBalanceCheckPoints.TextAlign = HorizontalAlignment.Center;
            nudSmoothBalanceCheckPoints.BeginInit();
            nudSmoothBalanceCheckPoints.Minimum = 1;
            nudSmoothBalanceCheckPoints.Maximum = 50;
            nudSmoothBalanceCheckPoints.Value   = 1;
            nudSmoothBalanceCheckPoints.EndInit();
            toolTip.SetToolTip(nudSmoothBalanceCheckPoints, Language.T("Check points count."));

            return;
        }

        /// <summary>
        /// Sets controls in panel Settings
        /// </summary>
        void SetPanelSettings()
        {
            chbOutOfSample = new CheckBox();
            chbOutOfSample.Parent    = pnlSettings;
            chbOutOfSample.ForeColor = colorText;
            chbOutOfSample.BackColor = Color.Transparent;
            chbOutOfSample.Text      = Language.T("Out of sample testing, percent of OOS bars");
            chbOutOfSample.Checked   = false;
            chbOutOfSample.AutoSize  = true;
            chbOutOfSample.CheckedChanged += new EventHandler(ChbOutOfSample_CheckedChanged);

            nudOutOfSample = new NumericUpDown();
            nudOutOfSample.Parent    = pnlSettings;
            nudOutOfSample.TextAlign = HorizontalAlignment.Center;
            nudOutOfSample.BeginInit();
            nudOutOfSample.Minimum   = 10;
            nudOutOfSample.Maximum   = 60;
            nudOutOfSample.Increment = 1;
            nudOutOfSample.Value     = 30;
            nudOutOfSample.EndInit();
            nudOutOfSample.ValueChanged += new EventHandler(NudOutOfSample_ValueChanged);

            chbOptimizerWritesReport = new CheckBox();
            chbOptimizerWritesReport.Parent    = pnlSettings;
            chbOptimizerWritesReport.ForeColor = colorText;
            chbOptimizerWritesReport.BackColor = Color.Transparent;
            chbOptimizerWritesReport.Text      = Language.T("Optimizer writes a report for each optimized strategy");
            chbOptimizerWritesReport.Checked   = false;
            chbOptimizerWritesReport.AutoSize  = true;

            chbHideFSB = new CheckBox();
            chbHideFSB.Parent    = pnlSettings;
            chbHideFSB.ForeColor = colorText;
            chbHideFSB.BackColor = Color.Transparent;
            chbHideFSB.Text      = Language.T("Hide FSB when Optimizer starts");
            chbHideFSB.Checked = true;
            chbHideFSB.AutoSize  = true;

            btnReset = new Button();
            btnReset.Parent = pnlSettings;
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Text = Language.T("Reset all parameters and settings");
            btnReset.Click += new EventHandler(BtnReset_Click);
        }

        /// <summary>
        /// Sets ups the chart's buttons.
        /// </summary>
        void SetupButtons()
        {
            tsOptimizerButtons = new ToolStrip();
            tsOptimizerButtons.Parent = this;

            aOptimizerButtons = new ToolStripButton[Enum.GetValues(typeof(OptimizerButtons)).Length];
            for (int i = 0; i < aOptimizerButtons.Length; i++)
            {
                aOptimizerButtons[i] = new ToolStripButton();
                aOptimizerButtons[i].Tag = (OptimizerButtons)i;
                aOptimizerButtons[i].Click += new EventHandler(Buttons_Click);
                tsOptimizerButtons.Items.Add(aOptimizerButtons[i]);
                if (i < 3)
                    aOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Image;
                else
                    aOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                if (i == 2 || i == 5)
                    tsOptimizerButtons.Items.Add(new ToolStripSeparator());
            }

            // Select All
            aOptimizerButtons[(int)OptimizerButtons.SelectAll].Image = Properties.Resources.optimizer_select_all;
            aOptimizerButtons[(int)OptimizerButtons.SelectAll].ToolTipText = Language.T("Select all parameters.");

            // Select None
            aOptimizerButtons[(int)OptimizerButtons.SelectNone].Image = Properties.Resources.optimizer_select_none;
            aOptimizerButtons[(int)OptimizerButtons.SelectNone].ToolTipText = Language.T("Select none of the parameters.");

            // Select Random
            aOptimizerButtons[(int)OptimizerButtons.SelectRandom].Image = Properties.Resources.optimizer_select_random;
            aOptimizerButtons[(int)OptimizerButtons.SelectRandom].ToolTipText = Language.T("Select a random number of parameters.");

            // Set step 5
            aOptimizerButtons[(int)OptimizerButtons.SetStep5].Text = "±5";
            aOptimizerButtons[(int)OptimizerButtons.SetStep5].ToolTipText = Language.T("Set Min / Max ± 5 steps.");

            // Set step 10
            aOptimizerButtons[(int)OptimizerButtons.SetStep10].Text = "±10";
            aOptimizerButtons[(int)OptimizerButtons.SetStep10].ToolTipText = Language.T("Set Min / Max ± 10 steps.");

            // Set step 15
            aOptimizerButtons[(int)OptimizerButtons.SetStep15].Text = "±15";
            aOptimizerButtons[(int)OptimizerButtons.SetStep15].ToolTipText = Language.T("Set Min / Max ± 15 steps.");

            // Show Parameters
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].Text = Language.T("Parameters");
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].ToolTipText = Language.T("Show indicator parameters.");
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = false;

            // Show Limitations
            aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Text =  Language.T("Limitations");
            aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].ToolTipText = Language.T("Show strategy limitations.");

            // Show Settings
            aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Text =  Language.T("Settings");
            aOptimizerButtons[(int)OptimizerButtons.ShowSettings].ToolTipText = Language.T("Show optimizer settings.");

            return;
        }

        /// <summary>
        /// Loads and parses the optimizer's options.
        /// </summary>
        void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.OptimizerOptions))
                return;

            string[] options = Configs.OptimizerOptions.Split(';');
            int i = 0;

            try {
                if (int.Parse(options[i++]) < OptionsVersion) return;
                chbOutOfSample.Checked            = bool.Parse(options[i++]);
                nudOutOfSample.Value              = int.Parse(options[i++]);
                chbAmbiguousBars.Checked          = bool.Parse(options[i++]);
                nudAmbiguousBars.Value            = int.Parse(options[i++]);
                chbMaxDrawdown.Checked            = bool.Parse(options[i++]);
                nudMaxDrawdown.Value              = int.Parse(options[i++]);
                chbMinTrades.Checked              = bool.Parse(options[i++]);
                nudMinTrades.Value                = int.Parse(options[i++]);
                chbMaxTrades.Checked              = bool.Parse(options[i++]);
                nudMaxTrades.Value                = int.Parse(options[i++]);
                chbWinLossRatio.Checked           = bool.Parse(options[i++]);
                nudWinLossRatio.Value             = int.Parse(options[i++]) / 100M;
                chbEquityPercent.Checked          = bool.Parse(options[i++]);
                nudEquityPercent.Value            = int.Parse(options[i++]);
                chbOOSPatternFilter.Checked       = bool.Parse(options[i++]);
                nudOOSPatternPercent.Value        = int.Parse(options[i++]);
                chbSmoothBalanceLines.Checked     = bool.Parse(options[i++]);
                nudSmoothBalancePercent.Value     = int.Parse(options[i++]);
                nudSmoothBalanceCheckPoints.Value = int.Parse(options[i++]);
                chbOptimizerWritesReport.Checked  = bool.Parse(options[i++]);
                chbHideFSB.Checked                = bool.Parse(options[i++]);
                formHeight                        = int.Parse(options[i++]);
            }
            catch
            {
            }

            return;
        }

        /// <summary>
        /// Saves the generator's options.
        /// </summary>
        void SaveOptions()
        {
            string options =
            OptionsVersion.ToString()                        + ";" +
            chbOutOfSample.Checked.ToString()                + ";" +
            nudOutOfSample.Value.ToString()                  + ";" +
            chbAmbiguousBars.Checked.ToString()              + ";" +
            nudAmbiguousBars.Value.ToString()                + ";" +
            chbMaxDrawdown.Checked.ToString()                + ";" +
            nudMaxDrawdown.Value.ToString()                  + ";" +
            chbMinTrades.Checked.ToString()                  + ";" +
            nudMinTrades.Value.ToString()                    + ";" +
            chbMaxTrades.Checked.ToString()                  + ";" +
            nudMaxTrades.Value.ToString()                    + ";" +
            chbWinLossRatio.Checked.ToString()               + ";" +
            ((int)(nudWinLossRatio.Value * 100M)).ToString() + ";" +
            chbEquityPercent.Checked.ToString()              + ";" +
            nudEquityPercent.Value.ToString()                + ";" +
            chbOOSPatternFilter.Checked.ToString()           + ";" +
            nudOOSPatternPercent.Value.ToString()            + ";" +
            chbSmoothBalanceLines.Checked.ToString()         + ";" +
            nudSmoothBalancePercent.Value.ToString()         + ";" +
            nudSmoothBalanceCheckPoints.Value.ToString()     + ";" +
            chbOptimizerWritesReport.Checked.ToString()      + ";" +
            chbHideFSB.Checked.ToString()                    + ";" +
            Height.ToString();

            Configs.OptimizerOptions = options;

            return;
        }

        /// <summary>
        /// Paints pnlCaptions
        /// </summary>
        void PnlCaptions_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;

            Data.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            Brush brush = new SolidBrush(pnl.ForeColor);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            g.DrawString(Language.T("Parameter"), Font, brush, 25,  3);
            g.DrawString(Language.T("Value"),     Font, brush, 190, 3, stringFormat);
            g.DrawString(Language.T("Minimum"),   Font, brush, 230, 3);
            g.DrawString(Language.T("Maximum"),   Font, brush, 303, 3);
            g.DrawString(Language.T("Step"),      Font, brush, 390, 3);

            return;
        }

        /// <summary>
        /// Paints PnlParamsBase
        /// </summary>
        void PnlParamsBase_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            g.DrawLine(penBorder, 1, 0, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 0, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            return;
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void NudOutOfSample_ValueChanged(object sender, EventArgs e)
        {
            isOOS = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;

            smallBalanceChart.OOSBar = barOOS;

            if (isOOS)
            {
                smallBalanceChart.SetChartData();
                smallBalanceChart.InitChart();
                smallBalanceChart.Invalidate();
            }
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void ChbOutOfSample_CheckedChanged(object sender, EventArgs e)
        {
            isOOS = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;

            smallBalanceChart.IsOOS    = isOOS;
            smallBalanceChart.OOSBar = barOOS;

            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        void Buttons_Click(object sender, EventArgs e)
        {

            ToolStripButton  btn    = (ToolStripButton)sender;
            OptimizerButtons button = (OptimizerButtons)btn.Tag;

            switch (button)
            {
                case OptimizerButtons.ShowParams:
                    pnlParamsBase.Visible  = true;
                    pnlLimitations.Visible = false;
                    pnlSettings.Visible    = false;
                    if (isOptimizing == false)
                        for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                            aOptimizerButtons[i].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowLimitations:
                    pnlParamsBase.Visible  = false;
                    pnlLimitations.Visible = true;
                    pnlSettings.Visible    = false;
                    for(int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = true;
                   break;
                case OptimizerButtons.ShowSettings:
                    pnlParamsBase.Visible  = false;
                    pnlLimitations.Visible = false;
                    pnlSettings.Visible    = true;
                    for(int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = false;
                    break;
                default:
                    break;
            }

            if (isOptimizing)
                return;

            switch (button)
            {
                case OptimizerButtons.SelectAll:
                    SelectParameters(OptimizerButtons.SelectAll);
                    break;
                case OptimizerButtons.SelectNone:
                    SelectParameters(OptimizerButtons.SelectNone);
                    break;
                case OptimizerButtons.SelectRandom:
                    SelectParameters(OptimizerButtons.SelectRandom);
                    break;
                case OptimizerButtons.SetStep5:
                    SetParamsMinMax(5);
                    break;
                case OptimizerButtons.SetStep10:
                    SetParamsMinMax(10);
                    break;
                case OptimizerButtons.SetStep15:
                    SetParamsMinMax(15);
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Check Box checked changed
        /// </summary>
        void Optimizer_CheckedChanged(object sender, EventArgs e)
        {
            btnOptimize.Focus();
        }

        /// <summary>
        /// Arranges the controls into the pnlParams
        /// </summary>
        void PnlParamsBase2_Resize(object sender, EventArgs e)
        {
            if (pnlParams.Height > pnlParamsBase2.Height)
            {
                scrollBar.Maximum     = pnlParams.Height - pnlParamsBase2.Height + 40;
                scrollBar.Value       = 0;
                scrollBar.SmallChange = 20;
                scrollBar.LargeChange = 40;
                scrollBar.Visible     = true;
            }
            else
            {
                scrollBar.Visible = false;
                scrollBar.Minimum = 0;
                scrollBar.Maximum = 0;
                scrollBar.Value   = 0;
            }

            pnlParams.Location = new Point(0, -scrollBar.Value);

            return;
        }

        /// <summary>
        /// Invalidate the Panel Parameters
        /// </summary>
        void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            pnlParams.Location = new Point(0, -scrollBar.Value);
        }

        /// <summary>
        /// Shift the pnlParams viewpoint
        /// </summary>
        void ScrollBar_MouseWheel(object sender, MouseEventArgs e)
        {
            if (scrollBar.Visible)
            {
                int newValue = scrollBar.Value - e.Delta / 120;

                if (newValue < scrollBar.Minimum)
                    scrollBar.Value = scrollBar.Minimum;
                else if (newValue > scrollBar.Maximum)
                    scrollBar.Value = scrollBar.Maximum;
                else
                    scrollBar.Value = newValue;
            }
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        void HideFSB_Click(object sender, EventArgs e)
        {
            formFSB.Visible = !chbHideFSB.Checked;
        }

        /// <summary>
        /// Resets Generator
        /// </summary>
        void BtnReset_Click(object sender, EventArgs e)
        {
            Configs.OptimizerOptions = "";
            isReset = true;
            Close();
        }
    }
}
