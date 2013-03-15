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

namespace ForexStrategyBuilder.CustomControls
{
    public class ContextPanel : Panel
    {
        private readonly CloseButton closeButton;
        private readonly ContextButton contextButton;
        private readonly Timer contextMenuTimer;

        public ContextPanel()
        {
            PopUpContextMenu = new ContextMenuStrip
                {
                    BackColor = LayoutColors.ColorControlBack,
                    ForeColor = LayoutColors.ColorControlText
                };
            PopUpContextMenu.MouseEnter += ContextMenuMouseEnter;
            PopUpContextMenu.MouseLeave += ContextMenuMouseLeave;

            contextMenuTimer = new Timer();
            contextMenuTimer.Tick += ContextMenuTimerTick;

            contextButton = new ContextButton {Parent = this, Visible = false};
            contextButton.MouseClick += ContextButtonOnMouseClick;
            contextButton.MouseDoubleClick += ContextButtonOnMouseClick;
            contextButton.MouseEnter += ContextButtonMouseEnter;
            contextButton.MouseLeave += ContextButtonMouseLeave;

            closeButton = new CloseButton {Parent = this, Visible = false};
        }

        public ContextMenuStrip PopUpContextMenu { get; private set; }

        public bool IsContextButtonVisible
        {
            set { contextButton.Visible = value; }
            get { return contextButton.Visible; }
        }

        public Color ButtonsColorBack
        {
            set
            {
                contextButton.ColorBack = value;
                closeButton.ColorBack = value;
            }
        }

        public Color ButtonColorFore
        {
            set
            {
                contextButton.ColorFore = value;
                closeButton.ColorFore = value;
            }
        }

        protected Color ContextMenuColorBack
        {
            set { PopUpContextMenu.BackColor = value; }
        }

        protected Color ContextMenuColorFore
        {
            set { PopUpContextMenu.ForeColor = value; }
        }

        protected Point ContextButtonLocation
        {
            get { return contextButton.Location; }
        }

        public CloseButton CloseButton
        {
            get { return closeButton; }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateButtonsLocation();
        }

        protected void UpdateButtonsLocation()
        {
            CloseButton.Location = new Point(Width - CloseButton.Width - 2, 0);
            contextButton.Location = new Point(2, 0);
        }

        private void ContextButtonOnMouseClick(object sender, MouseEventArgs mouseEventArgs)
        {
            ActivateContextMenu();
        }

        private void ContextButtonMouseEnter(object sender, EventArgs e)
        {
            ActivateContextMenu();
        }

        private void ActivateContextMenu()
        {
            contextMenuTimer.Stop();
            if (PopUpContextMenu.Visible) return;

            var position = new Point(contextButton.Left, contextButton.Bottom);
            PopUpContextMenu.Show(this, position, ToolStripDropDownDirection.BelowRight);
        }

        private void ContextButtonMouseLeave(object sender, EventArgs e)
        {
            contextMenuTimer.Interval = 1000;
            contextMenuTimer.Start();
        }

        private void ContextMenuMouseEnter(object sender, EventArgs e)
        {
            contextMenuTimer.Stop();
        }

        private void ContextMenuMouseLeave(object sender, EventArgs e)
        {
            contextMenuTimer.Interval = 500;
            contextMenuTimer.Start();
        }

        private void ContextMenuTimerTick(object sender, EventArgs e)
        {
            PopUpContextMenu.Close();
            contextMenuTimer.Stop();
        }
    }
}