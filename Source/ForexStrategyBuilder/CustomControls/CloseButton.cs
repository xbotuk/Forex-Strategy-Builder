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
using System.Windows.Forms;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder.CustomControls
{
    public sealed class CloseButton : Panel
    {
        private bool isHover;

        public CloseButton()
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

            float factor1 = isHover ? 0.55f : 0.25f;
            float factor2 = isHover ? 1.00f : 0.90f;

            var pen1 = new Pen(ColorMagic.GetIntermediateColor(ColorBack, ColorFore, factor1))
                {Width = 3, StartCap = LineCap.Round, EndCap = LineCap.Round};
            var pen2 = new Pen(ColorMagic.GetIntermediateColor(ColorBack, ColorFore, factor2));

            g.DrawLine(pen1, 11, 12, 3, 4);
            g.DrawLine(pen1, 3, 12, 11, 4);
            g.DrawLine(pen2, 11, 12, 3, 4);
            g.DrawLine(pen2, 3, 12, 11, 4);
        }
    }
}