// Bar Explorer
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    internal sealed class BarExplorer : Form
    {
        private const int Border = 2;
        private readonly Brush brushCaptionText;
        private readonly Brush brushEvenRow;
        private readonly Brush brushGridText;
        private readonly Brush brushRed;
        private readonly Button btnClose;
        private readonly Button btnGo;
        private readonly Button[] buttonsNavigate;

        private readonly Color colorBarBlack1;
        private readonly Color colorBarBlack2;
        private readonly Color colorBarWight1;
        private readonly Color colorBarWight2;
        private readonly Color colorClosedTrade1;
        private readonly Color colorClosedTrade2;

        private readonly Color colorLongTrade1;
        private readonly Color colorLongTrade2;
        private readonly Color colorShortTrade1;
        private readonly Color colorShortTrade2;
        private readonly Font fontInfo;
        private readonly int infoRowHeight;
        private readonly NumericUpDown nudGo;
        private readonly Pen penAxes;
        private readonly Pen penBarBorder;
        private readonly Pen penCross;
        private readonly Pen penGrid;
        private readonly Panel pnlChart;
        private readonly Panel pnlInfo;
        private readonly ToolTip toolTip;
        private int[] aiColumnX; // The horizontal position of the column
        private int[] aiX; // The scaled horizontal position of the column
        private string[] asContent; // The text journal data
        private string[] asTitles; // Journal title second row
        private int barCurrent;
        private string barInfo;
        private int columns; // The number of the columns
        private int maxWayPoints = 7;
        private Size szPrice;

        /// <summary>
        ///     Initialize the form and controls
        /// </summary>
        public BarExplorer(int barNumber)
        {
            pnlChart = new Panel();
            pnlInfo = new Panel();
            toolTip = new ToolTip();

            barCurrent = barNumber < Data.FirstBar ? Data.FirstBar : barNumber;

            Text = Language.T("Bar Explorer");
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            fontInfo = new Font(Font.FontFamily, 9);
            infoRowHeight = Math.Max(fontInfo.Height, 18);

            barInfo = Language.T("Bar") + ": " + (barCurrent + 1) + " " +
                      Data.Time[barCurrent].ToString(Data.DF) + " " +
                      Data.Time[barCurrent].ToString("HH:mm") + "; " +
                      Language.T("Interpolation method") + ": " +
                      Backtester.InterpolationMethodToString();

            pnlChart.Parent = this;
            pnlChart.Paint += PnlChartPaint;

            pnlInfo.Parent = this;
            pnlInfo.Paint += PnlInfoPaint;

            buttonsNavigate = new Button[6];
            var btnNavigateText = new[] {"< !", "<<", "<", ">", ">>", "! >"};
            var btnNavigateTips = new[]
                {
                    Language.T("Previous ambiguous bar."),
                    Language.T("Previous deal."),
                    Language.T("Previous bar."),
                    Language.T("Next bar."),
                    Language.T("Next deal."),
                    Language.T("Next ambiguous bar.")
                };

            for (int i = 0; i < 6; i++)
            {
                buttonsNavigate[i] = new Button {Parent = this, Text = btnNavigateText[i], Name = btnNavigateText[i]};
                buttonsNavigate[i].Click += BtnNavigateClick;
                buttonsNavigate[i].MouseWheel += BarExplorerMouseWheel;
                buttonsNavigate[i].KeyUp += BtnNavigateKeyUp;
                buttonsNavigate[i].UseVisualStyleBackColor = true;
                toolTip.SetToolTip(buttonsNavigate[i], btnNavigateTips[i]);
            }

            buttonsNavigate[0].Enabled = Backtester.AmbiguousBars > 0;
            buttonsNavigate[1].Enabled = Backtester.PositionsTotal > 0;
            buttonsNavigate[4].Enabled = Backtester.PositionsTotal > 0;
            buttonsNavigate[5].Enabled = Backtester.AmbiguousBars > 0;

            nudGo = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudGo.BeginInit();
            nudGo.Minimum = Data.FirstBar + 1;
            nudGo.Maximum = Data.Bars;
            nudGo.Increment = 1;
            nudGo.Value = barCurrent + 1;
            nudGo.KeyUp += BtnNavigateKeyUp;
            nudGo.EndInit();

            btnGo = new Button {Parent = this, Name = "Go", Text = Language.T("Go"), UseVisualStyleBackColor = true};
            btnGo.Click += BtnNavigateClick;
            btnGo.MouseWheel += BarExplorerMouseWheel;
            btnGo.KeyUp += BtnNavigateKeyUp;
            toolTip.SetToolTip(btnGo, Language.T("Go to the chosen bar."));

            //Button Close
            btnClose = new Button
                {
                    Parent = this,
                    Text = Language.T("Close"),
                    DialogResult = DialogResult.Cancel,
                    UseVisualStyleBackColor = true
                };

            // Colors
            brushRed = new SolidBrush(LayoutColors.ColorSignalRed);

            brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRow = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushGridText = new SolidBrush(LayoutColors.ColorChartFore);

            penGrid = new Pen(LayoutColors.ColorChartGrid)
                {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            penAxes = new Pen(LayoutColors.ColorChartFore);
            penCross = new Pen(LayoutColors.ColorChartCross);
            penBarBorder = new Pen(LayoutColors.ColorBarBorder);

            colorBarWight1 = Data.GetGradientColor(LayoutColors.ColorBarWhite, 30);
            colorBarWight2 = Data.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.GetGradientColor(LayoutColors.ColorBarBlack, 30);
            colorBarBlack2 = Data.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeLong, 30);
            colorLongTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeLong, -30);
            colorShortTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeShort, 30);
            colorShortTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeClose, 30);
            colorClosedTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeClose, -30);

            SetJournalPoints();
        }

        /// <summary>
        ///     Sets the journal columns with.
        /// </summary>
        private void SetJournalPoints()
        {
            columns = 7;
            aiColumnX = new int[8];
            aiX = new int[8];

            Graphics g = CreateGraphics();

            asTitles = new[]
                {
                    Language.T("Number"),
                    Language.T("Description"),
                    Language.T("Price"),
                    Language.T("Direction"),
                    Language.T("Lots"),
                    Language.T("Position"),
                    Language.T("Order")
                };

            string longestDescription = "";
            foreach (WayPointType wpType in Enum.GetValues(typeof (WayPointType)))
                if (g.MeasureString(Language.T(WayPoint.WPTypeToString(wpType)), fontInfo).Width >
                    g.MeasureString(longestDescription, fontInfo).Width)
                    longestDescription = Language.T(WayPoint.WPTypeToString(wpType));

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), fontInfo).Width >
                    g.MeasureString(longestDirection, fontInfo).Width)
                    longestDirection = Language.T(posDir.ToString());

            asContent = new[]
                {
                    "99",
                    longestDescription,
                    "99.99999",
                    longestDirection,
                    "99999",
                    "99999",
                    "99999"
                };

            aiColumnX[0] = Border;
            for (int i = 0; i < columns; i++)
                aiColumnX[i + 1] = aiColumnX[i] +
                                   (int)
                                   Math.Max(g.MeasureString(asContent[i], fontInfo).Width,
                                            g.MeasureString(asTitles[i], fontInfo).Width) + 4;

            szPrice = g.MeasureString("9.9999", fontInfo).ToSize();

            g.Dispose();
        }

        /// <summary>
        ///     Resizes the form.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetBtnNavigate();
            for (int bar = Data.FirstBar; bar < Data.Bars; bar++)
                if (Backtester.WayPoints(bar) > maxWayPoints)
                    maxWayPoints = Backtester.WayPoints(bar);

            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int clientSizeWidth = (Math.Max(aiColumnX[columns] + 2*btnHrzSpace, 550));
            ClientSize = new Size(clientSizeWidth, 310 + infoRowHeight*(maxWayPoints + 2));
        }

        /// <summary>
        ///     Arrange the controls.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var verticalSpace = (int) (Data.VerticalDLU*5.5);
            var hrzSpace = (int) (Data.HorizontalDLU*3);
            int space = hrzSpace;

            int width = ClientSize.Width - 2*space;

            nudGo.Size = new Size(65, buttonHeight);
            nudGo.Location = new Point(space, ClientSize.Height - buttonHeight - verticalSpace + 3);

            btnGo.Size = new Size(65, buttonHeight);
            btnGo.Location = new Point(nudGo.Right + hrzSpace, ClientSize.Height - buttonHeight - verticalSpace);

            //Button Close
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - hrzSpace,
                                          ClientSize.Height - buttonHeight - verticalSpace);

            int btnNavigateWidth = buttonWidth*2/5;
            int btnNavigateSpace = hrzSpace*2/3;
            int position = btnGo.Right + ((btnClose.Left - btnGo.Right) - (6*btnNavigateWidth + 5*btnNavigateSpace))/2;
            for (int btn = 0; btn < buttonsNavigate.Length; btn++)
            {
                buttonsNavigate[btn].Size = new Size(btnNavigateWidth, buttonHeight);
                buttonsNavigate[btn].Location = new Point(position + (btnNavigateWidth + btnNavigateSpace)*btn,
                                                          ClientSize.Height - buttonHeight - verticalSpace);
            }

            pnlInfo.Size = new Size(width, infoRowHeight*(maxWayPoints + 2));
            pnlInfo.Location = new Point(space, btnClose.Top - verticalSpace - pnlInfo.Height);
            pnlChart.Location = new Point(space, space);
            pnlChart.Size = new Size(width, pnlInfo.Top - 2*space);

            // Scales the columns position
            double scale = (double) pnlInfo.Width/aiColumnX[columns];
            for (int i = 0; i <= columns; i++)
                aiX[i] = (int) (aiColumnX[i]*scale);
        }

        /// <summary>
        ///     Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Navigate to a bar on mouse wheel.
        /// </summary>
        private void BarExplorerMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && buttonsNavigate[3].Enabled)
                Navigate(">");
            else if (e.Delta < 0 && buttonsNavigate[2].Enabled)
                Navigate("<");
        }

        /// <summary>
        ///     Navigate to a bar on button click.
        /// </summary>
        private void BtnNavigateClick(object sender, EventArgs e)
        {
            Navigate(((Button) sender).Name);
        }

        /// <summary>
        ///     Navigate to a bar on button KeyUp.
        /// </summary>
        private void BtnNavigateKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.PageUp)
                Navigate(">max");
            else if (e.KeyCode == Keys.PageUp)
                Navigate(">>");
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.PageDown)
                Navigate("<max");
            else if (e.KeyCode == Keys.PageDown)
                Navigate("<<");
        }

        /// <summary>
        ///     Navigates to a bar.
        /// </summary>
        private void Navigate(string sDir)
        {
            switch (sDir)
            {
                case "< !":
                    for (int i = barCurrent - 1; i >= Data.FirstBar; i--)
                        if (Backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                        {
                            barCurrent = i;
                            break;
                        }
                    break;
                case "! >":
                    for (int i = barCurrent + 1; i < Data.Bars; i++)
                        if (Backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                        {
                            barCurrent = i;
                            break;
                        }
                    break;
                case "<<":
                    for (int i = barCurrent - 1; i >= Data.FirstBar; i--)
                        if (Backtester.SummaryTrans(i) != Transaction.Transfer &&
                            Backtester.SummaryTrans(i) != Transaction.None)
                        {
                            barCurrent = i;
                            break;
                        }
                    break;
                case ">>":
                    for (int i = barCurrent + 1; i < Data.Bars; i++)
                        if (Backtester.SummaryTrans(i) != Transaction.Transfer &&
                            Backtester.SummaryTrans(i) != Transaction.None)
                        {
                            barCurrent = i;
                            break;
                        }
                    break;
                case "<max":
                    int maxWp = 0;
                    int maxBar = barCurrent;
                    for (int i = barCurrent - 1; i >= Data.FirstBar; i--)
                        if (Backtester.WayPoints(i) > maxWp)
                        {
                            maxWp = Backtester.WayPoints(i);
                            maxBar = i;
                        }
                    barCurrent = maxBar;
                    break;
                case ">max":
                    maxWp = 0;
                    maxBar = barCurrent;
                    for (int i = barCurrent + 1; i < Data.Bars; i++)
                        if (Backtester.WayPoints(i) > maxWp)
                        {
                            maxWp = Backtester.WayPoints(i);
                            maxBar = i;
                        }
                    barCurrent = maxBar;
                    break;
                case "<":
                    if (barCurrent > Data.FirstBar)
                        barCurrent--;
                    break;
                case ">":
                    if (barCurrent < Data.Bars - 1)
                        barCurrent++;
                    break;
                case "Go":
                    barCurrent = (int) nudGo.Value - 1;
                    break;
            }

            SetBtnNavigate();

            barInfo = Language.T("Bar") + ": " + (barCurrent + 1) +
                      " " + Data.Time[barCurrent].ToString(Data.DF) +
                      " " + Data.Time[barCurrent].ToString("HH:mm") + "; " +
                      Language.T("Interpolation method") + ": " +
                      Backtester.InterpolationMethodToString();

            var rectPnlChart = new Rectangle(Border, infoRowHeight, pnlChart.ClientSize.Width - 2*Border,
                                             pnlChart.ClientSize.Height - infoRowHeight - Border);
            pnlChart.Invalidate(rectPnlChart);

            var rectPnlInfo = new Rectangle(Border, 2*infoRowHeight, pnlInfo.ClientSize.Width - 2*Border,
                                            pnlInfo.ClientSize.Height - 2*infoRowHeight - Border);
            pnlInfo.Invalidate(rectPnlInfo);

            nudGo.Value = barCurrent + 1;
        }

        /// <summary>
        ///     Sets the navigation buttons
        /// </summary>
        private void SetBtnNavigate()
        {
            // Buttons "Ambiguous"
            if (Backtester.AmbiguousBars > 0)
            {
                bool isButtonAmbiguous = false;
                for (int i = Data.FirstBar; i < barCurrent; i++)
                    if (Backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                buttonsNavigate[0].Enabled = isButtonAmbiguous;

                isButtonAmbiguous = false;
                for (int i = barCurrent + 1; i < Data.Bars; i++)
                    if (Backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                buttonsNavigate[5].Enabled = isButtonAmbiguous;
            }

            // Buttons "Deals"
            if (Backtester.PositionsTotal > 0)
            {
                bool isButtonDeal = false;
                for (int i = Data.FirstBar; i < barCurrent; i++)
                    if (Backtester.Positions(i) > 0)
                    {
                        isButtonDeal = true;
                        break;
                    }
                buttonsNavigate[1].Enabled = isButtonDeal;

                isButtonDeal = false;
                for (int i = barCurrent + 1; i < Data.Bars; i++)
                    if (Backtester.Positions(i) > 0)
                    {
                        isButtonDeal = true;
                        break;
                    }
                buttonsNavigate[4].Enabled = isButtonDeal;
            }

            buttonsNavigate[2].Enabled = barCurrent > Data.FirstBar;
            buttonsNavigate[3].Enabled = barCurrent < Data.Bars - 1;

            buttonsNavigate[0].ForeColor = buttonsNavigate[0].Enabled ? Color.Red : buttonsNavigate[2].ForeColor;
            buttonsNavigate[5].ForeColor = buttonsNavigate[5].Enabled ? Color.Red : buttonsNavigate[2].ForeColor;
        }

        /// <summary>
        ///     Paints panel pnlChart
        /// </summary>
        private void PnlChartPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorChartBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            int width = pnl.ClientSize.Width;

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(width, infoRowHeight);
            var rectCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Alignment = StringAlignment.Center
                };
            string stringCaptionText = Language.T("Price Route Inside the Bar");
            rectCaption = new RectangleF(Border, 0, pnl.ClientSize.Width - 2*Border, infoRowHeight);
            g.DrawString(stringCaptionText, fontInfo, brushCaptionText, rectCaption, stringFormat);

            // Paint the panel background
            var rectClient = new RectangleF(0, infoRowHeight, pnl.ClientSize.Width, pnl.Height - infoRowHeight);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Paint bar info
            var rectBarInfo = new RectangleF(Border, infoRowHeight + 1, pnl.ClientSize.Width - 2*Border, infoRowHeight);
            g.DrawString(barInfo, fontInfo, brushGridText, rectBarInfo, stringFormat);

            // Searching the min and the max price and volume
            width = pnl.ClientSize.Width - 2*Border;
            double maxPrice = Data.High[barCurrent];
            double minPrice = Data.Low[barCurrent];
            const int space = 8;
            int spcRight = szPrice.Width + 4;
            const int xLeft = Border + space;
            int xRight = width - spcRight;
            int yTop = 2*infoRowHeight + 6;
            int yBottom = pnl.ClientSize.Height - 22;
            int barPixels = maxWayPoints < 10 ? 28 : maxWayPoints < 15 ? 24 : 20;
            const int spcLeft = 3;
            int x = barPixels + spcLeft;

            int pointLeft = x + barPixels + 30;
            int pointRight = xRight - 20;
            int points = Backtester.WayPoints(barCurrent);
            const int pointRadius = 3;

            // Grid
            var iCntLabels = (int) Math.Max((yBottom - yTop)/30d, 1);
            double deltaPoint = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3)
                                    ? Data.InstrProperties.Point*100
                                    : Data.InstrProperties.Point*10;
            double delta =
                Math.Max(Math.Round((maxPrice - minPrice)/iCntLabels, Data.InstrProperties.Point < 0.001 ? 3 : 1),
                         deltaPoint);

            minPrice = Math.Round(minPrice, Data.InstrProperties.Point < 0.001f ? 3 : 1) - Data.InstrProperties.Point*10;
            minPrice -= delta;
            maxPrice += delta;
            iCntLabels = (int) Math.Ceiling((maxPrice - minPrice)/delta);
            maxPrice = minPrice + iCntLabels*delta;

            double scaleY = (yBottom - yTop)/(iCntLabels*delta);
            var yOpen = (int) (yBottom - (Data.Open[barCurrent] - minPrice)*scaleY);
            var yHigh = (int) (yBottom - (Data.High[barCurrent] - minPrice)*scaleY);
            var yLow = (int) (yBottom - (Data.Low[barCurrent] - minPrice)*scaleY);
            var yClose = (int) (yBottom - (Data.Close[barCurrent] - minPrice)*scaleY);

            // Find the price distance
            double priceDistance = 0;
            for (int point = 1; point < points; point++)
            {
                priceDistance +=
                    Math.Abs(Backtester.WayPoint(barCurrent, point).Price -
                             Backtester.WayPoint(barCurrent, point - 1).Price);
            }
            double dPriceForAPixel = (pointRight - pointLeft)/priceDistance;

            // Points X
            var aiPointX = new int[points];
            aiPointX[0] = pointLeft;
            for (int point = 1; point < points - 1; point++)
            {
                var iDistance =
                    (int)
                    (Math.Abs(Backtester.WayPoint(barCurrent, point).Price -
                              Backtester.WayPoint(barCurrent, point - 1).Price)*dPriceForAPixel);
                aiPointX[point] = aiPointX[point - 1] + iDistance;
            }
            aiPointX[points - 1] = pointRight;
            for (int point = 1; point < points - 1; point++)
            {
                if (aiPointX[point] - aiPointX[point - 1] < barPixels + 1)
                    aiPointX[point] = aiPointX[point - 1] + barPixels + 1;
            }
            for (int point = points - 2; point > 0; point--)
            {
                if (aiPointX[point + 1] - aiPointX[point] < barPixels + 1)
                    aiPointX[point] = aiPointX[point + 1] - barPixels - 1;
            }

            // Find coordinates of the Way Points
            var pntWay = new Point[points];
            for (int point = 0; point < points; point++)
            {
                var pointY = (int) (yBottom - (Backtester.WayPoint(barCurrent, point).Price - minPrice)*scaleY);
                pntWay[point] = new Point(aiPointX[point], pointY);
            }

            // Horizontal grid and Price labels
            for (double label = minPrice; label <= maxPrice + Data.InstrProperties.Point; label += delta)
            {
                var labelY = (int) (yBottom - (label - minPrice)*scaleY);
                g.DrawString(label.ToString(Data.FF), Font, brushGridText, xRight, labelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, Border + space, labelY, xRight, labelY);
            }

            // Vertical Grid
            g.DrawLine(penGrid, x + barPixels/2 - 1, yTop, x + barPixels/2 - 1, yBottom + 2);
            for (int point = 0; point < points; point++)
            {
                var pt1 = new Point(pntWay[point].X, yTop);
                var pt2 = new Point(pntWay[point].X, yBottom + 2);
                var pt3 = new Point(pntWay[point].X - 5, yBottom + 4);
                g.DrawLine(penGrid, pt1, pt2);
                g.DrawString((point + 1).ToString(CultureInfo.InvariantCulture), Font, brushGridText, pt3);
            }

            // Bar Number
            string barNumber = (barCurrent + 1).ToString(CultureInfo.InvariantCulture);
            int stringX = x + barPixels/2 - 1 - g.MeasureString(barNumber, Font).ToSize().Width/2;
            Brush barBrush = Backtester.BackTestEval(barCurrent) == BacktestEval.Ambiguous ? brushRed : brushGridText;
            g.DrawString(barNumber, Font, barBrush, stringX, yBottom + 4);

            // Draw the bar
            g.DrawLine(penBarBorder, x + barPixels/2 - 1, yLow, x + barPixels/2 - 1, yHigh);
            if (yClose < yOpen) // White bar
            {
                var rect = new Rectangle(x, yClose, barPixels - 2, yOpen - yClose);
                var lgBrush = new LinearGradientBrush(rect, colorBarWight1, colorBarWight2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(penBarBorder, x, yClose, barPixels - 2, yOpen - yClose);
            }
            else if (yClose > yOpen) // Black bar
            {
                var rect = new Rectangle(x, yOpen, barPixels - 2, yClose - yOpen);
                var lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(penBarBorder, x, yOpen, barPixels - 2, yClose - yOpen);
            }
            else // Cross
            {
                g.DrawLine(penBarBorder, x, yClose, x + barPixels - 2, yClose);
            }

            // Draw cancelled orders
            for (int orderIndex = 0; orderIndex < Backtester.Orders(barCurrent); orderIndex++)
            {
                int ordNumber = Backtester.OrdNumb(barCurrent, orderIndex);
                Order order = Backtester.OrdFromNumb(ordNumber);
                if (order.OrdStatus != OrderStatus.Cancelled)
                    continue;

                if (order.OrdPrice > Data.High[barCurrent] || order.OrdPrice < Data.Low[barCurrent])
                    continue;

                int d = barPixels/2 - 1;
                int x1 = x + d;
                int x2 = x + barPixels - 2;
                var yDeal = (int) (yBottom - (order.OrdPrice - minPrice)*scaleY);
                var pen = new Pen(LayoutColors.ColorChartGrid, 2);

                if (order.OrdDir == OrderDirection.Buy)
                {
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                    g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                    g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                }
                else if (order.OrdDir == OrderDirection.Sell)
                {
                    g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                    g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                    g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                }
            }

            // Draw the deals on the bar
            for (int pos = 0; pos < Backtester.Positions(barCurrent); pos++)
            {
                if (Backtester.PosTransaction(barCurrent, pos) == Transaction.Transfer)
                    continue;

                var yDeal = (int) (yBottom - (Backtester.PosOrdPrice(barCurrent, pos) - minPrice)*scaleY);

                if (Backtester.PosDir(barCurrent, pos) == PosDirection.Long ||
                    Backtester.PosDir(barCurrent, pos) == PosDirection.Short)
                {
                    int d = barPixels/2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 2;
                    if (Backtester.OrdFromNumb(Backtester.PosOrdNumb(barCurrent, pos)).OrdDir == OrderDirection.Buy)
                    {
                        // Buy
                        var pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                    }
                    else
                    {
                        // Sell
                        var pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                    }
                }
                else if (Backtester.PosDir(barCurrent, pos) == PosDirection.Closed)
                {
                    // Close position
                    int d = barPixels/2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 3;
                    var pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                    g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                }
            }

            // Draw position lots
            for (int point = 0; point < points; point++)
            {
                int posNumber = Backtester.WayPoint(barCurrent, point).PosNumb;
                if (posNumber == -1) continue;

                Position position = Backtester.PosFromNumb(posNumber);
                double posLots = position.PosLots;
                PosDirection posDirection = position.PosDir;
                WayPointType wpType = Backtester.WayPoint(barCurrent, point).WPType;

                var height = (int) (Math.Max(posLots*2, 2));
                int length = barPixels;
                int posX = pntWay[point].X - (barPixels - 1)/2;
                int posY = yBottom - height;

                if (point < points - 1)
                    length = pntWay[point + 1].X - pntWay[point].X + 1;

                if (posDirection == PosDirection.Long)
                {
                    // Long
                    var rect = new Rectangle(posX - 1, posY - 1, length, height + 2);
                    var lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX - 1, posY, length, height);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Short)
                {
                    // Short
                    var rect = new Rectangle(posX - 1, posY - 1, length, height + 2);
                    var lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX - 1, posY, length, height);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Closed && wpType == WayPointType.Exit)
                {
                    // Closed
                    var rect = new Rectangle(posX - 1, yBottom - 2, barPixels + 1, 2);
                    var lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX, yBottom - 2, barPixels - 1, 2);
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Draw the Beziers
            for (int point = 1; point < points; point++)
            {
                Point ptKnot1 = pntWay[point - 1];
                Point ptKnot2 = pntWay[point];

                int ctrlX1 = (ptKnot1.X + ptKnot2.X)/2;
                int ctrlX2 = (ptKnot1.X + ptKnot2.X)/2;

                int ctrlY1 = ptKnot1.Y;
                int ctrlY2 = ptKnot2.Y;

                if (point > 1)
                {
                    if (pntWay[point - 2].Y > pntWay[point - 1].Y && pntWay[point - 1].Y > pntWay[point].Y ||
                        pntWay[point - 2].Y < pntWay[point - 1].Y && pntWay[point - 1].Y < pntWay[point].Y)
                    {
                        ctrlY1 = (pntWay[point - 1].Y + pntWay[point].Y)/2;
                    }
                }
                if (point < points - 1)
                {
                    if (pntWay[point - 1].Y > pntWay[point].Y && pntWay[point].Y > pntWay[point + 1].Y ||
                        pntWay[point - 1].Y < pntWay[point].Y && pntWay[point].Y < pntWay[point + 1].Y)
                    {
                        ctrlY2 = (pntWay[point - 1].Y + pntWay[point].Y)/2;
                    }
                }

                if (point == 1)
                {
                    ctrlX1 = ptKnot1.X;
                    ctrlY1 = ptKnot1.Y;
                }
                if (point == points - 1)
                {
                    ctrlX2 = ptKnot2.X;
                    ctrlY2 = ptKnot2.Y;
                }

                var ptControl1 = new Point(ctrlX1, ctrlY1);
                var ptControl2 = new Point(ctrlX2, ctrlY2);

                g.DrawBezier(penCross, ptKnot1, ptControl1, ptControl2, ptKnot2);
            }

            // Draw the WayPoints
            Brush brushWeyPnt = new SolidBrush(LayoutColors.ColorChartBack);
            for (int point = 0; point < points; point++)
            {
                g.FillEllipse(brushWeyPnt, pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2*pointRadius,
                              2*pointRadius);
                g.DrawEllipse(penCross, pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2*pointRadius,
                              2*pointRadius);
            }

            // Draw O, H, L, C labels
            for (int point = 0; point < points; point++)
            {
                WayPointType wpType = Backtester.WayPoint(barCurrent, point).WPType;
                if (wpType != WayPointType.Open && wpType != WayPointType.High &&
                    wpType != WayPointType.Low && wpType != WayPointType.Close)
                    continue;

                string label = "?";
                switch (wpType)
                {
                    case WayPointType.Open:
                        label = "O";
                        break;
                    case WayPointType.High:
                        label = "H";
                        break;
                    case WayPointType.Low:
                        label = "L";
                        break;
                    case WayPointType.Close:
                        label = "C";
                        break;
                }

                int xPoint = pntWay[point].X;
                int yPoint = pntWay[point].Y - Font.Height - 3;

                var stringFormatLabel = new StringFormat {Alignment = StringAlignment.Center};
                g.DrawString(label, Font, brushGridText, xPoint, yPoint, stringFormatLabel);
            }

            // Draw the deals on the route
            for (int point = 0; point < points; point++)
            {
                int posNumber = Backtester.WayPoint(barCurrent, point).PosNumb;
                int ordNumber = Backtester.WayPoint(barCurrent, point).OrdNumb;

                if (posNumber < 0 || ordNumber < 0)
                    continue;

                PosDirection posDirection = Backtester.PosFromNumb(posNumber).PosDir;
                OrderDirection ordDirection = Backtester.OrdFromNumb(ordNumber).OrdDir;
                WayPointType wpType = Backtester.WayPoint(barCurrent, point).WPType;

                if (wpType == WayPointType.None || wpType == WayPointType.Open || wpType == WayPointType.High ||
                    wpType == WayPointType.Low || wpType == WayPointType.Close)
                    continue;

                int yDeal = pntWay[point].Y;

                if (posDirection == PosDirection.Long || posDirection == PosDirection.Short ||
                    wpType == WayPointType.Cancel)
                {
                    int d = barPixels/2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 2;
                    if (ordDirection == OrderDirection.Buy)
                    {
                        // Buy
                        var pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        if (wpType == WayPointType.Cancel)
                            pen = new Pen(LayoutColors.ColorChartGrid, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                    }
                    else
                    {
                        // Sell
                        var pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        if (wpType == WayPointType.Cancel)
                            pen = new Pen(LayoutColors.ColorChartGrid, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                    }
                }

                if (posDirection == PosDirection.Closed)
                {
                    // Close position
                    int d = barPixels/2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 3;
                    var pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                    g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                }
            }

            // Coordinate axes
            g.DrawLine(penAxes, xLeft, yTop - 4, xLeft, yBottom); // Vertical left line
            g.DrawLine(penAxes, xLeft, yBottom, xRight, yBottom);

            // Border
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            g.DrawLine(penBorder, 1, infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, infoRowHeight, pnl.ClientSize.Width - Border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - Border + 1);
        }

        /// <summary>
        ///     Paints panel pnlInfo
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            // ---------------------------------------------------------------------+
            // |                          Way points description                    |
            // |--------------------------------------------------------------------+
            // | Number | Description | Price | Direction | Lots | Position | Order |
            // |--------------------------------------------------------------------+
            //xp0      xp1           xp2     xp3         xp4    xp5        xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            string ff = Data.FF; // Format modifier to print the floats
            var size = new Size(aiX[columns] - aiX[0], infoRowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near};

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(pnl.ClientSize.Width, 2*infoRowHeight);
            var rectCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Alignment = StringAlignment.Near
                };
            string stringCaptionText = Language.T("Way Points Description");
            float captionWidth = Math.Min(pnlInfo.ClientSize.Width, aiX[columns] - aiX[0]);
            float fCaptionTextWidth = g.MeasureString(stringCaptionText, fontInfo).Width;
            float fCaptionTextX = Math.Max((captionWidth - fCaptionTextWidth)/2f, 0);
            var pfCaptionText = new PointF(fCaptionTextX, 0);
            var sfCaptionText = new SizeF(captionWidth - fCaptionTextX, infoRowHeight);
            rectCaption = new RectangleF(pfCaptionText, sfCaptionText);

            // First caption raw
            g.DrawString(stringCaptionText, fontInfo, brushCaptionText, rectCaption, stringFormat);

            // Second caption raw
            for (int i = 0; i < columns; i++)
                g.DrawString(asTitles[i], fontInfo, brushCaptionText, (aiX[i] + aiX[i + 1])/2f, infoRowHeight, sf);

            Brush brush = new SolidBrush(LayoutColors.ColorControlText);

            for (int pnt = 0; pnt < Backtester.WayPoints(barCurrent); pnt++)
            {
                int y = (pnt + 2)*infoRowHeight;
                var point = new Point(aiX[0], y);

                // Even row
                if (Math.Abs(pnt%2f - 0) > 0.00001)
                    g.FillRectangle(brushEvenRow, new Rectangle(point, size));

                int positionNumber = Backtester.WayPoint(barCurrent, pnt).PosNumb;
                WayPointType wpType = Backtester.WayPoint(barCurrent, pnt).WPType;
                PosDirection posDirection = Backtester.PosFromNumb(positionNumber).PosDir;
                double posLots = Backtester.PosFromNumb(positionNumber).PosLots;
                int ordNumber = Backtester.WayPoint(barCurrent, pnt).OrdNumb;

                g.DrawString((pnt + 1).ToString(CultureInfo.InvariantCulture), fontInfo, brush, (aiX[0] + aiX[1])/2f,
                             y, sf);
                g.DrawString(Language.T(WayPoint.WPTypeToString(wpType)), fontInfo, brush, aiX[1] + 2, y);
                g.DrawString(Backtester.WayPoint(barCurrent, pnt).Price.ToString(ff), fontInfo, brush,
                             (aiX[3] + aiX[2])/2f, y, sf);

                if (positionNumber > -1)
                {
                    g.DrawString(Language.T(posDirection.ToString()), fontInfo, brush, (aiX[4] + aiX[3])/2f, y, sf);
                    g.DrawString(posLots.ToString(CultureInfo.InvariantCulture), fontInfo, brush,
                                 (aiX[5] + aiX[4])/2f, y, sf);
                    g.DrawString((positionNumber + 1).ToString(CultureInfo.InvariantCulture), fontInfo, brush,
                                 (aiX[6] + aiX[5])/2f, y, sf);
                }

                if (ordNumber > -1)
                {
                    g.DrawString((ordNumber + 1).ToString(CultureInfo.InvariantCulture), fontInfo, brush,
                                 (aiX[7] + aiX[6])/2f, y, sf);
                }
            }

            // Vertical lines
            var penLine = new Pen(LayoutColors.ColorJournalLines);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLine, aiX[i], 2*infoRowHeight, aiX[i], ClientSize.Height - Border);

            // Border
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            g.DrawLine(penBorder, 1, 2*infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, 2*infoRowHeight, pnl.ClientSize.Width - Border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - Border + 1);
        }
    }
}