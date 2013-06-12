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
using System.Security;
using System.Text;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Selected Win AI Function Calls
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class WinApi
    {
        private const int WM_COPYDATA = 0x004A;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int CS_CLOSE = 0xF060;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private const int SWP_SHOWWINDOW = 60; // 0x0040
        private static readonly IntPtr HWND_TOP = IntPtr.Zero;

        private static int ScreenX
        {
            get { return GetSystemMetrics(SM_CXSCREEN); }
        }

        private static int ScreenY
        {
            get { return GetSystemMetrics(SM_CYSCREEN); }
        }

        public static void SendWindowsStringMessage(int hWnd, int wParam, string msg)
        {
            if (hWnd <= 0) return;
            byte[] sarr = Encoding.Default.GetBytes(msg);
            int len = sarr.Length;
            var cds = new CopyDataStruct {DwData = (IntPtr) 100, LpData = msg, CbData = len + 1};
            SendMessage(hWnd, WM_COPYDATA, wParam, ref cds);
        }

        public static void CloseWindow(int hWnd)
        {
            SendMessage(hWnd, WM_SYSCOMMAND, CS_CLOSE, 0);
        }

        public static int GetWindowId(string className, string windowName)
        {
            return FindWindow(className, windowName);
        }

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int msg, int wParam, ref CopyDataStruct lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        private static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height,
                                                uint flags);
    }
}