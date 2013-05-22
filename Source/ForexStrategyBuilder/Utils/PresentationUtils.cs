using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ForexStrategyBuilder.Presentation.Enums;

namespace ForexStrategyBuilder.Utils
{
    public class PresentationUtils
    {
        public Size GetScreenDpi()
        {
            // no error checking here - being lazy
            IntPtr dc = GetDC(IntPtr.Zero);
            try
            {
                return new Size(
                    GetDeviceCaps(dc, (int) DeviceCap.LOGPIXELSX),
                    GetDeviceCaps(dc, (int) DeviceCap.LOGPIXELSY));
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, dc);
            }
        }

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
    }
}