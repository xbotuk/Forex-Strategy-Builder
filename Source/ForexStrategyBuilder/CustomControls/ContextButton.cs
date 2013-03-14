// ContextButton class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.CustomControls
{
    public sealed class ContextButton : Panel
    {
        private bool _isHover;

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
            _isHover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float factor1 = _isHover ? 0.40f : 0.20f;
            float factor2 = _isHover ? 0.85f : 0.65f;

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