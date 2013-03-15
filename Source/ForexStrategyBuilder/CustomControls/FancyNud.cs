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

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     New NumericUpDown
    /// </summary>
    public sealed class FancyNud : NumericUpDown
    {
        private readonly Color errorColor;
        private readonly Color foreColor;
        private readonly Timer timer;

        public FancyNud()
        {
            timer = new Timer();
            timer.Tick += TimerTick;

            foreColor = ForeColor;
            errorColor = Color.Red;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            ForeColor = foreColor;
            timer.Tag = Value;
            base.OnValueChanged(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            decimal value;
            if (!decimal.TryParse(Text, out value)) return;
            if (Minimum <= value && value <= Maximum)
            {
                SetValue(value);
                ForeColor = foreColor;
            }
            else
            {
                if (timer.Enabled)
                    timer.Stop();
                ForeColor = errorColor;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
                ChangeValue(Increment);
            else
                ChangeValue(-Increment);
        }

        private void SetValue(decimal value)
        {
            if (timer.Enabled)
                timer.Stop();

            timer.Tag = value;
            timer.Interval = 500;

            timer.Start();
        }

        private void ChangeValue(decimal change)
        {
            if (timer.Enabled)
                timer.Stop();

            decimal oldValue = (timer.Tag == null) ? Value : (decimal) timer.Tag;
            decimal newValue = oldValue + change;

            timer.Tag = newValue;
            if (newValue > Maximum)
                timer.Tag = Maximum;
            if (newValue < Minimum)
                timer.Tag = Minimum;

            timer.Interval = 50;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (timer.Enabled)
                timer.Stop();

            var value = (decimal) timer.Tag;
            if (value > Maximum)
                value = Maximum;
            if (value < Minimum)
                value = Minimum;

            Value = value;
        }
    }
}