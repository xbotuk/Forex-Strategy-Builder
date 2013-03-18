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

namespace ForexStrategyBuilder
{
    public partial class ScrollFlowPanel : UserControl
    {
        public ScrollFlowPanel()
        {
            InitializeComponent();
        }

        public int FlowPanelWidth
        {
            get { return flowPanel.Width; }
        }

        public ControlCollection FlowPanelControls
        {
            get { return flowPanel.Controls; }
        }

        public void AddControl(Control control)
        {
            flowPanel.Controls.Add(control);
        }

        public void ClearControls()
        {
            flowPanel.Controls.Clear();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            var scroll = (VScrollBar) sender;
            flowPanel.Location = new Point(0, -scroll.Value);
        }

        private void ScrollFlowPanel_Resize(object sender, EventArgs e)
        {
            flowPanel.SuspendLayout();
            SetControls();
            SetControlsWidth();
            flowPanel.ResumeLayout();
        }

        public void SetControls()
        {
            int height = ClientSize.Height;
            int wantedHeight = 0;
            foreach (Control c in flowPanel.Controls)
                wantedHeight += c.Height + c.Margin.Top + c.Margin.Bottom;

            flowPanel.Location = Point.Empty;
            flowPanel.Height = wantedHeight;

            if (wantedHeight <= height)
            {
                scrollBar.Enabled = false;
                scrollBar.Visible = false;
                flowPanel.Width = ClientSize.Width;
            }
            else
            {
                scrollBar.Enabled = true;
                scrollBar.Visible = true;
                scrollBar.Value = 0;
                scrollBar.SmallChange = 10;
                scrollBar.LargeChange = 60;
                scrollBar.Maximum = Math.Max(wantedHeight - height + scrollBar.LargeChange, 0);
                scrollBar.Location = new Point(ClientSize.Width - scrollBar.Width, 0);
                scrollBar.Height = height;
                flowPanel.Width = ClientSize.Width - scrollBar.Width;
            }

        }

        public void SetControlsWidth()
        {
            foreach (Control control in flowPanel.Controls)
            {
                control.Width = flowPanel.Width - control.Margin.Left - control.Margin.Right;
            }
        }
    }
}