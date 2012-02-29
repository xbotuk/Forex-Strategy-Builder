// Controls Account
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Dialogs;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls Account: MenuAndStatusBar
    /// </summary>
    public partial class Controls
    {
        protected SmallBalanceChart BalanceChart { get; private set; }
        protected ToolStripComboBox ComboBoxInterpolationMethod { get; private set; }
        protected InfoPanel InfoPanelAccountStatistics { get; private set; }

        /// <summary>
        /// Initializes the controls in panel pnlOverview
        /// </summary>
        private void InitializeAccount()
        {
            var toolTip = new ToolTip();

            string[] methods = Enum.GetNames(typeof (InterpolationMethod));
            for (int i = 0; i < methods.Length; i++)
                methods[i] = Language.T(methods[i]);

            Graphics g = CreateGraphics();
            int maxWidth = 0;
            foreach (string method in methods)
                if ((int) g.MeasureString(method, Font).Width > maxWidth)
                    maxWidth = (int) g.MeasureString(method, Font).Width;
            g.Dispose();

            // ComboBox Interpolation Methods
            ComboBoxInterpolationMethod = new ToolStripComboBox
            {
                Name = "tscbInterpolationMethod",
                AutoSize = false,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = maxWidth + (int) (18*Data.HorizontalDLU),
                ToolTipText = Language.T("Bar interpolation method.")
            };
            foreach (string method in methods)
                ComboBoxInterpolationMethod.Items.Add(method);
            ComboBoxInterpolationMethod.SelectedIndex = 0;
            ComboBoxInterpolationMethod.SelectedIndexChanged += SelectedIndexChanged;
            ToolStripAccount.Items.Add(ComboBoxInterpolationMethod);

            // Button Comparator
            var tsbtComparator = new ToolStripButton {Text = Language.T("Comparator"), Name = "Comparator"};
            tsbtComparator.Click += BtnTools_OnClick;
            tsbtComparator.ToolTipText = Language.T("Compare the interpolating methods.");
            ToolStripAccount.Items.Add(tsbtComparator);

            // Button Scanner
            var tsbtScanner = new ToolStripButton {Text = Language.T("Scanner"), Name = "Scanner"};
            tsbtScanner.Click += BtnTools_OnClick;
            tsbtScanner.ToolTipText = Language.T("Perform a deep intrabar scan.") + Environment.NewLine +
                                      Language.T("Quick scan") + " - F6.";
            ToolStripAccount.Items.Add(tsbtScanner);

            // Button Analyzer
            var tsbtAnalyzer = new ToolStripButton {Text = Language.T("Analyzer"), Name = "Analyzer"};
            tsbtAnalyzer.Click += BtnTools_OnClick;
            ToolStripAccount.Items.Add(tsbtAnalyzer);

            // Info Panel Account Statistics
            InfoPanelAccountStatistics = new InfoPanel {Parent = PanelAccount, Dock = DockStyle.Fill};

            new Splitter {Parent = PanelAccount, Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, Height = Gap};

            // Small Balance Chart
            BalanceChart = new SmallBalanceChart
            {
                Parent = PanelAccount,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Bottom,
                MinimumSize = new Size(100, 50),
                ShowDynamicInfo = true,
                IsContextButtonVisible = true
            };
            BalanceChart.PopUpContextMenu.Items.AddRange(GetBalanceChartContextMenuItems());
            BalanceChart.MouseMove += SmallBalanceChartMouseMove;
            BalanceChart.MouseLeave += SmallBalanceChartMouseLeave;
            BalanceChart.MouseUp += SmallBalanceChart_MouseUp;
            toolTip.SetToolTip(BalanceChart, Language.T("Click to view the full chart.") +
                                             Environment.NewLine +
                                             Language.T("Right click to detach chart."));

            PanelAccount.Resize += PnlAccountResize;
        }

        private ToolStripItem[] GetBalanceChartContextMenuItems()
        {
            var menuStripShowFullBalanceChart = new ToolStripMenuItem
            {
                Image = Properties.Resources.balance_chart,
                Text = Language.T("Show Full Balance Chart") + "..."
            };
            menuStripShowFullBalanceChart.Click += ContextMenuShowFullBalanceChartClick;

            var menuStripDetachChart = new ToolStripMenuItem
            {
                Image = Properties.Resources.pushpin_detach,
                Text = Language.T("Detach Balance Chart") + "..."
            };
            menuStripDetachChart.Click += ContextMenuDetachChartClick;

            var itemCollection = new ToolStripItem[]
            {
                menuStripShowFullBalanceChart,
                menuStripDetachChart
            };

            return itemCollection;
        }

        private void ContextMenuShowFullBalanceChartClick(object sender, EventArgs e)
        {
            ShowFullBalanceChart();
        }

        private void ContextMenuDetachChartClick(object sender, EventArgs e)
        {
            DetachBalanceChart();
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        private void PnlAccountResize(object sender, EventArgs e)
        {
            BalanceChart.Height = 2*PanelAccount.ClientSize.Height/(Configs.ShowJournal ? 3 : 4);
        }

        /// <summary>
        /// Show the dynamic info on the status bar
        /// </summary>
        private void SmallBalanceChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallBalanceChart) sender;
            StatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        /// Deletes the dynamic info on the status bar
        /// </summary>
        private void SmallBalanceChartMouseLeave(object sender, EventArgs e)
        {
            StatusLabelChartInfo = string.Empty;
        }

        /// <summary>
        /// Shows the full account chart after clicking on it
        /// </summary>
        private void SmallBalanceChart_MouseUp(object sender, MouseEventArgs e)
        {
            if(!Data.IsData || !Data.IsResult) return;

            if (e.Button == MouseButtons.Left)
            {
                ShowFullBalanceChart();
            }
            else if (e.Button == MouseButtons.Right)
            {
                DetachBalanceChart();
            }
        }

        private static void DetachBalanceChart()
        {
            if(!Data.IsData || !Data.IsResult) return;

            var balanceChart = new SeparateBalanceChart();
            balanceChart.ShowDialog();
        }

        private static void ShowFullBalanceChart()
        {
            if(!Data.IsData || !Data.IsResult) return;

            var chart = new Chart
            {
                BarPixels = Configs.BalanceChartZoom,
                ShowInfoPanel = Configs.BalanceChartInfoPanel,
                ShowDynInfo = Configs.BalanceChartDynamicInfo,
                ShowGrid = Configs.BalanceChartGrid,
                ShowCross = Configs.BalanceChartCross,
                ShowVolume = Configs.BalanceChartVolume,
                ShowPositionLots = Configs.BalanceChartLots,
                ShowOrders = Configs.BalanceChartEntryExitPoints,
                ShowPositionPrice = Configs.BalanceChartCorrectedPositionPrice,
                ShowBalanceEquity = Configs.BalanceChartBalanceEquityChart,
                ShowFloatingPL = Configs.BalanceChartFloatingPLChart,
                ShowIndicators = Configs.BalanceChartIndicators,
                ShowAmbiguousBars = Configs.BalanceChartAmbiguousMark,
                TrueCharts = Configs.BalanceChartTrueCharts,
                ShowProtections = Configs.BalanceChartProtections
            };

            chart.ShowDialog();

            Configs.BalanceChartZoom = chart.BarPixels;
            Configs.BalanceChartInfoPanel = chart.ShowInfoPanel;
            Configs.BalanceChartDynamicInfo = chart.ShowDynInfo;
            Configs.BalanceChartGrid = chart.ShowGrid;
            Configs.BalanceChartCross = chart.ShowCross;
            Configs.BalanceChartVolume = chart.ShowVolume;
            Configs.BalanceChartLots = chart.ShowPositionLots;
            Configs.BalanceChartEntryExitPoints = chart.ShowOrders;
            Configs.BalanceChartCorrectedPositionPrice = chart.ShowPositionPrice;
            Configs.BalanceChartBalanceEquityChart = chart.ShowBalanceEquity;
            Configs.BalanceChartFloatingPLChart = chart.ShowFloatingPL;
            Configs.BalanceChartIndicators = chart.ShowIndicators;
            Configs.BalanceChartAmbiguousMark = chart.ShowAmbiguousBars;
            Configs.BalanceChartTrueCharts = chart.TrueCharts;
            Configs.BalanceChartProtections = chart.ShowProtections;
        }

        /// <summary>
        /// Opens the corresponding tool
        /// </summary>
        protected virtual void BtnTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
        }
    }
}