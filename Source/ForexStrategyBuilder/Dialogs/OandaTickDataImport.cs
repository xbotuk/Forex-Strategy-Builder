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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Dialogs
{
    public sealed class OandaTickDataImport : Form
    {
        private readonly BackgroundWorker bgWorker;
        private readonly string outFolder = Configs.OandaImportDestFolder;
        private bool isImporting;
        private string lastFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private List<Bar> minuteBarList;
        private string oandaFile;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public OandaTickDataImport()
        {
            LblIntro = new Label();
            TxbFileName = new TextBox();
            BtnBrowse = new Button();
            LblSymbol = new Label();
            TxbSymbol = new TextBox();
            LblMinBars = new Label();
            NudMinBars = new NumericUpDown();
            PnlSettings = new FancyPanel();
            PnlImportInfo = new FancyPanel(Language.T("Imported Data"));
            TbxInfo = new TextBox();
            LblDestFolder = new Label();
            TxbDestFolder = new TextBox();
            BtnDestFolder = new Button();
            BtnHelp = new Button();
            BtnClose = new Button();
            BtnImport = new Button();
            ProgressBar = new ProgressBar();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnImport;
            CancelButton = BtnClose;
            Text = Language.T("Oanda Import");

            // Label Intro
            LblIntro.Parent = PnlSettings;
            LblIntro.ForeColor = LayoutColors.ColorControlText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.AutoSize = true;
            LblIntro.Text = Language.T("Select an Oanda file for import") + ":";

            // File Name
            TxbFileName.Parent = PnlSettings;
            TxbFileName.BackColor = LayoutColors.ColorControlBack;
            TxbFileName.ForeColor = LayoutColors.ColorControlText;
            TxbFileName.Text = string.Empty;

            // Button Browse
            BtnBrowse.Parent = PnlSettings;
            BtnBrowse.Name = "Browse";
            BtnBrowse.Text = Language.T("Browse");
            BtnBrowse.Click += BtnBrowseClick;
            BtnBrowse.UseVisualStyleBackColor = true;

            // Label Symbol
            LblSymbol.Parent = PnlSettings;
            LblSymbol.ForeColor = LayoutColors.ColorControlText;
            LblSymbol.BackColor = Color.Transparent;
            LblSymbol.AutoSize = true;
            LblSymbol.Text = Language.T("Enter Symbol") + ":";

            // Symbol
            TxbSymbol.Parent = PnlSettings;
            TxbSymbol.BackColor = LayoutColors.ColorControlBack;
            TxbSymbol.ForeColor = LayoutColors.ColorControlText;
            TxbSymbol.Text = string.Empty;

            // Label Symbol
            LblMinBars.Parent = PnlSettings;
            LblMinBars.ForeColor = LayoutColors.ColorControlText;
            LblMinBars.BackColor = Color.Transparent;
            LblMinBars.AutoSize = true;
            LblMinBars.Text = Language.T("Minimum bars in bar files") + ":";

            // Minimum Bars
            NudMinBars.Parent = PnlSettings;
            NudMinBars.BeginInit();
            NudMinBars.Minimum = 0;
            NudMinBars.Maximum = int.MaxValue;
            NudMinBars.Value = Configs.MinBarsInBarFile;
            NudMinBars.EndInit();

            // LblDestFolder
            LblDestFolder.Parent = PnlSettings;
            LblDestFolder.ForeColor = LayoutColors.ColorControlText;
            LblDestFolder.BackColor = Color.Transparent;
            LblDestFolder.AutoSize = true;
            LblDestFolder.Text = Language.T("Select a destination folder") + ":";

            // TxbDestFolder
            TxbDestFolder.Parent = PnlSettings;
            TxbDestFolder.BackColor = LayoutColors.ColorControlBack;
            TxbDestFolder.ForeColor = LayoutColors.ColorControlText;
            TxbDestFolder.Text = String.IsNullOrEmpty(Configs.OandaImportDestFolder)
                                     ? Data.OfflineDataDir
                                     : Configs.OandaImportDestFolder;

            // BtnDestFolder
            BtnDestFolder.Parent = PnlSettings;
            BtnDestFolder.Name = "BtnDestFolder";
            BtnDestFolder.Text = Language.T("Browse");
            BtnDestFolder.Click += BtnDestFolderClick;
            BtnDestFolder.UseVisualStyleBackColor = true;

            // PnlSettings
            PnlSettings.Parent = this;

            // PnlInfoBase
            PnlImportInfo.Parent = this;
            PnlImportInfo.Padding = new Padding(4, (int) PnlImportInfo.CaptionHeight, 2, 2);

            // TbxInfo
            TbxInfo.Parent = PnlImportInfo;
            TbxInfo.BorderStyle = BorderStyle.None;
            TbxInfo.Dock = DockStyle.Fill;
            TbxInfo.BackColor = LayoutColors.ColorControlBack;
            TbxInfo.ForeColor = LayoutColors.ColorControlText;
            TbxInfo.Multiline = true;
            TbxInfo.AcceptsReturn = true;
            TbxInfo.AcceptsTab = true;
            TbxInfo.ScrollBars = ScrollBars.Vertical;

            // ProgressBar
            ProgressBar.Parent = this;

            // Button Help
            BtnHelp.Parent = this;
            BtnHelp.Name = "Help";
            BtnHelp.Text = Language.T("Help");
            BtnHelp.Click += BtnHelpClick;
            BtnHelp.UseVisualStyleBackColor = true;

            // Button Close
            BtnClose.Parent = this;
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.Cancel;
            BtnClose.UseVisualStyleBackColor = true;

            // Button Import
            BtnImport.Parent = this;
            BtnImport.Name = "Import";
            BtnImport.Text = Language.T("Import");
            BtnImport.Click += BtnImportClick;
            BtnImport.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        private Label LblIntro { get; set; }
        private TextBox TxbFileName { get; set; }
        private Button BtnBrowse { get; set; }
        private Label LblSymbol { get; set; }
        private TextBox TxbSymbol { get; set; }
        private Label LblMinBars { get; set; }
        private NumericUpDown NudMinBars { get; set; }
        private Label LblDestFolder { get; set; }
        private TextBox TxbDestFolder { get; set; }
        private Button BtnDestFolder { get; set; }

        private FancyPanel PnlSettings { get; set; }
        private FancyPanel PnlImportInfo { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private TextBox TbxInfo { get; set; }
        private Button BtnHelp { get; set; }
        private Button BtnImport { get; set; }
        private Button BtnClose { get; set; }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var height = (int)(400 * Data.VDpiScale);
            var buttonWidth = (int)(Data.HorizontalDlu * 60 * Data.HDpiScale);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, height);
            BtnBrowse.Focus();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60 * Data.HDpiScale);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int border = btnHrzSpace;

            // Button Cancel
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            BtnHelp.Size = new Size(buttonWidth, buttonHeight);
            BtnHelp.Location = new Point(BtnClose.Left - buttonWidth - btnHrzSpace,
                                         ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Import
            BtnImport.Size = new Size(buttonWidth, buttonHeight);
            BtnImport.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            ProgressBar.Size = new Size(ClientSize.Width - 2*border, (int) (Data.VerticalDlu*9));
            ProgressBar.Location = new Point(border, BtnClose.Top - ProgressBar.Height - btnVertSpace);

            PnlSettings.Size = new Size(ClientSize.Width - 2*btnHrzSpace, (int) (180 * Data.VDpiScale));
            PnlSettings.Location = new Point(btnHrzSpace, border);

            PnlImportInfo.Size = new Size(ClientSize.Width - 2*btnHrzSpace,
                                          ProgressBar.Top - PnlSettings.Bottom - 2*border);
            PnlImportInfo.Location = new Point(btnHrzSpace, PnlSettings.Bottom + border);

            // Label Intro
            LblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            // Button Browse
            BtnBrowse.Size = new Size(buttonWidth, buttonHeight);
            BtnBrowse.Location = new Point(PnlSettings.Width - buttonWidth - btnHrzSpace, LblIntro.Bottom + border);

            // TxbFileName
            TxbFileName.Width = BtnBrowse.Left - 2*btnHrzSpace - border;
            TxbFileName.Location = new Point(btnHrzSpace + border, BtnBrowse.Top + (buttonHeight - TxbFileName.Height)/2);

            // Symbol
            LblSymbol.Location = new Point(btnHrzSpace + border, BtnBrowse.Bottom + border + 2);
            TxbSymbol.Width = 150;
            TxbSymbol.Location = new Point(PnlSettings.ClientSize.Width - btnHrzSpace - border - TxbSymbol.Width,
                                           BtnBrowse.Bottom + border);

            // Minimum bars
            LblMinBars.Location = new Point(btnHrzSpace + border, TxbSymbol.Bottom + border + 20);
            NudMinBars.Width = 75;
            NudMinBars.Location = new Point(PnlSettings.ClientSize.Width - btnHrzSpace - border - NudMinBars.Width,
                                            LblMinBars.Top - 2);

            // Destination folder
            LblDestFolder.Location = new Point(btnHrzSpace + border, NudMinBars.Bottom + 2*border);
            BtnDestFolder.Size = new Size(buttonWidth, buttonHeight);
            BtnDestFolder.Location = new Point(PnlSettings.Width - buttonWidth - btnHrzSpace,
                                               LblDestFolder.Bottom + border);
            TxbDestFolder.Width = BtnDestFolder.Left - 2*btnHrzSpace - border;
            TxbDestFolder.Location = new Point(btnHrzSpace + border,
                                               BtnDestFolder.Top + (buttonHeight - TxbDestFolder.Height)/2);
        }

        /// <summary>
        ///     Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Form closes.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Configs.MinBarsInBarFile = (int) NudMinBars.Value;
        }

        private void SetInfoText(string text)
        {
            if (TbxInfo.InvokeRequired)
            {
                BeginInvoke(new SetInfoTextDelegate(SetInfoText), new object[] {text});
            }
            else
            {
                TbxInfo.AppendText(text);
            }
        }

        /// <summary>
        ///     Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog {InitialDirectory = lastFolder};
            if (fd.ShowDialog() != DialogResult.OK) return;
            oandaFile = fd.FileName;
            lastFolder = Path.GetDirectoryName(oandaFile);
            TxbFileName.Text = Path.GetFileNameWithoutExtension(oandaFile);
            TxbSymbol.Text = string.Empty;
            TxbSymbol.Focus();
        }

        /// <summary>
        ///     BtnDestFolderClick
        /// </summary>
        private void BtnDestFolderClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog
                {
                    SelectedPath = TxbDestFolder.Text,
                    Description = Language.T("Select a destination folder") + "."
                };
            if (fd.ShowDialog() != DialogResult.OK) return;
            TxbDestFolder.Text = fd.SelectedPath;
            Configs.OandaImportDestFolder = fd.SelectedPath;
        }

        /// <summary>
        ///     Button Help Click
        /// </summary>
        private void BtnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/oanda_data");
            }
            catch (Exception exc)
            {
                Console.WriteLine("BtnHelpClick: " + exc.Message);
            }
        }

        /// <summary>
        ///     Button Import Click
        /// </summary>
        private void BtnImportClick(object sender, EventArgs e)
        {
            if (isImporting)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Configs.MetaTrader4DataPath = TxbFileName.Text;
            Cursor = Cursors.WaitCursor;
            isImporting = true;
            BtnImport.Text = Language.T("Stop");
            ProgressBar.Style = ProgressBarStyle.Marquee;

            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;
            if (worker == null) return;

            int count1MinBars = ImportTicks(oandaFile);

            foreach (DataPeriod period in Enum.GetValues(typeof (DataPeriod)))
            {
                if (count1MinBars < NudMinBars.Value*(int) period) continue;

                if (period == DataPeriod.M1)
                {
                    CompileMinuteBars();
                    SaveBars(minuteBarList, DataPeriod.M1);
                    continue;
                }

                List<Bar> barList = CompileBars(minuteBarList, period);
                SaveBars(barList, period);
            }
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            ProgressBar.Style = ProgressBarStyle.Blocks;
            isImporting = false;
            BtnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;

            ProgressBar.Value = ProgressBar.Maximum;
        }

        private int ImportTicks(string file)
        {
            string symbol = TxbSymbol.Text;
            if (string.IsNullOrEmpty(symbol))
                return 0;

            var streamReader = new StreamReader(file);
            var outStream = new FileStream(Path.Combine(outFolder, symbol + "0.bin"), FileMode.Create);
            var binaryWriter = new BinaryWriter(outStream);

            DateTime time = DateTime.MinValue;
            int volume = 0;
            var reccord = new List<double>();
            int totalVolume = 0;
            int count1MinBars = 1;

            string dateFormat = "#";
            char delimiter = '#';

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line != null)
                    {
                        if (delimiter == '#')
                            delimiter = FindDelimiter(line);

                        string[] data = line.Split(new[] {delimiter});

                        if (dateFormat == "#")
                            dateFormat = FindDateFormat(data[0]);

                        IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                        DateTime t = DateTime.ParseExact(data[0], dateFormat, formatProvider);
                        var tickTime = new DateTime(CorrectProblemYear2000(t.Year), t.Month, t.Day, t.Hour, t.Minute, 0);
                        double bid = StringToDouble(data[1]);

                        if (tickTime.Minute != time.Minute || volume == 0)
                        {
                            if (volume > 0)
                            {
                                FilterReccord(reccord);
                                SaveReccord(binaryWriter, time, volume, reccord);
                                count1MinBars++;
                            }

                            time = tickTime;
                            volume = 0;
                            reccord.Clear();
                            reccord.Add(bid);
                        }
                        else if (reccord.Count > 0 && Math.Abs(bid - reccord[reccord.Count - 1]) > 0.000001)
                        {
                            reccord.Add(bid);
                        }
                    }

                    volume++;
                    totalVolume++;
                }
                if (volume > 0)
                {
                    FilterReccord(reccord);
                    SaveReccord(binaryWriter, time, volume, reccord);
                    count1MinBars++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            streamReader.Close();
            binaryWriter.Close();
            outStream.Close();

            totalVolume--;
            count1MinBars--;

            SetInfoText(TxbSymbol.Text + Environment.NewLine);
            SetInfoText(Language.T("File") + ": " + Path.GetFileNameWithoutExtension(file) + Environment.NewLine);
            SetInfoText(Language.T("Saved") + " " + Language.T("Ticks") + " - " + totalVolume + Environment.NewLine);

            return count1MinBars;
        }

        private static void FilterReccord(IList<double> reccord)
        {
            int count = reccord.Count;
            if (count < 3)
                return;

            var bids = new List<double>(count - 1) {reccord[0], reccord[1]};

            int b = 1;
            bool isChanged = false;

            for (int i = 2; i < count; i++)
            {
                double bid = reccord[i];

                if (bids[b - 1] < bids[b] && bids[b] < bid || bids[b - 1] > bids[b] && bids[b] > bid)
                {
                    bids[b] = bid;
                    isChanged = true;
                }
                else
                {
                    bids.Add(bid);
                    b++;
                }
            }

            if (isChanged)
            {
                reccord.Clear();
                foreach (double bid in bids)
                    reccord.Add(bid);
            }
        }

        private static void SaveReccord(BinaryWriter binaryWriter, DateTime time, int volume,
                                        ICollection<double> reccord)
        {
            int count = reccord.Count;
            if (count < 1)
                return;

            binaryWriter.Write(time.ToBinary());
            binaryWriter.Write(volume);
            binaryWriter.Write(count);
            foreach (double bid in reccord)
                binaryWriter.Write(bid);
        }

        private List<Bar> CompileBars(IEnumerable<Bar> minuteBars, DataPeriod period)
        {
            var barList = new List<Bar>();
            var lastBarEndTime = new DateTime();
            foreach (Bar bar in minuteBars)
            {
                if (bar.Time >= lastBarEndTime)
                {
                    DateTime lastBarStartTime = GetBarStartTime(bar.Time, (int) period);
                    lastBarEndTime = lastBarStartTime.AddMinutes((int) period);
                    Bar newBar = bar;
                    newBar.Time = lastBarStartTime;
                    barList.Add(newBar);
                    continue;
                }

                Bar lastBar = barList[barList.Count - 1];

                if (lastBar.High < bar.High)
                    lastBar.High = bar.High;
                if (lastBar.Low > bar.Low)
                    lastBar.Low = bar.Low;
                lastBar.Close = bar.Close;
                lastBar.Volume += bar.Volume;

                barList[barList.Count - 1] = lastBar;
            }

            return barList;
        }

        private DateTime GetBarStartTime(DateTime time, int period)
        {
            if (period == 1)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
            if (period < 60)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - time.Minute%period, 0);
            if (period == 60)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            if (period == 240)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour - time.Hour%4, 0, 0);
            return new DateTime(time.Year, time.Month, time.Day);
        }

        /// <summary>
        ///     Saves bar data to a CSV file.
        /// </summary>
        private void SaveBars(List<Bar> barList, DataPeriod period)
        {
            var sb = new StringBuilder();
            foreach (Bar bar in barList)
                sb.AppendLine(
                    String.Format("{0:D4}-{1:D2}-{2:D2}\t{3:D2}:{4:D2}\t{5}\t{6}\t{7}\t{8}\t{9}",
                                  bar.Time.Year, bar.Time.Month, bar.Time.Day, bar.Time.Hour, bar.Time.Minute, bar.Open,
                                  bar.High, bar.Low, bar.Close, bar.Volume));

            string fileName = TxbSymbol.Text + (int) period + ".csv";
            string path = Path.Combine(outFolder, fileName);

            try
            {
                var sw = new StreamWriter(path);
                sw.Write(sb.ToString());
                sw.Close();

                SetInfoText(Language.T("Saved") + " " + Data.DataPeriodToString(period) + " " + Language.T("bars") +
                            " - " + barList.Count + Environment.NewLine);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        ///     Loads tick data and compiles 1M bars.
        /// </summary>
        private void CompileMinuteBars()
        {
            minuteBarList = new List<Bar>();
            var fileStream = new FileStream(Path.Combine(outFolder, TxbSymbol.Text + "0.bin"), FileMode.Open);
            var binaryReader = new BinaryReader(fileStream);

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

                SetMinuteBar(time, volume, bidTicks);
            }

            binaryReader.Close();
            fileStream.Close();
        }

        /// <summary>
        ///     Saves bar data to minute list
        /// </summary>
        private void SetMinuteBar(DateTime time, int volume, double[] ticks)
        {
            var bar = new Bar
                {
                    Time = time,
                    Volume = volume,
                    Open = ticks[0],
                    High = double.MinValue,
                    Low = double.MaxValue,
                    Close = ticks[ticks.Length - 1]
                };

            foreach (double tick in ticks)
            {
                if (bar.High < tick)
                    bar.High = tick;
                if (bar.Low > tick)
                    bar.Low = tick;
            }

            minuteBarList.Add(bar);
        }

        private char FindDelimiter(string line)
        {
            var delimiters = new[] {',', ' ', '\t', ';'};

            foreach (char delimiter in delimiters)
            {
                string[] data = line.Split(delimiter);
                if (data.Length < 3) continue;
                return delimiter;
            }

            throw new Exception("Cannot determine column delimiter!");
        }

        private string FindDateFormat(string timeString)
        {
            var dateFormats = new[]
                {
                    "dd/MM/yy HH:mm:ss",
                    "dd.MM.yy HH:mm:ss",
                    "dd-MM-yy HH:mm:ss",
                    "yy.MM.dd HH:mm:ss",
                    "yy-MM-dd HH:mm:ss",
                    "yy/MM/dd HH:mm:ss"
                };

            foreach (string format in dateFormats)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                DateTime time;
                bool status = DateTime.TryParseExact(timeString, format, formatProvider, DateTimeStyles.None, out time);
                if (status)
                    return format;
            }

            throw new Exception("Cannot determine date format!");
        }

        /// <summary>
        ///     Fixes wrong year interpretation.
        ///     For example 08 must be 2008 instead of 8.
        /// </summary>
        private int CorrectProblemYear2000(int year)
        {
            if (year < 100)
                year += 2000;
            if (year > DateTime.Now.Year)
                year -= 100;
            return year;
        }

        /// <summary>
        ///     Converts a string to a double number.
        /// </summary>
        private static double StringToDouble(string input)
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(decimalSeparator))
            {
                input = input.Replace(".", decimalSeparator);
                input = input.Replace(",", decimalSeparator);
            }

            double number = double.Parse(input);

            return number;
        }

        #region Nested type: SetInfoTextDelegate

        private delegate void SetInfoTextDelegate(string text);

        #endregion
    }
}