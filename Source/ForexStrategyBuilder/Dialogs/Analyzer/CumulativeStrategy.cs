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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    internal sealed class CumulativeStrategy : FancyPanel
    {
        public CumulativeStrategy(string caption) : base(caption)
        {
            LblInfo = new Label();
            BtnAddStrategy = new Button();
            BtnResetReport = new Button();
            BtnOpenFolder = new Button();
            BtnOpenReport = new Button();

            // Label Info
            LblInfo.Parent = this;
            LblInfo.Text = "Under Construction!!";
            LblInfo.BackColor = Color.Transparent;
            LblInfo.ForeColor = Color.Red;
            LblInfo.Font = new Font(Font.FontFamily, 12, FontStyle.Bold);

            // Button Add Strategy
            BtnAddStrategy.Parent = this;
            BtnAddStrategy.Text = Language.T("Add Strategy");
            BtnAddStrategy.Click += BtnStartClick;
            BtnAddStrategy.UseVisualStyleBackColor = true;

            // Button Reset Report
            BtnResetReport.Parent = this;
            BtnResetReport.Name = "btnResetReport";
            BtnResetReport.ImageAlign = ContentAlignment.MiddleCenter;
            BtnResetReport.Image = Resources.close_button;
            BtnResetReport.Click += ViewChartsClick;
            BtnResetReport.UseVisualStyleBackColor = true;

            // Button Open report folder
            BtnOpenFolder.Parent = this;
            BtnOpenFolder.Name = "btnOpenFolder";
            BtnOpenFolder.Text = Language.T("Open Folder");
            BtnOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            BtnOpenFolder.Image = Resources.folder_open;
            BtnOpenFolder.Enabled = false;
            BtnOpenFolder.Click += OpenFolderClick;
            BtnOpenFolder.UseVisualStyleBackColor = true;

            // Button Open Report
            BtnOpenReport.Parent = this;
            BtnOpenReport.Name = "btnOpenReport";
            BtnOpenReport.Text = Language.T("Open Report");
            BtnOpenReport.ImageAlign = ContentAlignment.MiddleLeft;
            BtnOpenReport.Image = Resources.export;
            BtnOpenReport.Enabled = false;
            BtnOpenReport.Click += OpenReportClick;
            BtnOpenReport.UseVisualStyleBackColor = true;

            Resize += PnlCumulativeStrategyResize;
        }

        private Button BtnAddStrategy { get; set; }
        private Button BtnResetReport { get; set; }
        private Button BtnOpenFolder { get; set; }
        private Button BtnOpenReport { get; set; }
        //string pathReportFile;

        private Label LblInfo { get; set; }

        private void PnlCumulativeStrategyResize(object sender, EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            const int border = 2;

            // Button Add Strategy.
            BtnAddStrategy.Size = new Size(buttonWidth, buttonHeight);
            BtnAddStrategy.Location = new Point(border + btnHrzSpace, 18 + btnVertSpace);

            // Button Reset Report.
            BtnResetReport.Size = new Size(buttonWidth/2, buttonHeight);
            BtnResetReport.Location = new Point(border + btnHrzSpace, BtnAddStrategy.Bottom + btnVertSpace);

            // Label Info
            LblInfo.Width = 250;
            LblInfo.Location = new Point(BtnAddStrategy.Right + btnHrzSpace, BtnAddStrategy.Top);

            int btnWideWidth = (ClientSize.Width - 2*border - 4*btnHrzSpace)/3;
            BtnOpenFolder.Size = new Size(btnWideWidth, buttonHeight);
            BtnOpenReport.Size = BtnOpenFolder.Size;

            BtnOpenFolder.Location = new Point(border + btnHrzSpace + btnWideWidth + btnHrzSpace,
                                               Height - buttonHeight - btnVertSpace);
            BtnOpenReport.Location = new Point(BtnOpenFolder.Right + btnHrzSpace, BtnOpenFolder.Top);
        }


        /// <summary>
        ///     Adds the strategy to the cumulative report.
        /// </summary>
        private void BtnStartClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Opens charts screen.
        /// </summary>
        private void ViewChartsClick(object sender, EventArgs e)
        {
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
            //try { System.Diagnostics.Process.Start(pathReportFile); }
            //catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}