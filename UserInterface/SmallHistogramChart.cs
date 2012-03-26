// Small Histogram Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.
//
// Contributed by Krog.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.CustomControls;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Draws a small histogram chart
    /// </summary>
    public class SmallHistogramChart : ContextPanel
    {
        private const int Border = 2;
        private const int Space = 5;

        private SortableDictionary<int, HistogramData> _chartData;

        private Brush _brushFore;
        private float _captionHeight;
        private int _chartBars;

        private int _countLabelsX;
        private int _countLabelsY;
        private float _deltaX;
        private float _deltaY;
        private Font _font;
        private bool _isCounts = true;
        private bool _isNotPaint;
        private bool _isShowDynamicInfo;

        private int _labelWidthX;
        private int _labelWidthY;
        private Pen _penBorder;
        private Pen _penGrid;
        private RectangleF _rectSubHeader;
        private RectangleF _rectfCaption;
        private StringFormat _sfCaption;
        private int _stepX;
        private int _stepY;
        private string _strChartTitle;
      
        private int _xAxisMax;
        private int _xAxisMin;
        private int _xAxisMin10;
        private int _xAxisY;
        private int _xLeft;
        private int _xRight;
        private float _xScale;
        private int _yAxisMax;
        private int _yAxisMin;
        private int _yBottom;
        private float _yScale;
        private int _yTop;

        private int _minIndex;
        private int _maxIndex;
        private int _maxCount;
        private double _maxTotal;
        private double _maxAbsTotal;

        /// <summary>
        /// Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo
        {
            set { _isShowDynamicInfo = value; }
        }

        /// <summary>
        /// Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get; private set; }

        /// <summary>
        /// Calculates Data to draw in histogram
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
                    bool isIndex = _chartData.ContainsKey(index);
                    int count = isIndex ? _chartData[index].TradesCount + 1 : 1;
                    double total = isIndex ? _chartData[index].TotalResult + result : result;
                    var data = new HistogramData {TradesCount = count, Result = result, TotalResult = total};

                    if (isIndex)
                        _chartData[index] = data;
                    else
                        _chartData.Add(index, data);

                    SetMinMaxValues(index, count, total);
                }
            }

            _chartData.Sort();
        }

        private void InitChartData()
        {
            _chartData = new SortableDictionary<int, HistogramData>();
            _minIndex = int.MaxValue;
            _maxIndex = int.MinValue;
            _maxCount = 0;
            _maxAbsTotal = 0;
            _maxTotal = double.MinValue;
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
            if (_minIndex > index)
                _minIndex = index;
            if (_maxIndex < index)
                _maxIndex = index;
            if (_maxCount < count)
                _maxCount = count;
            if (_maxTotal < total)
                _maxTotal = total;
            if (_maxAbsTotal < Math.Abs(total))
                _maxAbsTotal = Math.Abs(total);
        }

        /// <summary>
        /// Returns histogram data as a CSV string.
        /// </summary>
        private string GetHistogramDataString()
        {
            var sb = new StringBuilder();

            foreach (var data in _chartData)
                sb.AppendLine(data.Key + "\t" + data.Value.TradesCount + "\t" + data.Value.Result.ToString("F2") + "\t" +
                              data.Value.TotalResult.ToString("F2"));

            return sb.ToString();
        }


        /// <summary>
        /// Sets chart's instrument and back testing data.
        /// </summary>
        public void SetChartData()
        {
            _isNotPaint = !Data.IsData || !Data.IsResult || Data.Bars <= StatsBuffer.FirstBar;

            if (_isNotPaint) return;

            CalculateHistogramData();

            // set to 0 and length for X Axis
            _chartBars = _maxIndex - _minIndex + 2;

            // Min set to 0 -- will always be 0 or higher
            _yAxisMin = 0;
            _yAxisMax = _isCounts ? _maxCount : (int)_maxAbsTotal;

            // for X Axis labels
            // set minimum and maximum to indexes, expanded by 1 for border above and under drawn line
            _xAxisMax = _maxIndex + 1;
            _xAxisMin = _minIndex - 1;

            // if there are no trades for histogram, set Maxes to arbitrary values so chart draws and avoid errors
            if (_chartBars == 3)
                _chartBars = 51;
            if (_xAxisMax == 0)
                _xAxisMax = 51;

            // way to sync all X labels to multiples of 10
            _xAxisMin10 = (_xAxisMin < 0) ? (int) Math.Ceiling(_xAxisMin/10f)*10 : (int) Math.Floor(_xAxisMin/10f)*10;
        }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart()
        {
            // Chart Title
            _strChartTitle = Language.T("Trade Distribution Chart");
            _font = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_font.Height, 18);
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _rectSubHeader = new RectangleF(0, _captionHeight, ClientSize.Width, _captionHeight);

            _sfCaption = new StringFormat
                             {
                                 Alignment = StringAlignment.Center,
                                 LineAlignment = StringAlignment.Center,
                                 Trimming = StringTrimming.EllipsisCharacter,
                                 FormatFlags = StringFormatFlags.NoWrap
                             };

            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _penGrid = new Pen(LayoutColors.ColorChartGrid) {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            _penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);

            if (_isNotPaint) return;

            _yTop = (int) (2*_captionHeight + Space + 1);
            _yBottom = ClientSize.Height - 2*Space - 1 - Border;
            _xAxisY = _yBottom - 3 - _font.Height;

            Graphics g = CreateGraphics();
            _labelWidthY = (int)
                Math.Max(g.MeasureString(_yAxisMin.ToString(CultureInfo.InvariantCulture), Font).Width,
                         g.MeasureString(_yAxisMax.ToString(CultureInfo.InvariantCulture), Font).Width);
            _labelWidthX = (int)
                Math.Max(g.MeasureString(_xAxisMin.ToString(CultureInfo.InvariantCulture), Font).Width,
                         g.MeasureString(_xAxisMax.ToString(CultureInfo.InvariantCulture), Font).Width);
            g.Dispose();
            _labelWidthY = Math.Max(_labelWidthY, 30);
            _labelWidthX += 3;

            _xLeft = Border + Space;
            _xRight = ClientSize.Width - Border - Space - _labelWidthY;
            _xScale = (_xRight - 2*Space - Border)/(float) _chartBars;

            _countLabelsX = Math.Min((_xRight - _xLeft)/_labelWidthX, 20);
            _deltaX = (float) Math.Max(Math.Round((_xAxisMax - _xAxisMin)/(float) _countLabelsX), 10);
            _stepX = (int) Math.Ceiling(_deltaX/10)*10;
            _countLabelsX = (int) Math.Ceiling((_xAxisMax - _xAxisMin)/(float) _stepX);
            _xAxisMax = _xAxisMin + _countLabelsX*_stepX;

            // difference from Y Axis for Small_Balance_Chart:
            // prefer minimums because histogram counts are usually small, less than 10
            _countLabelsY = Math.Min((_xAxisY - _yTop)/20, 20);
            _deltaY = (float) Math.Round((_yAxisMax - _yAxisMin)/(float) _countLabelsY);
            // protect against deltaY infinity and stepY = Number.min
            _stepY = (float.IsInfinity(_deltaY)) ? 20 : (int) _deltaY;
            // protect against dividing by zero in case of no counts
            _stepY = (_stepY == 0) ? 1 : _stepY;
            _countLabelsY = (int) Math.Ceiling((_yAxisMax - _yAxisMin)/(float) _stepY);
            // protect against dividing by zero in case of no counts
            _countLabelsY = (_countLabelsY == 0) ? 5 : _countLabelsY;

            // protect against dividing by zero in case of no counts
            _yAxisMax = (_yAxisMax == 0) ? 5 : _yAxisMax;
            _yScale = (_xAxisY - _yTop)/(_countLabelsY*(float) _stepY);

            // Context button colors.
            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
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
            Data.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_strChartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), _rectfCaption, _sfCaption);

            // Border
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient
            var rectField = new RectangleF(Border, _captionHeight, ClientSize.Width - 2*Border, ClientSize.Height - _captionHeight - Border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (_isNotPaint) return;

            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";
            string subHeader = _isCounts ? Language.T("Count of Trades") : Language.T("Accumulated Amount") + unit;
            g.DrawString(subHeader, Font, new SolidBrush(LayoutColors.ColorChartFore), _rectSubHeader, _sfCaption);
            var formatCenter = new StringFormat {Alignment = StringAlignment.Center};
            // Grid and Price labels
            for (int label = _xAxisMin10; label <= _xAxisMax; label += _stepX)
            {
                float xPoint = _xLeft + ((_xAxisMin10 - _xAxisMin) + (label - _xAxisMin10))*_xScale;
                float yPoint = _yBottom - Font.Height;
                if (xPoint <= _xRight - _labelWidthX / 2)
                    g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, xPoint, yPoint, formatCenter);
            }

            for (int label = _yAxisMin; label <= _yAxisMax; label += _stepY)
            {
                var labelY = (int) (_xAxisY - (label - _yAxisMin)*_yScale);
                if (label > -1)
                    g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight, labelY - Font.Height/2 - 1);
                g.DrawLine(_penGrid, _xLeft, labelY, _xRight, labelY);
            }

            foreach (var data in _chartData)
            {
                double val = _isCounts ? data.Value.TradesCount : Math.Abs(data.Value.TotalResult);
                float xPt = _xLeft + (data.Key - _minIndex + 1)*_xScale;
                float yPtBottom = _xAxisY;
                var yPtTop = (float) (_xAxisY - (val - _yAxisMin)*_yScale);

                Color color = _isCounts ? Color.Blue : data.Value.TotalResult < 0 ? Color.Red : Color.Green;
                g.DrawLine(new Pen(color), xPt, yPtBottom, xPt, yPtTop);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yTop - Space, _xLeft - 1, _xAxisY);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _xAxisY, _xRight, _xAxisY);
        }

        /// <summary>
        /// Generates dynamic info on the status bar
        /// when we are Moving the mouse over the SmallHistogramChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_isShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            var index = FindNearestMeaningfulX(e.X);
            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            if (_chartData.Count == 0)
            {
                CurrentBarInfo = Language.T("No trades counted");
                return;
            }

            if (_chartData.ContainsKey(index))
            {
                CurrentBarInfo = Language.T("Result") + ": " + _chartData[index].Result.ToString("F2") + unit +
                                 "  " + Language.T("Count") + ": " + _chartData[index].TradesCount +
                                 "  " + Language.T("Total") + ": " + _chartData[index].TotalResult.ToString("F2") + unit;
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
                for(int i = oldIndex; i <= index; i++)
                    if (_chartData.ContainsKey(i))
                        return i;


                oldIndex = index;
                index = GetIndexFromX(x - dX);
                for (int i = index; i <= oldIndex; i++)
                    if (_chartData.ContainsKey(i))
                        return i;
            }

            return index;
        }

        private int GetIndexFromX(int x)
        {
             return (int)Math.Round(_minIndex - 1 + (x - _xLeft) / _xScale);
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
        ///  Handler to toggle between count and cumulative value views
        /// </summary>
        private void BtnToggleViewClick(object sender, EventArgs e)
        {
            _isCounts = !_isCounts;
            SetChartData();
            InitChart();
            Invalidate();
        }

        /// <summary>
        ///  Handler to copy histogram data to clipboard
        /// </summary>
        private void BtnClipboardClick(object sender, EventArgs e)
        {
            Clipboard.Clear();
            // protect against null if no trades in strategy
            if (_chartData.Count > 0)
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
        ///  Handler to write histogram data CSV file
        /// </summary>
        private void BtnExportClick(object sender, EventArgs e)
        {
            // protect against null if no trades in strategy
            if (_chartData.Count > 0)
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
        /// Invalidates the chart after resizing
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