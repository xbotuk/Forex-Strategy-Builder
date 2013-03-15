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

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Data Horizon Class
    /// </summary>
    public sealed class DataHorizon : Form
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public DataHorizon(int maxBars, DateTime startTime, DateTime endTime, bool useStartTime, bool useEndTime)
        {
            MaxBars = maxBars;
            StartTime = startTime;
            EndTime = endTime;
            UseEndTime = useEndTime;
            UseStartTime = useStartTime;

            BtnAccept = new Button();
            BtnHelp = new Button();
            BtnCancel = new Button();
            PnlBase = new FancyPanel();
            DtpStartTime = new DateTimePicker();
            DtpEndTime = new DateTimePicker();
            CbxUseEndTime = new CheckBox();
            CbxUseStartTime = new CheckBox();
            NudMaxBars = new NumericUpDown();
            LblMaxBars = new Label();
            LblMinBars = new Label();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Data Horizon");

            var toolTip = new ToolTip();

            // Button Help
            BtnHelp.Parent = this;
            BtnHelp.Name = "Help";
            BtnHelp.Text = Language.T("Help");
            BtnHelp.UseVisualStyleBackColor = true;
            BtnHelp.Click += BtnHelpClick;

            // Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Ok";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;

            // Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            // Panel Base
            PnlBase.Parent = this;

            // Check box UesStartTime
            CbxUseStartTime.Parent = PnlBase;
            CbxUseStartTime.AutoSize = true;
            CbxUseStartTime.ForeColor = LayoutColors.ColorControlText;
            CbxUseStartTime.BackColor = Color.Transparent;
            CbxUseStartTime.Text = Language.T("Remove data older than:");
            CbxUseStartTime.CheckStateChanged += UseStartTimeCheckStateChanged;
            toolTip.SetToolTip(CbxUseStartTime, Language.T("All data older than the specified date will be cut out."));

            // Check box UesEndTime
            CbxUseEndTime.Parent = PnlBase;
            CbxUseEndTime.ForeColor = LayoutColors.ColorControlText;
            CbxUseEndTime.BackColor = Color.Transparent;
            CbxUseEndTime.AutoSize = true;
            CbxUseEndTime.Text = Language.T("Remove data newer than:");
            CbxUseEndTime.CheckStateChanged += UseEndTimeCheckStateChanged;
            toolTip.SetToolTip(CbxUseEndTime, Language.T("All data newer than the specified date will be cut out."));

            // StartTime
            DtpStartTime.Parent = PnlBase;
            DtpStartTime.ForeColor = LayoutColors.ColorControlText;
            DtpStartTime.Format = DateTimePickerFormat.Custom;
            DtpStartTime.CustomFormat = "MMMM dd, yyyy - dddd,   HH : mm";
            DtpStartTime.ShowUpDown = true;
            DtpStartTime.ValueChanged += StartTimeValueChanged;

            // EndTime
            DtpEndTime.Parent = PnlBase;
            DtpEndTime.ForeColor = LayoutColors.ColorControlText;
            DtpEndTime.Format = DateTimePickerFormat.Custom;
            DtpEndTime.CustomFormat = "MMMM dd, yyyy - dddd,   HH : mm";
            DtpEndTime.ShowUpDown = true;
            DtpEndTime.ValueChanged += EndTimeValueChanged;

            // LabelMaxBars
            LblMaxBars.Parent = PnlBase;
            LblMaxBars.AutoSize = true;
            LblMaxBars.ForeColor = LayoutColors.ColorControlText;
            LblMaxBars.BackColor = Color.Transparent;
            LblMaxBars.Text = Language.T("Maximum number of bars:");
            LblMaxBars.TextAlign = ContentAlignment.MiddleLeft;

            // MaxBars
            NudMaxBars.BeginInit();
            NudMaxBars.Parent = PnlBase;
            NudMaxBars.Name = "MaxBars";
            NudMaxBars.Minimum = Configs.MinBars;
            NudMaxBars.Maximum = Configs.MaxBarsLimit;
            NudMaxBars.ThousandsSeparator = true;
            NudMaxBars.ValueChanged += MaxBarsValueChanged;
            NudMaxBars.TextAlign = HorizontalAlignment.Center;
            NudMaxBars.EndInit();

            // Label MinBars
            LblMinBars.Parent = PnlBase;
            LblMinBars.AutoSize = true;
            LblMinBars.ForeColor = LayoutColors.ColorControlText;
            LblMinBars.BackColor = Color.Transparent;
            LblMinBars.Text = Language.T("Minimum number of bars:") + " " + Configs.MinBars;
            LblMinBars.TextAlign = ContentAlignment.MiddleLeft;
        }

        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnHelp { get; set; }
        private FancyPanel PnlBase { get; set; }
        private DateTimePicker DtpStartTime { get; set; }
        private DateTimePicker DtpEndTime { get; set; }
        private CheckBox CbxUseEndTime { get; set; }
        private CheckBox CbxUseStartTime { get; set; }
        private NumericUpDown NudMaxBars { get; set; }
        private Label LblMaxBars { get; set; }
        private Label LblMinBars { get; set; }

        public int MaxBars { get; private set; }
        public bool UseStartTime { get; private set; }
        public bool UseEndTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        /// <summary>
        ///     Go to the online help
        /// </summary>
        private void BtnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/data_horizon");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void StartTimeValueChanged(object sender, EventArgs e)
        {
            StartTime = DtpStartTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                EndTime = StartTime + oneDay;
                DtpEndTime.Value = EndTime;
            }
        }

        private void EndTimeValueChanged(object sender, EventArgs e)
        {
            EndTime = DtpEndTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                StartTime = EndTime - oneDay;
                DtpStartTime.Value = StartTime;
            }
        }

        private void UseStartTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseStartTime = CbxUseStartTime.Checked;
            DtpStartTime.Enabled = UseStartTime;
        }

        private void UseEndTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseEndTime = CbxUseEndTime.Checked;
            DtpEndTime.Enabled = UseEndTime;
        }

        private void MaxBarsValueChanged(object sender, EventArgs e)
        {
            var num = (NumericUpDown) sender;
            MaxBars = (int) num.Value;
        }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            NudMaxBars.Value = MaxBars;
            DtpStartTime.Value = StartTime;
            DtpEndTime.Value = EndTime;
            CbxUseEndTime.Checked = UseEndTime;
            DtpEndTime.Enabled = UseEndTime;
            CbxUseStartTime.Checked = UseStartTime;
            DtpStartTime.Enabled = UseStartTime;

            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 230);
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
            const int border = 2;

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            BtnHelp.Size = new Size(buttonWidth, buttonHeight);
            BtnHelp.Location = new Point(BtnAccept.Left - buttonWidth - btnHrzSpace,
                                         ClientSize.Height - buttonHeight - btnVertSpace);

            // Panel Base
            PnlBase.Size = new Size(ClientSize.Width - 2*space, BtnAccept.Top - btnVertSpace - space);
            PnlBase.Location = new Point(space, space);

            // Label Max bars
            LblMaxBars.Location = new Point(space, 2*space + 2*border);

            // CbxUseStartTime
            CbxUseStartTime.Location = new Point(space + 4, LblMaxBars.Bottom + 4*space);

            // Start Date
            DtpStartTime.Width = PnlBase.ClientSize.Width - 2*space - 8;
            DtpStartTime.Location = new Point(space + 4, CbxUseStartTime.Bottom + space);

            // CbxUseEndTime
            CbxUseEndTime.Location = new Point(space + 4, DtpStartTime.Bottom + 4*space);

            // End Date
            DtpEndTime.Width = DtpStartTime.Width;
            DtpEndTime.Location = new Point(space + 4, CbxUseEndTime.Bottom + space);

            // NUDMaxBars
            NudMaxBars.Width = 80;
            NudMaxBars.Location = new Point(DtpStartTime.Right - NudMaxBars.Width, 2*space + 2*border - 2);

            // LblMinBars
            LblMinBars.Location = new Point(LblMaxBars.Left, DtpEndTime.Bottom + 3*space);
        }

        /// <summary>
        ///     Form OnPaint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}