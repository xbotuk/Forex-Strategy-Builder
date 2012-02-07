// Strategy Optimizer
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer
    /// </summary>
    public partial class Optimizer : Form
    {
        ToolStrip tsOptimizerButtons;
        ToolStripButton[] aOptimizerButtons;

        Panel               pnlParamsBase;
        Panel               pnlCaptions;
        Panel               pnlParamsBase2;
        Panel               pnlParams;
        Fancy_Panel         pnlLimitations;
        Fancy_Panel         pnlSettings;

        CheckBox[]          achbxParameterName;
        Label []            alblParameterValue;
        NumericUpDown []    anudParameterMin;
        NumericUpDown []    anudParameterMax;
        NumericUpDown []    anudParameterStep;
        Small_Balance_Chart smallBalanceChart;
        Label []            alblIndicatorName;
        Label               lblNoParams;
        ProgressBar         progressBar;
        Button              btnOptimize;
        Button              btnAccept;
        Button              btnCancel;
        Parameter[]         aParameter;
        ToolTip             toolTip = new ToolTip();
        Random              random  = new Random();
        VScrollBar          scrollBar;
        BackgroundWorker    bgWorker;
        bool                isStartegyChanged = false;

        CheckBox      chbAmbiguousBars;
        NumericUpDown nudAmbiguousBars;
        CheckBox      chbMaxDrawdown;
        NumericUpDown nudMaxDrawdown;
        CheckBox      chbMinTrades;
        NumericUpDown nudMinTrades;
        CheckBox      chbMaxTrades;
        NumericUpDown nudMaxTrades;
        CheckBox      chbWinLossRatio;
        NumericUpDown nudWinLossRatio;
        CheckBox      chbEquityPercent;
        NumericUpDown nudEquityPercent;
        CheckBox      chbOOSPatternFilter;
        NumericUpDown nudOOSPatternPercent;
        CheckBox      chbSmoothBalanceLines;
        NumericUpDown nudSmoothBalancePercent;
        NumericUpDown nudSmoothBalanceCheckPoints;
        CheckBox      chbOptimizerWritesReport;

        CheckBox chbHideFSB;
        Button btnReset;

        int   parameters;   // Count of the NumericParameters
        int   protections;  // Count of permanent protections
        bool  isOptimizing = false; // It is true when the optimizer is running

        int   checkedParams;   // Count of the checked parameters
        int[] aiChecked;       // An array of the checked parameters
        int   cycles;          // Count of the cycles
        int   computedCycles;  // Currently completed cycles
        int   progressPercent; // Reached progress in %

        // Out of Sample
        int  barOOS = Data.Bars - 1;
        bool isOOS  = false;
        CheckBox      chbOutOfSample;
        NumericUpDown nudOutOfSample;

        Font fontIndicator;
        Font fontParamValueBold;
        Font fontParamValueRegular;
        Color colorText;

        int border = 2;
        StringBuilder sbReport;

        Form formFSB;
        public Form SetParrentForm { set { formFSB = value; } }

        bool isReset = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Optimizer()
        {
            Icon         = Data.Icon;
            BackColor    = LayoutColors.ColorFormBack;
            AcceptButton = btnAccept;
            CancelButton = btnCancel;
            Text         = Language.T("Optimizer");
            FormClosing += new FormClosingEventHandler(Optimizer_FormClosing);

            InitializeControls();
            SetupButtons();
            SetPanelLimitations();
            SetPanelSettings();
            LoadOptions();

            chbHideFSB.CheckedChanged += new EventHandler(HideFSB_Click);

            return;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            formFSB.Visible = !chbHideFSB.Checked;

            SetIndicatorParams();
            SelectRandomParameters();

            Width  = 480;
            Height = 570;
            MinimumSize = new Size(480, 570);

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int buttonWidth  = (ClientSize.Width - 4 * btnHrzSpace) / 3;
            int space        = btnHrzSpace;

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Optimize
            btnOptimize.Size     = new Size(buttonWidth, buttonHeight);
            btnOptimize.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size     = new Size(ClientSize.Width - 2 * space, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(space, btnCancel.Top - progressBar.Height - btnVertSpace);

            // Panel Preview
            smallBalanceChart.Size     = new Size(ClientSize.Width - 2 * space, 200);
            smallBalanceChart.Location = new Point(space, progressBar.Top - space - smallBalanceChart.Height);

            // Panel Params Base
            pnlParamsBase.Size     = new Size(ClientSize.Width - 2 * space, smallBalanceChart.Top - 2 * space - tsOptimizerButtons.Bottom);
            pnlParamsBase.Location = new Point(space, tsOptimizerButtons.Bottom + space);

            // Panel Params Base 2
            pnlParamsBase2.Size = new Size(pnlParamsBase.Width - 2 * border, pnlParamsBase.Height - pnlCaptions.Height - border);
            pnlParamsBase2.Location = new Point(border, pnlCaptions.Height);

            // Panel Params
            pnlParams.Width = pnlParamsBase2.ClientSize.Width - scrollBar.Width;

            // No params
            lblNoParams.Location = new Point(5, 5);

            // Panel Limitations
            pnlLimitations.Size     = pnlParamsBase.Size;
            pnlLimitations.Location = pnlParamsBase.Location;

            int nudWidth = 55;

            // chbAmbiguousBars
            chbAmbiguousBars.Location = new Point(border + 5, 27);

            // nudAmbiguousBars
            nudAmbiguousBars.Width    = nudWidth;
            nudAmbiguousBars.Location = new Point(pnlLimitations.ClientSize.Width - nudWidth - border - 5, chbAmbiguousBars.Top - 1);

            // MaxDrawdown
            chbMaxDrawdown.Location = new Point(border + 5, chbAmbiguousBars.Bottom + border + 4);
            nudMaxDrawdown.Width    = nudWidth;
            nudMaxDrawdown.Location = new Point(nudAmbiguousBars.Left , chbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            chbEquityPercent.Location = new Point(border + 5, nudMaxDrawdown.Bottom + border + 4);
            nudEquityPercent.Width    = nudWidth;
            nudEquityPercent.Location = new Point(nudAmbiguousBars.Left, chbEquityPercent.Top - 1);

            // MinTrades
            chbMinTrades.Location = new Point(border + 5, chbEquityPercent.Bottom + border + 4);
            nudMinTrades.Width    = nudWidth;
            nudMinTrades.Location = new Point(nudAmbiguousBars.Left, chbMinTrades.Top - 1);

            // MaxTrades
            chbMaxTrades.Location = new Point(border + 5, chbMinTrades.Bottom + border + 4);
            nudMaxTrades.Width    = nudWidth;
            nudMaxTrades.Location = new Point(nudAmbiguousBars.Left, chbMaxTrades.Top - 1);

            // WinLossRatios
            chbWinLossRatio.Location = new Point(border + 5, chbMaxTrades.Bottom + border + 4);
            nudWinLossRatio.Width    = nudWidth;
            nudWinLossRatio.Location = new Point(nudAmbiguousBars.Left, chbWinLossRatio.Top - 1);

            // OOS Pattern Filter
            chbOOSPatternFilter.Location = new Point(border + 5, chbWinLossRatio.Bottom + border + 4);
            nudOOSPatternPercent.Width    = nudWidth;
            nudOOSPatternPercent.Location = new Point(nudAmbiguousBars.Left, chbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            chbSmoothBalanceLines.Location       = new Point(border + 5, chbOOSPatternFilter.Bottom + border + 4);
            nudSmoothBalancePercent.Width        = nudWidth;
            nudSmoothBalancePercent.Location     = new Point(nudAmbiguousBars.Left, chbSmoothBalanceLines.Top - 1);
            nudSmoothBalanceCheckPoints.Width    = nudWidth;
            nudSmoothBalanceCheckPoints.Location = new Point(nudSmoothBalancePercent.Left - nudWidth - border, chbSmoothBalanceLines.Top - 1);

            // Panel Settings
            pnlSettings.Size     = pnlParamsBase.Size;
            pnlSettings.Location = pnlParamsBase.Location;

            // Out Of Sample
            chbOutOfSample.Location = new Point(border + 5, 27);
            nudOutOfSample.Width    = nudWidth;
            nudOutOfSample.Location = new Point(pnlSettings.ClientSize.Width - nudWidth - border - 5, chbOutOfSample.Top - 1);

            // chbOptimizerWritesReport
            chbOptimizerWritesReport.Location = new Point(border + 5, chbOutOfSample.Bottom + border + 4);

            // Hide FSB when generator starts
            chbHideFSB.Location = new Point(border + 5, chbOptimizerWritesReport.Bottom + border + 4);

            // Button Reset
            btnReset.Width = pnlSettings.ClientSize.Width - 2 * (border + 5);
            btnReset.Location = new Point(border + 5, pnlSettings.Height - btnReset.Height - border - 2);

            // pnlCaptions
            pnlCaptions.Height = 20;
            pnlCaptions.Invalidate();

            return;
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        void Optimizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReset)
                SaveOptions();

            if (isOptimizing)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.Cancel && isStartegyChanged)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept changes to the strategy?"),
                    Language.T("Optimizer"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dr == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                }
                else if (dr == DialogResult.No)
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            else if (DialogResult == DialogResult.OK && !isStartegyChanged)
            {
                DialogResult = DialogResult.Cancel;
            }

            formFSB.Visible = true;
        }
   }
}
