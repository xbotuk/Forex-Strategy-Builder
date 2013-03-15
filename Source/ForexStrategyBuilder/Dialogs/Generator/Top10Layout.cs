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
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    public sealed class Top10Layout : Panel
    {
        private const int Space = 3;
        private readonly FlowLayoutPanel flowLayoutStrategy;

        private readonly int maxStrategies;
        private readonly ToolTip toolTip;
        private readonly SortableDictionary<float, Top10StrategyInfo> top10Holder;
        private readonly VScrollBar vScrollBarStrategy;

        private float maxValue = float.MinValue;
        private float minValue = float.MaxValue;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Top10Layout(int maxStrategies)
        {
            toolTip = new ToolTip();

            this.maxStrategies = maxStrategies;
            BackColor = LayoutColors.ColorControlBack;

            top10Holder = new SortableDictionary<float, Top10StrategyInfo>();

            flowLayoutStrategy = new FlowLayoutPanel();
            vScrollBarStrategy = new VScrollBar();

            // FlowLayoutStrategy
            flowLayoutStrategy.Parent = this;
            flowLayoutStrategy.AutoScroll = false;
            flowLayoutStrategy.BackColor = LayoutColors.ColorControlBack;

            // VScrollBarStrategy
            vScrollBarStrategy.Parent = this;
            vScrollBarStrategy.TabStop = true;
            vScrollBarStrategy.Scroll += VScrollBarScroll;
        }

        /// <summary>
        ///     Check whether the strategy has to be added in Top10 list
        /// </summary>
        public bool IsNominated(float value)
        {
            bool nominated = top10Holder.Count < maxStrategies && value > 0 ||
                             top10Holder.Count == maxStrategies && value > minValue;

            return nominated;
        }

        /// <summary>
        ///     Arranges the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            flowLayoutStrategy.SuspendLayout();
            SetVerticalScrollBar();
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        ///     Adds a strategy to the top 10 Layout
        /// </summary>
        public void AddStrategyInfo(Top10StrategyInfo strategyInfo)
        {
            if (top10Holder.ContainsKey(strategyInfo.Value))
                return;

            if (top10Holder.Count == maxStrategies && strategyInfo.Value <= minValue)
                return;

            if (top10Holder.Count == maxStrategies && strategyInfo.Value > minValue)
            {
                top10Holder.Remove(minValue);
                top10Holder.Add(strategyInfo.Value, strategyInfo);
            }
            else if (top10Holder.Count < maxStrategies)
            {
                top10Holder.Add(strategyInfo.Value, strategyInfo);
            }

            top10Holder.ReverseSort();

            minValue = float.MaxValue;
            maxValue = float.MinValue;
            foreach (var keyValue in top10Holder)
            {
                if (minValue > keyValue.Key)
                    minValue = keyValue.Key;
                if (maxValue < keyValue.Key)
                    maxValue = keyValue.Key;
            }

            foreach (var keyValue in top10Holder)
                keyValue.Value.Top10Slot.IsSelected = false;

            top10Holder[maxValue].Top10Slot.IsSelected = true;

            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        ///     Resets the Top 10 layout.
        /// </summary>
        public void ClearTop10Slots()
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
            top10Holder.Clear();

            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        ///     Clears the selection attribute of the slots.
        /// </summary>
        public void ClearSelectionOfSelectedSlot()
        {
            foreach (var keyValue in top10Holder)
                if (keyValue.Value.Top10Slot.IsSelected)
                {
                    keyValue.Value.Top10Slot.IsSelected = false;
                    keyValue.Value.Top10Slot.Invalidate();
                }
        }

        /// <summary>
        ///     Returns strategy with the selected balance;
        /// </summary>
        public Strategy GetStrategy(int balance)
        {
            foreach (var keyValuePair in top10Holder)
                if (keyValuePair.Value.Balance == balance)
                    return keyValuePair.Value.TheStrategy;

            throw new ArgumentOutOfRangeException("balance");
        }

        /// <summary>
        ///     Arranges slots in the layout.
        /// </summary>
        private void ArrangeTop10Slots()
        {
            flowLayoutStrategy.SuspendLayout();
            flowLayoutStrategy.Controls.Clear();
            foreach (var keyValue in top10Holder)
            {
                Top10Slot top10Slot = keyValue.Value.Top10Slot;
                top10Slot.Width = ClientSize.Width - vScrollBarStrategy.Width - 2*Space;
                top10Slot.Margin = new Padding(Space, Space, 0, 0);
                top10Slot.Cursor = Cursors.Hand;
                toolTip.SetToolTip(top10Slot, Language.T("Activate the strategy."));
                flowLayoutStrategy.Controls.Add(top10Slot);
            }
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        ///     Shows, hides, sets the scrollbar.
        /// </summary>
        private void SetVerticalScrollBar()
        {
            int width = ClientSize.Width - vScrollBarStrategy.Width;
            int height = ClientSize.Height;
            int totalHeight = 100;

            if (top10Holder != null && top10Holder.Count > 0)
                totalHeight = top10Holder.Count*70;

            if (totalHeight < height)
            {
                vScrollBarStrategy.Enabled = false;
                vScrollBarStrategy.Visible = false;
            }
            else
            {
                vScrollBarStrategy.Enabled = true;
                vScrollBarStrategy.Visible = true;
                vScrollBarStrategy.Value = 0;
                vScrollBarStrategy.SmallChange = 30;
                vScrollBarStrategy.LargeChange = 60;
                vScrollBarStrategy.Maximum = Math.Max(totalHeight - height + 80, 0);
                vScrollBarStrategy.Location = new Point(width, 0);
                vScrollBarStrategy.Height = height;
                vScrollBarStrategy.Cursor = Cursors.Default;
            }

            flowLayoutStrategy.Width = width;
            flowLayoutStrategy.Height = totalHeight;
            flowLayoutStrategy.Location = Point.Empty;
        }

        /// <summary>
        ///     The Scrolling moves the flowLayout
        /// </summary>
        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            var vscroll = (VScrollBar) sender;
            flowLayoutStrategy.Location = new Point(0, -vscroll.Value);
        }
    }
}