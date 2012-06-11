// Actions class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.Dialogs.Generator;
using Forex_Strategy_Builder.Dialogs.Optimizer;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public sealed partial class Actions : Controls
    {
        /// <summary>
        /// The starting point of the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UpdateStatusLabel("Loading...");
            Data.Start();
            Instruments.LoadInstruments();
            Configs.LoadConfigs();
            Language.InitLanguages();
            LayoutColors.InitColorSchemes();
            Data.InitMarketStatistic();

            Data.InstrProperties = Instruments.InstrumentList[Data.Strategy.Symbol].Clone();

            Application.Run(new Actions());
        }

        /// <summary>
        /// The default constructor.
        /// </summary>
        private Actions()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            StartPosition = FormStartPosition.CenterScreen;
            Size = GetFormSize();
            MinimumSize = new Size(500, 375);
            Icon = Data.Icon;
            Text = Data.ProgramName;
            FormClosing += ActionsFormClosing;
            Application.Idle += ApplicationIdle;

            PrepareInstruments();
            PrepareCustomIndicators();
            ProvideStrategy();
            Calculate(false);
            CheckUpdate.CheckForUpdate(Data.SystemDir, MiLiveContent, MiForex);
            ShowStartingTips();
            UpdateStatusLabel("Loading user interface...");
            SetStrategyDirWatcher();
        }

        private bool IsDiscardSelectedIndexChange { get; set; }

        private void PrepareInstruments()
        {
            UpdateStatusLabel("Loading historical data...");
            if (LoadInstrument(false) != 0)
            {
                LoadInstrument(true);
                string message = Language.T("Forex Strategy Builder cannot load a historical data file and is going to use integrated data!");
                MessageBox.Show(message, Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void PrepareCustomIndicators()
        {
            if (Configs.LoadCustomIndicators)
            {
                UpdateStatusLabel("Loading custom indicators...");
                CustomIndicators.LoadCustomIndicators();
            }
            else
                IndicatorStore.CombineAllIndicators();
        }

        private void ProvideStrategy()
        {
            UpdateStatusLabel("Loading strategy...");
            string strategyPath = Data.StrategyPath;

            if (Configs.RememberLastStr && Configs.LastStrategy != "")
            {
                string lastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (lastStrategy != "")
                    lastStrategy = Configs.LastStrategy;
                else
                {
                    string sPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    lastStrategy = Path.Combine(sPath, Configs.LastStrategy);
                }
                if (File.Exists(lastStrategy))
                    strategyPath = lastStrategy;
            }

            if (OpenStrategy(strategyPath) == 0)
            {
                AfterStrategyOpening(false);
            }
        }

        private static void ShowStartingTips()
        {
            if (!Configs.ShowStartingTip) return;
            var startingTips = new StartingTips();
            if (startingTips.TipsCount > 0)
                startingTips.Show();
        }

        /// <summary>
        /// Gets the starting size of the main screen.
        /// </summary>
        private Size GetFormSize()
        {
            int width = Math.Min(Configs.MainScreenWidth, SystemInformation.MaxWindowTrackSize.Width);
            int height = Math.Min(Configs.MainScreenHeight, SystemInformation.MaxWindowTrackSize.Height);

            return new Size(width, height);
        }

        /// <summary>
        /// Application idle
        /// </summary>
        private void ApplicationIdle(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdle;
            string lockFile = GetLockFile();
            if (!string.IsNullOrEmpty(lockFile))
                File.Delete(lockFile);
        }

        /// <summary>
        /// Updates the splash screen label.
        /// </summary>
        private static void UpdateStatusLabel(string comment)
        {
            try
            {
                TextWriter tw = new StreamWriter(GetLockFile(), false);
                tw.WriteLine(comment);
                tw.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// The lockfile name will be passed automatically by Splash.exe as a
        /// command line arg -lockfile="c:\temp\C1679A85-A4FA-48a2-BF77-E74F73E08768.lock"
        /// </summary>
        /// <returns>Lock file path</returns>
        private static string GetLockFile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-lockfile="))
                    return arg.Replace("-lockfile=", String.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Checks whether the strategy have been saved or not
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            if (!e.Cancel)
            {
                // Remember the last used strategy
                if (Configs.RememberLastStr)
                {
                    if (Data.LoadedSavedStrategy != "")
                    {
                        string strategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) +
                                              Path.DirectorySeparatorChar;
                        string defaultPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                        if (strategyPath == defaultPath)
                            Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                    }
                    Configs.LastStrategy = Data.LoadedSavedStrategy;
                }

                WindowState = FormWindowState.Normal;
                Configs.MainScreenWidth = Width;
                Configs.MainScreenHeight = Height;

                Configs.SaveConfigs();
                Instruments.SaveInstruments();
#if !DEBUG
                Hide();
                Data.SendStats();
#endif
            }
        }

// ---------------------------------------------------------- //

        /// <summary>
        /// Edits the Strategy Properties Slot
        /// </summary>
        private void EditStrategyProperties()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());

            var strategyProperties = new StrategyProperties();
            strategyProperties.ShowDialog();

            if (strategyProperties.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer();

                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                BalanceChart.SetChartData();
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {
                UndoStrategy();
            }
        }

        /// <summary>
        /// Edits the Strategy Slot
        /// </summary>
        private void EditSlot(int slot)
        {
            SlotTypes slotType = Data.Strategy.Slot[slot].SlotType;
            bool isSlotExist = Data.Strategy.Slot[slot].IsDefined;
            if (isSlotExist)
                Data.StackStrategy.Push(Data.Strategy.Clone());

            var indicatorDialog = new IndicatorDialog(slot, slotType, isSlotExist);
            indicatorDialog.ShowDialog();

            if (indicatorDialog.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer();

                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                IndicatorChart.InitChart();
                IndicatorChart.Invalidate();
                RebuildStrategyLayout();
                BalanceChart.SetChartData();
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {
                // Cancel was pressed
                UndoStrategy();
            }
        }

        /// <summary>
        /// Moves a Slot Upwards
        /// </summary>
        private void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterUpwards(iSlotToMove);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Moves a Slot Downwards
        /// </summary>
        private void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterDownwards(iSlotToMove);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Duplicates a Slot
        /// </summary>
        private void DuplicateSlot(int slotToDuplicate)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.DuplicateFilter(slotToDuplicate);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Adds a new Open filter
        /// </summary>
        private void AddOpenFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddOpenFilter();
            EditSlot(Data.Strategy.OpenFilters);
        }

        /// <summary>
        /// Adds a new Close filter
        /// </summary>
        private void AddCloseFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddCloseFilter();
            EditSlot(Data.Strategy.Slots - 1);
        }

        /// <summary>
        /// Removes a strategy slot.
        /// </summary>
        /// <param name="slotNumber">Slot to remove</param>
        private void RemoveSlot(int slotNumber)
        {
            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.RemoveFilter(slotNumber);
            RebuildStrategyLayout();

            Calculate(false);
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        private void UndoStrategy()
        {
            if (Data.StackStrategy.Count <= 1)
            {
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Data.Strategy = Data.StackStrategy.Pop();

                RebuildStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Performs actions when UPBV has been changed
        /// </summary>
        private void UsePreviousBarValueChange()
        {
            if (MiStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string message = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(message, Language.T("Use previous bar value"),
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // OK, we are sure
                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Data.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {
                    // Not just now
                    MiStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                Data.AutoUsePrvBarValue = true;
                Data.Strategy.AdjustUsePreviousBarValue();
                RepaintStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Ask for saving the changed strategy
        /// </summary>
        private DialogResult WhetherSaveChangedStrategy()
        {
            DialogResult dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string message = Language.T("Do you want to save the current strategy?") + Environment.NewLine + Data.StrategyName;
                dr = MessageBox.Show(message, Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        /// LoadInstrument
        /// </summary>
        private int LoadInstrument(bool useResource)
        {
            Cursor = Cursors.WaitCursor;

            //  Takes the instrument symbol and period
            string symbol = ComboBoxSymbol.Text;
            var dataPeriod = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(ComboBoxPeriod.SelectedIndex);
            InstrumentProperties instrProperties = Instruments.InstrumentList[symbol].Clone();

            //  Makes an instance of class Instrument
            var instrument = new Instrument(instrProperties, (int) dataPeriod)
                                 {
                                     DataDir = Data.OfflineDataDir,
                                     MaxBars = Configs.MaxBars,
                                     StartTime = Configs.DataStartTime,
                                     EndTime = Configs.DataEndTime,
                                     UseStartTime = Configs.UseStartTime,
                                     UseEndTime = Configs.UseEndTime
                                 };

            // Loads the data
            int loadDataResult = useResource ? instrument.LoadResourceData() : instrument.LoadData();

            if (instrument.Bars > 0 && loadDataResult == 0)
            {
                Data.InstrProperties = instrProperties.Clone();

                Data.Bars = instrument.Bars;
                Data.Period = dataPeriod;
                Data.Update = instrument.Update;

                Data.Time = new DateTime[Data.Bars];
                Data.Open = new double[Data.Bars];
                Data.High = new double[Data.Bars];
                Data.Low = new double[Data.Bars];
                Data.Close = new double[Data.Bars];
                Data.Volume = new int[Data.Bars];

                for (int bar = 0; bar < Data.Bars; bar++)
                {
                    Data.Open[bar] = instrument.Open(bar);
                    Data.High[bar] = instrument.High(bar);
                    Data.Low[bar] = instrument.Low(bar);
                    Data.Close[bar] = instrument.Close(bar);
                    Data.Time[bar] = instrument.Time(bar);
                    Data.Volume[bar] = instrument.Volume(bar);
                }

                Data.MinPrice = instrument.MinPrice;
                Data.MaxPrice = instrument.MaxPrice;
                Data.DaysOff = instrument.DaysOff;
                Data.AverageGap = instrument.AverageGap;
                Data.MaxGap = instrument.MaxGap;
                Data.AverageHighLow = instrument.AverageHighLow;
                Data.MaxHighLow = instrument.MaxHighLow;
                Data.AverageCloseOpen = instrument.AverageCloseOpen;
                Data.MaxCloseOpen = instrument.MaxCloseOpen;
                Data.DataCut = instrument.Cut;
                Data.IsIntrabarData = false;
                Data.IsTickData = false;
                Data.IsData = true;
                Data.IsResult = false;

                // Configs.SetAccountExchangeRate();

                CheckLoadedData();
                Data.GenerateMarketStats();
                InfoPanelMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue,
                                                 Data.MarketStatsFlag, Language.T("Market Statistics"));
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else if (loadDataResult == -1)
            {
                MessageBox.Show(Language.T("Error in the data file!"), Language.T("Data file loading"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                return 1;
            }
            else
            {
                MessageBox.Show(
                    Language.T("There is no data for") + " " + symbol + " " + Data.DataPeriodToString(dataPeriod) + " " +
                    Language.T("in folder") + " " + Data.OfflineDataDir + Environment.NewLine + Environment.NewLine +
                    Language.T("Check the offline data directory path (Menu Market -> Data Directory)"),
                    Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor = Cursors.Default;
                return 1;
            }

            Cursor = Cursors.Default;

            return 0;
        }

        /// <summary>
        /// Checks the loaded data
        /// </summary>
        private void CheckLoadedData()
        {
            SetInstrumentDataStatusBar();

            if (!Configs.CheckData)
                return;

            string errorMessage = "";

            // Check for defective data
            int maxConsecutiveBars = 0;
            int maxConsecutiveBar = 0;
            int consecutiveBars = 0;
            int lastBar = 0;
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (Math.Abs(Data.Open[bar] - Data.Close[bar]) < Data.InstrProperties.Point)
                {
                    if (lastBar == bar - 1 || lastBar == 0)
                    {
                        consecutiveBars++;
                        lastBar = bar;

                        if (consecutiveBars > maxConsecutiveBars)
                        {
                            maxConsecutiveBars = consecutiveBars;
                            maxConsecutiveBar = bar;
                        }
                    }
                }
                else
                {
                    consecutiveBars = 0;
                }
            }

            if (maxConsecutiveBars > 10)
            {
                errorMessage += Language.T("Defective till bar number:") + " " + (maxConsecutiveBar + 1) + " - " +
                                Data.Time[maxConsecutiveBar].ToString(CultureInfo.InvariantCulture) +
                                Environment.NewLine +
                                Language.T("You can try to cut it using \"Data Horizon\".") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (Data.Bars < 300)
            {
                errorMessage += Language.T("Contains less than 300 bars!") + Environment.NewLine +
                                Language.T("Check your data file or the limits in \"Data Horizon\".");
            }

            if (Data.DaysOff > 5 && Data.Period != DataPeriods.week)
            {
                errorMessage += Language.T("Maximum days off") + " " + Data.DaysOff + Environment.NewLine +
                                Language.T("The data is probably incomplete!") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (errorMessage != "")
            {
                errorMessage = Language.T("Market") + " " + Data.Symbol + " " + Data.DataPeriodToString(Data.Period) +
                               Environment.NewLine + errorMessage;
                MessageBox.Show(errorMessage, Language.T("Data File Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Open a strategy file
        /// </summary>
        private void OpenFile()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            var opendlg = new OpenFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml",
                                  Title = Language.T("Open Strategy")
                              };

            if (opendlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                OpenStrategy(opendlg.FileName);
                AfterStrategyOpening(true);
                Calculate(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        /// New Strategy
        /// </summary>
        private void NewStrategy()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

            if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
            {
                AfterStrategyOpening(false);
                Calculate(false);
            }
        }

        /// <summary>
        ///Reloads the Custom Indicators.
        /// </summary>
        private void ReloadCustomIndicators()
        {
            // Check if the strategy contains custom indicators
            bool strategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                // Searching the strategy slots for a custom indicator
                if (IndicatorStore.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    strategyHasCustomIndicator = true;
                    break;
                }
            }

            if (strategyHasCustomIndicator)
            {
                // Save the current strategy
                DialogResult dialogResult = WhetherSaveChangedStrategy();

                if (dialogResult == DialogResult.Yes)
                    SaveStrategy();
                else if (dialogResult == DialogResult.Cancel)
                    return;
            }

            // Reload all the custom indicators
            CustomIndicators.LoadCustomIndicators();

            if (strategyHasCustomIndicator)
            {
                // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
                {
                    AfterStrategyOpening(false);
                    Calculate(false);
                }
            }
        }

        /// <summary>
        /// Reads the strategy from a file.
        /// </summary>
        /// <returns>0 - success.</returns>
        private int OpenStrategy(string strategyFilePath)
        {
            try
            {
                if (File.Exists(strategyFilePath) && Strategy.Load(strategyFilePath))
                {
                    // Successfully opening
                    Data.Strategy.StrategyName = Path.GetFileNameWithoutExtension(strategyFilePath);
                    Data.StrategyDir = Path.GetDirectoryName(strategyFilePath);
                    Data.StrategyName = Path.GetFileName(strategyFilePath);
                    if (Data.Strategy.OpenFilters > Configs.MaxEntryFilters)
                        Configs.MaxEntryFilters = Data.Strategy.OpenFilters;
                    if (Data.Strategy.CloseFilters > Configs.MaxExitFilters)
                        Configs.MaxExitFilters = Data.Strategy.CloseFilters;
                }
                else
                {
                    Strategy.GenerateNew();
                    Data.LoadedSavedStrategy = "";
                    Text = Data.ProgramName;
                }

                Data.SetStrategyIndicators();
                RebuildStrategyLayout();

                Text = Data.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Strategy.GenerateNew();
                string message = Language.T("The strategy could not be loaded correctly!");
                MessageBox.Show(message, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                Text = Data.ProgramName;
                RebuildStrategyLayout();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveStrategy()
        {
            if (Data.StrategyName == "New.xml")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Data.Strategy.Save(Data.StrategyPath);
                    Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.SavedStrategies++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                }
            }
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveAsStrategy()
        {
            //Creates a dialog form SaveFileDialog
            var savedlg = new SaveFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  FileName = Path.GetFileName(Data.StrategyName),
                                  AddExtension = true,
                                  Title = Language.T("Save the Strategy As"),
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml"
                              };


            if (savedlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                Data.StrategyName = Path.GetFileName(savedlg.FileName);
                Data.StrategyDir = Path.GetDirectoryName(savedlg.FileName);
                Data.Strategy.Save(savedlg.FileName);
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;
                Data.SavedStrategies++;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        /// Calculates the strategy.
        /// </summary>
        /// <param name="recalcIndicators">true - to recalculate all the indicators.</param>
        private void Calculate(bool recalcIndicators)
        {
            bool isUPBVChanged = Data.Strategy.AdjustUsePreviousBarValue();

            // Calculates the indicators by slots if it's necessary
            if (recalcIndicators)
                foreach (IndicatorSlot indSlot in Data.Strategy.Slot)
                {
                    string indicatorName = indSlot.IndicatorName;
                    SlotTypes slotType = indSlot.SlotType;
                    Indicator indicator = IndicatorStore.ConstructIndicator(indicatorName, slotType);

                    indicator.IndParam = indSlot.IndParam;

                    indicator.Calculate(slotType);

                    indSlot.IndicatorName = indicator.IndicatorName;
                    indSlot.IndParam = indicator.IndParam;
                    indSlot.Component = indicator.Component;
                    indSlot.SeparatedChart = indicator.SeparatedChart;
                    indSlot.SpecValue = indicator.SpecialValues;
                    indSlot.MinValue = indicator.SeparatedChartMinValue;
                    indSlot.MaxValue = indicator.SeparatedChartMaxValue;
                    indSlot.IsDefined = true;
                }

            // Searches the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();

            // Calculates the backtest
            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            Data.IsResult = true;
            StatsBuffer.UpdateStatsBuffer();

            if (isUPBVChanged) RebuildStrategyLayout();
            IndicatorChart.InitChart();
            IndicatorChart.Invalidate();
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
        }

        /// <summary>
        /// Sets the market according to the strategy
        /// </summary>
        private void SetMarket(string symbol, DataPeriods dataPeriod)
        {
            Data.InstrProperties = Instruments.InstrumentList[symbol].Clone();
            Data.Period = dataPeriod;

            IsDiscardSelectedIndexChange = true;
            ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(symbol);

            switch (dataPeriod)
            {
                case DataPeriods.min1:
                    ComboBoxPeriod.SelectedIndex = 0;
                    break;
                case DataPeriods.min5:
                    ComboBoxPeriod.SelectedIndex = 1;
                    break;
                case DataPeriods.min15:
                    ComboBoxPeriod.SelectedIndex = 2;
                    break;
                case DataPeriods.min30:
                    ComboBoxPeriod.SelectedIndex = 3;
                    break;
                case DataPeriods.hour1:
                    ComboBoxPeriod.SelectedIndex = 4;
                    break;
                case DataPeriods.hour4:
                    ComboBoxPeriod.SelectedIndex = 5;
                    break;
                case DataPeriods.day:
                    ComboBoxPeriod.SelectedIndex = 6;
                    break;
                case DataPeriods.week:
                    ComboBoxPeriod.SelectedIndex = 7;
                    break;
            }

            IsDiscardSelectedIndexChange = false;
        }

        /// <summary>
        /// Edit the Trading Charges
        /// </summary>
        private void EditTradingCharges()
        {
            var tradingCharges = new TradingCharges
                                     {
                                         Spread = Data.InstrProperties.Spread,
                                         SwapLong = Data.InstrProperties.SwapLong,
                                         SwapShort = Data.InstrProperties.SwapShort,
                                         Commission = Data.InstrProperties.Commission,
                                         Slippage = Data.InstrProperties.Slippage
                                     };

            tradingCharges.ShowDialog();

            if (tradingCharges.DialogResult == DialogResult.OK)
            {
                Data.InstrProperties.Spread = tradingCharges.Spread;
                Data.InstrProperties.SwapLong = tradingCharges.SwapLong;
                Data.InstrProperties.SwapShort = tradingCharges.SwapShort;
                Data.InstrProperties.Commission = tradingCharges.Commission;
                Data.InstrProperties.Slippage = tradingCharges.Slippage;

                Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();

                Calculate(false);

                SetInstrumentDataStatusBar();
            }
            else if (tradingCharges.EditInstrument)
                ShowInstrumentEditor();
        }

        /// <summary>
        /// Check the needed market conditions
        /// </summary>
        /// <param name="isMessage">To show the message or not</param>
        private void AfterStrategyOpening(bool isMessage)
        {
            if (Data.Strategy.Symbol != Data.Symbol || Data.Strategy.DataPeriod != Data.Period)
            {
                bool toReload = true;

                if (isMessage)
                {
                    DialogResult result = MessageBox.Show(
                        Language.T("The loaded strategy has been designed for a different market!") +
                        Environment.NewLine + Environment.NewLine +
                        Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod) +
                        Environment.NewLine + Environment.NewLine +
                        Language.T("Do you want to load this market data?"),
                        Data.Strategy.StrategyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    toReload = (result == DialogResult.Yes);
                }

                if (toReload)
                {
                    if (!Instruments.InstrumentList.ContainsKey(Data.Strategy.Symbol))
                    {
                        MessageBox.Show(
                            Language.T("There is no information for this market!") +
                            Environment.NewLine + Environment.NewLine +
                            Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod),
                            Data.Strategy.StrategyName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }

                    string symbol = Data.Symbol;
                    DataPeriods dataPeriod = Data.Period;

                    SetMarket(Data.Strategy.Symbol, Data.Strategy.DataPeriod);

                    if (LoadInstrument(false) == 0)
                    {
                        Calculate(true);
                        PrepareScannerCompactMode();
                    }
                    else
                    {
                        SetMarket(symbol, dataPeriod);
                    }
                }
            }
            else if (!Data.IsIntrabarData)
            {
                PrepareScannerCompactMode();
            }
        }

        /// <summary>
        /// Load intrabar data by using scanner.
        /// </summary>
        private void PrepareScannerCompactMode()
        {
            if (!Configs.Autoscan || (Data.Period == DataPeriods.min1 && !Configs.UseTickData)) return;
            ComboBoxSymbol.Enabled = false;
            ComboBoxPeriod.Enabled = false;

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += LoadIntrabarData;
            bgWorker.RunWorkerCompleted += IntrabarDataLoaded;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Starts scanner and loads intrabar data.
        /// </summary>
        private void LoadIntrabarData(object sender, DoWorkEventArgs e)
        {
            var scanner = new Scanner {CompactMode = true};
            scanner.ShowDialog();
        }

        /// <summary>
        /// The intrabar data is loaded. Refresh the program.
        /// </summary>
        private void IntrabarDataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            Calculate(true);

            ComboBoxSymbol.Enabled = true;
            ComboBoxPeriod.Enabled = true;
        }

        /// <summary>
        /// Generate BBCode for the forum
        /// </summary>
        private void PublishStrategy()
        {
            var publisher = new StrategyPublish();
            publisher.Show();
        }

        /// <summary>
        /// Shows the Account Settings dialog.
        /// </summary>
        private void ShowAccountSettings()
        {
            var accountSettings = new AccountSettings
                                      {
                                          AccountCurrency = Configs.AccountCurrency,
                                          InitialAccount = Configs.InitialAccount,
                                          Leverage = Configs.Leverage,
                                          RateToUSD = Data.InstrProperties.RateToUSD,
                                          RateToEUR = Data.InstrProperties.RateToEUR
                                      };

            accountSettings.SetParams();

            if (accountSettings.ShowDialog() != DialogResult.OK) return;
            Configs.AccountCurrency = accountSettings.AccountCurrency;
            Configs.InitialAccount = accountSettings.InitialAccount;
            Configs.Leverage = accountSettings.Leverage;
            Data.InstrProperties.RateToUSD = accountSettings.RateToUSD;
            Data.InstrProperties.RateToEUR = accountSettings.RateToEUR;

            Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();
            Calculate(false);
        }

        /// <summary>
        ///  Shows Scanner.
        /// </summary>
        private void ShowScanner()
        {
            var scanner = new Scanner();
            scanner.ShowDialog();

            StatsBuffer.UpdateStatsBuffer();

            MiStrategyAutoscan.Checked = Configs.Autoscan;

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        /// Perform intrabar scanning.
        /// </summary>
        private void Scan()
        {
            if (!Data.IsIntrabarData)
                ShowScanner();
            else
                Backtester.Scan();

            StatsBuffer.UpdateStatsBuffer();

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        /// Starts Generator.
        /// </summary>
        private void ShowGenerator()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());

            string orginalDescription = Data.Strategy.Description;

            var generator = new Generator {ParrentForm = this};
            generator.ShowDialog();

            if (generator.DialogResult == DialogResult.OK)
            {
                // We accept the generated strategy
                Data.StrategyName = Data.Strategy.StrategyName + ".xml";
                Text = Data.Strategy.StrategyName + "* - " + Data.ProgramName;

                if (generator.IsStrategyModified)
                {
                    Data.Strategy.Description = (orginalDescription != string.Empty
                                                     ? orginalDescription + Environment.NewLine + Environment.NewLine +
                                                       "-----------" + Environment.NewLine +
                                                       generator.GeneratedDescription
                                                     : generator.GeneratedDescription);
                }
                else
                {
                    Data.SetStrategyIndicators();
                    Data.Strategy.Description = generator.GeneratedDescription;
                }
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                Calculate(true);
            }
            else
            {
                // When we cancel the Generating, we return the original strategy.
                UndoStrategy();
            }

            Data.GeneratorStarts++;
        }

        /// <summary>
        ///  Starts the generator
        /// </summary>
        private void ShowOverview()
        {
            var browser = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            browser.Show();
        }

        /// <summary>
        /// Call the Optimizer
        /// </summary>
        private void ShowOptimizer()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());

            var optimizer = new Optimizer {SetParrentForm = this};
            optimizer.ShowDialog();

            if (optimizer.DialogResult == DialogResult.OK)
            {
                // We accept the optimized strategy
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RepaintStrategyLayout();
                Calculate(true);
            }
            else
            {
                // If we cancel the optimizing, we return the original strategy.
                UndoStrategy();
            }

            Data.OptimizerStarts++;
        }

        /// <summary>
        ///  Show the method Comparator
        /// </summary>
        private void ShowComparator()
        {
            // Save the original method to return it later
            InterpolationMethod interpMethodOriginal = Backtester.InterpolationMethod;

            var comparator = new Comparator();
            comparator.ShowDialog();

            // Returns the original method
            Backtester.InterpolationMethod = interpMethodOriginal;
            Calculate(false);
        }

        /// <summary>
        ///  Shows the Bar Explorer tool.
        /// </summary>
        private void ShowBarExplorer()
        {
            var barExplorer = new BarExplorer(Data.FirstBar);
            barExplorer.ShowDialog();
        }

        /// <summary>
        ///  Sets the data starting parameters.
        /// </summary>
        private void DataHorizon()
        {
            var horizon = new DataHorizon(Configs.MaxBars, Configs.DataStartTime, Configs.DataEndTime,
                                           Configs.UseStartTime, Configs.UseEndTime);
            horizon.ShowDialog();

            if (horizon.DialogResult != DialogResult.OK) return;
            Configs.MaxBars = horizon.MaxBars;
            Configs.DataStartTime = horizon.StartTime;
            Configs.DataEndTime = horizon.EndTime;
            Configs.UseStartTime = horizon.UseStartTime;
            Configs.UseEndTime = horizon.UseEndTime;

            if (LoadInstrument(false) != 0) return;
            Calculate(true);
            PrepareScannerCompactMode();
        }

        /// <summary>
        ///  Shows the Instrument Editor dialog.
        /// </summary>
        private void ShowInstrumentEditor()
        {
            var instrEditor = new InstrumentEditor();
            instrEditor.ShowDialog();

            if (instrEditor.NeedReset)
            {
                IsDiscardSelectedIndexChange = true;

                ComboBoxSymbol.Items.Clear();
                foreach (string symbol in Instruments.SymbolList)
                    ComboBoxSymbol.Items.Add(symbol);
                ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(Data.Symbol);

                IsDiscardSelectedIndexChange = false;
            }

            Data.InstrProperties = Instruments.InstrumentList[Data.InstrProperties.Symbol].Clone();
            SetInstrumentDataStatusBar();
            Calculate(false);
        }

        /// <summary>
        /// Loads a color scheme.
        /// </summary>
        private void LoadColorScheme()
        {
            string colorFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (!File.Exists(colorFile)) return;
            LayoutColors.LoadColorScheme(colorFile);

            PanelWorkspace.BackColor = LayoutColors.ColorFormBack;
            RepaintStrategyLayout(); 
            InfoPanelAccountStatistics.SetColors();
            InfoPanelMarketStatistics.SetColors();
            IndicatorChart.InitChart();
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            SetupJournal();
            PanelWorkspace.Invalidate(true);
        }

        /// <summary>
        /// Sets the Status Bar Data Label
        /// </summary>
        private void SetInstrumentDataStatusBar()
        {
            string swapUnit = "p";
            if (Data.InstrProperties.SwapType == CommissionType.money)
                swapUnit = "m";
            else if (Data.InstrProperties.SwapType == CommissionType.percents)
                swapUnit = "%";

            string commUnit = "p";
            if (Data.InstrProperties.CommissionType == CommissionType.money)
                commUnit = "m";
            else if (Data.InstrProperties.CommissionType == CommissionType.percents)
                commUnit = "%";

            StatusLabelInstrument =
                Data.Symbol + " " +
                Data.PeriodString + " (" +
                Data.InstrProperties.Spread + ", " +
                Data.InstrProperties.SwapLong.ToString("F2") + swapUnit + ", " +
                Data.InstrProperties.SwapShort.ToString("F2") + swapUnit + ", " +
                Data.InstrProperties.Commission.ToString("F2") + commUnit + ", " +
                Data.InstrProperties.Slippage + ")" +
                (Data.DataCut ? " - " + Language.T("Cut") : "") +
                (Configs.FillInDataGaps ? " - " + Language.T("No Gaps") : "") +
                (Configs.CheckData ? "" : " - " + Language.T("Unchecked"));
        }
    }
}
