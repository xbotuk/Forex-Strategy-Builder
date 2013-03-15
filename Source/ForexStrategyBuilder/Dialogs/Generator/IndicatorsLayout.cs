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
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    public class IndicatorsLayout : Panel
    {
        private const int Space = 3;

        private readonly List<string> bannedEntryFilterIndicators;
        private readonly List<string> bannedEntryIndicators;
        private readonly List<string> bannedExitFilterIndicators;
        private readonly List<string> bannedExitIndicators;
        private readonly ToolStripComboBox cbxIndicatorSlot;
        private readonly FlowLayoutPanel flowLayoutIndicators;
        private readonly Panel layoutBase;
        private readonly ToolStrip tsIndLayout;
        private readonly ToolStripButton tsbtnSelectAll;
        private readonly ToolStripButton tsbtnSelectNone;
        private readonly ToolStripButton tsbtnStatus;
        private readonly VScrollBar vScrollBar;
        private SlotTypes currentSlotType = SlotTypes.Open;
        private bool isBlocked;

        /// <summary>
        ///     Constructor
        /// </summary>
        public IndicatorsLayout()
        {
            bannedEntryFilterIndicators = new List<string>();
            bannedEntryIndicators = new List<string>();
            bannedExitFilterIndicators = new List<string>();
            bannedExitIndicators = new List<string>();

            tsIndLayout = new ToolStrip();
            layoutBase = new Panel();
            flowLayoutIndicators = new FlowLayoutPanel();
            vScrollBar = new VScrollBar();
            cbxIndicatorSlot = new ToolStripComboBox();

            tsIndLayout.CanOverflow = false;

            cbxIndicatorSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxIndicatorSlot.AutoSize = false;
            var items = new[]
                {
                    Language.T("Opening Point of the Position"),
                    Language.T("Opening Logic Condition"),
                    Language.T("Closing Point of the Position"),
                    Language.T("Closing Logic Condition")
                };
            foreach (string item in items)
                cbxIndicatorSlot.Items.Add(item);
            cbxIndicatorSlot.SelectedIndex = 0;
            cbxIndicatorSlot.SelectedIndexChanged += CbxIndicatorSlotSelectedIndexChanged;

            tsbtnSelectAll = new ToolStripButton
                {
                    Name = "tsbtnSelectAll",
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.optimizer_select_all,
                    ToolTipText = Language.T("Allow all indicators."),
                    Alignment = ToolStripItemAlignment.Right
                };
            tsbtnSelectAll.Click += ButtonsClick;

            tsbtnSelectNone = new ToolStripButton
                {
                    Name = "tsbtnSelectNone",
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.optimizer_select_none,
                    ToolTipText = Language.T("Ban all indicators."),
                    Alignment = ToolStripItemAlignment.Right
                };
            tsbtnSelectNone.Click += ButtonsClick;

            tsbtnStatus = new ToolStripButton
                {
                    Name = "tsbtnStatus",
                    Text = Language.T("banned"),
                    Alignment = ToolStripItemAlignment.Right
                };
            tsbtnStatus.Click += ButtonsClick;

            tsIndLayout.Items.Add(cbxIndicatorSlot);
            tsIndLayout.Items.Add(tsbtnStatus);
            tsIndLayout.Items.Add(tsbtnSelectNone);
            tsIndLayout.Items.Add(tsbtnSelectAll);

            // Layout base
            layoutBase.Parent = this;
            layoutBase.Dock = DockStyle.Fill;
            layoutBase.BackColor = LayoutColors.ColorControlBack;

            // Tool Strip Strategy
            tsIndLayout.Parent = this;
            tsIndLayout.Dock = DockStyle.Top;

            // flowLayoutIndicators
            flowLayoutIndicators.Parent = layoutBase;
            flowLayoutIndicators.AutoScroll = false;
            flowLayoutIndicators.AutoSize = true;
            flowLayoutIndicators.FlowDirection = FlowDirection.TopDown;
            flowLayoutIndicators.BackColor = LayoutColors.ColorControlBack;

            // VScrollBarStrategy
            vScrollBar.Parent = layoutBase;
            vScrollBar.TabStop = true;
            vScrollBar.Scroll += VScrollBarScroll;

            InitBannedIndicators();
            SetStatusButton();
            ArrangeIndicatorsSlots();
            vScrollBar.Select();
        }

        /// <summary>
        ///     Reads config file record and arranges lists.
        /// </summary>
        private void InitBannedIndicators()
        {
            string config = Configs.BannedIndicators;
            const string nl = ";";
            if (config == "")
            {
                // Preparing config string after reset.
                config = "__OpenPoint__" + nl + "__OpenFilters__" + nl + "__ClosePoint__" + nl + "__CloseFilters__" + nl;
                Configs.BannedIndicators = config;
                return;
            }

            string[] banned = config.Split(new[] {nl}, StringSplitOptions.RemoveEmptyEntries);
            var indSlot = SlotTypes.NotDefined;
            foreach (string ind in banned)
            {
                if (ind == "__OpenPoint__")
                {
                    indSlot = SlotTypes.Open;
                    continue;
                }
                if (ind == "__OpenFilters__")
                {
                    indSlot = SlotTypes.OpenFilter;
                    continue;
                }
                if (ind == "__ClosePoint__")
                {
                    indSlot = SlotTypes.Close;
                    continue;
                }
                if (ind == "__CloseFilters__")
                {
                    indSlot = SlotTypes.CloseFilter;
                    continue;
                }

                if (indSlot == SlotTypes.Open && ind != "")
                    if (!bannedEntryIndicators.Contains(ind))
                        bannedEntryIndicators.Add(ind);

                if (indSlot == SlotTypes.OpenFilter && ind != "")
                    if (!bannedEntryFilterIndicators.Contains(ind))
                        bannedEntryFilterIndicators.Add(ind);

                if (indSlot == SlotTypes.Close && ind != "")
                    if (!bannedExitIndicators.Contains(ind))
                        bannedExitIndicators.Add(ind);

                if (indSlot == SlotTypes.CloseFilter && ind != "")
                    if (!bannedExitFilterIndicators.Contains(ind))
                        bannedExitFilterIndicators.Add(ind);
            }
        }

        /// <summary>
        ///     Checks if the indicator is in the ban list.
        /// </summary>
        public bool IsIndicatorBanned(SlotTypes slotType, string indicatorName)
        {
            bool bann = false;

            if (slotType == SlotTypes.Open)
                bann = bannedEntryIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.OpenFilter)
                bann = bannedEntryFilterIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.Close)
                bann = bannedExitIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.CloseFilter)
                bann = bannedExitFilterIndicators.Contains(indicatorName);

            return bann;
        }

        public void BlockIndicatorChange()
        {
            isBlocked = true;
            tsbtnSelectAll.Enabled = false;
            tsbtnSelectNone.Enabled = false;
            ArrangeIndicatorsSlots();
        }

        public void UnBlockIndicatorChange()
        {
            isBlocked = false;
            tsbtnSelectAll.Enabled = true;
            tsbtnSelectNone.Enabled = true;
            ArrangeIndicatorsSlots();
        }

        /// <summary>
        ///     Writes banned indicator in the config file.
        /// </summary>
        public void SetConfigFile()
        {
            const string nl = ";";
            string config = "__OpenPoint__" + nl;
            foreach (string ind in bannedEntryIndicators)
                config += ind + nl;
            config += "__OpenFilters__" + nl;
            foreach (string ind in bannedEntryFilterIndicators)
                config += ind + nl;
            config += "__ClosePoint__" + nl;
            foreach (string ind in bannedExitIndicators)
                config += ind + nl;
            config += "__CloseFilters__" + nl;
            foreach (string ind in bannedExitFilterIndicators)
                config += ind + nl;

            Configs.BannedIndicators = config;
        }

        /// <summary>
        ///     Rearranges layout.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            cbxIndicatorSlot.Width = tsIndLayout.ClientSize.Width - tsbtnSelectAll.Width - tsbtnSelectNone.Width -
                                     tsbtnStatus.Width - 15;
            SetVerticalScrollBar();
        }

        /// <summary>
        ///     Change of the slot type.
        /// </summary>
        private void CbxIndicatorSlotSelectedIndexChanged(object sender, EventArgs e)
        {
            currentSlotType =
                (SlotTypes) (Enum.GetValues(typeof (SlotTypes)).GetValue(cbxIndicatorSlot.SelectedIndex + 1));
            ArrangeIndicatorsSlots();
            SetStatusButton();
            SetVerticalScrollBar();
            vScrollBar.Select();
        }

        /// <summary>
        ///     Change of the indicator ban state.
        /// </summary>
        private void ChbxIndicatorCheckedChanged(object sender, EventArgs e)
        {
            var chbxIndicator = (CheckBox) sender;
            bool isBanned = !chbxIndicator.Checked;
            string indicatorName = chbxIndicator.Text;

            switch (currentSlotType)
            {
                case SlotTypes.Open:
                    if (isBanned)
                    {
                        if (!bannedEntryIndicators.Contains(indicatorName))
                            bannedEntryIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (bannedEntryIndicators.Contains(indicatorName))
                            bannedEntryIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.OpenFilter:
                    if (isBanned)
                    {
                        if (!bannedEntryFilterIndicators.Contains(indicatorName))
                            bannedEntryFilterIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (bannedEntryFilterIndicators.Contains(indicatorName))
                            bannedEntryFilterIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.Close:
                    if (isBanned)
                    {
                        if (!bannedExitIndicators.Contains(indicatorName))
                            bannedExitIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (bannedExitIndicators.Contains(indicatorName))
                            bannedExitIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.CloseFilter:
                    if (isBanned)
                    {
                        if (!bannedExitFilterIndicators.Contains(indicatorName))
                            bannedExitFilterIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (bannedExitFilterIndicators.Contains(indicatorName))
                            bannedExitFilterIndicators.Remove(indicatorName);
                    }
                    break;
            }

            SetStatusButton();
            vScrollBar.Select();
        }

        /// <summary>
        ///     Arranges the indicators in the layout.
        /// </summary>
        private void ArrangeIndicatorsSlots()
        {
            var currentIndicators = new List<string>();
            switch (currentSlotType)
            {
                case SlotTypes.Open:
                    currentIndicators = IndicatorStore.OpenPointIndicators;
                    break;
                case SlotTypes.OpenFilter:
                    currentIndicators = IndicatorStore.OpenFilterIndicators;
                    break;
                case SlotTypes.Close:
                    currentIndicators = IndicatorStore.ClosePointIndicators;
                    break;
                case SlotTypes.CloseFilter:
                    currentIndicators = IndicatorStore.CloseFilterIndicators;
                    break;
            }

            flowLayoutIndicators.SuspendLayout();
            flowLayoutIndicators.Controls.Clear();
            flowLayoutIndicators.Height = 0;
            foreach (string indicatorName in currentIndicators)
            {
                var chbxIndicator = new CheckBox {AutoSize = true, Checked = true};
                switch (currentSlotType)
                {
                    case SlotTypes.Open:
                        chbxIndicator.Checked = !bannedEntryIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.OpenFilter:
                        chbxIndicator.Checked = !bannedEntryFilterIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.Close:
                        chbxIndicator.Checked = !bannedExitIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.CloseFilter:
                        chbxIndicator.Checked = !bannedExitFilterIndicators.Contains(indicatorName);
                        break;
                }
                chbxIndicator.Margin = new Padding(Space, Space, 0, 0);
                chbxIndicator.Text = indicatorName;
                chbxIndicator.Enabled = !isBlocked;
                chbxIndicator.CheckedChanged += ChbxIndicatorCheckedChanged;
                flowLayoutIndicators.Controls.Add(chbxIndicator);
            }
            flowLayoutIndicators.ResumeLayout();
        }

        /// <summary>
        ///     Shows, hides, sets the scrollbar.
        /// </summary>
        private void SetVerticalScrollBar()
        {
            int width = layoutBase.Width - vScrollBar.Width;
            int height = layoutBase.Height;
            int totalHeight = flowLayoutIndicators.Height;

            vScrollBar.Enabled = true;
            vScrollBar.Visible = true;
            vScrollBar.Value = 0;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = 60;
            vScrollBar.Maximum = Math.Max(totalHeight - height + 60, 0);
            vScrollBar.Location = new Point(width, 0);
            vScrollBar.Height = height;
            vScrollBar.Cursor = Cursors.Default;

            flowLayoutIndicators.Location = new Point(0, 0);
        }

        /// <summary>
        ///     Sets the text of button Status.
        /// </summary>
        private void SetStatusButton()
        {
            int bannedCount = bannedEntryIndicators.Count + bannedEntryFilterIndicators.Count;
            bannedCount += bannedExitIndicators.Count + bannedExitFilterIndicators.Count;
            tsbtnStatus.Text = bannedCount + " " + Language.T("banned");
            cbxIndicatorSlot.Width = tsIndLayout.ClientSize.Width - tsbtnSelectAll.Width - tsbtnSelectNone.Width -
                                     tsbtnStatus.Width - 15;
        }

        /// <summary>
        ///     The Scrolling moves the flowLayout
        /// </summary>
        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            var vscroll = (VScrollBar) sender;
            flowLayoutIndicators.Location = new Point(0, -vscroll.Value);
        }

        /// <summary>
        ///     ToolStrip Buttons click
        /// </summary>
        private void ButtonsClick(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtnSelectAll")
            {
                switch (currentSlotType)
                {
                    case SlotTypes.Open:
                        bannedEntryIndicators.Clear();
                        break;
                    case SlotTypes.OpenFilter:
                        bannedEntryFilterIndicators.Clear();
                        break;
                    case SlotTypes.Close:
                        bannedExitIndicators.Clear();
                        break;
                    case SlotTypes.CloseFilter:
                        bannedExitFilterIndicators.Clear();
                        break;
                }

                ArrangeIndicatorsSlots();
                SetStatusButton();
            }
            else if (name == "tsbtnSelectNone")
            {
                switch (currentSlotType)
                {
                    case SlotTypes.Open:
                        bannedEntryIndicators.Clear();
                        bannedEntryIndicators.AddRange(IndicatorStore.OpenPointIndicators);
                        break;
                    case SlotTypes.OpenFilter:
                        bannedEntryFilterIndicators.Clear();
                        bannedEntryFilterIndicators.AddRange(IndicatorStore.OpenFilterIndicators);
                        break;
                    case SlotTypes.Close:
                        bannedExitIndicators.Clear();
                        bannedExitIndicators.AddRange(IndicatorStore.ClosePointIndicators);
                        break;
                    case SlotTypes.CloseFilter:
                        bannedExitFilterIndicators.Clear();
                        bannedExitFilterIndicators.AddRange(IndicatorStore.CloseFilterIndicators);
                        break;
                }
                ArrangeIndicatorsSlots();
                SetStatusButton();
            }
            else if (name == "tsbtnStatus")
            {
                ShowStatus();
            }
        }

        /// <summary>
        ///     Shows all banned indicators.
        /// </summary>
        private void ShowStatus()
        {
            string text = "";

            if (bannedEntryIndicators.Count > 0)
            {
                text = "<h2>" + Language.T("Opening Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedEntryIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedEntryFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Opening Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedEntryFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedExitIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedExitIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedExitFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedExitFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            var msgbox = new FancyMessageBox(text, Language.T("Banned Indicators"));
            msgbox.Show();
        }
    }
}