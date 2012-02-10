using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    public class Utility
    {
        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        const uint SHGFI_ICON = 0x100;
        const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath,
                                   uint dwFileAttributes,
                                   ref SHFILEINFO psfi,
                                   uint cbSizeFileInfo,
                                   uint uFlags);

        static IntPtr hImgSmall;    //the handle to the system image list

        static SHFILEINFO shinfo = new SHFILEINFO();

        /// <summary>
        /// Extract the Icon from given file name 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Icon GetIcon(string filename)
        {
            try
            {
                hImgSmall = SHGetFileInfo(filename, 0, ref shinfo,
                                               (uint)Marshal.SizeOf(shinfo),
                                                SHGFI_ICON |
                                                SHGFI_SMALLICON);

                return System.Drawing.Icon.FromHandle(shinfo.hIcon);
            }
            catch
            {
                // Return the default icon 
                return Properties.Resources.Icon;
            }
        }

    }
}
