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

namespace FSB_Launcher
{
    public class Settings : BaseXmlSettings, ISettings
    {
        public string FSBPath { get; set; }
        public string Arguments { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public int ShutDownTime { get; set; }

        public void SetDefaults()
        {
            FSBPath = @"Forex Strategy Builder.exe";
            Arguments = "";
            BackColor = ColorTranslator.FromHtml("#007049");
            ForeColor = ColorTranslator.FromHtml("#E3DEEB");
            ShutDownTime = 15;
        }

        public void LoadSettings()
        {
            BaseLoadSettings(this);
        }

        public void SaveSettings()
        {
            BaseSaveSettings(this);
        }
    }
}
