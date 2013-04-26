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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Indicator Chart : Form
    /// </summary>
    public sealed class Chart : Form
    {
        private readonly Brush brushBack;
        private readonly Brush brushDiIndicator;
        private readonly Brush brushDynamicInfo;
        private readonly Brush brushEvenRows;
        private readonly Brush brushFore;
        private readonly Brush brushLabelBackground;
        private readonly Brush brushLabelFore;
        private readonly Brush brushSignalRed;
        private readonly Brush brushTradeClose;
        private readonly Brush brushTradeLong;
        private readonly Brush brushTradeShort;
        private readonly Color colorBarBlack1;
        private readonly Color colorBarBlack2;
        private readonly Color colorBarWhite1;
        private readonly Color colorBarWhite2;
        private readonly Color colorClosedTrade1;
        private readonly Color colorClosedTrade2;
        private readonly Color colorLongTrade1;
        private readonly Color colorLongTrade2;
        private readonly Color colorShortTrade1;
        private readonly Color colorShortTrade2;
        private readonly Font font;
        private readonly Font fontDi; // Font for Dynamic info
        private readonly Font fontDiInd; // Font for Dynamic info Indicators
        private readonly Pen penBarBorder;
        private readonly Pen penBarThick;
        private readonly Pen penCross;
        private readonly Pen penGrid;
        private readonly Pen penGridSolid;
        private readonly Pen penTradeClose;
        private readonly Pen penTradeLong;
        private readonly Pen penTradeShort;
        private readonly Pen penVolume;
        private readonly int spcBottom; // pnlPrice bottom margin
        private readonly int spcLeft; // pnlPrice left margin
        private readonly int spcRight; // pnlPrice right margin
        private readonly int spcTop; // pnlPrice top margin
        private int[] aiInfoType; // 0 - text; 1 - Indicator;
        private string[] asInfoTitle;
        private string[] asInfoValue;
        private int barOld;

        private int chartBars;
        private string chartTitle;
        private int chartWidth;
        private int dynInfoScrollValue; // Dynamic info vertical scrolling position
        private int dynInfoWidth; // Dynamic info width
        private int firstBar;
        private int indPanels;
        private int infoRows; // Dynamic info rows
        private bool isCandleChart = true;
        private bool isDebug;
        private bool isDrawDinInfo; // Draw or not
        private bool isKeyCtrlPressed;
        private bool isMouseInIndicatorChart;
        private bool isMouseInPriceChart;
        private int lastBar;
        private double maxPrice;
        private int maxVolume; // Max Volume in the chart
        private double minPrice;
        private int mouseX;
        private int mouseXOld;
        private int mouseY;
        private int mouseYOld;
        private int posCount;
        private bool[] repeatedIndicators;
        private HScrollBar scroll;
        private Size szDate;
        private Size szDateL;
        private Size szPrice;

        private int verticalScale = 1;
        private int xDynInfoCol2; // Dynamic info second column X
        private int xLeft; // pnlPrice left coordinate
        private int xRight; // pnlPrice right coordinate
        private int yBottom; // pnlPrice bottom coordinate
        private int yBottomText; // pnlPrice bottom coordinate for date text
        private double yScale;
        private int yTop; // pnlPrice top coordinate
        private double yVolScale; // The scale for drawing the Volume

        /// <summary>
        ///     The default constructor.
        /// </summary>
        public Chart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            BarPixels = 8;
            Text = Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString + " - " + Data.ProgramName;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;

            PnlCharts = new Panel {Parent = this, Dock = DockStyle.Fill};

            PnlInfo = new Panel {Parent = this, BackColor = LayoutColors.ColorControlBack, Dock = DockStyle.Right};
            PnlInfo.Resize += PnlInfoResize;
            PnlInfo.Paint += PnlInfoPaint;

            ShowInfoPanel = true;
            dynInfoScrollValue = 0;

            font = new Font(Font.FontFamily, Font.Size);

            // Dynamic info fonts
            fontDi = font;
            fontDiInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();

            szDate = g.MeasureString("99/99 99:99", font).ToSize();
            szDateL = g.MeasureString("99/99/99 99:99", font).ToSize();
            szPrice = g.MeasureString("9.99999", font).ToSize();

            g.Dispose();

            spcTop = font.Height;
            spcBottom = font.Height*8/5;
            spcLeft = 2;
            spcRight = szPrice.Width + 4;

            brushBack = new SolidBrush(LayoutColors.ColorChartBack);
            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            brushLabelBackground = new SolidBrush(LayoutColors.ColorLabelBack);
            brushLabelFore = new SolidBrush(LayoutColors.ColorLabelText);
            brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            brushDiIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            brushEvenRows = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushTradeLong = new SolidBrush(LayoutColors.ColorTradeLong);
            brushTradeShort = new SolidBrush(LayoutColors.ColorTradeShort);
            brushTradeClose = new SolidBrush(LayoutColors.ColorTradeClose);
            brushSignalRed = new SolidBrush(LayoutColors.ColorSignalRed);

            penGrid = new Pen(LayoutColors.ColorChartGrid);
            penGridSolid = new Pen(LayoutColors.ColorChartGrid);
            penCross = new Pen(LayoutColors.ColorChartCross);
            penVolume = new Pen(LayoutColors.ColorVolume);
            penBarBorder = new Pen(LayoutColors.ColorBarBorder);
            penBarThick = new Pen(LayoutColors.ColorBarBorder, 3);
            penTradeLong = new Pen(LayoutColors.ColorTradeLong);
            penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            penGrid.DashStyle = DashStyle.Dash;
            penGrid.DashPattern = new float[] {4, 2};

            colorBarWhite1 = Data.GetGradientColor(LayoutColors.ColorBarWhite, 30);
            colorBarWhite2 = Data.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.GetGradientColor(LayoutColors.ColorBarBlack, 30);
            colorBarBlack2 = Data.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeLong, 30);
            colorLongTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeLong, -30);
            colorShortTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeShort, 30);
            colorShortTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeClose, 30);
            colorClosedTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeClose, -30);
        }

        private Panel PnlCharts { get; set; }
        private Panel PnlInfo { get; set; }
        private ToolStripButton[] AChartButtons { get; set; }
        private ToolStrip TsChartButtons { get; set; }
        private Panel PnlBalanceChart { get; set; }
        private Panel PnlFloatingPLChart { get; set; }
        private Panel[] PnlInd { get; set; }
        private Panel PnlPrice { get; set; }
        private Splitter SpliterBalance { get; set; }
        private Splitter SpliterFloatingPL { get; set; }
        private Splitter[] SplitterInd { get; set; }

        public int BarPixels { get; set; }
        public bool ShowInfoPanel { get; set; }
        public bool ShowGrid { get; set; }
        public bool ShowCross { get; set; }
        public bool ShowVolume { get; set; }
        public bool ShowPositionLots { get; set; }
        public bool ShowOrders { get; set; }
        public bool ShowDynInfo { get; set; }
        public bool ShowPositionPrice { get; set; }
        public bool ShowBalanceEquity { get; set; }
        public bool ShowFloatingPL { get; set; }
        public bool ShowIndicators { get; set; }
        public bool ShowProtections { get; set; }
        public bool ShowAmbiguousBars { get; set; }
        public bool TrueCharts { get; set; }

        /// <summary>
        ///     After loading select the Scrollbar
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PnlInfo.Visible = ShowInfoPanel;
            PnlCharts.Padding = ShowInfoPanel ? new Padding(0, 0, 2, 0) : new Padding(0);

            if (ShowInfoPanel)
                Text = Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString + " - " + Data.ProgramName;
            else
                Text = Data.ProgramName + @"   http://forexsb.com";

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            PnlCharts.Resize += PnlBaseResize;
            PnlPrice.Resize += PnlPriceResize;

            Size = new Size(720, 500);
            MinimumSize = new Size(500, 500);

            if (ShowInfoPanel)
            {
                GenerateDynamicInfo(lastBar);
                SetupDynamicInfo();
                isDrawDinInfo = true;
                PnlInfo.Invalidate();
            }

            scroll.Select();
        }

        /// <summary>
        ///     Sets ups the chart's buttons.
        /// </summary>
        private void SetupButtons()
        {
            TsChartButtons = new ToolStrip {Parent = PnlCharts};

            AChartButtons = new ToolStripButton[17];
            for (int i = 0; i < 17; i++)
            {
                AChartButtons[i] = new ToolStripButton
                    {Tag = (ChartButtons) i, DisplayStyle = ToolStripItemDisplayStyle.Image};
                AChartButtons[i].Click += ButtonChartClick;
                TsChartButtons.Items.Add(AChartButtons[i]);
                if (i > 13)
                    AChartButtons[i].Alignment = ToolStripItemAlignment.Right;
                if (i == 1 || i == 6 || i == 10 || i == 12)
                    TsChartButtons.Items.Add(new ToolStripSeparator());
            }

            // Grid
            AChartButtons[(int) ChartButtons.Grid].Image = Resources.chart_grid;
            AChartButtons[(int) ChartButtons.Grid].ToolTipText = Language.T("Grid") + "   G";
            AChartButtons[(int) ChartButtons.Grid].Checked = ShowGrid;

            // Cross
            AChartButtons[(int) ChartButtons.Cross].Image = Resources.chart_cross;
            AChartButtons[(int) ChartButtons.Cross].ToolTipText = Language.T("Cross") + "   C";
            AChartButtons[(int) ChartButtons.Cross].Checked = ShowCross;

            // Volume
            AChartButtons[(int) ChartButtons.Volume].Image = Resources.chart_volume;
            AChartButtons[(int) ChartButtons.Volume].ToolTipText = Language.T("Volume") + "   V";
            AChartButtons[(int) ChartButtons.Volume].Checked = ShowVolume;

            // Orders
            AChartButtons[(int) ChartButtons.Orders].Image = Resources.chart_entry_points;
            AChartButtons[(int) ChartButtons.Orders].ToolTipText = Language.T("Entry / Exit points") + "   O";
            AChartButtons[(int) ChartButtons.Orders].Checked = ShowOrders;

            // Position lots
            AChartButtons[(int) ChartButtons.PositionLots].Image = Resources.chart_lots;
            AChartButtons[(int) ChartButtons.PositionLots].ToolTipText = Language.T("Position lots") + "   L";
            AChartButtons[(int) ChartButtons.PositionLots].Checked = ShowPositionLots;

            // Position price
            AChartButtons[(int) ChartButtons.PositionPrice].Image = Resources.chart_pos_price;
            AChartButtons[(int) ChartButtons.PositionPrice].ToolTipText = Language.T("Corrected position price") +
                                                                          "   P";
            AChartButtons[(int) ChartButtons.PositionPrice].Checked = ShowPositionPrice;

            // Position price
            AChartButtons[(int) ChartButtons.Protections].Image = Resources.chart_protection;
            AChartButtons[(int) ChartButtons.Protections].ToolTipText =
                Language.T("Break Even, Permanent SL, Permanent TP") + "   E";
            AChartButtons[(int) ChartButtons.Protections].Checked = ShowProtections;

            // Ambiguous Bars
            AChartButtons[(int) ChartButtons.AmbiguousBars].Image = Resources.chart_ambiguous_bars;
            AChartButtons[(int) ChartButtons.AmbiguousBars].ToolTipText = Language.T("Ambiguous bars mark") + "   M";
            AChartButtons[(int) ChartButtons.AmbiguousBars].Checked = ShowAmbiguousBars;

            // Indicators
            AChartButtons[(int) ChartButtons.Indicators].Image = Resources.chart_indicators;
            AChartButtons[(int) ChartButtons.Indicators].ToolTipText = Language.T("Indicators charts") + "   D";
            AChartButtons[(int) ChartButtons.Indicators].Checked = ShowIndicators;

            // Balance Equity
            AChartButtons[(int) ChartButtons.BalanceEquity].Image = Resources.chart_balance_equity;
            AChartButtons[(int) ChartButtons.BalanceEquity].ToolTipText = Language.T("Balance / Equity chart") + "   B";
            AChartButtons[(int) ChartButtons.BalanceEquity].Checked = ShowBalanceEquity;

            // Floating P/L
            AChartButtons[(int) ChartButtons.FloatingPL].Image = Resources.chart_floating_pl;
            AChartButtons[(int) ChartButtons.FloatingPL].ToolTipText = Language.T("Floating P/L chart") + "   F";
            AChartButtons[(int) ChartButtons.FloatingPL].Checked = ShowFloatingPL;

            // Zoom in
            AChartButtons[(int) ChartButtons.ZoomIn].Image = Resources.chart_zoom_in;
            AChartButtons[(int) ChartButtons.ZoomIn].ToolTipText = Language.T("Zoom in") + "   +";

            // Zoom out
            AChartButtons[(int) ChartButtons.ZoomOut].Image = Resources.chart_zoom_out;
            AChartButtons[(int) ChartButtons.ZoomOut].ToolTipText = Language.T("Zoom out") + "   -";

            // True Charts
            AChartButtons[(int) ChartButtons.TrueCharts].Image = Resources.chart_true_charts;
            AChartButtons[(int) ChartButtons.TrueCharts].Checked = TrueCharts;
            AChartButtons[(int) ChartButtons.TrueCharts].ToolTipText = Language.T("True indicator charts") + "   T";

            // Show dynamic info
            AChartButtons[(int) ChartButtons.DynamicInfo].Image = Resources.chart_dyninfo;
            AChartButtons[(int) ChartButtons.DynamicInfo].Checked = ShowInfoPanel;
            AChartButtons[(int) ChartButtons.DynamicInfo].ToolTipText = Language.T("Show / hide the info panel") +
                                                                        "   I";

            // Move Dynamic Info Up
            AChartButtons[(int) ChartButtons.DInfoUp].Image = Resources.chart_dinfo_up;
            AChartButtons[(int) ChartButtons.DInfoUp].ToolTipText = Language.T("Scroll info upwards") + "   A, S";
            AChartButtons[(int) ChartButtons.DInfoUp].Visible = ShowInfoPanel;

            // Move Dynamic Info Down
            AChartButtons[(int) ChartButtons.DInfoDwn].Image = Resources.chart_dinfo_down;
            AChartButtons[(int) ChartButtons.DInfoDwn].ToolTipText = Language.T("Scroll info downwards") + "   Z, X";
            AChartButtons[(int) ChartButtons.DInfoDwn].Visible = ShowInfoPanel;
        }

        /// <summary>
        ///     Create and sets the indicator panels
        /// </summary>
        private void SetupIndicatorPanels()
        {
            PnlPrice = new Panel {Parent = PnlCharts, Dock = DockStyle.Fill, BackColor = LayoutColors.ColorChartBack};
            PnlPrice.MouseLeave += PnlPriceMouseLeave;
            PnlPrice.MouseDoubleClick += PnlPriceMouseDoubleClick;
            PnlPrice.MouseMove += PnlPriceMouseMove;
            PnlPrice.MouseDown += PanelMouseDown;
            PnlPrice.MouseUp += PanelMouseUp;
            PnlPrice.Paint += PnlPricePaint;

            // Indicator panels
            indPanels = 0;
            var asIndicatorTexts = new string[Data.Strategy.Slots];
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                Indicator indicator = IndicatorManager.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName);
                indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
                asIndicatorTexts[slot] = indicator.ToString();
                indPanels += Data.Strategy.Slot[slot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            repeatedIndicators = new bool[Data.Strategy.Slots];
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                repeatedIndicators[slot] = false;
                for (int i = 0; i < slot; i++)
                    repeatedIndicators[slot] = asIndicatorTexts[slot] == asIndicatorTexts[i];
            }

            PnlInd = new Panel[indPanels];
            SplitterInd = new Splitter[indPanels];
            for (int i = 0; i < indPanels; i++)
            {
                SplitterInd[i] = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

                PnlInd[i] = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorControlBack};
                PnlInd[i].Paint += PnlIndPaint;
                PnlInd[i].Resize += PnlIndResize;
                PnlInd[i].MouseMove += SepChartMouseMove;
                PnlInd[i].MouseLeave += SepChartMouseLeave;
                PnlInd[i].MouseDown += PanelMouseDown;
                PnlInd[i].MouseUp += PanelMouseUp;
            }

            int iIndex = 0;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (!Data.Strategy.Slot[slot].SeparatedChart) continue;
                PnlInd[iIndex].Tag = slot;
                iIndex++;
            }

            for (int i = 0; i < indPanels && ShowIndicators; i++)
            {
                SplitterInd[i].Parent = PnlCharts;
                PnlInd[i].Parent = PnlCharts;
            }

            // Balance chart
            SpliterBalance = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

            PnlBalanceChart = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorChartBack};
            PnlBalanceChart.Paint += PnlBalancePaint;
            PnlBalanceChart.Resize += PnlBalanceResize;
            PnlBalanceChart.MouseMove += SepChartMouseMove;
            PnlBalanceChart.MouseLeave += SepChartMouseLeave;
            PnlBalanceChart.MouseDown += PanelMouseDown;
            PnlBalanceChart.MouseUp += PanelMouseUp;

            if (ShowBalanceEquity)
            {
                SpliterBalance.Parent = PnlCharts;
                PnlBalanceChart.Parent = PnlCharts;
            }

            // Floating Profit Loss chart
            SpliterFloatingPL = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

            PnlFloatingPLChart = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorChartBack};
            PnlFloatingPLChart.Paint += PnlFloatingPLPaint;
            PnlFloatingPLChart.Resize += PnlFloatingPLResize;
            PnlFloatingPLChart.MouseMove += SepChartMouseMove;
            PnlFloatingPLChart.MouseLeave += SepChartMouseLeave;
            PnlFloatingPLChart.MouseDown += PanelMouseDown;
            PnlFloatingPLChart.MouseUp += PanelMouseUp;

            if (ShowFloatingPL)
            {
                SpliterFloatingPL.Parent = PnlCharts;
                PnlFloatingPLChart.Parent = PnlCharts;
            }

            scroll = new HScrollBar
                {
                    Parent = PnlCharts,
                    Dock = DockStyle.Bottom,
                    TabStop = true,
                    Minimum = Data.FirstBar,
                    Maximum = Data.Bars - 1,
                    SmallChange = 1
                };
            scroll.ValueChanged += ScrollValueChanged;
            scroll.MouseWheel += ScrollMouseWheel;
            scroll.KeyUp += ScrollKeyUp;
            scroll.KeyDown += ScrollKeyDown;
        }

        /// <summary>
        ///     Sets the chart's parameters.
        /// </summary>
        private void SetPriceChartParam()
        {
            chartBars = chartWidth/BarPixels;
            chartBars = Math.Min(chartBars, Data.Bars - Data.FirstBar);
            firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            firstBar = Math.Min(firstBar, Data.Bars - 1);
            lastBar = Math.Max(firstBar + chartBars - 1, firstBar);

            scroll.Value = firstBar;
            scroll.LargeChange = Math.Max(chartBars, 1);
        }

        /// <summary>
        ///     Sets the indicator chart title
        /// </summary>
        private void SetupChartTitle()
        {
            // Chart title
            chartTitle = Data.Symbol + "  " + Data.PeriodString + " " + Data.Strategy.StrategyName;

            if (!ShowIndicators) return;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                for (int iComp = 0; iComp < Data.Strategy.Slot[slot].Component.Length; iComp++)
                {
                    if (Data.Strategy.Slot[slot].Component[iComp].ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }
                }
                if (isChart)
                {
                    Indicator indicator = IndicatorManager.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName);
                    indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
                    if (!chartTitle.Contains(indicator.ToString()))
                        chartTitle += Environment.NewLine + indicator;
                }
            }
        }

        /// <summary>
        ///     Sets the sizes of the panels after resizing.
        /// </summary>
        private void PnlBaseResize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetPriceChartParam();
            dynInfoScrollValue = 0;
        }

        /// <summary>
        ///     Calculates the panels' height
        /// </summary>
        private void SetAllPanelsHeight()
        {
            int panelNumber = ShowIndicators ? indPanels : 0;
            panelNumber += ShowFloatingPL ? 1 : 0;
            panelNumber += ShowBalanceEquity ? 1 : 0;

            int iAvailableHeight = PnlCharts.ClientSize.Height - TsChartButtons.Height - scroll.Height - panelNumber*4;

            int iPnlIndHeight = iAvailableHeight/(2 + panelNumber);

            for (int i = 0; i < indPanels && ShowIndicators; i++)
                PnlInd[i].Height = iPnlIndHeight;

            if (ShowFloatingPL)
                PnlFloatingPLChart.Height = iPnlIndHeight;

            if (ShowBalanceEquity)
                PnlBalanceChart.Height = iPnlIndHeight;
        }

        /// <summary>
        ///     Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        private void PnlPriceResize(object sender, EventArgs e)
        {
            xLeft = spcLeft;
            xRight = PnlPrice.ClientSize.Width - spcRight;
            chartWidth = Math.Max(xRight - xLeft, 0);
            yTop = spcTop;
            yBottom = PnlPrice.ClientSize.Height - spcBottom;
            yBottomText = PnlPrice.ClientSize.Height - spcBottom*5/8 - 4;
            PnlPrice.Invalidate();
        }

        /// <summary>
        ///     Invalidates the panels
        /// </summary>
        private void PnlIndResize(object sender, EventArgs e)
        {
            if (!ShowIndicators) return;
            ((Panel) sender).Invalidate();
        }

        /// <summary>
        ///     Invalidates the panel
        /// </summary>
        private void PnlBalanceResize(object sender, EventArgs e)
        {
            if (!ShowBalanceEquity) return;

            ((Panel) sender).Invalidate();
        }

        /// <summary>
        ///     Invalidates the panel
        /// </summary>
        private void PnlFloatingPLResize(object sender, EventArgs e)
        {
            if (!ShowFloatingPL) return;

            ((Panel) sender).Invalidate();
        }

        /// <summary>
        ///     Paints the panel PnlPrice
        /// </summary>
        private void PnlPricePaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            // Searching the min and the max price and volume
            maxPrice = double.MinValue;
            minPrice = double.MaxValue;
            maxVolume = int.MinValue;
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (Data.High[bar] > maxPrice) maxPrice = Data.High[bar];
                if (Data.Low[bar] < minPrice) minPrice = Data.Low[bar];
                if (Data.Volume[bar] > maxVolume) maxVolume = Data.Volume[bar];
            }

            double pricePixel = (maxPrice - minPrice)/(yBottom - yTop);
            if (ShowVolume)
                minPrice -= pricePixel*30;
            else if (ShowPositionLots)
                minPrice -= pricePixel*10;

            maxPrice += pricePixel*verticalScale;
            minPrice -= pricePixel*verticalScale;

            // Grid
            int countLabels = Math.Max((yBottom - yTop)/30, 1);
            double deltaPoint = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3)
                                    ? Data.InstrProperties.Point*100
                                    : Data.InstrProperties.Point*10;
            double deltaLabel =
                Math.Max(Math.Round((maxPrice - minPrice)/countLabels, Data.InstrProperties.Point < 0.001 ? 3 : 1),
                         deltaPoint);
            minPrice = Math.Round(minPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) - deltaPoint;
            countLabels = (int) Math.Ceiling((maxPrice - minPrice)/deltaLabel);
            maxPrice = minPrice + countLabels*deltaLabel;
            yScale = (yBottom - yTop)/(countLabels*deltaLabel);
            yVolScale = maxVolume > 0 ? 40.0f/maxVolume : 0f; // 40 - the highest volume line

            // Price labels
            for (double label = minPrice; label <= maxPrice + Data.InstrProperties.Point; label += deltaLabel)
            {
                var iLabelY = (int) Math.Round(yBottom - (label - minPrice)*yScale);
                g.DrawString(label.ToString(Data.Ff), Font, brushFore, xRight, iLabelY - Font.Height/2 - 1);
                if (ShowGrid || Math.Abs(label - minPrice) < 0.000001)
                    g.DrawLine(penGrid, spcLeft, iLabelY, xRight, iLabelY);
                else
                    g.DrawLine(penGrid, xRight - 5, iLabelY, xRight, iLabelY);
            }
            // Date labels
            for (int lineBar = lastBar;
                 lineBar > firstBar;
                 lineBar -= (int) Math.Round((szDate.Width + 10.0)/BarPixels + 1))
            {
                int xLine = (lineBar - firstBar)*BarPixels + spcLeft + BarPixels/2 - 1;
                if (ShowGrid)
                    g.DrawLine(penGrid, xLine, yTop, xLine, yBottom + 2);
                string date = String.Format("{0} {1}", Data.Time[lineBar].ToString(Data.Dfs),
                                            Data.Time[lineBar].ToString("HH:mm"));
                g.DrawString(date, font, brushFore, xLine - szDate.Width/2, yBottomText);
            }

            // Cross
            if (ShowCross && mouseX > xLeft - 1 && mouseX < xRight + 1)
            {
                int crossBar = (mouseX - spcLeft)/BarPixels;
                crossBar = Math.Max(0, crossBar);
                crossBar = Math.Min(chartBars - 1, crossBar);
                crossBar += firstBar;
                crossBar = Math.Min(Data.Bars - 1, crossBar);

                // Vertical positions
                var point = new Point(mouseX - szDateL.Width/2, yBottomText);
                var rec = new Rectangle(point, szDateL);

                // Vertical line
                if (isMouseInPriceChart && mouseY > yTop - 1 && mouseY < yBottom + 1)
                {
                    g.DrawLine(penCross, mouseX, yTop, mouseX, mouseY - 10);
                    g.DrawLine(penCross, mouseX, mouseY + 10, mouseX, yBottomText);
                }
                else if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.DrawLine(penCross, mouseX, yTop, mouseX, yBottomText);
                }

                // Date Window
                if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.FillRectangle(brushLabelBackground, rec);
                    g.DrawRectangle(penCross, rec);
                    string sDate = Data.Time[crossBar].ToString(Data.Df) + " " + Data.Time[crossBar].ToString("HH:mm");
                    g.DrawString(sDate, font, brushLabelFore, point);
                }

                if (isMouseInPriceChart && mouseY > yTop - 1 && mouseY < yBottom + 1)
                {
                    //Horizontal positions
                    point = new Point(xRight, mouseY - szPrice.Height/2);
                    rec = new Rectangle(point, szPrice);
                    // Horizontal line
                    g.DrawLine(penCross, xLeft, mouseY, mouseX - 10, mouseY);
                    g.DrawLine(penCross, mouseX + 10, mouseY, xRight, mouseY);
                    // Price Window
                    g.FillRectangle(brushLabelBackground, rec);
                    g.DrawRectangle(penCross, rec);
                    string sPrice = ((yBottom - mouseY)/yScale + minPrice).ToString(Data.Ff);
                    g.DrawString(sPrice, font, brushLabelFore, point);
                }
            }

            // Draws Volume, Lots and Bars
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x = (bar - firstBar)*BarPixels + spcLeft;
                var yOpen = (int) Math.Round(yBottom - (Data.Open[bar] - minPrice)*yScale);
                var yHigh = (int) Math.Round(yBottom - (Data.High[bar] - minPrice)*yScale);
                var yLow = (int) Math.Round(yBottom - (Data.Low[bar] - minPrice)*yScale);
                var yClose = (int) Math.Round(yBottom - (Data.Close[bar] - minPrice)*yScale);
                var yVolume = (int) Math.Round(yBottom - Data.Volume[bar]*yVolScale);

                // Draw the volume
                if (ShowVolume && yVolume != yBottom)
                    g.DrawLine(penVolume, x + BarPixels/2 - 1, yVolume, x + BarPixels/2 - 1, yBottom);

                // Draw position lots
                if (ShowPositionLots && Backtester.IsPos(bar))
                {
                    var posHeight = (int) Math.Round(Math.Max(Backtester.SummaryLots(bar)*2, 2));
                    int posY = yBottom - posHeight + 1;

                    if (Backtester.SummaryDir(bar) == PosDirection.Long)
                    {
                        // Long
                        var rect = new Rectangle(x - 1, posY, BarPixels + 1, posHeight);
                        var lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, posY, BarPixels - 1, posHeight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                    {
                        // Short
                        var rect = new Rectangle(x - 1, posY, BarPixels + 1, posHeight);
                        var lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, posY, BarPixels - 1, posHeight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else
                    {
                        // Closed
                        var rect = new Rectangle(x - 1, 2, BarPixels + 1, 2);
                        var lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, yBottom - 1, BarPixels - 1, 2);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (isCandleChart)
                {
                    g.DrawLine(BarPixels < 25 ? penBarBorder : penBarThick, x + BarPixels/2 - 1, yLow,
                               x + BarPixels/2 - 1, yHigh);

                    if (BarPixels == 2)
                        g.DrawLine(penBarBorder, x, yClose, x + 1, yClose);
                    else
                    {
                        if (yClose < yOpen)
                        {
                            // White bar
                            var rect = new Rectangle(x, yClose, BarPixels - 2, yOpen - yClose);
                            var lgBrush = new LinearGradientBrush(rect, colorBarWhite1, colorBarWhite2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(penBarBorder, x, yClose, BarPixels - 2, yOpen - yClose);
                        }
                        else if (yClose > yOpen)
                        {
                            // Black bar
                            var rect = new Rectangle(x, yOpen, BarPixels - 2, yClose - yOpen);
                            var lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(penBarBorder, rect);
                        }
                        else
                        {
                            // Cross
                            g.DrawLine(BarPixels < 25 ? penBarBorder : penBarThick, x, yClose, x + BarPixels - 2,
                                       yClose);
                        }
                    }
                }
                else
                {
                    if (BarPixels == 2)
                    {
                        g.DrawLine(penBarBorder, x, yClose, x + 1, yClose);
                        g.DrawLine(penBarBorder, x + BarPixels/2 - 1, yLow, x + BarPixels/2 - 1, yHigh);
                    }
                    else if (BarPixels <= 16)
                    {
                        g.DrawLine(penBarBorder, x + BarPixels/2 - 1, yLow, x + BarPixels/2 - 1, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarBorder, x, yOpen, x + BarPixels/2 - 1, yOpen);
                            g.DrawLine(penBarBorder, x + BarPixels/2 - 1, yClose, x + BarPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarBorder, x, yClose, x + BarPixels - 2, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(penBarThick, x + BarPixels/2 - 1, yLow + 2, x + BarPixels/2 - 1, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarThick, x + 1, yOpen, x + BarPixels/2 - 1, yOpen);
                            g.DrawLine(penBarThick, x + BarPixels/2, yClose, x + BarPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarThick, x, yClose, x + BarPixels - 2, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(0, yTop, xRight, yBottom - yTop));
            for (int slot = 0; slot < Data.Strategy.Slots && ShowIndicators; slot++)
            {
                if (Data.Strategy.Slot[slot].SeparatedChart || repeatedIndicators[slot]) continue;

                int cloudUp = -1; // For Ichimoku and similar
                int cloudDown = -1; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int indicatorValueShift = 1;
                foreach (ListParam listParam in Data.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < Data.Strategy.Slot[slot].Component.Length; comp++)
                {
                    var pen = new Pen(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                    var penTc = new Pen(Data.Strategy.Slot[slot].Component[comp].ChartColor)
                        {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                    if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {
                        // Line
                        if (TrueCharts)
                        {
                            // True Charts
                            var point = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar)*BarPixels + indicatorValueShift*(BarPixels - 2);
                                var y = (int) Math.Round(yBottom - (value - minPrice)*yScale);

                                if (Math.Abs(value - 0) < 0.00001)
                                    point[bar - firstBar] = point[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    point[bar - firstBar] = new Point(x, y);
                            }

                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                // All bars except the last one
                                int i = bar - firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                                if (bar == firstBar && isIndicatorValueAtClose)
                                {
                                    // First bar
                                    double value = Data.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = spcLeft + (bar - firstBar)*BarPixels;
                                    var y = (int) Math.Round(yBottom - (value - minPrice)*yScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (BarPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 5, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 4, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 2, y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTc, x + BarPixels - 2, y + 2, x + BarPixels - 2,
                                                       point[i].Y - 2);
                                        else
                                            g.DrawLine(penTc, x + BarPixels - 2, y - 2, x + BarPixels - 2,
                                                       point[i].Y + 2);
                                    }
                                }

                                if (bar < lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);

                                    if (BarPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                        else
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i + 1].Y > point[i].Y)
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                       point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                       point[i + 1].Y + 2);
                                    }
                                }

                                if (bar == lastBar && !isIndicatorValueAtClose && BarPixels > 3)
                                {
                                    // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + BarPixels - 2, point[i].Y);
                                }
                            }
                        }
                        else
                        {
                            // Regular Charts
                            var aPoint = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(yBottom - (dValue - minPrice)*yScale);

                                if (Math.Abs(dValue - 0) < 0.00001)
                                    aPoint[bar - firstBar] = aPoint[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    aPoint[bar - firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {
                        // Dots
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                            var y = (int) Math.Round(yBottom - (dValue - minPrice)*yScale);
                            if (BarPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {
                        // Level
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar)*BarPixels + spcLeft;
                            var y = (int) Math.Round(yBottom - (dValue - minPrice)*yScale);
                            g.DrawLine(pen, x, y, x + BarPixels - 1, y);
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp >= 0 && cloudDown >= 0)
                {
                    var pointsUp = new PointF[lastBar - firstBar + 1];
                    var pointsDown = new PointF[lastBar - firstBar + 1];
                    for (int bar = firstBar; bar <= lastBar; bar++)
                    {
                        double dValueUp = Data.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = Data.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        pointsUp[bar - firstBar].X = (bar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                        pointsUp[bar - firstBar].Y = (int) Math.Round(yBottom - (dValueUp - minPrice)*yScale);
                        pointsDown[bar - firstBar].X = (bar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                        pointsDown[bar - firstBar].Y = (int) Math.Round(yBottom - (dValueDown - minPrice)*yScale);
                    }

                    var pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(pointsUp[0].X, 0), pointsUp[0]);
                    pathUp.AddLines(pointsUp);
                    pathUp.AddLine(pointsUp[lastBar - firstBar], new PointF(pointsUp[lastBar - firstBar].X, 0));
                    pathUp.AddLine(new PointF(pointsUp[lastBar - firstBar].X, 0), new PointF(pointsUp[0].X, 0));

                    var pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(pointsDown[0].X, 0), pointsDown[0]);
                    pathDown.AddLines(pointsDown);
                    pathDown.AddLine(pointsDown[lastBar - firstBar], new PointF(pointsDown[lastBar - firstBar].X, 0));
                    pathDown.AddLine(new PointF(pointsDown[lastBar - firstBar].X, 0), new PointF(pointsDown[0].X, 0));

                    Color colorUp = Color.FromArgb(50, Data.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, Data.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    var penUp = new Pen(Data.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    var penDown = new Pen(Data.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    penUp.DashStyle = DashStyle.Dash;
                    penDown.DashStyle = DashStyle.Dash;

                    Brush brushUp = new SolidBrush(colorUp);
                    Brush brushDown = new SolidBrush(colorDown);

                    var regionUp = new Region(pathUp);
                    regionUp.Exclude(pathDown);
                    g.FillRegion(brushDown, regionUp);

                    var regionDown = new Region(pathDown);
                    regionDown.Exclude(pathUp);
                    g.FillRegion(brushUp, regionDown);

                    g.DrawLines(penUp, pointsUp);
                    g.DrawLines(penDown, pointsDown);
                }
            }
            g.ResetClip();

            // Draws position price, deals and Ambiguous note
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x = (bar - firstBar)*BarPixels + spcLeft;
                var yHigh = (int) Math.Round(yBottom - (Data.High[bar] - minPrice)*yScale);

                // Draw the corrected position price
                for (int iPos = 0; iPos < Backtester.Positions(bar) && ShowPositionPrice; iPos++)
                {
                    var yPrice = (int) Math.Round(yBottom - (Backtester.SummaryPrice(bar) - minPrice)*yScale);

                    if (yPrice >= yBottom || yPrice <= yTop) continue;

                    if (Backtester.SummaryDir(bar) == PosDirection.Long)
                    {
                        // Long
                        g.DrawLine(penTradeLong, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                    {
                        // Short
                        g.DrawLine(penTradeShort, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Closed)
                    {
                        // Closed
                        g.DrawLine(penTradeClose, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                }

                // Draw Break Even

                for (int ord = 0; ord < Backtester.Orders(bar) && ShowProtections; ord++)
                {
                    Order order = Backtester.OrdFromNumb(Backtester.OrdNumb(bar, ord));
                    if (order.OrdOrigin == OrderOrigin.BreakEven)
                    {
                        var yOrder = (int) Math.Round(yBottom - (order.OrdPrice - minPrice)*yScale);
                        if (yOrder < yBottom && yOrder > yTop)
                        {
                            var penBreakEven = new Pen(LayoutColors.ColorTradeClose) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penBreakEven, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                    else if (order.OrdOrigin == OrderOrigin.PermanentStopLoss)
                    {
                        var yOrder = (int) Math.Round(yBottom - (order.OrdPrice - minPrice)*yScale);
                        if (yOrder < yBottom && yOrder > yTop)
                        {
                            var penPermSL = new Pen(LayoutColors.ColorTradeShort) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penPermSL, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                    else if (order.OrdOrigin == OrderOrigin.PermanentTakeProfit)
                    {
                        var yOrder = (int) Math.Round(yBottom - (order.OrdPrice - minPrice)*yScale);
                        if (yOrder < yBottom && yOrder > yTop)
                        {
                            var penPermTP = new Pen(LayoutColors.ColorTradeLong) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penPermTP, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                }

                // Draw the deals
                for (int iPos = 0; iPos < Backtester.Positions(bar) && ShowOrders; iPos++)
                {
                    if (Backtester.PosTransaction(bar, iPos) == Transaction.Transfer) continue;

                    var yDeal = (int) Math.Round(yBottom - (Backtester.PosOrdPrice(bar, iPos) - minPrice)*yScale);

                    if (Backtester.PosDir(bar, iPos) == PosDirection.Long ||
                        Backtester.PosDir(bar, iPos) == PosDirection.Short)
                    {
                        if (Backtester.OrdFromNumb(Backtester.PosOrdNumb(bar, iPos)).OrdDir == OrderDirection.Buy)
                        {
                            // Buy
                            var pen = new Pen(brushTradeLong, 2);
                            if (BarPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                            }
                            else if (BarPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + 4, yDeal);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal - 3);
                            }
                            else if (BarPixels > 8)
                            {
                                int d = BarPixels/2 - 1;
                                int x1 = x + d;
                                int x2 = x + BarPixels - 2;
                                g.DrawLine(pen, x, yDeal, x1, yDeal);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                                g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                                g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                            }
                        }
                        else
                        {
                            // Sell
                            var pen = new Pen(brushTradeShort, 2);
                            if (BarPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                            }
                            else if (BarPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal + 1, x + 4, yDeal + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal + 3);
                            }
                            else if (BarPixels > 8)
                            {
                                int d = BarPixels/2 - 1;
                                int x1 = x + d;
                                int x2 = x + BarPixels - 2;
                                g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                                g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                                g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                            }
                        }
                    }
                    else if (Backtester.PosDir(bar, iPos) == PosDirection.Closed)
                    {
                        // Close
                        var pen = new Pen(brushTradeClose, 2);

                        if (BarPixels < 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                        }
                        else if (BarPixels == 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + 7, yDeal);
                            g.DrawLine(pen, x + 5, yDeal - 2, x + 5, yDeal + 2);
                        }
                        else if (BarPixels > 8)
                        {
                            int d = BarPixels/2 - 1;
                            int x1 = x + d;
                            int x2 = x + BarPixels - 3;
                            g.DrawLine(pen, x, yDeal, x1, yDeal);
                            g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                            g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                        }
                    }
                }

                // Ambiguous note
                if (ShowAmbiguousBars && Backtester.BackTestEval(bar) == BacktestEval.Ambiguous)
                    g.DrawString("!", Font, brushSignalRed, x + BarPixels/2 - 4, yHigh - 20);
            }

            // Chart title
            g.DrawString(chartTitle, font, brushFore, spcLeft, 0);
        }

        /// <summary>
        ///     Paints the panel PnlInd
        /// </summary>
        private void PnlIndPaint(object sender, PaintEventArgs e)
        {
            if (!ShowIndicators) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            var slot = (int) pnl.Tag;

            int topSpace = font.Height/2 + 2;
            int bottomSpace = font.Height/2;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar - 1, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, Data.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, Data.Strategy.Slot[slot].MaxValue);

            foreach (double value in Data.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value - 0) < 0.00001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace)/(Math.Max(maxValue - minValue, 0.0001));

            // Grid
            String strFormat;
            double labelAbs;
            int xGridRight = pnl.ClientSize.Width - spcRight + 2;

            double label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, xRight, labelYZero - font.Height/2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue; // Bottom line
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, xRight, labelYMin - font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(penGrid, spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }

            label = maxValue; // Top line
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, xRight, labelYMax - font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(penGrid, spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }

            if (Data.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < Data.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = Data.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        var labelY = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
                        if (Math.Abs(labelY - labelYZero) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMin) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMax) < font.Height) continue;
                        labelAbs = Math.Abs(label);
                        strFormat = labelAbs < 10
                                        ? "F4"
                                        : labelAbs < 100
                                              ? "F3"
                                              : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(strFormat), font, brushFore, xRight, labelY - font.Height/2 - 1);
                        if (ShowGrid)
                            g.DrawLine(penGrid, spcLeft, labelY, xGridRight, labelY);
                        else
                            g.DrawLine(penGrid, xGridRight - 5, labelY, xGridRight, labelY);
                    }
                }

            // Vertical line
            if (ShowGrid)
            {
                string date = Data.Time[firstBar].ToString("dd.MM") + " " + Data.Time[firstBar].ToString("HH:mm");
                var dateWidth = (int) g.MeasureString(date, font).Width;
                for (int lineBar = lastBar;
                     lineBar > firstBar;
                     lineBar -= (int) Math.Round((dateWidth + 10.0)/BarPixels + 1))
                {
                    int xLine = (lineBar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                    g.DrawLine(penGrid, xLine, topSpace, xLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int indicatorValueShift = 1;
            foreach (ListParam listParam in Data.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
            {
                if (component.ChartType == IndChartType.Histogram)
                {
                    // Histogram
                    double zero = 0;
                    if (zero < minValue) zero = minValue;
                    if (zero > maxValue) zero = maxValue;
                    var y0 = (int) Math.Round(pnl.ClientSize.Height - 5 - (zero - minValue)*scale);

                    var penGreen = new Pen(LayoutColors.ColorTradeLong);
                    var penRed = new Pen(LayoutColors.ColorTradeShort);

                    bool isPrevBarGreen = false;

                    if (TrueCharts)
                    {
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = spcLeft + (bar - firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penGreen, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penRed, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penRed, x, y0 - 2, x, y);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*BarPixels + BarPixels - 2;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    isPrevBarGreen = true;
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    isPrevBarGreen = false;
                                }
                            }
                        }
                        else
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penGreen, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penRed, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penRed, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - firstBar)*BarPixels + spcLeft;
                            var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                            LinearGradientBrush lgBrush;
                            Rectangle rect;
                            if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = true;
                            }
                            else
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = false;
                            }
                        }
                    }
                }

                if (component.ChartType == IndChartType.Line)
                {
                    // Line
                    var pen = new Pen(component.ChartColor);
                    var penTc = new Pen(component.ChartColor)
                        {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};
                    int yIndChart = pnl.ClientSize.Height - 7;

                    if (TrueCharts)
                    {
                        // True Charts
                        var point = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = spcLeft + (bar - firstBar + indicatorValueShift)*BarPixels - 2*indicatorValueShift;
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                            point[bar - firstBar] = new Point(x, y);
                        }

                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            // All bars except the last one
                            int i = bar - firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == firstBar && isIndicatorValueAtClose)
                            {
                                // First bar
                                double value = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*BarPixels;
                                var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (BarPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 5, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 4, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 2, y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTc, x + BarPixels - 2, y + 2, x + BarPixels - 2, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTc, x + BarPixels - 2, y - 2, x + BarPixels - 2, point[i].Y + 2);
                                }
                            }

                            if (bar < lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (BarPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                    else
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i + 1].Y > point[i].Y)
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                   point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                   point[i + 1].Y + 2);
                                }
                            }

                            if (bar == lastBar && !isIndicatorValueAtClose && BarPixels > 3)
                            {
                                // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + BarPixels - 2, point[i].Y);
                            }
                        }
                    }
                    else
                    {
                        // Regular Charts
                        var aPoint = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = (bar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);
                            aPoint[bar - firstBar] = new Point(x, y);
                        }
                        g.DrawLines(pen, aPoint);
                    }
                }
            }

            // Vertical cross line
            if (ShowCross && mouseX > xLeft - 1 && mouseX < xRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);


            // Chart title
            Indicator indicator = IndicatorManager.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName);
            indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
            string indicatorText = indicator.ToString();
            Size sizeTitle = g.MeasureString(indicatorText, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(indicatorText, Font, brushFore, spcLeft, 0);
        }

        /// <summary>
        ///     Paints the panel FloatingPL
        /// </summary>
        private void PnlFloatingPLPaint(object sender, PaintEventArgs e)
        {
            if (!ShowFloatingPL) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            int topSpace = font.Height/2 + 2;
            int bottomSpace = font.Height/2;
            int maxValue = 10;
            int minValue = -10;
            int value;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            for (int bar = Math.Max(firstBar, Data.FirstBar); bar <= lastBar; bar++)
            {
                if (!Backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney
                            ? (int) Math.Round(Backtester.MoneyProfitLoss(bar) + Backtester.MoneyFloatingPL(bar))
                            : Backtester.ProfitLoss(bar) + Backtester.FloatingPL(bar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            minValue = 10*(int) Math.Floor(minValue/10.0);
            maxValue = 10*(int) Math.Ceiling(maxValue/10.0);

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace)/((double) Math.Max(maxValue - minValue, 1));

            // Grid
            int xGridRight = pnl.ClientSize.Width - spcRight + 2;
            int label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelYZero - Font.Height/2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue;
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= Font.Height)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelYMin - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(penGrid, spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }
            label = maxValue;
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= Font.Height)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelYMax - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(penGrid, spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }

            // Vertical line
            if (ShowGrid)
            {
                string date = Data.Time[firstBar].ToString("dd.MM") + " " + Data.Time[firstBar].ToString("HH:mm");
                var isDataWidth = (int) g.MeasureString(date, Font).Width;
                for (int lineBar = lastBar;
                     lineBar > firstBar;
                     lineBar -= (int) Math.Round((isDataWidth + 10.0)/BarPixels + 1))
                {
                    int xLine = (lineBar - firstBar)*BarPixels + BarPixels/2 - 1 + spcLeft;
                    g.DrawLine(penGrid, xLine, topSpace, xLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            // Chart
            var y0 = (int) Math.Round(pnl.ClientSize.Height - 5 + minValue*scale);
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (!Backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney
                            ? (int) Math.Round(Backtester.MoneyProfitLoss(bar) + Backtester.MoneyFloatingPL(bar))
                            : Backtester.ProfitLoss(bar) + Backtester.FloatingPL(bar);
                int x = (bar - firstBar)*BarPixels + spcLeft;
                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                if (y == y0) continue;
                Rectangle rect;
                LinearGradientBrush lgBrush;
                if (Backtester.SummaryDir(bar) == PosDirection.Long)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Vertical cross line
            if (ShowCross && mouseX > xLeft - 1 && mouseX < xRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);


            // Chart title
            string sTitle = Language.T("Floating P/L") + " [" +
                            (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, brushFore, spcLeft, 0);
        }

        /// <summary>
        ///     Paints the panel PnlBalance
        /// </summary>
        private void PnlBalancePaint(object sender, PaintEventArgs e)
        {
            if (!ShowBalanceEquity) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            int topSpace = Font.Height/2 + 2;
            int bottomSpace = Font.Height/2;
            int top = topSpace;
            int bottom = pnl.ClientSize.Height - bottomSpace;

            // Min and Max values
            int maxValue = int.MinValue;
            int minValue = int.MaxValue;
            int value;
            for (int iBar = Math.Max(firstBar, Data.FirstBar); iBar <= lastBar; iBar++)
            {
                value = Configs.AccountInMoney ? (int) Backtester.MoneyBalance(iBar) : Backtester.Balance(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
                value = Configs.AccountInMoney ? (int) Backtester.MoneyEquity(iBar) : Backtester.Equity(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            if (maxValue == 0 && minValue == 0)
            {
                maxValue = 10;
                minValue = -10;
            }

            if (maxValue == minValue)
            {
                maxValue += 10;
                minValue -= 10;
            }

            int countLabels = Math.Max((bottom - top)/30, 1);
            int deltaLabels = 10*((Math.Max((maxValue - minValue)/countLabels, 10))/10);

            minValue = 10*(int) Math.Floor(minValue/10.0);
            countLabels = (int) Math.Ceiling((maxValue - minValue)/(double) deltaLabels);
            maxValue = minValue + countLabels*deltaLabels;

            double scale = (bottom - top)/((double) countLabels*deltaLabels);

            // Grid
            for (int label = minValue; label <= maxValue; label += deltaLabels)
            {
                var yLabel = (int) Math.Round(bottom - (label - minValue)*scale);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             yLabel - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(penGrid, xLeft, yLabel, xRight, yLabel);
                else
                    g.DrawLine(penGrid, xRight - 5, yLabel, xRight, yLabel);
            }

            // Vertical grid lines
            if (ShowGrid)
            {
                for (int lineBar = lastBar;
                     lineBar > firstBar;
                     lineBar -= (int) Math.Round((szDate.Width + 10.0)/BarPixels + 1))
                {
                    int xLine = (lineBar - firstBar)*BarPixels + xLeft + BarPixels/2 - 1;
                    g.DrawLine(penGrid, xLine, top, xLine, bottom);
                }
            }

            // Chart
            var pointsBalance = new Point[lastBar - firstBar + 1];
            var pointsEquity = new Point[lastBar - firstBar + 1];
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                value = Configs.AccountInMoney ? (int) Backtester.MoneyBalance(bar) : Backtester.Balance(bar);
                int x = (bar - firstBar)*BarPixels + BarPixels/2 - 1 + xLeft;
                var y = (int) Math.Round(bottom - (value - minValue)*scale);
                pointsBalance[bar - firstBar] = new Point(x, y);
                value = Configs.AccountInMoney ? (int) Backtester.MoneyEquity(bar) : Backtester.Equity(bar);
                y = (int) Math.Round(bottom - (value - minValue)*scale);
                pointsEquity[bar - firstBar] = new Point(x, y);
            }
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), pointsEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), pointsBalance);

            // Vertical cross line
            if (ShowCross && mouseX > xLeft - 1 && mouseX < xRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);

            // Chart title
            string sTitle = Language.T("Balance") + " / " + Language.T("Equity") +
                            " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, brushFore, spcLeft, 0);
        }

        /// <summary>
        ///     Invalidates the panels
        /// </summary>
        private void InvalidateAllPanels()
        {
            PnlPrice.Invalidate();

            if (ShowIndicators)
                foreach (Panel panel in PnlInd)
                    panel.Invalidate();

            if (ShowFloatingPL)
                PnlFloatingPLChart.Invalidate();

            if (ShowBalanceEquity)
                PnlBalanceChart.Invalidate();
        }

        /// <summary>
        ///     Sets the width of the info panel
        /// </summary>
        private void SetupDynInfoWidth()
        {
            asInfoTitle = new string[200];
            aiInfoType = new int[200];
            infoRows = 0;

            string sUnit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = Language.T("Balance") + sUnit;
            asInfoTitle[infoRows++] = Language.T("Equity") + sUnit;
            asInfoTitle[infoRows++] = Language.T("Profit Loss") + sUnit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + sUnit;

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = Data.Strategy.Slot[iSlot].IndicatorName +
                                          (Data.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Number of open lots");
            asInfoTitle[infoRows++] = Language.T("Type of the transaction");
            asInfoTitle[infoRows++] = Language.T("Forming order number");
            asInfoTitle[infoRows++] = Language.T("Forming order price");
            asInfoTitle[infoRows++] = Language.T("Corrected position price");
            asInfoTitle[infoRows++] = Language.T("Profit Loss") + sUnit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + sUnit;

            Graphics g = CreateGraphics();

            int maxLength = 0;
            foreach (string str in asInfoTitle)
            {
                var length = (int) g.MeasureString(str, fontDi).Width;
                if (maxLength < length) maxLength = length;
            }

            xDynInfoCol2 = maxLength + 10;
            int iMaxInfoWidth = Configs.AccountInMoney
                                    ? (int)
                                      Math.Max(
                                          g.MeasureString(Backtester.MinMoneyEquity.ToString("F2"), fontDi).Width,
                                          g.MeasureString(Backtester.MaxMoneyEquity.ToString("F2"), fontDi).Width)
                                    : (int)
                                      Math.Max(
                                          g.MeasureString(Backtester.MinEquity.ToString(CultureInfo.InvariantCulture),
                                                          fontDi).Width,
                                          g.MeasureString(Backtester.MaxEquity.ToString(CultureInfo.InvariantCulture),
                                                          fontDi).Width);
            iMaxInfoWidth = (int) Math.Max(g.MeasureString("99/99/99", fontDi).Width, iMaxInfoWidth);

            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), fontDi).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int) g.MeasureString(Language.T(posDir.ToString()), fontDi).Width;

            foreach (Transaction transaction in Enum.GetValues(typeof (Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), fontDi).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int) g.MeasureString(Language.T(transaction.ToString()), fontDi).Width;

            g.Dispose();

            dynInfoWidth = xDynInfoCol2 + iMaxInfoWidth + (isDebug ? 40 : 5);

            PnlInfo.ClientSize = new Size(dynInfoWidth, PnlInfo.ClientSize.Height);
            isDrawDinInfo = false;
        }

        /// <summary>
        ///     Sets the dynamic info panel
        /// </summary>
        private void SetupDynamicInfo()
        {
            asInfoTitle = new string[200];
            aiInfoType = new int[200];
            infoRows = 0;

            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Balance") + unit;
            asInfoTitle[infoRows++] = Language.T("Equity") + unit;
            asInfoTitle[infoRows++] = Language.T("Profit Loss") + unit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + unit;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) compToShow++;
                if (compToShow == 0) continue;

                asInfoTitle[infoRows++] = "";
                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = Data.Strategy.Slot[slot].IndicatorName +
                                          (Data.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            for (int pos = 0; pos < posCount; pos++)
            {
                asInfoTitle[infoRows++] = "";
                asInfoTitle[infoRows++] = Language.T("Position direction");
                asInfoTitle[infoRows++] = Language.T("Number of open lots");
                asInfoTitle[infoRows++] = Language.T("Type of the transaction");
                asInfoTitle[infoRows++] = Language.T("Forming order number");
                asInfoTitle[infoRows++] = Language.T("Forming order price");
                asInfoTitle[infoRows++] = Language.T("Corrected position price");
                asInfoTitle[infoRows++] = Language.T("Profit Loss") + unit;
                asInfoTitle[infoRows++] = Language.T("Floating P/L") + unit;
            }

            isDrawDinInfo = false;
        }

        /// <summary>
        ///     Generates the DynamicInfo.
        /// </summary>
        private void GenerateDynamicInfo(int barNumb)
        {
            if (!ShowDynInfo || !ShowInfoPanel) return;

            barNumb = Math.Max(0, barNumb);
            barNumb = Math.Min(chartBars - 1, barNumb);

            int bar = firstBar + barNumb;
            bar = Math.Min(Data.Bars - 1, bar);

            if (barOld == bar) return;
            barOld = bar;

            int row = 0;
            asInfoValue = new String[200];
            asInfoValue[row++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
            asInfoValue[row++] = Data.Time[bar].ToString(Data.Df);
            asInfoValue[row++] = Data.Time[bar].ToString("HH:mm");
            if (isDebug)
            {
                asInfoValue[row++] = Data.Open[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = Data.High[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = Data.Low[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = Data.Close[bar].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                asInfoValue[row++] = Data.Open[bar].ToString(Data.Ff);
                asInfoValue[row++] = Data.High[bar].ToString(Data.Ff);
                asInfoValue[row++] = Data.Low[bar].ToString(Data.Ff);
                asInfoValue[row++] = Data.Close[bar].ToString(Data.Ff);
            }
            asInfoValue[row++] = Data.Volume[bar].ToString(CultureInfo.InvariantCulture);

            asInfoValue[row++] = "";
            if (Configs.AccountInMoney)
            {
                // Balance
                asInfoValue[row++] = Backtester.MoneyBalance(bar).ToString("F2");

                // Equity
                asInfoValue[row++] = Backtester.MoneyEquity(bar).ToString("F2");

                // Profit Loss
                if (Backtester.SummaryTrans(bar) == Transaction.Close ||
                    Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    Backtester.SummaryTrans(bar) == Transaction.Reverse)
                    asInfoValue[row++] = Backtester.MoneyProfitLoss(bar).ToString("F2");
                else
                    asInfoValue[row++] = "   -";

                // Floating P/L
                if (Backtester.Positions(bar) > 0 && Backtester.SummaryTrans(bar) != Transaction.Close)
                    asInfoValue[row++] = Backtester.MoneyFloatingPL(bar).ToString("F2");
                else
                    asInfoValue[row++] = "   -";
            }
            else
            {
                // Balance
                asInfoValue[row++] = Backtester.Balance(bar).ToString(CultureInfo.InvariantCulture);

                // Equity
                asInfoValue[row++] = Backtester.Equity(bar).ToString(CultureInfo.InvariantCulture);

                // Profit Loss
                if (Backtester.SummaryTrans(bar) == Transaction.Close ||
                    Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    Backtester.SummaryTrans(bar) == Transaction.Reverse)
                    asInfoValue[row++] = Backtester.ProfitLoss(bar).ToString(CultureInfo.InvariantCulture);
                else
                    asInfoValue[row++] = "   -";

                // Profit Loss
                if (Backtester.Positions(bar) > 0 && Backtester.SummaryTrans(bar) != Transaction.Close)
                    asInfoValue[row++] = Backtester.FloatingPL(bar).ToString(CultureInfo.InvariantCulture);
                else
                    asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    asInfoValue[row++] = "";
                    asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataType = indComp.DataType;
                            if (indDataType == IndComponentType.AllowOpenLong ||
                                indDataType == IndComponentType.AllowOpenShort ||
                                indDataType == IndComponentType.ForceClose ||
                                indDataType == IndComponentType.ForceCloseLong ||
                                indDataType == IndComponentType.ForceCloseShort)
                                asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (isDebug)
                                {
                                    asInfoValue[row++] = indComp.Value[bar].ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFr = dl < 10
                                                     ? "F5"
                                                     : dl < 100
                                                           ? "F4"
                                                           : dl < 1000
                                                                 ? "F3"
                                                                 : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (Math.Abs(indComp.Value[bar] - 0) > 0.000001)
                                        asInfoValue[row++] = indComp.Value[bar].ToString(sFr);
                                    else
                                        asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            // Positions
            int pos;
            for (pos = 0; pos < Backtester.Positions(bar); pos++)
            {
                asInfoValue[row++] = "";
                asInfoValue[row++] = Language.T(Backtester.PosDir(bar, pos).ToString());
                asInfoValue[row++] = Backtester.PosLots(bar, pos).ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = Language.T(Backtester.PosTransaction(bar, pos).ToString());
                asInfoValue[row++] = Backtester.PosOrdNumb(bar, pos).ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = Backtester.PosOrdPrice(bar, pos).ToString(Data.Ff);
                asInfoValue[row++] = Backtester.PosPrice(bar, pos).ToString(Data.Ff);

                // Profit Loss
                if (Backtester.PosTransaction(bar, pos) == Transaction.Close ||
                    Backtester.PosTransaction(bar, pos) == Transaction.Reduce ||
                    Backtester.PosTransaction(bar, pos) == Transaction.Reverse)
                    asInfoValue[row++] = Configs.AccountInMoney
                                             ? Backtester.PosMoneyProfitLoss(bar, pos).ToString("F2")
                                             : Math.Round(Backtester.PosProfitLoss(bar, pos)).ToString(
                                                 CultureInfo.InvariantCulture);
                else
                    asInfoValue[row++] = "   -";

                // Floating P/L
                if (pos == Backtester.Positions(bar) - 1 && Backtester.PosTransaction(bar, pos) != Transaction.Close)
                    asInfoValue[row++] = Configs.AccountInMoney
                                             ? Backtester.PosMoneyFloatingPL(bar, pos).ToString("F2")
                                             : Math.Round(Backtester.PosFloatingPL(bar, pos)).ToString(
                                                 CultureInfo.InvariantCulture);
                else
                    asInfoValue[row++] = "   -";
            }

            if (posCount != pos)
            {
                posCount = pos;
                SetupDynamicInfo();
                isDrawDinInfo = true;
                PnlInfo.Invalidate();
            }
            else
            {
                PnlInfo.Invalidate(new Rectangle(xDynInfoCol2, 0, dynInfoWidth - xDynInfoCol2,
                                                 PnlInfo.ClientSize.Height));
            }
        }

        /// <summary>
        ///     PnlInfo Resize
        /// </summary>
        private void PnlInfoResize(object sender, EventArgs e)
        {
            PnlInfo.Invalidate();
        }

        /// <summary>
        ///     Paints the panel PnlInfo.
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            if (!ShowInfoPanel) return;

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (isDrawDinInfo && ShowDynInfo)
            {
                int rowHeight = fontDi.Height + 1;
                var size = new Size(dynInfoWidth, rowHeight);

                for (int i = 0; i < infoRows; i++)
                {
                    if (Math.Abs(i%2f - 0) > 0.00001)
                        g.FillRectangle(brushEvenRows, new Rectangle(new Point(0, i*rowHeight + 1), size));

                    if (aiInfoType[i + dynInfoScrollValue] == 1)
                        g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDiInd, brushDiIndicator, 5,
                                     i*rowHeight - 1);
                    else
                        g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDi, brushDynamicInfo, 5,
                                     i*rowHeight + 1);

                    g.DrawString(asInfoValue[i + dynInfoScrollValue], fontDi, brushDynamicInfo, xDynInfoCol2,
                                 i*rowHeight + 1);
                }
            }
        }

        /// <summary>
        ///     Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        private void PnlPriceMouseMove(object sender, MouseEventArgs e)
        {
            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (mouseX > xRight)
                {
                    if (mouseY > mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();
                    return;
                }
                int newScrollValue = scroll.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int) Math.Round(scroll.SmallChange*0.1*(100 - BarPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int) Math.Round(scroll.SmallChange*0.1*(100 - BarPixels));

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (mouseXOld >= xLeft && mouseXOld <= xRight)
                {
                    if (mouseYOld >= yTop && mouseYOld <= yBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, mouseYOld, PnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(xRight - 1, mouseYOld - font.Height/2 - 1, szPrice.Width + 2,
                                                        font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1,
                                                    szDateL.Width + 2,
                                                    font.Height + 3));
                }

                // Adding the new positions
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    if (mouseYOld >= yTop && mouseYOld <= yBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, mouseY, PnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(xRight - 1, mouseY - font.Height/2 - 1, szPrice.Width + 2,
                                                        font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseX, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseX - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                    font.Height + 3));
                }
                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && ShowIndicators; i++)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                    PnlInd[i].Invalidate(new Region(path1));
                }

                if (ShowBalanceEquity)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlBalanceChart.ClientSize.Height));
                    PnlBalanceChart.Invalidate(new Region(path1));
                }

                if (ShowFloatingPL)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    PnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    // Moving inside the chart
                    isMouseInPriceChart = true;
                    isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - xLeft)/BarPixels);
                }
                else
                {
                    // Escaping from the bar area of chart
                    isMouseInPriceChart = false;
                    PnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= xLeft && mouseX <= xRight)
            {
                // Entering into the chart
                if (ShowCross)
                    PnlPrice.Cursor = Cursors.Cross;
                isMouseInPriceChart = true;
                isDrawDinInfo = true;
                PnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - xLeft)/BarPixels);
            }
        }

        /// <summary>
        ///     Deletes the cross and Dynamic Info
        /// </summary>
        private void PnlPriceMouseLeave(object sender, EventArgs e)
        {
            PnlPrice.Cursor = Cursors.Default;
            isMouseInPriceChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Horizontal Line
                path.AddRectangle(new Rectangle(0, mouseYOld, PnlPrice.ClientSize.Width, 1));
                // PriceBox
                path.AddRectangle(new Rectangle(xRight - 1, mouseYOld - font.Height/2 - 1, szPrice.Width + 2,
                                                font.Height + 2));
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && ShowIndicators; i++)
                    PnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

                if (ShowBalanceEquity)
                    PnlBalanceChart.Invalidate(new Rectangle(mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));

                if (ShowFloatingPL)
                    PnlFloatingPLChart.Invalidate(new Rectangle(mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
            }
        }

        /// <summary>
        ///     Mouse moves inside a chart
        /// </summary>
        private void SepChartMouseMove(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = scroll.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int) Math.Round(scroll.SmallChange*0.1*(100 - BarPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int) Math.Round(scroll.SmallChange*0.1*(100 - BarPixels));

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (mouseXOld >= xLeft && mouseXOld <= xRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1,
                                                    szDateL.Width + 2,
                                                    font.Height + 3));
                }

                // Adding the new positions
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseX, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseX - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                    font.Height + 3));
                }
                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && ShowIndicators; i++)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                    PnlInd[i].Invalidate(new Region(path1));
                }

                if (ShowBalanceEquity)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlBalanceChart.ClientSize.Height));
                    PnlBalanceChart.Invalidate(new Region(path1));
                }

                if (ShowFloatingPL)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    PnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    // Moving inside the chart
                    isMouseInIndicatorChart = true;
                    isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - xLeft)/BarPixels);
                }
                else
                {
                    // Escaping from the bar area of chart
                    isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= xLeft && mouseX <= xRight)
            {
                // Entering into the chart
                if (ShowCross)
                    panel.Cursor = Cursors.Cross;
                isMouseInIndicatorChart = true;
                isDrawDinInfo = true;
                PnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - xLeft)/BarPixels);
            }
        }

        /// <summary>
        ///     Mouse leaves a chart.
        /// </summary>
        private void SepChartMouseLeave(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = Cursors.Default;

            isMouseInIndicatorChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && ShowIndicators; i++)
                    PnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

                if (ShowBalanceEquity)
                    PnlBalanceChart.Invalidate(new Rectangle(mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));

                if (ShowFloatingPL)
                    PnlFloatingPLChart.Invalidate(new Rectangle(mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
            }
        }

        /// <summary>
        ///     Sets the parameters when scrolling.
        /// </summary>
        private void ScrollValueChanged(object sender, EventArgs e)
        {
            firstBar = scroll.Value;
            lastBar = Math.Min(Data.Bars - 1, firstBar + chartBars - 1);
            lastBar = Math.Max(lastBar, firstBar);

            InvalidateAllPanels();

            // Dynamic Info
            barOld = 0;
            // Sends the shown bar from the chart beginning
            if (isDrawDinInfo && ShowDynInfo)
            {
                int selectedBar = (mouseX - spcLeft)/BarPixels;
                GenerateDynamicInfo(selectedBar);
            }
        }

        /// <summary>
        ///     Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        private void ScrollMouseWheel(object sender, MouseEventArgs e)
        {
            if (isKeyCtrlPressed)
            {
                if (e.Delta > 0)
                    ZoomIn();
                if (e.Delta < 0)
                    ZoomOut();
            }
            else
            {
                int newScrollValue = scroll.Value +
                                     scroll.LargeChange*e.Delta/SystemInformation.MouseWheelScrollLines/120;

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        private void ScrollKeyUp(object sender, KeyEventArgs e)
        {
            isKeyCtrlPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        private void ScrollKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                isKeyCtrlPressed = true;
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            isKeyCtrlPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        ///     Mouse button down on a panel.
        /// </summary>
        private void PanelMouseDown(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            if (panel == PnlPrice && mouseX > xRight)
                panel.Cursor = Cursors.SizeNS;
            else
                panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        ///     Mouse button up on a panel.
        /// </summary>
        private void PanelMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = ShowCross ? Cursors.Cross : Cursors.Default;
        }

        /// <summary>
        ///     Shows the Bar Explorer
        /// </summary>
        private void PnlPriceMouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Determines the shown bar.
            if (mouseXOld >= xLeft && mouseXOld <= xRight && mouseYOld >= yTop && mouseYOld <= yBottom)
            {
                // Moving inside the chart
                if (mouseX >= xLeft && mouseX <= xRight && mouseY >= yTop && mouseY <= yBottom)
                {
                    int selectedBar = (e.X - xLeft)/BarPixels + firstBar;
                    var be = new BarExplorer(selectedBar);
                    be.ShowDialog();
                }
            }
        }

        /// <summary>
        ///     Changes chart's settings after a button click.
        /// </summary>
        private void ButtonChartClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            var button = (ChartButtons) btn.Tag;

            switch (button)
            {
                case ChartButtons.Grid:
                    ShortcutKeyUp(new KeyEventArgs(Keys.G));
                    break;
                case ChartButtons.Cross:
                    ShortcutKeyUp(new KeyEventArgs(Keys.C));
                    break;
                case ChartButtons.Volume:
                    ShortcutKeyUp(new KeyEventArgs(Keys.V));
                    break;
                case ChartButtons.Orders:
                    ShortcutKeyUp(new KeyEventArgs(Keys.O));
                    break;
                case ChartButtons.PositionLots:
                    ShortcutKeyUp(new KeyEventArgs(Keys.L));
                    break;
                case ChartButtons.PositionPrice:
                    ShortcutKeyUp(new KeyEventArgs(Keys.P));
                    break;
                case ChartButtons.Protections:
                    ShortcutKeyUp(new KeyEventArgs(Keys.E));
                    break;
                case ChartButtons.AmbiguousBars:
                    ShortcutKeyUp(new KeyEventArgs(Keys.M));
                    break;
                case ChartButtons.Indicators:
                    ShortcutKeyUp(new KeyEventArgs(Keys.D));
                    break;
                case ChartButtons.BalanceEquity:
                    ShortcutKeyUp(new KeyEventArgs(Keys.B));
                    break;
                case ChartButtons.FloatingPL:
                    ShortcutKeyUp(new KeyEventArgs(Keys.F));
                    break;
                case ChartButtons.ZoomIn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Add));
                    break;
                case ChartButtons.ZoomOut:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Subtract));
                    break;
                case ChartButtons.TrueCharts:
                    ShortcutKeyUp(new KeyEventArgs(Keys.T));
                    break;
                case ChartButtons.DynamicInfo:
                    ShortcutKeyUp(new KeyEventArgs(Keys.I));
                    break;
                case ChartButtons.DInfoDwn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Z));
                    break;
                case ChartButtons.DInfoUp:
                    ShortcutKeyUp(new KeyEventArgs(Keys.A));
                    break;
            }
        }

        /// <summary>
        ///     Shortcut keys
        /// </summary>
        private void ShortcutKeyUp(KeyEventArgs e)
        {
            // Zoom in
            if (!e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                ZoomIn();
            }
            // Zoom out
            else if (!e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                ZoomOut();
            }
            // Vertical scale increase
            else if (e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                VerticalScaleIncrease();
            }
            // Vertical scale decrease
            else if (e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                VerticalScaleDecrease();
            }
            else if (e.KeyCode == Keys.Space)
            {
                isCandleChart = !isCandleChart;
                PnlPrice.Invalidate();
            }
            // Dynamic info scroll up
            else if (e.KeyCode == Keys.A)
            {
                if (!ShowInfoPanel)
                    return;
                dynInfoScrollValue -= 5;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
            // Dynamic info scroll up fast
            else if (e.KeyCode == Keys.S)
            {
                if (!ShowInfoPanel)
                    return;
                dynInfoScrollValue -= 10;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
            // Dynamic info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                if (!ShowInfoPanel)
                    return;
                dynInfoScrollValue += 5;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
            // Dynamic info scroll down fast
            else if (e.KeyCode == Keys.X)
            {
                if (!ShowInfoPanel)
                    return;
                dynInfoScrollValue += 10;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
            // Grid
            else if (e.KeyCode == Keys.G)
            {
                ShowGrid = !ShowGrid;
                AChartButtons[(int) ChartButtons.Grid].Checked = ShowGrid;
                InvalidateAllPanels();
            }
            // Cross
            else if (e.KeyCode == Keys.C)
            {
                ShowCross = !ShowCross;
                AChartButtons[(int) ChartButtons.Cross].Checked = ShowCross;
                PnlPrice.Cursor = ShowCross ? Cursors.Cross : Cursors.Default;
                InvalidateAllPanels();
            }
            // Volume
            else if (e.KeyCode == Keys.V)
            {
                ShowVolume = !ShowVolume;
                AChartButtons[(int) ChartButtons.Volume].Checked = ShowVolume;
                PnlPrice.Invalidate();
            }
            // Lots
            else if (e.KeyCode == Keys.L)
            {
                ShowPositionLots = !ShowPositionLots;
                AChartButtons[(int) ChartButtons.PositionLots].Checked = ShowPositionLots;
                PnlPrice.Invalidate();
            }
            // Orders
            else if (e.KeyCode == Keys.O)
            {
                ShowOrders = !ShowOrders;
                AChartButtons[(int) ChartButtons.Orders].Checked = ShowOrders;
                PnlPrice.Invalidate();
            }
            // Position price
            else if (e.KeyCode == Keys.P)
            {
                ShowPositionPrice = !ShowPositionPrice;
                AChartButtons[(int) ChartButtons.PositionPrice].Checked = ShowPositionPrice;
                PnlPrice.Invalidate();
            }
            // Protections
            else if (e.KeyCode == Keys.E)
            {
                ShowProtections = !ShowProtections;
                AChartButtons[(int) ChartButtons.Protections].Checked = ShowProtections;
                PnlPrice.Invalidate();
            }
            // Ambiguous bars mark
            else if (e.KeyCode == Keys.M)
            {
                ShowAmbiguousBars = !ShowAmbiguousBars;
                AChartButtons[(int) ChartButtons.AmbiguousBars].Checked = ShowAmbiguousBars;
                PnlPrice.Invalidate();
            }
            // True Charts
            else if (e.KeyCode == Keys.T)
            {
                TrueCharts = !TrueCharts;
                AChartButtons[(int) ChartButtons.TrueCharts].Checked = TrueCharts;
                InvalidateAllPanels();
            }
            // Indicator Charts
            else if (e.KeyCode == Keys.D)
            {
                ShowIndicators = !ShowIndicators;
                AChartButtons[(int) ChartButtons.Indicators].Checked = ShowIndicators;
                if (ShowIndicators)
                {
                    scroll.Parent = null;
                    for (int i = 0; i < indPanels; i++)
                    {
                        SplitterInd[i].Parent = PnlCharts;
                        PnlInd[i].Parent = PnlCharts;
                    }
                    scroll.Parent = PnlCharts;
                }
                else
                {
                    for (int i = 0; i < indPanels; i++)
                    {
                        PnlInd[i].Parent = null;
                        SplitterInd[i].Parent = null;
                    }
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // FloatingPL Charts
            else if (e.KeyCode == Keys.F)
            {
                ShowFloatingPL = !ShowFloatingPL;
                AChartButtons[(int) ChartButtons.FloatingPL].Checked = ShowFloatingPL;
                if (ShowFloatingPL)
                {
                    scroll.Parent = null;
                    SpliterFloatingPL.Parent = PnlCharts;
                    PnlFloatingPLChart.Parent = PnlCharts;
                    scroll.Parent = PnlCharts;
                }
                else
                {
                    SpliterFloatingPL.Parent = null;
                    PnlFloatingPLChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // Balance/Equity Charts
            else if (e.KeyCode == Keys.B)
            {
                ShowBalanceEquity = !ShowBalanceEquity;
                AChartButtons[(int) ChartButtons.BalanceEquity].Checked = ShowBalanceEquity;
                if (ShowBalanceEquity)
                {
                    scroll.Parent = null;
                    SpliterBalance.Parent = PnlCharts;
                    PnlBalanceChart.Parent = PnlCharts;
                    scroll.Parent = PnlCharts;
                }
                else
                {
                    SpliterBalance.Parent = null;
                    PnlBalanceChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // Show info panel
            else if (e.KeyCode == Keys.I)
            {
                ShowInfoPanel = !ShowInfoPanel;
                PnlInfo.Visible = ShowInfoPanel;
                PnlCharts.Padding = ShowInfoPanel ? new Padding(0, 0, 2, 0) : new Padding(0);
                Text = ShowInfoPanel
                           ? Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString
                           : Data.ProgramName + @"   http://forexsb.com";
                AChartButtons[(int) ChartButtons.DynamicInfo].Checked = ShowInfoPanel;
                AChartButtons[(int) ChartButtons.DInfoUp].Visible = ShowInfoPanel;
                AChartButtons[(int) ChartButtons.DInfoDwn].Visible = ShowInfoPanel;
                if (ShowInfoPanel)
                {
                    GenerateDynamicInfo(lastBar);
                    SetupDynamicInfo();
                    isDrawDinInfo = true;
                    PnlInfo.Invalidate();
                }
            }
            // Debug
            else if (e.KeyCode == Keys.F12)
            {
                isDebug = !isDebug;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                PnlInfo.Invalidate();
            }
        }

        /// <summary>
        ///     Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleDecrease()
        {
            if (verticalScale <= 10) return;
            verticalScale -= 10;
            PnlPrice.Invalidate();
        }

        /// <summary>
        ///     Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleIncrease()
        {
            if (verticalScale >= 300) return;
            verticalScale += 10;
            PnlPrice.Invalidate();
        }

        /// <summary>
        ///     Zooms the chart in.
        /// </summary>
        private void ZoomIn()
        {
            BarPixels += 4;
            if (BarPixels == 6)
                BarPixels = 4;
            if (BarPixels > 40)
                BarPixels = 40;

            int oldChartBars = chartBars;

            chartBars = chartWidth/BarPixels;
            if (chartBars > Data.Bars - Data.FirstBar)
                chartBars = Data.Bars - Data.FirstBar;

            if (lastBar < Data.Bars - 1)
            {
                firstBar += (oldChartBars - chartBars)/2;
                if (firstBar > Data.Bars - chartBars)
                    firstBar = Data.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            InvalidateAllPanels();
        }

        /// <summary>
        ///     Zooms the chart out.
        /// </summary>
        private void ZoomOut()
        {
            BarPixels -= 4;
            if (BarPixels < 4)
                BarPixels = 2;

            int oldChartBars = chartBars;

            chartBars = chartWidth/BarPixels;
            if (chartBars > Data.Bars - Data.FirstBar)
                chartBars = Data.Bars - Data.FirstBar;

            if (lastBar < Data.Bars - 1)
            {
                firstBar -= (chartBars - oldChartBars)/2;
                if (firstBar < Data.FirstBar)
                    firstBar = Data.FirstBar;

                if (firstBar > Data.Bars - chartBars)
                    firstBar = Data.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            InvalidateAllPanels();
        }
    }
}