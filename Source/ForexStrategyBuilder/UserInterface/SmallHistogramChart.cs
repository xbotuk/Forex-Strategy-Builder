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
using System.Text;
using System.Windows.Forms;
using ForexStrategyBuilder.Common;
using ForexStrategyBuilder.CustomControls;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Draws a small histogram chart
    /// </summary>
    public class SmallHistogramChart : ContextPanel
    {
        private const int Border = 2;
        private const int Space = 5;

        private Brush brushFore;
        private float captionHeight;
        private int chartBars;
        private SortableDictionary<int, HistogramData> chartData;

        private int countLabelsX;
        private int countLabelsY;
        private float deltaX;
        private float deltaY;
        private Font font;
        private bool isCounts = true;
        private bool isNotPaint;
        private bool isShowDynamicInfo;

        private int labelWidthX;
        private int labelWidthY;
        private double maxAbsTotal;
        private int maxCount;
        private int maxIndex;
        private double maxTotal;
        private int minIndex;
        private Pen penBorder;
        private Pen penGrid;
        private RectangleF rectCaption;
        private RectangleF rectSubHeader;
        private StringFormat sfCaption;
        private int stepX;
        private int stepY;
        private string strChartTitle;

        private int xAxisMax;
        private int xAxisMin;
        private int xAxisMin10;
        private int xAxisY;
        private int xLeft;
        private int xRight;
        private float xScale;
        private int yAxisMax;
        private int yAxisMin;
        private int yBottom;
        private float yScale;
        private int yTop;

        /// <summary>
        ///     Whether to show dynamic info
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
        ///     Calculates Data to draw in histogram
        /// </summary>
        private void CalculateHistogramData()
        {
            InitChartData();

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                int positions = StatsBuffer.Positions(bar);
                for (int pos = 0; pos < positions; pos++)
                {
                    if (!IsTrade(StatsBuffer.PosTransaction(bar, pos))) continue;

                    double result = GetProfit(bar, pos);
                    var index = (int) Math.Round(result);
                    bool isIndex = chartData.ContainsKey(index);
                    int count = isIndex ? chartData[index].TradesCount + 1 : 1;
                    double total = isIndex ? chartData[index].TotalResult + result : result;
                    var data = new HistogramData {TradesCount = count, Result = result, TotalResult = total};

                    if (isIndex)
                        chartData[index] = data;
                    else
                        chartData.Add(index, data);

                    SetMinMaxValues(index, count, total);
                }
            }

            chartData.Sort();
        }

        private void InitChartData()
        {
            chartData = new SortableDictionary<int, HistogramData>();
            minIndex = int.MaxValue;
            maxIndex = int.MinValue;
            maxCount = 0;
            maxAbsTotal = 0;
            maxTotal = double.MinValue;
        }

        private static bool IsTrade(Transaction transaction)
        {
            return transaction == Transaction.Close ||
                   transaction == Transaction.Reduce ||
                   transaction == Transaction.Reverse;
        }

        private static double GetProfit(int bar, int pos)
        {
            return Configs.AccountInMoney
                       ? StatsBuffer.PosMoneyProfitLoss(bar, pos)
                       : StatsBuffer.PosProfitLoss(bar, pos);
        }

        private void SetMinMaxValues(int index, int count, double total)
        {
            if (minIndex > index)
                minIndex = index;
            if (maxIndex < index)
                maxIndex = index;
            if (maxCount < count)
                maxCount = count;
            if (maxTotal < total)
                maxTotal = total;
            if (maxAbsTotal < Math.Abs(total))
                maxAbsTotal = Math.Abs(total);
        }

        /// <summary>
        ///     Returns histogram data as a CSV string.
        /// </summary>
        private string GetHistogramDataString()
        {
            var sb = new StringBuilder();

            foreach (var data in chartData)
                sb.AppendLine(data.Key + "\t" + data.Value.TradesCount + "\t" + data.Value.Result.ToString("F2") + "\t" +
                              data.Value.TotalResult.ToString("F2"));

            return sb.ToString();
        }


        /// <summary>
        ///     Sets chart's instrument and back testing data.
        /// </summary>
        public void SetChartData()
        {
            isNotPaint = !Data.IsData || !Data.IsResult || Data.Bars <= StatsBuffer.FirstBar;

            if (isNotPaint) return;

            CalculateHistogramData();

            // set to 0 and length for X Axis
            chartBars = maxIndex - minIndex + 2;

            // Min set to 0 -- will always be 0 or higher
            yAxisMin = 0;
            yAxisMax = isCounts ? maxCount : (int) maxAbsTotal;

            // for X Axis labels
            // set minimum and maximum to indexes, expanded by 1 for border above and under drawn line
            xAxisMax = maxIndex + 1;
            xAxisMin = minIndex - 1;

            // if there are no trades for histogram, set Maxes to arbitrary values so chart draws and avoid errors
            if (chartBars == 3)
                chartBars = 51;
            if (xAxisMax == 0)
                xAxisMax = 51;

            // way to sync all X labels to multiples of 10
            xAxisMin10 = (xAxisMin < 0) ? (int) Math.Ceiling(xAxisMin/10f)*10 : (int) Math.Floor(xAxisMin/10f)*10;
        }

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        public void InitChart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            // Chart Title
            strChartTitle = Language.T("Trade Distribution Chart");
            font = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(font.Height, 18);
            rectCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            rectSubHeader = new RectangleF(0, captionHeight, ClientSize.Width, captionHeight);

            sfCaption = new StringFormat
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
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);

            if (isNotPaint) return;

            yTop = (int) (2*captionHeight + Space + 1);
            yBottom = ClientSize.Height - 2*Space - 1 - Border;
            xAxisY = yBottom - 3 - font.Height;

            Graphics g = CreateGraphics();
            labelWidthY = (int)
                          Math.Max(g.MeasureString(yAxisMin.ToString(CultureInfo.InvariantCulture), Font).Width,
                                   g.MeasureString(yAxisMax.ToString(CultureInfo.InvariantCulture), Font).Width);
            labelWidthX = (int)
                          Math.Max(g.MeasureString(xAxisMin.ToString(CultureInfo.InvariantCulture), Font).Width,
                                   g.MeasureString(xAxisMax.ToString(CultureInfo.InvariantCulture), Font).Width);
            g.Dispose();
            labelWidthY = Math.Max(labelWidthY, 30);
            labelWidthX += 3;

            xLeft = Border + Space;
            xRight = ClientSize.Width - Border - Space - labelWidthY;
            xScale = (xRight - 2*Space - Border)/(float) chartBars;

            countLabelsX = Math.Min((xRight - xLeft)/labelWidthX, 20);
            deltaX = (float) Math.Max(Math.Round((xAxisMax - xAxisMin)/(float) countLabelsX), 10);
            stepX = (int) Math.Ceiling(deltaX/10)*10;
            countLabelsX = (int) Math.Ceiling((xAxisMax - xAxisMin)/(float) stepX);
            xAxisMax = xAxisMin + countLabelsX*stepX;

            // difference from Y Axis for SmallBalanceChart:
            // prefer minimums because histogram counts are usually small, less than 10
            countLabelsY = Math.Min((xAxisY - yTop)/20, 20);
            deltaY = (float) Math.Round((yAxisMax - yAxisMin)/(float) countLabelsY);
            // protect against deltaY infinity and stepY = Number.min
            stepY = (float.IsInfinity(deltaY)) ? 20 : (int) deltaY;
            // protect against dividing by zero in case of no counts
            stepY = (stepY == 0) ? 1 : stepY;
            countLabelsY = (int) Math.Ceiling((yAxisMax - yAxisMin)/(float) stepY);
            // protect against dividing by zero in case of no counts
            countLabelsY = (countLabelsY == 0) ? 5 : countLabelsY;

            // protect against dividing by zero in case of no counts
            yAxisMax = (yAxisMax == 0) ? 5 : yAxisMax;
            yScale = (xAxisY - yTop)/(countLabelsY*(float) stepY);

            // Context button colors.
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
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(strChartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectCaption, sfCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient
            var rectField = new RectangleF(Border, captionHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - captionHeight - Border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (isNotPaint) return;

            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("points")) + "]";
            string subHeader = isCounts ? Language.T("Count of Trades") : Language.T("Accumulated Amount") + unit;
            g.DrawString(subHeader, Font, new SolidBrush(LayoutColors.ColorChartFore), rectSubHeader, sfCaption);
            var formatCenter = new StringFormat {Alignment = StringAlignment.Center};
            // Grid and Price labels
            for (int label = xAxisMin10; label <= xAxisMax; label += stepX)
            {
                float xPoint = xLeft + ((xAxisMin10 - xAxisMin) + (label - xAxisMin10))*xScale;
                float yPoint = yBottom - Font.Height;
                if (xPoint <= xRight - labelWidthX/2)
                    g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xPoint, yPoint,
                                 formatCenter);
            }

            for (int label = yAxisMin; label <= yAxisMax; label += stepY)
            {
                var labelY = (int) (xAxisY - (label - yAxisMin)*yScale);
                if (label > -1)
                    g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                                 labelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, xLeft, labelY, xRight, labelY);
            }

            foreach (var data in chartData)
            {
                double val = isCounts ? data.Value.TradesCount : Math.Abs(data.Value.TotalResult);
                float xPt = xLeft + (data.Key - minIndex + 1)*xScale;
                float yPtBottom = xAxisY;
                var yPtTop = (float) (xAxisY - (val - yAxisMin)*yScale);

                Color color = isCounts ? Color.Blue : data.Value.TotalResult < 0 ? Color.Red : Color.Green;
                g.DrawLine(new Pen(color), xPt, yPtBottom, xPt, yPtTop);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yTop - Space, xLeft - 1, xAxisY);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, xAxisY, xRight, xAxisY);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Generates dynamic info on the status bar
        ///     when we are Moving the mouse over the SmallHistogramChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int index = FindNearestMeaningfulX(e.X);
            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("points")) + "]";

            if (chartData.Count == 0)
            {
                CurrentBarInfo = Language.T("No trades counted");
                return;
            }

            if (chartData.ContainsKey(index))
            {
                CurrentBarInfo = Language.T("Result") + ": " + chartData[index].Result.ToString("F2") + unit +
                                 "  " + Language.T("Count") + ": " + chartData[index].TradesCount +
                                 "  " + Language.T("Total") + ": " + chartData[index].TotalResult.ToString("F2") + unit;
            }
            else
            {
                CurrentBarInfo = Language.T("Result") + ": " + index +
                                 "  " + Language.T("Count") + ": 0" +
                                 "  " + Language.T("Total") + ": 0";
            }
        }

        private int FindNearestMeaningfulX(int x)
        {
            int index = GetIndexFromX(x);

            for (int dX = 0; dX < 6; dX++)
            {
                int oldIndex = index;
                index = GetIndexFromX(x + dX);
                for (int i = oldIndex; i <= index; i++)
                    if (chartData.ContainsKey(i))
                        return i;


                oldIndex = index;
                index = GetIndexFromX(x - dX);
                for (int i = index; i <= oldIndex; i++)
                    if (chartData.ContainsKey(i))
                        return i;
            }

            return index;
        }

        private int GetIndexFromX(int x)
        {
            return (int) Math.Round(minIndex - 1 + (x - xLeft)/xScale);
        }

        public void AddContextMenuItems()
        {
            var sep1 = new ToolStripSeparator();

            var mi1 = new ToolStripMenuItem
                {
                    Image = Resources.toggle,
                    Text = Language.T("Toggle Chart Representation")
                };
            mi1.Click += BtnToggleViewClick;

            var sep2 = new ToolStripSeparator();

            var mi2 = new ToolStripMenuItem
                {
                    Image = Resources.export,
                    Text = Language.T("Export Data to CSV File")
                };
            mi2.Click += BtnExportClick;

            var mi3 = new ToolStripMenuItem
                {
                    Image = Resources.copy,
                    Text = Language.T("Copy Data to Clipboard")
                };
            mi3.Click += BtnClipboardClick;


            var itemCollection = new ToolStripItem[]
                {
                    sep1, mi1, sep2, mi2, mi3
                };

            PopUpContextMenu.Items.AddRange(itemCollection);
        }

        /// <summary>
        ///     Handler to toggle between count and cumulative value views
        /// </summary>
        private void BtnToggleViewClick(object sender, EventArgs e)
        {
            isCounts = !isCounts;
            SetChartData();
            InitChart();
            Invalidate();
        }

        /// <summary>
        ///     Handler to copy histogram data to clipboard
        /// </summary>
        private void BtnClipboardClick(object sender, EventArgs e)
        {
            Clipboard.Clear();
            // protect against null if no trades in strategy
            if (chartData.Count > 0)
            {
                string s = GetHistogramDataString();
                Clipboard.SetText(s);
            }
            else
            {
                string info = Language.T("No trades in Strategy to copy to Clipboard.");
                string caption = Language.T("No Trades");
                MessageBox.Show(info, caption, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///     Handler to write histogram data CSV file
        /// </summary>
        private void BtnExportClick(object sender, EventArgs e)
        {
            // protect against null if no trades in strategy
            if (chartData.Count > 0)
            {
                var exporter = new Exporter();
                exporter.ExportHistogramData(GetHistogramDataString());
            }
            else
            {
                string info = Language.T("No trades in Strategy to Export to CSV.");
                string caption = Language.T("No Trades");
                MessageBox.Show(info, caption, MessageBoxButtons.OK);
            }
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

        private struct HistogramData
        {
            public int TradesCount { get; set; }
            public double Result { get; set; }
            public double TotalResult { get; set; }
        }
    }
}