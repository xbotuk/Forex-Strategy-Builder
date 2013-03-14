// FormState
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class used to preserve / restore state of the form
    /// </summary>
    public static class FormState
    {
        private static FormWindowState _winState;
        private static FormBorderStyle _brdStyle;
        private static Rectangle _bounds;
        private static bool _topMost;
        private static bool _isMaximized;

        public static void Maximize(Form targetForm)
        {
            if (_isMaximized) return;
            _isMaximized = true;
            Save(targetForm);
            targetForm.WindowState = FormWindowState.Maximized;
            targetForm.FormBorderStyle = FormBorderStyle.None;
            targetForm.TopMost = true;
            WinApi.SetWinFullScreen(targetForm.Handle);
        }

        private static void Save(Form targetForm)
        {
            _winState = targetForm.WindowState;
            _brdStyle = targetForm.FormBorderStyle;
            _bounds = targetForm.Bounds;
            _topMost = targetForm.TopMost;
        }

        public static void Restore(Form targetForm)
        {
            targetForm.WindowState = _winState;
            targetForm.FormBorderStyle = _brdStyle;
            targetForm.Bounds = _bounds;
            targetForm.TopMost = _topMost;
            _isMaximized = false;
        }
    }
}