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
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Dialogs;
using ForexStrategyBuilder.Dialogs.Analyzer;
using ForexStrategyBuilder.Dialogs.JForex;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Actions : Controls
    /// </summary>
    public sealed partial class Actions
    {
        private Benchmark benchmark;

        /// <summary>
        ///     Changes the Full Screen mode.
        /// </summary>
        protected override void MenuViewFullScreenOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            if (mi.Checked)
                FormState.Maximize(this);
            else
                FormState.Restore(this);
        }

        /// <summary>
        ///     Opens the averaging parameters dialog.
        /// </summary>
        protected override void PnlAveragingClick(object sender, EventArgs e)
        {
            EditStrategyProperties();
        }

        /// <summary>
        ///     Opens the indicator parameters dialog.
        /// </summary>
        protected override void PnlSlotMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            var tag = (int) panel.Tag;
            if (e.Button == MouseButtons.Left)
                EditSlot(tag);
        }

        /// <summary>
        ///     Strategy panel menu items clicked
        /// </summary>
        protected override void SlotContextMenuClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            var tag = (int) mi.Tag;
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
        ///     Performs actions after the button add open filter was clicked.
        /// </summary>
        protected override void BtnAddOpenFilterClick(object sender, EventArgs e)
        {
            AddOpenFilter();
        }

        /// <summary>
        ///     Performs actions after the button add close filter was clicked.
        /// </summary>
        protected override void BtnAddCloseFilterClick(object sender, EventArgs e)
        {
            AddCloseFilter();
        }

        /// <summary>
        ///     Performs actions after selecting a new ComboBox item.
        ///     Handler for: ComboBoxSymbol, ComboBoxPeriod, ComboBoxInterpolationMethod
        /// </summary>
        protected override void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsDiscardSelectedIndexChange)
                return;

            var cbx = (ToolStripComboBox) sender;

            if (cbx.Name == "ComboBoxInterpolationMethod")
            {
                Backtester.InterpolationMethod =
                    (InterpolationMethod)
                    (Enum.GetValues(typeof (InterpolationMethod)).GetValue(ComboBoxInterpolationMethod.SelectedIndex));
            }

            if (cbx.Name == "ComboBoxSymbol" || cbx.Name == "ComboBoxPeriod")
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
        ///     Whether to express account in pips or in currency
        /// </summary>
        protected override void AccountShowInMoneyOnClick(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem) sender).Name)
            {
                case "miAccountShowInMoney":
                    Configs.AccountInMoney = true;
                    MiAccountShowInMoney.Checked = true;
                    MiAccountShowInPips.Checked = false;
                    break;
                case "miAccountShowInPips":
                    Configs.AccountInMoney = false;
                    MiAccountShowInMoney.Checked = false;
                    MiAccountShowInPips.Checked = true;
                    break;
            }

            Calculate(false);
        }

        /// <summary>
        ///     Opens the account setting dialog
        /// </summary>
        protected override void MenuAccountSettingsOnClick(object sender, EventArgs e)
        {
            ShowAccountSettings();
        }

        /// <summary>
        ///     Copies the strategy to clipboard.
        /// </summary>
        protected override void MenuStrategyCopyOnClick(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = StrategyXML.CreateStrategyXmlDoc(Data.Strategy);
            Clipboard.SetText(xmlDoc.InnerXml);
        }

        /// <summary>
        ///     Pastes a strategy from clipboard.
        /// </summary>
        protected override void MenuStrategyPasteOnClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            var xmlDoc = new XmlDocument();
            var strategyXml = new StrategyXML();
            Strategy tempStrategy;

            try
            {
                xmlDoc.InnerXml = Clipboard.GetText();
                tempStrategy = strategyXml.ParseXmlStrategy(xmlDoc);
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
            if (filePath == Data.StrategyPath)
                return; // Prevents reloading of current strategy.

            if (Application.OpenForms.Count > 1)
                return; // Prevents loading a strategy when some tool is running.

            if (BalanceChart.InvokeRequired)
            {
                Invoke(new DelegateLoadDroppedStrategy(LoadDroppedStrategy), new object[] {filePath});
            }
            else
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
        }

        /// <summary>
        ///     Load a color scheme.
        /// </summary>
        protected override void MenuLoadColorOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
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
        ///     Performs actions corresponding on the menu item Load.
        /// </summary>
        protected override void MenuLoadDataOnClick(object sender, EventArgs e)
        {
            if (LoadInstrument(false) == 0)
                Calculate(true);

            PrepareScannerCompactMode();
        }

        /// <summary>
        ///     Check the data.
        /// </summary>
        protected override void MenuCheckDataOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.CheckData = toolStripMenuItem.Checked;

            CheckLoadedData();
        }

        /// <summary>
        ///     Refine the data
        /// </summary>
        protected override void MenuRefineDataOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            if (toolStripMenuItem.Name == "miCutBadData")
                Configs.CutBadData = toolStripMenuItem.Checked;

            if (toolStripMenuItem.Name == "miCutSatSunData")
                Configs.CutSatSunData = toolStripMenuItem.Checked;

            if (toolStripMenuItem.Name == "miFillDataGaps")
                Configs.FillInDataGaps = toolStripMenuItem.Checked;

            if (LoadInstrument(false) == 0)
                Calculate(true);
        }

        /// <summary>
        ///     Data Horizon
        /// </summary>
        protected override void MenuDataHorizonOnClick(object sender, EventArgs e)
        {
            DataHorizon();
        }

        /// <summary>
        ///     Data Directory
        /// </summary>
        protected override void MenuDataDirectoryOnClick(object sender, EventArgs e)
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
        ///     Autos can
        /// </summary>
        protected override void MenuStrategyAutoscanOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.Autoscan = toolStripMenuItem.Checked;
            Calculate(false);

            if (toolStripMenuItem.Checked && !Data.IsIntrabarData)
                PrepareScannerCompactMode();
        }

        /// <summary>
        ///     TradeUntilMC
        /// </summary>
        protected override void TradeUntilMCOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.TradeUntilMarginCall = toolStripMenuItem.Checked;
            Calculate(false);
        }

        /// <summary>
        ///     AdditionalStatsOnClick
        /// </summary>
        protected override void AdditionalStatsOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.AdditionalStatistics = toolStripMenuItem.Checked;
            Calculate(false);
        }

        /// <summary>
        ///     Opens the strategy settings dialogue.
        /// </summary>
        protected override void MenuStrategyAupbvOnClick(object sender, EventArgs e)
        {
            UsePreviousBarValueChange();
        }

        /// <summary>
        ///     Monitor the "Strategies" directory and automatically load new strategy files
        /// </summary>
        protected override void MenuFileDirWatch(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.StrategyDirWatch = toolStripMenuItem.Checked;
            SetStrategyDirWatcher();
        }

        /// <summary>
        ///     Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected override void MenuStrategyBBCodeOnClick(object sender, EventArgs e)
        {
            PublishStrategy();
        }

        /// <summary>
        ///     Remove the corresponding indicator slot.
        /// </summary>
        protected override void BtnRemoveSlotClick(object sender, EventArgs e)
        {
            var slot = (int) ((Panel) sender).Tag;
            RemoveSlot(slot);
        }

        /// <summary>
        ///     Forces the calculation of the strategy.
        /// </summary>
        protected override void MenuAnalysisCalculateOnClick(object sender, EventArgs e)
        {
            Calculate(true);
        }

        /// <summary>
        ///     Forces the intrabar scanning of the strategy.
        /// </summary>
        protected override void MenuQuickScanOnClick(object sender, EventArgs e)
        {
            Scan();
        }

        /// <summary>
        ///     Loads the default strategy.
        /// </summary>
        protected override void MenuStrategyNewOnClick(object sender, EventArgs e)
        {
            NewStrategy();
        }

        /// <summary>
        ///     Opens the dialog form OpenFileDialog.
        /// </summary>
        protected override void MenuFileOpenOnClick(object sender, EventArgs e)
        {
            OpenFile();
        }

        /// <summary>
        ///     Saves the strategy.
        /// </summary>
        protected override void MenuFileSaveOnClick(object sender, EventArgs e)
        {
            SaveStrategy();
        }

        /// <summary>
        ///     Opens the dialog form SaveFileDialog.
        /// </summary>
        protected override void MenuFileSaveAsOnClick(object sender, EventArgs e)
        {
            SaveAsStrategy();
        }

        /// <summary>
        ///     Undoes the strategy.
        /// </summary>
        protected override void MenuStrategyUndoOnClick(object sender, EventArgs e)
        {
            UndoStrategy();
        }

        /// <summary>
        ///     Loads the previously generated strategy
        /// </summary>
        protected override void MenuPrevHistoryOnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count <= 0 || Data.GenHistoryIndex <= 0) return;
            Data.GenHistoryIndex--;
            Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///     Loads the next generated strategy
        /// </summary>
        protected override void MenuNextHistoryOnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count <= 0 || Data.GenHistoryIndex >= Data.GeneratorHistory.Count - 1) return;
            Data.GenHistoryIndex++;
            Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///     Tools menu
        /// </summary>
        protected override void MenuToolsOnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem) sender).Name;

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
                    try
                    {
                        Process.Start(Data.SourceFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "miReloadInd":
                    Cursor = Cursors.WaitCursor;
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miCheckInd":
                    CustomIndicators.TestCustomIndicators();
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
                case "Benchmark":
                    ShowBenchmark();
                    break;
                case "miMetaTrader4Import":
                    MetaTrader4Import();
                    break;
                case "miJForexImport":
                    JForexImport();
                    break;
                case "miOandaDataImport":
                    OandaDataImport();
                    break;
                case "miTrueFxDataImport":
                    TrueFxDataImport();
                    break;
                case "tsmiOverOptimization": // Analyzer
                    ShowAnalyzer("tsmiOverOptimization");
                    break;
                case "tsmiCumulativeStrategy": // Analyzer
                    ShowAnalyzer("tsmiCumulativeStrategy");
                    break;
            }
        }

        private void ShowBenchmark()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Benchmark test will continue about 1 minute.");
            sb.AppendLine("During the test you do not have to touch FSB.");
            sb.AppendLine("Base benchmark score is 100 points.");
            sb.AppendLine();
            sb.AppendLine("Run benchmark?");

            DialogResult dialogResult = MessageBox.Show(sb.ToString(), "Benchmark", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;

            PanelWorkspace.Visible = false;

            benchmark = new Benchmark();

            var benchmarkWorker = new BackgroundWorker();
            benchmarkWorker.DoWork += BackgroundWorker_DoWork;
            benchmarkWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            benchmarkWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(5000);

            benchmark.InitializeBenchmark();
            Data.GenerateMarketStats();

            double score = benchmark.RunLoadFilesTest(5);
            var rateDataParse = (int) (100*6500.0/score);

            score = benchmark.RunStrategyTest(30);
            var rateStrategy = (int) (100*29100.0/score);

            e.Result = (100*rateStrategy + 2*rateDataParse)/102;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            benchmark.FinishBenchmark();
            AfterStrategyOpening(false);
            Calculate(true);

            PanelWorkspace.Visible = true;
            MessageBox.Show("Benchmark score: " + e.Result);
        }

        /// <summary>
        ///     Tools button
        /// </summary>
        protected override void BtnToolsOnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripButton) sender).Name;

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
        ///     Reset settings
        /// </summary>
        private void ResetSettings()
        {
            DialogResult result = MessageBox.Show(
                Language.T("Do you want to reset all settings?") + Environment.NewLine + Environment.NewLine +
                Language.T("Restart the program to activate the changes!"),
                Language.T("Reset Settings"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
                Configs.ResetParams();
        }

        /// <summary>
        ///     Menu Journal
        /// </summary>
        protected override void MenuJournalOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;

            if (toolStripMenuItem.Name == "miJournalByPosWithoutTransfers")
            {
                if (MiJournalByPosWithoutTransfers.Checked)
                {
                    MiJournalByPosWithoutTransfers.Checked = false;
                    MiJournalByPos.Checked = false;
                    MiJournalByBars.Checked = false;
                    Configs.ShowJournal = false;
                    Configs.JournalByBars = false;
                    Configs.JournalShowTransfers = false;
                }
                else
                {
                    MiJournalByPosWithoutTransfers.Checked = true;
                    MiJournalByPos.Checked = false;
                    MiJournalByBars.Checked = false;
                    Configs.ShowJournal = true;
                    Configs.JournalByBars = false;
                    Configs.JournalShowTransfers = false;
                }
            }
            if (toolStripMenuItem.Name == "miJournalByPos")
            {
                if (MiJournalByPos.Checked)
                {
                    MiJournalByPosWithoutTransfers.Checked = false;
                    MiJournalByPos.Checked = false;
                    MiJournalByBars.Checked = false;
                    Configs.ShowJournal = false;
                    Configs.JournalByBars = false;
                    Configs.JournalShowTransfers = true;
                }
                else
                {
                    MiJournalByPosWithoutTransfers.Checked = false;
                    MiJournalByPos.Checked = true;
                    MiJournalByBars.Checked = false;
                    Configs.ShowJournal = true;
                    Configs.JournalByBars = false;
                    Configs.JournalShowTransfers = true;
                }
            }
            else if (toolStripMenuItem.Name == "miJournalByBars")
            {
                if (MiJournalByBars.Checked)
                {
                    MiJournalByPosWithoutTransfers.Checked = false;
                    MiJournalByPos.Checked = false;
                    MiJournalByBars.Checked = false;
                    Configs.ShowJournal = false;
                    Configs.JournalByBars = true;
                }
                else
                {
                    MiJournalByPosWithoutTransfers.Checked = false;
                    MiJournalByPos.Checked = false;
                    MiJournalByBars.Checked = true;
                    Configs.ShowJournal = true;
                    Configs.JournalByBars = true;
                }
            }

            ResetJournal();
        }

        /// <summary>
        ///     Starts the Analyzer.
        /// </summary>
        private void ShowAnalyzer(string menuItem)
        {
            var analyzer = new Analyzer(menuItem) {SetParrentForm = this};
            analyzer.ShowDialog();
        }

        /// <summary>
        ///     Starts the Calculator.
        /// </summary>
        private void ShowCalculator()
        {
            var calculator = new Calculator();
            calculator.Show();
        }

        /// <summary>
        ///     Starts the Profit Calculator.
        /// </summary>
        private void ShowProfitCalculator()
        {
            var profitCalculator = new ProfitCalculator();
            profitCalculator.Show();
        }

        /// <summary>
        ///     Starts the Pivot Points Calculator.
        /// </summary>
        private void ShowPivotPoints()
        {
            var pivotPointsCalculator = new PivotPointsCalculator();
            pivotPointsCalculator.Show();
        }

        /// <summary>
        ///     Starts the Calculator.
        /// </summary>
        private void ShowCommandConsole()
        {
            var commandConsole = new CommandConsole();
            commandConsole.Show();
        }

        /// <summary>
        ///     Makes new language file.
        /// </summary>
        private void MakeNewTranslation()
        {
            var newTranslation = new NewTranslation();
            newTranslation.Show();
        }

        /// <summary>
        ///     Edit translation.
        /// </summary>
        private void EditTranslation()
        {
            var editTranslation = new EditTranslation();
            editTranslation.Show();
        }

        /// <summary>
        ///     Starts MetaTrader4Import.
        /// </summary>
        private void MetaTrader4Import()
        {
            var metaTrader4Import = new MetaTrader4Import();
            metaTrader4Import.ShowDialog();
        }

        /// <summary>
        ///     Starts JForexImport.
        /// </summary>
        private void JForexImport()
        {
            var jForexImport = new JForexImport();
            jForexImport.ShowDialog();
        }

        /// <summary>
        ///     Starts OandaTickDataImport.
        /// </summary>
        private void OandaDataImport()
        {
            var oandaTickDataImport = new OandaTickDataImport();
            oandaTickDataImport.ShowDialog();
        }

        /// <summary>
        ///     Starts TrueForexTickDataImport.
        /// </summary>
        private void TrueFxDataImport()
        {
            var trueFxTickDataImport = new TrueForexTickDataImport();
            trueFxTickDataImport.ShowDialog();
        }

        /// <summary>
        ///     Use logical groups menu item.
        /// </summary>
        protected override void MenuUseLogicalGroupsOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;

            if (toolStripMenuItem.Checked)
            {
                Configs.UseLogicalGroups = toolStripMenuItem.Checked;
                RebuildStrategyLayout();
                return;
            }

            // Check if the current strategy uses logical groups
            bool useGroups = false;
            var closeGroup = new List<string>();
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                if (slot.SlotType == SlotTypes.OpenFilter && slot.LogicalGroup != "A")
                    useGroups = true;

                if (slot.SlotType == SlotTypes.CloseFilter)
                {
                    if (closeGroup.Contains(slot.LogicalGroup) || slot.LogicalGroup == "all")
                        useGroups = true;
                    else
                        closeGroup.Add(slot.LogicalGroup);
                }
            }

            if (!useGroups)
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
        ///     Menu MenuOpeningLogicSlotsOnClick.
        /// </summary>
        protected override void MenuOpeningLogicSlotsOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.MaxEntryFilters = (int) toolStripMenuItem.Tag;

            foreach (ToolStripMenuItem menuItem in toolStripMenuItem.Owner.Items)
                menuItem.Checked = ((int) menuItem.Tag == Configs.MaxEntryFilters);

            RebuildStrategyLayout();
        }

        /// <summary>
        ///     Menu MenuClosingLogicSlotsOnClick.
        /// </summary>
        protected override void MenuClosingLogicSlotsOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.MaxExitFilters = (int) toolStripMenuItem.Tag;

            foreach (ToolStripMenuItem menuItem in toolStripMenuItem.Owner.Items)
                menuItem.Checked = ((int) menuItem.Tag == Configs.MaxExitFilters);

            RebuildStrategyLayout();
        }

        /// <summary>
        ///     Menu ShowPriceLineOnClick.
        /// </summary>
        protected override void ShowPriceLineOnClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            Configs.ShowPriceChartOnAccountChart = toolStripMenuItem.Checked;

            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }

        /// <summary>
        ///     Sets the program's language.
        /// </summary>
        protected override void LanguageClick(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem) sender;
            if (!toolStripMenuItem.Checked)
            {
                Configs.Language = toolStripMenuItem.Name;
                Language.InitLanguages();

                MainMenuStrip.Items.Clear();
                InitializeMenu();

                StatusBarStrip.Items.Clear();
                InitializeStatusBar();

                Calculate(false);
                RebuildStrategyLayout();
                InfoPanelMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue, Data.MarketStatsFlag,
                                                 Language.T("Market Statistics"));
                SetupJournal();
                PanelWorkspace.Invalidate(true);
                string messageText = Language.T("Restart the program to activate the changes!");
                MessageBox.Show(messageText, Language.T("Language Change"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }

            foreach (ToolStripMenuItem tsmi in toolStripMenuItem.Owner.Items)
            {
                tsmi.Checked = false;
            }
            toolStripMenuItem.Checked = true;
        }

        private delegate void DelegateLoadDroppedStrategy(string filePath);
    }
}