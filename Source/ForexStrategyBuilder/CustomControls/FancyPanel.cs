// Fancy Panel class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class FancyPanel : Panel
    {
        private const int Border = 2;
        private readonly string captionText;
        private readonly bool showCaption = true;
        private Brush brushCaption;
        private Color colorCaptionBack;
        private Font fontCaption;
        private float height;
        private Pen penBorder;
        private RectangleF rectCaption;
        private StringFormat stringFormatCaption;
        private float width;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public FancyPanel()
        {
            captionText = "";
            showCaption = false;
            InitializeParameters();
            SetColors();
            Padding = new Padding(Border, 2*Border, Border, Border);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public FancyPanel(string captionText)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            this.captionText = captionText;
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public FancyPanel(string captionText, Color borderColor)
        {
            this.captionText = captionText;
            colorCaptionBack = borderColor;
            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            penBorder = new Pen(Data.GetGradientColor(borderColor, -LayoutColors.DepthCaption), Border);

            InitializeParameters();
        }

        /// <summary>
        ///     Gets the caption height.
        /// </summary>
        public float CaptionHeight { get; private set; }

        /// <summary>
        ///     Sets the panel colors
        /// </summary>
        private void SetColors()
        {
            colorCaptionBack = LayoutColors.ColorCaptionBack;
            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        ///     Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            fontCaption = new Font(Font.FontFamily, 9);
            stringFormatCaption = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

            CaptionHeight = showCaption ? Math.Max(fontCaption.Height, 18) : 2*Border;
        }

        /// <summary>
        ///     On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ClientSize.Width == 0 || ClientSize.Height == 0) return;
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            // Caption
            Data.GradientPaint(g, rectCaption, colorCaptionBack, LayoutColors.DepthCaption);
            if (showCaption)
                g.DrawString(captionText, fontCaption, brushCaption, rectCaption, stringFormatCaption);

            g.DrawLine(penBorder, 1, CaptionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, CaptionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paint the panel background
            var rectClient = new RectangleF(Border, CaptionHeight, width, height);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (showCaption)
            {
                width = ClientSize.Width - 2*Border;
                height = ClientSize.Height - CaptionHeight - Border;
                rectCaption = new RectangleF(0, 0, ClientSize.Width, CaptionHeight);
            }
            else
            {
                width = ClientSize.Width - 2*Border;
                height = ClientSize.Height - CaptionHeight - Border;
                rectCaption = new RectangleF(0, 0, ClientSize.Width, CaptionHeight);
            }

            Invalidate();
        }
    }
}