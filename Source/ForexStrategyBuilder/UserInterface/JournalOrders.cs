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
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public class JournalOrders : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel
        private Brush brushCaptionText;
        private Brush brushEvenRowBack;
        private Brush brushRowText;
        private Color colorBack;

        private int columns; // The number of the columns
        private int firstOrd; // The number of the first shown orders
        private Font font;
        private string[,] journalData; // The text journal data
        private int lastOrd; // The number of the last shown orders
        private Image[] orderIcons; // Shows the position's type and transaction
        private int orders; // The total number of the orders during the bar
        private Pen penBorder;
        private Pen penLines;
        private int rowHeight; // The journal row height
        private int rows; // The number of rows can be shown (without the caption bar)
        private int selectedBar; // The selected bar
        private int selectedBarOld; // The old selected bar
        private int shownOrd; // How many orders are shown
        private string[] titlesMoney; // Journal title
        private string[] titlesPoints; // Journal title
        private int visibleWidth; // The width of the panel visible part
        private int[] xColumns; // The horizontal position of the column
        private int[] xScaled; // The scaled horizontal position of the column

        /// <summary>
        ///     Constructor
        /// </summary>
        public JournalOrders()
        {
            InitializeJournal();
            SetUpJournal();
        }

        private VScrollBar VScrollBar { get; set; }
        private HScrollBar HScrollBar { get; set; }

        /// <summary>
        ///     Sets the selected bar
        /// </summary>
        public int SelectedBar
        {
            set { selectedBar = value; }
        }

        /// <summary>
        ///     Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            SetSizes();
            SetJournalColors();
            UpdateJournalData();
        }

        /// <summary>
        ///     Sets the journal colors
        /// </summary>
        private void SetJournalColors()
        {
            colorBack = LayoutColors.ColorControlBack;
            brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushRowText = new SolidBrush(LayoutColors.ColorControlText);
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
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            columns = 8;
            xColumns = new int[9];
            xScaled = new int[9];
            font = new Font(Font.FontFamily, 9);
            rowHeight = Math.Max(font.Height, 18);
            Padding = new Padding(Border, 2*rowHeight, Border, Border);

            // Horizontal ScrollBar
            HScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 50, LargeChange = 200};
            HScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            VScrollBar = new VScrollBar
                {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 2};
            VScrollBar.ValueChanged += VScrollBarValueChanged;

            var asComments = new[]
                {
                    "Exit Order to position",
                    "Exit Order to order",
                    "Take Profit to position",
                    "Take Profit to order",
                    "Trailing Stop to position",
                    "Trailing Stop to order",
                    "Permanent S/L to position",
                    "Permanent S/L to order",
                    "Permanent T/P to position",
                    "Permanent T/P to order"
                };

            Graphics g = CreateGraphics();

            string longestDirection = "";
            foreach (OrderDirection ordDir in Enum.GetValues(typeof (OrderDirection)))
                if (g.MeasureString(Language.T(ordDir.ToString()), font).Width >
                    g.MeasureString(longestDirection, font).Width)
                    longestDirection = Language.T(ordDir.ToString());

            string longestType = "";
            foreach (OrderType ordType in Enum.GetValues(typeof (OrderType)))
                if (g.MeasureString(Language.T(ordType.ToString()), font).Width >
                    g.MeasureString(longestType, font).Width)
                    longestType = Language.T(ordType.ToString());

            string longestStatus = "";
            foreach (OrderStatus ordStatus in Enum.GetValues(typeof (OrderStatus)))
                if (g.MeasureString(Language.T(ordStatus.ToString()), font).Width >
                    g.MeasureString(longestStatus, font).Width)
                    longestStatus = Language.T(ordStatus.ToString());

            string longestComment = "";
            foreach (string ordComment in asComments)
                if (g.MeasureString(Language.T(ordComment) + " 99999", font).Width >
                    g.MeasureString(longestComment, font).Width)
                    longestComment = Language.T(ordComment) + " 99999";

            titlesPoints = new[]
                {
                    Language.T("Order"),
                    Language.T("Direction"),
                    Language.T("Type"),
                    Language.T("Lots"),
                    Language.T("Price 1"),
                    Language.T("Price 2"),
                    Language.T("Status"),
                    Language.T("Comment")
                };

            titlesMoney = new[]
                {
                    Language.T("Order"),
                    Language.T("Direction"),
                    Language.T("Type"),
                    Language.T("Amount"),
                    Language.T("Price 1"),
                    Language.T("Price 2"),
                    Language.T("Status"),
                    Language.T("Comment")
                };

            var asColumContent = new[]
                {
                    "99999",
                    longestDirection,
                    longestType,
                    "-9999999",
                    "99.99999",
                    "99.99999",
                    longestStatus,
                    longestComment
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
        ///     Updates the journal data from the StatsBuffer.
        /// </summary>
        private void UpdateJournalData()
        {
            if (!Data.IsResult)
                return;

            journalData = new string[orders,columns];
            orderIcons = new Image[orders];
            int[] ordNumbers = ArrangeOrderNumbers();

            for (int ord = firstOrd; ord < firstOrd + shownOrd; ord++)
            {
                int row = ord - firstOrd;
                Order order = StatsBuffer.OrdFromNumb(ordNumbers[ord]);

                journalData[row, 0] = (order.OrdNumb + 1).ToString(CultureInfo.InvariantCulture);
                journalData[row, 1] = Language.T(order.OrdDir.ToString());
                journalData[row, 2] = Language.T(order.OrdType.ToString());
                journalData[row, 3] = Configs.AccountInMoney
                                          ? (order.OrdDir == OrderDirection.Sell ? "-" : "") +
                                            (order.OrdLots*Data.InstrProperties.LotSize)
                                          : order.OrdLots.ToString(CultureInfo.InvariantCulture);
                journalData[row, 4] = order.OrdPrice.ToString(Data.Ff);
                journalData[row, 5] = (order.OrdPrice2 > 0 ? order.OrdPrice2.ToString(Data.Ff) : "-");
                journalData[row, 6] = Language.T(order.OrdStatus.ToString());
                journalData[row, 7] = order.OrdNote;

                // Icons
                orderIcons[row] = Order.OrderIconImage(order.OrderIcon);
            }
        }

        private int[] ArrangeOrderNumbers()
        {
            var ordNumbers = new int[orders];
            int index = 0;
            for (int wayPoint = 0; wayPoint < StatsBuffer.WayPoints(selectedBar); wayPoint++)
            {
                int ordNumb = StatsBuffer.WayPoint(selectedBar, wayPoint).OrdNumb;
                WayPointType wpType = StatsBuffer.WayPoint(selectedBar, wayPoint).WpType;

                if (ordNumb == -1) continue; // There is no order
                if (ordNumb < StatsBuffer.OrdNumb(selectedBar, 0)) continue; // For a transferred position

                if (wpType == WayPointType.Add || wpType == WayPointType.Cancel ||
                    wpType == WayPointType.Entry || wpType == WayPointType.Exit ||
                    wpType == WayPointType.Reduce || wpType == WayPointType.Reverse)
                {
                    ordNumbers[index] = ordNumb;
                    index++;
                }
            }

            for (int ord = 0; ord < orders; ord++)
            {
                int ordNumb = StatsBuffer.OrdNumb(selectedBar, ord);
                bool toIncluded = true;
                for (int i = 0; i < index; i++)
                {
                    if (ordNumb != ordNumbers[i]) continue;
                    toIncluded = false;
                    break;
                }
                if (!toIncluded) continue;
                ordNumbers[index] = ordNumb;
                index++;
            }

            return ordNumbers;
        }

        /// <summary>
        ///     Sets the size and position of the controls
        /// </summary>
        private void SetSizes()
        {
            orders = Data.IsResult ? StatsBuffer.Orders(selectedBar) : 0;
            rows = ClientSize.Height > 2*rowHeight + Border
                       ? (ClientSize.Height - 2*rowHeight - Border)/rowHeight
                       : 0;

            if (orders == 0)
            {
                firstOrd = 0;
                lastOrd = 0;
                shownOrd = 0;

                VScrollBar.Visible = false;
                visibleWidth = ClientSize.Width;
            }
            else if (orders < rows)
            {
                firstOrd = 0;
                lastOrd = rows;
                shownOrd = orders;

                VScrollBar.Visible = false;
                visibleWidth = ClientSize.Width;
            }
            else
            {
                VScrollBar.Visible = true;
                if (selectedBar != selectedBarOld)
                    VScrollBar.Value = 0;
                VScrollBar.Maximum = orders - 1;
                visibleWidth = VScrollBar.Left;

                firstOrd = VScrollBar.Value;
                if (firstOrd + rows > orders)
                {
                    lastOrd = orders - 1;
                    shownOrd = lastOrd - firstOrd + 1;
                }
                else
                {
                    shownOrd = rows;
                    lastOrd = firstOrd + shownOrd - 1;
                }
            }

            if (visibleWidth <= xColumns[columns])
                xColumns.CopyTo(xScaled, 0);
            else
            {
                // Scales the columns position
                float scale = (float) visibleWidth/xColumns[columns];
                for (int i = 0; i <= columns; i++)
                    xScaled[i] = (int) (xColumns[i]*scale);
            }

            if (visibleWidth < xColumns[columns])
            {
                HScrollBar.Visible = true;
                int pointShort = xColumns[columns] - visibleWidth;
                if (HScrollBar.Value > pointShort)
                    HScrollBar.Value = pointShort;
                HScrollBar.Maximum = pointShort + HScrollBar.LargeChange - 2;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Visible = false;
            }

            selectedBarOld = selectedBar;
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

            // Caption background
            var rectCaption = new RectangleF(0, 0, ClientSize.Width, 2*rowHeight);
            Data.GradientPaint(g, rectCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            int hScroll = -HScrollBar.Value;
            //var size = new Size(ContextButtonLocation.X, rowHeight);
            var size = new Size(ClientSize.Width, rowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Print the journal caption
            string stringCaptionText = Language.T("Orders During the Bar") +
                                       (Configs.AccountInMoney
                                            ? " [" + Configs.AccountCurrency + "]"
                                            : " [" + Language.T("points") + "]");
            g.DrawString(stringCaptionText, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
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

            size = new Size(ClientSize.Width - VScrollBar.Width - 2*Border, rowHeight);

            // Prints the journal data
            for (int ord = firstOrd; ord < firstOrd + shownOrd; ord++)
            {
                int row = ord - firstOrd;
                if (journalData == null || journalData[row, 0] == null)
                {
                    Console.WriteLine("Break");
                    break;
                }

                int y = (row + 2)*rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs(row%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((rowHeight - 16)/2.0);
                g.DrawImage(orderIcons[row], hScroll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(journalData[row, 0], font, brushRowText, hScroll + (16 + xScaled[1])/2,
                             (row + 2)*rowHeight, sf);
                for (int i = 1; i < columns; i++)
                {
                    if (i == columns - 1)
                        g.DrawString(journalData[row, i], font, brushRowText, hScroll + xScaled[i],
                                     (row + 2)*rowHeight);
                    else
                        g.DrawString(journalData[row, i], font, brushRowText,
                                     hScroll + (xScaled[i] + xScaled[i + 1])/2, (row + 2)*rowHeight, sf);
                }
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
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
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
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*rowHeight - height - Border);
            Invalidate(rect);
        }
    }
}