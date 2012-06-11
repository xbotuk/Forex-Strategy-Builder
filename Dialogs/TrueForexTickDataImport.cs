// Forex Strategy Builder - TrueForexTickDataImport
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.
// Contributed by Acerguest.

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

namespace Forex_Strategy_Builder.Dialogs
{
    public sealed class TrueForexTickDataImport : Form, ITrueForexTickDataImport
    {
        private readonly BackgroundWorker _bgWorker;
        private bool _isImporting;
        private List<Bar> _minuteBarList;
        private string _lastFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private string _outFolder = string.Empty; 
        private string _trrueforexFile;
        private string Symbol { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TrueForexTickDataImport()
        {
            _outFolder = Configs.TrueFxImportDestFolder;

            LblIntro = new Label();
            TxbFileName = new TextBox();
            BtnBrowse = new Button();
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
            Text = Language.T("TrueFX Import");

            // Label Intro
            LblIntro.Parent = PnlSettings;
            LblIntro.ForeColor = LayoutColors.ColorControlText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.AutoSize = true;
            LblIntro.Text = Language.T("Select a TrueFX file for import") + ":";

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
            TxbDestFolder.Text = String.IsNullOrEmpty(Configs.TrueFxImportDestFolder) ? Data.OfflineDataDir : Configs.TrueFxImportDestFolder;

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
            _bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            
        }

        private Label LblIntro { get; set; }
        private TextBox TxbFileName { get; set; }
        private Button BtnBrowse { get; set; }
        
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
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 400);
            BtnBrowse.Focus();
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
            ProgressBar.Size = new Size(ClientSize.Width - 2*border, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(border, BtnClose.Top - ProgressBar.Height - btnVertSpace);

            PnlSettings.Size = new Size(ClientSize.Width - 2*btnHrzSpace, 180);
            PnlSettings.Location = new Point(btnHrzSpace, border);

            PnlImportInfo.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, ProgressBar.Top - PnlSettings.Bottom - 2 * border);
            PnlImportInfo.Location = new Point(btnHrzSpace, PnlSettings.Bottom + border);

            // Label Intro
            LblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            // Button Browse
            BtnBrowse.Size = new Size(buttonWidth, buttonHeight);
            BtnBrowse.Location = new Point(PnlSettings.Width - buttonWidth - btnHrzSpace, LblIntro.Bottom + border);

            // TxbFileName
            TxbFileName.Width = BtnBrowse.Left - 2*btnHrzSpace - border;
            TxbFileName.Location = new Point(btnHrzSpace + border, BtnBrowse.Top + (buttonHeight - TxbFileName.Height)/2);

            // Minimum bars
            LblMinBars.Location = new Point(btnHrzSpace + border, TxbFileName.Bottom + border + 20);
            NudMinBars.Width = 75;
            NudMinBars.Location = new Point(PnlSettings.ClientSize.Width - btnHrzSpace - border - NudMinBars.Width, LblMinBars.Top - 2);

            // Destination folder
            LblDestFolder.Location = new Point(btnHrzSpace + border, NudMinBars.Bottom + 2*border);
            BtnDestFolder.Size = new Size(buttonWidth, buttonHeight);
            BtnDestFolder.Location = new Point(PnlSettings.Width - buttonWidth - btnHrzSpace, LblDestFolder.Bottom + border);
            TxbDestFolder.Width = BtnDestFolder.Left - 2 * btnHrzSpace - border;
            TxbDestFolder.Location = new Point(btnHrzSpace + border, BtnDestFolder.Top + (buttonHeight - TxbDestFolder.Height) / 2);
        }

        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Form closes.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Configs.MinBarsInBarFile = (int)NudMinBars.Value;
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
        /// Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog {SelectedPath = _lastFolder};

            if (fd.ShowDialog() != DialogResult.OK) return;
            _trrueforexFile = fd.SelectedPath;
            _lastFolder = _trrueforexFile;
            TxbFileName.Text = Path.GetFileNameWithoutExtension(_trrueforexFile);
            TxbFileName.Focus();

