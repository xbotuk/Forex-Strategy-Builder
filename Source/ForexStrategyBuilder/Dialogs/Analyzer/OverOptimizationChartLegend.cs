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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    internal class OverOptimizationChartLegend : Panel
    {
        private const int Space = 5;
        private const int Border = 2;
        private PointF[] apntParameters;
        private Brush brushFore;
        private float captionHeight;
        private string chartTitle;
        private Font font;
        private List<string> paramNames;
        private int parameters;
        private Pen penBorder;
        private RectangleF rectfCaption;
        private StringFormat stringFormatCaption;
        private int xLeft;
        private int xRight;
        private int yBottom;
        private int yDelta;
        private int yTop;

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        public void InitChart(List<string> paramsNames)
        {
            paramNames = paramsNames;

            // Chart Title
            chartTitle = Language.T("Legend");
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
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                Border);

            parameters = paramNames.Count;

            if (parameters == 0)
                return;

            yTop = (int) captionHeight + 2*Space + 1;
            yBottom = ClientSize.Height - 2*Space - 1 - Border - Space - Font.Height;
            yDelta = 20;

            xLeft = Border + 2*Space;
            xRight = xLeft + 30;

            apntParameters = new PointF[parameters];

            for (int param = 0; param < parameters; param++)
            {
                apntParameters[param].X = xLeft;
                apntParameters[param].Y = yTop + param*yDelta;
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

            if (paramNames == null || parameters == 0)
                return;

            for (int param = 0; param < parameters; param++)
            {
                // Parameter lines
                float y = apntParameters[param].Y;
                float x = apntParameters[param].X;

                if (y > yBottom) break;

                // Parameter name
                g.DrawLine(new Pen(GetNextColor(param)), x, y, xRight, y);
                g.DrawString(paramNames[param], Font, brushFore, xRight + Space, y - Font.Height/2f - 1);
            }
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
            if (paramNames == null)
                return;

            InitChart(paramNames);
            Invalidate();
        }
    }
}