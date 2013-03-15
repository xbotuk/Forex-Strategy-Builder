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

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    internal class OverOptimizationCharts : Panel
    {
        private const int Space = 5;
        private const int Border = 2;
        private PointF[][] apntParameters;
        private Brush brushFore;
        private float captionHeight;
        private string chartTitle;
        private int countLabels;
        private float delta;
        private int devSteps;
        private Font font;
        private int labelWidth;
        private int maximum;
        private int minimum;
        private int parameters;
        private Pen penBorder;
        private Pen penGrid;
        private int percentDev;
        private RectangleF rectfCaption;
        private int step;
        private StringFormat stringFormatCaption;
        private int xLeft;
        private int xMiddle;
        private int xRight;
        private float xScale;
        private int yBottom;
        private float yScale;
        private int yTop;
        private OverOptimizationDataTable Table { get; set; }

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        public void InitChart(OverOptimizationDataTable table)
        {
            Table = table;

            // Chart Title
            chartTitle = table.Name;
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
                {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                Border);

            devSteps = table.CountDeviationSteps;
            percentDev = (devSteps - 1)/2;
            parameters = table.CountStrategyParams;

            if (parameters == 0)
                return;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            for (int param = 0; param < parameters; param++)
            {
                double min = double.MaxValue;
                double max = double.MinValue;
                for (int dev = 0; dev < devSteps; dev++)
                {
                    int index = dev - percentDev;
                    double value = table.GetData(index, param);
                    if (min > value) min = value;
                    if (max < value) max = value;
                }
                if (minValue > min) minValue = min;
                if (maxValue < max) maxValue = max;
            }

            maximum = (int) Math.Round(maxValue + 0.1*Math.Abs(maxValue));
            minimum = (int) Math.Round(minValue - 0.1*Math.Abs(minValue));
            int roundStep = Math.Abs(minimum) > 1 ? 10 : 1;
            minimum = (int) (Math.Floor(minimum/(float) roundStep)*roundStep);
            if (Math.Abs(maximum - minimum) < 1) maximum = minimum + 1;

            yTop = (int) captionHeight + 2*Space + 1;
            yBottom = ClientSize.Height - 2*Space - 1 - Border - Space - Font.Height;

            Graphics g = CreateGraphics();
            labelWidth =
                (int)
                Math.Max(g.MeasureString(minimum.ToString(CultureInfo.InvariantCulture), font).Width,
                         g.MeasureString(maximum.ToString(CultureInfo.InvariantCulture), font).Width);
            g.Dispose();

            labelWidth = Math.Max(labelWidth, 30);
            xLeft = Border + 3*Space;
            xRight = ClientSize.Width - Border - 2*Space - labelWidth;
            xScale = (xRight - xLeft)/(float) (devSteps - 1);
            xMiddle = (int) (xLeft + percentDev*xScale);

            countLabels = Math.Max((yBottom - yTop)/20, 2);
            delta = (float) Math.Max(Math.Round((maximum - minimum)/(float) countLabels), roundStep);
            step = (int) Math.Ceiling(delta/roundStep)*roundStep;
            countLabels = (int) Math.Ceiling((maximum - minimum)/(float) step);
            maximum = minimum + countLabels*step;
            yScale = (yBottom - yTop)/(countLabels*(float) step);

            apntParameters = new PointF[parameters][];

            for (int param = 0; param < parameters; param++)
            {
                apntParameters[param] = new PointF[devSteps];
                for (int dev = 0; dev < devSteps; dev++)
                {
                    int index = dev - percentDev;
                    apntParameters[param][dev].X = xLeft + dev*xScale;
                    apntParameters[param][dev].Y =
                        (float) (yBottom - (table.GetData(index, param) - minimum)*yScale);
                }
            }
        }

        /// <summary>
        ///     Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar.
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption,
                         stringFormatCaption);

            // Border.
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient.
            var rectField = new RectangleF(Border, captionHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - captionHeight - Border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (Table == null || parameters == 0)
                return;

            // Grid and value labels.
            for (int label = minimum; label <= maximum; label += step)
            {
                var iLabelY = (int) (yBottom - (label - minimum)*yScale);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight + Space,
                             iLabelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, xLeft, iLabelY, xRight, iLabelY);
            }

            // Deviation labels.
            var strFormatDevLabel = new StringFormat {Alignment = StringAlignment.Center};
            int xLabelRight = 0;
            for (int dev = 0; dev < devSteps; dev++)
            {
                int index = dev - percentDev;
                var xVertLine = (int) (xLeft + dev*xScale);
                g.DrawLine(penGrid, xVertLine, yTop, xVertLine, yBottom + 2);
                string devLabel = index + (index != 0 ? "%" : "");
                var devLabelWidth = (int) g.MeasureString(devLabel, font).Width;
                if (xVertLine - devLabelWidth/2 > xLabelRight)
                {
                    g.DrawString(devLabel, font, brushFore, xVertLine, yBottom + Space, strFormatDevLabel);
                    xLabelRight = xVertLine + devLabelWidth/2;
                }
            }

            // Vertical coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xMiddle - 1, yTop - Space, xMiddle - 1, yBottom + 1);

            // Chart lines
            for (int param = 0; param < parameters; param++)
                g.DrawLines(new Pen(GetNextColor(param)), apntParameters[param]);

            // Horizontal coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yBottom + 1, xRight, yBottom + 1);
        }

        /// <summary>
        ///     Gets color for the chart lines.
        /// </summary>
        private Color GetNextColor(int param)
        {
            var colors = new[]
                {
                    Color.DarkViolet,
                    Color.Red,
                    Color.Peru,
                    Color.DarkSalmon,
                    Color.Orange,
                    Color.Green,
                    Color.Lime,
                    Color.Gold
                };

            if (param >= colors.Length)
                param = param%colors.Length;

            return colors[param];
        }

        /// <summary>
        ///     Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            if (Table == null)
                return;

            InitChart(Table);
            Invalidate();
        }
    }
}