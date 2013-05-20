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
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public class JournalPositions : Panel
    {
        private const int Border = 2; // The width of outside border of the panel

        private int columns; // The number of the columns
        private int firstPos; // The number of the first shown positions
        private HScrollBar hScrollBar;
        private string[,] journalData; // The text journal data
        private int lastPos; // The number of the last shown positions
        private Image[] positionIcons; // Shows the position's type and transaction
        private int positions; // The total number of the positions during the bar
        private int rowHeight; // The journal row height
        private int rows; // The number of rows can be shown (without the caption bar)
        private int selectedBarOld; // The old selected bar
        private int shownPos; // How many positions are shown
        private string[] titlesMoney; // Journal title
        private string[] titlesPoints; // Journal title
        private VScrollBar vScrollBar;
        private int visibleWidth; // The visible part width of the panel
        private int[] xColumns; // The horizontal position of the column
        private int[] xScaled; // The scaled horizontal position of the column

        /// <summary>
        ///     Constructor
        /// </summary>
        public JournalPositions()
        {
            InitializeJournal();
            SetUpJournal();
        }

        /// <summary>
        ///     Sets the selected bar
        /// </summary>
        public int SelectedBar { private get; set; }

        /// <summary>
        ///     Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            SetSizes();
            UpdateJournalData();
        }

        /// <summary>
        ///     Initializes the Journal.
        /// </summary>
        private void InitializeJournal()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            var font = new Font(Font.FontFamily, 9);
            rowHeight = Math.Max(font.Height, 18);
            columns = 9;
            xColumns = new int[10];
            xScaled = new int[10];
            Padding = new Padding(Border, 2*rowHeight, Border, Border);

            // Horizontal ScrollBar
            hScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 50, LargeChange = 200};
            hScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            vScrollBar = new VScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Right,
                    TabStop = true,
                    SmallChange = 1,
                    LargeChange = 2
                };
            vScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();

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

            titlesPoints = new[]
                {
                    Language.T("Position"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Lots"),
                    Language.T("Order"),
                    Language.T("Ord Price"),
                    Language.T("Price"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L")
                };

            titlesMoney = new[]
                {
                    Language.T("Position"),
                    Language.T("Transaction"),
                    Language.T("Direction"),
                    Language.T("Amount"),
                    Language.T("Order"),
                    Language.T("Ord Price"),
                    Language.T("Price"),
                    Language.T("Profit Loss"),
                    Language.T("Floating P/L")
                };

            var asColumContent = new[]
                {
                    "99999",
                    longestTransaction,
                    longestDirection,
                    "-9999999",
                    "99999",
                    "99.99999",
                    "99.99999",
                    "-99999.99",
                    "-99999.99"
                };

            xColumns[0] = Border;
            xColumns[1] = xColumns[0] +
                          (int)
                          Math.Max(g.MeasureString(asColumContent[0], font).Width + 16,
                                   g.MeasureString(titlesMoney[0], font).Width) + 4;
            for (int i = 1; i < columns; i++)
                xColumns[i + 1] = xColumns[i] +
                                  (int)
                                  Math.Max(g.MeasureString(asColumContent[i], font).Width,
                                           g.MeasureString(titlesMoney[i], font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        ///     Updates the journal data from the backtester
        /// </summary>
        private void UpdateJournalData()
        {
            if (!Data.IsResult)
                return;

            journalData = new string[positions,columns];
            positionIcons = new Image[shownPos];

            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int row = pos - firstPos;

                journalData[row, 0] = (StatsBuffer.PosNumb(SelectedBar, pos) + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, 1] = Language.T(StatsBuffer.PosTransaction(SelectedBar, pos).ToString());
                journalData[row, 2] = Language.T(StatsBuffer.PosDir(SelectedBar, pos).ToString());
                journalData[row, 3] = Configs.AccountInMoney
                                          ? (StatsBuffer.PosDir(SelectedBar, pos) == PosDirection.Short ? "-" : "") +
                                            (StatsBuffer.PosLots(SelectedBar, pos)*Data.InstrProperties.LotSize)
                                          : StatsBuffer.PosLots(SelectedBar, pos).ToString(CultureInfo.InvariantCulture);
                journalData[row, 4] =
                    (StatsBuffer.PosOrdNumb(SelectedBar, pos) + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, 5] = StatsBuffer.PosOrdPrice(SelectedBar, pos).ToString(Data.Ff);
                journalData[row, 6] = StatsBuffer.PosPrice(SelectedBar, pos).ToString(Data.Ff);

                // Profit Loss
                if (StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Close ||
                    StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Reduce ||
                    StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Reverse)
                {
                    journalData[row, 7] = Configs.AccountInMoney
                                              ? StatsBuffer.PosMoneyProfitLoss(SelectedBar, pos).ToString("F2")
                                              : Math.Round(StatsBuffer.PosProfitLoss(SelectedBar, pos))
                                                    .ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    journalData[row, 7] = "-";
                }

                // Floating Profit Loss
                if (pos == positions - 1 && StatsBuffer.PosTransaction(SelectedBar, pos) != Transaction.Close)
                {
                    // Last bar position only
                    journalData[row, 8] = Configs.AccountInMoney
                                              ? StatsBuffer.PosMoneyFloatingPL(SelectedBar, pos).ToString("F2")
                                              : Math.Round(StatsBuffer.PosFloatingPL(SelectedBar, pos))
                                                    .ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    journalData[row, 8] = "-";
                }

                // Icons
                positionIcons[row] = Position.PositionIconImage(StatsBuffer.PosIcon(SelectedBar, pos));
            }
        }

        /// <summary>
        ///     Sets the size and position of the controls
        /// </summary>
        private void SetSizes()
        {
            positions = Data.IsResult ? StatsBuffer.Positions(SelectedBar) : 0;
            rows = ClientSize.Height > 2*rowHeight + Border ? (ClientSize.Height - 2*rowHeight - Border)/rowHeight : 0;

            if (positions == 0)
            {
                firstPos = 0;
                lastPos = 0;
                shownPos = 0;

                vScrollBar.Visible = false;
                visibleWidth = ClientSize.Width;
            }
            else if (positions < rows)
            {
                firstPos = 0;
                lastPos = rows;
                shownPos = positions;

                vScrollBar.Visible = false;
                visibleWidth = ClientSize.Width;
            }
            else
            {
                vScrollBar.Visible = true;
                if (SelectedBar != selectedBarOld)
                    vScrollBar.Value = 0;
                vScrollBar.Maximum = positions - 1;
                visibleWidth = vScrollBar.Left;

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

            if (visibleWidth <= xColumns[columns])
                xColumns.CopyTo(xScaled, 0);
            else
            {
                // Scales the columns position
                float fScale = (float) visibleWidth/xColumns[columns];
                for (int i = 0; i <= columns; i++)
                    xScaled[i] = (int) (xColumns[i]*fScale);
            }

            if (visibleWidth < xColumns[columns])
            {
                hScrollBar.Visible = true;
                int iPoinShort = xColumns[columns] - visibleWidth;
                if (hScrollBar.Value > iPoinShort)
                    hScrollBar.Value = iPoinShort;
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }
            else
            {
                hScrollBar.Value = 0;
                hScrollBar.Visible = false;
            }

            selectedBarOld = SelectedBar;
        }

        /// <summary>
        ///     Set parameters on resize.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            SetUpJournal();
            base.OnResize(eventargs);
            Invalidate();
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

            int hScroll = -hScrollBar.Value;
            var size = new Size(visibleWidth, rowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectCaption = new RectangleF(0, 0, ClientSize.Width, 2*rowHeight);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            Color colorBack = LayoutColors.ColorControlBack;
            var brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            var brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            var brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            var penLines = new Pen(LayoutColors.ColorJournalLines);
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            var font = new Font(Font.FontFamily, 9);

            // Print the journal caption
            string caption = Language.T("Positions During the Bar") +
                             (Configs.AccountInMoney
                                  ? " [" + Configs.AccountCurrency + "]"
                                  : " [" + Language.T("points") + "]");
            g.DrawString(caption, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(Border, rowHeight, ClientSize.Width - 2*Border, rowHeight));
            if (Configs.AccountInMoney)
                for (int i = 0; i < columns; i++)
                    g.DrawString(titlesMoney[i], font, brushCaptionText, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 rowHeight, sf);
            else
                for (int i = 0; i < columns; i++)
                    g.DrawString(titlesPoints[i], font, brushCaptionText, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 rowHeight, sf);
            g.ResetClip();

            // Paints the journal's data field
            var rectField = new RectangleF(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*rowHeight - Border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2*Border, rowHeight);

            // Prints the journal data
            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int row = pos - firstPos;
                int y = (row + 2)*rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs(row%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((rowHeight - 16)/2.0);
                g.DrawImage(positionIcons[pos - firstPos], hScroll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(journalData[row, 0], font, brushRowText, hScroll + (16 + xScaled[1])/2, (row + 2)*rowHeight,
                             sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(journalData[row, i], font, brushRowText, hScroll + (xScaled[i] + xScaled[i + 1])/2,
                                 (row + 2)*rowHeight, sf);
            }

            for (int i = 1; i < columns; i++)
                g.DrawLine(penLines, xScaled[i] + hScroll, 2*rowHeight, xScaled[i] + hScroll, ClientSize.Height);

            // Border
            g.DrawLine(penBorder, 1, 2*rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, 2*rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int height = hScrollBar.Visible ? hScrollBar.Height : 0;
            var rect = new Rectangle(Border, rowHeight + 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - rowHeight - height - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        ///     Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int height = hScrollBar.Visible ? hScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*rowHeight - height - Border);
            Invalidate(rect);
        }
    }
}