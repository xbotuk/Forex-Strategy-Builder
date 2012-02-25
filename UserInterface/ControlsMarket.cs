// Forex Strategy Builder - Market controls.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls : MenuAndStatusBar
    /// </summary>
    public partial class Controls
    {
        protected ToolStripComboBox ComboBoxPeriod { get; private set; }
        protected ToolStripComboBox ComboBoxSymbol { get; private set; }
        protected SmallIndicatorChart IndicatorChart { get; private set; }
        protected InfoPanel InfoPanelMarketStatistics { get; private set; }
        private ToolStripButton ButtonCharges { get; set; }

        /// <summary>
        /// Initialize the controls in panel pnlMarket
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
            ButtonCharges.Click += BtnTools_OnClick;
            ToolStripMarket.Items.Add(ButtonCharges);

            ToolStripMarket.Resize += TsMarketResize;

            // Info Panel Market Statistics
            InfoPanelMarketStatistics = new InfoPanel {Parent = PanelMarket, Dock = DockStyle.Fill};

            // Splitter
            new Splitter {Parent = PanelMarket, Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, Height = Gap};

            // Small Indicator Chart
            IndicatorChart = new SmallIndicatorChart
                                 {
                                     Parent = PanelMarket,
                                     Cursor = Cursors.Hand,
                                     Dock = DockStyle.Bottom,
                                     MinimumSize = new Size(100, 50),
                                     ShowDynamicInfo = true
                                 };
            IndicatorChart.MouseUp += IndicatorChartMouseUp;
            IndicatorChart.MouseMove += IndicatorChartMouseMove;
            IndicatorChart.MouseLeave += IndicatorChartMouseLeave;
            toolTip.SetToolTip(IndicatorChart, Language.T("Click to view the full chart."));

            PanelMarket.Resize += PnlMarketResize;
        }

        /// <summary>
        /// Arrange the controls after resizing
        /// </summary>
        private void TsMarketResize(object sender, EventArgs e)
        {
            float width = (ToolStripMarket.ClientSize.Width - ButtonCharges.Width - 18)/100.0F;
            ComboBoxSymbol.Width = (int) (49*width);
            ComboBoxPeriod.Width = (int) (51*width);
        }

        /// <summary>
        /// Arrange the controls after resizing
        /// </summary>
        private void PnlMarketResize(object sender, EventArgs e)
        {
            IndicatorChart.Height = 2*PanelMarket.ClientSize.Height/(Configs.ShowJournal ? 3 : 4);
        }

        /// <summary>
        /// Controls the ComboBoxes: cbxSymbol, cbxPeriod, cbxTax
        /// </summary>
        protected virtual void SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Shows the full chart after clicking on the small indicator chart
        /// </summary>
        private void IndicatorChartMouseUp(object sender, MouseEventArgs e)
        {
            if (!Data.IsData || !Data.IsResult || e.Button != MouseButtons.Left) return;
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

        /// <summary>
        /// Shows the market dynamic info on the Status Bar
        /// </summary>
        private void IndicatorChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallIndicatorChart) sender;
            StatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        /// Deletes the market dynamic info from the Status Bar
        /// </summary>
        private void IndicatorChartMouseLeave(object sender, EventArgs e)
        {
            StatusLabelChartInfo = string.Empty;
        }
    }
}