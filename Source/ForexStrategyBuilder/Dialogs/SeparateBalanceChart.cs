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

namespace ForexStrategyBuilder.Dialogs
{
    public sealed class SeparateBalanceChart : Form
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public SeparateBalanceChart()
        {
            Text = Language.T("Balance / Equity Chart");
            Icon = Data.Icon;
            AcceptButton = BtnClose;

            // Button Close
            BtnClose = new Button
                {
                    Parent = this,
                    Text = Language.T("Close"),
                    DialogResult = DialogResult.Cancel,
                    UseVisualStyleBackColor = true
                };

            // Balance chart
            BalanceChart = new SmallBalanceChart {Parent = this, ShowDynamicInfo = true};
            BalanceChart.MouseMove += BalanceChartMouseMove;
            BalanceChart.MouseLeave += BalanceChartMouseLeave;
            BalanceChart.SetChartData();
            BalanceChart.InitChart();

            // Label Dynamic Info
            LblDynInfo = new Label
                {Parent = this, ForeColor = LayoutColors.ColorControlText, BackColor = Color.Transparent};
        }

        private Button BtnClose { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private Label LblDynInfo { get; set; }

        /// <summary>
        ///     Perform initializing.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(500, 400);
            MinimumSize = new Size(300, 250);
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Balance Chart
            BalanceChart.Size = new Size(ClientSize.Width - 2*space, BtnClose.Top - space - btnVertSpace);
            BalanceChart.Location = new Point(space, space);

            // Label dynamic info.
            LblDynInfo.Width = BtnClose.Left - 2*space;
            LblDynInfo.Location = new Point(space, BtnClose.Top + 6);
        }

        /// <summary>
        ///     Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Show the dynamic info on the status bar.
        /// </summary>
        private void BalanceChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallBalanceChart) sender;
            LblDynInfo.Text = chart.CurrentBarInfo;
        }

        /// <summary>
        ///     Deletes the dynamic info on the status bar.
        /// </summary>
        private void BalanceChartMouseLeave(object sender, EventArgs e)
        {
            LblDynInfo.Text = string.Empty;
        }
    }
}