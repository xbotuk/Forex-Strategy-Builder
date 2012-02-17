// Data Horizon
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Data Horizon Class
    /// </summary>
    public class Data_Horizon : Form
    {
        Button         btnAccept;
        Button         btnCancel;
        Button         btnHelp;
        Fancy_Panel    pnlBase;
        DateTimePicker dtpStartTime;
        DateTimePicker dtpEndTime;
        CheckBox       cbxUseEndTime;
        CheckBox       cbxUseStartTime;
        NumericUpDown  nudMaxBars;
        Label          lblMaxBars;
        Label          lblMinBars;
        ToolTip        toolTip = new ToolTip();

        public int MaxBars { get; private set; }
        public bool UseStartTime { get; private set; }
        public bool UseEndTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Data_Horizon(int maxBars, DateTime startTime, DateTime endTime, bool useStartTime, bool useEndTime)
        {
            MaxBars      = maxBars;
            StartTime    = startTime;
            EndTime      = endTime;
            UseEndTime   = useEndTime;
            UseStartTime = useStartTime;

            btnAccept       = new Button();
            btnHelp         = new Button();
            btnCancel       = new Button();
            pnlBase         = new Fancy_Panel();
            dtpStartTime    = new DateTimePicker();
            dtpEndTime      = new DateTimePicker();
            cbxUseEndTime   = new CheckBox();
            cbxUseStartTime = new CheckBox();
            nudMaxBars      = new NumericUpDown();
            lblMaxBars      = new Label();
            lblMinBars      = new Label();

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Data Horizon");

            //Button Help
            btnHelp.Parent = this;
            btnHelp.Name   = "Help";
            btnHelp.Text   = Language.T("Help");
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Click += BtnHelp_Click;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Ok";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // Panel Base
            pnlBase.Parent = this;

            // Check box UesStartTime
            cbxUseStartTime.Parent    = pnlBase;
            cbxUseStartTime.AutoSize  = true;
            cbxUseStartTime.ForeColor = LayoutColors.ColorControlText;
            cbxUseStartTime.BackColor = Color.Transparent;
            cbxUseStartTime.Text      = Language.T("Remove data older than:");
            cbxUseStartTime.CheckStateChanged += UseStartTimeCheckStateChanged;
            toolTip.SetToolTip(cbxUseStartTime, Language.T("All data older than the specified date will be cut out."));

            // Check box UesEndTime
            cbxUseEndTime.Parent    = pnlBase;
            cbxUseEndTime.ForeColor = LayoutColors.ColorControlText;
            cbxUseEndTime.BackColor = Color.Transparent;
            cbxUseEndTime.AutoSize  = true;
            cbxUseEndTime.Text      = Language.T("Remove data newer than:");
            cbxUseEndTime.CheckStateChanged += UseEndTimeCheckStateChanged;
            toolTip.SetToolTip(cbxUseEndTime, Language.T("All data newer than the specified date will be cut out."));

            // StartTime
            dtpStartTime.Parent        = pnlBase;
            dtpStartTime.ForeColor     = LayoutColors.ColorControlText;
            dtpStartTime.Format        = DateTimePickerFormat.Custom;
            dtpStartTime.CustomFormat  = "yyyy-MM-dd, HH:mm";
            dtpStartTime.ShowUpDown    = true;
            dtpStartTime.ValueChanged += StartTimeValueChanged;

            // EndTime
            dtpEndTime.Parent        = pnlBase;
            dtpEndTime.ForeColor     = LayoutColors.ColorControlText;
            dtpEndTime.Format        = DateTimePickerFormat.Custom;
            dtpEndTime.CustomFormat  = "yyyy-MM-dd, HH:mm";
            dtpEndTime.ShowUpDown    = true;
            dtpEndTime.ValueChanged += EndTimeValueChanged;

            // LabelMaxBars
            lblMaxBars.Parent    = pnlBase;
            lblMaxBars.AutoSize  = true;
            lblMaxBars.ForeColor = LayoutColors.ColorControlText;
            lblMaxBars.BackColor = Color.Transparent;
            lblMaxBars.Text      = Language.T("Maximum number of bars:");
            lblMaxBars.TextAlign = ContentAlignment.MiddleLeft;

            // MaxBars
            nudMaxBars.BeginInit();
            nudMaxBars.Parent    = pnlBase;
            nudMaxBars.Name      = "MaxBars";
            nudMaxBars.Minimum   = Configs.MIN_BARS;
            nudMaxBars.Maximum   = Configs.MAX_BARS;
            nudMaxBars.ThousandsSeparator = true;
            nudMaxBars.ValueChanged += MaxBarsValueChanged;
            nudMaxBars.TextAlign     = HorizontalAlignment.Center;
            nudMaxBars.EndInit();

            // Label MinBars
            lblMinBars.Parent    = pnlBase;
            lblMinBars.AutoSize  = true;
            lblMinBars.ForeColor = LayoutColors.ColorControlText;
            lblMinBars.BackColor = Color.Transparent;
            lblMinBars.Text      = Language.T("Minimum number of bars:") + " " + Configs.MIN_BARS;
            lblMinBars.TextAlign = ContentAlignment.MiddleLeft;
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
            catch { }
        }

        void StartTimeValueChanged(object sender, EventArgs e)
        {
            StartTime = dtpStartTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                EndTime = StartTime + oneDay;
                dtpEndTime.Value = EndTime;
            }
        }

        void EndTimeValueChanged(object sender, EventArgs e)
        {
            EndTime = dtpEndTime.Value;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);
            if (EndTime - StartTime < oneDay)
            {
                StartTime = EndTime - oneDay;
                dtpStartTime.Value = StartTime;
            }
        }

        void UseStartTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseStartTime = cbxUseStartTime.Checked;
            dtpStartTime.Enabled = UseStartTime;
        }

        void UseEndTimeCheckStateChanged(object sender, EventArgs e)
        {
            UseEndTime = cbxUseEndTime.Checked;
            dtpEndTime.Enabled = UseEndTime;
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
            nudMaxBars.Value        = MaxBars;
            dtpStartTime.Value      = StartTime;
            dtpEndTime.Value        = EndTime;
            cbxUseEndTime.Checked   = UseEndTime;
            dtpEndTime.Enabled      = UseEndTime;
            cbxUseStartTime.Checked = UseStartTime;
            dtpStartTime.Enabled    = UseStartTime;

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
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            btnHelp.Size     = new Size(buttonWidth, buttonHeight);
            btnHelp.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Panel Base
            pnlBase.Size     = new Size(ClientSize.Width - 2 * space, btnAccept.Top - btnVertSpace - space);
            pnlBase.Location = new Point(space, space);

            //Label Max bars
            lblMaxBars.Location = new Point(space, 2 * space + 2 * border);

            //chboxUseStartDate
            cbxUseStartTime.Location = new Point(space + 4, lblMaxBars.Bottom + 4 * space);

            // Start Date
            dtpStartTime.Width    = pnlBase.ClientSize.Width - 2 * space - 8;
            dtpStartTime.Location = new Point(space + 4, cbxUseStartTime.Bottom + space);

            //chboxUseEndDate
            cbxUseEndTime.Location = new Point(space + 4, dtpStartTime.Bottom + 4 * space);

            // End Date
            dtpEndTime.Width    = dtpStartTime.Width;
            dtpEndTime.Location = new Point(space + 4, cbxUseEndTime.Bottom + space);

            //numUpDownMaxBars
            nudMaxBars.Width    = 80;
            nudMaxBars.Location = new Point(dtpStartTime.Right - nudMaxBars.Width, 2 * space + 2 * border - 2);

            // lblMinBars
            lblMinBars.Location = new Point(lblMaxBars.Left, dtpEndTime.Bottom + 3 * space);
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
