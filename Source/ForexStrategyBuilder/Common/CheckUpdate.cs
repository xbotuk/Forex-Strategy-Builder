// Forex Strategy Builder - CheckUpdate class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Builder
{
    public static class CheckUpdate
    {
        private const string UpdateFileUrl = "http://forexsb.com/products/fsb-update.xml";
        static string _pathUpdateFile;
        static Dictionary<string, string> _brokersDictionary;
        static ToolStripMenuItem _miLiveContent;
        static ToolStripMenuItem _miForex;
        static BackgroundWorker  _bgWorker;

        static XmlDocument _xmlUpdateFile;

        /// <summary>
        /// Checks online for update.
        /// </summary>
        public static void CheckForUpdate(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex)
        {
            _miLiveContent  = miLiveContent;
            _miForex        = miForex;
            _pathUpdateFile = Path.Combine(pathSystem, "fsb-update.xml");

            _brokersDictionary = new Dictionary<string, string>();
            _xmlUpdateFile     = new XmlDocument();

            LoadUpdateFile();
            ReadBrokers();
            SetBrokers();

            // BackGroundWorker
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += DoWork;
            _bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        static void DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateTheUpdateFile();
            CheckProgramsVersionNumber();
        }

        /// <summary>
        /// Update the config file if it is necessary
        /// </summary>
        static void UpdateTheUpdateFile()
        {
            var url = new Uri(UpdateFileUrl);
            var webClient = new WebClient();
            try
            {
                _xmlUpdateFile.LoadXml(webClient.DownloadString(url));
                SaveUpdateFile();
            }
            catch { }
        }

        /// <summary>
        /// Load config file
        /// </summary>
        static void LoadUpdateFile()
        {
            try
            {
                if (!File.Exists(_pathUpdateFile))
                {
                    _xmlUpdateFile = new XmlDocument {InnerXml = Properties.Resources.fsb_update};
                }
                else
                {
                    _xmlUpdateFile.Load(_pathUpdateFile);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Config");
            }
        }

        /// <summary>
        /// Save config file
        /// </summary>
        static void SaveUpdateFile()
        {
            try
            {
                _xmlUpdateFile.Save(_pathUpdateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }
        }

        /// <summary>
        /// Checks the program version
        /// </summary>
        static void CheckProgramsVersionNumber()
        {
            string text = "";
            try
            {
                int iProgramVersion = int.Parse(_xmlUpdateFile.SelectSingleNode("update/versions/release").InnerText);
                if (Configs.CheckForUpdates && iProgramVersion > Data.ProgramID)
                {   // A newer release version was published
                    text = Language.T("New Version");
                }
                else
                {
                    int iBetaVersion = int.Parse(_xmlUpdateFile.SelectSingleNode("update/versions/beta").InnerText);
                    if (Configs.CheckForNewBeta && iBetaVersion > Data.ProgramID)
                    {   // A newer beta version was published
                        text = Language.T("New Beta");
                    }
                }

                if (text != "")
                {
                    _miLiveContent.Text    = text;
                    _miLiveContent.Visible = true;
                    _miLiveContent.Click  += MenuLiveContentOnClick;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }

        }

        /// <summary>
        /// Opens the Live Content browser
        /// </summary>
        static void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/download/");
                HideMenuItemLiveContent();
            }
            catch { }
        }

        /// <summary>
        /// Hides the menu news
        /// </summary>
        static void HideMenuItemLiveContent()
        {
            _miLiveContent.Visible = false;
            _miLiveContent.Click  -= MenuLiveContentOnClick;
        }

        /// <summary>
        /// Reads the brokers
        /// </summary>
        static void ReadBrokers()
        {
            try
            {
                XmlNodeList xmlListBrokers = _xmlUpdateFile.GetElementsByTagName("broker");

                foreach (XmlNode nodeBroker in xmlListBrokers)
                {
                    string title = nodeBroker.SelectSingleNode("title").InnerText;
                    string link  = nodeBroker.SelectSingleNode("link").InnerText;

                    _brokersDictionary.Add(title, link);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        static void SetBrokers()
        {
            foreach (KeyValuePair<string, string> kvpBroker in _brokersDictionary)
            {
                var miBroker = new ToolStripMenuItem
                                   {
                                       Text = kvpBroker.Key + "...",
                                       Image = Properties.Resources.globe,
                                       Tag = kvpBroker.Value
                                   };
                miBroker.Click += MenuForexContentsOnClick;

                _miForex.DropDownItems.Add(miBroker);
            }
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        static void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch { }
        }
    }
}
