// MenuAndStatusBar class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Provides MainMenu and StatusBar.
    /// </summary>
    public class MenuAndStatusBar : Workspace
    {
        protected ToolStripMenuItem MiAccountShowInMoney { get; private set; }
        protected ToolStripMenuItem MiAccountShowInPips{ get; private set; }
        protected ToolStripMenuItem MiForex{ get; private set; }
        protected ToolStripMenuItem MiJournalByBars{ get; private set; }
        protected ToolStripMenuItem MiJournalByPos{ get; private set; }
        protected ToolStripMenuItem MiJournalByPosWithoutTransfers{ get; private set; }
        protected ToolStripMenuItem MiLiveContent{ get; private set; }
        protected ToolStripMenuItem MiStrategyAUPBV{ get; private set; }
        protected ToolStripMenuItem MiStrategyAutoscan{ get; private set; }

        private ToolStripStatusLabel SlInstrument { get; set; }
        private ToolStripStatusLabel SlChartInfo { get; set; }
        private ToolStripStatusLabel SlDate { get; set; }
        private ToolStripStatusLabel SlTime { get; set; }

        /// <summary>
        /// The default constructor
        /// </summary>
        protected MenuAndStatusBar()
        {
            InitializeMenu();
            InitializeStatusBar();
        }

        /// <summary>
        /// Sets the instrument info on the status bar.
        /// </summary>
        protected string StatusLabelInstrument { set { SlInstrument.Text = value; } }

        /// <summary>
        /// Sets the dynamic info for the instrument chart on the status bar
        /// </summary>
        protected string StatusLabelChartInfo { set { SlChartInfo.Text = value; } }


        /// <summary>
        /// Sets the Main Menu.
        /// </summary>
        protected void InitializeMenu()
        {
            // File
            var miFile = new ToolStripMenuItem(Language.T("File"));

            var miNew = new ToolStripMenuItem
                            {
                                Text = Language.T("New"),
                                Image = Resources.new_startegy,
                                ShortcutKeys = Keys.Control | Keys.N,
                                ToolTipText = Language.T("Open the default strategy \"New.xml\".")
                            };
            miNew.Click += MenuStrategyNew_OnClick;
            miFile.DropDownItems.Add(miNew);

            var miOpen = new ToolStripMenuItem
                             {
                                 Text = Language.T("Open..."),
                                 Image = Resources.open,
                                 ShortcutKeys = Keys.Control | Keys.O,
                                 ToolTipText = Language.T("Open a strategy.")
                             };
            miOpen.Click += MenuFileOpen_OnClick;
            miFile.DropDownItems.Add(miOpen);

            var miSave = new ToolStripMenuItem
                             {
                                 Text = Language.T("Save"),
                                 Image = Resources.save,
                                 ShortcutKeys = Keys.Control | Keys.S,
                                 ToolTipText = Language.T("Save the strategy.")
                             };
            miSave.Click += MenuFileSave_OnClick;
            miFile.DropDownItems.Add(miSave);

            var miSaveAs = new ToolStripMenuItem
                               {
                                   Text = Language.T("Save As") + "...",
                                   Image = Resources.save_as,
                                   ToolTipText = Language.T("Save a copy of the strategy.")
                               };
            miSaveAs.Click += MenuFileSaveAs_OnClick;
            miFile.DropDownItems.Add(miSaveAs);

            miFile.DropDownItems.Add(new ToolStripSeparator());

            var miClose = new ToolStripMenuItem
                              {
                                  Text = Language.T("Exit"),
                                  Image = Resources.exit,
                                  ToolTipText = Language.T("Close the program."),
                                  ShortcutKeys = Keys.Control | Keys.X
                              };
            miClose.Click += MenuFileCloseOnClick;
            miFile.DropDownItems.Add(miClose);

            // Edit
            var miEdit = new ToolStripMenuItem(Language.T("Edit"));

            var miStrategyUndo = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Undo"),
                                         Image = Resources.undo,
                                         ToolTipText = Language.T("Undo the last change in the strategy."),
                                         ShortcutKeys = Keys.Control | Keys.Z
                                     };
            miStrategyUndo.Click += MenuStrategyUndo_OnClick;
            miEdit.DropDownItems.Add(miStrategyUndo);

            miEdit.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyCopy = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Copy Strategy"),
                                         ToolTipText = Language.T("Copy the entire strategy to the clipboard."),
                                         Image = Resources.copy,
                                         ShortcutKeys = Keys.Control | Keys.C
                                     };
            miStrategyCopy.Click += MenuStrategyCopy_OnClick;
            miEdit.DropDownItems.Add(miStrategyCopy);

            var miStrategyPaste = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Paste Strategy"),
                                          ToolTipText = Language.T("Load a strategy from the clipboard."),
                                          Image = Resources.paste,
                                          ShortcutKeys = Keys.Control | Keys.V
                                      };
            miStrategyPaste.Click += MenuStrategyPaste_OnClick;
            miEdit.DropDownItems.Add(miStrategyPaste);

            miEdit.DropDownItems.Add(new ToolStripSeparator());

            var miPrevGenHistory = new ToolStripMenuItem
                                       {
                                           Text = Language.T("Previous Generated Strategy"),
                                           Image = Resources.prev_gen,
                                           ShortcutKeys = Keys.Control | Keys.H
                                       };
            miPrevGenHistory.Click += MenuPrevHistory_OnClick;
            miEdit.DropDownItems.Add(miPrevGenHistory);

            var miNextGenHistory = new ToolStripMenuItem
                                       {
                                           Text = Language.T("Next Generated Strategy"),
                                           Image = Resources.next_gen,
                                           ShortcutKeys = Keys.Control | Keys.J
                                       };
            miNextGenHistory.Click += MenuNextHistory_OnClick;
            miEdit.DropDownItems.Add(miNextGenHistory);

            //View
            var miView = new ToolStripMenuItem(Language.T("View"));

            var miLanguage = new ToolStripMenuItem {Text = "Language", Image = Resources.lang};
            for (int i = 0; i < Language.LanguageList.Length; i++)
            {
                var miLang = new ToolStripMenuItem {Text = Language.LanguageList[i], Name = Language.LanguageList[i]};
                miLang.Checked = miLang.Name == Configs.Language;
                miLang.Click += LanguageClick;
                miLanguage.DropDownItems.Add(miLang);
            }

            miView.DropDownItems.Add(miLanguage);

            var miLanguageTools = new ToolStripMenuItem {Text = Language.T("Language Tools"), Image = Resources.lang_tools};

            var miNewTranslation = new ToolStripMenuItem
                                       {
                                           Name = "miNewTranslation",
                                           Text = Language.T("Make New Translation") + "...",
                                           Image = Resources.new_translation
                                       };
            miNewTranslation.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miNewTranslation);

            var miEditTranslation = new ToolStripMenuItem
                                        {
                                            Name = "miEditTranslation",
                                            Text = Language.T("Edit Current Translation") + "...",
                                            Image = Resources.edit_translation
                                        };
            miEditTranslation.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miEditTranslation);

            miLanguageTools.DropDownItems.Add(new ToolStripSeparator());

            var miShowEnglishPhrases = new ToolStripMenuItem
                                           {
                                               Name = "miShowEnglishPhrases",
                                               Text = Language.T("Show English Phrases") + "...",
                                               Image = Resources.view_translation
                                           };
            miShowEnglishPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowEnglishPhrases);

            var miShowAltPhrases = new ToolStripMenuItem
                                       {
                                           Name = "miShowAltPhrases",
                                           Text = Language.T("Show Translated Phrases") + "...",
                                           Image = Resources.view_translation
                                       };
            miShowAltPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowAltPhrases);

            var miShowBothPhrases = new ToolStripMenuItem
                                        {
                                            Name = "miShowAllPhrases",
                                            Text = Language.T("Show All Phrases") + "...",
                                            Image = Resources.view_translation
                                        };
            miShowBothPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowBothPhrases);

            miView.DropDownItems.Add(miLanguageTools);

            miView.DropDownItems.Add(new ToolStripSeparator());

            var miShowPriceChart = new ToolStripMenuItem
                                       {
                                           Text = Language.T("Indicator Chart") + "...",
                                           ToolTipText = Language.T("Show the full Indicator Chart."),
                                           ShortcutKeys = Keys.F2,
                                           Image = Resources.bar_chart
                                       };
            miShowPriceChart.Click += ShowPriceChart_OnClick;
            miView.DropDownItems.Add(miShowPriceChart);

            var miShowAccountChart = new ToolStripMenuItem
                                         {
                                             Text = Language.T("Account Chart") + "...",
                                             ToolTipText = Language.T("Show the full Account Chart."),
                                             Image = Resources.balance_chart,
                                             ShortcutKeys = Keys.F3
                                         };
            miShowAccountChart.Click += ShowAccountChart_OnClick;
            miView.DropDownItems.Add(miShowAccountChart);

            miView.DropDownItems.Add(new ToolStripSeparator());

            MiJournalByPosWithoutTransfers = new ToolStripMenuItem
                                                 {
                                                     Name = "miJournalByPosWithoutTransfers",
                                                     Text = Language.T("Journal by Positions") + " " + Language.T("without Transfers"),
                                                     Checked = Configs.ShowJournal && !Configs.JournalByBars && !Configs.JournalShowTransfers
                                                 };
            MiJournalByPosWithoutTransfers.Click += MenuJournal_OnClick;
            miView.DropDownItems.Add(MiJournalByPosWithoutTransfers);

            MiJournalByPos = new ToolStripMenuItem
                                 {
                                     Name = "miJournalByPos",
                                     Text = Language.T("Journal by Positions"),
                                     Checked = Configs.ShowJournal && !Configs.JournalByBars && Configs.JournalShowTransfers
                                 };
            MiJournalByPos.Click += MenuJournal_OnClick;
            miView.DropDownItems.Add(MiJournalByPos);

            MiJournalByBars = new ToolStripMenuItem
                                  {
                                      Name = "miJournalByBars",
                                      Text = Language.T("Journal by Bars"),
                                      Checked = Configs.ShowJournal && Configs.JournalByBars
                                  };
            MiJournalByBars.Click += MenuJournal_OnClick;
            miView.DropDownItems.Add(MiJournalByBars);

            miView.DropDownItems.Add(new ToolStripSeparator());

            var miFullScreen = new ToolStripMenuItem
                                   {
                                       Text = Language.T("Full Screen"),
                                       Name = "miFullScreen",
                                       Checked = false,
                                       CheckOnClick = true,
                                       ShortcutKeys = Keys.Alt | Keys.Enter
                                   };
            miFullScreen.Click += MenuViewFullScreen_OnClick;
            miView.DropDownItems.Add(miFullScreen);

            var miLoadColor = new ToolStripMenuItem {Text = Language.T("Color Scheme"), Image = Resources.palette};
            for (int i = 0; i < LayoutColors.ColorSchemeList.Length; i++)
            {
                var miColor = new ToolStripMenuItem
                                  {Text = LayoutColors.ColorSchemeList[i], Name = LayoutColors.ColorSchemeList[i]};
                miColor.Checked = miColor.Name == Configs.ColorScheme;
                miColor.Click += MenuLoadColor_OnClick;
                miLoadColor.DropDownItems.Add(miColor);
            }

            miView.DropDownItems.Add(miLoadColor);

            var miGradientView = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Gradient View"),
                                         Name = "miGradientView",
                                         Checked = Configs.GradientView,
                                         CheckOnClick = true
                                     };
            miGradientView.Click += MenuGradientView_OnClick;
            miView.DropDownItems.Add(miGradientView);

            var miShowStatusBar = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Show Status Bar"),
                                          Name = "miShowStatusBar",
                                          Checked = Configs.ShowStatusBar,
                                          CheckOnClick = true
                                      };
            miShowStatusBar.Click += ShowStatusBar_OnClick;
            miView.DropDownItems.Add(miShowStatusBar);

            // Account
            var miAccount = new ToolStripMenuItem(Language.T("Account"));

            MiAccountShowInMoney = new ToolStripMenuItem
                                       {
                                           Name = "miAccountShowInMoney",
                                           Text = Language.T("Information in Currency"),
                                           ToolTipText =
                                               Language.T("Display the account and the statistics in currency."),
                                           Checked = Configs.AccountInMoney
                                       };
            MiAccountShowInMoney.Click += AccountShowInMoney_OnClick;
            miAccount.DropDownItems.Add(MiAccountShowInMoney);

            MiAccountShowInPips = new ToolStripMenuItem
                                      {
                                          Name = "miAccountShowInPips",
                                          Text = Language.T("Information in Pips"),
                                          ToolTipText = Language.T("Display the account and the statistics in pips."),
                                          Checked = !Configs.AccountInMoney
                                      };
            MiAccountShowInPips.Click += AccountShowInMoney_OnClick;
            miAccount.DropDownItems.Add(MiAccountShowInPips);

            miAccount.DropDownItems.Add(new ToolStripSeparator());

            var miAccountSettings = new ToolStripMenuItem
                                        {
                                            Text = Language.T("Account Settings") + "...",
                                            Image = Resources.account_sett,
                                            ToolTipText = Language.T("Set the account parameters.")
                                        };
            miAccountSettings.Click += MenuAccountSettings_OnClick;
            miAccount.DropDownItems.Add(miAccountSettings);

            // Market
            var miMarket = new ToolStripMenuItem(Language.T("Market"));

            var miReLoadData = new ToolStripMenuItem
                                   {
                                       Text = Language.T("Reload"),
                                       Image = Resources.reload_data,
                                       ToolTipText = Language.T("Reload the market data."),
                                       ShortcutKeys = Keys.Control | Keys.L
                                   };
            miReLoadData.Click += MenuLoadData_OnClick;
            miMarket.DropDownItems.Add(miReLoadData);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            var miCharges = new ToolStripMenuItem
                                {
                                    Name = "Charges",
                                    Text = Language.T("Charges") + "...",
                                    ToolTipText = Language.T("Spread, Swap numbers, Slippage."),
                                    Image = Resources.charges
                                };
            miCharges.Click += MenuTools_OnClick;
            miMarket.DropDownItems.Add(miCharges);

            var miDataHorizon = new ToolStripMenuItem
                                    {
                                        Text = Language.T("Data Horizon") + "...",
                                        Image = Resources.data_horizon,
                                        ToolTipText = Language.T("Limit the number of data bars and the starting date.")
                                    };
            miDataHorizon.Click += MenuDataHorizon_OnClick;
            miMarket.DropDownItems.Add(miDataHorizon);

            var miDataDirectory = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Data Directory") + "...",
                                          Image = Resources.data_directory,
                                          ToolTipText = Language.T("Change the current offline data directory.")
                                      };
            miDataDirectory.Click += MenuDataDirectory_OnClick;
            miMarket.DropDownItems.Add(miDataDirectory);

            var miInstrumentEditor = new ToolStripMenuItem
                                         {
                                             Name = "miInstrumentEditor",
                                             Text = Language.T("Edit Instruments") + "...",
                                             Image = Resources.instr_edit,
                                             ToolTipText = Language.T("Add, edit, or delete instruments.")
                                         };
            miInstrumentEditor.Click += MenuTools_OnClick;
            miMarket.DropDownItems.Add(miInstrumentEditor);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            var miCheckData = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Check the Data"),
                                      ToolTipText = Language.T("Check the data during loading."),
                                      CheckOnClick = true,
                                      Checked = Configs.CheckData
                                  };
            miCheckData.Click += MenuCheckData_OnClick;
            miMarket.DropDownItems.Add(miCheckData);

            var miCutBadData = new ToolStripMenuItem
                                   {
                                       Name = "miCutBadData",
                                       Text = Language.T("Cut Off Bad Data"),
                                       CheckOnClick = true,
                                       Checked = Configs.CutBadData
                                   };
            miCutBadData.Click += MenuRefineData_OnClick;
            miMarket.DropDownItems.Add(miCutBadData);

            var miFillDataGaps = new ToolStripMenuItem
                                     {
                                         Name = "miFillDataGaps",
                                         Text = Language.T("Fill In Data Gaps"),
                                         CheckOnClick = true,
                                         Checked = Configs.FillInDataGaps
                                     };
            miFillDataGaps.Click += MenuRefineData_OnClick;
            miMarket.DropDownItems.Add(miFillDataGaps);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            var miDownload = new ToolStripMenuItem
                                 {
                                     Text = Language.T("Download Forex Rates") + "...",
                                     Image = Resources.download_data,
                                     Tag = "http://forexsb.com/wiki/fsb/rates",
                                     ToolTipText = Language.T("Download historical data from the program's website.")
                                 };
            miDownload.Click += MenuHelpContentsOnClick;
            miMarket.DropDownItems.Add(miDownload);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            var miMetaTrader4Import = new ToolStripMenuItem
                                          {
                                              Name = "miMetaTrader4Import",
                                              Text = Language.T("Import MetaTrader 4 HST Files") + "...",
                                              Image = Resources.metatrader4
                                          };
            miMetaTrader4Import.Click += MenuTools_OnClick;
            miMarket.DropDownItems.Add(miMetaTrader4Import);

            var miJForexImport = new ToolStripMenuItem
                                     {
                                         Name = "miJForexImport",
                                         Text = Language.T("Import JForex Data Files") + "...",
                                         Image = Resources.jforex
                                     };
            miJForexImport.Click += MenuTools_OnClick;
            miMarket.DropDownItems.Add(miJForexImport);

            // Strategy
            var miStrategy = new ToolStripMenuItem(Language.T("Strategy"));

            var miStrategyOverview = new ToolStripMenuItem
                                         {
                                             Text = Language.T("Overview") + "...",
                                             Image = Resources.overview,
                                             ToolTipText = Language.T("See the strategy overview."),
                                             ShortcutKeys = Keys.F4
                                         };
            miStrategyOverview.Click += MenuStrategyOverview_OnClick;
            miStrategy.DropDownItems.Add(miStrategyOverview);

            var miCalculate = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Recalculate"),
                                      Image = Resources.recalculate,
                                      ToolTipText = Language.T("Recalculate the strategy."),
                                      ShortcutKeys = Keys.F5
                                  };
            miCalculate.Click += MenuAnalysisCalculate_OnClick;
            miStrategy.DropDownItems.Add(miCalculate);

            var miQuickScan = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Quick Scan"),
                                      ToolTipText = Language.T("Perform quick intrabar scan."),
                                      Image = Resources.fast_scan,
                                      ShortcutKeys = Keys.F6
                                  };
            miQuickScan.Click += MenuQuickScan_OnClick;
            miStrategy.DropDownItems.Add(miQuickScan);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyPublish = new ToolStripMenuItem
                                        {
                                            Text = Language.T("Publish") + "...",
                                            Image = Resources.publish_strategy,
                                            ToolTipText = Language.T("Publish the strategy in the program's forum.")
                                        };
            miStrategyPublish.Click += MenuStrategyBBcode_OnClick;
            miStrategy.DropDownItems.Add(miStrategyPublish);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miUseLogicalGroups = new ToolStripMenuItem
                                         {
                                             Text = Language.T("Use Logical Groups"),
                                             ToolTipText = Language.T("Groups add AND and OR logic interaction of the indicators."),
                                             Checked = Configs.UseLogicalGroups,
                                             CheckOnClick = true
                                         };
            miUseLogicalGroups.Click += MenuUseLogicalGroups_OnClick;
            miStrategy.DropDownItems.Add(miUseLogicalGroups);

            var miOpeningLogicConditions = new ToolStripMenuItem
                                               {
                                                   Text = Language.T("Max number of Opening Logic Conditions"),
                                                   Image = Resources.numb_gr
                                               };
            miStrategy.DropDownItems.Add(miOpeningLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                var miOpeningLogicSlots = new ToolStripMenuItem
                                              {
                                                  Text = i.ToString(CultureInfo.InvariantCulture),
                                                  Tag = i,
                                                  Checked = (Configs.MaxEntryFilters == i)
                                              };
                miOpeningLogicSlots.Click += MenuOpeningLogicSlots_OnClick;
                miOpeningLogicConditions.DropDownItems.Add(miOpeningLogicSlots);
            }

            var miClosingLogicConditions = new ToolStripMenuItem
                                               {
                                                   Text = Language.T("Max number of Closing Logic Conditions"),
                                                   Image = Resources.numb_br
                                               };
            miStrategy.DropDownItems.Add(miClosingLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                var miClosingLogicSlots = new ToolStripMenuItem
                                              {
                                                  Text = i.ToString(CultureInfo.InvariantCulture),
                                                  Tag = i,
                                                  Checked = (Configs.MaxExitFilters == i)
                                              };
                miClosingLogicSlots.Click += MenuClosingLogicSlots_OnClick;
                miClosingLogicConditions.DropDownItems.Add(miClosingLogicSlots);
            }

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyRemember = new ToolStripMenuItem
                                         {
                                             Text = Language.T("Remember the Last Strategy"),
                                             ToolTipText = Language.T("Load the last used strategy at startup."),
                                             Checked = Configs.RememberLastStr,
                                             CheckOnClick = true
                                         };
            miStrategyRemember.Click += MenuStrategyRemember_OnClick;
            miStrategy.DropDownItems.Add(miStrategyRemember);

            MiStrategyAUPBV = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Auto Control of \"Use previous bar value\""),
                                      ToolTipText = Language.T("Provides automatic setting of the indicators' check box \"Use previous bar value\"."),
                                      Checked = true,
                                      CheckOnClick = true
                                  };
            MiStrategyAUPBV.Click += MenuStrategyAUPBV_OnClick;
            miStrategy.DropDownItems.Add(MiStrategyAUPBV);

            // Export
            var miExport = new ToolStripMenuItem(Language.T("Export"));

            var miExpDataOnly = new ToolStripMenuItem
                                    {
                                        Name = "dataOnly",
                                        Image = Resources.export,
                                        Text = Language.T("Market Data") + "...",
                                        ToolTipText = Language.T("Export market data as a spreadsheet.")
                                    };
            miExpDataOnly.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpDataOnly);

            var miExpCSVData = new ToolStripMenuItem
                                   {
                                       Name = "CSVData",
                                       Image = Resources.export,
                                       Text = Language.T("Data File") + "...",
                                       ToolTipText = Language.T("Export market data as a CSV file.")
                                   };
            miExpCSVData.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpCSVData);

            var miExpIndicators = new ToolStripMenuItem
                                      {
                                          Name = "indicators",
                                          Text = Language.T("Indicators") + "...",
                                          Image = Resources.export,
                                          ToolTipText = Language.T("Export market data and indicators as a spreadsheet.")
                                      };
            miExpIndicators.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpIndicators);

            var miExpBarSummary = new ToolStripMenuItem
                                      {
                                          Name = "summary",
                                          Text = Language.T("Bar Summary") + "...",
                                          Image = Resources.export, ToolTipText = Language.T("Export the transactions summary by bars as a spreadsheet.")
                                      };
            miExpBarSummary.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpBarSummary);

            var miExpPositions = new ToolStripMenuItem
                                     {
                                         Name = "positions",
                                         Text = Language.T("Positions") + "...",
                                         ToolTipText = Language.T("Export positions in pips as a spreadsheet."),
                                         Image = Resources.export
                                     };
            miExpPositions.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpPositions);

            var miExpPositionsNoTransfer = new ToolStripMenuItem
                                               {
                                                   Name = "positionsNoTransfer",
                                                   Text = Language.T("Positions") + " " + Language.T("without Transfers") + "...",
                                                   ToolTipText = Language.T("Export positions in pips as a spreadsheet."),
                                                   Image = Resources.export
                                               };
            miExpPositionsNoTransfer.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpPositionsNoTransfer);

            var miExpMoneyPositions = new ToolStripMenuItem
                                          {
                                              Name = "positionInMoney",
                                              Text = Language.T("Positions in Currency") + "...",
                                              Image = Resources.export,
                                              ToolTipText = Language.T("Export positions in currency as a spreadsheet.")
                                          };
            miExpMoneyPositions.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpMoneyPositions);

            var miExpMoneyPositionsNoTransfer = new ToolStripMenuItem
                                                    {
                                                        Name = "positionInMoneyNoTransfer",
                                                        Text = Language.T("Positions in Currency") + " " + Language.T("without Transfers") + "...",
                                                        Image = Resources.export,
                                                        ToolTipText = Language.T("Export positions in currency as a spreadsheet.")
                                                    };
            miExpMoneyPositionsNoTransfer.Click += Export_OnClick;
            miExport.DropDownItems.Add(miExpMoneyPositionsNoTransfer);

            // Testing
            var miTesting = new ToolStripMenuItem(Language.T("Testing"));

            MiStrategyAutoscan = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Automatic Scan"),
                                         ToolTipText =
                                             Language.T("Scan the strategy using all available intrabar data.") +
                                             Environment.NewLine + Language.T("Use the scanner to load the data."),
                                         Checked = Configs.Autoscan,
                                         CheckOnClick = true
                                     };
            MiStrategyAutoscan.Click += MenuStrategyAutoscan_OnClick;
            miTesting.DropDownItems.Add(MiStrategyAutoscan);

            var miTradeUntilMC = new ToolStripMenuItem
                                     {
                                         Name = "miTradeUntilMC",
                                         Text = Language.T("Trade until a Margin Call"),
                                         Checked = Configs.TradeUntilMarginCall,
                                         CheckOnClick = true,
                                         ToolTipText =
                                             Language.T("Close an open position after a Margin Call.") +
                                             Environment.NewLine +
                                             Language.T("Do not open a new position when the Free Margin is insufficient.")
                                     };
            miTradeUntilMC.Click += TradeUntilMC_OnClick;
            miTesting.DropDownItems.Add(miTradeUntilMC);

            var miAdditionalStats = new ToolStripMenuItem
                                        {
                                            Name = "miAdditionalStats",
                                            Text = Language.T("Additional Statistics"),
                                            Checked = Configs.AdditionalStatistics,
                                            CheckOnClick = true,
                                            ToolTipText = Language.T("Show long/short balance lines on the chart and more statistics in the overview.")
                                        };
            miAdditionalStats.Click += AdditionalStats_OnClick;
            miTesting.DropDownItems.Add(miAdditionalStats);

            var miShowClosePrice = new ToolStripMenuItem
                                       {
                                           Name = "miShowClosePrice",
                                           Text = Language.T("Show Price Line on Account Chart"),
                                           Checked = Configs.ShowPriceChartOnAccountChart,
                                           CheckOnClick = true
                                       };
            miShowClosePrice.Click += ShowPriceLine_OnClick;
            miTesting.DropDownItems.Add(miShowClosePrice);

            // Analysis
            var miAnalysis = new ToolStripMenuItem(Language.T("Analysis"));

            var tsmiOverOptimization = new ToolStripMenuItem
                                           {
                                               Text = Language.T("Over-optimization Report"),
                                               Name = "tsmiOverOptimization",
                                               Image = Resources.overoptimization_chart
                                           };
            tsmiOverOptimization.Click += MenuTools_OnClick;
            miAnalysis.DropDownItems.Add(tsmiOverOptimization);

            var tsmiCumulativeStrategy = new ToolStripMenuItem
                                             {
                                                 Text = Language.T("Cumulative Strategy"),
                                                 Name = "tsmiCumulativeStrategy",
                                                 Image = Resources.cumulative_str
                                             };
            tsmiCumulativeStrategy.Click += MenuTools_OnClick;
            //miAnalysis.DropDownItems.Add(tsmiCumulativeStrategy);

            // Tools
            var miTools = new ToolStripMenuItem(Language.T("Tools"));

            var miComparator = new ToolStripMenuItem
                                   {
                                       Name = "Comparator",
                                       Text = Language.T("Comparator") + "...",
                                       ToolTipText = Language.T("Compare the interpolating methods."),
                                       Image = Resources.comparator
                                   };
            miComparator.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miComparator);

            var miScanner = new ToolStripMenuItem
                                {
                                    Name = "Scanner",
                                    Text = Language.T("Scanner") + "...",
                                    ToolTipText = Language.T("Perform a deep intrabar scan."),
                                    Image = Resources.scanner
                                };
            miScanner.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miScanner);

            var miOptimizer = new ToolStripMenuItem
                                  {
                                      Name = "Optimizer",
                                      Text = Language.T("Optimizer") + "...",
                                      ToolTipText = Language.T("Optimize the strategy parameters."),
                                      Image = Resources.optimizer
                                  };
            miOptimizer.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miOptimizer);

            var miGenerator = new ToolStripMenuItem
                                  {
                                      Name = "Generator",
                                      Text = Language.T("Generator") + "...",
                                      ToolTipText = Language.T("Generate or improve a strategy."),
                                      Image = Resources.generator
                                  };
            miGenerator.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miGenerator);

            var miBarExplorer = new ToolStripMenuItem
                                    {
                                        Name = "Bar Explorer",
                                        Text = Language.T("Bar Explorer") + "...",
                                        ToolTipText = Language.T("Show the price route inside a bar."),
                                        Image = Resources.bar_explorer
                                    };
            miBarExplorer.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miBarExplorer);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miCustomInd = new ToolStripMenuItem
                                  {
                                      Name = "CustomIndicators",
                                      Text = Language.T("Custom Indicators"),
                                      Image = Resources.custom_ind
                                  };

            var miReloadInd = new ToolStripMenuItem
                                  {
                                      Name = "miReloadInd",
                                      Text = Language.T("Reload the Custom Indicators"),
                                      Image = Resources.reload_ind,
                                      ShortcutKeys = Keys.Control | Keys.I
                                  };
            miReloadInd.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miReloadInd);

            var miCheckInd = new ToolStripMenuItem
                                 {
                                     Name = "miCheckInd",
                                     Text = Language.T("Check the Custom Indicators"),
                                     Image = Resources.check_ind
                                 };
            miCheckInd.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miCheckInd);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            var miExportAsCi = new ToolStripMenuItem
                                   {
                                       Name = "miExportAsCI",
                                       Text = Language.T("Export the Strategy as a Custom Indicator"),
                                       Image = Resources.str_export_as_ci
                                   };
            miExportAsCi.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miExportAsCi);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            var miOpenIndFolder = new ToolStripMenuItem
                                      {
                                          Name = "miOpenIndFolder",
                                          Text = Language.T("Open the Source Files Folder") + "...",
                                          Image = Resources.folder_open
                                      };
            miOpenIndFolder.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miOpenIndFolder);

            var miCustIndForum = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Custom Indicators Forum") + "...",
                                         Image = Resources.forum_icon,
                                         Tag = "http://forexsb.com/forum/forum/30/"
                                     };
            miCustIndForum.Click += MenuHelpContentsOnClick;
            miCustomInd.DropDownItems.Add(miCustIndForum);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            var miLoadCstomInd = new ToolStripMenuItem
                                     {
                                         Name = "miLoadCstomInd",
                                         Text = Language.T("Load the Custom Indicators at Startup"),
                                         Checked = Configs.LoadCustomIndicators,
                                         CheckOnClick = true
                                     };
            miLoadCstomInd.Click += LoadCustomIndicators_OnClick;
            miCustomInd.DropDownItems.Add(miLoadCstomInd);

            var miShowCstomInd = new ToolStripMenuItem
                                     {
                                         Name = "miShowCstomInd",
                                         Text = Language.T("Show the Loaded Custom Indicators"),
                                         Checked = Configs.ShowCustomIndicators,
                                         CheckOnClick = true
                                     };
            miShowCstomInd.Click += ShowCustomIndicators_OnClick;
            miCustomInd.DropDownItems.Add(miShowCstomInd);

            miTools.DropDownItems.Add(miCustomInd);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miPlaySounds = new ToolStripMenuItem
                                   {
                                       Text = Language.T("Play Sounds"),
                                       Name = "miPlaySounds",
                                       Checked = Configs.PlaySounds,
                                       CheckOnClick = true
                                   };
            miPlaySounds.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miPlaySounds);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miProfitCalculator = new ToolStripMenuItem
                                         {
                                             Name = "ProfitCalculator",
                                             Image = Resources.profit_calculator,
                                             Text = Language.T("Profit Calculator") + "..."
                                         };
            miProfitCalculator.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miProfitCalculator);

            var miPivotPoints = new ToolStripMenuItem
                                    {
                                        Name = "PivotPoints",
                                        Image = Resources.pivot_points,
                                        Text = Language.T("Pivot Points") + "..."
                                    };
            miPivotPoints.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miPivotPoints);

            var miFibonacciLevels = new ToolStripMenuItem
                                        {
                                            Name = "FibonacciLevels",
                                            Image = Resources.fibo_levels,
                                            Text = Language.T("Fibonacci Levels") + "..."
                                        };
            miFibonacciLevels.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miFibonacciLevels);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miAdditional = new ToolStripMenuItem {Text = Language.T("Additional"), Image = Resources.tools};

            miTools.DropDownItems.Add(miAdditional);

            var miCalculator = new ToolStripMenuItem
                                   {
                                       Name = "Calculator",
                                       Image = Resources.calculator,
                                       Text = Language.T("Calculator") + "...",
                                       ToolTipText = Language.T("A simple calculator."),
                                       ShortcutKeys = Keys.F12
                                   };
            miCalculator.Click += MenuTools_OnClick;
            miAdditional.DropDownItems.Add(miCalculator);

            miAdditional.DropDownItems.Add(new ToolStripSeparator());

            var miCommandConsole = new ToolStripMenuItem
                                       {
                                           Name = "CommandConsole",
                                           Text = Language.T("Command Console") + "...",
                                           Image = Resources.prompt
                                       };
            miCommandConsole.Click += MenuTools_OnClick;
            miAdditional.DropDownItems.Add(miCommandConsole);

            if (Directory.Exists(Data.AdditionalFolder))
            {
                var files = new List<string>();
                files.AddRange(Directory.GetFiles(Data.AdditionalFolder, "*.lnk"));
                if (files.Count > 0)
                {
                    files.Sort();
                    miAdditional.DropDownItems.Add(new ToolStripSeparator());
                }
                Keys key = Keys.F1;
                foreach (string file in files)
                {
                    var miAdditionalSubMenu = new ToolStripMenuItem();
                    string name = Path.GetFileNameWithoutExtension(file);
                    miAdditionalSubMenu.Name = "miAdditionalFile" + name;
                    miAdditionalSubMenu.Text = Language.T(name) + "...";
                    miAdditionalSubMenu.Image = FileIconExtractor.GetIcon(file).ToBitmap();
                    miAdditionalSubMenu.Tag = file;
                    miAdditionalSubMenu.Click += AdditionalSubMenu_OnClick;
                    miAdditional.DropDownItems.Add(miAdditionalSubMenu);
                    if (key != Keys.None)
                        miAdditionalSubMenu.ShortcutKeys = Keys.Control | Keys.Shift | key;
                    switch (key)
                    {
                        case Keys.F1:
                            key = Keys.F2;
                            break;
                        case Keys.F2:
                            key = Keys.F3;
                            break;
                        case Keys.F3:
                            key = Keys.F4;
                            break;
                        case Keys.F4:
                            key = Keys.F5;
                            break;
                        case Keys.F5:
                            key = Keys.F6;
                            break;
                        case Keys.F6:
                            key = Keys.F7;
                            break;
                        case Keys.F7:
                            key = Keys.F8;
                            break;
                        case Keys.F8:
                            key = Keys.F9;
                            break;
                        case Keys.F9:
                            key = Keys.F10;
                            break;
                        case Keys.F10:
                            key = Keys.F11;
                            break;
                        case Keys.F11:
                            key = Keys.F12;
                            break;
                        case Keys.F12:
                            key = Keys.None;
                            break;
                    }
                }
            }

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miResetConfigs = new ToolStripMenuItem
                                     {
                                         Name = "Reset settings",
                                         Text = Language.T("Reset Settings"),
                                         ToolTipText = Language.T("Reset the program settings to their default values. You will need to restart!"),
                                         Image = Resources.warning
                                     };
            miResetConfigs.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miResetConfigs);

            // Help
            var miHelp = new ToolStripMenuItem(Language.T("Help"));

            var miTipOfTheDay = new ToolStripMenuItem
                                    {
                                        Text = Language.T("Tip of the Day") + "...",
                                        ToolTipText = Language.T("Show a tip."),
                                        Image = Resources.hint,
                                        Tag = "tips"
                                    };
            miTipOfTheDay.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miTipOfTheDay);

            var miHelpOnlineHelp = new ToolStripMenuItem
                                       {
                                           Text = Language.T("Online Help") + "...",
                                           Image = Resources.help,
                                           ToolTipText = Language.T("Show the online help."),
                                           Tag = "http://forexsb.com/wiki/fsb/manual/start",
                                           ShortcutKeys = Keys.F1
                                       };
            miHelpOnlineHelp.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpOnlineHelp);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpForum = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Support Forum") + "...",
                                      Image = Resources.forum_icon,
                                      Tag = "http://forexsb.com/forum/",
                                      ToolTipText = Language.T("Show the program's forum.")
                                  };
            miHelpForum.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpForum);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpDonateNow = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Contribute") + "...",
                                          Image = Resources.contribute,
                                          ToolTipText = Language.T("Donate, Support, Advertise!"),
                                          Tag = "http://forexsb.com/wiki/contribution"
                                      };
            miHelpDonateNow.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpDonateNow);

            var miUsageStats = new ToolStripMenuItem
                                   {
                                       Text = Language.T("Send anonymous usage statistics"),
                                       Checked = Configs.SendUsageStats,
                                       CheckOnClick = true
                                   };
            miUsageStats.Click += MenuUsageStats_OnClick;
            miHelp.DropDownItems.Add(miUsageStats);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpUpdates = new ToolStripMenuItem
                                    {
                                        Text = Language.T("Check for Updates at Startup"),
                                        Checked = Configs.CheckForUpdates,
                                        CheckOnClick = true
                                    };
            miHelpUpdates.Click += MenuHelpUpdates_OnClick;
            miHelp.DropDownItems.Add(miHelpUpdates);

            var miHelpNewBeta = new ToolStripMenuItem
                                    {
                                        Text = Language.T("Check for New Beta Versions"),
                                        Checked = Configs.CheckForNewBeta,
                                        CheckOnClick = true
                                    };
            miHelpNewBeta.Click += MenuHelpNewBeta_OnClick;
            miHelp.DropDownItems.Add(miHelpNewBeta);


            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpAbout = new ToolStripMenuItem
                                  {
                                      Text = Language.T("About") + " " + Data.ProgramName + "...",
                                      ToolTipText = Language.T("Show the program information."),
                                      Image = Resources.information
                                  };
            miHelpAbout.Click += MenuHelpAboutOnClick;
            miHelp.DropDownItems.Add(miHelpAbout);

            // Forex
            MiForex = new ToolStripMenuItem(Language.T("Forex"));

            var miForexBrokers = new ToolStripMenuItem
                                     {
                                         Text = Language.T("Forex Brokers") + "...",
                                         Image = Resources.forex_brokers,
                                         Tag = "http://forexsb.com/forex-brokers/"
                                     };
            miForexBrokers.Click += MenuForexContentsOnClick;

            MiForex.DropDownItems.Add(miForexBrokers);

            // LiveContent
            MiLiveContent = new ToolStripMenuItem(Language.T("New Version"))
                                {
                                    Alignment = ToolStripItemAlignment.Right,
                                    BackColor = Color.Khaki,
                                    ForeColor = Color.DarkGreen,
                                    Visible = false
                                };

            // Forex Forum
            var miForum = new ToolStripMenuItem(Resources.forum_icon)
                              {
                                  Alignment = ToolStripItemAlignment.Right,
                                  Tag = "http://forexsb.com/forum/",
                                  ToolTipText = Language.T("Show the program's forum.")
                              };
            miForum.Click += MenuForexContentsOnClick;

            // MainMenu
            var mainMenu = new[]
                               {
                                   miFile, miEdit, miView, miAccount, miMarket, miStrategy, miExport,
                                   miTesting, miAnalysis, miTools, miHelp, MiForex, MiLiveContent, miForum
                               };
            foreach (ToolStripMenuItem item in mainMenu)
                MainMenuStrip.Items.Add(item);
            MainMenuStrip.ShowItemToolTips = true;
        }

        /// <summary>
        ///   Sets the StatusBar
        /// </summary>
        protected void InitializeStatusBar()
        {
            SlInstrument = new ToolStripStatusLabel
                                {
                                    Text = "",
                                    ToolTipText = Language.T("Symbol Period (Spread, Swap numbers, Slippage)"),
                                    BorderStyle = Border3DStyle.Raised
                                };
            StatusBarStrip.Items.Add(SlInstrument);
            StatusBarStrip.Items.Add(new ToolStripSeparator());

            SlChartInfo = new ToolStripStatusLabel
                               {
                                   Text = "",
                                   ToolTipText = Language.T("Price close"),
                                   BorderStyle = Border3DStyle.Raised,
                                   Spring = true
                               };
            StatusBarStrip.Items.Add(SlChartInfo);

#if DEBUG
            StatusBarStrip.Items.Add(new ToolStripSeparator());
            var lblDebug = new ToolStripStatusLabel {ForeColor = Color.LightCoral, Text = "[Debug]"};
            StatusBarStrip.Items.Add(lblDebug);
#endif

            StatusBarStrip.Items.Add(new ToolStripSeparator());

            SlDate = new ToolStripStatusLabel {ToolTipText = Language.T("The current date")};
            StatusBarStrip.Items.Add(SlDate);

            SlTime = new ToolStripStatusLabel {ToolTipText = Language.T("The current time")};
            StatusBarStrip.Items.Add(SlTime);

            var timer = new Timer();
            timer.Tick += TimerTick;
            timer.Interval = 1000;
            timer.Start();
        }

        /// <summary>
        ///   Updates the clock in the status bar.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            SlDate.Text = dt.ToString(Data.DF);
            SlTime.Text = dt.ToShortTimeString();
        }

        /// <summary>
        ///   Saves the current strategy
        /// </summary>
        protected virtual void MenuFileSave_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Opens the SaveAs menu
        /// </summary>
        protected virtual void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Opens a saved strategy
        /// </summary>
        protected virtual void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Closes the program
        /// </summary>
        private void MenuFileCloseOnClick(object sender, EventArgs e)
        {
            Close();
        }

        // Sets the programs language
        protected virtual void LanguageClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Gradient View Changed
        /// </summary>
        private void MenuGradientView_OnClick(object sender, EventArgs e)
        {
            Configs.GradientView = ((ToolStripMenuItem) sender).Checked;
            PanelWorkspace.Invalidate(true);
        }

        /// <summary>
        ///   Load a color scheme
        /// </summary>
        protected virtual void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Change Full Scream mode
        /// </summary>
        protected virtual void MenuViewFullScreen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Whether to express account in pips or in currency
        /// </summary>
        protected virtual void AccountShowInMoney_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Open the account setting dialog
        /// </summary>
        protected virtual void MenuAccountSettings_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Loads data
        /// </summary>
        protected virtual void MenuLoadData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Check the data
        /// </summary>
        protected virtual void MenuCheckData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Refine the data
        /// </summary>
        protected virtual void MenuRefineData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Data Horizon
        /// </summary>
        protected virtual void MenuDataHorizon_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Data Directory
        /// </summary>
        protected virtual void MenuDataDirectory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Loads the default strategy
        /// </summary>
        protected virtual void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Use logical groups menu item.
        /// </summary>
        protected virtual void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Remember the last used strategy
        /// </summary>
        private void MenuStrategyRemember_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.RememberLastStr = mi.Checked;
            if (mi.Checked) return;
            Configs.LastStrategy = "";
        }

        /// <summary>
        ///   Automatic scanning.
        /// </summary>
        protected virtual void MenuStrategyAutoscan_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Opens the strategy overview window
        /// </summary>
        private void MenuStrategyOverview_OnClick(object sender, EventArgs e)
        {
            var so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            so.Show();
        }

        /// <summary>
        ///   Undoes the strategy
        /// </summary>
        protected virtual void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Copies the strategy to clipboard.
        /// </summary>
        protected virtual void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Pastes a strategy from clipboard.
        /// </summary>
        protected virtual void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected virtual void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Forces the calculating of the strategy
        /// </summary>
        protected virtual void MenuAnalysisCalculate_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Forces the scanning of the strategy
        /// </summary>
        protected virtual void MenuQuickScan_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Opens the about window
        /// </summary>
        private void MenuHelpAboutOnClick(object sender, EventArgs e)
        {
            var abScr = new AboutScreen();
            abScr.ShowDialog();
        }

        /// <summary>
        ///   Menu Journal mode click
        /// </summary>
        protected virtual void MenuJournal_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu TradeUntilMC mode click
        /// </summary>
        protected virtual void TradeUntilMC_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu miAdditionalStats mode click
        /// </summary>
        protected virtual void AdditionalStats_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Tools menu
        /// </summary>
        protected virtual void MenuTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   AdditionalSub menu
        /// </summary>
        private void AdditionalSubMenu_OnClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem) sender;
            Process.Start(item.Tag.ToString());
        }

        /// <summary>
        ///   Show the full Price Chart
        /// </summary>
        private void ShowPriceChart_OnClick(object sender, EventArgs e)
        {
            if (!Data.IsData || !Data.IsResult) return;
            var chart = new Chart
                            {
                                BarPixels = Configs.IndicatorChartZoom,
                                ShowInfoPanel = Configs.IndicatorChartInfoPanel,
                                ShowDynInfo = Configs.IndicatorChartDynamicInfo,
                                ShowGrid = Configs.IndicatorChartGrid,
                                ShowCross = Configs.IndicatorChartCross,
                                ShowVolume = Configs.IndicatorChartVolume,
                                ShowPositionLots = Configs.IndicatorChartLots,
                                ShowOrders = Configs.IndicatorChartEntryExitPoints,
                                ShowPositionPrice = Configs.IndicatorChartCorrectedPositionPrice,
                                ShowBalanceEquity = Configs.IndicatorChartBalanceEquityChart,
                                ShowFloatingPL = Configs.IndicatorChartFloatingPLChart,
                                ShowIndicators = Configs.IndicatorChartIndicators,
                                ShowAmbiguousBars = Configs.IndicatorChartAmbiguousMark,
                                TrueCharts = Configs.IndicatorChartTrueCharts
                            };


            chart.ShowDialog();

            Configs.IndicatorChartZoom = chart.BarPixels;
            Configs.IndicatorChartInfoPanel = chart.ShowInfoPanel;
            Configs.IndicatorChartDynamicInfo = chart.ShowDynInfo;
            Configs.IndicatorChartGrid = chart.ShowGrid;
            Configs.IndicatorChartCross = chart.ShowCross;
            Configs.IndicatorChartVolume = chart.ShowVolume;
            Configs.IndicatorChartLots = chart.ShowPositionLots;
            Configs.IndicatorChartEntryExitPoints = chart.ShowOrders;
            Configs.IndicatorChartCorrectedPositionPrice = chart.ShowPositionPrice;
            Configs.IndicatorChartBalanceEquityChart = chart.ShowBalanceEquity;
            Configs.IndicatorChartFloatingPLChart = chart.ShowFloatingPL;
            Configs.IndicatorChartIndicators = chart.ShowIndicators;
            Configs.IndicatorChartAmbiguousMark = chart.ShowAmbiguousBars;
            Configs.IndicatorChartTrueCharts = chart.TrueCharts;
        }

        /// <summary>
        ///   Show the full Account Chart
        /// </summary>
        private void ShowAccountChart_OnClick(object sender, EventArgs e)
        {
            if (!Data.IsData || !Data.IsResult) return;
            var chart = new Chart
                            {
                                BarPixels = Configs.BalanceChartZoom,
                                ShowInfoPanel = Configs.BalanceChartInfoPanel,
                                ShowDynInfo = Configs.BalanceChartDynamicInfo,
                                ShowGrid = Configs.BalanceChartGrid,
                                ShowCross = Configs.BalanceChartCross,
                                ShowVolume = Configs.BalanceChartVolume,
                                ShowPositionLots = Configs.BalanceChartLots,
                                ShowOrders = Configs.BalanceChartEntryExitPoints,
                                ShowPositionPrice = Configs.BalanceChartCorrectedPositionPrice,
                                ShowBalanceEquity = Configs.BalanceChartBalanceEquityChart,
                                ShowFloatingPL = Configs.BalanceChartFloatingPLChart,
                                ShowIndicators = Configs.BalanceChartIndicators,
                                ShowAmbiguousBars = Configs.BalanceChartAmbiguousMark,
                                TrueCharts = Configs.BalanceChartTrueCharts
                            };


            chart.ShowDialog();

            Configs.BalanceChartZoom = chart.BarPixels;
            Configs.BalanceChartInfoPanel = chart.ShowInfoPanel;
            Configs.BalanceChartDynamicInfo = chart.ShowDynInfo;
            Configs.BalanceChartGrid = chart.ShowGrid;
            Configs.BalanceChartCross = chart.ShowCross;
            Configs.BalanceChartVolume = chart.ShowVolume;
            Configs.BalanceChartLots = chart.ShowPositionLots;
            Configs.BalanceChartEntryExitPoints = chart.ShowOrders;
            Configs.BalanceChartCorrectedPositionPrice = chart.ShowPositionPrice;
            Configs.BalanceChartBalanceEquityChart = chart.ShowBalanceEquity;
            Configs.BalanceChartFloatingPLChart = chart.ShowFloatingPL;
            Configs.BalanceChartIndicators = chart.ShowIndicators;
            Configs.BalanceChartAmbiguousMark = chart.ShowAmbiguousBars;
            Configs.BalanceChartTrueCharts = chart.TrueCharts;
        }

        /// <summary>
        ///   Export menu
        /// </summary>
        private void Export_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            string name = mi.Name;

            var exporter = new Exporter();

            switch (name)
            {
                case "dataOnly":
                    exporter.ExportDataOnly();
                    break;
                case "CSVData":
                    exporter.ExportCSVData();
                    break;
                case "indicators":
                    exporter.ExportIndicators();
                    break;
                case "summary":
                    exporter.ExportBarSummary();
                    break;
                case "positions":
                    exporter.ExportPositions(true);
                    break;
                case "positionsNoTransfer":
                    exporter.ExportPositions(false);
                    break;
                case "positionInMoney":
                    exporter.ExportPositionsInMoney(true);
                    break;
                case "positionInMoneyNoTransfer":
                    exporter.ExportPositionsInMoney(false);
                    break;
            }
        }

        /// <summary>
        ///   Opens the help window
        /// </summary>
        private void MenuHelpContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            if ((string) mi.Tag == "tips")
            {
                var shv = new Starting_Tips();
                shv.Show();
                return;
            }

            try
            {
                Process.Start((string) mi.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///   Opens the forex news
        /// </summary>
        private void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            try
            {
                Process.Start((string) mi.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///   Menu miHelpUpdates click
        /// </summary>
        private void MenuHelpUpdates_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.CheckForUpdates = mi.Checked;
        }

        /// <summary>
        ///   Menu miHelpNewBeta click
        /// </summary>
        private void MenuHelpNewBeta_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.CheckForNewBeta = mi.Checked;
        }

        /// <summary>
        ///   Menu UsageStatistics click
        /// </summary>
        private void MenuUsageStats_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.SendUsageStats = mi.Checked;
        }

        /// <summary>
        ///   Menu LoadCustomIndicators click
        /// </summary>
        private void LoadCustomIndicators_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.LoadCustomIndicators = mi.Checked;
        }

        /// <summary>
        ///   Menu ShowCustomIndicators click
        /// </summary>
        private void ShowCustomIndicators_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.ShowCustomIndicators = mi.Checked;
        }

        /// <summary>
        ///   Menu Shows or hides the status bar.
        /// </summary>
        private void ShowStatusBar_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.ShowStatusBar = mi.Checked;

            StatusBarStrip.Visible = Configs.ShowStatusBar;
            OnResize(new EventArgs());
        }

        /// <summary>
        ///   Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu MenuPrevHistory_OnClick
        /// </summary>
        protected virtual void MenuPrevHistory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu MenuNextHistory_OnClick
        /// </summary>
        protected virtual void MenuNextHistory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   Menu ShowPriceLine_OnClick
        /// </summary>
        protected virtual void ShowPriceLine_OnClick(object sender, EventArgs e)
        {
        }
    }
}