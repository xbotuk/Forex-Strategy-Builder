using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    public static class FileIconExtractor
    {
        private static SHFileInfo _shinfo;

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFileInfo psfi, uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        /// Extract the Icon from given file name 
        /// </summary>
        public static Icon GetIcon(string filename)
        {
            try
            {
                const uint SHGFI_ICON = 0x100;
                //const uint SHGFI_LARGEICON = 0x0; // 'Large icon
                const uint SHGFI_SMALLICON = 0x1; // 'Small icon
                SHGetFileInfo(filename, 0, ref _shinfo, (uint) Marshal.SizeOf(_shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                return Icon.FromHandle(_shinfo.hIcon);
            }
            catch
            {   // Return the default icon 
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