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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    public sealed partial class OverOptimization : FancyPanel
    {
        private readonly BackgroundWorker bgWorker;
        private readonly Button btnOpenFolder;
        private readonly Button btnOpenReport;
        private readonly Button btnStart;
        private readonly Button btnViewCharts;
        private readonly Label lblDeviation;
        private readonly Label lblIntro;
        private readonly Label lblNoParams;
        private readonly Label lblParams;
        private readonly NumericUpDown nudDeviation;
        private readonly NumericUpDown nudParams;
        private readonly ProgressBar progressBar;

        /// <summary>
        ///     Public constructor.
        /// </summary>
        public OverOptimization(string caption) : base(caption)
        {
            lblIntro = new Label();
            lblDeviation = new Label();
            lblParams = new Label();
            lblNoParams = new Label();
            nudDeviation = new NumericUpDown();
            nudParams = new NumericUpDown();
            btnStart = new Button();
            btnViewCharts = new Button();
            btnOpenFolder = new Button();
            btnOpenReport = new Button();
            progressBar = new ProgressBar();

            Color colorText = LayoutColors.ColorControlText;

            CountStrategyParams();
            bool isParams = countStratParams > 0;

            // Label Intro
            lblIntro.Parent = this;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.AutoSize = false;
            lblIntro.Text =
                Language.T(
                    "The Over-optimization Report shows how the results of the backtest vary as the numerical parameters of the strategy change by a given percent.");

            // Label Deviation
            lblDeviation.Parent = this;
            lblDeviation.ForeColor = colorText;
            lblDeviation.BackColor = Color.Transparent;
            lblDeviation.AutoSize = true;
            lblDeviation.Text = Language.T("Parameters deviation % [recommended 20]");

            // Label Parameters
            lblParams.Parent = this;
            lblParams.ForeColor = colorText;
            lblParams.BackColor = Color.Transparent;
            lblParams.AutoSize = true;
            lblParams.Text = Language.T("Parameters number [recommended 20]");

            // lblNoParams
            lblNoParams.Parent = this;
            lblNoParams.Text = Language.T("There are no parameters suitable for analysis.");
            lblNoParams.ForeColor = LayoutColors.ColorSignalRed;
            lblNoParams.BackColor = Color.Transparent;
            lblNoParams.AutoSize = true;
            lblNoParams.Visible = !isParams;

            // NumericUpDown Deviation
            nudDeviation.BeginInit();
            nudDeviation.Parent = this;
            nudDeviation.Name = "Deviation";
            nudDeviation.TextAlign = HorizontalAlignment.Center;
            nudDeviation.Minimum = 1;
            nudDeviation.Maximum = 100;
            nudDeviation.Value = 20;
            nudDeviation.EndInit();

            // NumericUpDown Swap Long
            nudParams.BeginInit();
            nudParams.Parent = this;
            nudParams.Name = "Parameters";
            nudParams.TextAlign = HorizontalAlignment.Center;
            nudParams.Minimum = 1;
            nudParams.Maximum = 100;
            nudParams.Value = 20;
            nudParams.EndInit();

            // Button View Charts
            btnViewCharts.Parent = this;
            btnViewCharts.Name = "btnViewCharts";
            btnViewCharts.Text = Language.T("View Charts");
            btnViewCharts.ImageAlign = ContentAlignment.MiddleLeft;
            btnViewCharts.Image = Resources.overoptimization_chart;
            btnViewCharts.Enabled = false;
            btnViewCharts.Click += ViewChartsClick;
            btnViewCharts.UseVisualStyleBackColor = true;

            // Button Open report folder
            btnOpenFolder.Parent = this;
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Text = Language.T("Open Folder");
            btnOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenFolder.Image = Resources.folder_open;
            btnOpenFolder.Enabled = false;
            btnOpenFolder.Click += OpenFolderClick;
            btnOpenFolder.UseVisualStyleBackColor = true;

            // Button Open Report
            btnOpenReport.Parent = this;
            btnOpenReport.Name = "btnOpenReport";
            btnOpenReport.Text = Language.T("Open Report");
            btnOpenReport.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenReport.Image = Resources.export;
            btnOpenReport.Enabled = false;
            btnOpenReport.Click += OpenReportClick;
            btnOpenReport.UseVisualStyleBackColor = true;

            // Button Start
            btnStart.Parent = this;
            btnStart.Text = Language.T("Start");
            btnStart.Enabled = isParams;
            btnStart.Click += BtnStartClick;
            btnStart.UseVisualStyleBackColor = true;

            // ProgressBar
            progressBar.Parent = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step = 1;

            // BackgroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.ProgressChanged += BgWorkerProgressChanged;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            Resize += PnlOverOptimizationResize;
        }

        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Calculates controls positions on resizing.
        /// </summary>
        private void PnlOverOptimizationResize(object sender, EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            int space = btnHrzSpace;
            const int border = 2;

            // Labels
            lblIntro.Width = Width - 2*(btnHrzSpace + border);
            lblIntro.Height = 2*buttonHeight;
            lblIntro.Location = new Point(btnHrzSpace + border, 2*space + 18);

            lblDeviation.Location = new Point(btnHrzSpace + border, 3*buttonHeight + 2*space + 18);
            lblParams.Location = new Point(btnHrzSpace + border, 4*buttonHeight + 3*space + 18);
            lblNoParams.Location = new Point(btnHrzSpace + border, 5*buttonHeight + 4*space + 18 + 20);

            // FancyNud Parameters
            int maxLabelRight = lblDeviation.Right;
            if (lblParams.Right > maxLabelRight) maxLabelRight = lblParams.Right;
            int nudLeft = maxLabelRight + btnHrzSpace;
            const int nudWidth = 70;
            nudDeviation.Size = new Size(nudWidth, buttonHeight);
            nudDeviation.Location = new Point(nudLeft, 3*buttonHeight + 2*space + 16);
            nudParams.Size = new Size(nudWidth, buttonHeight);
            nudParams.Location = new Point(nudLeft, 4*buttonHeight + 3*space + 16);

            int btnWideWidth = (ClientSize.Width - 2*border - 4*btnHrzSpace)/3;
            btnViewCharts.Size = new Size(btnWideWidth, buttonHeight);
            btnOpenFolder.Size = btnViewCharts.Size;
            btnOpenReport.Size = btnViewCharts.Size;

            btnViewCharts.Location = new Point(border + btnHrzSpace, Height - buttonHeight - btnVertSpace);
            btnOpenFolder.Location = new Point(btnViewCharts.Right + btnHrzSpace, btnViewCharts.Top);
            btnOpenReport.Location = new Point(btnOpenFolder.Right + btnHrzSpace, btnViewCharts.Top);

            // Progress Bar
            progressBar.Size = new Size(ClientSize.Width - 2*border - 2*btnHrzSpace, (int) (Data.VerticalDlu*9));
            progressBar.Location = new Point(border + btnHrzSpace, btnOpenFolder.Top - progressBar.Height - btnVertSpace);

            // Button Run
            btnStart.Size = btnViewCharts.Size;
            btnStart.Location = new Point(border + btnHrzSpace, progressBar.Top - buttonHeight - btnVertSpace);
        }

        /// <summary>
        ///     Button Run clicked.
        /// </summary>
        private void BtnStartClick(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                // Cancel the asynchronous operation
                bgWorker.CancelAsync();
            }
            else
            {
                // Start the bgWorker
                Cursor = Cursors.WaitCursor;
                btnStart.Text = Language.T("Stop");
                IsRunning = true;

                nudDeviation.Enabled = false;
                nudParams.Enabled = false;

                btnViewCharts.Enabled = false;
                btnOpenFolder.Enabled = false;
                btnOpenReport.Enabled = false;

                progressBar.Value = 1;
                progressPercent = 0;
                computedCycles = 0;

                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        ///     Does the job.
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            SetParametersValues((int) nudDeviation.Value, (int) nudParams.Value);
            CalculateStatsTables((int) nudDeviation.Value, (int) nudParams.Value);
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            string report = GenerateReport();
            SaveReport(report);

            foreach (Control control in Controls)
                control.Enabled = true;

            btnStart.Enabled = true;

            btnStart.Text = Language.T("Start");
            Cursor = Cursors.Default;
            IsRunning = false;
        }

        /// <summary>
        ///     Opens a chart screen.
        /// </summary>
        private void ViewChartsClick(object sender, EventArgs e)
        {
            var chartForm = new OverOptimizationChartsForm(tableReport, paramNames);
            chartForm.ShowDialog();
        }

        /// <summary>
        ///     Opens the report folder.
        /// </summary>
        private void OpenFolderClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Data.StrategyDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Opens the report.
        /// </summary>
        private void OpenReportClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(pathReportFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}