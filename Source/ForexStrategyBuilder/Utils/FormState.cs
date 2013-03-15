//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class used to preserve / restore state of the form
    /// </summary>
    public static class FormState
    {
        private static FormWindowState winState;
        private static FormBorderStyle brdStyle;
        private static Rectangle bounds;
        private static bool topMost;
        private static bool isMaximized;

        public static void Maximize(Form targetForm)
        {
            if (isMaximized) return;
            isMaximized = true;
            Save(targetForm);
            targetForm.WindowState = FormWindowState.Maximized;
            targetForm.FormBorderStyle = FormBorderStyle.None;
            targetForm.TopMost = true;
            WinApi.SetWinFullScreen(targetForm.Handle);
        }

        private static void Save(Form targetForm)
        {
            winState = targetForm.WindowState;
            brdStyle = targetForm.FormBorderStyle;
            bounds = targetForm.Bounds;
            topMost = targetForm.TopMost;
        }

        public static void Restore(Form targetForm)
        {
            targetForm.WindowState = winState;
            targetForm.FormBorderStyle = brdStyle;
            targetForm.Bounds = bounds;
            targetForm.TopMost = topMost;
            isMaximized = false;
        }
    }
}