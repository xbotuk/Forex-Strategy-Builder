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
using System.Runtime.InteropServices;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public static class FileIconExtractor
    {
        private static SHFileInfo shinfo;

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFileInfo psfi,
                                                   uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        ///     Extract the Icon from given file name
        /// </summary>
        public static Icon GetIcon(string filename)
        {
            try
            {
                const uint SHGFI_ICON = 0x100;
                //const uint SHGFI_LARGEICON = 0x0; // 'Large icon
                const uint SHGFI_SMALLICON = 0x1; // 'Small icon
                SHGetFileInfo(filename, 0, ref shinfo, (uint) Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                return Icon.FromHandle(shinfo.hIcon);
            }
            catch
            {
                // Return the default icon 
                return Resources.Icon;
            }
        }

        #region Nested type: SHFileInfo

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFileInfo
        {
            public readonly IntPtr hIcon;
            private readonly IntPtr iIcon;
            private readonly uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] private readonly string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] private readonly string szTypeName;
        };

        #endregion
    }
}