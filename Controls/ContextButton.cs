// ContextButton class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.User_interface
{
    public sealed class ContextButton : Panel
    {
        public ContextButton()
        {
            Width = 16;
            Height = 16;
            BackColor = Color.Transparent;
            ColorBack = LayoutColors.ColorCaptionBack;
            ColorFore = LayoutColors.ColorCaptionText;
        }

        public Color ColorBack { private get; set; }
        public Color ColorFore { private get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            const float factor1 = 0.20f;
            const float factor2 = 0.65f;

            Brush brush1 = new SolidBrush(GetIntermediateColor(ColorBack, ColorFore, factor1));
            Brush brush2 = new SolidBrush(GetIntermediateColor(ColorBack, ColorFore, factor2));

            var outside = new Rectangle(1, 3, 14, 3);
            var inside = new Rectangle(2, 4, 12, 1);
            var offset = new Point(0, 4);

            g.FillRectangle(brush1, outside);
            g.FillRectangle(brush2, inside);

            outside.Offset(offset);
            inside.Offset(offset);

            g.FillRectangle(brush1, outside);
            g.FillRectangle(brush2, inside);

            outside.Offset(offset);
            inside.Offset(offset);

            g.FillRectangle(brush1, outside);
            g.FillRectangle(brush2, inside);
        }

        private static Color GetIntermediateColor(Color start, Color end, float factor)
        {
            var r = (int) Math.Round((end.R - start.R)*factor + start.R);
            var g = (int) Math.Round((end.G - start.G)*factor + start.G);
            var b = (int) Math.Round((end.B - start.B)*factor + start.B);
            return Color.FromArgb(r, g, b);
        }
    }
}