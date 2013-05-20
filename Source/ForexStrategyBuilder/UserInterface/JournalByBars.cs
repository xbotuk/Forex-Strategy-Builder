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
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Common;
using ForexStrategyBuilder.CustomControls;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public sealed class JournalByBars : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel
        private int bars; // The total number of bars
        private int columns; // The number of the columns
        private int firstBar; // The number of the first shown bar
        private string[,] journalData; // The text journal data
        private int lastBar; // The number of the last shown bar
        private Image[] positionIcons; // Shows the position's type and transaction
        private int rowHeight; // The journal row height
        private int rows; // The number of bars can be shown (without the caption bar)
        private int selectedBarOld; // The old selected bar
        private int selectedRow; // The number of selected row
        private int shownBars; // How many bars are shown
        private string[] titlesInMoney; // Journal title second row
        private string[] titlesInPoints; // Journal title second row
        private int[] xPositions; // The horizontal position of the column
        private int[] xScaled; // The scaled horizontal position of the column

        /// <summary>
        ///     Constructor
        /// </summary>
        public JournalByBars()
        {
            InitializeJournal();
            SetUpJournal();
            UpdateJournalData();
        }

        private VScrollBar VScrollBar { get; set; }
        private HScrollBar HScrollBar { get; set; }


        /// <summary>
        ///     Gets the selected bar.
        /// </summary>
        public int SelectedBar
        {
            get { return firstBar + selectedRow; }
        }

        public event EventHandler SelectedBarChange;

        /// <summary>
        ///     Initializes the Journal
        /// </summary>
        private void InitializeJournal()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            // Horizontal ScrollBar
            HScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 100, LargeChange = 300};
            HScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            VScrollBar = new VScrollBar
                {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 4};
            VScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();
            var font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), font).Width >
                    g.MeasureString(longestDirection, font).Width)
                    longestDirection = Language.T(posDir.ToString());

            string longestTransaction = "";
            foreach (Transaction transaction in Enum.GetValues(typeof (Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), font).Width >
                    g.MeasureString(longestTransaction, font).Width)
                    longestTransaction = Language.T(transaction.ToString());

            string longestBacktestEval = "";
            foreach (BacktestEval eval in Enum.GetValues(typeof (BacktestEval)))
                if (g.MeasureString(Language.T(eval.ToString()), font).Width >
                    g.MeasureString(longestBacktestEval, font).Width)
                    longestBacktestEval = Language.T(eval.ToString());

            titlesInPoints = new[]
                {
                    Language.T("Bar"),
                    Language.T("Date"),
                    Language.T("Hour"),
                    Language.T("Open"),
                    Language.T("High"),
                    Language.T("Low"),
                    Language.T("Close"),
                    Language.T("Volume"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Lots"),
                    Language.T("Price"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L"),
                    Language.T("Balance"),
                    Language.T("Equity"),
                    Language.T("Required"),
                    Language.T("Free"),
                    Language.T("Interpolation")
                };

            titlesInMoney = new[]
                {
                    Language.T("Bar"),
                    Language.T("Date"),
                    Language.T("Hour"),
                    Language.T("Open"),
                    Language.T("High"),
                    Language.T("Low"),
                    Language.T("Close"),
                    Language.T("Volume"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Amount"),
                    Language.T("Price"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L"),
                    Language.T("Balance"),
                    Language.T("Equity"),
                    Language.T("Required"),
                    Language.T("Free"),
                    Language.T("Interpolation")
                };

            var asColumContent = new[]
                {
                    "99999",
                    "99/99/99",
                    "99:99",
                    "99.99999",
                    "99.99999",
                    "99.99999",
                    "99.99999",
                    "999999",
                    longestTransaction,
                    longestDirection,
                    "-9999999",
                    "99.99999",
                    "-9999999.99",
                    "-9999999.99",
                    "-9999999.99",
                    "-9999999.99",
                    "-9999999.99",
                    "-9999999.99",
                    longestBacktestEval
                };

            rowHeight = Math.Max(font.Height, 18);
            Padding = new Padding(Border, 2*rowHeight, Border, Border);

            columns = 19;
            xPositions = new int[20];
            xScaled = new int[20];

            xPositions[0] = Border;
            xPositions[1] = xPositions[0] +
                            (int)
                            Math.Max(g.MeasureString(asColumContent[0], font).Width + 16,
                                     g.MeasureString(titlesInMoney[0], font).Width) + 4;
            for (int i = 1; i < columns; i++)
                xPositions[i + 1] = xPositions[i] +
                                    (int)
                                    Math.Max(g.MeasureString(asColumContent[i], font).Width,
                                             g.MeasureString(titlesInMoney[i], font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        ///     Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            bars = Data.Bars;

            if (bars == 0)
            {
                firstBar = 0;
                lastBar = 0;
                shownBars = 0;

                VScrollBar.Enabled = false;
            }
            else if (bars < rows)
            {
                firstBar = 0;
                lastBar = rows;
                shownBars = bars;

                VScrollBar.Enabled = false;
            }
            else
            {
                VScrollBar.Enabled = true;
                VScrollBar.Maximum = bars - 1;

                firstBar = VScrollBar.Value;
                if (firstBar + rows > bars)
                {
                    lastBar = bars - 1;
                    shownBars = lastBar - firstBar + 1;
                }
                else
                {
                    shownBars = rows;
                    lastBar = firstBar + shownBars - 1;
                }
            }

            selectedRow = Math.Min(selectedRow, shownBars - 1);
            selectedRow = Math.Max(selectedRow, 0);

            SetButtonsColor();
        }

        /// <summary>
        ///     Updates the journal data from StatsBuffer
        /// </summary>
        public void UpdateJournalData()
        {
            journalData = new string[shownBars,columns];
            positionIcons = new Image[shownBars];

            for (int bar = firstBar; bar < firstBar + shownBars; bar++)
            {
                int row = bar - firstBar;
                int col = 0;
                bool isPos = StatsBuffer.IsPos(bar);
                bool inMoney = Configs.AccountInMoney;

                journalData[row, col++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, col++] = Data.Time[bar].ToString(Data.Df);
                journalData[row, col++] = Data.Time[bar].ToString("HH:mm");
                journalData[row, col++] = Data.Open[bar].ToString(Data.Ff);
                journalData[row, col++] = Data.High[bar].ToString(Data.Ff);
                journalData[row, col++] = Data.Low[bar].ToString(Data.Ff);
                journalData[row, col++] = Data.Close[bar].ToString(Data.Ff);
                journalData[row, col++] = Data.Volume[bar].ToString(CultureInfo.InvariantCulture);
                journalData[row, col++] = isPos ? Language.T(StatsBuffer.SummaryTrans(bar).ToString()) : "";
                journalData[row, col++] = isPos ? Language.T(StatsBuffer.SummaryDir(bar).ToString()) : "";
                journalData[row, col++] = isPos ? GetPositionAmmountString(bar) : "";
                journalData[row, col++] = isPos ? StatsBuffer.SummaryPrice(bar).ToString(Data.Ff) : "";
                journalData[row, col++] = isPos ? GetPositionProfitString(bar) : "";
                journalData[row, col++] = isPos ? GetPositionFloatingPLString(bar) : "";
                journalData[row, col++] = inMoney
                                              ? StatsBuffer.MoneyBalance(bar).ToString("F2")
                                              : StatsBuffer.Balance(bar).ToString(CultureInfo.InvariantCulture);
                journalData[row, col++] = inMoney
                                              ? StatsBuffer.MoneyEquity(bar).ToString("F2")
                                              : StatsBuffer.Equity(bar).ToString(CultureInfo.InvariantCulture);
                journalData[row, col++] = StatsBuffer.SummaryRequiredMargin(bar).ToString("F2");
                journalData[row, col++] = StatsBuffer.SummaryFreeMargin(bar).ToString("F2");
                journalData[row, col] = Language.T(StatsBuffer.BackTestEvalToString(bar));

                positionIcons[row] = isPos
                                         ? Position.PositionIconImage(StatsBuffer.SummaryPositionIcon(bar))
                                         : Resources.pos_square;
            }
        }

        private string GetPositionAmmountString(int bar)
        {
            string sign = StatsBuffer.SummaryDir(bar) == PosDirection.Short ? "-" : "";
            if (Configs.AccountInMoney)
                return sign + StatsBuffer.SummaryAmount(bar).ToString(CultureInfo.InvariantCulture);
            return StatsBuffer.SummaryLots(bar).ToString(CultureInfo.InvariantCulture);
        }

        private string GetPositionProfitString(int bar)
        {
            Transaction trans = StatsBuffer.SummaryTrans(bar);
            if (trans == Transaction.Close || trans == Transaction.Reduce || trans == Transaction.Reverse)
            {
                return Configs.AccountInMoney
                           ? StatsBuffer.MoneyProfitLoss(bar).ToString("F2")
                           : StatsBuffer.ProfitLoss(bar).ToString(CultureInfo.InvariantCulture);
            }
            return "-";
        }

        private string GetPositionFloatingPLString(int bar)
        {
            if (StatsBuffer.SummaryTrans(bar) == Transaction.Close)
                return "-";
            return Configs.AccountInMoney
                       ? StatsBuffer.MoneyFloatingPL(bar).ToString("F2")
                       : StatsBuffer.FloatingPL(bar).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Set parameters on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            SetCountOfRows();
            SetColumnsWidth();
            SetHScrollBar();
            SetUpJournal();
            if (Data.IsResult)
                UpdateJournalData();
            base.OnResize(eventargs);
            if (Data.IsResult)
                Invalidate();
        }

        private void SetButtonsColor()
        {
            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        private void SetCountOfRows()
        {
            rows = ClientSize.Height > 2*rowHeight ? (ClientSize.Height - 2*rowHeight)/rowHeight : 0;
        }

        private void SetHScrollBar()
        {
            if (ClientSize.Width - VScrollBar.Width - 2*Border < xPositions[columns])
            {
                HScrollBar.Visible = true;
                int pointShort = xPositions[columns] - ClientSize.Width + VScrollBar.Width + 2*Border;
                if (HScrollBar.Value > pointShort)
                    HScrollBar.Value = pointShort;
                HScrollBar.Maximum = pointShort + HScrollBar.LargeChange - 2;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Visible = false;
            }
        }

        private void SetColumnsWidth()
        {
            if (ClientSize.Width - VScrollBar.Width - 2*Border <= xPositions[columns])
                xPositions.CopyTo(xScaled, 0);
            else
            {
                // Scales the columns position
                float scale = (float) (ClientSize.Width - VScrollBar.Width - 2*Border)/xPositions[columns];
                for (int i = 0; i <= columns; i++)
                    xScaled[i] = (int) (xPositions[i]*scale);
            }
        }

        /// <summary>
        ///     Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ClientSize.Width == 0 || ClientSize.Height == 0) return;
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            int hScroll = -HScrollBar.Value;
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectCaption = new RectangleF(0, 0, ClientSize.Width, 2*rowHeight);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            var font = new Font(Font.FontFamily, 9);
            Color colorBack = LayoutColors.ColorControlBack;
            var brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            var brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            var brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            var brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            var brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            var brushWarningBack = new SolidBrush(LayoutColors.ColorWarningRowBack);
            var brushWarningText = new SolidBrush(LayoutColors.ColorWarningRowText);
            var penLines = new Pen(LayoutColors.ColorJournalLines);
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);

            // Print the journal caption
            string unit = Configs.AccountInMoney
                              ? " [" + Configs.AccountCurrency + "]"
                              : " [" + Language.T("points") + "]";
            string accUnit = " [" + Configs.AccountCurrency + "]";
            g.SetClip(new RectangleF(Border, 0, ClientSize.Width - 2*Border, 2*rowHeight));
            g.DrawString(Language.T("Market Data"), font, brushCaptionText, hScroll + (xScaled[8] + xScaled[0])/2, 0,
                         sf);
            g.DrawString(Language.T("Summary") + unit, font, brushCaptionText, hScroll + (xScaled[14] + xScaled[8])/2,
                         0, sf);
            g.DrawString(Language.T("Account") + unit, font, brushCaptionText, hScroll + (xScaled[16] + xScaled[14])/2,
                         0, sf);
            g.DrawString(Language.T("Margin") + accUnit, font, brushCaptionText,
                         hScroll + (xScaled[18] + xScaled[16])/2, 0, sf);
            g.DrawString(Language.T("Backtest"), font, brushCaptionText, hScroll + (xScaled[19] + xScaled[18])/2, 0,
                         sf);
            if (Configs.AccountInMoney)
            {
                for (int i = 0; i < columns; i++)
                    g.DrawString(titlesInMoney[i], font, brushCaptionText, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 rowHeight, sf);
            }
            else
            {
                for (int i = 0; i < columns; i++)
                    g.DrawString(titlesInPoints[i], font, brushCaptionText, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 rowHeight, sf);
            }
            g.ResetClip();

            var rectField = new RectangleF(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*rowHeight - Border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            var size = new Size(ClientSize.Width - VScrollBar.Width - 2*Border, rowHeight);

            // Prints the journal data
            for (int bar = firstBar; bar < firstBar + shownBars; bar++)
            {
                int y = (bar - firstBar + 2)*rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs((bar - firstBar)%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                bool isWarningRow = false;
                if (journalData[bar - firstBar, columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(brushWarningBack, new Rectangle(point, size));
                    isWarningRow = true;
                }

                // Selected row
                Brush brush;
                if (bar - firstBar == selectedRow)
                {
                    g.FillRectangle(brushSelectedRowBack, new Rectangle(point, size));
                    brush = brushSelectedRowText;
                }
                else
                {
                    brush = isWarningRow ? brushWarningText : brushRowText;
                }

                int index = bar - firstBar;

                // Draw the position icon
                int imgY = y + (int) Math.Floor((rowHeight - 16)/2.0);
                g.DrawImage(positionIcons[index], hScroll + 2, imgY, 16, 16);

                // Prints the data
                g.DrawString(journalData[index, 0], font, brush, hScroll + (16 + xScaled[1])/2, (index + 2)*rowHeight,
                             sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(journalData[index, i], font, brush, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 (index + 2)*rowHeight, sf);
            }

            // Vertical grid lines
            for (int i = 1; i < columns; i++)
            {
                if (i == 8 || i == 14 || i == 16 || i == 18)
                {
                    var rectSeparator = new RectangleF(xScaled[i] + hScroll, (float) (rowHeight/2.0), 1,
                                                       (float) (3*rowHeight/2.0));
                    Data.GradientPaint(g, rectSeparator, LayoutColors.ColorCaptionBack, -2*LayoutColors.DepthCaption);
                }
                g.DrawLine(penLines, xScaled[i] + hScroll, 2*rowHeight, xScaled[i] + hScroll, ClientSize.Height);
            }

            // Borders
            g.DrawLine(penBorder, 1, 2*rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, 2*rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);

            OnSelectedBarChange(new EventArgs());
        }

        /// <summary>
        ///     Selects a row on Mouse Up
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            selectedRow = e.Y/rowHeight - 2;

            if (selectedRow < 0)
                selectedRow = 0;
            else if (selectedRow > shownBars - 1)
                selectedRow = shownBars - 1;

            var rect = new Rectangle(0, 2*rowHeight, ClientSize.Width, ClientSize.Height - 2*rowHeight);
            Invalidate(rect);
            VScrollBar.Select();
        }

        /// <summary>
        ///     Raises the event by invoking the delegates
        /// </summary>
        private void OnSelectedBarChange(EventArgs e)
        {
            if (SelectedBarChange == null || selectedBarOld == SelectedBar) return;
            SelectedBarChange(this, e);
            selectedBarOld = SelectedBar;
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int scrollBarHeight = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - scrollBarHeight - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            UpdateJournalData();
            int scrollBarSize = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*rowHeight - scrollBarSize - Border);
            Invalidate(rect);
        }
    }
}