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
using ForexStrategyBuilder.CustomControls;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Draws a small balance chart
    /// </summary>
    public class SmallBalanceChart : ContextPanel
    {
        private const int Space = 5;
        private const int Border = 2;
        private readonly SmallBalanceChartData data;

        private PointF[] balancePoints;
        private Brush brushFore;
        private float captionHeight;
        private int chartBars;
        private string chartTitle;
        private PointF[] closePricePoints;
        private int countLabels;
        private float delta;
        private PointF[] equityPoints;

        private Font font;
        private bool isHideScanningLine;
        private bool isNotPaint;
        private bool isScanPerformed;
        private int labelStep;
        private int labelWidth;
        private PointF[] longBalancePoints;
        private Pen penBorder;
        private Pen penGrid;
        private RectangleF rectfCaption;
        private PointF[] shortBalancePoints;
        private bool showPriceLine;
        private StringFormat stringFormatCaption;
        private int xLeft;
        private float xMarginCallBar;
        private float xOOSBar;
        private int xRight;
        private float xScale;
        private float yBalance;
        private int yBottom;
        private float yPriceScale;
        private float yScale;
        private int yTop;

        public SmallBalanceChart()
        {
            OOSBar = Data.Bars - 1;
            data = new SmallBalanceChartData();
        }

        /// <summary>
        ///     Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo { private get; set; }

        /// <summary>
        ///     Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get; private set; }

        /// <summary>
        ///     Set the OOS Bar
        /// </summary>
        public int OOSBar { private get; set; }

        /// <summary>
        ///     Set the OOS
        /// </summary>
        public bool IsOOS { private get; set; }

        /// <summary>
        ///     Sets chart's back testing data.
        /// </summary>
        public void SetChartData()
        {
            isNotPaint = !Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar;

            if (isNotPaint) return;

            showPriceLine = Configs.ShowPriceChartOnAccountChart && Backtester.ExecutedOrders > 0;
            isScanPerformed = Backtester.IsScanPerformed;

            data.FirstBar = Data.FirstBar;
            data.Bars = Data.Bars;
            chartBars = Data.Bars - Data.FirstBar;

            int maxBalance = Configs.AccountInMoney ? (int) Backtester.MaxMoneyBalance : Backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int) Backtester.MinMoneyBalance : Backtester.MinBalance;
            int maxEquity = Configs.AccountInMoney ? (int) Backtester.MaxMoneyEquity : Backtester.MaxEquity;
            int minEquity = Configs.AccountInMoney ? (int) Backtester.MinMoneyEquity : Backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance = Configs.AccountInMoney
                                         ? (int) Backtester.MaxLongMoneyBalance
                                         : Backtester.MaxLongBalance;
                int minLongBalance = Configs.AccountInMoney
                                         ? (int) Backtester.MinLongMoneyBalance
                                         : Backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney
                                          ? (int) Backtester.MaxShortMoneyBalance
                                          : Backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney
                                          ? (int) Backtester.MinShortMoneyBalance
                                          : Backtester.MinShortBalance;
                int maxLongShortBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLongShortBalance = Math.Min(minLongBalance, minShortBalance);

                data.Maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLongShortBalance) + 1;
                data.Minimum = Math.Min(Math.Min(minBalance, minEquity), minLongShortBalance) - 1;
            }
            else
            {
                data.Maximum = Math.Max(maxBalance, maxEquity) + 1;
                data.Minimum = Math.Min(minBalance, minEquity) - 1;
            }

            data.Minimum = (int) (Math.Floor(data.Minimum/10f)*10);

            data.DataMaxPrice = Data.MaxPrice;
            data.DataMinPrice = Data.MinPrice;

            if (showPriceLine)
            {
                data.ClosePrice = new double[data.Bars];
                Data.Close.CopyTo(data.ClosePrice, 0);
            }

            if (Configs.AccountInMoney)
            {
                data.MoneyBalance = new double[data.Bars];
                data.MoneyEquity = new double[data.Bars];
            }
            else
            {
                data.Balance = new int[data.Bars];
                data.Equity = new int[data.Bars];
            }

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                {
                    data.LongMoneyBalance = new double[data.Bars];
                    data.ShortMoneyBalance = new double[data.Bars];
                }
                else
                {
                    data.LongBalance = new int[data.Bars];
                    data.ShortBalance = new int[data.Bars];
                }
            }


            for (int bar = data.FirstBar; bar < data.Bars; bar++)
            {
                if (Configs.AccountInMoney)
                {
                    data.MoneyBalance[bar] = Backtester.MoneyBalance(bar);
                    data.MoneyEquity[bar] = Backtester.MoneyEquity(bar);
                }
                else
                {
                    data.Balance[bar] = Backtester.Balance(bar);
                    data.Equity[bar] = Backtester.Equity(bar);
                }

                if (Configs.AdditionalStatistics)
                {
                    if (Configs.AccountInMoney)
                    {
                        data.LongMoneyBalance[bar] = Backtester.LongMoneyBalance(bar);
                        data.ShortMoneyBalance[bar] = Backtester.ShortMoneyBalance(bar);
                    }
                    else
                    {
                        data.LongBalance[bar] = Backtester.LongBalance(bar);
                        data.ShortBalance[bar] = Backtester.ShortBalance(bar);
                    }
                }
            }

            data.MarginCallBar = Backtester.MarginCallBar;

            if (IsOOS && OOSBar > data.FirstBar)
            {
                data.NetBalance =
                    (float) (Configs.AccountInMoney ? Backtester.MoneyBalance(OOSBar) : Backtester.Balance(OOSBar));
                data.DataTimeBarOOS = Data.Time[OOSBar];
            }
            else
                data.NetBalance = (float) (Configs.AccountInMoney ? Backtester.NetMoneyBalance : Backtester.NetBalance);
        }

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        public void InitChart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            // Chart Title
            chartTitle = Language.T("Balance / Equity Chart") + " [" +
                         (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("points") + "]");
            font = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(font.Height, 18);
            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid = new Pen(LayoutColors.ColorChartGrid)
                {
                    DashStyle = DashStyle.Dash,
                    DashPattern = new float[] {4, 2}
                };
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                Border);

            if (isNotPaint) return;

            yTop = (int) captionHeight + 2*Space + 1;
            yBottom = ClientSize.Height - 2*Space - 1 - Border;

            Graphics g = CreateGraphics();
            float widthMinimum = g.MeasureString(data.Minimum.ToString(CultureInfo.InvariantCulture), Font).Width;
            float widthMaximum = g.MeasureString(data.Maximum.ToString(CultureInfo.InvariantCulture), Font).Width;
            labelWidth = (int) Math.Max(widthMinimum, widthMaximum);
            labelWidth = Math.Max(labelWidth, 30);
            g.Dispose();

            xLeft = Border + Space;
            xRight = ClientSize.Width - Border - Space - labelWidth;
            xScale = (xRight - 2*Space - Border)/(float) chartBars;

            countLabels = (int) Math.Max((yBottom - yTop)/20.0, 1);
            delta = (float) Math.Max(Math.Round((data.Maximum - data.Minimum)/(float) countLabels), 10);
            labelStep = (int) Math.Ceiling(delta/10)*10;
            countLabels = (int) Math.Ceiling((data.Maximum - data.Minimum)/(float) labelStep);
            yScale = (yBottom - yTop)/(countLabels*(float) labelStep);

            balancePoints = new PointF[chartBars];
            equityPoints = new PointF[chartBars];

            if (Configs.AdditionalStatistics)
            {
                longBalancePoints = new PointF[chartBars];
                shortBalancePoints = new PointF[chartBars];
            }

            closePricePoints = new PointF[chartBars];

            // Close Price
            if (showPriceLine)
                yPriceScale = (float) ((yBottom - yTop)/(data.DataMaxPrice - data.DataMinPrice));

            for (int bar = data.FirstBar; bar < data.Bars; bar++)
            {
                int index = bar - data.FirstBar;
                balancePoints[index].X = xLeft + index*xScale;
                equityPoints[index].X = xLeft + index*xScale;
                if (Configs.AccountInMoney)
                {
                    balancePoints[index].Y = (float) (yBottom - (data.MoneyBalance[bar] - data.Minimum)*yScale);
                    equityPoints[index].Y = (float) (yBottom - (data.MoneyEquity[bar] - data.Minimum)*yScale);
                }
                else
                {
                    balancePoints[index].Y = yBottom - (data.Balance[bar] - data.Minimum)*yScale;
                    equityPoints[index].Y = yBottom - (data.Equity[bar] - data.Minimum)*yScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    longBalancePoints[index].X = xLeft + index*xScale;
                    shortBalancePoints[index].X = xLeft + index*xScale;
                    if (Configs.AccountInMoney)
                    {
                        longBalancePoints[index].Y =
                            (float) (yBottom - (data.LongMoneyBalance[bar] - data.Minimum)*yScale);
                        shortBalancePoints[index].Y =
                            (float) (yBottom - (data.ShortMoneyBalance[bar] - data.Minimum)*yScale);
                    }
                    else
                    {
                        longBalancePoints[index].Y = yBottom - (data.LongBalance[bar] - data.Minimum)*yScale;
                        shortBalancePoints[index].Y = yBottom - (data.ShortBalance[bar] - data.Minimum)*yScale;
                    }
                }

                if (showPriceLine)
                {
                    closePricePoints[index].X = xLeft + index*xScale;
                    closePricePoints[index].Y = yBottom -
                                                (float) (data.ClosePrice[bar] - data.DataMinPrice)*yPriceScale;
                }
            }

            // Margin Call
            xMarginCallBar = data.MarginCallBar >= data.FirstBar
                                 ? xLeft + (data.MarginCallBar - data.FirstBar)*xScale
                                 : 0;

            //OOS
            if (IsOOS && OOSBar > data.FirstBar)
                xOOSBar = xLeft + (OOSBar - data.FirstBar)*xScale;
            else
                xOOSBar = 0;

            yBalance = yBottom - (data.NetBalance - data.Minimum)*yScale;

            isHideScanningLine = false;
            data.ModellingQuolity = " MQ " + Data.ModellingQuality.ToString("N2") + "%";

            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
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

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption,
                         stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient
            var rectChartField = new RectangleF(Border, captionHeight, ClientSize.Width - 2*Border,
                                                ClientSize.Height - captionHeight - Border);
            Data.GradientPaint(g, rectChartField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (isNotPaint) return;

            // Grid and Price labels
            for (int labelPrice = data.Minimum;
                 labelPrice <= data.Minimum + countLabels*labelStep;
                 labelPrice += labelStep)
            {
                var labelY = (int) (yBottom - (labelPrice - data.Minimum)*yScale);
                g.DrawString(labelPrice.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, xLeft, labelY, xRight, labelY);
            }

            // Price close
            if (showPriceLine)
                g.DrawLines(new Pen(LayoutColors.ColorChartGrid), closePricePoints);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), equityPoints);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red), shortBalancePoints);
                g.DrawLines(new Pen(Color.Green), longBalancePoints);
            }

            // Out of Sample
            if (IsOOS && OOSBar > 0)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), xOOSBar, yTop, xOOSBar, yBottom);
                Brush brushOOS = new Pen(LayoutColors.ColorChartFore).Brush;
                g.DrawString("OOS", Font, brushOOS, xOOSBar, yBottom - Font.Height);
                float widthOOSBarDate = g.MeasureString(data.DataTimeBarOOS.ToShortDateString(), Font).Width;
                g.DrawString(data.DataTimeBarOOS.ToShortDateString(), Font, brushOOS, xOOSBar - widthOOSBarDate,
                             yBottom - Font.Height);
            }

            // Draw Balance Line
            if (data.MarginCallBar > 0) // In case of Margin Call
            {
                // Draw balance line up to Margin Call
                var balPoints = new PointF[data.MarginCallBar - data.FirstBar];
                for (int i = 0; i < balPoints.Length; i++)
                    balPoints[i] = balancePoints[i];
                if (balPoints.Length > 1)
                    g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balPoints);

                // Draw balance line after Margin Call
                var redBalancePoints = new PointF[data.Bars - data.MarginCallBar];
                for (int i = 0; i < redBalancePoints.Length; i++)
                    redBalancePoints[i] = balancePoints[i + data.MarginCallBar - data.FirstBar];
                g.DrawLines(new Pen(LayoutColors.ColorSignalRed), redBalancePoints);

                // Margin Call line
                g.DrawLine(new Pen(LayoutColors.ColorChartCross), xMarginCallBar, yTop, xMarginCallBar, yBottom);

                // Margin Call label
                float widthMarginCallLabel = g.MeasureString(Language.T("Margin Call"), Font).Width;
                if (xMarginCallBar < xRight - widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, xMarginCallBar, yTop);
                else if (xMarginCallBar > Space + widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, xMarginCallBar - widthMarginCallLabel,
                                 yTop);
                else
                    g.DrawString("MC", Font, brushFore, xMarginCallBar, yTop);
            }
            else
            {
                // Draw the balance line
                g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balancePoints);
            }

            // Scanning note
            var fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Data.Period != DataPeriod.M1 && Configs.Autoscan && !Data.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, xLeft, captionHeight - 2);
            else if (Data.Period != DataPeriod.M1 && isScanPerformed)
                g.DrawString(Language.T("Scanned") + data.ModellingQuolity, fontNote, Brushes.LimeGreen, xLeft,
                             captionHeight - 2);

            // Scanned bars
            if (isScanPerformed && !isHideScanningLine &&
                (Data.IntraBars != null && Data.IsIntrabarData ||
                 Data.Period == DataPeriod.M1 && Data.IsTickData && Configs.UseTickData))
            {
                DataPeriod dataPeriod = Data.Period;
                Color color = Data.PeriodColor[Data.Period];
                int fromBar = data.FirstBar;
                for (int bar = data.FirstBar; bar < data.Bars; bar++)
                {
                    if (Data.IntraBarsPeriods[bar] == dataPeriod && bar != data.Bars - 1) continue;
                    int xStart = (int) ((fromBar - data.FirstBar)*xScale) + xLeft;
                    int xEnd = (int) ((bar - data.FirstBar)*xScale) + xLeft;
                    fromBar = bar;
                    dataPeriod = Data.IntraBarsPeriods[bar];
                    Data.GradientPaint(g, new RectangleF(xStart, yBottom + 4, xEnd - xStart + 2, 5), color, 60);
                    color = Data.PeriodColor[Data.IntraBarsPeriods[bar]];
                }

                // Tick Data
                if (Data.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    for (int b = 0; b < data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                            lastBarWithTicks = b;
                    }
                    int xStart = (int) (firstBarWithTicks*xScale) + xLeft;
                    int xEnd = (int) ((lastBarWithTicks - data.FirstBar)*xScale) + xLeft;
                    if (xStart < xLeft)
                        xStart = xLeft;
                    if (xEnd < xStart)
                        xEnd = xStart;

                    Data.DrawCheckerBoard(g, Color.ForestGreen, new Rectangle(xStart, yBottom + 5, xEnd - xStart + 2, 3));
                }

                // Vertical coordinate axes
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yBottom, xLeft - 1, yBottom + 9);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yTop - Space, xLeft - 1, yBottom + 1);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yBottom + 1, xRight, yBottom + 1);

            // Balance level
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), xLeft, yBalance, xRight - Space + 1, yBalance);

            // Balance label
            var labelSize = new Size(labelWidth + Space, Font.Height + 2);
            var labelPoint = new Point(xRight - Space + 2, (int) (yBalance - Font.Height/2.0 - 1));
            var labelRect = new Rectangle(labelPoint, labelSize);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), labelRect);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), labelRect);
            g.DrawString((Math.Round(data.NetBalance)).ToString(CultureInfo.InvariantCulture), Font,
                         new SolidBrush(LayoutColors.ColorLabelText), labelRect, stringFormatCaption);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Generates dynamic info on the status bar
        ///     when we are Moving the mouse over the SmallBalanceChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!ShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int bar = (int) ((e.X - xLeft)/xScale) + data.FirstBar;

            bar = Math.Max(data.FirstBar, bar);
            bar = Math.Min(data.Bars - 1, bar);

            if (Configs.AccountInMoney)
                CurrentBarInfo = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                                               Data.Time[bar].ToString(Data.Df),
                                               Data.Time[bar].ToString("HH:mm"),
                                               Language.T("Balance"),
                                               data.MoneyBalance[bar].ToString("F2"),
                                               Configs.AccountCurrency,
                                               Language.T("Equity"),
                                               data.MoneyEquity[bar].ToString("F2"),
                                               Configs.AccountCurrency);
            else
                CurrentBarInfo = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                                               Data.Time[bar].ToString(Data.Df),
                                               Data.Time[bar].ToString("HH:mm"),
                                               Language.T("Balance"),
                                               data.Balance[bar],
                                               Language.T("points"),
                                               Language.T("Equity"),
                                               data.Equity[bar],
                                               Language.T("points"));

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                    CurrentBarInfo += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                                                    Language.T("Long balance"),
                                                    data.LongMoneyBalance[bar].ToString("F2"),
                                                    Configs.AccountCurrency,
                                                    Language.T("Short balance"),
                                                    data.ShortMoneyBalance[bar].ToString("F2"),
                                                    Configs.AccountCurrency);
                else
                    CurrentBarInfo += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                                                    Language.T("Long balance"),
                                                    data.LongBalance[bar],
                                                    Language.T("points"),
                                                    Language.T("Short balance"),
                                                    data.ShortBalance[bar],
                                                    Language.T("points"));
            }
            if (Configs.ShowPriceChartOnAccountChart)
                CurrentBarInfo += String.Format(" {0}: {1}",
                                                Language.T("Price close"),
                                                Data.Close[bar]);
        }

        /// <summary>
        ///     Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            UpdateButtonsLocation();
            InitChart();
            Invalidate();
        }
    }
}