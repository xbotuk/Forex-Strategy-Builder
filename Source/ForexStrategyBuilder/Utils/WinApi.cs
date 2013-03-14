// FormState
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Runtime.InteropServices;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Selected Win AI Function Calls
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