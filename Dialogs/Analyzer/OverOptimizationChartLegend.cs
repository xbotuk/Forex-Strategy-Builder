// Strategy Analyzer - OverOptimization Chart Legend
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    internal class OverOptimizationChartLegend : Panel
    {
        private const int Space = 5;
        private const int Border = 2;
        private PointF[] _apntParameters;
        private Brush _brushFore;
        private float _captionHeight;
        private string _chartTitle;
        private Font _font;
        private int _parameters;
        private Pen _penBorder;
        private RectangleF _rectfCaption;
        private StringFormat _stringFormatCaption;
        private int _xLeft;
        private int _xRight;
        private int _yBottom;
        private int _yTop;
        private int _yDelta;
        private List<string> _paramNames;

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart(List<string> paramNames)
        {
            _paramNames = paramNames;

            // Chart Title
            _chartTitle = Language.T("Legend");
            _font = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_font.Height, 18);
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _stringFormatCaption = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                 Border);

            _parameters = _paramNames.Count;

            if (_parameters == 0)
                return;

            _yTop = (int)_captionHeight + 2 * Space + 1;
            _yBottom = ClientSize.Height - 2 * Space - 1 - Border - Space - Font.Height;
            _yDelta = 20;

            _xLeft = Border + 2 * Space;
            _xRight = _xLeft + 30;

            _apntParameters = new PointF[_parameters];

            for (int param = 0; param < _parameters; param++)
            {
                _apntParameters[param].X = _xLeft;
                _apntParameters[param].Y = _yTop + param * _yDelta;
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar.
            Data.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), _rectfCaption,
                         _stringFormatCaption);

            // Border.
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient.
            var rectField = new RectangleF(Border, _captionHeight, ClientSize.Width - 2 * Border,
                                           ClientSize.Height - _captionHeight - Border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (_paramNames == null || _parameters == 0)
                return;

            for (int param = 0; param < _parameters; param++)
            {
                // Parameter lines
                float y = _apntParameters[param].Y;
                float x = _apntParameters[param].X;

                if (y > _yBottom) break;

                // Parameter name
                g.DrawLine(new Pen(GetNextColor(param)), x, y, _xRight, y);
                g.DrawString(_paramNames[param], Font, _brushFore, _xRight + Space, y - Font.Height / 2f - 1);
            }
        }

        /// <summary>
        /// Gets color for the chart lines.
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
                param = param % colors.Length;

            return colors[param];
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            if (_paramNames == null)
                return;

            InitChart(_paramNames);
            Invalidate();
        }

    }
}