            Symbol = TxbFileName.Text; //folder needs to have the same names as pair
        }

        /// <summary>
        /// BtnDestFolderClick
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
            Configs.TrueFxImportDestFolder = fd.SelectedPath;
        }

        /// <summary>
        /// Button Help Click
        /// </summary>
        private void BtnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/truefx_data");
            }
            catch (Exception exc)
            {
                Console.WriteLine("BtnHelpClick: " + exc.Message);
            }
        }

        /// <summary>
        /// Button Import Click
        /// </summary>
        private void BtnImportClick(object sender, EventArgs e)
        {
            if (_isImporting)
            {
                // Cancel the asynchronous operation.
                _bgWorker.CancelAsync();
                return;
            }

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = GetSortedCsvFiles(_trrueforexFile).Length;
            ProgressBar.Value = 0;
            ProgressBar.Style = ProgressBarStyle.Blocks;

            _outFolder = TxbDestFolder.Text;
            Configs.MetaTrader4DataPath = TxbFileName.Text;
            Cursor = Cursors.WaitCursor;
            _isImporting = true;
            BtnImport.Text = Language.T("Stop");
            
            _bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;
            if (worker == null) return;

            int count1MinBars = ImportTicks(_trrueforexFile, worker);

            foreach (DataPeriods period in Enum.GetValues(typeof(DataPeriods)))
            {
                if (count1MinBars < NudMinBars.Value * (int)period) continue;

                if (period == DataPeriods.min1)
                {
                    CompileMinuteBars();
                    SaveBars(_minuteBarList, DataPeriods.min1);
                    continue;
                }
                
                List<Bar> barList = CompileBars(_minuteBarList, period);
                SaveBars(barList, period);
            }

        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            ProgressBar.Style = ProgressBarStyle.Blocks;
            _isImporting = false;
            BtnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;

            ProgressBar.Value = ProgressBar.Maximum;
        }

        public int ImportTicks(string directory, BackgroundWorker worker)
        {
            Symbol = directory.Split('\\')[directory.Split('\\').Length - 1];
            if(string.IsNullOrEmpty(Symbol))
                return 0;

            int totalVolume = 0;
            int count1MinBars = 1;
            using (var outStream = new FileStream(Path.Combine(_outFolder, Symbol + "0.bin"), FileMode.Create))
            {
                using (var binaryWriter = new BinaryWriter(outStream))
                {
                    foreach (FileInfo info in GetSortedCsvFiles(directory))
                    {
                        if (!worker.CancellationPending)
                        {
                            ImportFromSingleFile(info.FullName, ref count1MinBars, binaryWriter, ref totalVolume);
                            UpdateProgressBar(1);
                        }
                        
                    }
                }
            }

            totalVolume--;
            count1MinBars--;

            SetInfoText(Symbol + Environment.NewLine);
            SetInfoText(Language.T("Directory") + ": " + Path.GetFileNameWithoutExtension(directory) + Environment.NewLine);
            SetInfoText(Language.T("Saved") + " " + Language.T("Ticks") + " - " + totalVolume + Environment.NewLine);
            return count1MinBars;
        }

        private FileInfo[] GetSortedCsvFiles(string directory)
        {
            var taskDirectory = new DirectoryInfo(directory);
            FileInfo[] taskFiles = taskDirectory.GetFiles(Symbol + "*.csv");
            Array.Sort(taskFiles, (f1, f2) => String.CompareOrdinal(f1.Name, f2.Name));
            return taskFiles;
        }

        private void ImportFromSingleFile(string fullFilePath, ref int count1MinBars, BinaryWriter binaryWriter, ref int totalVolume)
        {
            DateTime time = DateTime.MinValue;
            int volume = 0;
            var reccord = new List<double>();
            string dateFormat = "#";
            char delimiter = '#';

            using (var streamReader = new StreamReader(fullFilePath))
            {
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
                                dateFormat = FindDateFormat(data[1]);

                            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                            DateTime t = DateTime.ParseExact(data[1], dateFormat, formatProvider);
                            var tickTime = new DateTime(CorrectProblemYear2000(t.Year), t.Month, t.Day, t.Hour, t.Minute, 0);
                            double bid = StringToDouble(data[2]);

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
            }

            //return count1MinBars;
        }

        private static void FilterReccord(IList<double> reccord)
        {
            int count = reccord.Count;
            if (count < 3)
                return;

            var bids = new List<double>(count - 1) { reccord[0], reccord[1] };

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

        private static void SaveReccord(BinaryWriter binaryWriter, DateTime time, int volume, ICollection<double> reccord)
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

        private List<Bar> CompileBars(IEnumerable<Bar> minuteBarList, DataPeriods period)
        {
            var barList = new List<Bar>();
            var lastBarEndTime = new DateTime();
            foreach (var bar in minuteBarList)
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
                return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - time.Minute % period, 0);
            if (period == 60)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            if (period == 240)
                return new DateTime(time.Year, time.Month, time.Day, time.Hour - time.Hour % 4, 0, 0);
            return new DateTime(time.Year, time.Month, time.Day);
        }

        /// <summary>
        /// Saves bar data to a CSV file.
        /// </summary>
        private void SaveBars(List<Bar> barList, DataPeriods period )
        {
            var sb = new StringBuilder();
            foreach (Bar bar in barList)
                sb.AppendLine(
                    String.Format("{0:D4}-{1:D2}-{2:D2}\t{3:D2}:{4:D2}\t{5}\t{6}\t{7}\t{8}\t{9}",
                                  bar.Time.Year, bar.Time.Month, bar.Time.Day, bar.Time.Hour, bar.Time.Minute, bar.Open,
                                  bar.High, bar.Low, bar.Close, bar.Volume));

            string fileName = Symbol + (int)period + ".csv";
            string path = Path.Combine(_outFolder, fileName);

            try
            {
                var sw = new StreamWriter(path);
                sw.Write(sb.ToString());
                sw.Close();

                SetInfoText(Language.T("Saved") + " " + Data.DataPeriodToString(period) + " " + Language.T("bars") + " - " + barList.Count + Environment.NewLine);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Loads tick data and compiles 1M bars.
        /// </summary>
        private void CompileMinuteBars()
        {
            _minuteBarList = new List<Bar>();
            var fileStream = new FileStream(Path.Combine(_outFolder, Symbol + "0.bin"), FileMode.Open);
            var binaryReader = new BinaryReader(fileStream);

            long pos = 0;
            long length = binaryReader.BaseStream.Length;
            while (pos < length)
            {
                DateTime time = DateTime.FromBinary(binaryReader.ReadInt64());
                pos += sizeof(Int64);

                int volume = binaryReader.ReadInt32();
                pos += sizeof(Int32);

                int count = binaryReader.ReadInt32();
                pos += sizeof(Int32);

                var bidTicks = new double[count];
                for (int i = 0; i < count; i++)
                    bidTicks[i] = binaryReader.ReadDouble();
                pos += count * sizeof(Double);

                SetMinuteBar(time, volume, bidTicks);
            }

            binaryReader.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Saves bar data to minute list
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

            _minuteBarList.Add(bar);
        }

        private char FindDelimiter(string line)
        {
            var delimiters = new[] { ',', ' ', '\t', ';' };

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
                                      "yyyyMMdd HH:mm:ss.fff",
                                      "dd/MM/yy HH:mm:ss.fff",
                                      "dd.MM.yy HH:mm:ss.fff",
                                      "dd-MM-yy HH:mm:ss.fff",
                                      "yy.MM.dd HH:mm:ss.fff",
                                      "yy-MM-dd HH:mm:ss.fff",
                                      "yy/MM/dd HH:mm:ss.fff"
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
        /// Fixes wrong year interpretation. 
        /// For example 08 must be 2008 instead of 8.
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
        /// Converts a string to a double number.
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

        #region Delegates

        private delegate void SetInfoTextDelegate(string text);
        private delegate void SetupProgressBarFileDelegate(long maximum);
        private delegate void UpdateProgressBarDelegate(int increment);
        private delegate void UpdateProgressBarFileDelegate(long position);

        #endregion

        private void UpdateProgressBar(int increment)
        {
            if (ProgressBar.InvokeRequired)
            {
                BeginInvoke(new UpdateProgressBarDelegate(UpdateProgressBar), new object[] { increment });
            }
            else
            {
                ProgressBar.Value = ProgressBar.Value + increment;
            }
        }


    }

    public interface ITrueForexTickDataImport
    {
        int ImportTicks(string file, BackgroundWorker worker);
    }
}
