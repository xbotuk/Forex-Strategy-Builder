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
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Controls : MenuAndStatusBar
    /// </summary>
    public partial class Controls
    {
        private Panel marketChartsBase;
        protected ToolStripComboBox ComboBoxPeriod { get; private set; }
        protected ToolStripComboBox ComboBoxSymbol { get; private set; }
        protected SmallIndicatorChart IndicatorChart { get; private set; }
        protected SmallHistogramChart HistogramChart { get; private set; }
        protected InfoPanel InfoPanelMarketStatistics { get; private set; }
        private ToolStripButton ButtonCharges { get; set; }

        /// <summary>
        ///     Initialize the controls in panel pnlMarket
        /// </summary>
        private void InitializeMarket()
        {
            var toolTip = new ToolTip();

            // Symbol
            ComboBoxSymbol = new ToolStripComboBox
                {
                    Name = "ComboBoxSymbol",
                    AutoSize = false,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ToolTipText = Language.T("Symbol"),
                    Overflow = ToolStripItemOverflow.Never
                };
            foreach (string symbol in Instruments.SymbolList)
                ComboBoxSymbol.Items.Add(symbol);
            ComboBoxSymbol.SelectedIndex = 0;
            ComboBoxSymbol.SelectedIndexChanged += SelectedIndexChanged;
            ToolStripMarket.Items.Add(ComboBoxSymbol);

            // Period
            var periods = new[]
                {
                    "  1 " + Language.T("Minute"),
                    "  5 " + Language.T("Minutes"),
                    "15 " + Language.T("Minutes"),
                    "30 " + Language.T("Minutes"),
                    "  1 " + Language.T("Hour"),
                    "  4 " + Language.T("Hours"),
                    "  1 " + Language.T("Day"),
                    "  1 " + Language.T("Week")
                };
            ComboBoxPeriod = new ToolStripComboBox
                {
                    Name = "ComboBoxPeriod",
                    AutoSize = false,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ToolTipText = Language.T("Data time frame."),
                    Overflow = ToolStripItemOverflow.Never
                };
            foreach (string period in periods)
                ComboBoxPeriod.Items.Add(period);
            ComboBoxPeriod.SelectedIndex = 6;
            ComboBoxPeriod.SelectedIndexChanged += SelectedIndexChanged;
            ToolStripMarket.Items.Add(ComboBoxPeriod);

            // Button Market Properties
            ButtonCharges = new ToolStripButton
                {
                    Text = Language.T("Charges"),
                    Name = "Charges",
                    ToolTipText = Language.T("Spread, Swap numbers, Slippage."),
                    Overflow = ToolStripItemOverflow.Never
                };
            ButtonCharges.Click += BtnToolsOnClick;
            ToolStripMarket.Items.Add(ButtonCharges);

            ToolStripMarket.Resize += TsMarketResize;

            // Info Panel Market Statistics
            InfoPanelMarketStatistics = new InfoPanel {Parent = PanelMarket, Dock = DockStyle.Fill};

            // Splitter
            new Splitter {Parent = PanelMarket, Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, Height = Gap};

            // Panel Charts Base
            marketChartsBase = new Panel
                {
                    Parent = PanelMarket,
                    Dock = DockStyle.Bottom,
                    MinimumSize = new Size(100, 50)
                };

            // Small Indicator Chart
            IndicatorChart = new SmallIndicatorChart
                {
                    Parent = marketChartsBase,
                    Cursor = Cursors.Hand,
                    Dock = DockStyle.Fill,
                    ShowDynamicInfo = true,
                    IsContextButtonVisible = true
                };
            IndicatorChart.PopUpContextMenu.Items.AddRange(GetIndicatorChartContextMenuItems());
            IndicatorChart.MouseUp += IndicatorChartMouseUp;
            IndicatorChart.MouseMove += IndicatorChartMouseMove;
            IndicatorChart.MouseLeave += IndicatorChartMouseLeave;
            toolTip.SetToolTip(IndicatorChart, Language.T("Click to view the full chart."));

            // Small Histogram Chart
            HistogramChart = new SmallHistogramChart
                {
                    Parent = marketChartsBase,
                    Dock = DockStyle.Fill,
                    ShowDynamicInfo = true,
                    Visible = false,
                    IsContextButtonVisible = true
                };
            HistogramChart.PopUpContextMenu.Items.AddRange(GetHistogramChartContextMenuItems());
            HistogramChart.AddContextMenuItems();
            HistogramChart.MouseMove += HistogramChartMouseMove;
            HistogramChart.MouseLeave += IndicatorChartMouseLeave;

            PanelMarket.Resize += PnlMarketResize;
        }

        /// <summary>
        ///     Arrange the controls after resizing
        /// </summary>
        private void TsMarketResize(object sender, EventArgs e)
        {
            float width = (ToolStripMarket.ClientSize.Width - ButtonCharges.Width - 18)/100.0F;
            ComboBoxSymbol.Width = (int) (49*width);
            ComboBoxPeriod.Width = (int) (51*width);
        }

        /// <summary>
        ///     Arrange the controls after resizing
        /// </summary>
        private void PnlMarketResize(object sender, EventArgs e)
        {
            marketChartsBase.Height = 2*PanelMarket.ClientSize.Height/(Configs.ShowJournal ? 3 : 4);
        }

        private ToolStripItem[] GetIndicatorChartContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
                {
                    Image = Resources.bar_chart,
                    Text = Language.T("Full Indicator Chart") + "..."
                };
            mi1.Click += ContextMenuShowFullIndicatorChartClick;

            var mi2 = new ToolStripMenuItem
                {
                    Image = Resources.histogram_chart,
                    Text = Language.T("Trade Distribution Chart")
                };
            mi2.Click += ContextMenuShowHistogramChartClick;

            var itemCollection = new ToolStripItem[]
                {
                    mi1,
                    mi2
                };

            return itemCollection;
        }

        private ToolStripItem[] GetHistogramChartContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
                {
                    Image = Resources.ind_chart,
                    Text = Language.T("Indicator Chart")
                };
            mi1.Click += ContextMenuShowIndicatorChartClick;


            var itemCollection = new ToolStripItem[]
                {
                    mi1
                };

            return itemCollection;
        }

        private void ContextMenuShowFullIndicatorChartClick(object sender, EventArgs e)
        {
            ShowFullIndicatorChart();
        }

        private void ContextMenuShowHistogramChartClick(object sender, EventArgs e)
        {
            IndicatorChart.Visible = false;
            HistogramChart.Visible = true;
        }

        private void ContextMenuShowIndicatorChartClick(object sender, EventArgs e)
        {
            HistogramChart.Visible = false;
            IndicatorChart.Visible = true;
        }

        /// <summary>
        ///     Controls the ComboBoxes: cbxSymbol, cbxPeriod, cbxTax
        /// </summary>
        protected virtual void SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Shows the full chart after clicking on the small indicator chart
        /// </summary>
        private void IndicatorChartMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ShowFullIndicatorChart();
        }

        /// <summary>
        ///     Shows the market dynamic info on the Status Bar
        /// </summary>
        private void HistogramChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallHistogramChart) sender;
            StatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        ///     Shows the market dynamic info on the Status Bar
        /// </summary>
        private void IndicatorChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallIndicatorChart) sender;
            StatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        ///     Deletes the market dynamic info from the Status Bar
        /// </summary>
        private void IndicatorChartMouseLeave(object sender, EventArgs e)
        {
            StatusLabelChartInfo = string.Empty;
        }

        private void ShowFullIndicatorChart()
        {
            if (!Data.IsData || !Data.IsResult) return;
            var chart = new Chart
                {
                    BarPixels = Configs.IndicatorChartZoom,
                    ShowInfoPanel = Configs.IndicatorChartInfoPanel,
                    ShowDynInfo = Configs.IndicatorChartDynamicInfo,
                    ShowGrid = Configs.IndicatorChartGrid,
                    ShowCross = Configs.IndicatorChartCross,
                    ShowVolume = Configs.IndicatorChartVolume,
                    ShowPositionLots = Configs.IndicatorChartLots,
                    ShowOrders = Configs.IndicatorChartEntryExitPoints,
                    ShowPositionPrice = Configs.IndicatorChartCorrectedPositionPrice,
                    ShowBalanceEquity = Configs.IndicatorChartBalanceEquityChart,
                    ShowFloatingPL = Configs.IndicatorChartFloatingPLChart,
                    ShowIndicators = Configs.IndicatorChartIndicators,
                    ShowAmbiguousBars = Configs.IndicatorChartAmbiguousMark,
                    TrueCharts = Configs.IndicatorChartTrueCharts,
                    ShowProtections = Configs.IndicatorChartProtections
                };


            chart.ShowDialog();

            Configs.IndicatorChartZoom = chart.BarPixels;
            Configs.IndicatorChartInfoPanel = chart.ShowInfoPanel;
            Configs.IndicatorChartDynamicInfo = chart.ShowDynInfo;
            Configs.IndicatorChartGrid = chart.ShowGrid;
            Configs.IndicatorChartCross = chart.ShowCross;
            Configs.IndicatorChartVolume = chart.ShowVolume;
            Configs.IndicatorChartLots = chart.ShowPositionLots;
            Configs.IndicatorChartEntryExitPoints = chart.ShowOrders;
            Configs.IndicatorChartCorrectedPositionPrice = chart.ShowPositionPrice;
            Configs.IndicatorChartBalanceEquityChart = chart.ShowBalanceEquity;
            Configs.IndicatorChartFloatingPLChart = chart.ShowFloatingPL;
            Configs.IndicatorChartIndicators = chart.ShowIndicators;
            Configs.IndicatorChartAmbiguousMark = chart.ShowAmbiguousBars;
            Configs.IndicatorChartTrueCharts = chart.TrueCharts;
            Configs.IndicatorChartProtections = chart.ShowProtections;
        }
    }
}