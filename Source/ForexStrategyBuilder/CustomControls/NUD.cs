// NUD class
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
    /// New NumericUpDown
    /// </summary>
    public sealed class NUD : NumericUpDown
    {
        private readonly Color _errorColor;
        private readonly Color _foreColor;
        private readonly Timer _timer;

        public NUD()
        {
            _timer = new Timer();
            _timer.Tick += TimerTick;

            _foreColor = ForeColor;
            _errorColor = Color.Red;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            ForeColor = _foreColor;
            _timer.Tag = Value;
            base.OnValueChanged(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            decimal value;
            if (!decimal.TryParse(Text, out value)) return;
            if (Minimum <= value && value <= Maximum)
            {
                SetValue(value);
                ForeColor = _foreColor;
            }
            else
            {
                if (_timer.Enabled)
                    _timer.Stop();
                ForeColor = _errorColor;
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
            if (_timer.Enabled)
                _timer.Stop();

            _timer.Tag = value;
            _timer.Interval = 500;

            _timer.Start();
        }

        private void ChangeValue(decimal change)
        {
            if (_timer.Enabled)
                _timer.Stop();

            decimal oldValue = (_timer.Tag == null) ? Value : (decimal) _timer.Tag;
            decimal newValue = oldValue + change;

            _timer.Tag = newValue;
            if (newValue > Maximum)
                _timer.Tag = Maximum;
            if (newValue < Minimum)
                _timer.Tag = Minimum;

            _timer.Interval = 50;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_timer.Enabled)
                _timer.Stop();

            var value = (decimal) _timer.Tag;
            if (value > Maximum)
                value = Maximum;
            if (value < Minimum)
                value = Minimum;

            Value = value;
        }
    }
}