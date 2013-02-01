// Strategy Generator - Math
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        private readonly List<string> _entryFilterIndicators = new List<string>();
        private readonly List<string> _entryIndicators = new List<string>();
        private readonly List<string> _exitFilterIndicators = new List<string>();
        private readonly List<string> _exitIndicators = new List<string>();
        private readonly List<string> _exitIndicatorsWithFilters = new List<string>();
        private readonly List<string> _indicatorBlackList;
        private IndicatorSlot[] _aLockedEntryFilter; // Holds all locked entry filters.
        private IndicatorSlot[] _aLockedExitFilter; // Holds all locked exit filters.
        private int _barOOS = Data.Bars - 1;
        private int _bestBalance;
        private int _cycles;
        private bool _isEntryLocked; // Shows if the entry logic is locked
        private bool _isExitLocked; // Shows if the exit logic is locked
        private bool _isGenerating;
        private bool _isOOS;
        private bool _isStartegyChanged;
        private int _lockedEntryFilters;
        private IndicatorSlot _lockedEntrySlot; // Holds a locked entry slot.
        private int _lockedExitFilters;
        private IndicatorSlot _lockedExitSlot; // Holds a locked exit slot.
        private int _maxClosingLogicSlots;
        private int _maxOpeningLogicSlots;
        private int _minutes;
        private int _progressPercent;
        private Strategy _strategyBest;

        // Out of Sample
        private float _targetBalanceRatio = 1;

        /// <summary>
        /// BtnGenerate_Click
        /// </summary>
        private void BtnGenerateClick(object sender, EventArgs e)
        {
            if (_isGenerating)
            {
                // Cancel the asynchronous operation
                BgWorker.CancelAsync();
            }
            else
            {
                // Start the bgWorker
                PrepareStrategyForGenerating();
                CheckForLockedSlots();
                PrepareIndicatorLists();
                bool isEnoughIndicators = CheckAvailableIndicators();

                if (_isEntryLocked && _isExitLocked || !isEnoughIndicators)
                {
                    SystemSounds.Hand.Play();
                    return;
                }

                Cursor = Cursors.WaitCursor;

                _minutes = (int) NudWorkingMinutes.Value;
                ProgressBar.Style = _minutes > 0 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;

                GeneratedDescription = String.Empty;

                foreach (Control control in PnlCommon.Controls)
                    control.Enabled = false;
                foreach (Control control in PnlLimitations.Controls)
                    control.Enabled = false;
                foreach (Control control in PnlSettings.Controls)
                    control.Enabled = false;

                IndicatorsField.BlockIndicatorChange();

                TsbtLockAll.Enabled = false;
                TsbtUnlockAll.Enabled = false;
                TsbtLinkAll.Enabled = false;
                TsbtOverview.Enabled = false;
                TsbtStrategyInfo.Enabled = false;

                LblCalcStrInfo.Enabled = true;
                LblCalcStrNumb.Enabled = true;
                ChbHideFsb.Enabled = true;

                BtnAccept.Enabled = false;
                BtnCancel.Enabled = false;
                BtnGenerate.Text = Language.T("Stop");

                _isGenerating = true;

                ProgressBar.Value = 1;
                _progressPercent = 0;
                _cycles = 0;

                if (ChbGenerateNewStrategy.Checked)
                    Top10Field.ClearTop10Slots();

                BgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event
            var worker = sender as BackgroundWorker;

            // Generate a strategy
            Generating(worker, e);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            RestoreFromBest();

            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();

            StrategyField.Enabled = true;
            RebuildStrategyLayout(_strategyBest);

            _isGenerating = false;

            BtnAccept.Enabled = true;
            BtnCancel.Enabled = true;

            foreach (Control control in PnlCommon.Controls)
                control.Enabled = true;
            foreach (Control control in PnlLimitations.Controls)
                control.Enabled = true;
            foreach (Control control in PnlSettings.Controls)
                control.Enabled = true;

            IndicatorsField.UnBlockIndicatorChange();

            TsbtLockAll.Enabled = true;
            TsbtUnlockAll.Enabled = true;
            TsbtLinkAll.Enabled = true;
            TsbtOverview.Enabled = true;
            TsbtStrategyInfo.Enabled = true;

            BtnGenerate.Text = Language.T("Generate");
            ProgressBar.Style = ProgressBarStyle.Blocks;

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Prepare the strategy for generating
        /// </summary>
        private void PrepareStrategyForGenerating()
        {
            _lockedEntrySlot = null;
            _lockedEntryFilters = 0;
            _aLockedEntryFilter = new IndicatorSlot[Math.Max(Strategy.MaxOpenFilters, _strategyBest.OpenFilters)];
            _lockedExitSlot = null;
            _lockedExitFilters = 0;
            _aLockedExitFilter = new IndicatorSlot[Math.Max(Strategy.MaxCloseFilters, _strategyBest.CloseFilters)];

            // Copy the locked slots
            for (int slot = 0; slot < _strategyBest.Slots; slot++)
            {
                if (_strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked ||
                    _strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                {
                    if (_strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                        _lockedEntrySlot = _strategyBest.Slot[slot];
                    else if (_strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    {
                        _aLockedEntryFilter[_lockedEntryFilters] = _strategyBest.Slot[slot];
                        _lockedEntryFilters++;
                    }
                    else if (_strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                        _lockedExitSlot = _strategyBest.Slot[slot];
                    else if (_strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    {
                        _aLockedExitFilter[_lockedExitFilters] = _strategyBest.Slot[slot];
                        _lockedExitFilters++;
                    }
                }
            }

            if (ChbGenerateNewStrategy.Checked)
                _bestBalance = 0;
            else
                _bestBalance = (_isOOS ? Backtester.Balance(_barOOS) : Backtester.NetBalance);

            _maxOpeningLogicSlots = ChbMaxOpeningLogicSlots.Checked
                                       ? (int) NUDMaxOpeningLogicSlots.Value
                                       : Strategy.MaxOpenFilters;
            _maxClosingLogicSlots = ChbMaxClosingLogicSlots.Checked
                                       ? (int) NUDMaxClosingLogicSlots.Value
                                       : Strategy.MaxCloseFilters;
        }

        /// <summary>
        /// Check if all slots are locked.
        /// </summary>
        private void CheckForLockedSlots()
        {
            _isEntryLocked = false;
            _isExitLocked = false;

            if (_lockedEntrySlot != null && _lockedEntryFilters >= _maxOpeningLogicSlots)
                _isEntryLocked = true;

            if (_lockedEntryFilters > _maxOpeningLogicSlots)
                _maxOpeningLogicSlots = _lockedEntryFilters;

            if (_lockedExitSlot != null && !IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(_lockedExitSlot.IndicatorName))
                _isExitLocked = true;
            else if (_lockedExitSlot != null && IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(_lockedExitSlot.IndicatorName) && _lockedExitFilters >= _maxClosingLogicSlots)
                _isExitLocked = true;
            else if (_lockedExitSlot == null && _lockedExitFilters > 0 && _lockedExitFilters >= _maxClosingLogicSlots)
                _isExitLocked = true;

            if (_lockedExitFilters > _maxClosingLogicSlots)
                _maxClosingLogicSlots = _lockedExitFilters;

            for (int slot = 0; slot < _strategyBest.Slots; slot++)
            {
                if (_strategyBest.Slot[slot].SlotStatus != StrategySlotStatus.Linked) continue;
                if (_strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                    _isEntryLocked = _isEntryLocked ? !IsSlotHasParameters(_strategyBest.Slot[slot]) : _isEntryLocked;
                else if (_strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    _isEntryLocked = _isEntryLocked ? !IsSlotHasParameters(_strategyBest.Slot[slot]) : _isEntryLocked;
                else if (_strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                    _isExitLocked = _isExitLocked ? !IsSlotHasParameters(_strategyBest.Slot[slot]) : _isExitLocked;
                else if (_strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    _isExitLocked = _isExitLocked ? !IsSlotHasParameters(_strategyBest.Slot[slot]) : _isExitLocked;
            }
        }

        /// <summary>
        /// Shows if the slot has any parameters to generate.
        /// </summary>
        private bool IsSlotHasParameters(IndicatorSlot slot)
        {
            foreach (ListParam listParam in slot.IndParam.ListParam)
                if (listParam.Enabled && listParam.ItemList.Length > 1)
                    return true;
            foreach (NumericParam numericParam in slot.IndParam.NumParam)
                if (numericParam.Enabled)
                    return true;

            return false;
        }

        /// <summary>
        /// Prepare available indicators for each slot.
        /// </summary>
        private void PrepareIndicatorLists()
        {
            // Clear lists
            _entryIndicators.Clear();
            _entryFilterIndicators.Clear();
            _exitIndicators.Clear();
            _exitIndicatorsWithFilters.Clear();
            _exitFilterIndicators.Clear();

            // Copy all no banned indicators
            foreach (string indicator in IndicatorStore.OpenPointIndicators)
                if (!IndicatorsField.IsIndicatorBanned(SlotTypes.Open, indicator))
                    _entryIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.OpenFilterIndicators)
                if (!IndicatorsField.IsIndicatorBanned(SlotTypes.OpenFilter, indicator))
                    _entryFilterIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.ClosePointIndicators)
                if (!IndicatorsField.IsIndicatorBanned(SlotTypes.Close, indicator))
                    _exitIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.ClosingIndicatorsWithClosingFilters)
                if (!IndicatorsField.IsIndicatorBanned(SlotTypes.Close, indicator))
                    _exitIndicatorsWithFilters.Add(indicator);
            foreach (string indicator in IndicatorStore.CloseFilterIndicators)
                if (!IndicatorsField.IsIndicatorBanned(SlotTypes.CloseFilter, indicator))
                    _exitFilterIndicators.Add(indicator);

            // Remove special cases
            bool isPeriodDayOrWeek = Data.Period == DataPeriods.day || Data.Period == DataPeriods.week;

            if (_entryIndicators.Contains("Fibonacci"))
                _entryIndicators.Remove("Fibonacci");
            if (_entryIndicators.Contains("Day Opening") && isPeriodDayOrWeek)
                _entryIndicators.Remove("Day Opening");
            if (_entryIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                _entryIndicators.Remove("Hourly High Low");
            if (_entryIndicators.Contains("Entry Hour") && isPeriodDayOrWeek)
                _entryIndicators.Remove("Entry Hour");

            if (_entryFilterIndicators.Contains("Random Filter"))
                _entryFilterIndicators.Remove("Random Filter");
            if (_entryFilterIndicators.Contains("Data Bars Filter"))
                _entryFilterIndicators.Remove("Data Bars Filter");
            if (_entryFilterIndicators.Contains("Date Filter"))
                _entryFilterIndicators.Remove("Date Filter");
            if (_entryFilterIndicators.Contains("Long or Short"))
                _entryFilterIndicators.Remove("Long or Short");
            if (_entryFilterIndicators.Contains("Entry Time"))
                _entryFilterIndicators.Remove("Entry Time");
            if (_entryFilterIndicators.Contains("Lot Limiter"))
                _entryFilterIndicators.Remove("Lot Limiter");
            if (_entryFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                _entryFilterIndicators.Remove("Hourly High Low");

            if (_exitIndicators.Contains("Day Closing") && isPeriodDayOrWeek)
                _exitIndicators.Remove("Day Closing");
            if (_exitIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                _exitIndicators.Remove("Hourly High Low");
            if (_exitIndicators.Contains("Exit Hour") && isPeriodDayOrWeek)
                _exitIndicators.Remove("Exit Hour");
            if (_exitIndicators.Contains("Close and Reverse") &&
                _strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                _exitIndicators.Remove("Close and Reverse");

            if (_exitIndicatorsWithFilters.Contains("Day Closing") && isPeriodDayOrWeek)
                _exitIndicatorsWithFilters.Remove("Day Closing");
            if (_exitIndicatorsWithFilters.Contains("Close and Reverse") &&
                _strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                _exitIndicatorsWithFilters.Remove("Close and Reverse");

            if (_exitFilterIndicators.Contains("Random Filter"))
                _exitFilterIndicators.Remove("Random Filter");
            if (_exitFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                _exitFilterIndicators.Remove("Hourly High Low");
        }

        /// <summary>
        /// Checks if enough indicators are allowed
        /// </summary>
        private bool CheckAvailableIndicators()
        {
            if (!_isEntryLocked && _entryIndicators.Count == 0)
                return false;
            if (_entryFilterIndicators.Count < _maxOpeningLogicSlots - _lockedEntryFilters)
                return false;
            if (!_isExitLocked && _exitIndicators.Count == 0)
                return false;
            if (!_isExitLocked && _exitIndicatorsWithFilters.Count == 0 && ChbMaxClosingLogicSlots.Enabled && NUDMaxClosingLogicSlots.Value > 0)
                return false;
            if (_lockedExitFilters > 0 && _exitIndicatorsWithFilters.Count == 0)
                return false;
            if (ChbMaxClosingLogicSlots.Enabled && _exitFilterIndicators.Count < NUDMaxClosingLogicSlots.Value - _lockedExitFilters)
                return false;
            if (!ChbMaxClosingLogicSlots.Enabled && _exitFilterIndicators.Count < _maxClosingLogicSlots - _lockedExitFilters)
                return false;

            return true;
        }

        /// <summary>
        /// Generates a strategy
        /// </summary>
        private void Generating(BackgroundWorker worker, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            var workTime = new TimeSpan(0, _minutes, 0);
            DateTime stopTime = startTime + workTime;

            bool isStopGenerating = false;
            do
            {
                // The generating cycle
                if (worker.CancellationPending)
                {
                    // The Generating was stopped by the user
                    e.Cancel = true;
                    isStopGenerating = true;
                }
                else if (_minutes > 0 && stopTime < DateTime.Now)
                {
                    // The time finished
                    isStopGenerating = true;
                }
                else
                {
                    // The main job
                    GenerateStrategySlots();
                    GenerateSameOppSignal();
                    GeneratePermanentSL();
                    GeneratePermanentTP();
                    GenerateBreakEven();
                    GenerateMartingale();

                    // Calculates the back test.
                    bool isBetter = CalculateTheResult(false);

                    // Initial Optimization
                    if (ChbInitialOptimization.Checked)
                        PerformInitialOptimization(worker, isBetter);
                }

                if (_minutes > 0)
                {
                    // Report progress as a percentage of the total task.
                    TimeSpan passedTime = DateTime.Now - startTime;
                    var percentComplete = (int) (100*passedTime.TotalSeconds/workTime.TotalSeconds);
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > _progressPercent)
                    {
                        _progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            } while (!isStopGenerating);
        }

        /// <summary>
        /// Calculates the generated result
        /// </summary>
        private bool CalculateTheResult(bool isSaveEqualResult)
        {
            bool isBetter = false;
            _cycles++;

            Data.FirstBar = Data.Strategy.SetFirstBar();
            Data.Strategy.AdjustUsePreviousBarValue();

            // Sets default logical group for all slots that are open (not locked or linked).
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.SlotStatus == StrategySlotStatus.Open)
                    slot.LogicalGroup = Data.Strategy.GetDefaultGroup(slot.SlotNumber);

#if !DEBUG
            try
            {
#endif
            Backtester.Calculate();

            int balance = (_isOOS ? Backtester.Balance(_barOOS) : Backtester.NetBalance);
            bool isLimitationsOk = IsLimitationsFulfilled();

            if (isLimitationsOk)
            {
                if (_bestBalance < balance ||
                    (_bestBalance == balance && (isSaveEqualResult || Data.Strategy.Slots < _strategyBest.Slots)))
                {
                    _strategyBest = Data.Strategy.Clone();
                    _strategyBest.PropertiesStatus = Data.Strategy.PropertiesStatus;
                    for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                        _strategyBest.Slot[slot].SlotStatus = Data.Strategy.Slot[slot].SlotStatus;

                    string description = GenerateDescription();
                    if (balance > _bestBalance)
                        AddStrategyToGeneratorHistory(description);
                    else
                        UpdateStrategyInGeneratorHistory(description);
                    SetStrategyDescriptionButton();

                    _bestBalance = balance;
                    isBetter = true;
                    _isStartegyChanged = true;

                    RefreshSmallBalanceChart();
                    RefreshAccountStatisticas();
                    RebuildStrategyLayout(_strategyBest);
                    Top10AddStrategy();
                }
                else if (Top10Field.IsNominated(balance))
                {
                    Top10AddStrategy();
                }
            }

            SetLabelCyclesText(_cycles.ToString(CultureInfo.InvariantCulture));
#if !DEBUG
            }
            catch (Exception exception)
            {
                string text = GenerateCalculationErrorMessage(exception.Message);
                const string caption = "Strategy Calculation Error";
                ReportIndicatorError(text, caption);

                isBetter = false;
            }
#endif

            return isBetter;
        }

        /// <summary>
        /// Calculates an indicator and returns OK status.
        /// </summary>
        private bool CalculateIndicator(SlotTypes slotType, Indicator indicator)
        {
#if !DEBUG
            try
            {
#endif
                indicator.Calculate(slotType);
                return true;
#if !DEBUG
            }
            catch (Exception exception)
            {
                string message = "Please report this error in the support forum!";
                if (indicator.CustomIndicator)
                    message = "Please report this error to the author of the indicator!<br />" +
                        "You may remove this indicator from the Custom Indicators folder.";

                string text =
                    "<h1>Error: " + exception.Message + "</h1>" +
                    "<p>" +
                        "Slot type: <strong>" + slotType  + "</strong><br />" +
                        "Indicator: <strong>" + indicator + "</strong>" +
                    "</p>" +
                    "<p>" +
                        message +
                    "</p>";

                const string caption = "Indicator Calculation Error";
                ReportIndicatorError(text, caption);
                _indicatorBlackList.Add(indicator.IndicatorName);
                return false;
            }
#endif
        }

        /// <summary>
        /// Restores the strategy from the best one
        /// </summary>
        private void RestoreFromBest()
        {
            Data.Strategy = _strategyBest.Clone();
            Data.Strategy.PropertiesStatus = _strategyBest.PropertiesStatus;
            for (int slot = 0; slot < _strategyBest.Slots; slot++)
                Data.Strategy.Slot[slot].SlotStatus = _strategyBest.Slot[slot].SlotStatus;

            RecalculateSlots();
        }

        /// <summary>
        /// Check the strategy limitations
        /// </summary>
        private bool IsLimitationsFulfilled()
        {
            // The calculated strategy has higher profit
            // or the same profit but lower number of slots
            Backtester.CalculateAccountStats();

            // Limitation Max Ambiguous Bars
            if (ChbAmbiguousBars.Checked && Backtester.AmbiguousBars > NUDAmbiguousBars.Value)
                return false;

            // Limitation Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney
                                           ? Backtester.MaxMoneyEquityDrawdown
                                           : Backtester.MaxEquityDrawdown;
            if (ChbMaxDrawdown.Checked && maxEquityDrawdown > (double) NUDMaxDrawdown.Value)
                return false;

            // Limitation Max Equity percent drawdown
            if (ChbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double) NUDEquityPercent.Value)
                return false;

            // Limitation Min Trades
            if (ChbMinTrades.Checked && Backtester.ExecutedOrders < NUDMinTrades.Value)
                return false;

            // Limitation Max Trades
            if (ChbMaxTrades.Checked && Backtester.ExecutedOrders > NUDMaxTrades.Value)
                return false;

            // Limitation Win / Loss ratio
            if (ChbWinLossRatio.Checked && Backtester.WinLossRatio < (double) NUDWinLossRatio.Value)
                return false;

            // OOS Pattern filter
            if (ChbOOSPatternFilter.Checked && ChbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int oosBalance = Backtester.Balance(_barOOS);
                var targetBalance = (int) (oosBalance*_targetBalanceRatio);
                var minBalance = (int) (targetBalance*(1 - NUDOOSPatternPercent.Value/100));
                if (netBalance < oosBalance || netBalance < minBalance)
                    return false;
            }

            // Smooth Balance Line
            if (ChbSmoothBalanceLines.Checked)
            {
                var checkPoints = (int) NUDSmoothBalanceCheckPoints.Value;
                var maxPercentDeviation = (double) (NUDSmoothBalancePercent.Value/100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Data.FirstBar;
                    int bar = Data.FirstBar + i*(Data.Bars - firstBar)/(checkPoints + 1);
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    double minBalance = targetBalance*(1 - maxPercentDeviation);
                    double maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                        return false;

                    if (Configs.AdditionalStatistics)
                    {
                        // Long balance line
                        netBalance = Backtester.NetLongMoneyBalance;
                        checkPointBalance = Backtester.LongMoneyBalance(bar);
                        startBalance = Backtester.LongMoneyBalance(firstBar);
                        targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                        minBalance = targetBalance*(1 - maxPercentDeviation);
                        maxBalance = targetBalance*(1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;

                        // Short balance line
                        netBalance = Backtester.NetShortMoneyBalance;
                        checkPointBalance = Backtester.ShortMoneyBalance(bar);
                        startBalance = Backtester.ShortMoneyBalance(firstBar);
                        targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                        minBalance = targetBalance*(1 - maxPercentDeviation);
                        maxBalance = targetBalance*(1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Generates a random strategy
        /// </summary>
        private void GenerateStrategySlots()
        {
            // Determines the number of slots
            int openFilters = _random.Next(_lockedEntryFilters, _maxOpeningLogicSlots + 1);

            int closeFilters = 0;
            if (_lockedExitSlot == null ||
                _exitIndicatorsWithFilters.Contains(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName))
                closeFilters = _random.Next(_lockedExitFilters, _maxClosingLogicSlots + 1);

            // Create a strategy
            Data.Strategy = new Strategy(openFilters, closeFilters)
                                {
                                    StrategyName = "Generated",
                                    UseAccountPercentEntry = _strategyBest.UseAccountPercentEntry,
                                    MaxOpenLots = _strategyBest.MaxOpenLots,
                                    EntryLots = _strategyBest.EntryLots,
                                    AddingLots = _strategyBest.AddingLots,
                                    ReducingLots = _strategyBest.ReducingLots
                                };

            // Entry Slot
            int slot = 0;
            if (_lockedEntrySlot != null)
            {
                Data.Strategy.Slot[slot] = _lockedEntrySlot.Clone();
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            else
            {
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Entry filter slots
            for (int i = 0; i < _lockedEntryFilters; i++)
            {
                slot++;
                Data.Strategy.Slot[slot] = _aLockedEntryFilter[i].Clone();
                Data.Strategy.Slot[slot].SlotNumber = slot;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            for (int i = _lockedEntryFilters; i < openFilters; i++)
            {
                slot++;
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Exit slot
            if (_lockedExitSlot != null)
            {
                slot++;
                Data.Strategy.Slot[slot] = _lockedExitSlot.Clone();
                Data.Strategy.Slot[slot].SlotNumber = slot;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            else
            {
                slot++;
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Exit filter slots
            if (IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName) && closeFilters > 0)
            {
                for (int i = 0; i < _lockedExitFilters; i++)
                {
                    slot++;
                    Data.Strategy.Slot[slot] = _aLockedExitFilter[i].Clone();
                    Data.Strategy.Slot[slot].SlotNumber = slot;
                    if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                        GenerateIndicatorParameters(slot);
                }
                for (int i = _lockedExitFilters; i < closeFilters; i++)
                {
                    slot++;
                    GenerateIndicatorName(slot);
                    GenerateIndicatorParameters(slot);
                }
            }
        }

        /// <summary>
        /// Calculate the indicator in the designated slot
        /// </summary>
        private void GenerateIndicatorName(int slot)
        {
            SlotTypes slotType = Data.Strategy.GetSlotType(slot);
            string indicatorName;

            switch (slotType)
            {
                case SlotTypes.Open:
                    do
                    {
                        indicatorName = _entryIndicators[_random.Next(_entryIndicators.Count)];
                    } while (_indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.OpenFilter:
                    do
                    {
                        indicatorName = _entryFilterIndicators[_random.Next(_entryFilterIndicators.Count)];
                    } while (_indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.Close:
                    do
                    {
                        indicatorName = Data.Strategy.CloseFilters > 0
                            ? _exitIndicatorsWithFilters[_random.Next(_exitIndicatorsWithFilters.Count)]
                            : _exitIndicators[_random.Next(_exitIndicators.Count)];
                    } while (_indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.CloseFilter:
                    do
                    {
                        indicatorName = _exitFilterIndicators[_random.Next(_exitFilterIndicators.Count)];
                    } while (_indicatorBlackList.Contains(indicatorName));
                    break;
                default:
                    indicatorName = "Error!";
                    break;
            }

            Data.Strategy.Slot[slot].IndicatorName = indicatorName;
        }

        /// <summary>
        /// Calculate the indicator in the designated slot
        /// </summary>
        private void GenerateIndicatorParameters(int slot)
        {
            string indicatorName = Data.Strategy.Slot[slot].IndicatorName;
            SlotTypes slotType = Data.Strategy.GetSlotType(slot);
            Indicator indicator = IndicatorStore.ConstructIndicator(indicatorName, slotType);

            // List parameters
            foreach (ListParam list in indicator.IndParam.ListParam)
                if (list.Enabled)
                {
                    do
                    {
                        list.Index = _random.Next(list.ItemList.Length);
                        list.Text = list.ItemList[list.Index];
                    } while (list.Caption == "Base price" && (list.Text == "High" || list.Text == "Low"));
                }

            int firstBar;
            do
            {
                // Numeric parameters
                foreach (NumericParam num in indicator.IndParam.NumParam)
                    if (num.Enabled)
                    {
                        if (num.Caption == "Level" && !indicator.IndParam.ListParam[0].Text.Contains("Level"))
                            continue;
                        if (!ChbUseDefaultIndicatorValues.Checked)
                        {
                            double step = Math.Pow(10, -num.Point);
                            double minimum = num.Min;
                            double maximum = num.Max;

                            if (maximum > Data.Bars/3.0 && ((num.Caption.ToLower()).Contains("period") ||
                                                          (num.Caption.ToLower()).Contains("shift") ||
                                                          (num.ToolTip.ToLower()).Contains("period")))
                            {
                                maximum = Math.Max(minimum + step, Data.Bars/3.0);
                            }

                            double value = minimum + step*_random.Next((int) ((maximum - minimum)/step));
                            num.Value = Math.Round(value, num.Point);
                        }
                    }

                if (!CalculateIndicator(slotType, indicator))
                    return;

                firstBar = 0;
                foreach (IndicatorComp comp in indicator.Component)
                    if (comp.FirstBar > firstBar)
                        firstBar = comp.FirstBar;
            } while (firstBar > Data.Bars - 10);

            //Set the Data.Strategy
            IndicatorSlot indSlot = Data.Strategy.Slot[slot];
            indSlot.IndicatorName = indicator.IndicatorName;
            indSlot.IndParam = indicator.IndParam;
            indSlot.Component = indicator.Component;
            indSlot.SeparatedChart = indicator.SeparatedChart;
            indSlot.SpecValue = indicator.SpecialValues;
            indSlot.MinValue = indicator.SeparatedChartMinValue;
            indSlot.MaxValue = indicator.SeparatedChartMaxValue;
            indSlot.IsDefined = true;
        }

        /// <summary>
        /// Generate random same and opposite signal action
        /// </summary>
        private void GenerateSameOppSignal()
        {
            if (_strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.PropertiesStatus = _strategyBest.PropertiesStatus;
                Data.Strategy.SameSignalAction = _strategyBest.SameSignalAction;
                Data.Strategy.OppSignalAction = _strategyBest.OppSignalAction;
            }
            else
            {
                Data.Strategy.SameSignalAction =
                    (SameDirSignalAction) Enum.GetValues(typeof (SameDirSignalAction)).GetValue(_random.Next(3));
                Data.Strategy.OppSignalAction =
                    (OppositeDirSignalAction) Enum.GetValues(typeof (OppositeDirSignalAction)).GetValue(_random.Next(4));

                if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                    Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
            }
        }

        /// <summary>
        /// Generates the Permanent Stop Loss
        /// </summary>
        private void GeneratePermanentSL()
        {
            if (ChbPreservePermSL.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentSL = _strategyBest.UsePermanentSL;
                Data.Strategy.PermanentSLType = _strategyBest.PermanentSLType;
                Data.Strategy.PermanentSL = _strategyBest.PermanentSL;
            }
            else
            {
                bool usePermSL = _random.Next(100) > 30;
                bool changePermSL = _random.Next(100) > 50;
                Data.Strategy.UsePermanentSL = usePermSL;
                Data.Strategy.PermanentSLType = PermanentProtectionType.Relative;
                if (usePermSL && changePermSL)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentSL = multiplier*_random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add   || 
                    //    Data.Strategy.SameSignalAction == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentSLType = PermanentProtectionType.Absolute;
                }
            }
        }

        /// <summary>
        /// Generates the Permanent Take Profit
        /// </summary>
        private void GeneratePermanentTP()
        {
            if (ChbPreservePermTP.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentTP = _strategyBest.UsePermanentTP;
                Data.Strategy.PermanentTPType = _strategyBest.PermanentTPType;
                Data.Strategy.PermanentTP = _strategyBest.PermanentTP;
            }
            else
            {
                bool usePermTP = _random.Next(100) > 30;
                bool changePermTP = _random.Next(100) > 50;
                Data.Strategy.UsePermanentTP = usePermTP;
                Data.Strategy.PermanentTPType = PermanentProtectionType.Relative;
                if (usePermTP && changePermTP)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentTP = multiplier*_random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add    ||
                    //    Data.Strategy.SameSignalAction  == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction   == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentTPType = PermanentProtectionType.Absolute;
                }
            }
        }

        /// <summary>
        /// Generates Break Even stop.
        /// </summary>
        private void GenerateBreakEven()
        {
            if (ChbPreserveBreakEven.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseBreakEven = _strategyBest.UseBreakEven;
                Data.Strategy.BreakEven = _strategyBest.BreakEven;
            }
            else
            {
                bool useBreakEven = _random.Next(100) > 30;
                bool changeBreakEven = _random.Next(100) > 50;
                Data.Strategy.UseBreakEven = useBreakEven;
                if (useBreakEven && changeBreakEven)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.BreakEven = multiplier*_random.Next(5, 50);
                }
            }
        }

        private void GenerateMartingale()
        {
            if (_strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseMartingale = _strategyBest.UseMartingale;
                Data.Strategy.MartingaleMultiplier = _strategyBest.MartingaleMultiplier;
            }
            else
            {
                Data.Strategy.UseMartingale = false;
                Data.Strategy.MartingaleMultiplier = 2.0;
            }
        }

        /// <summary>
        /// Recalculate all the indicator slots 
        /// </summary>
        private void RecalculateSlots()
        {
            foreach (IndicatorSlot indSlot in Data.Strategy.Slot)
            {
                string indicatorName = indSlot.IndicatorName;
                SlotTypes slotType = indSlot.SlotType;
                Indicator indicator = IndicatorStore.ConstructIndicator(indicatorName, slotType);

                indicator.IndParam = indSlot.IndParam;
                indicator.Calculate(slotType);

                indSlot.Component = indicator.Component;
                indSlot.IsDefined = true;
            }

            // Searches the indicators' components to determine the Data.FirstBar 
            Data.FirstBar = Data.Strategy.SetFirstBar();
        }
    }
}