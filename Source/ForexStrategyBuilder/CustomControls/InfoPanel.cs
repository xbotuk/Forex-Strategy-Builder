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
using ForexStrategyBuilder.CustomControls;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public class InfoPanel : ContextPanel
    {
        private const int Border = 2;
        private const float PaddingParamData = 4;
        private Brush brushCaption;
        private Brush brushData;
        private Brush brushParams;
        private float captionHeight;
        private string captionText;
        private Color colorBackroundEvenRows;
        private Color colorBackroundOddRows;
        private Color colorBackroundWarningRow;
        private Color colorCaptionBack;
        private Color colorTextWarningRow;
        private bool[] flagsList;
        private Font fontCaption;
        private Font fontData;
        private HScrollBar hScrollBar;
        private float height;
        private float maxParamWidth;
        private float maxValueWidth;
        private float paramTab;
        private string[] paramsList;
        private Pen penBorder;
        private RectangleF rectCaption;
        private float requiredHeight;
        private float rowHeight;
        private int rows;
        private StringFormat stringFormatCaption;
        private StringFormat stringFormatData;
        private VScrollBar vScrollBar;
        private float valueTab;
        private string[] valuesList;
        private int visibleRows;
        private float width;

        /// <summary>
        ///     Default Constructor
        /// </summary>
        public InfoPanel()
        {
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        ///     Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorCaptionBack = LayoutColors.ColorCaptionBack;
            colorBackroundEvenRows = LayoutColors.ColorEvenRowBack;
            colorBackroundWarningRow = LayoutColors.ColorWarningRowBack;
            colorTextWarningRow = LayoutColors.ColorWarningRowText;
            colorBackroundOddRows = LayoutColors.ColorOddRowBack;

            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            brushParams = new SolidBrush(LayoutColors.ColorControlText);
            brushData = new SolidBrush(LayoutColors.ColorControlText);

            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        ///     Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            // Caption
            stringFormatCaption = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

            fontCaption = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(fontCaption.Height, 18);
            rectCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);

            // Data row
            stringFormatData = new StringFormat {Trimming = StringTrimming.EllipsisCharacter};
            fontData = new Font(Font.FontFamily, 9);
            rowHeight = fontData.Height + 4;

            Padding = new Padding(Border, (int) captionHeight, Border, Border);

            hScrollBar = new HScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Bottom,
                    Enabled = false,
                    Visible = false,
                    SmallChange = 10,
                    LargeChange = 30
                };
            hScrollBar.Scroll += ScrollBarScroll;

            vScrollBar = new VScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Right,
                    TabStop = true,
                    Enabled = false,
                    Visible = false,
                    SmallChange = 1,
                    LargeChange = 3,
                    Maximum = 20
                };
            vScrollBar.Scroll += ScrollBarScroll;

            MouseUp += InfoPanelMouseUp;
        }

        /// <summary>
        ///     Update
        /// </summary>
        public void Update(string[] asParams, string[] asValues, bool[] abFlags, string caption)
        {
            paramsList = asParams;
            valuesList = asValues;
            flagsList = abFlags;
            captionText = caption;

            rows = paramsList.Length;
            requiredHeight = captionHeight + rows*rowHeight + Border;

            // Maximum width
            maxParamWidth = 0;
            maxValueWidth = 0;
            Graphics g = CreateGraphics();
            for (int i = 0; i < rows; i++)
            {
                float fWidthParam = g.MeasureString(paramsList[i], fontData).Width;
                if (fWidthParam > maxParamWidth)
                    maxParamWidth = fWidthParam;

                float fValueWidth = g.MeasureString(valuesList[i], fontData).Width;
                if (fValueWidth > maxValueWidth)
                    maxValueWidth = fValueWidth;
            }
            g.Dispose();

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();
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
            g.DrawString(captionText, fontCaption, brushCaption, rectCaption, stringFormatCaption);

            for (int i = 0; i*rowHeight + captionHeight < height; i++)
            {
                float fVerticalPosition = i*rowHeight + captionHeight;
                var pointFParam = new PointF(paramTab + 2, fVerticalPosition);
                var pointFValue = new PointF(valueTab + 2, fVerticalPosition);
                var rectRow = new RectangleF(Border, fVerticalPosition, width, rowHeight);

                // Row background
                if (i + vScrollBar.Value < rows && flagsList[i + vScrollBar.Value])
                    g.FillRectangle(new SolidBrush(colorBackroundWarningRow), rectRow);
                else if (Math.Abs(i%2f - 0) > 0.001)
                    g.FillRectangle(new SolidBrush(colorBackroundEvenRows), rectRow);
                else
                    g.FillRectangle(new SolidBrush(colorBackroundOddRows), rectRow);

                if (i + vScrollBar.Value >= rows)
                    continue;

                if (i + vScrollBar.Value < rows && flagsList[i + vScrollBar.Value])
                {
                    Brush brush = new SolidBrush(colorTextWarningRow);
                    g.DrawString(paramsList[i + vScrollBar.Value], fontData, brush, pointFParam, stringFormatData);
                    g.DrawString(valuesList[i + vScrollBar.Value], fontData, brush, pointFValue, stringFormatData);
                }
                else
                {
                    g.DrawString(paramsList[i + vScrollBar.Value], fontData, brushParams, pointFParam,
                                 stringFormatData);
                    g.DrawString(valuesList[i + vScrollBar.Value], fontData, brushData, pointFValue,
                                 stringFormatData);
                }
            }

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            CalculateScrollBarStatus();
            CalculateTabs();
            base.OnResize(eventargs);
            Invalidate();
        }

        /// <summary>
        ///     Scroll Bars status
        /// </summary>
        private void CalculateScrollBarStatus()
        {
            width = ClientSize.Width - 2*Border;
            height = ClientSize.Height - Border;

            bool needHorizontal = width < maxParamWidth + PaddingParamData + maxValueWidth - 2;
            bool needVertical = height < requiredHeight;
            bool isHorizontal = needHorizontal;
            bool isVertical = needVertical;

            if (needHorizontal && !needVertical)
            {
                height = ClientSize.Height - hScrollBar.Height - Border;
                isVertical = height < requiredHeight;
            }
            else if (needVertical && !needHorizontal)
            {
                width = ClientSize.Width - vScrollBar.Width - 2*Border;
                isHorizontal = width < maxParamWidth + PaddingParamData + maxValueWidth - 2;
            }

            if (isHorizontal)
                height = ClientSize.Height - hScrollBar.Height - Border;

            if (isVertical)
                width = ClientSize.Width - vScrollBar.Width - 2*Border;

            vScrollBar.Enabled = isVertical;
            vScrollBar.Visible = isVertical;
            hScrollBar.Enabled = isHorizontal;
            hScrollBar.Visible = isHorizontal;

            hScrollBar.Value = 0;
            if (isHorizontal)
            {
                var iPoinShort = (int) (maxParamWidth + PaddingParamData + maxValueWidth - width);
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }

            rectCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            visibleRows = (int) Math.Min(((height - captionHeight)/rowHeight), rows);

            vScrollBar.Value = 0;
            vScrollBar.Maximum = rows - visibleRows + vScrollBar.LargeChange - 1;
        }

        /// <summary>
        ///     Tabs
        /// </summary>
        private void CalculateTabs()
        {
            if (width < maxParamWidth + PaddingParamData + maxValueWidth)
            {
                paramTab = -hScrollBar.Value + Border;
                valueTab = paramTab + maxParamWidth;
            }
            else
            {
                float fSpace = (width - (maxParamWidth + maxValueWidth))/5;
                paramTab = 2*fSpace;
                valueTab = paramTab + maxParamWidth + fSpace;
            }
        }

        /// <summary>
        ///     ScrollBarScroll
        /// </summary>
        private void ScrollBarScroll(object sender, ScrollEventArgs e)
        {
            CalculateTabs();
            int horizontal = hScrollBar.Visible ? hScrollBar.Height : 0;
            int vertical = vScrollBar.Visible ? vScrollBar.Width : 0;
            var rect = new Rectangle(Border, (int) captionHeight, ClientSize.Width - vertical - 2*Border,
                                     ClientSize.Height - (int) captionHeight - horizontal - Border);
            Invalidate(rect);
        }

        /// <summary>
        ///     Selects the vertical scrollbar
        /// </summary>
        private void InfoPanelMouseUp(object sender, MouseEventArgs e)
        {
            vScrollBar.Select();
        }
    }
}