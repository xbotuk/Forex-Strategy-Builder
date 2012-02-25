// Forex Strategy Builder - MetaTrader4_Import
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

// Contribution by Adam Burgess (February 2012)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class MetaTrader4_Import : Form
    {
        Label             lblIntro;
        TextBox           txbDataDirectory;
        Button            btnBrowse;
        Label             lblStartingDate;
        DateTimePicker    dtpStartingDate;
        Label             lblEndingDate;
        DateTimePicker    dtpEndingDate;

        FancyPanel       pnlSettings;
        FancyPanel       pnlInfoBase;
        ProgressBar       progressBarFile;     // Progress of the Current File
        ProgressBar       progressBar;         // Progress of the Entire Import Process
        TextBox           tbxInfo;
        Button            btnHelp;
        Button            btnImport;
        Button            btnClose;
        ToolTip           toolTip = new ToolTip();

        Font              font;
        Color             colorText;

        BackgroundWorker bgWorker;
        bool             isImporting;
        List<string>     files = new List<string>();
        DateTime         startingDate;
        DateTime         endingDate;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetaTrader4_Import()
        {
            lblIntro         = new Label();
            txbDataDirectory = new TextBox();
            btnBrowse        = new Button();
            pnlSettings      = new FancyPanel();
            pnlInfoBase      = new FancyPanel(Language.T("Imported Files"));
            tbxInfo          = new TextBox();
            btnHelp          = new Button();
            btnClose         = new Button();
            btnImport        = new Button();
            progressBarFile  = new ProgressBar();
            progressBar      = new ProgressBar();

            lblStartingDate  = new Label();
            dtpStartingDate  = new DateTimePicker();
            lblEndingDate    = new Label();
            dtpEndingDate    = new DateTimePicker();

            font             = this.Font;
            colorText        = LayoutColors.ColorControlText;

            MaximizeBox      = false;
            MinimizeBox      = false;
            ShowInTaskbar    = false;
            Icon             = Data.Icon;
            FormBorderStyle  = FormBorderStyle.FixedDialog;
            AcceptButton     = btnImport;
            CancelButton     = btnClose;
            Text             = Language.T("MetaTrader 4 Import");

            // Label Intro
            lblIntro.Parent    = pnlSettings;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.AutoSize  = true;
            lblIntro.Text      = Language.T("Directory containing MetaTrader 4 HST files:");

            // Data Directory
            txbDataDirectory.Parent    = pnlSettings;
            txbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            txbDataDirectory.ForeColor = colorText;
            txbDataDirectory.Text      = Configs.MetaTrader4DataPath;

            // Button Browse
            btnBrowse.Parent = pnlSettings;
            btnBrowse.Name   = "Browse";
            btnBrowse.Text   = Language.T("Browse");
            btnBrowse.Click += new EventHandler(BtnBrowse_Click);
            btnBrowse.UseVisualStyleBackColor = true;

            // Label Starting Date
            lblStartingDate.Parent    = pnlSettings;
            lblStartingDate.ForeColor = colorText;
            lblStartingDate.BackColor = Color.Transparent;
            lblStartingDate.AutoSize  = true;
            lblStartingDate.Text      = Language.T("Starting Date:");

            // Starting Date
            dtpStartingDate.Parent     = pnlSettings;
            dtpStartingDate.ForeColor  = LayoutColors.ColorCaptionText;
            dtpStartingDate.ShowUpDown = true;

            // Label Ending Date
            lblEndingDate.Parent    = pnlSettings;
            lblEndingDate.ForeColor = colorText;
            lblEndingDate.BackColor = Color.Transparent;
            lblEndingDate.AutoSize  = true;
            lblEndingDate.Text      = Language.T("Ending Date:");

            // Ending Date
            dtpEndingDate.Parent     = pnlSettings;
            dtpEndingDate.ForeColor  = LayoutColors.ColorCaptionText;
            dtpEndingDate.ShowUpDown = true;

            // pnlSettings
            pnlSettings.Parent = this;

            // TODO - Controls Under This
            
            // pnlInfoBase
            pnlInfoBase.Parent = this;
            pnlInfoBase.Padding = new Padding(4, (int)pnlInfoBase.CaptionHeight, 2, 2);

            // tbxInfo
            tbxInfo.Parent = pnlInfoBase;
            tbxInfo.BorderStyle = BorderStyle.None;
            tbxInfo.Dock = DockStyle.Fill;
            tbxInfo.BackColor = LayoutColors.ColorControlBack;
            tbxInfo.ForeColor = LayoutColors.ColorControlText;
            tbxInfo.Multiline = true;
            tbxInfo.AcceptsReturn = true;
            tbxInfo.AcceptsTab = true;
            tbxInfo.ScrollBars = ScrollBars.Vertical;

            // ProgressBarFile
            progressBarFile.Parent = this;

            // ProgressBar
            progressBar.Parent = this;

            // Button Help
            btnHelp.Parent = this;
            btnHelp.Name = "Help";
            btnHelp.Text = Language.T("Help");
            btnHelp.Click += new EventHandler(BtnHelp_Click);
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
            btnImport.Click += new EventHandler(BtnImport_Click);
            btnImport.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, 400);

            // Starting DateTime
            if (string.IsNullOrEmpty(Configs.ImportStartingDate))
                dtpStartingDate.Value = DateTime.Now.AddYears(-2);
            else
                dtpStartingDate.Value = DateTime.Parse(Configs.ImportStartingDate);

            // Ending DateTime
            if (string.IsNullOrEmpty(Configs.ImportEndingDate))
                dtpEndingDate.Value = DateTime.Now;
            else
                dtpEndingDate.Value = DateTime.Parse(Configs.ImportEndingDate);

            btnImport.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int border = btnHrzSpace;
            int textHeight = Font.Height;
            int nudWidth = 70;

            // Button Cancel
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            btnHelp.Size = new Size(buttonWidth, buttonHeight);
            btnHelp.Location = new Point(btnClose.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Import
            btnImport.Size = new Size(buttonWidth, buttonHeight);
            btnImport.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size = new Size(ClientSize.Width - 2 * border, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(border, btnClose.Top - progressBar.Height - btnVertSpace);

            // ProgressBarFile
            progressBarFile.Size = new Size(ClientSize.Width - 2 * border, (int)(Data.VerticalDLU * 9));
            progressBarFile.Location = new Point(border, progressBar.Top - progressBar.Height - btnVertSpace);

            pnlSettings.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, 110);
            pnlSettings.Location = new Point(btnHrzSpace, border);

            pnlInfoBase.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, progressBarFile.Top - pnlSettings.Bottom - 2 * border);
            pnlInfoBase.Location = new Point(btnHrzSpace, pnlSettings.Bottom + border);

            // Label Intro
            lblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            // Button Browse
            btnBrowse.Size = new Size(buttonWidth, buttonHeight);
            btnBrowse.Location = new Point(pnlSettings.Width - buttonWidth - btnHrzSpace, lblIntro.Bottom + border);

            // TextBox txbDataDirectory
            txbDataDirectory.Width = btnBrowse.Left - 2 * btnHrzSpace - border;
            txbDataDirectory.Location = new Point(btnHrzSpace + border, btnBrowse.Top + (buttonHeight - txbDataDirectory.Height) / 2);

            // Date Pickers
            int pickerWidth = 200;
            int pickerLeft = pnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border + nudWidth - pickerWidth;
            dtpStartingDate.Size = new Size(pickerWidth, textHeight);
            dtpStartingDate.Location = new Point(pickerLeft, btnBrowse.Bottom + border);
            dtpEndingDate.Size = new Size(pickerWidth, textHeight);
            dtpEndingDate.Location = new Point(pickerLeft, dtpStartingDate.Bottom + border);

            // Labels
            lblStartingDate.Location = new Point(btnHrzSpace + border, dtpStartingDate.Top + 2);
            lblEndingDate.Location = new Point(btnHrzSpace + border, dtpEndingDate.Top + 2);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        delegate void SetInfoTextDelegate(string text);
        void SetInfoText(string text)
        {
            if (tbxInfo.InvokeRequired)
            {
                BeginInvoke(new SetInfoTextDelegate(SetInfoText), new object[] { text });
            }
            else
            {
                tbxInfo.AppendText(text);
            }
        }

        delegate void UpdateProgressBarDelegate(int increment);
        void UpdateProgressBar(int increment)
        {
            if (progressBar.InvokeRequired)
            {
                BeginInvoke(new UpdateProgressBarDelegate(UpdateProgressBar), new object[] { increment });
            }
            else
            {
                progressBar.Value = progressBar.Value + increment;
            }
        }

        delegate void SetupProgressBarFileDelegate(long maximum);
        void SetupProgressBarFile(long maximum)
        {
            if (progressBarFile.InvokeRequired)
            {
                BeginInvoke(new SetupProgressBarFileDelegate(SetupProgressBarFile), new object[] { maximum });
            }
            else
            {
                if (maximum > Int32.MaxValue)
                    progressBarFile.Maximum = Int32.MaxValue;
                else
                    progressBarFile.Maximum = (int)maximum;
            }
        }

        delegate void UpdateProgressBarFileDelegate(long position);
        void UpdateProgressBarFile(long position)
        {
            if (progressBarFile.InvokeRequired)
            {
                BeginInvoke(new UpdateProgressBarFileDelegate(UpdateProgressBarFile), new object[] { position });
            }
            else
            {
                if (position > Int32.MaxValue)
                    progressBarFile.Value = Int32.MaxValue;
                else
                    progressBarFile.Value = (int)position;
            }
        }

        /// <summary>
        /// Button Browse Click
        /// <summary>
        void BtnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Configs.MetaTrader4DataPath = fd.SelectedPath;
                txbDataDirectory.Text = fd.SelectedPath;
            }
        }

        /// <summary>
        /// Button Help Click
        /// </summary>
        void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/fsb/manual/metatrader4_data");
            }
            catch { }
        }

        /// <summary>
        /// Button Import Click
        /// </summary>
        void BtnImport_Click(object sender, EventArgs e)
        {
            if (isImporting)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Configs.MetaTrader4DataPath = txbDataDirectory.Text;
            Cursor = Cursors.WaitCursor;
            isImporting = true;
            btnImport.Text = Language.T("Stop");
            Configs.ImportStartingDate = String.Format("{0:u}", dtpStartingDate.Value);
            Configs.ImportEndingDate = String.Format("{0:u}", dtpEndingDate.Value);

            // Load MetaTrader4 HST Files
            string[] hstFiles = Directory.GetFiles(txbDataDirectory.Text, "*.hst", SearchOption.TopDirectoryOnly);
            List<string> orderedFiles = new List<string>();
            foreach (string file in hstFiles)
                orderedFiles.Add(GetSortableName(file));
            orderedFiles.Sort();
            files.Clear();
            foreach (string file in orderedFiles)
                files.Add(Path.Combine(txbDataDirectory.Text, GetFullName(file)));

            // Setup the Progress Bars
            progressBarFile.Minimum = 0;
            progressBarFile.Value   = 0;
            progressBarFile.Maximum = 0;

            progressBar.Minimum     = 0;
            progressBar.Value       = 0;
            progressBar.Maximum     = files.Count;

            // Setup date Filters
            startingDate = dtpStartingDate.Value;
            endingDate   = dtpEndingDate.Value;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            foreach (string file in files)
                if (!worker.CancellationPending)
                {
                    ImportHSTFile(file);
                    UpdateProgressBar(1);
                }
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                System.Media.SystemSounds.Exclamation.Play();

            progressBar.Style = ProgressBarStyle.Blocks;
            isImporting = false;
            btnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;

            progressBar.Value     = progressBar.Maximum;
            progressBarFile.Value = progressBarFile.Maximum;

            return;
        }

        void ImportHSTFile(string file)
        {
            string outpath = Path.Combine(Data.DefaultOfflineDataDir, Path.GetFileNameWithoutExtension(file) + ".csv");

            string message = " ({0} bars)";
            if (File.Exists(outpath))
                message = " ({0} bars) *";

            /* MetaTrader4 HST File Format
            * 
            * ~~ HEADER ~~
            * int      version (400)
            * string   copyright
            * int      period (TimeSpan)
            * int      digits
            * int      timesign
            * int      lastSync
            * byte[]   13 reserved bytes
            * byte[]   nn reserved bytes
            * 
            * ~~ DATA ~~
            * int      time
            * double   open
            * double   low
            * double   high
            * double   close
            * double   volume
            */
            
            // Update Status
            SetInfoText(Language.T("Import") + " " + Path.GetFileNameWithoutExtension(file) + " ... ");

            try
            {
                StringBuilder data = new StringBuilder();       // Output Data Buffer
                int importCount = 0;                            // Imported Record Count
                int exportCount = 0;                            // Exported Record Count

                // Open the Destination File
                StreamWriter sw = new StreamWriter(new FileStream(outpath, FileMode.Create));

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    // Setup the (File) ProgressBar
                    SetupProgressBarFile(fs.Length);

                    // Open the HST File
                    BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

                    // Read the File Header
                    long version = br.ReadInt32();                                  // database version
                    string copyright = new string(br.ReadChars(64)).TrimEnd('\0');      // copyright info
                    string symbol = new string(br.ReadChars(12)).TrimEnd('\0');      // symbol name
                    TimeSpan period = TimeSpan.FromMinutes(br.ReadInt32());            // symbol timeframe
                    int digits = br.ReadInt32();                                  // the amount of digits after decimal point in the symbol
                    int timesign = br.ReadInt32();                                  // timesign of the database creation
                    int lastSync = br.ReadInt32();                                  // the last synchronization time
                    byte[] res1 = br.ReadBytes(13);                                // to be used in the future
                    List<byte> res2 = new List<byte>();                                // reserved
                    int chars = 0;
                    while (br.PeekChar() == 0 && chars < 39)                            // seems to be some additional 'reserved' bytes here
                    {
                        chars++;
                        res2.Add(br.ReadByte());
                    }

                    // Read the File Data
                    while (fs.Position < fs.Length - 1)
                    {
                        // Update the (File) ProgressBar
                        importCount++;
                        if (Math.IEEERemainder(importCount, 5000) == 0)
                            UpdateProgressBarFile(fs.Position);

                        // Output data is saved after each 50000 records are imported
                        if (exportCount > 0
                            && Math.IEEERemainder(exportCount, 50000) == 0)
                        {
                            sw.Write(data);
                            data = new StringBuilder();
                        }

                        int seconds = br.ReadInt32();                                       // bar time in seconds since 1/1/1970
                        DateTime? dt = new DateTime(1970, 1, 1).AddSeconds(seconds);   // bar time
                        decimal open = Math.Round((decimal)br.ReadDouble(), digits);   // open
                        decimal low = Math.Round((decimal)br.ReadDouble(), digits);   // low
                        decimal high = Math.Round((decimal)br.ReadDouble(), digits);   // high
                        decimal close = Math.Round((decimal)br.ReadDouble(), digits);   // close
                        decimal volume = Math.Round((decimal)br.ReadDouble(), digits);   // vol                       
                        DateTime dateTime = dt.GetValueOrDefault();

                        if (dateTime > startingDate && dateTime < endingDate)
                        {
                            StringBuilder output = new StringBuilder();
                            output.Append(dateTime.ToString("yyyy"));
                            output.Append("-");
                            output.Append(dateTime.ToString("MM"));
                            output.Append("-");
                            output.Append(dateTime.ToString("dd"));
                            output.Append("\t");
                            output.Append(dateTime.ToString("HH"));
                            output.Append(":");
                            output.Append(dateTime.ToString("mm"));
                            output.Append("\t");
                            output.Append(open.ToString());
                            output.Append("\t");
                            output.Append(high.ToString());
                            output.Append("\t");
                            output.Append(low.ToString());
                            output.Append("\t");
                            output.Append(close.ToString());
                            output.Append("\t");
                            output.Append(volume.ToString());

                            data.AppendLine(output.ToString());

                            exportCount++;
                        }
                    }
                }
                sw.Write(data);
                sw.Flush();
                sw.Close();

                SetInfoText(string.Format(message, exportCount));
                SetInfoText(Environment.NewLine);

            }
            catch (Exception ex)
            {
                SetInfoText("Error (" + ex.Message + ")");
            }

        }

        /// <summary>
        /// Converts a file name in an ASCII sortable format
        /// </summary>
        /// <param name="file">File Name</param>
        /// <returns>Sortable Name</returns>
        public string GetSortableName(string file)
        {
            StringBuilder symbol   = new StringBuilder();
            StringBuilder interval = new StringBuilder();

            foreach (char c in Path.GetFileNameWithoutExtension(file).Trim().ToUpper())
            {
                if (Char.IsLetter(c))
                    symbol.Append(c);
                else if (Char.IsDigit(c))
                    interval.Append(c);
            }

            return symbol + Int32.Parse(interval.ToString()).ToString("00000");
        }

        /// <summary>
        /// Converts a Sortable Name back to the Full File Name
        /// </summary>
        /// <param name="sortableName">Sortable Name</param>
        /// <returns>File Name (without directory information)</returns>
        public string GetFullName(string sortableName)
        {
            StringBuilder symbol = new StringBuilder();
            StringBuilder interval = new StringBuilder();

            foreach (char c in sortableName)
            {
                if (Char.IsLetter(c))
                    symbol.Append(c);
                else if (Char.IsDigit(c))
                    interval.Append(c);
            }

            return symbol + Int32.Parse(interval.ToString()).ToString() + ".hst";
        }
 
    }
}
