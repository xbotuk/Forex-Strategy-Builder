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
using System.Windows.Forms;
using ForexStrategyBuilder.Common;
using ForexStrategyBuilder.CustomControls;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Draws a small indicator chart
    /// </summary>
    public class SmallIndicatorChart : ContextPanel
    {
        private const int Space = 5;
        private const int Border = 2;
        private readonly Pen penDarkGray = new Pen(Color.DarkGray);
        private readonly Pen penGreen = new Pen(Color.Green);
        private readonly Pen penRed = new Pen(Color.Red);
        private readonly HScrollBar scrollBar;
        private int[] axisX;
        private Brush[] brushPosition;
        private Brush captionBrush;

        private Font captionFont;
        private float captionHeight;
        private RectangleF captionRectangle;
        private StringFormat captionStringFormat;
        private string captionText;
        private float captionWidth;

        private int chartBarWidth;
        private int chartBars;
        private Brush[][] chartBrush;
        private Rectangle[][][] chartDot;
        private int chartFirstBar;
        private int chartLastBar;
        private Rectangle[][][] chartLevel;
        private Point[][][] chartLine;
        private Pen[][][] chartPen;
        private IndChartType[][] chartType;
        private double[][][] chartValue;
        private int chartWidth;
        private int[] componentLength;
        private int[] indicatorSlots;
        private bool[] isSeparatedChart;
        private bool isShowDynamicInfo;
        private bool isValueChangedActive;

        private double maxPrice;
        private double[] maxValues;
        private int maxVolume;
        private double minPrice;
        private double[] minValues;
        private Pen penBorder;
        private Pen penFore;
        private Pen penVolume;
        private Rectangle[] rectPosition;
        private double scaleY;
        private double scaleYVol;
        private double[] scales;
        private int separateIndicatorsChartHeight;
        private int separateIndicatorsCount;

        private int xLeft;
        private int xRight;
        private int yBottom;
        private int[] yClose;
        private int[] yHigh;
        private int[] yIndBottom;
        private int[] yIndTop;
        private int[] yLow;
        private int[] yOpen;
        private int yPriceBottom;
        private int yTop;
        private int[] yVolume;

        /// <summary>
        ///     Public constructor
        /// </summary>
        public SmallIndicatorChart()
        {
            Padding = new Padding(Border, 0, Border, Border);

            // Horizontal scroll bar
            scrollBar = new HScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Bottom,
                    SmallChange = 1,
                    LargeChange = 50,
                    Minimum = 0,
                    Maximum = 1000,
                    Visible = true
                };
            scrollBar.ValueChanged += ScrollBarValueChanged;
        }

        /// <summary>
        ///     Gets or sets whether to show dynamic info or not
        /// </summary>
        public bool ShowDynamicInfo
        {
            set { isShowDynamicInfo = value; }
        }

        /// <summary>
        ///     Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get; private set; }


        /// <summary>
        ///     Sets the parameters of the Indicators Chart
        /// </summary>
        public void InitChart()
        {
            if (!Data.IsData || !Data.IsResult) return;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            chartBarWidth = 2;
            xLeft = Space;
            xRight = ClientSize.Width - Space;
            chartWidth = xRight - xLeft;

            chartBars = chartWidth/chartBarWidth;
            chartBars = Math.Min(chartBars, Data.Bars - StatsBuffer.FirstBar);

            isValueChangedActive = false;
            scrollBar.Minimum = Math.Max(StatsBuffer.FirstBar, 0);
            scrollBar.Maximum = Math.Max(Data.Bars - 1, 1);
            scrollBar.LargeChange = Math.Max(chartBars, 1);

            chartFirstBar = Math.Max(StatsBuffer.FirstBar, Data.Bars - chartBars);
            chartFirstBar = Math.Min(chartFirstBar, Data.Bars - 1);
            chartFirstBar = Math.Max(chartFirstBar, 1);
            chartLastBar = Math.Max(chartFirstBar + chartBars - 1, chartFirstBar);

            scrollBar.Value = chartFirstBar;
            isValueChangedActive = true;

            SetUpPaintData();

            // Context button colors.
            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }


        /// <summary>
        ///     Prepare the parameters
        /// </summary>
        private void SetUpPaintData()
        {
            // Panel caption
            captionText = Language.T("Indicator Chart");
            captionFont = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(captionFont.Height, 18);
            captionWidth = ClientSize.Width;
            captionBrush = new SolidBrush(LayoutColors.ColorCaptionText);
            captionRectangle = new RectangleF(0, 0, captionWidth, captionHeight);
            captionStringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

            if (!Data.IsData || !Data.IsResult || Data.Bars <= StatsBuffer.FirstBar) return;

            xLeft = Space;
            xRight = ClientSize.Width - Space;
            yTop = (int) captionHeight + Space;
            yBottom = ClientSize.Height - scrollBar.Height - Space;
            yPriceBottom = yBottom;
            separateIndicatorsCount = 0;
            separateIndicatorsChartHeight = 0;
            indicatorSlots = new int[Configs.MaxEntryFilters + Configs.MaxExitFilters + 2];

            penFore = new Pen(LayoutColors.ColorChartFore);
            penVolume = new Pen(LayoutColors.ColorVolume);
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                Border);

            for (int slot = StatsBuffer.Strategy.Slots - 1; slot >= 0; slot--)
                if (StatsBuffer.Strategy.Slot[slot].SeparatedChart)
                    indicatorSlots[separateIndicatorsCount++] = slot;

            if (separateIndicatorsCount > 0)
            {
                separateIndicatorsChartHeight = (yBottom - yTop)/(2 + separateIndicatorsCount);
                yPriceBottom = yBottom - separateIndicatorsCount*separateIndicatorsChartHeight;
            }

            maxPrice = double.MinValue;
            minPrice = double.MaxValue;
            maxVolume = int.MinValue;

            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
            {
                if (Data.High[bar] > maxPrice) maxPrice = Data.High[bar];
                if (Data.Low[bar] < minPrice) minPrice = Data.Low[bar];
                if (Data.Volume[bar] > maxVolume) maxVolume = Data.Volume[bar];
            }
            minPrice = Math.Round(minPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) -
                       Data.InstrProperties.Point*10;
            maxPrice = Math.Round(maxPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) +
                       Data.InstrProperties.Point*10;
            scaleY = (yPriceBottom - yTop)/(maxPrice - minPrice);
            scaleYVol = maxVolume > 0 ? ((yPriceBottom - yTop)/8d)/maxVolume : 0d;

            // Volume, Lots and Price
            axisX = new int[chartBars];
            yOpen = new int[chartBars];
            yHigh = new int[chartBars];
            yLow = new int[chartBars];
            yClose = new int[chartBars];
            yVolume = new int[chartBars];
            rectPosition = new Rectangle[chartBars];
            brushPosition = new Brush[chartBars];

            int index = 0;
            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
            {
                axisX[index] = (bar - chartFirstBar)*chartBarWidth + xLeft;
                yOpen[index] = (int) (yPriceBottom - (Data.Open[bar] - minPrice)*scaleY);
                yHigh[index] = (int) (yPriceBottom - (Data.High[bar] - minPrice)*scaleY);
                yLow[index] = (int) (yPriceBottom - (Data.Low[bar] - minPrice)*scaleY);
                yClose[index] = (int) (yPriceBottom - (Data.Close[bar] - minPrice)*scaleY);
                yVolume[index] = (int) (yPriceBottom - Data.Volume[bar]*scaleYVol);

                // Draw position lots
                if (StatsBuffer.IsPos(bar))
                {
                    var posHeight = (int) (Math.Max(StatsBuffer.SummaryLots(bar)*2, 2));
                    int yPos = yPriceBottom - posHeight;

                    switch (StatsBuffer.SummaryDir(bar))
                    {
                        case PosDirection.Long:
                            rectPosition[index] = new Rectangle(axisX[index], yPos, 1, posHeight);
                            brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeLong);
                            break;
                        case PosDirection.Short:
                            rectPosition[index] = new Rectangle(axisX[index], yPos, 1, posHeight);
                            brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeShort);
                            break;
                        case PosDirection.Closed:
                            rectPosition[index] = new Rectangle(axisX[index], yPos - 2, 1, 2);
                            brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeClose);
                            break;
                    }
                }
                else
                {
                    // There is no position
                    rectPosition[index] = Rectangle.Empty;
                    brushPosition[index] = new SolidBrush(LayoutColors.ColorChartBack);
                }
                index++;
            }

            // Indicators in the chart
            int slots = StatsBuffer.Strategy.Slots;
            isSeparatedChart = new bool[slots];
            componentLength = new int[slots];
            chartType = new IndChartType[slots][];
            chartLine = new Point[slots][][];
            chartDot = new Rectangle[slots][][];
            chartLevel = new Rectangle[slots][][];
            chartValue = new double[slots][][];
            chartPen = new Pen[slots][][];
            chartBrush = new Brush[slots][];

            for (int slot = 0; slot < slots; slot++)
            {
                isSeparatedChart[slot] = StatsBuffer.Strategy.Slot[slot].SeparatedChart;
                int count = StatsBuffer.Strategy.Slot[slot].Component.Length;
                componentLength[slot] = count;
                chartType[slot] = new IndChartType[count];
                chartLine[slot] = new Point[count][];
                chartDot[slot] = new Rectangle[count][];
                chartLevel[slot] = new Rectangle[count][];
                chartValue[slot] = new double[count][];
                chartPen[slot] = new Pen[count][];
                chartBrush[slot] = new Brush[count];
            }

            for (int slot = 0; slot < slots; slot++)
            {
                if (isSeparatedChart[slot]) continue;

                for (int comp = 0; comp < componentLength[slot]; comp++)
                {
                    chartType[slot][comp] = StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType;
                    switch (StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType)
                    {
                        case IndChartType.Line:
                        case IndChartType.CloudUp:
                        case IndChartType.CloudDown:
                            chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            chartLine[slot][comp] = new Point[chartLastBar - chartFirstBar + 1];
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - chartFirstBar)*chartBarWidth + xLeft;
                                var y = (int) (yPriceBottom - (value - minPrice)*scaleY);

                                if (Math.Abs(value - 0) < 0.0001)
                                    chartLine[slot][comp][bar - chartFirstBar] =
                                        chartLine[slot][comp][Math.Max(bar - chartFirstBar - 1, 0)];
                                else
                                    chartLine[slot][comp][bar - chartFirstBar] = new Point(x, y);
                            }
                            break;
                        case IndChartType.Dot:
                            chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            chartDot[slot][comp] = new Rectangle[chartLastBar - chartFirstBar + 1];
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - chartFirstBar)*chartBarWidth + xLeft;
                                var y = (int) (yPriceBottom - (value - minPrice)*scaleY);
                                chartDot[slot][comp][bar - chartFirstBar] = new Rectangle(x, y, 1, 1);
                            }
                            break;
                        case IndChartType.Level:
                            chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            chartLevel[slot][comp] = new Rectangle[chartLastBar - chartFirstBar + 1];
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - chartFirstBar)*chartBarWidth + xLeft;
                                var y = (int) (yPriceBottom - (value - minPrice)*scaleY);
                                chartLevel[slot][comp][bar - chartFirstBar] = new Rectangle(x, y, chartBarWidth, 1);
                            }
                            break;
                    }
                }
            }

            // Separate indicators
            yIndTop = new int[separateIndicatorsCount];
            yIndBottom = new int[separateIndicatorsCount];
            maxValues = new double[separateIndicatorsCount];
            minValues = new double[separateIndicatorsCount];
            scales = new double[separateIndicatorsCount];

            for (int ind = 0; ind < separateIndicatorsCount; ind++)
            {
                yIndTop[ind] = yBottom - (ind + 1)*separateIndicatorsChartHeight + 1;
                yIndBottom[ind] = yBottom - ind*separateIndicatorsChartHeight - 1;
                maxValues[ind] = double.MinValue;
                minValues[ind] = double.MaxValue;
                int slot = indicatorSlots[ind];

                for (int comp = 0; comp < componentLength[slot]; comp++)
                    if (StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType != IndChartType.NoChart)
                        for (
                            int bar = Math.Max(chartFirstBar, StatsBuffer.Strategy.Slot[slot].Component[comp].FirstBar);
                            bar <= chartLastBar;
                            bar++)
                        {
                            double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                            if (value > maxValues[ind]) maxValues[ind] = value;
                            if (value < minValues[ind]) minValues[ind] = value;
                        }

                maxValues[ind] = Math.Max(maxValues[ind], StatsBuffer.Strategy.Slot[slot].MaxValue);
                minValues[ind] = Math.Min(minValues[ind], StatsBuffer.Strategy.Slot[slot].MinValue);

                foreach (double specialValue in StatsBuffer.Strategy.Slot[slot].SpecValue)
                    if (Math.Abs(specialValue - 0) < 0.0001)
                    {
                        maxValues[ind] = Math.Max(maxValues[ind], 0);
                        minValues[ind] = Math.Min(minValues[ind], 0);
                    }

                scales[ind] = (yIndBottom[ind] - yIndTop[ind] - 2)/(Math.Max(maxValues[ind] - minValues[ind], 0.0001f));

                // Indicator chart
                for (int comp = 0; comp < StatsBuffer.Strategy.Slot[slot].Component.Length; comp++)
                {
                    chartType[slot][comp] = StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType;
                    switch (chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            chartLine[slot][comp] = new Point[chartLastBar - chartFirstBar + 1];
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - chartFirstBar)*chartBarWidth + xLeft;
                                var y = (int) (yIndBottom[ind] - 1 - (value - minValues[ind])*scales[ind]);
                                chartLine[slot][comp][bar - chartFirstBar] = new Point(x, y);
                            }
                            break;
                        case IndChartType.Histogram:
                            chartValue[slot][comp] = new double[chartLastBar - chartFirstBar + 1];
                            chartPen[slot][comp] = new Pen[chartLastBar - chartFirstBar + 1];
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                chartValue[slot][comp][bar - chartFirstBar] = value;
                                if (value > StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar - 1])
                                    chartPen[slot][comp][bar - chartFirstBar] = penGreen;
                                else
                                    chartPen[slot][comp][bar - chartFirstBar] = penRed;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ClientSize.Width == 0 || ClientSize.Height == 0) return;
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            var chartArea = new Rectangle(Border, (int) captionHeight, ClientSize.Width - 2*Border,
                                          ClientSize.Height - (int) captionHeight - Border);
            Data.GradientPaint(g, chartArea, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Panel caption
            Data.GradientPaint(g, captionRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(captionText, captionFont, captionBrush, captionRectangle, captionStringFormat);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= StatsBuffer.FirstBar) return;

            // Limits the drawing into the chart area only
            g.SetClip(new Rectangle(xLeft, yTop, xRight - xLeft, yPriceBottom - yTop));

            // Draws Volume, Lots and Price
            int index = 0;
            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
            {
                // Draw the volume
                if (yVolume[index] != yPriceBottom)
                    g.DrawLine(penVolume, axisX[index], yVolume[index], axisX[index], yPriceBottom - 1);

                // Draw position lots
                if (rectPosition[index] != Rectangle.Empty)
                    g.FillRectangle(brushPosition[index], rectPosition[index]);

                // Draw the bar
                var penBar = new Pen(LayoutColors.ColorBarBorder);
                g.DrawLine(penBar, axisX[index], yLow[index], axisX[index], yHigh[index]);
                g.DrawLine(penBar, axisX[index], yClose[index], axisX[index] + 1, yClose[index]);
                index++;
            }

            // Drawing the indicators in the chart
            int slots = StatsBuffer.Strategy.Slots;
            for (int slot = 0; slot < slots; slot++)
            {
                if (isSeparatedChart[slot]) continue;
                for (int comp = 0; comp < componentLength[slot]; comp++)
                {
                    switch (chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            g.DrawLines(new Pen(chartBrush[slot][comp]), chartLine[slot][comp]);
                            break;
                        case IndChartType.Dot:
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                                g.FillRectangle(chartBrush[slot][comp], chartDot[slot][comp][bar - chartFirstBar]);
                            break;
                        case IndChartType.Level:
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                                g.FillRectangle(chartBrush[slot][comp], chartLevel[slot][comp][bar - chartFirstBar]);
                            break;
                        case IndChartType.CloudUp:
                            g.DrawLines(new Pen(chartBrush[slot][comp]) {DashStyle = DashStyle.Dash},
                                        chartLine[slot][comp]);
                            break;
                        case IndChartType.CloudDown:
                            g.DrawLines(new Pen(chartBrush[slot][comp]) {DashStyle = DashStyle.Dash},
                                        chartLine[slot][comp]);
                            break;
                    }
                }
            }
            g.ResetClip();

            // Separate indicators
            for (int ind = 0; ind < separateIndicatorsCount; ind++)
            {
                int slot = indicatorSlots[ind];

                for (int comp = 0; comp < componentLength[slot]; comp++)
                {
                    switch (chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            g.DrawLines(new Pen(chartBrush[slot][comp]), chartLine[slot][comp]);
                            break;
                        case IndChartType.Histogram:
                            double zero = 0;
                            if (zero < minValues[ind]) zero = minValues[ind];
                            if (zero > maxValues[ind]) zero = maxValues[ind];
                            var y0 = (int) (yIndBottom[ind] - (zero - minValues[ind])*scales[ind]);
                            g.DrawLine(penDarkGray, xLeft, y0, xRight, y0);
                            for (int bar = chartFirstBar; bar <= chartLastBar; bar++)
                            {
                                double val = chartValue[slot][comp][bar - chartFirstBar];
                                int x = (bar - chartFirstBar)*chartBarWidth + xLeft;
                                var y = (int) (yIndBottom[ind] - (val - minValues[ind])*scales[ind]);
                                g.DrawLine(chartPen[slot][comp][bar - chartFirstBar], x, y0, x, y);
                            }
                            break;
                    }
                }
            }

            // Lines
            for (int ind = 0; ind < separateIndicatorsCount; ind++)
            {
                int y = yBottom - (ind + 1)*separateIndicatorsChartHeight;
                g.DrawLine(penFore, xLeft, y, xRight, y);
            }
            g.DrawLine(penFore, xLeft, yBottom, xRight, yBottom);
            g.DrawLine(penFore, xLeft, yTop, xLeft, yBottom);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Generates dynamic info on the status bar
        ///     when we are Moving the mouse over the Indicator Chart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult || Data.Bars < StatsBuffer.FirstBar) return;

            int currentBar = (e.X - Space)/chartBarWidth;
            currentBar = Math.Max(0, currentBar);
            currentBar = Math.Min(chartBars - 1, currentBar);

            int bar = Math.Min(Data.Bars - 1, chartFirstBar + currentBar);

            CurrentBarInfo = string.Format("{0} {1} O:{2} H:{3} L:{4} C:{5} V:{6}",
                                           Data.Time[bar].ToString(Data.Df),
                                           Data.Time[bar].ToString("HH:mm"),
                                           Data.Open[bar].ToString(Data.Ff),
                                           Data.High[bar].ToString(Data.Ff),
                                           Data.Low[bar].ToString(Data.Ff),
                                           Data.Close[bar].ToString(Data.Ff),
                                           Data.Volume[bar]);
        }

        /// <summary>
        ///     Sets the parameters after the horizontal scrollbar position has been changed.
        /// </summary>
        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            if (!isValueChangedActive) return;

            chartFirstBar = scrollBar.Value;
            chartLastBar = Math.Max(chartFirstBar + chartBars - 1, chartFirstBar);

            SetUpPaintData();
            var chartArea = new Rectangle(xLeft + 1, yTop, xRight - xLeft, yBottom - yTop);
            Invalidate(chartArea);
        }

        /// <summary>
        ///     Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            InitChart();
            base.OnResize(eventargs);
            Invalidate();
        }
    }
}