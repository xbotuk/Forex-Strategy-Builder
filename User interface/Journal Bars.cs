// Journal Bars
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Journal_Bars : Panel
    {
        VScrollBar _vScrollBar;
        HScrollBar _hScrollBar;

        Image[]   _positionIcons; // Shows the position's type and transaction
        string[]  _titlesInPips;  // Journal title second row
        string[]  _titlesInMoney; // Journal title second row
        string[,] _journalData;   // The text journal data

        int[] _xPositions; // The horizontal position of the column
        int[] _xScaled;    // The scaled horizontal position of the column
        int   _columns;    // The number of the columns
        int   _rowHeight;  // The journal row height
        private const int Border = 2; // The width of outside border of the panel

        int _rows;           // The number of bars can be shown (without the caption bar)
        int _bars;           // The total number of the bars
        int _firstBar;       // The number of the first shown bar
        int _lastBar;        // The number of the last shown bar
        int _shownBars;      // How many bars are shown
        int _selectedRow;    // The number of the selected row
        int _selectedBarOld; // The old selected bar

        Font  _font;
        Color _colorBack;
        Brush _brushCaptionText;
        Brush _brushEvenRowBack;
        Brush _brushWarningBack;
        Brush _brushWarningText;
        Brush _brushSelectedRowBack;
        Brush _brushSelectedRowText;
        Brush _brushRowText;
        Pen   _penLines;
        Pen   _penBorder;

        /// <summary>
        /// Gets the selected bar.
        /// </summary>
        public int SelectedBar { get { return _firstBar + _selectedRow; } }
        public event EventHandler SelectedBarChange;

        /// <summary>
        /// Constructor
        /// </summary>
        public Journal_Bars()
        {
            InitializeJournal();
            SetUpJournal();
            SetJournalColors();
        }

        /// <summary>
        /// Initializes the Journal
        /// </summary>
        void InitializeJournal()
        {
            // Horizontal ScrollBar
            _hScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 100, LargeChange = 300};
            _hScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            _vScrollBar = new VScrollBar {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 4};
            _vScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();
            _font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof(PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), _font).Width >
                    g.MeasureString(longestDirection, _font).Width)
                    longestDirection = Language.T(posDir.ToString());

            string longestTransaction = "";
            foreach (Transaction transaction in Enum.GetValues(typeof(Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), _font).Width >
                    g.MeasureString(longestTransaction, _font).Width)
                    longestTransaction = Language.T(transaction.ToString());

            string longestBacktestEval = "";
            foreach (BacktestEval eval in Enum.GetValues(typeof(BacktestEval)))
                if (g.MeasureString(Language.T(eval.ToString()), _font).Width >
                    g.MeasureString(longestBacktestEval, _font).Width)
                    longestBacktestEval = Language.T(eval.ToString());

            _titlesInPips = new[]
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

            _titlesInMoney = new[]
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

            _rowHeight = Math.Max(_font.Height, 18);
            Padding = new Padding(Border, 2 * _rowHeight, Border, Border);

            _columns    = 19;
            _xPositions = new int[20];
            _xScaled    = new int[20];

            _xPositions[0] = Border;
            _xPositions[1] = _xPositions[0] + (int)Math.Max(g.MeasureString(asColumContent[0], _font).Width + 16, g.MeasureString(_titlesInMoney[0], _font).Width) + 4;
            for (int i = 1; i < _columns; i++)
                _xPositions[i + 1] = _xPositions[i] + (int)Math.Max(g.MeasureString(asColumContent[i], _font).Width, g.MeasureString(_titlesInMoney[i], _font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            _bars = Data.Bars;

            if (_bars == 0)
            {
                _firstBar  = 0;
                _lastBar   = 0;
                _shownBars = 0;

                _vScrollBar.Enabled = false;
            }
            else if (_bars < _rows)
            {
                _firstBar  = 0;
                _lastBar   = _rows;
                _shownBars = _bars;

                _vScrollBar.Enabled = false;
            }
            else
            {
                _vScrollBar.Enabled = true;
                _vScrollBar.Maximum = _bars - 1;

                _firstBar = _vScrollBar.Value;
                if (_firstBar + _rows > _bars)
                {
                    _lastBar   = _bars - 1;
                    _shownBars = _lastBar - _firstBar + 1;
                }
                else
                {
                    _shownBars = _rows;
                    _lastBar   = _firstBar + _shownBars - 1;
                }
            }

            _selectedRow = Math.Min(_selectedRow, _shownBars - 1);
            _selectedRow = Math.Max(_selectedRow, 0);

            UpdateJournalData();
            SetJournalColors();
        }

        /// <summary>
        /// Sets the journal colors
        /// </summary>
        void SetJournalColors()
        {
            _colorBack        = LayoutColors.ColorControlBack;

            _brushCaptionText     = new SolidBrush(LayoutColors.ColorCaptionText);
            _brushEvenRowBack     = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            _brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            _brushRowText         = new SolidBrush(LayoutColors.ColorControlText);
            _brushWarningBack     = new SolidBrush(LayoutColors.ColorWarningRowBack);
            _brushWarningText     = new SolidBrush(LayoutColors.ColorWarningRowText);

            _penLines     = new Pen(LayoutColors.ColorJournalLines);
            _penBorder    = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        /// Updates the journal data from the backtester
        /// </summary>
        void UpdateJournalData()
        {
            _journalData = new string[_shownBars, _columns];
            _positionIcons = new Image[_shownBars];

            for (int bar = _firstBar; bar < _firstBar + _shownBars; bar++)
            {
                var row = bar - _firstBar;
                var col = 0;
                var isPos = Backtester.IsPos(bar);
                var inMoney = Configs.AccountInMoney;

                _journalData[row, col++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = Data.Time[bar].ToString(Data.DF);
                _journalData[row, col++] = Data.Time[bar].ToString("HH:mm");
                _journalData[row, col++] = Data.Open[bar].ToString(Data.FF);
                _journalData[row, col++] = Data.High[bar].ToString(Data.FF);
                _journalData[row, col++] = Data.Low[bar].ToString(Data.FF);
                _journalData[row, col++] = Data.Close[bar].ToString(Data.FF);
                _journalData[row, col++] = Data.Volume[bar].ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = isPos ? Language.T(Backtester.SummaryTrans(bar).ToString()) : "";
                _journalData[row, col++] = isPos ? Language.T(Backtester.SummaryDir(bar).ToString()) : "";
                _journalData[row, col++] = isPos ? GetPositionAmmountString(bar) : "";
                _journalData[row, col++] = isPos ? Backtester.SummaryPrice(bar).ToString(Data.FF) : "";
                _journalData[row, col++] = isPos ? GetPositionProfitString(bar) : "";
                _journalData[row, col++] = isPos ? GetPositionFloatingPLString(bar) : "";
                _journalData[row, col++] = inMoney ? Backtester.MoneyBalance(bar).ToString("F2") : Backtester.Balance(bar).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = inMoney ? Backtester.MoneyEquity(bar).ToString("F2") : Backtester.Equity(bar).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = Backtester.SummaryRequiredMargin(bar).ToString("F2");
                _journalData[row, col++] = Backtester.SummaryFreeMargin(bar).ToString("F2");
                _journalData[row, col++] = Language.T(Backtester.BackTestEval(bar));

                _positionIcons[row] = isPos ? Backtester.SummaryPositionIcon(bar) : Properties.Resources.pos_square;
            }
        }

        private string GetPositionAmmountString(int bar)
        {
            var sign = Backtester.SummaryDir(bar) == PosDirection.Short ? "-" : "";
            if (Configs.AccountInMoney)
                return sign + Backtester.SummaryAmount(bar).ToString(CultureInfo.InvariantCulture);
            return Backtester.SummaryLots(bar).ToString(CultureInfo.InvariantCulture);
        }

        private string GetPositionProfitString(int bar)
        {
            if (Backtester.SummaryTrans(bar) == Transaction.Close ||
                Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                Backtester.SummaryTrans(bar) == Transaction.Reverse)
            {
                return Configs.AccountInMoney
                           ? Backtester.MoneyProfitLoss(bar).ToString("F2")
                           : Backtester.ProfitLoss(bar).ToString(CultureInfo.InvariantCulture);
            }

            return "-";
        }

        private string GetPositionFloatingPLString(int bar)
        {
            if (Backtester.SummaryTrans(bar) == Transaction.Close)
                return "-";

            return Configs.AccountInMoney
                       ? Backtester.MoneyFloatingPL(bar).ToString("F2")
                       : Backtester.FloatingPL(bar).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Set parameters on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (ClientSize.Height > 2 * _rowHeight)
            {
                _rows = (ClientSize.Height - 2 * _rowHeight) / _rowHeight;
            }
            else
            {
                _rows = 0;
            }

            if (ClientSize.Width - _vScrollBar.Width - 2 * Border <= _xPositions[_columns])
                _xPositions.CopyTo(_xScaled, 0);
            else
            {   // Scales the columns position
                float scale = (float)(ClientSize.Width - _vScrollBar.Width - 2 * Border) / _xPositions[_columns];
                for (int i = 0; i <= _columns; i++)
                    _xScaled[i] = (int)(_xPositions[i] * scale);
            }

            if (ClientSize.Width - _vScrollBar.Width - 2 * Border < _xPositions[_columns])
            {
                _hScrollBar.Visible = true;
                int poinShort = _xPositions[_columns] - ClientSize.Width + _vScrollBar.Width + 2 * Border;
                if (_hScrollBar.Value > poinShort)
                    _hScrollBar.Value = poinShort;
                _hScrollBar.Maximum = poinShort + _hScrollBar.LargeChange - 2;
            }
            else
            {
                _hScrollBar.Value = 0;
                _hScrollBar.Visible = false;
            }

            SetUpJournal();
            Invalidate();
        }

        /// <summary>
        /// Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int iHScrll = -_hScrollBar.Value;
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2 * _rowHeight);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string unit = Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]";
            string accUnit = " [" + Configs.AccountCurrency + "]";
            g.SetClip(new RectangleF(Border, 0, ClientSize.Width - 2 * Border, 2 * _rowHeight));
            g.DrawString(Language.T("Market Data"), _font, _brushCaptionText, iHScrll + (_xScaled[8] + _xScaled[0]) / 2, 0, sf);
            g.DrawString(Language.T("Summary") + unit, _font, _brushCaptionText, iHScrll + (_xScaled[14] + _xScaled[8]) / 2, 0, sf);
            g.DrawString(Language.T("Account") + unit, _font, _brushCaptionText, iHScrll + (_xScaled[16] + _xScaled[14]) / 2, 0, sf);
            g.DrawString(Language.T("Margin")  + accUnit, _font, _brushCaptionText, iHScrll + (_xScaled[18] + _xScaled[16]) / 2, 0, sf);
            g.DrawString(Language.T("Backtest"), _font, _brushCaptionText, iHScrll + (_xScaled[19] + _xScaled[18]) / 2, 0, sf);
            if (Configs.AccountInMoney)
            {
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesInMoney[i], _font, _brushCaptionText, iHScrll + (_xScaled[i] + _xScaled[i + 1]) / 2, _rowHeight, sf);
            }
            else
            {
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesInPips[i], _font, _brushCaptionText, iHScrll + (_xScaled[i] + _xScaled[i + 1]) / 2, _rowHeight, sf);
            }
            g.ResetClip();

            var rectField = new RectangleF(Border, 2 * _rowHeight, ClientSize.Width - 2 * Border, ClientSize.Height - 2 * _rowHeight - Border);
            g.FillRectangle(new SolidBrush(_colorBack), rectField);

            var size = new Size(ClientSize.Width - _vScrollBar.Width - 2 * Border, _rowHeight);

            // Prints the journal data
            for (int bar = _firstBar; bar < _firstBar + _shownBars; bar++)
            {
                int y = (bar - _firstBar + 2) * _rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs((bar - _firstBar) % 2f - 0) > 0.0001)
                    g.FillRectangle(_brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                bool isWarningRow = false;
                if (_journalData[bar - _firstBar, _columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(_brushWarningBack, new Rectangle(point, size));
                    isWarningRow = true;
                }

                // Selected row
                Brush brush;
                if (bar - _firstBar == _selectedRow)
                {
                    g.FillRectangle(_brushSelectedRowBack, new Rectangle(point, size));
                    brush = _brushSelectedRowText;
                }
                else
                {
                    brush = isWarningRow ? _brushWarningText : _brushRowText;
                }

                // Draw the position icon
                int imgY = y + (int)Math.Floor((_rowHeight - 16) / 2.0);
                g.DrawImage(_positionIcons[bar - _firstBar], iHScrll + 2, imgY, 16, 16);

                // Prints the data
                g.DrawString(_journalData[bar - _firstBar, 0], _font, brush, iHScrll + (16 + _xScaled[1]) / 2, (bar - _firstBar + 2) * _rowHeight, sf);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_journalData[bar - _firstBar, i], _font, brush, iHScrll + (_xScaled[i] + _xScaled[i + 1]) / 2, (bar - _firstBar + 2) * _rowHeight, sf);
            }

            // Vertical grid lines
            for (var i = 1; i < _columns; i++)
            {
                if (i == 8 || i == 14 || i == 16 || i == 18)
                {
                    var rectfSeparator = new RectangleF(_xScaled[i] + iHScrll, _rowHeight / 2, 1, 3 * _rowHeight / 2);
                    Data.GradientPaint(g, rectfSeparator, LayoutColors.ColorCaptionBack, -2 * LayoutColors.DepthCaption);
                }
                g.DrawLine(_penLines, _xScaled[i] + iHScrll, 2 * _rowHeight, _xScaled[i] + iHScrll, ClientSize.Height);
            }

            // Borders
            g.DrawLine(_penBorder, 1, 2 * _rowHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, 2 * _rowHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            OnSelectedBarChange(new EventArgs());
        }

        /// <summary>
        /// Selects a row on Mouse Up
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _selectedRow = e.Y / _rowHeight - 2;

            if (_selectedRow < 0)
                _selectedRow = 0;
            else if(_selectedRow > _shownBars - 1)
                _selectedRow = _shownBars - 1;

            var rect = new Rectangle(0, 2 * _rowHeight, ClientSize.Width, ClientSize.Height - 2 * _rowHeight);
            Invalidate(rect);
            _vScrollBar.Select();
        }

        /// <summary>
        /// Raises the event by invoking the delegates
        /// </summary>
        protected virtual void OnSelectedBarChange(EventArgs e)
        {
            if (SelectedBarChange == null || _selectedBarOld == SelectedBar) return;
            SelectedBarChange(this, e);
            _selectedBarOld = SelectedBar;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int scrallBarSize = _hScrollBar.Visible ? _hScrollBar.Height : 0;
            var rect = new Rectangle(Border, 1, ClientSize.Width - 2 * Border, ClientSize.Height - scrallBarSize - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int scrallBarSize = _hScrollBar.Visible ? _hScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2 * _rowHeight, ClientSize.Width - 2 * Border, ClientSize.Height - 2 * _rowHeight - scrallBarSize - Border);
            Invalidate(rect);
        }
    }
}
