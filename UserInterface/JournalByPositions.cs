// Journal_Positions Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.CustomControls;

namespace Forex_Strategy_Builder
{
    public sealed class JournalByPositions : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel

        private Brush brushCaptionText;
        private Brush brushEvenRowBack;
        private Brush brushRowText;
        private Brush brushSelectedRowBack;
        private Brush brushSelectedRowText;
        private Brush brushWarningBack;
        private Brush brushWarningText;
        private Color colorBack;
        private int[] columnX; // The horizontal position of the column
        private int columns; // The number of the columns
        private int firstPos; // The number of the first shown position
        private Font font;
        private HScrollBar hScrollBar;
        private string[,] journalData; // The text journal data
        private int lastPos; // The number of the last shown position
        private Pen penBorder;
        private Pen penLines;
        private Image[] posIcons; // Shows the position's type and transaction
        private int[] posNumbers; // Contains the numbers of all positions without transferred
        private int positions; // The total number of the positions
        private int rowHeight; // The journal row height
        private int rows; // The number of rows can be shown (without the caption bar)
        private int[] scaledX; // The scaled horizontal position of the column
        private int selectedBarOld; // The number of the old selected bar
        private int selectedRow; // The number of the selected row
        private int shownPos; // How many positions are shown
        private string[] titlesInMoney; // Journal title
        private string[] titlesInPips; // Journal title
        private VScrollBar vScrollBar;

        /// <summary>
        ///     Constructor
        /// </summary>
        public JournalByPositions()
        {
            InitializeJournal();
            SetUpJournal();
            SetJournalColors();
        }

        /// <summary>
        ///     Gets the selected bar
        /// </summary>
        public int SelectedBar
        {
            get
            {
                return ShowTransfers
                           ? StatsBuffer.PosCoordinates[firstPos + selectedRow].Bar
                           : StatsBuffer.PosCoordinates[posNumbers[firstPos + selectedRow]].Bar;
            }
        }

        /// <summary>
        ///     Sets whether Journal shows transfers.
        /// </summary>
        public bool ShowTransfers { private get; set; }

        public event EventHandler SelectedBarChange;

        /// <summary>
        ///     Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            if (!StatsBuffer.IsStatsBufferValid) return;

            if (ShowTransfers)
            {
                positions = StatsBuffer.PositionsTotal;
            }
            else
            {
                posNumbers = StatsBuffer.PositionsTotal > 0 ? new int[StatsBuffer.PositionsTotal] : new int[1];
                positions = 0;
                for (int bar = 0; bar < Data.Bars; bar++)
                {
                    for (int pos = 0; pos < StatsBuffer.Positions(bar); pos++)
                    {
                        Transaction transaction = StatsBuffer.PosTransaction(bar, pos);
                        if (transaction == Transaction.None || transaction == Transaction.Transfer) continue;
                        posNumbers[positions] = StatsBuffer.PosNumb(bar, pos);
                        positions++;
                    }
                }
            }

            if (positions == 0)
            {
                firstPos = 0;
                lastPos = 0;
                shownPos = 0;
                selectedRow = 0;

                vScrollBar.Enabled = false;
            }
            else if (positions < rows)
            {
                firstPos = 0;
                lastPos = rows;
                shownPos = positions;
                selectedRow = 0;

                vScrollBar.Enabled = false;
            }
            else
            {
                vScrollBar.Enabled = true;
                vScrollBar.Maximum = positions - 1;

                firstPos = vScrollBar.Value;
                if (firstPos + rows > positions)
                {
                    lastPos = positions - 1;
                    shownPos = lastPos - firstPos + 1;
                }
                else
                {
                    shownPos = rows;
                    lastPos = firstPos + shownPos - 1;
                }
            }

            selectedRow = Math.Min(selectedRow, shownPos - 1);
            selectedRow = Math.Max(selectedRow, 0);

