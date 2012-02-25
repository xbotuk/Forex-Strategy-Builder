// Data Horizon
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Data Horizon Class
    /// </summary>
    public class DataHorizon : Form
    {
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnHelp { get; set; }
        private FancyPanel PnlBase { get; set; }
        private DateTimePicker DtpStartTime { get; set; }
        private DateTimePicker DtpEndTime { get; set; }
        private CheckBox CbxUseEndTime { get; set; }
        private CheckBox CbxUseStartTime { get; set; }
        private NumericUpDown NUDMaxBars { get; set; }
        private Label LblMaxBars { get; set; }
        private Label LblMinBars { get; set; }
        private readonly ToolTip _toolTip = new ToolTip();

        public int MaxBars { get; private set; }
        public bool UseStartTime { get; private set; }
        public bool UseEndTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataHorizon(int maxBars, DateTime startTime, DateTime endTime, bool useStartTime, bool useEndTime)
        {
            MaxBars      = maxBars;
            StartTime    = startTime;
            EndTime      = endTime;
            UseEndTime   = useEndTime;
            UseStartTime = useStartTime;

            BtnAccept       = new Button();
            BtnHelp         = new Button();
            BtnCancel       = new Button();
            PnlBase         = new FancyPanel();
            DtpStartTime    = new DateTimePicker();
            DtpEndTime      = new DateTimePicker();
            CbxUseEndTime   = new CheckBox();
            CbxUseStartTime = new CheckBox();
            NUDMaxBars      = new NumericUpDown();
            LblMaxBars      = new Label();
            LblMinBars      = new Label();

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = BtnAccept;
            Text            = Language.T("Data Horizon");

            //Button Help
            BtnHelp.Parent = this;
            BtnHelp.Name   = "Help";
            BtnHelp.Text   = Language.T("Help");
            BtnHelp.UseVisualStyleBackColor = true;
            BtnHelp.Click += BtnHelp_Click;

            //Button Accept
            BtnAccept.Parent       = this;
            BtnAccept.Name         = "Ok";
            BtnAccept.Text         = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent       = this;
            BtnCancel.Text         = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            // Panel Base
            PnlBase.Parent = this;

            // Check box UesStartTime
            CbxUseStartTime.Parent    = PnlBase;
            CbxUseStartTime.AutoSize  = true;
            CbxUseStartTime.ForeColor = LayoutColors.ColorControlText;
            CbxUseStartTime.BackColor = Color.Transparent;
            CbxUseStartTime.Text      = Language.T("Remove data older than:");
            CbxUseStartTime.CheckStateChanged += UseStartTimeCheckStateChanged;
            _toolTip.SetToolTip(CbxUseStartTime, Language.T("All data older than the specified date will be cut out."));

            // Check box UesEndTime
            CbxUseEndTime.Parent    = PnlBase;
            CbxUseEndTime.ForeColor = LayoutColors.ColorControlText;
            CbxUseEndTime.BackColor = Color.Transparent;
            CbxUseEndTime.AutoSize  = true;
            CbxUseEndTime.Text      = Language.T("Remove data newer than:");
            CbxUseEndTime.CheckStateChanged += UseEndTimeCheckStateChanged;
            _toolTip.SetToolTip(CbxUseEndTime, Language.T("All data newer than the specified date will be cut out."));

            // StartTime
            DtpStartTime.Parent        = PnlBase;
            DtpStartTime.ForeColor     = LayoutColors.ColorControlText;
            DtpStartTime.Format        = DateTimePickerFormat.Custom;
            DtpStartTime.CustomFormat  = "yyyy-MM-dd, HH:mm";
            DtpStartTime.ShowUpDown    = true;
            DtpStartTime.ValueChanged += StartTimeValueChanged;

            // EndTime
            DtpEndTime.Parent        = PnlBase;
            DtpEndTime.ForeColor     = LayoutColors.ColorControlText;
            DtpEndTime.Format        = DateTimePickerFormat.Custom;
            DtpEndTime.CustomFormat  = "yyyy-MM-dd, HH:mm";
            DtpEndTime.ShowUpDown    = true;
            DtpEndTime.ValueChanged += EndTimeValueChanged;

            // LabelMaxBars
            LblMaxBars.Parent    = PnlBase;
            LblMaxBars.AutoSize  = true;
            LblMaxBars.ForeColor = LayoutColors.ColorControlText;
            LblMaxBars.BackColor = Color.Transparent;
            LblMaxBars.Text      = Language.T("Maximum number of bars:");
            LblMaxBars.TextAlign = ContentAlignment.MiddleLeft;

            // MaxBars
            NUDMaxBars.BeginInit();
            NUDMaxBars.Parent    = PnlBase;
            NUDMaxBars.Name      = "MaxBars";
            NUDMaxBars.Minimum   = Configs.MinBars;
            NUDMaxBars.Maximum   = Configs.MaxBarsLimit;
            NUDMaxBars.ThousandsSeparator = true;
            NUDMaxBars.ValueChanged += MaxBarsValueChanged;
            NUDMaxBars.TextAlign     = HorizontalAlignment.Center;
            NUDMaxBars.EndInit();

            // Label MinBars
            LblMinBars.Parent    = PnlBase;
            LblMinBars.AutoSize  = true;
            LblMinBars.ForeColor = LayoutColors.ColorControlText;
            LblMinBars.BackColor = Color.Transparent;
            LblMinBars.Text      = Language.T("Minimum number of bars:") + " " + Configs.MinBars;
            LblMinBars.TextAlign = ContentAlignment.MiddleLeft;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Go to the online help
        /// </summary>
        void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/fsb/manual/data_horizon");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        void StartTimeValueChanged(object sender, EventArgs e)
        {
            StartTime = DtpStartTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                EndTime = StartTime + oneDay;
                DtpEndTime.Value = EndTime;
            }
        }

        void EndTimeValueChanged(object sender, EventArgs e)
        {
            EndTime = DtpEndTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                StartTime = EndTime - oneDay;
                DtpStartTime.Value = StartTime;
            }
        }

        void UseStartTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseStartTime = CbxUseStartTime.Checked;
            DtpStartTime.Enabled = UseStartTime;
        }

        void UseEndTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseEndTime = CbxUseEndTime.Checked;
            DtpEndTime.Enabled = UseEndTime;
        }

        void MaxBarsValueChanged(object sender, EventArgs e)
        {
            var num = (NumericUpDown)sender;
            MaxBars = (int)num.Value;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            NUDMaxBars.Value        = MaxBars;
            DtpStartTime.Value      = StartTime;
            DtpEndTime.Value        = EndTime;
            CbxUseEndTime.Checked   = UseEndTime;
            DtpEndTime.Enabled      = UseEndTime;
            CbxUseStartTime.Checked = UseStartTime;
            DtpStartTime.Enabled    = UseStartTime;

            var buttonWidth  = (int)(Data.HorizontalDLU * 60);
            var btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, 230);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int)(Data.VerticalDLU * 15.5);
            var buttonWidth  = (int)(Data.HorizontalDLU * 60);
            var btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            var btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            var space        = btnHrzSpace;
            const int border = 2;

            // Button Cancel
            BtnCancel.Size     = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size     = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            BtnHelp.Size     = new Size(buttonWidth, buttonHeight);
            BtnHelp.Location = new Point(BtnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Panel Base
            PnlBase.Size     = new Size(ClientSize.Width - 2 * space, BtnAccept.Top - btnVertSpace - space);
            PnlBase.Location = new Point(space, space);

            //Label Max bars
            LblMaxBars.Location = new Point(space, 2 * space + 2 * border);

            //chboxUseStartDate
            CbxUseStartTime.Location = new Point(space + 4, LblMaxBars.Bottom + 4 * space);

            // Start Date
            DtpStartTime.Width    = PnlBase.ClientSize.Width - 2 * space - 8;
            DtpStartTime.Location = new Point(space + 4, CbxUseStartTime.Bottom + space);

            //chboxUseEndDate
            CbxUseEndTime.Location = new Point(space + 4, DtpStartTime.Bottom + 4 * space);

            // End Date
            DtpEndTime.Width    = DtpStartTime.Width;
            DtpEndTime.Location = new Point(space + 4, CbxUseEndTime.Bottom + space);

            //numUpDownMaxBars
            NUDMaxBars.Width    = 80;
            NUDMaxBars.Location = new Point(DtpStartTime.Right - NUDMaxBars.Width, 2 * space + 2 * border - 2);

            // lblMinBars
            LblMinBars.Location = new Point(LblMaxBars.Left, DtpEndTime.Bottom + 3 * space);
        }

        /// <summary>
        /// Form OnPaint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
