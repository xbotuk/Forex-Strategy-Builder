// Forex Strategy Builder - Scanner
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The Scanner
    /// </summary>
    public sealed class Scanner : Form
    {
        // Controls
        private readonly Small_Balance_Chart _balanceChart;
        private readonly Panel _infoPanel;
        private readonly ProgressBar _progressBar;
        private readonly Label _lblProgress;
        private readonly Button _btnClose;
        private readonly CheckBox _chbAutoscan;
        private readonly CheckBox _chbTickScan;
        private readonly BackgroundWorker _bgWorker;

        private readonly Color _colorText;
        private readonly Font _fontInfo;
        private readonly int _infoRowHeight;
        private readonly bool _isTickDataFile;
        private bool _isCompactMode;
        private bool _isLoadingNow;
        private int _progressPercent;
        private string _warningMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        public Scanner()
        {
            _infoPanel = new Panel();
            _balanceChart = new Small_Balance_Chart();
            _progressBar = new ProgressBar();
            _lblProgress = new Label();
            _chbAutoscan = new CheckBox();
            _chbTickScan = new CheckBox();
            _btnClose = new Button();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = _btnClose;
            Text = Language.T("Intrabar Scanner");
            BackColor = LayoutColors.ColorFormBack;
            FormClosing += ScannerFormClosing;

            _colorText = LayoutColors.ColorControlText;
            _fontInfo = new Font(Font.FontFamily, 9);
            _infoRowHeight = Math.Max(_fontInfo.Height, 18);
            _isTickDataFile = CheckTickDataFile();

            // pnlInfo
            _infoPanel.Parent = this;
            _infoPanel.BackColor = LayoutColors.ColorControlBack;
            _infoPanel.Paint += PnlInfoPaint;

            // Small Balance Chart
            _balanceChart.Parent = this;

            // ProgressBar
            _progressBar.Parent = this;

            // Label Progress
            _lblProgress.Parent = this;
            _lblProgress.ForeColor = LayoutColors.ColorControlText;
            _lblProgress.AutoSize = true;

            // Automatic Scan checkbox.
            _chbAutoscan.Parent = this;
            _chbAutoscan.ForeColor = _colorText;
            _chbAutoscan.BackColor = Color.Transparent;
            _chbAutoscan.Text = Language.T("Automatic Scan");
            _chbAutoscan.Checked = Configs.Autoscan;
            _chbAutoscan.AutoSize = true;
            _chbAutoscan.CheckedChanged += ChbAutoscanCheckedChanged;

            // Tick Scan checkbox.
            _chbTickScan.Parent = this;
            _chbTickScan.ForeColor = _colorText;
            _chbTickScan.BackColor = Color.Transparent;
            _chbTickScan.Text = Language.T("Use Ticks");
            _chbTickScan.Checked = Configs.UseTickData && _isTickDataFile;
            _chbTickScan.AutoSize = true;
            _chbTickScan.Visible = _isTickDataFile;
            _chbTickScan.CheckedChanged += ChbTickScanCheckedChanged;

            //Button Close
            _btnClose.Parent = this;
            _btnClose.Name = "Close";
            _btnClose.Text = Language.T("Close");
            _btnClose.DialogResult = DialogResult.OK;
            _btnClose.UseVisualStyleBackColor = true;

            // BackGroundWorker
            _bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.ProgressChanged += BgWorkerProgressChanged;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            _isLoadingNow = false;

            if (!_isTickDataFile)
                Configs.UseTickData = false;

            return;
        }

        /// <summary>
        /// Sets scanner compact mode.
        /// </summary>
        public bool CompactMode
        {
            set { _isCompactMode = value; }
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_isCompactMode)
            {
                _infoPanel.Visible = false;
                _balanceChart.Visible = false;
                _lblProgress.Visible = true;
                _chbAutoscan.Visible = false;

                Width = 300;
                Height = 95;
                TopMost = true;

                StartLoading();
            }
            else
            {
                _lblProgress.Visible = false;
                _chbAutoscan.Visible = true;
                Width = 460;
                Height = 540;
                if (!_isTickDataFile)
                    Height -= _infoRowHeight;
                _balanceChart.SetChartData();
            }

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;

            if (_isCompactMode)
            {
                //Button Close
                _btnClose.Size = new Size(buttonWidth, buttonHeight);
                _btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                               ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                _progressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
                _progressBar.Location = new Point(space, btnVertSpace);

                // Label Progress
                _lblProgress.Location = new Point(space, _btnClose.Top + 5);
            }
            else
            {
                //Button Close
                _btnClose.Size = new Size(buttonWidth, buttonHeight);
                _btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                               ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                _progressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
                _progressBar.Location = new Point(space, _btnClose.Top - _progressBar.Height - btnVertSpace);

                // Panel Info
                int pnlInfoHeight = _isTickDataFile ? _infoRowHeight*11 + 2 : _infoRowHeight*10 + 2;
                _infoPanel.Size = new Size(ClientSize.Width - 2*space, pnlInfoHeight);
                _infoPanel.Location = new Point(space, space);

                // Panel balance chart
                _balanceChart.Size = new Size(ClientSize.Width - 2*space, _progressBar.Top - _infoPanel.Bottom - 2*space);
                _balanceChart.Location = new Point(space, _infoPanel.Bottom + space);

                // Label Progress
                _lblProgress.Location = new Point(space, _btnClose.Top + 5);

                // Auto scan checkbox
                _chbAutoscan.Location = new Point(space, _btnClose.Top + 5);

                // TickScan checkbox
                _chbTickScan.Location = new Point(_chbAutoscan.Right + space, _btnClose.Top + 5);
            }

            return;
        }

        /// <summary>
        /// Loads data and recalculates.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_isCompactMode)
                return;

            if (!Data.IsIntrabarData)
            {
                StartLoading();
            }
            else
            {
                Backtester.Scan();
                ShowScanningResult();
                _progressBar.Value = 100;
                _btnClose.Focus();
            }

            return;
        }

        /// <summary>
        /// Stops the background worker.
        /// </summary>
        private void ScannerFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isLoadingNow)
            {
                // Cancels the asynchronous operation.
                _bgWorker.CancelAsync();
                e.Cancel = true;
            }

            return;
        }

        /// <summary>
        /// Repaint the panel Info
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

            var size = new Size(xp7 - xp0, _infoRowHeight);

            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near};

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(pnl.ClientSize.Width - 0, 2*_infoRowHeight);
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
            float captionWidth = Math.Min(_infoPanel.ClientSize.Width, xp7 - xp0);
            float captionTextWidth = g.MeasureString(stringCaptionText, _fontInfo).Width;
            float captionTextX = Math.Max((captionWidth - captionTextWidth)/2f, 0);
            var pfCaptionText = new PointF(captionTextX, 0);
            var sfCaptionText = new SizeF(captionWidth - captionTextX, _infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            Brush brush = new SolidBrush(LayoutColors.ColorCaptionText);
            // First caption row
            g.DrawString(stringCaptionText, _fontInfo, brush, rectfCaption, stringFormatCaption);

            // Second title row
            g.DrawString(Language.T("Period"), _fontInfo, brush, (xp1 + xp0)/2, _infoRowHeight, sf);
            g.DrawString(Language.T("Bars"), _fontInfo, brush, (xp2 + xp1)/2, _infoRowHeight, sf);
            g.DrawString(Language.T("From"), _fontInfo, brush, (xp3 + xp2)/2, _infoRowHeight, sf);
            g.DrawString(Language.T("Until"), _fontInfo, brush, (xp4 + xp3)/2, _infoRowHeight, sf);
            g.DrawString(Language.T("Coverage"), _fontInfo, brush, (xp5 + xp4)/2, _infoRowHeight, sf);
            g.DrawString("%", _fontInfo, brush, (xp6 + xp5)/2, _infoRowHeight, sf);
            g.DrawString(Language.T("Label"), _fontInfo, brush, (xp7 + xp6)/2, _infoRowHeight, sf);

            brush = new SolidBrush(LayoutColors.ColorControlText);
            int allPeriods = Enum.GetValues(typeof (DataPeriods)).Length;
            for (int period = 0; period <= allPeriods; period++)
            {
                int y = (period + 2)*_infoRowHeight;
                var point = new Point(xp0, y);

                if (period%2f != 0)
                    g.FillRectangle(new SolidBrush(LayoutColors.ColorEvenRowBack), new Rectangle(point, size));
            }

            // Tick statistics
            if (_isTickDataFile)
            {
                g.DrawString(Language.T("Tick"), _fontInfo, brush, (xp1 + xp0)/2, 2*_infoRowHeight, sf);
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

                    int y = 2*_infoRowHeight;
                    string ticks = (Data.Ticks > 999999) ? (Data.Ticks/1000).ToString() + "K" : Data.Ticks.ToString();
                    g.DrawString(ticks, _fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString((firstBarWithTicks + 1).ToString(), _fontInfo, brush, (xp3 + xp2)/2, y, sf);
                    g.DrawString((lastBarWithTicks + 1).ToString(), _fontInfo, brush, (xp4 + xp3)/2, y, sf);
                    g.DrawString(tickBars.ToString(), _fontInfo, brush, (xp5 + xp4)/2, y, sf);
                    g.DrawString(percentage.ToString("F2"), _fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xp6 + 10, y + 7, xp7 - xp6 - 20, 3);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }
            }

            for (int prd = 0; prd < allPeriods; prd++)
            {
                int startY = _isTickDataFile ? 3 : 2;
                int y = (prd + startY)*_infoRowHeight;

                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);
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

                g.DrawString(Data.DataPeriodToString(period), _fontInfo, brush, (xp1 + xp0)/2, y, sf);

                if (coveredBars > 0 || period == Data.Period)
                {
                    g.DrawString(intraBars.ToString(), _fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString(fromBar.ToString(), _fontInfo, brush, (xp3 + xp2)/2, y, sf);
                    g.DrawString(untilBar.ToString(), _fontInfo, brush, (xp4 + xp3)/2, y, sf);
                    g.DrawString(coveredBars.ToString() + (isMultyAreas ? "*" : ""), _fontInfo, brush, (xp5 + xp4)/2, y,
                                 sf);
                    g.DrawString(percentage.ToString("F2"), _fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[period], 60);
                }
            }

            var penLine = new Pen(LayoutColors.ColorJournalLines);
            g.DrawLine(penLine, xp1, 2*_infoRowHeight, xp1, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp2, 2*_infoRowHeight, xp2, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp3, 2*_infoRowHeight, xp3, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp4, 2*_infoRowHeight, xp4, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp5, 2*_infoRowHeight, xp5, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp6, 2*_infoRowHeight, xp6, pnl.ClientSize.Height);

            // Border
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, 2*_infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 2*_infoRowHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);

            return;
        }

        /// <summary>
        /// Starts intrabar data loading.
        /// </summary>
        private void StartLoading()
        {
            if (_isLoadingNow)
            {
                // Cancel the asynchronous operation.
                _bgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            _progressBar.Value = 0;
            _warningMessage = string.Empty;
            _isLoadingNow = true;
            _progressPercent = 0;
            _lblProgress.Visible = true;
            _chbAutoscan.Visible = false;
            _chbTickScan.Visible = false;

            _btnClose.Text = Language.T("Cancel");

            // Start the bgWorker
            _bgWorker.RunWorkerAsync();

            return;
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            LoadData(worker);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 200)
                _progressBar.Style = ProgressBarStyle.Marquee;
            else
                _progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Data.IsIntrabarData ||
                Configs.UseTickData && Data.IsTickData ||
                Data.Period == DataPeriods.min1)
                Backtester.Scan();

            if (!_isCompactMode)
                ShowScanningResult();
            CompleteScanning();

            if (_warningMessage != string.Empty && Configs.CheckData)
                MessageBox.Show(_warningMessage + Environment.NewLine + Environment.NewLine +
                                Language.T("The data is probably incomplete and the scanning may not be reliable!") +
                                Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\"."),
                                Language.T("Scanner"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (_isCompactMode)
                Close();

            return;
        }

        /// <summary>
        /// Updates the chart and info panel.
        /// </summary>
        private void ShowScanningResult()
        {
            _balanceChart.SetChartData();
            _balanceChart.InitChart();
            _balanceChart.Invalidate();
            _infoPanel.Invalidate();

            _chbAutoscan.Visible = true;
            _chbTickScan.Visible = Configs.UseTickData || _isTickDataFile;
        }

        /// <summary>
        /// Resets controls after loading data.
        /// </summary>
        private void CompleteScanning()
        {
            _progressBar.Style = ProgressBarStyle.Blocks;

            _lblProgress.Text = string.Empty;
            _lblProgress.Visible = false;

            _btnClose.Text = Language.T("Close");
            Cursor = Cursors.Default;
            _isLoadingNow = false;
            _btnClose.Focus();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData(BackgroundWorker worker)
        {
            int periodsToLoad = 0;
            int allPeriods = Enum.GetValues(typeof (DataPeriods)).Length;
            Data.IntraBars = new int[allPeriods];
            Data.IntraBarData = new Bar[Data.Bars][];
            Data.IntraBarBars = new int[Data.Bars];
            Data.IntraBarsPeriods = new DataPeriods[Data.Bars];
            Data.LoadedIntraBarPeriods = 0;

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                Data.IntraBarsPeriods[bar] = Data.Period;
                Data.IntraBarBars[bar] = 0;
            }

            // Counts how many periods to load
            for (int prd = 0; prd < allPeriods; prd++)
            {
                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);
                if (period < Data.Period)
                {
                    periodsToLoad++;
                }
            }

            // Load the intrabar data (Starts from 1 Min)
            for (int prd = 0; prd < allPeriods && _isLoadingNow; prd++)
            {
                if (worker.CancellationPending) break;

                int loadedBars = 0;
                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);

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
                if (percentComplete > _progressPercent)
                {
                    _progressPercent = percentComplete;
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

            return;
        }

        /// <summary>
        /// Loads the Intrabar data.
        /// </summary>
        private int LoadIntrabarData(DataPeriods period)
        {
            var instrument = new Instrument(Data.InstrProperties.Clone(), (int) period)
                                 {
                                     DataDir = Data.OfflineDataDir,
                                     FormatDate = DateFormat.Unknown,
                                     MaxBars = Configs.MAX_INTRA_BARS
                                 };


            // Loads the data
            int loadingResult = instrument.LoadData();
            int loadedIntrabars = instrument.Bars;

            if (loadingResult == 0 && loadedIntrabars > 0)
            {
                if (Data.Period != DataPeriods.week)
                {
                    if (instrument.DaysOff > 5)
                        _warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                           Data.DataPeriodToString(period) + " - " + Language.T("Maximum days off:") +
                                           " " + instrument.DaysOff;
                    if (Data.Update - instrument.Update > new TimeSpan(24, 0, 0))
                        _warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                           Data.DataPeriodToString(period) + " - " + Language.T("Updated on:") + " " +
                                           instrument.Update.ToString();
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
        /// Checks the intrabar data.
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
                    _warningMessage += Environment.NewLine +
                                       Language.T("There is no intrabar data from bar No:") + " " +
                                       (bar + 1) + " - " + Data.Time[bar];
                }
            }

            return;
        }

        /// <summary>
        /// Repairs the intrabar data.
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

            return;
        }

        /// <summary>
        /// Loads available tick data.
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
                        (Math.Abs(Data.Open[bar] - bidTicks[0]) < 10*Data.InstrProperties.Pip))
                        Data.TickData[bar] = bidTicks;
                    else
                        AddTickData(bar, bidTicks);
                }

                totalVolume += volume;
            }

            binaryReader.Close();
            fileStream.Close();

            Data.IsTickData = false;
            var barsWithTicks = 0;
            for (var b = 0; b < Data.Bars; b++)
                if (Data.TickData[b] != null)
                    barsWithTicks++;

            if (barsWithTicks > 0)
            {
                Data.Ticks = totalVolume;
                Data.IsTickData = true;
            }
        }

        /// <summary>
        /// Determines whether a tick data file exists.
        /// </summary>
        private bool CheckTickDataFile()
        {
            return File.Exists(Data.OfflineDataDir + Data.Symbol + "0.bin");
        }

        /// <summary>
        /// Adds tick data to Data
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
        /// Export tick data to a .CSV file.
        /// </summary>
        private void ExportTickToCSV()
        {
            using (var sw = new StreamWriter(Data.OfflineDataDir + Data.Symbol + "0.csv"))
            {
                for (var bar = 0; bar < Data.Bars; bar++)
                {
                    if (Data.TickData[bar] == null)
                    {
                        sw.WriteLine((bar + 1).ToString() + "\t" +
                                     Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t" +
                                     Data.Time[bar].DayOfWeek);
                    }
                    else
                    {
                        sw.Write((bar + 1) + "\t" +
                                 Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t");
                        foreach (var tick in Data.TickData[bar])
                            sw.Write(tick.ToString("F5") + "\t");
                        sw.WriteLine();
                    }
                }
                sw.Close();
            }
        }

        /// <summary>
        /// Sets the lblProgress.Text.
        /// </summary>
        private void SetLabelProgressText(string text)
        {
            if (_lblProgress.InvokeRequired)
            {
                Invoke(new SetLabelProgressCallback(SetLabelProgressText), new object[] {text});
            }
            else
            {
                _lblProgress.Text = text;
            }
        }

        /// <summary>
        /// Auto scan checkbox.
        /// </summary>
        private void ChbAutoscanCheckedChanged(object sender, EventArgs e)
        {
            Configs.Autoscan = _chbAutoscan.Checked;

            return;
        }

        /// <summary>
        /// Tick scan checkbox.
        /// </summary>
        private void ChbTickScanCheckedChanged(object sender, EventArgs e)
        {
            Configs.UseTickData = _chbTickScan.Checked;
            StartLoading();

            return;
        }

        #region Nested type: SetLabelProgressCallback

        private delegate void SetLabelProgressCallback(string text);

        #endregion
    }
}