            UpdateJournalData();
            SetJournalColors();
        }

        /// <summary>
        ///     Sets the journal colors
        /// </summary>
        private void SetJournalColors()
        {
            colorBack = LayoutColors.ColorControlBack;
            brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            brushWarningBack = new SolidBrush(LayoutColors.ColorWarningRowBack);
            brushWarningText = new SolidBrush(LayoutColors.ColorWarningRowText);
            penLines = new Pen(LayoutColors.ColorJournalLines);
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                Border);

            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        /// <summary>
        ///     Initializes the Journal
        /// </summary>
        private void InitializeJournal()
        {
            // Horizontal ScrollBar
            hScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 100, LargeChange = 300};
            hScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            vScrollBar = new VScrollBar
                {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 4};
            vScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();
            font = new Font(Font.FontFamily, 9);

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

            titlesInPips = new[]
                {
                    Language.T("Position"),
                    Language.T("Bar"),
                    Language.T("Bar Opening Time"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Lots"),
                    Language.T("Ord Price"),
                    Language.T("Avrg Price"),
                    Language.T("Margin"),
                    Language.T("Spread"),
                    Language.T("Rollover"),
                    Language.T("Commission"),
                    Language.T("Slippage"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L"),
                    Language.T("Balance"),
                    Language.T("Equity"),
                    Language.T("Backtest")
                };

            titlesInMoney = new[]
                {
                    Language.T("Position"),
                    Language.T("Bar"),
                    Language.T("Bar Opening Time"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Amount"),
                    Language.T("Ord Price"),
                    Language.T("Avrg Price"),
                    Language.T("Margin"),
                    Language.T("Spread"),
                    Language.T("Rollover"),
                    Language.T("Commission"),
                    Language.T("Slippage"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L"),
                    Language.T("Balance"),
                    Language.T("Equity"),
                    Language.T("Backtest")
                };

            var asColumContent = new[]
                {
                    "99999",
                    "99999",
                    "12/03/08 00:00",
                    longestTransaction,
                    longestDirection,
                    "-99999999",
                    "99.99999",
                    "99.99999",
                    "999999.99",
                    "-999.99",
                    "-999.99",
                    "-999.99",
                    "-999.99",
                    "-999999.99",
                    "-999999.99",
                    "-99999999.99",
                    "-99999999.99",
                    longestBacktestEval
                };

            rowHeight = Math.Max(font.Height, 18);
            Padding = new Padding(Border, 2*rowHeight, Border, Border);

            columns = 18;
            columnX = new int[19];
            scaledX = new int[19];

            columnX[0] = Border;
            columnX[1] = columnX[0] +
                         (int)
                         Math.Max(g.MeasureString(asColumContent[0], font).Width + 16,
                                  g.MeasureString(titlesInMoney[0], font).Width) + 4;
            for (int i = 1; i < columns; i++)
                columnX[i + 1] = columnX[i] +
                                 (int)
                                 Math.Max(g.MeasureString(asColumContent[i], font).Width,
                                          g.MeasureString(titlesInMoney[i], font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        ///     Updates the journal data from the StatsBuffer.
        /// </summary>
        private void UpdateJournalData()
        {
            journalData = new string[shownPos,columns];
            posIcons = new Image[shownPos];

            for (int posIndex = firstPos; posIndex < firstPos + shownPos; posIndex++)
            {
                int posNumber = posIndex;

                if (!ShowTransfers)
                    posNumber = posNumbers[posIndex];

                int row = posIndex - firstPos;
                int bar = StatsBuffer.PosCoordinates[posNumber].Bar;
                Position position = StatsBuffer.PosFromNumb(posNumber);

                string posAmount = Configs.AccountInMoney
                                       ? (position.PosDir == PosDirection.Short ? "-" : "") +
                                         (position.PosLots*Data.InstrProperties.LotSize).ToString(
                                             CultureInfo.InvariantCulture)
                                       : position.PosLots.ToString(CultureInfo.InvariantCulture);

                string profitLoss = Configs.AccountInMoney
                                        ? position.MoneyProfitLoss.ToString("F2")
                                        : position.ProfitLoss.ToString("F2");

                string floatingPL = Configs.AccountInMoney
                                        ? position.MoneyFloatingPL.ToString("F2")
                                        : position.FloatingPL.ToString("F2");

                int p = 0;
                journalData[row, p++] = (posNumber + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, p++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, p++] = Data.Time[bar].ToString(Data.DF) + Data.Time[bar].ToString(" HH:mm");
                journalData[row, p++] = Language.T(position.Transaction.ToString());
                journalData[row, p++] = Language.T(position.PosDir.ToString());
                journalData[row, p++] = posAmount;
                journalData[row, p++] = position.FormOrdPrice.ToString(Data.FF);
                journalData[row, p++] = position.PosPrice.ToString(Data.FF);
                journalData[row, p++] = position.RequiredMargin.ToString("F2");

                // Charges
                if (Configs.AccountInMoney)
                {
                    // in currency
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reverse)
                        journalData[row, p++] = position.MoneySpread.ToString("F2");
                    else
                        journalData[row, p++] = "-";

                    journalData[row, p++] = position.Transaction == Transaction.Transfer
                                                ? position.MoneyRollover.ToString("F2")
                                                : "-";

                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Close ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        journalData[row, p++] = position.MoneyCommission.ToString("F2");
                        journalData[row, p++] = position.MoneySlippage.ToString("F2");
                    }
                    else
                    {
                        journalData[row, p++] = "-";
                        journalData[row, p++] = "-";
                    }
                }
                else
                {
                    // In pips
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reverse)
                        journalData[row, p++] = position.Spread.ToString(CultureInfo.InvariantCulture);
                    else
                        journalData[row, p++] = "-";

                    journalData[row, p++] = position.Transaction == Transaction.Transfer
                                                ? position.Rollover.ToString("F2")
                                                : "-";

                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Close ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        journalData[row, p++] = position.Commission.ToString("F2");
                        journalData[row, p++] = position.Slippage.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        journalData[row, p++] = "-";
                        journalData[row, p++] = "-";
                    }
                }

                // Profit Loss
                if (position.Transaction == Transaction.Close ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                    journalData[row, p++] = profitLoss;
                else
                    journalData[row, p++] = "-";

                // Floating Profit Loss
                if (position.PosNumb == StatsBuffer.SummaryPosNumb(bar) &&
                    position.Transaction != Transaction.Close)
                    journalData[row, p++] = floatingPL; //Last position of the bar only
                else
                    journalData[row, p++] = "-";

                // Balance / Equity
                if (Configs.AccountInMoney)
                {
                    journalData[row, p++] = position.MoneyBalance.ToString("F2");
                    journalData[row, p++] = position.MoneyEquity.ToString("F2");
                }
                else
                {
                    journalData[row, p++] = position.Balance.ToString("F2");
                    journalData[row, p++] = position.Equity.ToString("F2");
                }

                journalData[row, p] = Language.T(StatsBuffer.BackTestEvalToString(bar));

                // Icons
                posIcons[row] = Position.PositionIconImage(position.PositionIcon);
            }
        }

        /// <summary>
        ///     Set parameters on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (ClientSize.Height > 2*rowHeight + Border)
            {
                rows = (ClientSize.Height - 2*rowHeight - Border)/rowHeight;
            }
            else
            {
                rows = 0;
            }

            if (ClientSize.Width - vScrollBar.Width - 2*Border <= columnX[columns])
                columnX.CopyTo(scaledX, 0);
            else
            {
                // Scales the columns position
                float fScale = (float) (ClientSize.Width - vScrollBar.Width - 2*Border)/columnX[columns];
                for (int i = 0; i <= columns; i++)
                    scaledX[i] = (int) (columnX[i]*fScale);
            }

            if (ClientSize.Width - vScrollBar.Width - 2*Border < columnX[columns])
            {
                hScrollBar.Visible = true;
                int iPoinShort = columnX[columns] - ClientSize.Width + vScrollBar.Width + 2*Border;
                if (hScrollBar.Value > iPoinShort)
                    hScrollBar.Value = iPoinShort;
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }
            else
            {
                hScrollBar.Value = 0;
                hScrollBar.Visible = false;
            }

            UpdateButtonsLocation();
            SetUpJournal();
            if (Data.IsResult)
                Invalidate();
        }

        /// <summary>
        ///     Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int scroll = -hScrollBar.Value;
            var size = new Size(ClientSize.Width, rowHeight);
            var stringFormat = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectCaption = new RectangleF(0, 0, ClientSize.Width, 2*rowHeight);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string stringCaptionText = Language.T("Journal by Positions") +
                                       (ShowTransfers ? "" : " " + Language.T("without Transfers")) +
                                       (Configs.AccountInMoney
                                            ? " [" + Configs.AccountCurrency + "]"
                                            : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, font, brushCaptionText, new RectangleF(Point.Empty, size), stringFormat);
            g.SetClip(new RectangleF(Border, rowHeight, ClientSize.Width - 2*Border, rowHeight));
            if (Configs.AccountInMoney)
            {
                g.DrawString(titlesInMoney[0], font, brushCaptionText, scroll + (scaledX[0] + scaledX[1])/2,
                             rowHeight, stringFormat);
                for (int i = 1; i < columns; i++)
                    g.DrawString(titlesInMoney[i], font, brushCaptionText, scroll + (scaledX[i] + scaledX[i + 1])/2,
                                 rowHeight, stringFormat);
            }
            else
            {
                g.DrawString(titlesInPips[0], font, brushCaptionText, scroll + (scaledX[0] + scaledX[1])/2,
                             rowHeight, stringFormat);
                for (int i = 1; i < columns; i++)
                    g.DrawString(titlesInPips[i], font, brushCaptionText, scroll + (scaledX[i] + scaledX[i + 1])/2,
                                 rowHeight, stringFormat);
            }
            g.ResetClip();

            // Paints the journal's data field
            var rectField = new RectangleF(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*rowHeight - Border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2*Border, rowHeight);

            // Prints the journal data
            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int y = (pos - firstPos + 2)*rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs((pos - firstPos)%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                bool isWarning = false;
                if (journalData[pos - firstPos, columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(brushWarningBack, new Rectangle(point, size));
                    isWarning = true;
                }

                // Selected row
                Brush brush;
                if (pos - firstPos == selectedRow)
                {
                    g.FillRectangle(brushSelectedRowBack, new Rectangle(point, size));
                    brush = brushSelectedRowText;
                }
                else
                {
                    brush = isWarning ? brushWarningText : brushRowText;
                }

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((rowHeight - 16)/2.0);
                g.DrawImage(posIcons[pos - firstPos], scroll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(journalData[pos - firstPos, 0], font, brush, scroll + (16 + scaledX[1])/2,
                             (pos - firstPos + 2)*rowHeight, stringFormat);
                for (int i = 1; i < columns; i++)
                    g.DrawString(journalData[pos - firstPos, i], font, brush,
                                 scroll + (scaledX[i] + scaledX[i + 1])/2, (pos - firstPos + 2)*rowHeight,
                                 stringFormat);
            }

            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLines, scaledX[i] + scroll, 2*rowHeight, scaledX[i] + scroll, ClientSize.Height);

            // Border
            g.DrawLine(penBorder, 1, 2*rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, 2*rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

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
            else if (selectedRow > shownPos - 1)
                selectedRow = shownPos - 1;

            var rect = new Rectangle(0, 2*rowHeight, ClientSize.Width, ClientSize.Height - 2*rowHeight);
            Invalidate(rect);
            vScrollBar.Select();
        }

        /// <summary>
        ///     Raises the event by invoking the delegates
        /// </summary>
        private void OnSelectedBarChange(EventArgs e)
        {
            // Invokes the delegate
            if (firstPos + selectedRow < 0)
                return;

            if (SelectedBarChange == null || selectedBarOld == SelectedBar) return;
            SelectedBarChange(this, e);
            selectedBarOld = SelectedBar;
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int scrollBarWidth = hScrollBar.Visible ? hScrollBar.Height : 0;
            var rect = new Rectangle(Border, rowHeight + 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - rowHeight - scrollBarWidth - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int scrollBarWidth = hScrollBar.Visible ? hScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*rowHeight - scrollBarWidth - Border);
            Invalidate(rect);
        }
    }
}