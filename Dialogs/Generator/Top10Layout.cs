// Top10Layout Classes
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    public sealed class Top10Layout : Panel
    {
        private readonly VScrollBar _vScrollBarStrategy;
        private readonly FlowLayoutPanel _flowLayoutStrategy;

        private readonly int _maxStrategies;
        private readonly ToolTip _toolTip;
        private readonly SortableDictionary<int, Top10StrategyInfo> _top10Holder;

        private const int Space = 3;
        private int _maxBalance = int.MinValue;
        private int _minBalance = int.MaxValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public Top10Layout(int maxStrategies)
        {
            _toolTip = new ToolTip();

            _maxStrategies = maxStrategies;
            BackColor = LayoutColors.ColorControlBack;

            _top10Holder = new SortableDictionary<int, Top10StrategyInfo>();

            _flowLayoutStrategy = new FlowLayoutPanel();
            _vScrollBarStrategy = new VScrollBar();

            // FlowLayoutStrategy
            _flowLayoutStrategy.Parent = this;
            _flowLayoutStrategy.AutoScroll = false;
            _flowLayoutStrategy.BackColor = LayoutColors.ColorControlBack;

            //VScrollBarStrategy
            _vScrollBarStrategy.Parent = this;
            _vScrollBarStrategy.TabStop = true;
            _vScrollBarStrategy.Scroll += VScrollBarScroll;
        }

        /// <summary>
        /// Check whether the strategy has to be added in Top10 list
        /// </summary>
        public bool IsNominated(int balance)
        {
            bool nominated = _top10Holder.Count < _maxStrategies && balance > 0;

            if (_top10Holder.Count == _maxStrategies && balance > _minBalance)
                nominated = true;

            return nominated;
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            _flowLayoutStrategy.SuspendLayout();
            SetVerticalScrollBar();
            _flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Adds a strategy to the top 10 Layout
        /// </summary>
        public void AddStrategyInfo(Top10StrategyInfo top10StrategyInfo)
        {
            if (_top10Holder.ContainsKey(top10StrategyInfo.Balance))
            {
                return;
            }

            if (_top10Holder.Count == _maxStrategies && top10StrategyInfo.Balance <= _minBalance)
                return;

            if (_top10Holder.Count == _maxStrategies && top10StrategyInfo.Balance > _minBalance)
            {
                _top10Holder.Remove(_minBalance);
                _top10Holder.Add(top10StrategyInfo.Balance, top10StrategyInfo);
            }
            else if (_top10Holder.Count < _maxStrategies)
                _top10Holder.Add(top10StrategyInfo.Balance, top10StrategyInfo);

            _top10Holder.ReverseSort();

            _minBalance = int.MaxValue;
            _maxBalance = int.MinValue;
            foreach (var keyValue in _top10Holder)
            {
                if (_minBalance > keyValue.Key)
                    _minBalance = keyValue.Key;
                if (_maxBalance < keyValue.Key)
                    _maxBalance = keyValue.Key;
            }

            foreach (var keyValue in _top10Holder)
                keyValue.Value.Top10Slot.IsSelected = false;

            _top10Holder[_maxBalance].Top10Slot.IsSelected = true;

            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Resets the Top 10 layout.
        /// </summary>
        public void ClearTop10Slots()
        {
            _minBalance = int.MaxValue;
            _maxBalance = int.MinValue;
            _top10Holder.Clear();

            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Clears the selection attribute of the slots.
        /// </summary>
        public void ClearSelectionOfSelectedSlot()
        {
            foreach (var keyValue in _top10Holder)
                if (keyValue.Value.Top10Slot.IsSelected)
                {
                    keyValue.Value.Top10Slot.IsSelected = false;
                    keyValue.Value.Top10Slot.Invalidate();
                }
        }

        /// <summary>
        /// Returns strategy with the selected balance;
        /// </summary>
        public Strategy GetStrategy(int balance)
        {
            return _top10Holder[balance].TheStrategy.Clone();
        }

        /// <summary>
        /// Arranges slots in the layout.
        /// </summary>
        private void ArrangeTop10Slots()
        {
            _flowLayoutStrategy.SuspendLayout();
            _flowLayoutStrategy.Controls.Clear();
            foreach (var keyValue in _top10Holder)
            {
                Top10Slot top10Slot = keyValue.Value.Top10Slot;
                top10Slot.Width = ClientSize.Width - _vScrollBarStrategy.Width - 2*Space;
                top10Slot.Margin = new Padding(Space, Space, 0, 0);
                top10Slot.Cursor = Cursors.Hand;
                _toolTip.SetToolTip(top10Slot, Language.T("Activate the strategy."));
                _flowLayoutStrategy.Controls.Add(top10Slot);
            }
            _flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Shows, hides, sets the scrollbar.
        /// </summary>
        private void SetVerticalScrollBar()
        {
            int width = ClientSize.Width - _vScrollBarStrategy.Width;
            int height = ClientSize.Height;
            int totalHeight = 100;

            if (_top10Holder != null && _top10Holder.Count > 0)
                totalHeight = _top10Holder.Count*70;

            if (totalHeight < height)
            {
                _vScrollBarStrategy.Enabled = false;
                _vScrollBarStrategy.Visible = false;
            }
            else
            {
                _vScrollBarStrategy.Enabled = true;
                _vScrollBarStrategy.Visible = true;
                _vScrollBarStrategy.Value = 0;
                _vScrollBarStrategy.SmallChange = 30;
                _vScrollBarStrategy.LargeChange = 60;
                _vScrollBarStrategy.Maximum = Math.Max(totalHeight - height + 80, 0);
                _vScrollBarStrategy.Location = new Point(width, 0);
                _vScrollBarStrategy.Height = height;
                _vScrollBarStrategy.Cursor = Cursors.Default;
            }

            _flowLayoutStrategy.Width = width;
            _flowLayoutStrategy.Height = totalHeight;
            _flowLayoutStrategy.Location = Point.Empty;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// </summary>
        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            var vscroll = (VScrollBar) sender;
            _flowLayoutStrategy.Location = new Point(0, -vscroll.Value);
        }
    }
}