// Actions OnClick
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder
// Website http://forexsb.com
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions
    {
        /// <summary>
        /// Changes the Full Screen mode.
        /// </summary>
        protected override void MenuViewFullScreen_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            if (mi.Checked)
                FormState.Maximize(this);
            else
                FormState.Restore(this);
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected override void PnlAveraging_Click(object sender, EventArgs e)
        {
            EditStrategyProperties();
        }

        /// <summary>
        /// Opens the indicator parameters dialog.
        /// </summary>
        protected override void PnlSlot_MouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel)sender;
            var tag = (int) panel.Tag;
            if (e.Button == MouseButtons.Left)
                EditSlot(tag);
        }

        /// <summary>
        /// Strategy panel menu items clicked
        /// </summary>
        protected override void SlotContextMenu_Click(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            var tag = (int)mi.Tag;
            switch (mi.Name)
            {
                case "Edit":
                    EditSlot(tag);
                    break;
                case "Upwards":
                    MoveSlotUpwards(tag);
                    break;
                case "Downwards":
                    MoveSlotDownwards(tag);
                    break;
                case "Duplicate":
                    DuplicateSlot(tag);
                    break;
                case "Delete":
                    RemoveSlot(tag);
                    break;
            }
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected override void BtnAddOpenFilter_Click(object sender, EventArgs e)
        {
            AddOpenFilter();
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected override void BtnAddCloseFilter_Click(object sender, EventArgs e)
        {
            AddCloseFilter();
        }

        /// <summary>
        /// Performs actions after selecting a new ComboBox item.
        /// Handler for: cbxMode, cbxSymbol, cbxPeriod, tscbInterpolationMethod
        /// </summary>
        protected override void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isDiscardSelectedIndexChange)
                return;

            var cbx = (ToolStripComboBox)sender;

            if (cbx.Name == "tscbMode")
            {
                Configs.UseTickData = cbx.SelectedIndex == 1;
            }

            if (cbx.Name == "tscbInterpolationMethod")
            {
                Backtester.InterpolationMethod = (InterpolationMethod)(Enum.GetValues(typeof(InterpolationMethod)).GetValue(ComboBoxInterpolationMethod.SelectedIndex));
            }

            if (cbx.Name == "tscbSymbol" || cbx.Name == "tscbPeriod")
            {
                if (LoadInstrument(false) == 0)
                {
                    Calculate(true);
                    PrepareScannerCompactMode();
                }
                else
                {
                    SetMarket(Data.Symbol, Data.Period);
                }
            }
            else
            {
                Calculate(false);
            }
        }

        /// <summary>
        /// Whether to express account in pips or in currency
        /// </summary>
        protected override void AccountShowInMoney_OnClick(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem)sender).Name)
            {
                case "miAccountShowInMoney":
                    Configs.AccountInMoney       = true;
                    miAccountShowInMoney.Checked = true;
                    miAccountShowInPips.Checked  = false;
                    break;
                case "miAccountShowInPips":
                    Configs.AccountInMoney       = false;
                    miAccountShowInMoney.Checked = false;
                    miAccountShowInPips.Checked  = true;
                    break;
            }

            Calculate(false);
        }

        /// <summary>
        /// Opens the account setting dialog
        /// </summary>
        protected override void MenuAccountSettings_OnClick(object sender, EventArgs e)
        {
            ShowAccountSettings();
        }

        /// <summary>
        /// Copies the strategy to clipboard.
        /// </summary>
        protected override void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
            var strategyXML = new Strategy_XML();
            var xmlDoc = strategyXML.CreateStrategyXmlDoc(Data.Strategy);
            Clipboard.SetText(xmlDoc.InnerXml);
        }

        /// <summary>
        /// Pastes a strategy from clipboard.
        /// </summary>
        protected override void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            var xmlDoc = new System.Xml.XmlDocument();
            var strategyXML = new Strategy_XML();
            Strategy tempStrategy;

            try
            {
                xmlDoc.InnerXml = Clipboard.GetText();
                tempStrategy = strategyXML.ParseXmlStrategy(xmlDoc);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            Data.Strategy = tempStrategy;
            Data.StrategyName = tempStrategy.StrategyName;

            Data.SetStrategyIndicators();
            RebuildStrategyLayout();

            Text = Data.Strategy.StrategyName + " - " + Data.ProgramName;
            Data.IsStrategyChanged = false;
            Data.LoadedSavedStrategy = Data.StrategyPath;
            Data.StackStrategy.Clear();

            AfterStrategyOpening(false);
            Calculate(false);
        }

        protected override void LoadDroppedStrategy(string filePath)
        {
            try
            {
                OpenStrategy(filePath);
                AfterStrategyOpening(true);
                Calculate(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        /// Load a color scheme.
        /// </summary>
        protected override void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            if (!toolStripMenuItem.Checked)
            {
                Configs.ColorScheme = toolStripMenuItem.Name;
            }
            foreach (ToolStripMenuItem menuItem in toolStripMenuItem.Owner.Items)
            {
                menuItem.Checked = false;
            }
            toolStripMenuItem.Checked = true;

            LoadColorScheme();
        }

        /// <summary>
        /// Performs actions corresponding on the menu item Load.
        /// </summary>
        protected override void MenuLoadData_OnClick(object sender, EventArgs e)
        {
            if (LoadInstrument(false) == 0)
                Calculate(true);

            PrepareScannerCompactMode();
        }

        /// <summary>
        /// Check the data.
        /// </summary>
        protected override void MenuCheckData_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.CheckData = toolStripMenuItem.Checked;

            CheckLoadedData();
        }

        /// <summary>
        /// Refine the data
        /// </summary>
        protected override void MenuRefineData_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            if (toolStripMenuItem.Name == "miCutBadData")
                Configs.CutBadData = toolStripMenuItem.Checked;

            if (toolStripMenuItem.Name == "miFillDataGaps")
                Configs.FillInDataGaps = toolStripMenuItem.Checked;

            if (LoadInstrument(false) == 0)
                Calculate(true);
        }

        /// <summary>
        /// Data Horizon
        /// </summary>
        protected override void MenuDataHorizon_OnClick(object sender, EventArgs e)
        {
            DataHorizon();
        }

        /// <summary>
        /// Data Directory
        /// </summary>
        protected override void MenuDataDirectory_OnClick(object sender, EventArgs e)
        {
            var dataDirectory = new DataDirectory();

            if (dataDirectory.ShowDialog() == DialogResult.OK)
            {
                string dataDirPath = dataDirectory.DataFolder;

                if (dataDirPath == "")
                    Data.OfflineDataDir = Data.DefaultOfflineDataDir;
                else if (dataDirPath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
                    Data.OfflineDataDir = dataDirPath;
                else
                    Data.OfflineDataDir = dataDirPath + Path.DirectorySeparatorChar;

                if (LoadInstrument(false) == 0)
                {
                    Calculate(true);

                    // The new folder will be saved in the config file only when
                    // the data are loaded successfully
                    Configs.DataDirectory = Data.OfflineDataDir == Data.DefaultOfflineDataDir ? "" : Data.OfflineDataDir;

                    PrepareScannerCompactMode();
                }
            }
        }

        /// <summary>
        /// Opens the strategy settings dialog.
        /// </summary>
        protected override void MenuStrategySettings_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValueChange();
        }

        /// <summary>
        /// Autos can
        /// </summary>
        protected override void MenuStrategyAutoscan_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.Autoscan = toolStripMenuItem.Checked;
            Calculate(false);

            if (toolStripMenuItem.Checked && !Data.IsIntrabarData)
                PrepareScannerCompactMode();
        }

        /// <summary>
        /// TradeUntillMC
        /// </summary>
        protected override void TradeUntilMC_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.TradeUntilMarginCall = toolStripMenuItem.Checked;
            Calculate(false);
        }

        /// <summary>
        /// AdditionalStats_OnClick
        /// </summary>
        protected override void AdditionalStats_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.AdditionalStatistics = toolStripMenuItem.Checked;
            Calculate(false);
        }

        /// <summary>
        /// Opens the strategy settings dialogue.
        /// </summary>
        protected override void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValueChange();
        }

        /// <summary>
        /// Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected override void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
            PublishStrategy();
        }

        /// <summary>
        /// Remove the corresponding indicator slot.
        /// </summary>
        protected override void BtnRemoveSlot_Click(object sender, EventArgs e)
        {
            var slot = (int)((Button)sender).Tag;
            RemoveSlot(slot);
        }

        /// <summary>
        /// Forces the calculation of the strategy.
        /// </summary>
        protected override void MenuAnalysisCalculate_OnClick(object sender, EventArgs e)
        {
            Calculate(true);
        }

        /// <summary>
        /// Forces the intrabar scanning of the strategy.
        /// </summary>
        protected override void MenuQuickScan_OnClick(object sender, EventArgs e)
        {
            Scan();
        }

        /// <summary>
        /// Performs a detailed back-test.
        /// </summary>
        protected override void MenuDetailedBacktest_OnClick(object sender, EventArgs e)
        {
            ShowScanner();
        }

        /// <summary>
        /// Loads the default strategy.
        /// </summary>
        protected override void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
            NewStrategy();
        }

        /// <summary>
        /// Opens the dialog form OpenFileDialog.
        /// </summary>
        protected override void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
            OpenFile();
        }

        /// <summary>
        /// Saves the strategy.
        /// </summary>
        protected override void MenuFileSave_OnClick(object sender, EventArgs e)
        {
            SaveStrategy();
        }

        /// <summary>
        /// Opens the dialog form SaveFileDialog.
        /// </summary>
        protected override void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
            SaveAsStrategy();
        }

        /// <summary>
        /// Undoes the strategy.
        /// </summary>
        protected override void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
            UndoStrategy();
        }

        /// <summary>
        /// Loads the previously generated strategy
        /// </summary>
        protected override void MenuPrevHistory_OnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count > 0 && Data.GenHistoryIndex > 0)
            {
                Data.GenHistoryIndex--;
                Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
                RebuildStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Loads the next generated strategy
        /// </summary>
        protected override void MenuNextHistory_OnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count > 0 && Data.GenHistoryIndex < Data.GeneratorHistory.Count - 1)
            {
                Data.GenHistoryIndex++;
                Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
                RebuildStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Name;

            switch (name)
            {
                case "Comparator":
                    ShowComparator();
                    break;
                case "Scanner":
                    ShowScanner();
                    break;
                case "Generator":
                    ShowGenerator();
                    break;
                case "Optimizer":
                    ShowOptimizer();
                    break;
                case "Bar Explorer":
                    ShowBarExplorer();
                    break;
                case "ProfitCalculator":
                    ShowProfitCalculator();
                    break;
                case "PivotPoints":
                    ShowPivotPoints();
                    break;
                case "FibonacciLevels":
                    ShowFibonacciLevels();
                    break;
                case "Charges":
                    EditTradingCharges();
                    break;
                case "miInstrumentEditor":
                    ShowInstrumentEditor();
                    break;
                case "Reset settings":
                    ResetSettings();
                    break;
                case "miNewTranslation":
                    MakeNewTranslation();
                    break;
                case "miEditTranslation":
                    EditTranslation();
                    break;
                case "miShowEnglishPhrases":
                    Language.ShowPhrases(1);
                    break;
                case "miShowAltPhrases":
                    Language.ShowPhrases(2);
                    break;
                case "miShowAllPhrases":
                    Language.ShowPhrases(3);
                    break;
                case "miOpenIndFolder":
                    try { System.Diagnostics.Process.Start(Data.SourceFolder); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    break;
                case "miReloadInd":
                    Cursor = Cursors.WaitCursor;
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miExportAsCI":
                    Cursor = Cursors.WaitCursor;
                    Strategy_to_Indicator.ExportStrategyToIndicator();
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miCheckInd":
                    Custom_Indicators.TestCustomIndicators();
                    break;
                case "Calculator":
                    ShowCalculator();
                    break;
                case "miPlaySounds":
                    Configs.PlaySounds = !Configs.PlaySounds;
                    break;
                case "CommandConsole":
                    ShowCommandConsole();
                    break;
                case "miMetaTrader4Import":
                    MetaTrader4Import();
                    break;
                case "miJForexImport":
                    JForexImport();
                    break;
                case "tsmiOverOptimization": // Analyzer
                    ShowAnalyzer("tsmiOverOptimization");
                    break;
                case "tsmiCumulativeStrategy": // Analyzer
                    ShowAnalyzer("tsmiCumulativeStrategy");
                    break;
            }
        }

        /// <summary>
        /// Tools button
        /// </summary>
        protected override void BtnTools_OnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripButton)sender).Name;

            switch (name)
            {
                case "Analyzer":
                    ShowAnalyzer("tsmiOverOptimization");
                    break;
                case "Overview":
                    ShowOverview();
                    break;
                case "Comparator":
                    ShowComparator();
                    break;
                case "Scanner":
                    ShowScanner();
                    break;
                case "Generator":
                    ShowGenerator();
                    break;
                case "Optimizer":
                    ShowOptimizer();
                    break;
                case "Charges":
                    EditTradingCharges();
                    break;
            }
        }

        /// <summary>
        /// Reset settings
        /// </summary>
        void ResetSettings()
        {
            DialogResult result = MessageBox.Show(
                Language.T("Do you want to reset all settings?") + Environment.NewLine + Environment.NewLine +
                Language.T("Restart the program to activate the changes!"),
                Language.T("Reset Settings"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
                Configs.ResetParams();
        }

        /// <summary>
        /// Menu Journal
        /// </summary>
        protected override void MenuJournal_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;

            if (toolStripMenuItem.Name == "miJournalByPosWithoutTransfers")
            {
                if (miJournalByPosWithoutTransfers.Checked)
                {
                    miJournalByPosWithoutTransfers.Checked = false;
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = false;
                    Configs.JournalByBars   = false;
                    Configs.JournalShowTransfers = false;
                }
                else
                {
                    miJournalByPosWithoutTransfers.Checked = true;
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = true;
                    Configs.JournalByBars   = false;
                    Configs.JournalShowTransfers = false;
               }
            }
            if (toolStripMenuItem.Name == "miJournalByPos")
            {
                if (miJournalByPos.Checked)
                {
                    miJournalByPosWithoutTransfers.Checked = false;
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = false;
                    Configs.JournalByBars   = false;
                    Configs.JournalShowTransfers = true;
                }
                else
                {
                    miJournalByPosWithoutTransfers.Checked = false;
                    miJournalByPos.Checked  = true;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = true;
                    Configs.JournalByBars   = false;
                    Configs.JournalShowTransfers = true;
                }
            }
            else if (toolStripMenuItem.Name == "miJournalByBars")
            {
                if (miJournalByBars.Checked)
                {
                    miJournalByPosWithoutTransfers.Checked = false;
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = false;
                    Configs.JournalByBars   = true;
                }
                else
                {
                    miJournalByPosWithoutTransfers.Checked = false;
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = true;
                    Configs.ShowJournal     = true;
                    Configs.JournalByBars   = true;
                }
            }

            ResetJournal();
        }

        /// <summary>
        /// Starts the Analyzer.
        /// </summary>
        void ShowAnalyzer(string menuItem)
        {
            var analyzer = new Dialogs.Analyzer.Analyzer(menuItem) {SetParrentForm = this};
            analyzer.ShowDialog();
        }

        /// <summary>
        /// Starts the Calculator.
        /// </summary>
        void ShowCalculator()
        {
            var calculator = new Calculator();
            calculator.Show();
        }

        /// <summary>
        /// Starts the Profit Calculator.
        /// </summary>
        void ShowProfitCalculator()
        {
            var profitCalculator = new Profit_Calculator();
            profitCalculator.Show();
        }

        /// <summary>
        /// Starts the Pivot Points Calculator.
        /// </summary>
        void ShowPivotPoints()
        {
            var pivotPointsCalculator = new Pivot_Points_Calculator();
            pivotPointsCalculator.Show();
        }

        /// <summary>
        /// Starts the Fibonacci Levels Calculator.
        /// </summary>
        void ShowFibonacciLevels()
        {
            var fibonacciLevelsCalculator = new Fibonacci_Levels_Calculator();
            fibonacciLevelsCalculator.Show();
        }

        /// <summary>
        /// Starts the Calculator.
        /// </summary>
        void ShowCommandConsole()
        {
            var commandConsole = new Command_Console();
            commandConsole.Show();
        }

        /// <summary>
        /// Makes new language file.
        /// </summary>
        void MakeNewTranslation()
        {
            var newTranslation = new New_Translation();
            newTranslation.Show();
        }

        /// <summary>
        /// Edit translation.
        /// </summary>
        void EditTranslation()
        {
            var editTranslation = new Edit_Translation();
            editTranslation.Show();
        }

        /// <summary>
        /// Starts MetaTrader4Import.
        /// </summary>
        void MetaTrader4Import()
        {
            var metaTrader4Import = new MetaTrader4_Import();
            metaTrader4Import.ShowDialog();
        }

        /// <summary>
        /// Starts JForexImport.
        /// </summary>
        void JForexImport()
        {
            var jForexImport = new JForex_Import();
            jForexImport.ShowDialog();
        }

        /// <summary>
        /// Use logical groups menu item.
        /// </summary>
        protected override void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;

            if (toolStripMenuItem.Checked)
            {
                Configs.UseLogicalGroups = toolStripMenuItem.Checked;
                RebuildStrategyLayout();
                return;
            }

            // Check if the current strategy uses logical groups
            bool usefroup = false;
            var closegroup = new List<string>();
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                if (slot.SlotType == SlotTypes.OpenFilter && slot.LogicalGroup != "A")
                    usefroup = true;

                if (slot.SlotType == SlotTypes.CloseFilter)
                {
                    if (closegroup.Contains(slot.LogicalGroup) || slot.LogicalGroup == "all")
                        usefroup = true;
                    else
                        closegroup.Add(slot.LogicalGroup);
                }
            }

            if (!usefroup)
            {
                Configs.UseLogicalGroups = false;
                RebuildStrategyLayout();
            }
            else
            {
                MessageBox.Show(
                    Language.T("The strategy requires logical groups.") + Environment.NewLine +
                    Language.T("\"Use Logical Groups\" option cannot be switched off."),
                    Language.T("Logical Groups"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                toolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// Menu MenuOpeningLogicSlots_OnClick.
        /// </summary>
        protected override void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.MAX_ENTRY_FILTERS = (int)toolStripMenuItem.Tag;

            foreach (ToolStripMenuItem m in toolStripMenuItem.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_ENTRY_FILTERS);

            RebuildStrategyLayout();
        }

        /// <summary>
        /// Menu MenuClosingLogicSlots_OnClick.
        /// </summary>
        protected override void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.MAX_EXIT_FILTERS = (int)toolStripMenuItem.Tag;

            foreach (ToolStripMenuItem m in toolStripMenuItem.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_EXIT_FILTERS);

            RebuildStrategyLayout();
        }

        /// <summary>
        /// Menu ShowPriceLine_OnClick.
        /// </summary>
        protected override void ShowPriceLine_OnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            Configs.ShowPriceChartOnAccountChart = toolStripMenuItem.Checked;

            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }

        /// <summary>
        /// Sets the program's language.
        /// </summary>
        protected override void Language_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            if (!toolStripMenuItem.Checked)
            {
                Configs.Language = toolStripMenuItem.Name;
                Language.InitLanguages();

                MainMenuStrip.Items.Clear();
                InitializeMenu();

                statusStrip.Items.Clear();
                InitializeStatusBar();

                Calculate(false);
                RebuildStrategyLayout();
                infpnlMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue, Data.MarketStatsFlag, Language.T("Market Statistics"));
                SetupJournal();
                pnlWorkspace.Invalidate(true);
                string messageText = Language.T("Restart the program to activate the changes!");
                MessageBox.Show(messageText, Language.T("Language Change"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            foreach (ToolStripMenuItem tsmi in toolStripMenuItem.Owner.Items)
            {
                tsmi.Checked = false;
            }
            toolStripMenuItem.Checked = true;
        }
    }
}
