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
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Dialogs.JForex
{
    public sealed class JForexImport : Form
    {
        private readonly BackgroundWorker bgWorker;

        private readonly Button btnBrowse;
        private readonly Button btnClose;
        private readonly Button btnDestFolder;
        private readonly Button btnHelp;
        private readonly Button btnImport;
        private readonly Color colorText;
        private readonly List<JForexDataFiles> files = new List<JForexDataFiles>();
        private readonly Label lblDestFolder;
        private readonly Label lblIntro;
        private readonly Label lblMarketClose;
        private readonly Label lblMarketOpen;
        private readonly NumericUpDown nudMarketClose;
        private readonly NumericUpDown nudMarketOpen;
        private readonly FancyPanel pnlInfoBase;
        private readonly FancyPanel pnlSettings;
        private readonly ProgressBar progressBar;
        private readonly TextBox tbxDataDirectory;
        private readonly TextBox tbxDestFolder;
        private readonly TextBox tbxInfo;
        private bool isImporting;

        /// <summary>
        ///     Constructor
        /// </summary>
        public JForexImport()
        {
            lblIntro = new Label();
            tbxDataDirectory = new TextBox();
            btnBrowse = new Button();
            pnlSettings = new FancyPanel();
            pnlInfoBase = new FancyPanel(Language.T("Imported Files"));
            tbxInfo = new TextBox();
            btnHelp = new Button();
            btnClose = new Button();
            btnImport = new Button();
            progressBar = new ProgressBar();

            lblMarketClose = new Label();
            lblMarketOpen = new Label();
            nudMarketClose = new NumericUpDown();
            nudMarketOpen = new NumericUpDown();
            lblDestFolder = new Label();
            tbxDestFolder = new TextBox();
            btnDestFolder = new Button();

            colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = btnImport;
            CancelButton = btnClose;
            Text = Language.T("JForex Import");

            // Label Intro
            lblIntro.Parent = pnlSettings;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.AutoSize = true;
            lblIntro.Text = Language.T("Directory containing JForex data files:");

            // Data Directory
            tbxDataDirectory.Parent = pnlSettings;
            tbxDataDirectory.BackColor = LayoutColors.ColorControlBack;
            tbxDataDirectory.ForeColor = colorText;
            tbxDataDirectory.Text = Configs.JForexDataPath;

            // Button Browse
            btnBrowse.Parent = pnlSettings;
            btnBrowse.Name = "Browse";
            btnBrowse.Text = Language.T("Browse");
            btnBrowse.Click += BtnBrowseClick;
            btnBrowse.UseVisualStyleBackColor = true;

            // Label Market Close
            lblMarketClose.Parent = pnlSettings;
            lblMarketClose.ForeColor = colorText;
            lblMarketClose.BackColor = Color.Transparent;
            lblMarketClose.AutoSize = true;
            lblMarketClose.Text = Language.T("Market closing hour on Friday:");

            // Label Market Open
            lblMarketOpen.Parent = pnlSettings;
            lblMarketOpen.ForeColor = colorText;
            lblMarketOpen.BackColor = Color.Transparent;
            lblMarketOpen.AutoSize = true;
            lblMarketOpen.Text = Language.T("Market opening hour on Sunday:");

            // NUDMarketClose
            nudMarketClose.BeginInit();
            nudMarketClose.Parent = pnlSettings;
            nudMarketClose.TextAlign = HorizontalAlignment.Center;
            nudMarketClose.Minimum = 0;
            nudMarketClose.Maximum = 24;
            nudMarketClose.Increment = 1;
            nudMarketClose.Value = Configs.MarketClosingHour;
            nudMarketClose.EndInit();

            // NUDMarketOpen
            nudMarketOpen.BeginInit();
            nudMarketOpen.Parent = pnlSettings;
            nudMarketOpen.TextAlign = HorizontalAlignment.Center;
            nudMarketOpen.Minimum = 0;
            nudMarketOpen.Maximum = 24;
            nudMarketOpen.Increment = 1;
            nudMarketOpen.Value = Configs.MarketOpeningHour;
            nudMarketOpen.EndInit();

            // lblDestFolder
            lblDestFolder.Parent = pnlSettings;
            lblDestFolder.ForeColor = LayoutColors.ColorControlText;
            lblDestFolder.BackColor = Color.Transparent;
            lblDestFolder.AutoSize = true;
            lblDestFolder.Text = Language.T("Select a destination folder") + ":";

            // tbxDestFolder
            tbxDestFolder.Parent = pnlSettings;
            tbxDestFolder.BackColor = LayoutColors.ColorControlBack;
            tbxDestFolder.ForeColor = LayoutColors.ColorControlText;
            tbxDestFolder.Text = String.IsNullOrEmpty(Configs.JForexImportDestFolder)
                                     ? Data.OfflineDataDir
                                     : Configs.JForexImportDestFolder;

            // btnDestFolder
            btnDestFolder.Parent = pnlSettings;
            btnDestFolder.Name = "btnDestFolder";
            btnDestFolder.Text = Language.T("Browse");
            btnDestFolder.Click += BtnDestFolderClick;
            btnDestFolder.UseVisualStyleBackColor = true;

            // pnlSettings
            pnlSettings.Parent = this;

            // pnlInfoBase
            pnlInfoBase.Parent = this;
            pnlInfoBase.Padding = new Padding(4, (int) pnlInfoBase.CaptionHeight, 2, 2);

            // TbxInfo
            tbxInfo.Parent = pnlInfoBase;
            tbxInfo.BorderStyle = BorderStyle.None;
            tbxInfo.Dock = DockStyle.Fill;
            tbxInfo.BackColor = LayoutColors.ColorControlBack;
            tbxInfo.ForeColor = LayoutColors.ColorControlText;
            tbxInfo.Multiline = true;
            tbxInfo.AcceptsReturn = true;
            tbxInfo.AcceptsTab = true;
            tbxInfo.ScrollBars = ScrollBars.Vertical;

            // ProgressBar
            progressBar.Parent = this;

            // Button Help
            btnHelp.Parent = this;
            btnHelp.Name = "Help";
            btnHelp.Text = Language.T("Help");
            btnHelp.Click += btnHelpClick;
            btnHelp.UseVisualStyleBackColor = true;

            // Button Close
            btnClose.Parent = this;
            btnClose.Text = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            // Button Import
            btnImport.Parent = this;
            btnImport.Name = "Import";
            btnImport.Text = Language.T("Import");
            btnImport.Click += BtnImportClick;
            btnImport.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        /// <summary>
        ///     Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int)(Data.HorizontalDlu * 60 * Data.HDpiScale);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);

            var height = (int) (400*Data.VDpiScale);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, height);

            btnImport.Focus();
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
            int nudWidth = (int)(70 * Data.HDpiScale);

            // Button Cancel
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            btnHelp.Size = new Size(buttonWidth, buttonHeight);
            btnHelp.Location = new Point(btnClose.Left - buttonWidth - btnHrzSpace,
                                         ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Import
            btnImport.Size = new Size(buttonWidth, buttonHeight);
            btnImport.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size = new Size(ClientSize.Width - 2*border, (int) (Data.VerticalDlu*9));
            progressBar.Location = new Point(border, btnClose.Top - progressBar.Height - btnVertSpace);

            var height = (int)(160 * Data.VDpiScale);
            pnlSettings.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, height);
            pnlSettings.Location = new Point(btnHrzSpace, border);

            pnlInfoBase.Size = new Size(ClientSize.Width - 2*btnHrzSpace,
                                        progressBar.Top - pnlSettings.Bottom - 2*border);
            pnlInfoBase.Location = new Point(btnHrzSpace, pnlSettings.Bottom + border);

            // Label Intro
            lblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            // Button Browse
            btnBrowse.Size = new Size(buttonWidth, buttonHeight);
            btnBrowse.Location = new Point(pnlSettings.Width - buttonWidth - btnHrzSpace, lblIntro.Bottom + border);

            // TextBox txbDataDirectory
            tbxDataDirectory.Width = btnBrowse.Left - 2*btnHrzSpace - border;
            tbxDataDirectory.Location = new Point(btnHrzSpace + border,
                                                  btnBrowse.Top + (buttonHeight - tbxDataDirectory.Height)/2);

            int nudLeft = pnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border;
            nudMarketClose.Size = new Size(nudWidth, buttonHeight);
            nudMarketClose.Location = new Point(nudLeft, btnBrowse.Bottom + border);
            nudMarketOpen.Size = new Size(nudWidth, buttonHeight);
            nudMarketOpen.Location = new Point(nudLeft, nudMarketClose.Bottom + border);

            // Labels
            lblMarketClose.Location = new Point(btnHrzSpace + border, nudMarketClose.Top + 2);
            lblMarketOpen.Location = new Point(btnHrzSpace + border, nudMarketOpen.Top + 2);

            // Destination folder
            lblDestFolder.Location = new Point(btnHrzSpace + border, nudMarketOpen.Bottom + 2*border);
            btnDestFolder.Size = new Size(buttonWidth, buttonHeight);
            btnDestFolder.Location = new Point(pnlSettings.Width - buttonWidth - btnHrzSpace,
                                               lblDestFolder.Bottom + border);
            tbxDestFolder.Width = btnDestFolder.Left - 2*btnHrzSpace - border;
            tbxDestFolder.Location = new Point(btnHrzSpace + border,
                                               btnDestFolder.Top + (buttonHeight - tbxDestFolder.Height)/2);
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        private void SetInfoText(string text)
        {
            if (tbxInfo.InvokeRequired)
            {
                BeginInvoke(new SetInfoTextCallback(SetInfoText), new object[] {text});
            }
            else
            {
                tbxInfo.AppendText(text);
            }
        }

        /// <summary>
        ///     Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != DialogResult.OK) return;
            Configs.JForexDataPath = fd.SelectedPath;
            tbxDataDirectory.Text = fd.SelectedPath;
        }

        /// <summary>
        ///     btnDestFolderClick
        /// </summary>
        private void BtnDestFolderClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog
                {
                    SelectedPath = tbxDestFolder.Text,
                    Description = Language.T("Select a destination folder") + "."
                };
            if (fd.ShowDialog() != DialogResult.OK) return;
            Configs.JForexImportDestFolder = fd.SelectedPath;
            tbxDestFolder.Text = fd.SelectedPath;
        }

        /// <summary>
        ///     Button Help Click
        /// </summary>
        private void btnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/jforex_data");
            }
            catch (Exception exc)
            {
                Console.WriteLine("btnHelpClick: " + exc.Message);
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

            Configs.JForexDataPath = tbxDataDirectory.Text;
            Cursor = Cursors.WaitCursor;
            progressBar.Style = ProgressBarStyle.Marquee;
            isImporting = true;
            btnImport.Text = Language.T("Stop");
            Configs.MarketClosingHour = (int) nudMarketClose.Value;
            Configs.MarketOpeningHour = (int) nudMarketOpen.Value;

            // Start the bgWorker
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
            files.Clear();
            ReadJForexFiles();
            foreach (JForexDataFiles file in files)
            {
                if (worker.CancellationPending) break;
                if (file.Period > 0) ImportBarFile(file);
                if (worker.CancellationPending) break;
                if (file.Period == 0) ImportTicks(file);
            }
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            progressBar.Style = ProgressBarStyle.Blocks;
            isImporting = false;
            btnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;
        }

        private void ReadJForexFiles()
        {
            if (!Directory.Exists(tbxDataDirectory.Text))
                return;

            string[] dataFiles = Directory.GetFiles(tbxDataDirectory.Text);
            foreach (string filePath in dataFiles)
            {
                if (Path.GetExtension(filePath) != ".csv") continue;
                var file = new JForexDataFiles(filePath, tbxDestFolder.Text);
                if (file.IsCorrect)
                    files.Add(file);
            }
        }

        private void ImportBarFile(JForexDataFiles file)
        {
            var streamReader = new StreamReader(file.FilePath);
            var streamWriter = new StreamWriter(file.FileTargetPath);
            string dateFormat = "#";
            char delimiter = '#';
            int bars = 0;

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line == null) continue;
                    if (line.StartsWith("Time"))
                        continue; // Skips annotation line.

                    if (delimiter == '#')
                        delimiter = FindDelimiter(line);

                    string[] data = line.Split(new[] {delimiter});

                    string timeInput = data.Length == 6 ? data[0] : data[0] + " " + data[1];

                    if (dateFormat == "#")
                        dateFormat = FindDateFormat(timeInput);


                    DateTime time = ParseDateWithoutSeconds(timeInput, dateFormat);
                    double open = StringToDouble(data[data.Length - 5]);
                    double high = StringToDouble(data[data.Length - 4]);
                    double low = StringToDouble(data[data.Length - 3]);
                    double close = StringToDouble(data[data.Length - 2]);
                    var volume = (int) StringToDouble(data[data.Length - 1]);

                    if (Math.Abs(open - high) < 0.000001 &&
                        Math.Abs(open - low) < 0.000001 &&
                        Math.Abs(open - close) < 0.000001) continue;

                    streamWriter.WriteLine(time.ToString("yyyy-MM-dd\tHH:mm") + "\t" +
                                           open + "\t" + high + "\t" + low + "\t" + close + "\t" + volume);
                    bars++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            streamWriter.Close();
            streamReader.Close();
            SetInfoText(file.Symbol + " " + Data.DataPeriodToString((DataPeriod) file.Period) + " - " +
                        (Language.T("Bars")).ToLower() + ": " + bars + Environment.NewLine);
        }

        private char FindDelimiter(string line)
        {
            var delimiters = new[] {' ', ',', '.', '/'};

            foreach (char delimiter in delimiters)
            {
                string[] data = line.Split(delimiter);
                if (data.Length <= 4) continue;
                return delimiter;
            }

            return ',';
        }

        private string FindDateFormat(string timeString)
        {
            string stripped = timeString.Remove(timeString.LastIndexOf(':'));
            var dateFormats = new[]
                {
                    "yyyy.MM.dd HH:mm",
                    "yyyy-MM-dd HH:mm",
                    "yyyy/MM/dd HH:mm",
                    "dd.MM.yyyy HH:mm",
                    "dd-MM-yyyy HH:mm",
                    "dd/MM/yyyy HH:mm"
                };

            foreach (string format in dateFormats)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                DateTime time;
                bool status = DateTime.TryParseExact(stripped, format, formatProvider, DateTimeStyles.None, out time);
                if (status)
                    return format;
            }

            return "yyyy.MM.dd HH:mm";
        }

        private void ImportTicks(JForexDataFiles file)
        {
            var streamReader = new StreamReader(file.FilePath);
            var outStream = new FileStream(file.FileTargetPath, FileMode.Create);
            var binaryWriter = new BinaryWriter(outStream);

            DateTime time = DateTime.MinValue;
            int volume = 0;
            var reccord = new List<double>();
            int count1MinBars = 1;
            int totalVolume = 0;

            string dateFormat = "#";
            char delimiter = '#';

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line != null && line.StartsWith("Time"))
                        continue; // Skips annotation line.
                    if (line != null)
                    {
                        if (delimiter == '#')
                            delimiter = FindDelimiter(line);

                        string[] data = line.Split(new[] {delimiter});

                        if (dateFormat == "#")
                            dateFormat = FindDateFormat(data[0] + " " + data[1]);

                        DateTime t = ParseDateWithoutSeconds(data[0] + " " + data[1], dateFormat);
                        var tickTime = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
                        double bid = StringToDouble(data[2]);

                        if (tickTime.Minute != time.Minute || volume == 0)
                        {
                            if (volume > 0 && !IsWeekend(time))
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
                if (volume > 0 && !IsWeekend(time))
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

            SetInfoText(file.Symbol + " " + Language.T("Ticks") + " - " + (Language.T("Ticks")).ToLower() + ": " +
                        totalVolume + " - 1M " + (Language.T("Bars")).ToLower() + ": " + count1MinBars +
                        Environment.NewLine);
        }

        private DateTime ParseDateWithoutSeconds(string input, string dateFormat)
        {
            string stripped = input.Remove(input.LastIndexOf(':'));
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            DateTime time = DateTime.ParseExact(stripped, dateFormat, formatProvider);
            return time;
        }

        private bool IsWeekend(DateTime time)
        {
            return time.DayOfWeek == DayOfWeek.Friday && time.Hour >= Configs.MarketClosingHour ||
                   time.DayOfWeek == DayOfWeek.Saturday ||
                   time.DayOfWeek == DayOfWeek.Sunday && time.Hour < Configs.MarketOpeningHour;
        }

        private static void FilterReccord(List<double> reccord)
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

        private static void SaveReccord(BinaryWriter binaryWriter, DateTime time, int volume, List<double> reccord)
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

        #region Nested type: SetInfoTextCallback

        private delegate void SetInfoTextCallback(string text);

        #endregion
    }
}