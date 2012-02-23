// Small Balance Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.User_interface;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Draws a small balance chart
    /// </summary>
    public class Small_Balance_Chart : ContextPanel
    {
        private const int Space = 5;
        private const int Border = 2;

        int    XLeft;
        int    XRight;
        int    YTop;
        int    YBottom;
        float  XScale;
        float  YScale;
        float  YPriceScale;
        bool   showPriceLine;
        bool   isNotPaint = false;

        int   countLabels;
        float delta;
        int   step;

        string strStatusBarText;
        bool   isShowDynamicInfo = false;

        int    bars;
        int    chartBars;
        int    maximum;
        int    minimum;
        int    labelWidth;
        string strChartTitle;
        Font   font;
        float  captionHeight;
        RectangleF   rectfCaption;
        StringFormat stringFormatCaption;
        Brush brushFore;
        Pen   penGrid;
        Pen   penBorder;
        PointF[] apntBalance;
        PointF[] apntEquity;
        PointF[] apntLongBalance;
        PointF[] apntShortBalance;
        PointF[] apntClosePrice;
        float  YBalance;
        float  balance;
        float  XMarginCallBar;
        int    marginCallBar;
        int    firstBar;
        bool   isHideScanningLine;
        bool   isScanPerformed;
        string modellingQuolity;

        double dataMaxPrice;
        double dataMinPrice;

        int[] backtesterBalance;
        int[] backtesterEquity;
        int[] backtesterLongBalance;
        int[] backtesterShortBalance;
        double[] backtesterMoneyBalance;
        double[] backtesterMoneyEquity;
        double[] backtesterLongMoneyBalance;
        double[] backtesterShortMoneyBalance;
        double[] dataClose;

        // Out of Sample
        int   barOOS = Data.Bars - 1;
        bool  isOOS  = false;
        DateTime dataTimeBarOOS;
        float XOOSBar;

        /// <summary>
        /// Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo { set { isShowDynamicInfo = value; } }

        /// <summary>
        /// Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get { return strStatusBarText; } }

        /// <summary>
        /// Set the OOS Bar
        /// </summary>
        public int OOSBar { set { barOOS = value; } }

        /// <summary>
        /// Set the OOS
        /// </summary>
        public bool OOS { set { isOOS = value; } }

        /// <summary>
        /// Sets chart's instrument and back testing data.
        /// </summary>
        public void SetChartData()
        {
            isNotPaint = !Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar;

            if (isNotPaint) return;

            showPriceLine   = Configs.ShowPriceChartOnAccountChart && Backtester.ExecutedOrders > 0;
            isScanPerformed = Backtester.IsScanPerformed;

            firstBar   = Data.FirstBar;
            bars       = Data.Bars;
            chartBars  = Data.Bars - firstBar;

            int maxBalance = Configs.AccountInMoney ? (int)Backtester.MaxMoneyBalance : Backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int)Backtester.MinMoneyBalance : Backtester.MinBalance;
            int maxEquity  = Configs.AccountInMoney ? (int)Backtester.MaxMoneyEquity  : Backtester.MaxEquity;
            int minEquity  = Configs.AccountInMoney ? (int)Backtester.MinMoneyEquity  : Backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance  = Configs.AccountInMoney ? (int)Backtester.MaxLongMoneyBalance  : Backtester.MaxLongBalance;
                int minLongBalance  = Configs.AccountInMoney ? (int)Backtester.MinLongMoneyBalance  : Backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney ? (int)Backtester.MaxShortMoneyBalance : Backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney ? (int)Backtester.MinShortMoneyBalance : Backtester.MinShortBalance;
                int maxLongShortBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLongShortBalance = Math.Min(minLongBalance, minShortBalance);

                maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLongShortBalance) + 1;
                minimum = Math.Min(Math.Min(minBalance, minEquity), minLongShortBalance) - 1;
            }
            else
            {
                maximum = Math.Max(maxBalance, maxEquity) + 1;
                minimum = Math.Min(minBalance, minEquity) - 1;
            }

            minimum = (int)(Math.Floor(minimum / 10f) * 10);

            dataMaxPrice = Data.MaxPrice;
            dataMinPrice = Data.MinPrice;

            if (showPriceLine)
            {
                dataClose = new double[bars];
                Data.Close.CopyTo(dataClose, 0);
            }

            if (Configs.AccountInMoney)
            {
                backtesterMoneyBalance = new double[bars];
                backtesterMoneyEquity  = new double[bars];
            }
            else
            {
                backtesterBalance = new int[bars];
                backtesterEquity  = new int[bars];
            }

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                {
                    backtesterLongMoneyBalance  = new double[bars];
                    backtesterShortMoneyBalance = new double[bars];
                }
                else
                {
                    backtesterLongBalance  = new int[bars];
                    backtesterShortBalance = new int[bars];
                }
            }


            for (var bar = firstBar; bar < bars; bar++)
            {
                if (Configs.AccountInMoney)
                {
                    backtesterMoneyBalance[bar] = Backtester.MoneyBalance(bar);
                    backtesterMoneyEquity[bar]  = Backtester.MoneyEquity(bar);
                }
                else
                {
                    backtesterBalance[bar] = Backtester.Balance(bar);
                    backtesterEquity[bar]  = Backtester.Equity(bar);
                }

                if (Configs.AdditionalStatistics)
                {
                    if (Configs.AccountInMoney)
                    {
                        backtesterLongMoneyBalance[bar]  = Backtester.LongMoneyBalance(bar);
                        backtesterShortMoneyBalance[bar] = Backtester.ShortMoneyBalance(bar);
                    }
                    else
                    {
                        backtesterLongBalance[bar]  = Backtester.LongBalance(bar);
                        backtesterShortBalance[bar] = Backtester.ShortBalance(bar);
                    }
                }
            }

            marginCallBar = Backtester.MarginCallBar;

            if (isOOS && barOOS > firstBar)
            {
                balance = (float)(Configs.AccountInMoney ? Backtester.MoneyBalance(barOOS) : Backtester.Balance(barOOS));
                dataTimeBarOOS = Data.Time[barOOS];
            }
            else
                balance = (float)(Configs.AccountInMoney ? Backtester.NetMoneyBalance : Backtester.NetBalance);
        }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart()
        {
            // Chart Title
            strChartTitle = Language.T("Balance / Equity Chart") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            font          = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(font.Height, 18);
            rectfCaption  = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption = new StringFormat
                                      {
                                          Alignment = StringAlignment.Center,
                                          LineAlignment = StringAlignment.Center,
                                          Trimming = StringTrimming.EllipsisCharacter,
                                          FormatFlags = StringFormatFlags.NoWrap
                                      };

            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid = new Pen(LayoutColors.ColorChartGrid) {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);

            if (isNotPaint) return;

            YTop    = (int)captionHeight + 2 * Space + 1;
            YBottom = ClientSize.Height  - 2 * Space - 1 - Border;

            Graphics  g = CreateGraphics();
            var widthMinimum = g.MeasureString(minimum.ToString(CultureInfo.InvariantCulture), Font).Width;
            var widthMaximum = g.MeasureString(maximum.ToString(CultureInfo.InvariantCulture), Font).Width;
            labelWidth = (int)Math.Max(widthMinimum, widthMaximum);
            labelWidth = Math.Max(labelWidth, 30);
            g.Dispose();

            XLeft  = Border + Space;
            XRight = ClientSize.Width - Border - Space - labelWidth;
            XScale = (XRight - 2 * Space - Border) / (float)chartBars;

            countLabels = Math.Max((YBottom - YTop) / 20, 1);
            delta       = (float)Math.Max(Math.Round((maximum - minimum) / (float)countLabels), 10);
            step        = (int)Math.Ceiling(delta / 10) * 10;
            countLabels = (int)Math.Ceiling((maximum - minimum) / (float)step);
            YScale      = (YBottom - YTop) / (countLabels * (float)step);

            apntBalance = new PointF[chartBars];
            apntEquity  = new PointF[chartBars];

            if (Configs.AdditionalStatistics)
            {
                apntLongBalance  = new PointF[chartBars];
                apntShortBalance = new PointF[chartBars];
            }

            apntClosePrice = new PointF[chartBars];

            // Close Price
            if (showPriceLine)
                YPriceScale = (float)((YBottom - YTop) / (dataMaxPrice - dataMinPrice));

            for (var bar = firstBar; bar < bars; bar++)
            {
                var index = bar - firstBar;
                apntBalance[index].X = XLeft + index * XScale;
                apntEquity[index].X  = XLeft + index * XScale;
                if (Configs.AccountInMoney)
                {
                    apntBalance[index].Y = (float)(YBottom - (backtesterMoneyBalance[bar] - minimum) * YScale);
                    apntEquity[index].Y  = (float)(YBottom - (backtesterMoneyEquity[bar]  - minimum) * YScale);
                }
                else
                {
                    apntBalance[index].Y = YBottom -  (backtesterBalance[bar] - minimum) * YScale;
                    apntEquity[index].Y  = YBottom -  (backtesterEquity[bar]  - minimum) * YScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    apntLongBalance[index].X  = XLeft + index * XScale;
                    apntShortBalance[index].X = XLeft + index * XScale;
                    if (Configs.AccountInMoney)
                    {
                        apntLongBalance[index].Y  = (float)(YBottom - (backtesterLongMoneyBalance[bar]  - minimum) * YScale);
                        apntShortBalance[index].Y = (float)(YBottom - (backtesterShortMoneyBalance[bar] - minimum) * YScale);
                    }
                    else
                    {
                        apntLongBalance[index].Y  = YBottom - (backtesterLongBalance[bar]  - minimum) * YScale;
                        apntShortBalance[index].Y = YBottom - (backtesterShortBalance[bar] - minimum) * YScale;
                    }
                }

                if (showPriceLine)
                {
                    apntClosePrice[index].X = XLeft + index * XScale;
                    apntClosePrice[index].Y = YBottom - (float)(dataClose[bar] - dataMinPrice) * YPriceScale;
                }
            }

            // Margin Call
            if (marginCallBar >= firstBar)
                XMarginCallBar = XLeft + (marginCallBar - firstBar) * XScale;
            else
                XMarginCallBar = 0;

            //OOS
            if (isOOS && barOOS > firstBar)
                XOOSBar = XLeft + (barOOS - firstBar) * XScale;
            else
                XOOSBar = 0;

            YBalance = YBottom - (balance - minimum) * YScale;

            isHideScanningLine = false;
            modellingQuolity = " MQ " + Data.ModellingQuality.ToString("N2") + "%";

            ContextButtonColorBack = LayoutColors.ColorCaptionBack;
            ContextButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(strChartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient
            var rectChartField = new RectangleF(Border, captionHeight, ClientSize.Width - 2 * Border, ClientSize.Height - captionHeight - Border);
            Data.GradientPaint(g, rectChartField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (isNotPaint) return;

            // Grid and Price labels
            for (int labelPrice = minimum; labelPrice <= minimum + countLabels * step; labelPrice += step)
            {
                var labelY = (int)(YBottom - (labelPrice - minimum) * YScale);
                g.DrawString(labelPrice.ToString(CultureInfo.InvariantCulture), Font, brushFore, XRight, labelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, XLeft, labelY, XRight, labelY);
            }

            // Price close
            if (showPriceLine)
                g.DrawLines(new Pen(LayoutColors.ColorChartGrid), apntClosePrice);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red),  apntShortBalance);
                g.DrawLines(new Pen(Color.Green), apntLongBalance);
            }

            // Out of Sample
            if (isOOS && barOOS > 0)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), XOOSBar, YTop, XOOSBar, YBottom);
                Brush brushOOS = new Pen(LayoutColors.ColorChartFore).Brush;
                g.DrawString("OOS", Font, brushOOS, XOOSBar, YBottom - Font.Height);
                float widthOOSBarDate = g.MeasureString(dataTimeBarOOS.ToShortDateString(), Font).Width;
                g.DrawString(dataTimeBarOOS.ToShortDateString(), Font, brushOOS, XOOSBar - widthOOSBarDate, YBottom - Font.Height);
            }

            // Draw Balance Line
            if (marginCallBar > 0)  // In case of Margin Call
            { 
                // Draw balance line up to Margin Call
                var balancePoints = new PointF[marginCallBar - firstBar];
                for (var i = 0; i < balancePoints.Length; i++)
                    balancePoints[i] = apntBalance[i];
                if (balancePoints.Length > 1)
                    g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balancePoints);

                // Draw balance line after Margin Call
                var redBalancePoints = new PointF[bars - marginCallBar];
                for (var i = 0; i < redBalancePoints.Length; i++)
                    redBalancePoints[i] = apntBalance[i + marginCallBar - firstBar];
                g.DrawLines(new Pen(LayoutColors.ColorSignalRed), redBalancePoints);

                // Margin Call line
                g.DrawLine(new Pen(LayoutColors.ColorChartCross), XMarginCallBar, YTop, XMarginCallBar, YBottom);

                // Margin Call label
                float widthMarginCallLabel = g.MeasureString(Language.T("Margin Call"), Font).Width;
                if (XMarginCallBar < XRight - widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, XMarginCallBar, YTop);
                else if (XMarginCallBar > Space + widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, XMarginCallBar - widthMarginCallLabel, YTop);
                else
                    g.DrawString("MC", Font, brushFore, XMarginCallBar, YTop);
            }
            else
            {   // Draw the balance line
                g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);
            }

            // Balance level
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), XLeft, YBalance, XRight - Space + 1, YBalance);

            // Balance label
            var labelSize = new Size(labelWidth + Space, Font.Height + 2);
            var labelPoint = new Point(XRight - Space + 2, (int)(YBalance - Font.Height / 2 - 1));
            var labelRect = new Rectangle(labelPoint, labelSize);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), labelRect);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), labelRect);
            g.DrawString((Math.Round(balance)).ToString(CultureInfo.InvariantCulture), Font, new SolidBrush(LayoutColors.ColorLabelText), labelRect, stringFormatCaption);

            // Scanning note
            var fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Data.Period != DataPeriods.min1 && Configs.Autoscan && !Data.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, XLeft, captionHeight - 2);
            else if (Data.Period != DataPeriods.min1 && isScanPerformed)
                g.DrawString(Language.T("Scanned") + modellingQuolity, fontNote, Brushes.LimeGreen, XLeft, captionHeight - 2);

            // Scanned bars
            if (isScanPerformed && !isHideScanningLine &&
                (Data.IntraBars != null && Data.IsIntrabarData ||
                 Data.Period == DataPeriods.min1 && Data.IsTickData && Configs.UseTickData))
            {
                DataPeriods dataPeriod = Data.Period;
                Color color = Data.PeriodColor[Data.Period];
                int fromBar = firstBar;
                for (int bar = firstBar; bar < bars; bar++)
                {
                    if (Data.IntraBarsPeriods[bar] == dataPeriod && bar != bars - 1) continue;
                    var xStart = (int)((fromBar - firstBar) * XScale) + XLeft;
                    var xEnd   = (int)((bar     - firstBar) * XScale) + XLeft;
                    fromBar = bar;
                    dataPeriod = Data.IntraBarsPeriods[bar];
                    Data.GradientPaint(g, new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5), color, 60);
                    color = Data.PeriodColor[Data.IntraBarsPeriods[bar]];
                }

                // Tick Data
                if (Data.IsTickData && Configs.UseTickData)
                {
                    var firstBarWithTicks = -1;
                    var lastBarWithTicks  = -1;
                    for (var b = 0; b < bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                            lastBarWithTicks = b;
                    }
                    var xStart = (int)((firstBarWithTicks - firstBar) * XScale) + XLeft;
                    var xEnd   = (int)((lastBarWithTicks  - firstBar) * XScale) + XLeft;
                    Data.GradientPaint(g, new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5), color, 60);

                    var rectf = new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xStart, YBottom + 6, xEnd - xStart + 2, 1);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }

                // Vertical coordinate axes
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YBottom, XLeft - 1, YBottom + 9);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YTop - Space, XLeft - 1, YBottom + 1);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YBottom + 1, XRight, YBottom + 1);
        }

        /// <summary>
        /// Generates dynamic info on the status bar
        /// when we are Moving the mouse over the SmallBalanceChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int bar = (int)((e.X - XLeft) / XScale) + firstBar;

            bar = Math.Max(firstBar, bar);
            bar = Math.Min(Data.Bars - 1, bar);

            if (Configs.AccountInMoney)
                strStatusBarText = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                    Data.Time[bar].ToString(Data.DF),
                    Data.Time[bar].ToString("HH:mm"),
                    Language.T("Balance"),
                    Backtester.MoneyBalance(bar).ToString("F2"),
                    Configs.AccountCurrency,
                    Language.T("Equity"),
                    Backtester.MoneyEquity(bar).ToString("F2"),
                    Configs.AccountCurrency);
            else
                strStatusBarText = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                    Data.Time[bar].ToString(Data.DF),
                    Data.Time[bar].ToString("HH:mm"),
                    Language.T("Balance"),
                    Backtester.Balance(bar),
                    Language.T("pips"),
                    Language.T("Equity"),
                    Backtester.Equity(bar),
                    Language.T("pips"));

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                    strStatusBarText += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                        Language.T("Long balance"),
                        Backtester.LongMoneyBalance(bar).ToString("F2"),
                        Configs.AccountCurrency,
                        Language.T("Short balance"),
                        Backtester.ShortMoneyBalance(bar).ToString("F2"),
                        Configs.AccountCurrency);
                else
                    strStatusBarText += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                        Language.T("Long balance"),
                        Backtester.LongBalance(bar),
                        Language.T("pips"),
                        Language.T("Short balance"),
                        Backtester.ShortBalance(bar),
                        Language.T("pips"));
            }
            if (Configs.ShowPriceChartOnAccountChart)
                strStatusBarText += String.Format(" {0}: {1}",
                        Language.T("Price close"),
                        Data.Close[bar]);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            InitChart();
            Invalidate();
        }
    }
}
