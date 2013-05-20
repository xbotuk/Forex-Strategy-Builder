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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     The Scanner
    /// </summary>
    public sealed class Scanner : Form
    {
        // Controls

        private readonly Color colorText;
        private readonly Font fontInfo;
        private readonly int infoRowHeight;
        private readonly bool isTickDataFile;
        private bool isLoadingNow;
        private int progressPercent;
        private string warningMessage;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Scanner()
        {
            InfoPanel = new Panel();
            BalanceChart = new SmallBalanceChart();
            ProgressBar = new ProgressBar();
            LblProgress = new Label();
            ChbAutoscan = new CheckBox();
            ChbTickScan = new CheckBox();
            BtnClose = new Button();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnClose;
            Text = Language.T("Intrabar Scanner");
            BackColor = LayoutColors.ColorFormBack;
            FormClosing += ScannerFormClosing;

            colorText = LayoutColors.ColorControlText;
            fontInfo = new Font(Font.FontFamily, 9);
            infoRowHeight = Math.Max(fontInfo.Height, 18);
            isTickDataFile = CheckTickDataFile();

            // pnlInfo
            InfoPanel.Parent = this;
            InfoPanel.BackColor = LayoutColors.ColorControlBack;
            InfoPanel.Paint += PnlInfoPaint;

            // Small Balance Chart
            BalanceChart.Parent = this;

            // ProgressBar
            ProgressBar.Parent = this;

            // Label Progress
            LblProgress.Parent = this;
            LblProgress.ForeColor = LayoutColors.ColorControlText;
            LblProgress.AutoSize = true;

            // Automatic Scan checkbox.
            ChbAutoscan.Parent = this;
            ChbAutoscan.ForeColor = colorText;
            ChbAutoscan.BackColor = Color.Transparent;
            ChbAutoscan.Text = Language.T("Automatic Scan");
            ChbAutoscan.Checked = Configs.Autoscan;
            ChbAutoscan.AutoSize = true;
            ChbAutoscan.CheckedChanged += ChbAutoscanCheckedChanged;

            // Tick Scan checkbox.
            ChbTickScan.Parent = this;
            ChbTickScan.ForeColor = colorText;
            ChbTickScan.BackColor = Color.Transparent;
            ChbTickScan.Text = Language.T("Use Ticks");
            ChbTickScan.Checked = Configs.UseTickData && isTickDataFile;
            ChbTickScan.AutoSize = true;
            ChbTickScan.Visible = isTickDataFile;
            ChbTickScan.CheckedChanged += ChbTickScanCheckedChanged;

            //Button Close
            BtnClose.Parent = this;
            BtnClose.Name = "Close";
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.OK;
            BtnClose.UseVisualStyleBackColor = true;

            // BackGroundWorker
            BgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            BgWorker.DoWork += BgWorkerDoWork;
            BgWorker.ProgressChanged += BgWorkerProgressChanged;
            BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            isLoadingNow = false;

            if (!isTickDataFile)
                Configs.UseTickData = false;
        }

        private SmallBalanceChart BalanceChart { get; set; }
        private Panel InfoPanel { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private Label LblProgress { get; set; }
        private Button BtnClose { get; set; }
        private CheckBox ChbAutoscan { get; set; }
        private CheckBox ChbTickScan { get; set; }
        private BackgroundWorker BgWorker { get; set; }

        /// <summary>
        ///     Sets scanner compact mode.
        /// </summary>
        public bool CompactMode { private get; set; }

        /// <summary>
        ///     Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (CompactMode)
            {
                InfoPanel.Visible = false;
                BalanceChart.Visible = false;
                LblProgress.Visible = true;
                ChbAutoscan.Visible = false;

                Width = 300;
                Height = 95;
                TopMost = true;

                StartLoading();
            }
            else
            {
                LblProgress.Visible = false;
                ChbAutoscan.Visible = true;
                BalanceChart.SetChartData();
                Width = 460;
                Height = 540;
                if (!isTickDataFile)
                    Height -= infoRowHeight;
            }
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;

            if (CompactMode)
            {
                //Button Close
                BtnClose.Size = new Size(buttonWidth, buttonHeight);
                BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                              ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDlu*9));
                ProgressBar.Location = new Point(space, btnVertSpace);

                // Label Progress
                LblProgress.Location = new Point(space, BtnClose.Top + 5);
            }
            else
            {
                //Button Close
                BtnClose.Size = new Size(buttonWidth, buttonHeight);
                BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                              ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDlu*9));
                ProgressBar.Location = new Point(space, BtnClose.Top - ProgressBar.Height - btnVertSpace);

                // Panel Info
                int pnlInfoHeight = isTickDataFile ? infoRowHeight*11 + 2 : infoRowHeight*10 + 2;
                InfoPanel.Size = new Size(ClientSize.Width - 2*space, pnlInfoHeight);
                InfoPanel.Location = new Point(space, space);

                // Panel balance chart
                BalanceChart.Size = new Size(ClientSize.Width - 2*space, ProgressBar.Top - InfoPanel.Bottom - 2*space);
                BalanceChart.Location = new Point(space, InfoPanel.Bottom + space);

                // Label Progress
                LblProgress.Location = new Point(space, BtnClose.Top + 5);

                // Auto scan checkbox
                ChbAutoscan.Location = new Point(space, BtnClose.Top + 5);

                // TickScan checkbox
                ChbTickScan.Location = new Point(ChbAutoscan.Right + space, BtnClose.Top + 5);
            }
        }

        /// <summary>
        ///     Loads data and recalculates.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (CompactMode)
                return;

            if (!Data.IsIntrabarData)
            {
                StartLoading();
            }
            else
            {
                Backtester.Scan();
                ShowScanningResult();
                ProgressBar.Value = 100;
                BtnClose.Focus();
            }
        }

        /// <summary>
        ///     Stops the background worker.
        /// </summary>
        private void ScannerFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isLoadingNow) return;
            // Cancels the asynchronous operation.
            BgWorker.CancelAsync();
            e.Cancel = true;
        }

        /// <summary>
        ///     Repaint the panel Info
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            // +------------------------------------------------------+
            // |                   Data                               |
            // |------------------- ----------------------------------+
            // | Period  | Bars  | From | Until | Cover |  %  | Label |
            // |------------------------------------------------------+
            //xp0       xp1     xp2    xp3     xp4     xp5   xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            const int border = 2;
            const int xp0 = border;
            const int xp1 = 80;
            const int xp2 = 140;
            const int xp3 = 200;
            const int xp4 = 260;
            const int xp5 = 320;
            const int xp6 = 370;
            int xp7 = pnl.ClientSize.Width - border;

            var size = new Size(xp7 - xp0, infoRowHeight);

            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near};

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(pnl.ClientSize.Width - 0, 2*infoRowHeight);
            var rectfCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormatCaption = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Alignment = StringAlignment.Near
                };
            string stringCaptionText = Language.T("Intrabar Data");
            float captionWidth = Math.Min(InfoPanel.ClientSize.Width, xp7 - xp0);
            float captionTextWidth = g.MeasureString(stringCaptionText, fontInfo).Width;
            float captionTextX = Math.Max((captionWidth - captionTextWidth)/2f, 0);
            var pfCaptionText = new PointF(captionTextX, 0);
            var sfCaptionText = new SizeF(captionWidth - captionTextX, infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            Brush brush = new SolidBrush(LayoutColors.ColorCaptionText);
            // First caption row
            g.DrawString(stringCaptionText, fontInfo, brush, rectfCaption, stringFormatCaption);

            // Second title row
            g.DrawString(Language.T("Period"), fontInfo, brush, (xp1 + xp0)/2f, infoRowHeight, sf);
            g.DrawString(Language.T("Bars"), fontInfo, brush, (xp2 + xp1)/2f, infoRowHeight, sf);
            g.DrawString(Language.T("From"), fontInfo, brush, (xp3 + xp2)/2f, infoRowHeight, sf);
            g.DrawString(Language.T("Until"), fontInfo, brush, (xp4 + xp3)/2f, infoRowHeight, sf);
            g.DrawString(Language.T("Coverage"), fontInfo, brush, (xp5 + xp4)/2f, infoRowHeight, sf);
            g.DrawString("%", fontInfo, brush, (xp6 + xp5)/2f, infoRowHeight, sf);
            g.DrawString(Language.T("Label"), fontInfo, brush, (xp7 + xp6)/2f, infoRowHeight, sf);

            brush = new SolidBrush(LayoutColors.ColorControlText);
            int allPeriods = Enum.GetValues(typeof (DataPeriod)).Length;
            for (int period = 0; period <= allPeriods; period++)
            {
                int y = (period + 2)*infoRowHeight;
                var point = new Point(xp0, y);

                if (Math.Abs(period%2f - 0) > 0.0001)
                    g.FillRectangle(new SolidBrush(LayoutColors.ColorEvenRowBack), new Rectangle(point, size));
            }

            // Tick statistics
            if (isTickDataFile)
            {
                g.DrawString(Language.T("Tick"), fontInfo, brush, (xp1 + xp0)/2, 2*infoRowHeight, sf);
                if (Data.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    int tickBars = 0;
                    for (int b = 0; b < Data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                        {
                            lastBarWithTicks = b;
                            tickBars++;
                        }
                    }
                    double percentage = 100d*tickBars/Data.Bars;

                    int y = 2*infoRowHeight;
                    string ticks = (Data.Ticks > 999999)
                                       ? (Data.Ticks/1000).ToString(CultureInfo.InvariantCulture) + "K"
                                       : Data.Ticks.ToString(CultureInfo.InvariantCulture);
                    g.DrawString(ticks, fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString((firstBarWithTicks + 1).ToString(CultureInfo.InvariantCulture), fontInfo, brush,
                                 (xp3 + xp2)/2, y, sf);
                    g.DrawString((lastBarWithTicks + 1).ToString(CultureInfo.InvariantCulture), fontInfo, brush,
                                 (xp4 + xp3)/2, y, sf);
                    g.DrawString(tickBars.ToString(CultureInfo.InvariantCulture), fontInfo, brush, (xp5 + xp4)/2, y, sf);
                    g.DrawString(percentage.ToString("F2"), fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriod.M1], 60);
                    rectf = new RectangleF(xp6 + 10, y + 7, xp7 - xp6 - 20, 3);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriod.D1], 60);
                }
            }

            for (int prd = 0; prd < allPeriods; prd++)
            {
                int startY = isTickDataFile ? 3 : 2;
                int y = (prd + startY)*infoRowHeight;

                var period = (DataPeriod) Enum.GetValues(typeof (DataPeriod)).GetValue(prd);
                int intraBars = Data.IntraBars == null || !Data.IsIntrabarData ? 0 : Data.IntraBars[prd];
                int fromBar = 0;
                int untilBar = 0;
                int coveredBars = 0;
                double percentage = 0;

                bool isMultyAreas = false;
                if (intraBars > 0)
                {
                    bool isFromBarFound = false;
                    bool isUntilBarFound = false;
                    untilBar = Data.Bars;
                    for (int bar = 0; bar < Data.Bars; bar++)
                    {
                        if (!isFromBarFound && Data.IntraBarsPeriods[bar] == period)
                        {
                            fromBar = bar;
                            isFromBarFound = true;
                        }
                        if (isFromBarFound && !isUntilBarFound &&
                            (Data.IntraBarsPeriods[bar] != period || bar == Data.Bars - 1))
                        {
                            if (bar < Data.Bars - 1)
                            {
                                isUntilBarFound = true;
                                untilBar = bar;
                            }
                            else
                            {
                                untilBar = Data.Bars;
                            }
                            coveredBars = untilBar - fromBar;
                        }
                        if (isFromBarFound && isUntilBarFound && Data.IntraBarsPeriods[bar] == period)
                        {
                            isMultyAreas = true;
                            coveredBars++;
                        }
                    }
                    if (isFromBarFound)
                    {
                        percentage = 100d*coveredBars/Data.Bars;
                        fromBar++;
                    }
                    else
                    {
                        fromBar = 0;
                        untilBar = 0;
                        coveredBars = 0;
                        percentage = 0;
                    }
                }
                else if (period == Data.Period)
                {
                    intraBars = Data.Bars;
                    fromBar = 1;
                    untilBar = Data.Bars;
                    coveredBars = Data.Bars;
                    percentage = 100;
                }

                g.DrawString(Data.DataPeriodToString(period), fontInfo, brush, (xp1 + xp0)/2, y, sf);

                if (coveredBars > 0 || period == Data.Period)
                {
                    g.DrawString(intraBars.ToString(CultureInfo.InvariantCulture), fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString(fromBar.ToString(CultureInfo.InvariantCulture), fontInfo, brush, (xp3 + xp2)/2, y, sf);
                    g.DrawString(untilBar.ToString(CultureInfo.InvariantCulture), fontInfo, brush, (xp4 + xp3)/2, y, sf);
                    g.DrawString(coveredBars.ToString(CultureInfo.InvariantCulture) + (isMultyAreas ? "*" : ""),
                                 fontInfo, brush, (xp5 + xp4)/2, y, sf);
                    g.DrawString(percentage.ToString("F2"), fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[period], 60);
                }
            }

            var penLine = new Pen(LayoutColors.ColorJournalLines);
            g.DrawLine(penLine, xp1, 2*infoRowHeight, xp1, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp2, 2*infoRowHeight, xp2, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp3, 2*infoRowHeight, xp3, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp4, 2*infoRowHeight, xp4, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp5, 2*infoRowHeight, xp5, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp6, 2*infoRowHeight, xp6, pnl.ClientSize.Height);

            // Border
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, 2*infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 2*infoRowHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);
        }

        /// <summary>
        ///     Starts intrabar data loading.
        /// </summary>
        private void StartLoading()
        {
            if (isLoadingNow)
            {
                // Cancel the asynchronous operation.
                BgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            ProgressBar.Value = 0;
            warningMessage = string.Empty;
            isLoadingNow = true;
            progressPercent = 0;
            LblProgress.Visible = true;
            ChbAutoscan.Visible = false;
            ChbTickScan.Visible = false;

            BtnClose.Text = Language.T("Cancel");

            // Start the bgWorker
            BgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            LoadData(worker);
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 200)
                ProgressBar.Style = ProgressBarStyle.Marquee;
            else
                ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Data.IsIntrabarData || Configs.UseTickData && Data.IsTickData || Data.Period == DataPeriod.M1)
                Backtester.Scan();

            if (!CompactMode)
                ShowScanningResult();
            CompleteScanning();

            if (warningMessage != string.Empty && Configs.CheckData)
                MessageBox.Show(warningMessage + Environment.NewLine + Environment.NewLine +
                                Language.T("The data is probably incomplete and the scanning may not be reliable!") +
                                Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\"."),
                                Language.T("Scanner"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (CompactMode)
                Close();
        }

        /// <summary>
        ///     Updates the chart and info panel.
        /// </summary>
        private void ShowScanningResult()
        {
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            InfoPanel.Invalidate();

            ChbAutoscan.Visible = true;
            ChbTickScan.Visible = Configs.UseTickData || isTickDataFile;
        }

        /// <summary>
        ///     Resets controls after loading data.
        /// </summary>
        private void CompleteScanning()
        {
            ProgressBar.Style = ProgressBarStyle.Blocks;

            LblProgress.Text = string.Empty;
            LblProgress.Visible = false;

            BtnClose.Text = Language.T("Close");
            Cursor = Cursors.Default;
            isLoadingNow = false;
            BtnClose.Focus();
        }

        /// <summary>
        ///     Loads the data.
        /// </summary>
        private void LoadData(BackgroundWorker worker)
        {
            int periodsToLoad = 0;
            int allPeriods = Enum.GetValues(typeof (DataPeriod)).Length;
            Data.IntraBars = new int[allPeriods];
            Data.IntraBarData = new Bar[Data.Bars][];
            Data.IntraBarBars = new int[Data.Bars];
            Data.IntraBarsPeriods = new DataPeriod[Data.Bars];
            Data.LoadedIntraBarPeriods = 0;

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                Data.IntraBarsPeriods[bar] = Data.Period;
                Data.IntraBarBars[bar] = 0;
            }

            // Counts how many periods to load
            for (int prd = 0; prd < allPeriods; prd++)
            {
                var period = (DataPeriod) Enum.GetValues(typeof (DataPeriod)).GetValue(prd);
                if (period < Data.Period)
                {
                    periodsToLoad++;
                }
            }

            // Load the intrabar data (Starts from 1 Min)
            for (int prd = 0; prd < allPeriods && isLoadingNow; prd++)
            {
                if (worker.CancellationPending) break;

                int loadedBars = 0;
                var period = (DataPeriod) Enum.GetValues(typeof (DataPeriod)).GetValue(prd);

                SetLabelProgressText(Language.T("Loading:") + " " + Data.DataPeriodToString(period) + "...");

                if (period < Data.Period)
                {
                    loadedBars = LoadIntrabarData(period);
                    if (loadedBars > 0)
                    {
                        Data.IsIntrabarData = true;
                        Data.LoadedIntraBarPeriods++;
                    }
                }
                else if (period == Data.Period)
                {
                    loadedBars = Data.Bars;
                    Data.LoadedIntraBarPeriods++;
                }

                Data.IntraBars[prd] = loadedBars;

                // Report progress as a percentage of the total task.
                int percentComplete = periodsToLoad > 0 ? 100*(prd + 1)/periodsToLoad : 100;
                percentComplete = percentComplete > 100 ? 100 : percentComplete;
                if (percentComplete > progressPercent)
                {
                    progressPercent = percentComplete;
                    worker.ReportProgress(percentComplete);
                }
            }

            CheckIntrabarData();
            RepairIntrabarData();

            if (Configs.UseTickData)
            {
                SetLabelProgressText(Language.T("Loading:") + " " + Language.T("Ticks") + "...");
                worker.ReportProgress(200);
                try
                {
                    LoadTickData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        ///     Loads the Intrabar data.
        /// </summary>
        private int LoadIntrabarData(DataPeriod period)
        {
            var instrument = new Instrument(Data.InstrProperties.Clone(), (int) period)
                {
                    DataDir = Data.OfflineDataDir,
                    MaxBars = Configs.MaxIntraBars
                };


            // Loads the data
            int loadingResult = instrument.LoadData();
            int loadedIntrabars = instrument.Bars;

            if (loadingResult == 0 && loadedIntrabars > 0)
            {
                if (Data.Period != DataPeriod.W1)
                {
                    if (instrument.DaysOff > 5)
                        warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                          Data.DataPeriodToString(period) + " - " + Language.T("Maximum days off:") +
                                          " " + instrument.DaysOff;
                    if (Data.Update - instrument.Update > new TimeSpan(24, 0, 0))
                        warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                          Data.DataPeriodToString(period) + " - " + Language.T("Updated on:") + " " +
                                          instrument.Update.ToString(CultureInfo.InvariantCulture);
                }

                int startBigBar;
                for (startBigBar = 0; startBigBar < Data.Bars; startBigBar++)
                    if (Data.Time[startBigBar] >= instrument.Time(0))
                        break;

                int stopBigBar;
                for (stopBigBar = startBigBar; stopBigBar < Data.Bars; stopBigBar++)
                    if (Data.IntraBarsPeriods[stopBigBar] != Data.Period)
                        break;

                // Seek for a place to put the intrabars.
                int lastIntraBar = 0;
                for (int bar = startBigBar; bar < stopBigBar; bar++)
                {
                    Data.IntraBarData[bar] = new Bar[(int) Data.Period/(int) period];
                    DateTime endTime = Data.Time[bar] + new TimeSpan(0, (int) Data.Period, 0);
                    int indexBar = 0;
                    for (int intrabar = lastIntraBar;
                         intrabar < loadedIntrabars && instrument.Time(intrabar) < endTime;
                         intrabar++)
                    {
                        if (instrument.Time(intrabar) >= Data.Time[bar])
                        {
                            Data.IntraBarData[bar][indexBar].Time = instrument.Time(intrabar);
                            Data.IntraBarData[bar][indexBar].Open = instrument.Open(intrabar);
                            Data.IntraBarData[bar][indexBar].High = instrument.High(intrabar);
                            Data.IntraBarData[bar][indexBar].Low = instrument.Low(intrabar);
                            Data.IntraBarData[bar][indexBar].Close = instrument.Close(intrabar);
                            Data.IntraBarsPeriods[bar] = period;
                            Data.IntraBarBars[bar]++;
                            indexBar++;
                            lastIntraBar = intrabar;
                        }
                    }
                }
            }

            return loadedIntrabars;
        }

        /// <summary>
        ///     Checks the intrabar data.
        /// </summary>
        private void CheckIntrabarData()
        {
            int inraBarDataStarts = 0;
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (inraBarDataStarts == 0 && Data.IntraBarsPeriods[bar] != Data.Period)
                    inraBarDataStarts = bar;

                if (inraBarDataStarts > 0 && Data.IntraBarsPeriods[bar] == Data.Period)
                {
                    inraBarDataStarts = 0;
                    warningMessage += Environment.NewLine +
                                      Language.T("There is no intrabar data from bar No:") + " " +
                                      (bar + 1) + " - " + Data.Time[bar];
                }
            }
        }

        /// <summary>
        ///     Repairs the intrabar data.
        /// </summary>
        private void RepairIntrabarData()
        {
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (Data.IntraBarsPeriods[bar] != Data.Period)
                {
                    // We have intrabar data here

                    // Repair the Opening prices
                    double price = Data.Open[bar];
                    int b = 0;
                    Data.IntraBarData[bar][b].Open = Data.Open[bar];
                    if (price > Data.IntraBarData[bar][b].High &&
                        price > Data.IntraBarData[bar][b].Low)
                    {
                        // Adjust the High price
                        Data.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.IntraBarData[bar][b].High &&
                             price < Data.IntraBarData[bar][b].Low)
                    {
                        // Adjust the Low price
                        Data.IntraBarData[bar][b].Low = price;
                    }

                    // Repair the Closing prices
                    price = Data.Close[bar];
                    b = Data.IntraBarBars[bar] - 1;
                    Data.IntraBarData[bar][b].Close = Data.Close[bar];
                    if (price > Data.IntraBarData[bar][b].High &&
                        price > Data.IntraBarData[bar][b].Low)
                    {
                        // Adjust the High price
                        Data.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.IntraBarData[bar][b].High &&
                             price < Data.IntraBarData[bar][b].Low)
                    {
                        // Adjust the Low price
                        Data.IntraBarData[bar][b].Low = price;
                    }

                    int minIntrabar = -1; // Contains the min price
                    int maxIntrabar = -1; // Contains the max price
                    double minPrice = double.MaxValue;
                    double maxPrice = double.MinValue;

                    for (b = 0; b < Data.IntraBarBars[bar]; b++)
                    {
                        // Find min and max
                        if (Data.IntraBarData[bar][b].Low < minPrice)
                        {
                            // Min found
                            minPrice = Data.IntraBarData[bar][b].Low;
                            minIntrabar = b;
                        }
                        if (Data.IntraBarData[bar][b].High > maxPrice)
                        {
                            // Max found
                            maxPrice = Data.IntraBarData[bar][b].High;
                            maxIntrabar = b;
                        }
                        if (b > 0)
                        {
                            // Repair the Opening prices
                            price = Data.IntraBarData[bar][b - 1].Close;
                            Data.IntraBarData[bar][b].Open = price;
                            if (price > Data.IntraBarData[bar][b].High &&
                                price > Data.IntraBarData[bar][b].Low)
                            {
                                // Adjust the High price
                                Data.IntraBarData[bar][b].High = price;
                            }
                            else if (price < Data.IntraBarData[bar][b].High &&
                                     price < Data.IntraBarData[bar][b].Low)
                            {
                                // Adjust the Low price
                                Data.IntraBarData[bar][b].Low = price;
                            }
                        }
                    }

                    if (minPrice > Data.Low[bar]) // Repair the Bottom
                        Data.IntraBarData[bar][minIntrabar].Low = Data.Low[bar];
                    if (maxPrice < Data.High[bar]) // Repair the Top
                        Data.IntraBarData[bar][maxIntrabar].High = Data.High[bar];
                }
            }
        }

        /// <summary>
        ///     Loads available tick data.
        /// </summary>
        private void LoadTickData()
        {
            var fileStream = new FileStream(Data.OfflineDataDir + Data.Symbol + "0.bin", FileMode.Open);
            var binaryReader = new BinaryReader(fileStream);
            Data.TickData = new double[Data.Bars][];
            int bar = 0;

            long totalVolume = 0;

            long pos = 0;
            long length = binaryReader.BaseStream.Length;
            while (pos < length)
            {
                DateTime time = DateTime.FromBinary(binaryReader.ReadInt64());
                pos += sizeof (Int64);

                int volume = binaryReader.ReadInt32();
                pos += sizeof (Int32);

                int count = binaryReader.ReadInt32();
                pos += sizeof (Int32);

                var bidTicks = new double[count];
                for (int i = 0; i < count; i++)
                    bidTicks[i] = binaryReader.ReadDouble();
                pos += count*sizeof (Double);

                while (bar < Data.Bars - 1 && Data.Time[bar] < time)
                {
                    if (time < Data.Time[bar + 1])
                        break;
                    bar++;
                }

                if (time == Data.Time[bar])
                {
                    Data.TickData[bar] = bidTicks;
                }
                else if ((bar < Data.Bars - 1 && time > Data.Time[bar] && time < Data.Time[bar + 1]) ||
                         bar == Data.Bars - 1)
                {
                    if (Data.TickData[bar] == null &&
                        (Math.Abs(Data.Open[bar] - bidTicks[0]) < 10*Data.InstrProperties.Point))
                        Data.TickData[bar] = bidTicks;
                    else
                        AddTickData(bar, bidTicks);
                }

                totalVolume += volume;
            }

            binaryReader.Close();
            fileStream.Close();

            Data.IsTickData = false;
            int barsWithTicks = 0;
            for (int b = 0; b < Data.Bars; b++)
                if (Data.TickData[b] != null)
                    barsWithTicks++;

            if (barsWithTicks > 0)
            {
                Data.Ticks = totalVolume;
                Data.IsTickData = true;
            }
        }

        /// <summary>
        ///     Determines whether a tick data file exists.
        /// </summary>
        private bool CheckTickDataFile()
        {
            return File.Exists(Data.OfflineDataDir + Data.Symbol + "0.bin");
        }

        /// <summary>
        ///     Adds tick data to Data
        /// </summary>
        private void AddTickData(int bar, double[] bidTicks)
        {
            if (Data.TickData[bar] == null) return;
            int oldLenght = Data.TickData[bar].Length;
            int ticksAdd = bidTicks.Length;
            Array.Resize(ref Data.TickData[bar], oldLenght + ticksAdd);
            Array.Copy(bidTicks, 0, Data.TickData[bar], oldLenght, ticksAdd);
        }

        /// <summary>
        ///     Export tick data to a .CSV file.
        /// </summary>
        private void ExportTickToCSV()
        {
            using (var sw = new StreamWriter(Data.OfflineDataDir + Data.Symbol + "0.csv"))
            {
                for (int bar = 0; bar < Data.Bars; bar++)
                {
                    if (Data.TickData[bar] == null)
                    {
                        sw.WriteLine((bar + 1).ToString(CultureInfo.InvariantCulture) + "\t" +
                                     Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t" +
                                     Data.Time[bar].DayOfWeek);
                    }
                    else
                    {
                        sw.Write((bar + 1) + "\t" + Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t");
                        foreach (double tick in Data.TickData[bar])
                            sw.Write(tick.ToString("F5") + "\t");
                        sw.WriteLine();
                    }
                }
                sw.Close();
            }
        }

        /// <summary>
        ///     Sets the lblProgress.Text.
        /// </summary>
        private void SetLabelProgressText(string text)
        {
            if (LblProgress.InvokeRequired)
            {
                Invoke(new SetLabelProgressCallback(SetLabelProgressText), new object[] {text});
            }
            else
            {
                LblProgress.Text = text;
            }
        }

        /// <summary>
        ///     Auto scan checkbox.
        /// </summary>
        private void ChbAutoscanCheckedChanged(object sender, EventArgs e)
        {
            Configs.Autoscan = ChbAutoscan.Checked;
        }

        /// <summary>
        ///     Tick scan checkbox.
        /// </summary>
        private void ChbTickScanCheckedChanged(object sender, EventArgs e)
        {
            Configs.UseTickData = ChbTickScan.Checked;
            StartLoading();
        }

        #region Nested type: SetLabelProgressCallback

        private delegate void SetLabelProgressCallback(string text);

        #endregion
    }
}