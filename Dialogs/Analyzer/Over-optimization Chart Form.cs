// Over_optimization Charts Form
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    class Over_optimization_Charts_Form : Form
    {
        Button btnClose;
        Button btnNextCharts;

        Over_optimization_Data_Table[] tableReport;

        int currentChartNumber = 0;
        Over_optimization_Charts currentChart;

        /// <summary>
        /// Constructor
        /// </summary>
        public Over_optimization_Charts_Form(Over_optimization_Data_Table[] tableReport)
        {
            this.tableReport = tableReport;

            Text            = Language.T("Over-optimization Report");
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.Sizable;
            AcceptButton    = btnClose;

            // Button Close
            btnNextCharts = new Button();
            btnNextCharts.Parent = this;
            btnNextCharts.Text   = Language.T("Next Chart");
            btnNextCharts.Click       += new EventHandler(BtnNextCharts_Click);
            btnNextCharts.DoubleClick += new EventHandler(BtnNextCharts_Click);
            btnNextCharts.MouseWheel  += new MouseEventHandler(Chart_MouseWheel);
            btnNextCharts.KeyUp       += new KeyEventHandler(NextCharts_KeyUp);
            btnNextCharts.UseVisualStyleBackColor = true;

            // Button Close
            btnClose = new Button();
            btnClose.Parent = this;
            btnClose.Text   = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            currentChart = new Over_optimization_Charts();
            currentChart.Parent = this;
            currentChart.InitChart(tableReport[currentChartNumber]);
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize  = new Size(500, 400);
            MinimumSize = new Size(300, 200); 

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;

            // Button Close
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Next Chart
            btnNextCharts.Size = new Size((int)(1.5 * buttonWidth), buttonHeight);
            btnNextCharts.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Chart
            currentChart.Size = new Size(ClientSize.Width - 2 * space, btnClose.Top - space - btnVertSpace);
            currentChart.Location = new Point(space, space);
        }

        /// <summary>
        /// Opens next chart.
        /// </summary>
        void BtnNextCharts_Click(object sender, EventArgs e)
        {
            ShowNextChart();
        }

        /// <summary>
        /// Shows a chart on mouse wheel.
        /// </summary>
        void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                ShowNextChart();
            else if (e.Delta < 0)
                ShowPreviousChart();

            return;
        }

        /// <summary>
        /// Shows a chart on keyup.
        /// </summary>
        void NextCharts_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
                ShowNextChart();
            else if (e.KeyCode == Keys.PageUp)
                ShowPreviousChart();
        }

        /// <summary>
        /// Shows next chart.
        /// </summary>
        void ShowNextChart()
        {
            currentChartNumber++;
            if (currentChartNumber >= tableReport.Length)
                currentChartNumber = 0;

            currentChart.Parent = this;
            currentChart.InitChart(tableReport[currentChartNumber]);
            currentChart.Invalidate();
        }

        /// <summary>
        /// Shows previous chart.
        /// </summary>
        void ShowPreviousChart()
        {
            currentChartNumber--;
            if (currentChartNumber < 0)
                currentChartNumber = tableReport.Length - 1;

            currentChart.Parent = this;
            currentChart.InitChart(tableReport[currentChartNumber]);
            currentChart.Invalidate();
        }
    }
}
