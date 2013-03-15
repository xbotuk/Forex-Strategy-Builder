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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    internal sealed class OverOptimizationChartsForm : Form
    {
        private int currentChartNumber;

        /// <summary>
        ///     Constructor
        /// </summary>
        public OverOptimizationChartsForm(OverOptimizationDataTable[] tableReport, List<string> paramNames)
        {
            TableReport = tableReport;

            Text = Language.T("Over-optimization Report");
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.Sizable;
            AcceptButton = BtnClose;

            // Button Next Chart
            BtnNextCharts = new Button {Parent = this, Text = Language.T("Next Chart")};
            BtnNextCharts.Click += BtnNextChartsClick;
            BtnNextCharts.DoubleClick += BtnNextChartsClick;
            BtnNextCharts.MouseWheel += ChartMouseWheel;
            BtnNextCharts.KeyUp += NextChartsKeyUp;
            BtnNextCharts.UseVisualStyleBackColor = true;

            // Button Close
            BtnClose = new Button
                {
                    Parent = this,
                    Text = Language.T("Close"),
                    DialogResult = DialogResult.Cancel,
                    UseVisualStyleBackColor = true
                };

            CurrentChart = new OverOptimizationCharts {Parent = this};
            CurrentChart.InitChart(tableReport[currentChartNumber]);

            ChartLegend = new OverOptimizationChartLegend {Parent = this};
            ChartLegend.InitChart(paramNames);
        }

        private Button BtnClose { get; set; }
        private Button BtnNextCharts { get; set; }

        private OverOptimizationDataTable[] TableReport { get; set; }

        private OverOptimizationCharts CurrentChart { get; set; }
        private OverOptimizationChartLegend ChartLegend { get; set; }

        /// <summary>
        ///     Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(650, 400);
            MinimumSize = new Size(450, 200);
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
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

            // Next Chart
            BtnNextCharts.Size = new Size((int) (1.5*buttonWidth), buttonHeight);
            BtnNextCharts.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Legend
            ChartLegend.Size = new Size(160, BtnClose.Top - space - btnVertSpace);
            ChartLegend.Location = new Point(ClientSize.Width - space - ChartLegend.Width, space);

            // Chart
            CurrentChart.Size = new Size(ClientSize.Width - 3*space - ChartLegend.Width,
                                         BtnClose.Top - space - btnVertSpace);
            CurrentChart.Location = new Point(space, space);
        }

        /// <summary>
        ///     Opens next chart.
        /// </summary>
        private void BtnNextChartsClick(object sender, EventArgs e)
        {
            ShowNextChart();
        }

        /// <summary>
        ///     Shows a chart on mouse wheel.
        /// </summary>
        private void ChartMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                ShowNextChart();
            else if (e.Delta < 0)
                ShowPreviousChart();
        }

        /// <summary>
        ///     Shows a chart on keyup.
        /// </summary>
        private void NextChartsKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    ShowNextChart();
                    break;
                case Keys.PageUp:
                    ShowPreviousChart();
                    break;
            }
        }

        /// <summary>
        ///     Shows next chart.
        /// </summary>
        private void ShowNextChart()
        {
            currentChartNumber++;
            if (currentChartNumber >= TableReport.Length)
                currentChartNumber = 0;

            CurrentChart.Parent = this;
            CurrentChart.InitChart(TableReport[currentChartNumber]);
            CurrentChart.Invalidate();
        }

        /// <summary>
        ///     Shows previous chart.
        /// </summary>
        private void ShowPreviousChart()
        {
            currentChartNumber--;
            if (currentChartNumber < 0)
                currentChartNumber = TableReport.Length - 1;

            CurrentChart.Parent = this;
            CurrentChart.InitChart(TableReport[currentChartNumber]);
            CurrentChart.Invalidate();
        }
    }
}