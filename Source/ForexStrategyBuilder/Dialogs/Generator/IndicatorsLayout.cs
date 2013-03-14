// Ban Indicators
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    public class IndicatorsLayout : Panel
    {
        private List<string> BannedEntryFilterIndicators { get; set; }
        private List<string> BannedEntryIndicators { get; set; }
        private List<string> BannedExitFilterIndicators { get; set; }
        private List<string> BannedExitIndicators { get; set; }
        private ToolStripComboBox CbxIndicatorSlot { get; set; }
        private FlowLayoutPanel FlowLayoutIndicators { get; set; }
        private Panel LayoutBase { get; set; }
        private ToolStrip TsIndLayout { get; set; }
        private ToolStripButton TsbtnSelectAll { get; set; }
        private ToolStripButton TsbtnSelectNone { get; set; }
        private ToolStripButton TsbtnStatus { get; set; }
        private VScrollBar VScrollBar { get; set; }

        private SlotTypes _currentSlotType = SlotTypes.Open;

        private bool _isBlocked;
        private const int Space = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        public IndicatorsLayout()
        {
            BannedEntryFilterIndicators = new List<string>();
            BannedEntryIndicators = new List<string>();
            BannedExitFilterIndicators = new List<string>();
            BannedExitIndicators = new List<string>();

            TsIndLayout = new ToolStrip();
            LayoutBase = new Panel();
            FlowLayoutIndicators = new FlowLayoutPanel();
            VScrollBar = new VScrollBar();
            CbxIndicatorSlot = new ToolStripComboBox();

            TsIndLayout.CanOverflow = false;

            CbxIndicatorSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxIndicatorSlot.AutoSize = false;
            var items = new[]
                              {
                                  Language.T("Opening Point of the Position"),
                                  Language.T("Opening Logic Condition"),
                                  Language.T("Closing Point of the Position"),
                                  Language.T("Closing Logic Condition")
                              };
            foreach (var item in items)
            CbxIndicatorSlot.Items.Add(item);
            CbxIndicatorSlot.SelectedIndex = 0;
            CbxIndicatorSlot.SelectedIndexChanged += CbxIndicatorSlotSelectedIndexChanged;

            TsbtnSelectAll = new ToolStripButton
                                 {
                                     Name = "tsbtnSelectAll",
                                     DisplayStyle = ToolStripItemDisplayStyle.Image,
                                     Image = Resources.optimizer_select_all,
                                     ToolTipText = Language.T("Allow all indicators."),
                                     Alignment = ToolStripItemAlignment.Right
                                 };
            TsbtnSelectAll.Click += ButtonsClick;

            TsbtnSelectNone = new ToolStripButton
                                  {
                                      Name = "tsbtnSelectNone",
                                      DisplayStyle = ToolStripItemDisplayStyle.Image,
                                      Image = Resources.optimizer_select_none,
                                      ToolTipText = Language.T("Ban all indicators."),
                                      Alignment = ToolStripItemAlignment.Right
                                  };
            TsbtnSelectNone.Click += ButtonsClick;

            TsbtnStatus = new ToolStripButton
                              {
                                  Name = "tsbtnStatus",
                                  Text = Language.T("banned"),
                                  Alignment = ToolStripItemAlignment.Right
                              };
            TsbtnStatus.Click += ButtonsClick;

            TsIndLayout.Items.Add(CbxIndicatorSlot);
            TsIndLayout.Items.Add(TsbtnStatus);
            TsIndLayout.Items.Add(TsbtnSelectNone);
            TsIndLayout.Items.Add(TsbtnSelectAll);

            // Layout base
            LayoutBase.Parent = this;
            LayoutBase.Dock = DockStyle.Fill;
            LayoutBase.BackColor = LayoutColors.ColorControlBack;

            // Tool Strip Strategy
            TsIndLayout.Parent = this;
            TsIndLayout.Dock = DockStyle.Top;

            // flowLayoutIndicators
            FlowLayoutIndicators.Parent = LayoutBase;
            FlowLayoutIndicators.AutoScroll = false;
            FlowLayoutIndicators.AutoSize = true;
            FlowLayoutIndicators.FlowDirection = FlowDirection.TopDown;
            FlowLayoutIndicators.BackColor = LayoutColors.ColorControlBack;

            // VScrollBarStrategy
            VScrollBar.Parent = LayoutBase;
            VScrollBar.TabStop = true;
            VScrollBar.Scroll += VScrollBarScroll;

            InitBannedIndicators();
            SetStatusButton();
            ArrangeIndicatorsSlots();
            VScrollBar.Select();
        }

        /// <summary>
        /// Reads config file record and arranges lists.
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
            SlotTypes indSlot = SlotTypes.NotDefined;
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
                    if (!BannedEntryIndicators.Contains(ind))
                        BannedEntryIndicators.Add(ind);

                if (indSlot == SlotTypes.OpenFilter && ind != "")
                    if (!BannedEntryFilterIndicators.Contains(ind))
                        BannedEntryFilterIndicators.Add(ind);

                if (indSlot == SlotTypes.Close && ind != "")
                    if (!BannedExitIndicators.Contains(ind))
                        BannedExitIndicators.Add(ind);

                if (indSlot == SlotTypes.CloseFilter && ind != "")
                    if (!BannedExitFilterIndicators.Contains(ind))
                        BannedExitFilterIndicators.Add(ind);
            }
        }

        /// <summary>
        /// Checks if the indicator is in the ban list.
        /// </summary>
        public bool IsIndicatorBanned(SlotTypes slotType, string indicatorName)
        {
            bool bann = false;

            if (slotType == SlotTypes.Open)
                bann = BannedEntryIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.OpenFilter)
                bann = BannedEntryFilterIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.Close)
                bann = BannedExitIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.CloseFilter)
                bann = BannedExitFilterIndicators.Contains(indicatorName);

            return bann;
        }

        public void BlockIndicatorChange()
        {
            _isBlocked = true;
            TsbtnSelectAll.Enabled = false;
            TsbtnSelectNone.Enabled = false;
            ArrangeIndicatorsSlots();
        }

        public void UnBlockIndicatorChange()
        {
            _isBlocked = false;
            TsbtnSelectAll.Enabled = true;
            TsbtnSelectNone.Enabled = true;
            ArrangeIndicatorsSlots();
        }

        /// <summary>
        /// Writes banned indicator in the config file.
        /// </summary>
        public void SetConfigFile()
        {
            const string nl = ";";
            string config = "__OpenPoint__" + nl;
            foreach (string ind in BannedEntryIndicators)
                config += ind + nl;
            config += "__OpenFilters__" + nl;
            foreach (string ind in BannedEntryFilterIndicators)
                config += ind + nl;
            config += "__ClosePoint__" + nl;
            foreach (string ind in BannedExitIndicators)
                config += ind + nl;
            config += "__CloseFilters__" + nl;
            foreach (string ind in BannedExitFilterIndicators)
                config += ind + nl;

            Configs.BannedIndicators = config;
        }

        /// <summary>
        /// Rearranges layout.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            CbxIndicatorSlot.Width = TsIndLayout.ClientSize.Width - TsbtnSelectAll.Width - TsbtnSelectNone.Width -
                                     TsbtnStatus.Width - 15;
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Change of the slot type.
        /// </summary>
        private void CbxIndicatorSlotSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSlotType = (SlotTypes) (Enum.GetValues(typeof (SlotTypes)).GetValue(CbxIndicatorSlot.SelectedIndex + 1));
            ArrangeIndicatorsSlots();
            SetStatusButton();
            SetVerticalScrollBar();
            VScrollBar.Select();
        }

        /// <summary>
        /// Change of the indicator ban state.
        /// </summary>
        private void ChbxIndicatorCheckedChanged(object sender, EventArgs e)
        {
            var chbxIndicator = (CheckBox) sender;
            bool isBanned = !chbxIndicator.Checked;
            string indicatorName = chbxIndicator.Text;

            switch (_currentSlotType)
            {
                case SlotTypes.Open:
                    if (isBanned)
                    {
                        if (!BannedEntryIndicators.Contains(indicatorName))
                            BannedEntryIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (BannedEntryIndicators.Contains(indicatorName))
                            BannedEntryIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.OpenFilter:
                    if (isBanned)
                    {
                        if (!BannedEntryFilterIndicators.Contains(indicatorName))
                            BannedEntryFilterIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (BannedEntryFilterIndicators.Contains(indicatorName))
                            BannedEntryFilterIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.Close:
                    if (isBanned)
                    {
                        if (!BannedExitIndicators.Contains(indicatorName))
                            BannedExitIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (BannedExitIndicators.Contains(indicatorName))
                            BannedExitIndicators.Remove(indicatorName);
                    }
                    break;
                case SlotTypes.CloseFilter:
                    if (isBanned)
                    {
                        if (!BannedExitFilterIndicators.Contains(indicatorName))
                            BannedExitFilterIndicators.Add(indicatorName);
                    }
                    else
                    {
                        if (BannedExitFilterIndicators.Contains(indicatorName))
                            BannedExitFilterIndicators.Remove(indicatorName);
                    }
                    break;
            }

            SetStatusButton();
            VScrollBar.Select();
        }

        /// <summary>
        /// Arranges the indicators in the layout.
        /// </summary>
        private void ArrangeIndicatorsSlots()
        {
            var currentIndicators = new List<string>();
            switch (_currentSlotType)
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

            FlowLayoutIndicators.SuspendLayout();
            FlowLayoutIndicators.Controls.Clear();
            FlowLayoutIndicators.Height = 0;
            foreach (string indicatorName in currentIndicators)
            {
                var chbxIndicator = new CheckBox {AutoSize = true, Checked = true};
                switch (_currentSlotType)
                {
                    case SlotTypes.Open:
                        chbxIndicator.Checked = !BannedEntryIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.OpenFilter:
                        chbxIndicator.Checked = !BannedEntryFilterIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.Close:
                        chbxIndicator.Checked = !BannedExitIndicators.Contains(indicatorName);
                        break;
                    case SlotTypes.CloseFilter:
                        chbxIndicator.Checked = !BannedExitFilterIndicators.Contains(indicatorName);
                        break;
                }
                chbxIndicator.Margin = new Padding(Space, Space, 0, 0);
                chbxIndicator.Text = indicatorName;
                chbxIndicator.Enabled = !_isBlocked;
                chbxIndicator.CheckedChanged += ChbxIndicatorCheckedChanged;
                FlowLayoutIndicators.Controls.Add(chbxIndicator);
            }
            FlowLayoutIndicators.ResumeLayout();
        }

        /// <summary>
        /// Shows, hides, sets the scrollbar.
        /// </summary>
        private void SetVerticalScrollBar()
        {
            int width = LayoutBase.Width - VScrollBar.Width;
            int height = LayoutBase.Height;
            int totalHeight = FlowLayoutIndicators.Height;

            VScrollBar.Enabled = true;
            VScrollBar.Visible = true;
            VScrollBar.Value = 0;
            VScrollBar.SmallChange = 30;
            VScrollBar.LargeChange = 60;
            VScrollBar.Maximum = Math.Max(totalHeight - height + 60, 0);
            VScrollBar.Location = new Point(width, 0);
            VScrollBar.Height = height;
            VScrollBar.Cursor = Cursors.Default;

            FlowLayoutIndicators.Location = new Point(0, 0);
        }

        /// <summary>
        /// Sets the text of button Status.
        /// </summary>
        private void SetStatusButton()
        {
            int bannedCount = BannedEntryIndicators.Count + BannedEntryFilterIndicators.Count;
            bannedCount += BannedExitIndicators.Count + BannedExitFilterIndicators.Count;
            TsbtnStatus.Text = bannedCount + " " + Language.T("banned");
            CbxIndicatorSlot.Width = TsIndLayout.ClientSize.Width - TsbtnSelectAll.Width - TsbtnSelectNone.Width -
                                     TsbtnStatus.Width - 15;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// </summary>
        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            var vscroll = (VScrollBar) sender;
            FlowLayoutIndicators.Location = new Point(0, -vscroll.Value);
        }

        /// <summary>
        /// ToolStrip Buttons click
        /// </summary>
        private void ButtonsClick(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtnSelectAll")
            {
                switch (_currentSlotType)
                {
                    case SlotTypes.Open:
                        BannedEntryIndicators.Clear();
                        break;
                    case SlotTypes.OpenFilter:
                        BannedEntryFilterIndicators.Clear();
                        break;
                    case SlotTypes.Close:
                        BannedExitIndicators.Clear();
                        break;
                    case SlotTypes.CloseFilter:
                        BannedExitFilterIndicators.Clear();
                        break;
                }

                ArrangeIndicatorsSlots();
                SetStatusButton();
            }
            else if (name == "tsbtnSelectNone")
            {
                switch (_currentSlotType)
                {
                    case SlotTypes.Open:
                        BannedEntryIndicators.Clear();
                        BannedEntryIndicators.AddRange(IndicatorStore.OpenPointIndicators.GetRange(0, IndicatorStore.OpenPointIndicators.Count));
                        break;
                    case SlotTypes.OpenFilter:
                        BannedEntryFilterIndicators.Clear();
                        BannedEntryFilterIndicators.AddRange(IndicatorStore.OpenFilterIndicators.GetRange(0, IndicatorStore.OpenFilterIndicators.Count));
                        break;
                    case SlotTypes.Close:
                        BannedExitIndicators.Clear();
                        BannedExitIndicators.AddRange(IndicatorStore.ClosePointIndicators.GetRange(0, IndicatorStore.ClosePointIndicators.Count));
                        break;
                    case SlotTypes.CloseFilter:
                        BannedExitFilterIndicators.Clear();
                        BannedExitFilterIndicators.AddRange(IndicatorStore.CloseFilterIndicators.GetRange(0, IndicatorStore.CloseFilterIndicators.Count));
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
        /// Shows all banned indicators.
        /// </summary>
        private void ShowStatus()
        {
            string text = "";

            if (BannedEntryIndicators.Count > 0)
            {
                text = "<h2>" + Language.T("Opening Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in BannedEntryIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (BannedEntryFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Opening Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in BannedEntryFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (BannedExitIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in BannedExitIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (BannedExitFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in BannedExitFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            var msgbox = new FancyMessageBox(text, Language.T("Banned Indicators"));
            msgbox.Show();
        }
    }
}