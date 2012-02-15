// Small Histogram Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace Forex_Strategy_Builder {
    /// <summary>
    /// Draws a small histogram chart
    /// </summary>
    public class Small_Histogram_Chart : Panel {
        int space = 5;
        int border = 2;

        int XLeft;
        int XRight;
        int YTop;
        int YBottom;
        float XScale;
        float YScale;
        int XAxis_Y;
        bool showPriceLine;
        bool isNotPaint = false;

        int countLabelsX;
        float deltaX;
        int stepX;
        int countLabelsY;
        float deltaY;
        int stepY;

        string strStatusBarText;
        bool isShowDynamicInfo = false;

        // bars -- 0-index for how many bars to draw on chart
        int bars;
        int chartBars;
        // for Y Axis labels
        int YAxisMax;
        int YAxisMin;
        // for X Axis labels
        int XAxisMin;
        int XAxisMin10;
        int XAxisMax;

        int labelWidthX;
        int labelWidthY;
        string strChartTitle;
        Font font;
        float captionHeight;
        RectangleF rectfCaption;
        StringFormat stringFormatCaption;
        Brush brushFore;
        Pen penGrid;
        Pen penBorder;
        Button btnToggleView;
        Button btnClipboard;
        Button btnExport;
        ToolTip toolTip;
        bool isCounts = true;

        int firstBar;
        bool isScanPerformed;

        // array for all the result values from trades
        int[] tradeResults;
        // array to go from lowest result to highest with indexes for all values in between
        int[] tradeIndexes;
        // count of how many trades resulted in that particular trade amount
        int[] tradeCounts;
        // pip value of trades that resulted in that amount (ie, result * count)
        int[] tradeCumulatives;


        /// <summary>
        /// Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo { get { return isShowDynamicInfo; } set { isShowDynamicInfo = value; } }

        /// <summary>
        /// Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get { return strStatusBarText; } }

        /// <summary>
        /// Returns X Left
        /// </summary>
        public int XLeftMargin { get { return border + space; } }

        /// <summary>
        /// Returns X Right
        /// </summary>
        public int XRightMargin { get { return XRight - space; } }

        /// <summary>
        /// Gets the Button Toggle View
        /// </summary>
        public Button BtnToggleView { get { return btnToggleView; } }

        /// <summary>
        /// Gets the Button Clipboard
        /// </summary>
        public Button BtnClipboard { get { return btnClipboard; } }

        /// <summary>
        /// Gets the Button Export
        /// </summary>
        public Button BtnExport { get { return btnExport; } }

        /// <summary>
        /// Returns Histogram Data as CSV string
        /// </summary>
        public string GetHistogramDataString { get { return getHistogramDataString(); } }


        /// <summary>
        /// Transforms input arrays into histogram data
        /// </summary>
        public static void GetHistogramDataInts(out int[] results, out int[] indexes, out int[] counts, out int[] cumulatives) {
            getHistogramDataInts(out results, out indexes, out counts, out cumulatives);
        }

        /// <summary>
        /// Get Data to draw in histogram
        /// </summary>
        private static void getHistogramDataInts(out int[] results, out int[] indexes, out int[] counts, out int[] cumulatives) {
            // crummy way to get number of trades for init array
            // TBD -- find better property
            int ctr = 0;
            int min = 0;
            int max = 0;
            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++) {
                Position position = Backtester.PosFromNumb(iPos);
                if (position.Transaction == Transaction.Close) {
                    ctr++;
                }
            }

            results = new int[ctr];
            ctr = 0;
            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++) {
                Position position = Backtester.PosFromNumb(iPos);
                if (position.Transaction == Transaction.Close) {
                    results[ctr] = (int)(position.ProfitLoss);
                    ctr++;
                }
            }

            Array.Sort(results);
            if (results.Length > 0) {
                min = results[0];
                max = results[results.Length - 1];
            }
            else {
                min = 0;
                max = 0;
            }
            indexes = new int[(max - min) + 1];
            counts = new int[(max - min) + 1];
            cumulatives = new int[(max - min) + 1];

            // fill indexes with index values, then count how many in results
            for (int ctr1 = 0; ctr1 < indexes.Length; ctr1++) {
                indexes[ctr1] = min + ctr1;
                int iCount = 0;
                for (int ctr2 = 0; ctr2 < results.Length; ctr2++) {
                    if (results[ctr2] == indexes[ctr1]) {
                        iCount++;
                    }
                }
                counts[ctr1] = iCount;
                cumulatives[ctr1] = indexes[ctr1] * iCount;
            }
        }

        /// <summary>
        /// Returns histogram data as a CSV string
        /// </summary>
        /// <returns>string</returns>
        private string getHistogramDataString() {
            string s = "";
            if (tradeResults.Length > 0) {
                for (int i = 0; i < tradeIndexes.Length; i++) {
                    s += tradeIndexes[i].ToString() + "\t" + tradeCounts[i].ToString() + "\t" + tradeCumulatives[i].ToString() + Environment.NewLine;
                }
            }
            return s;
        }


        /// <summary>
        /// Sets chart's instrument and back testing data.
        /// </summary>
        public void SetChartData() {
            isNotPaint = !Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar;

            if (isNotPaint) return;

            getHistogramDataInts(out tradeResults, out tradeIndexes, out tradeCounts, out tradeCumulatives);

            showPriceLine = Configs.ShowPriceChartOnAccountChart && Backtester.ExecutedOrders > 0;
            isScanPerformed = Backtester.IsScanPerformed;

            // set to 0 and length for X Axis
            firstBar  = 0;
            bars      = tradeIndexes.Length;
            chartBars = tradeIndexes.Length + 2;

            // Min set to 0 -- will always be 0 or higher
            YAxisMin = 0;
            YAxisMax = (isCounts) ? FindMax(tradeCounts, false) : FindMax(tradeCumulatives, true);

            // for X Axis labels
            // set minimum and maximum to indexes, expanded by 1 for border above and under drawn line
            XAxisMax = tradeIndexes[tradeIndexes.Length - 1] + 1;
            XAxisMin = tradeIndexes[0] - 1;

            // if there are no trades for histogram, set Maxes to arbitrary values so chart draws and avoid errors
            if (chartBars == 3)
                chartBars = 51;
            if (XAxisMax == 0)
                XAxisMax = 51;


            // way to sync all X labels to multiples of 10
            XAxisMin10 = (XAxisMin < 0) ? (int)Math.Ceiling((double)(XAxisMin / 10)) * 10 : (int)Math.Floor((double)(XAxisMin / 10)) * 10;

            return;
        }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        public void InitChart() {
            // Tool Tips
            toolTip = new ToolTip();

            // Chart Title
            strChartTitle                     = Language.T("Histogram Chart");
            font                              = new Font(Font.FontFamily, 9);
            captionHeight                     = (float)Math.Max(font.Height, 18);
            rectfCaption                      = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption               = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;

            // Button Toggle View between counts and total cumulative amounts
            btnToggleView                         = new Button();
            btnToggleView.Parent                  = this;
            btnToggleView.BackgroundImage         = Properties.Resources.toggle_journal;
            btnToggleView.BackgroundImageLayout   = ImageLayout.Center;
            btnToggleView.Cursor                  = Cursors.Hand;
            btnToggleView.Size                    = new Size(20, font.Height);
            btnToggleView.UseVisualStyleBackColor = true;
            btnToggleView.Click                  += new EventHandler(BtnToggleView_Click);
            toolTip.SetToolTip(btnToggleView, Language.T("Toggle between Trade Counts and Cumulative Amounts"));

            // Button Clipboard copies histogram data to clipboard
            btnClipboard                          = new Button();
            btnClipboard.Parent                   = this;
            btnClipboard.BackgroundImage          = Properties.Resources.copy;
            btnClipboard.BackgroundImageLayout    = ImageLayout.Center;
            btnClipboard.Cursor                   = Cursors.Hand;
            btnClipboard.Size                     = new Size(20, font.Height);
            btnClipboard.Location                 = new Point(btnToggleView.Location.X + btnToggleView.Width + space, btnToggleView.Location.Y);
            btnClipboard.UseVisualStyleBackColor  = true;
            btnClipboard.Click                   += new EventHandler(BtnClipboard_Click);
            toolTip.SetToolTip(btnClipboard, Language.T("Copy Histogram Data to Clipboard"));

            // Button Export writes histogram data to csv file
            btnExport                             = new Button();
            btnExport.Parent                      = this;
            btnExport.BackgroundImage             = Properties.Resources.export;
            btnExport.BackgroundImageLayout       = ImageLayout.Center;
            btnExport.Cursor                      = Cursors.Hand;
            btnExport.Size                        = new Size(20, font.Height);
            btnExport.Location                    = new Point(btnClipboard.Location.X + btnClipboard.Width + space, btnClipboard.Location.Y);
            btnExport.UseVisualStyleBackColor     = true;
            btnExport.Click                      += new EventHandler(BtnExport_Click);
            toolTip.SetToolTip(btnExport, Language.T("Write Histogram Data to CSV File"));


            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid             = new Pen(LayoutColors.ColorChartGrid);
            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float[] { 4, 2 };
            penBorder           = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            if (isNotPaint) return;

            YTop    = (int)captionHeight + 2 * space + 1;
            YBottom = ClientSize.Height - 2 * space - 1 - border;
            XAxis_Y = YBottom - 3 - font.Height;

            Graphics g  = CreateGraphics();
            labelWidthY = (int)Math.Max(g.MeasureString(YAxisMin.ToString(), Font).Width, g.MeasureString(YAxisMax.ToString(), Font).Width);
            labelWidthX = (int)Math.Max(g.MeasureString(XAxisMin.ToString(), Font).Width, g.MeasureString(XAxisMax.ToString(), Font).Width);
            g.Dispose();
            labelWidthY = Math.Max(labelWidthY, 30);
            labelWidthX += 3;

            XLeft  = border + space;
            XRight = ClientSize.Width - border - space - labelWidthY;
            XScale = (XRight - 2 * space - border) / (float)chartBars;

            countLabelsX = (int)Math.Min((XRight - XLeft) / labelWidthX, 20);
            deltaX       = (float)Math.Max(Math.Round((XAxisMax - XAxisMin) / (float)countLabelsX), 10);
            stepX        = (int)Math.Ceiling(deltaX / 10) * 10;
            countLabelsX = (int)Math.Ceiling((XAxisMax - XAxisMin) / (float)stepX);
            XAxisMax     = XAxisMin + countLabelsX * stepX;


            // difference from Y Axis for Small_Balance_Chart:
            // prefer minimums because histogram counts are usually small, less than 10
            countLabelsY = (int)Math.Min((XAxis_Y - YTop) / 20, 20);
            deltaY       = (float)Math.Round((YAxisMax - YAxisMin) / (float)countLabelsY);
            // protect against deltaY infinity and stepY = Number.min
            stepY        = (float.IsInfinity(deltaY)) ? 20 : (int)deltaY;
            // protect against dividing by zero in case of no counts
            stepY        = (stepY == 0) ? 1 : stepY;
            countLabelsY = (int)Math.Ceiling((YAxisMax - YAxisMin) / (float)stepY);
            // protect against dividing by zero in case of no counts
            countLabelsY = (countLabelsY == 0) ? 5 : countLabelsY;

            // protect against dividing by zero in case of no counts
            YAxisMax = (YAxisMax == 0) ? 5 : YAxisMax;
            YScale   = (XAxis_Y - YTop) / (countLabelsY * (float)stepY);


        }


        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(strChartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            // Paints the background by gradient
            RectangleF rectField = new RectangleF(border, captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - captionHeight - border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (isNotPaint) return;

            // Grid and Price labels
            for (int label = XAxisMin10; label <= XAxisMax; label += stepX) {
                float xPoint = XLeft + ((XAxisMin10 - XAxisMin) + (label - XAxisMin10)) * XScale;
                float yPoint = YBottom - Font.Height;
                if (xPoint < XRight - labelWidthX) {
                    g.DrawString(label.ToString(), Font, brushFore, xPoint, yPoint);
                }
            }

            for (int label = YAxisMin; label <= YAxisMax; label += stepY) {
                int labelY = (int)(XAxis_Y - (label - YAxisMin) * YScale);
                if (label > -1) {
                    g.DrawString(label.ToString(), Font, brushFore, XRight, labelY - Font.Height / 2 - 1);
                }
                g.DrawLine(penGrid, XLeft, labelY, XRight, labelY);
            }

            for (int i = 0; i < tradeCounts.Length; i++) {
                Single xPt = XLeft + (i + 1) * XScale;
                Single yPtBottom = XAxis_Y;
                Single yPtTop;
                if (isCounts) {
                    yPtTop = (XAxis_Y - (tradeCounts[i] - YAxisMin) * YScale);
                    g.DrawLine(new Pen(Color.Blue), xPt, yPtBottom, xPt, yPtTop);
                }
                else {
                    yPtTop = XAxis_Y - (Math.Abs(tradeCumulatives[i]) - YAxisMin) * YScale;
                    // change Pen so red for losses, green for wins
                    if (tradeIndexes[i] < 0) {
                        g.DrawLine(new Pen(Color.Red), xPt, yPtBottom, xPt, yPtTop);
                    }
                    else {
                        g.DrawLine(new Pen(Color.Green), xPt, yPtBottom, xPt, yPtTop);
                    }
                }
            }


            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YTop - space, XLeft - 1, XAxis_Y);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, XAxis_Y, XRight, XAxis_Y);

            return;
        }

        /// <summary>
        /// Generates dynamic info on the status bar
        /// when we are Moving the mouse over the SmallBalanceChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int bar = (int)((e.X - XLeft) / XScale) + firstBar;

            bar = Math.Max(firstBar, bar);
            bar = Math.Min(chartBars, bar);

            if (tradeResults.Length == 0) {
                strStatusBarText = Language.T("No trades counted");
            }
            else {
                if (bar > 0 && bar <= tradeIndexes.Length) {
                    strStatusBarText = Language.T("Result: ") + tradeIndexes[bar - 1].ToString() +
                                       Language.T("  Count: ") + tradeCounts[bar - 1].ToString() +
                                       Language.T("  Total: ") + tradeCumulatives[bar - 1].ToString();
                }
                else {
                    strStatusBarText = Language.T("Result: ") + (bar + tradeIndexes[0] - 1).ToString() +
                                       Language.T("  Count: 0  Total: 0");
                }
            }
        }


        /// <summary>
        ///  Handler to toggle between count and cumulative value views
        /// </summary>
        void BtnToggleView_Click(object sender, EventArgs e) {
            isCounts = (isCounts) ? false : true;
            SetChartData();
            InitChart();
            Invalidate();
        }

        /// <summary>
        ///  Handler to copy histogram data to clipboard
        /// </summary>
        void BtnClipboard_Click(object sender, EventArgs e) {
            Clipboard.Clear();
            // protect against null if no trades in strategy
            if (tradeResults.Length > 0) {
                string s = getHistogramDataString();
                Clipboard.SetText(s);
            }
            else {
                string sInfo = Language.T("No trades in Strategy to copy to Clipboard.");
                string sCaption = Language.T("No Trades");
                MessageBox.Show(sInfo, sCaption, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  Handler to write histogram data CSV file
        /// </summary>
        void BtnExport_Click(object sender, EventArgs e) {
            // protect against null if no trades in strategy
            if (tradeResults.Length > 0) {
                Exporter exporter = new Exporter();
                exporter.ExportHistogramData(getHistogramDataString());
            }
            else {
                string sInfo = Language.T("No trades in Strategy to Export to CSV.");
                string sCaption = Language.T("No Trades");
                MessageBox.Show(sInfo, sCaption, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs) {
            InitChart();
            Invalidate();
        }

        /// <summary>
        /// Utility function for finding max value in an array
        /// </summary>
        private int FindMax(int[] arr, bool abs) {
            int max = int.MinValue;
            if (abs) {
                for (int i = 0; i < arr.Length; i++) {
                    if (Math.Abs(arr[i]) > max) {
                        max = Math.Abs(arr[i]);
                    }
                }
            }
            else {
                for (int i = 0; i < arr.Length; i++) {
                    if (arr[i] > max) {
                        max = arr[i];
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Utility function for finding min value in an array
        /// </summary>
        private int FindMin(int[] arr) {
            int[] arrCopy = (int[])arr.Clone();
            Array.Sort(arrCopy);
            return arrCopy[0];
        }
    }
}
