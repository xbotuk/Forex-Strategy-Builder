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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Actions : Controls
    /// </summary>
    public sealed class Browser : Form
    {
        private readonly string webPage;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Browser(string caption, string webPage)
        {
            Text = caption;
            ResizeRedraw = true;
            BackColor = SystemColors.GradientInactiveCaption;

            WbBrowser = new WebBrowser {Parent = this, Dock = DockStyle.Fill};

            this.webPage = webPage;

            // Create MenuStrip
            MainMenu = new MenuStrip {Parent = this, Dock = DockStyle.Top};

            MainMenuStrip = MainMenu;
            MainMenu.Items.Add(FileMenu());
            MainMenu.Items.Add(HelpMenu());
        }

        private MenuStrip MainMenu { get; set; }
        private WebBrowser WbBrowser { get; set; }
        private ToolStripMenuItem ItemForum { get; set; }
        private ToolStripMenuItem ItemOnlineHelp { get; set; }
        private ToolStripMenuItem ItemPreview { get; set; }
        private ToolStripMenuItem ItemPrint { get; set; }
        private ToolStripMenuItem ItemProps { get; set; }
        private ToolStripMenuItem ItemSaveAs { get; set; }

        /// <summary>
        ///     Overrides the base method OnLoad.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            Icon = Data.Icon;
            Size = new Size(660, 450);
            ShowDocument();
        }

        /// <summary>
        ///     Loads the help page.
        /// </summary>
        private void ShowDocument()
        {
            WbBrowser.DocumentText = webPage;
        }

        private ToolStripMenuItem FileMenu()
        {
            var itemFile = new ToolStripMenuItem(Language.T("File"));

            ItemSaveAs = new ToolStripMenuItem(Language.T("Save As") + "...");
            ItemSaveAs.Click += SaveAsOnClick;
            itemFile.DropDownItems.Add(ItemSaveAs);

            var item = new ToolStripMenuItem(Language.T("Page Setup") + "...");
            item.Click += PageSetupOnClick;
            itemFile.DropDownItems.Add(item);

            ItemPrint = new ToolStripMenuItem(Language.T("Print") + "...") {ShortcutKeys = Keys.Control | Keys.P};
            ItemPrint.Click += PrintDialogOnClick;
            itemFile.DropDownItems.Add(ItemPrint);

            ItemPreview = new ToolStripMenuItem(Language.T("Print Preview") + "...");
            ItemPreview.Click += PreviewOnClick;
            itemFile.DropDownItems.Add(ItemPreview);

            itemFile.DropDownItems.Add(new ToolStripSeparator());

            ItemProps = new ToolStripMenuItem(Language.T("Properties") + "...");
            ItemProps.Click += PropertiesOnClick;
            itemFile.DropDownItems.Add(ItemProps);

            itemFile.DropDownItems.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem(Language.T("Exit"));
            item.Click += ExitOnClick;
            itemFile.DropDownItems.Add(item);

            return itemFile;
        }

        private ToolStripMenuItem HelpMenu()
        {
            var itemHelp = new ToolStripMenuItem(Language.T("Help"));

            ItemOnlineHelp = new ToolStripMenuItem(Language.T("Online Help") + "...") {ShortcutKeys = Keys.F1};
            ItemOnlineHelp.Click += OnlineHelpOnClick;
            itemHelp.DropDownItems.Add(ItemOnlineHelp);

            ItemForum = new ToolStripMenuItem(Language.T("Forum") + "...");
            ItemForum.Click += ForumOnClick;
            itemHelp.DropDownItems.Add(ItemForum);

            return itemHelp;
        }

        private void SaveAsOnClick(object objSrc, EventArgs args)
        {
            try
            {
                string tempFile = Path.GetTempFileName();
                string dirName = Path.GetDirectoryName(tempFile);
                if (dirName == null) return;
                tempFile = Path.Combine(dirName, Path.GetFileNameWithoutExtension(tempFile) + ".html");
                StreamWriter writer = File.CreateText(tempFile);
                writer.Write(WbBrowser.DocumentText);
                writer.Flush();
                writer.Close();
                WbBrowser.Navigate(tempFile);
                WbBrowser.ShowSaveAsDialog();
                WbBrowser.DocumentText = webPage;
                File.Delete(tempFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void PageSetupOnClick(object objSrc, EventArgs args)
        {
            WbBrowser.ShowPageSetupDialog();
        }

        private void PrintDialogOnClick(object objSrc, EventArgs args)
        {
            WbBrowser.ShowPrintDialog();
        }

        private void PreviewOnClick(object objSrc, EventArgs args)
        {
            WbBrowser.ShowPrintPreviewDialog();
        }

        private void PropertiesOnClick(object objSrc, EventArgs args)
        {
            WbBrowser.ShowPropertiesDialog();
        }

        private void ExitOnClick(object objSrc, EventArgs args)
        {
            Close();
        }

        private void OnlineHelpOnClick(object objSrc, EventArgs args)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/start");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ForumOnClick(object objSrc, EventArgs args)
        {
            try
            {
                Process.Start("http://forexsb.com/forum/");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}