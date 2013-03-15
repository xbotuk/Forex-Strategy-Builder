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
using System.Runtime.InteropServices;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Selected Win AI Function Calls
    /// </summary>
    public static class WinApi
    {
        private const int SmCxscreen = 0;
        private const int SmCyscreen = 1;
        private const int SwpShowwindow = 64; // 0x0040
        private static readonly IntPtr HwndTop = IntPtr.Zero;

        private static int ScreenX
        {
            get { return GetSystemMetrics(SmCxscreen); }
        }

        private static int ScreenY
        {
            get { return GetSystemMetrics(SmCyscreen); }
        }

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        private static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height,
                                                uint flags);

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HwndTop, 0, 0, ScreenX, ScreenY, SwpShowwindow);
        }
    }
}