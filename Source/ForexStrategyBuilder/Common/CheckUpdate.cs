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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public static class CheckUpdate
    {
        private const string UpdateFileUrl = "http://forexsb.com/products/fsb-update.xml";
        private static string pathUpdateFile;
        private static Dictionary<string, string> brokersDictionary;
        private static ToolStripMenuItem itemLiveContent;
        private static ToolStripMenuItem itemForex;
        private static BackgroundWorker bgWorker;

        private static XmlDocument xmlUpdateFile;

        /// <summary>
        ///     Checks online for update.
        /// </summary>
        public static void CheckForUpdate(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex)
        {
            itemLiveContent = miLiveContent;
            itemForex = miForex;
            pathUpdateFile = Path.Combine(pathSystem, "fsb-update.xml");

            brokersDictionary = new Dictionary<string, string>();
            xmlUpdateFile = new XmlDocument();

            LoadUpdateFile();
            ReadBrokers();
            SetBrokers();

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWork;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateTheUpdateFile();
            CheckProgramsVersionNumber();
        }

        /// <summary>
        ///     Update the config file if it is necessary
        /// </summary>
        private static void UpdateTheUpdateFile()
        {
            var url = new Uri(UpdateFileUrl);
            var webClient = new WebClient();
            try
            {
                xmlUpdateFile.LoadXml(webClient.DownloadString(url));
                SaveUpdateFile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Load update file
        /// </summary>
        private static void LoadUpdateFile()
        {
            try
            {
                if (!File.Exists(pathUpdateFile))
                {
                    xmlUpdateFile = new XmlDocument {InnerXml = Resources.fsb_update};
                }
                else
                {
                    xmlUpdateFile.Load(pathUpdateFile);
                }
            }
            catch (Exception e)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(e.Message, "Configuration");
            }
        }

        /// <summary>
        ///     Save config file
        /// </summary>
        private static void SaveUpdateFile()
        {
            try
            {
                xmlUpdateFile.Save(pathUpdateFile);
            }
            catch (Exception e)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(e.Message, "Check for Updates");
            }
        }

        /// <summary>
        ///     Checks the program version
        /// </summary>
        private static void CheckProgramsVersionNumber()
        {
            string text = "";
            try
            {
                var selectSingleNode = xmlUpdateFile.SelectSingleNode("update/versions/release");
                if (selectSingleNode != null)
                {
                    int iProgramVersion = int.Parse(selectSingleNode.InnerText);
                    if (Configs.CheckForUpdates && iProgramVersion > Data.ProgramId)
                    {
                        // A newer release version was published
                        text = Language.T("New Version");
                    }
                    else
                    {
                        var singleNode = xmlUpdateFile.SelectSingleNode("update/versions/beta");
                        if (singleNode != null)
                        {
                            int iBetaVersion = int.Parse(singleNode.InnerText);
                            if (Configs.CheckForNewBeta && iBetaVersion > Data.ProgramId)
                            {
                                // A newer beta version was published
                                text = Language.T("New Beta");
                            }
                        }
                    }
                }

                if (text != "")
                {
                    itemLiveContent.Text = text;
                    itemLiveContent.Visible = true;
                    itemLiveContent.Click += MenuLiveContentOnClick;
                }
            }
            catch (Exception e)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(e.Message, "Check for Updates");
            }
        }

        /// <summary>
        ///     Opens the Live Content browser
        /// </summary>
        private static void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/download/");
                HideMenuItemLiveContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        ///     Hides the menu news
        /// </summary>
        private static void HideMenuItemLiveContent()
        {
            itemLiveContent.Visible = false;
            itemLiveContent.Click -= MenuLiveContentOnClick;
        }

        /// <summary>
        ///     Reads the brokers
        /// </summary>
        private static void ReadBrokers()
        {
            try
            {
                XmlNodeList xmlListBrokers = xmlUpdateFile.GetElementsByTagName("broker");

                foreach (XmlNode nodeBroker in xmlListBrokers)
                {
                    var selectSingleNode = nodeBroker.SelectSingleNode("title");
                    if (selectSingleNode != null)
                    {
                        string title = selectSingleNode.InnerText;
                        var singleNode = nodeBroker.SelectSingleNode("link");
                        if (singleNode != null)
                        {
                            string link = singleNode.InnerText;

                            brokersDictionary.Add(title, link);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(e.Message, "Check for Updates");
            }
        }

        /// <summary>
        ///     Sets the brokers in the menu
        /// </summary>
        private static void SetBrokers()
        {
            foreach (var kvpBroker in brokersDictionary)
            {
                var miBroker = new ToolStripMenuItem
                    {
                        Text = kvpBroker.Key + "...",
                        Image = Resources.globe,
                        Tag = kvpBroker.Value
                    };
                miBroker.Click += MenuForexContentsOnClick;

                itemForex.DropDownItems.Add(miBroker);
            }
        }

        /// <summary>
        ///     Opens the forex news
        /// </summary>
        private static void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            try
            {
                Process.Start((string) mi.Tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}