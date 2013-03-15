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
using System.Windows.Forms;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder.CustomControls
{
    public sealed class ContextButton : Panel
    {
        private bool isHover;

        public ContextButton()
        {
            Width = 16;
            Height = 16;
            BackColor = Color.Transparent;
            ColorBack = LayoutColors.ColorCaptionBack;
            ColorFore = LayoutColors.ColorCaptionText;
            Cursor = Cursors.Arrow;
        }

        public Color ColorBack { private get; set; }
        public Color ColorFore { private get; set; }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float factor1 = isHover ? 0.40f : 0.20f;
            float factor2 = isHover ? 0.85f : 0.65f;

            Brush brush1 = new SolidBrush(ColorMagic.GetIntermediateColor(ColorBack, ColorFore, factor1));
            Brush brush2 = new SolidBrush(ColorMagic.GetIntermediateColor(ColorBack, ColorFore, factor2));

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
    }
}