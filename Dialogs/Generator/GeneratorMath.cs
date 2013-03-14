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
    ///     Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        private readonly List<string> entryFilterIndicators = new List<string>();
        private readonly List<string> entryIndicators = new List<string>();
        private readonly List<string> exitFilterIndicators = new List<string>();
        private readonly List<string> exitIndicators = new List<string>();
        private readonly List<string> exitIndicatorsWithFilters = new List<string>();
        private readonly List<string> indicatorBlackList;
        private IndicatorSlot[] aLockedEntryFilter; // Holds all locked entry filters.
        private IndicatorSlot[] aLockedExitFilter; // Holds all locked exit filters.
        private int barOOS = Data.Bars - 1;
        private int bestBalance;
        private int cycles;
        private bool isEntryLocked; // Shows if the entry logic is locked
        private bool isExitLocked; // Shows if the exit logic is locked
        private bool isGenerating;
        private bool isOOS;
        private bool isStartegyChanged;
        private int lockedEntryFilters;
        private IndicatorSlot lockedEntrySlot; // Holds a locked entry slot.
        private int lockedExitFilters;
        private IndicatorSlot lockedExitSlot; // Holds a locked exit slot.
        private int maxClosingLogicSlots;
        private int maxOpeningLogicSlots;
        private int minutes;
        private int progressPercent;
        private Strategy strategyBest;

        // Out of Sample
        private float targetBalanceRatio = 1;

        /// <summary>
        ///     BtnGenerate_Click
        /// </summary>
        private void BtnGenerateClick(object sender, EventArgs e)
        {
            if (isGenerating)
            {
                // Cancel the asynchronous operation
                bgWorker.CancelAsync();
            }
            else
            {
                // Start the bgWorker
                PrepareStrategyForGenerating();
                CheckForLockedSlots();
                PrepareIndicatorLists();
                bool isEnoughIndicators = CheckAvailableIndicators();

                if (isEntryLocked && isExitLocked || !isEnoughIndicators)
                {
                    SystemSounds.Hand.Play();
                    return;
                }

                Cursor = Cursors.WaitCursor;

                minutes = (int) nudWorkingMinutes.Value;
                progressBar.Style = minutes > 0 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;

                GeneratedDescription = String.Empty;

                foreach (Control control in pnlCommon.Controls)
                    control.Enabled = false;
                foreach (Control control in pnlLimitations.Controls)
                    control.Enabled = false;
                foreach (Control control in pnlSettings.Controls)
                    control.Enabled = false;

                indicatorsField.BlockIndicatorChange();

                tsbtLockAll.Enabled = false;
                tsbtUnlockAll.Enabled = false;
                tsbtLinkAll.Enabled = false;
                tsbtOverview.Enabled = false;
                tsbtStrategyInfo.Enabled = false;

                lblCalcStrInfo.Enabled = true;
                lblCalcStrNumb.Enabled = true;
                chbHideFsb.Enabled = true;

                btnAccept.Enabled = false;
                btnCancel.Enabled = false;
                btnGenerate.Text = Language.T("Stop");

                isGenerating = true;

                progressBar.Value = 1;
                progressPercent = 0;
                cycles = 0;

                if (chbGenerateNewStrategy.Checked)
                    top10Field.ClearTop10Slots();

                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event
            var worker = sender as BackgroundWorker;

            // Generate a strategy
            Generating(worker, e);
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            RestoreFromBest();

            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();

            strategyField.Enabled = true;
            RebuildStrategyLayout(strategyBest);

            isGenerating = false;

            btnAccept.Enabled = true;
            btnCancel.Enabled = true;

            foreach (Control control in pnlCommon.Controls)
                control.Enabled = true;
            foreach (Control control in pnlLimitations.Controls)
                control.Enabled = true;
            foreach (Control control in pnlSettings.Controls)
                control.Enabled = true;

            indicatorsField.UnBlockIndicatorChange();

            tsbtLockAll.Enabled = true;
            tsbtUnlockAll.Enabled = true;
            tsbtLinkAll.Enabled = true;
            tsbtOverview.Enabled = true;
            tsbtStrategyInfo.Enabled = true;

            btnGenerate.Text = Language.T("Generate");
            progressBar.Style = ProgressBarStyle.Blocks;

            Cursor = Cursors.Default;
        }

        /// <summary>
        ///     Prepare the strategy for generating
        /// </summary>
        private void PrepareStrategyForGenerating()
        {
            lockedEntrySlot = null;
            lockedEntryFilters = 0;
            aLockedEntryFilter = new IndicatorSlot[Math.Max(Strategy.MaxOpenFilters, strategyBest.OpenFilters)];
            lockedExitSlot = null;
            lockedExitFilters = 0;
            aLockedExitFilter = new IndicatorSlot[Math.Max(Strategy.MaxCloseFilters, strategyBest.CloseFilters)];

            // Copy the locked slots
            for (int slot = 0; slot < strategyBest.Slots; slot++)
            {
                if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked ||
                    strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                {
                    if (strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                        lockedEntrySlot = strategyBest.Slot[slot];
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    {
                        aLockedEntryFilter[lockedEntryFilters] = strategyBest.Slot[slot];
                        lockedEntryFilters++;
                    }
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                        lockedExitSlot = strategyBest.Slot[slot];
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    {
                        aLockedExitFilter[lockedExitFilters] = strategyBest.Slot[slot];
                        lockedExitFilters++;
                    }
                }
            }

            if (chbGenerateNewStrategy.Checked)
                bestBalance = 0;
            else
                bestBalance = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);

            maxOpeningLogicSlots = chbMaxOpeningLogicSlots.Checked
                                       ? (int) nudMaxOpeningLogicSlots.Value
                                       : Strategy.MaxOpenFilters;
            maxClosingLogicSlots = chbMaxClosingLogicSlots.Checked
                                       ? (int) nudMaxClosingLogicSlots.Value
                                       : Strategy.MaxCloseFilters;
        }

        /// <summary>
        ///     Check if all slots are locked.
        /// </summary>
        private void CheckForLockedSlots()
        {
            isEntryLocked = false;
            isExitLocked = false;

            if (lockedEntrySlot != null && lockedEntryFilters >= maxOpeningLogicSlots)
                isEntryLocked = true;

            if (lockedEntryFilters > maxOpeningLogicSlots)
                maxOpeningLogicSlots = lockedEntryFilters;

            if (lockedExitSlot != null &&
                !IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(lockedExitSlot.IndicatorName))
                isExitLocked = true;
            else if (lockedExitSlot != null &&
                     IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(lockedExitSlot.IndicatorName) &&
                     lockedExitFilters >= maxClosingLogicSlots)
                isExitLocked = true;
            else if (lockedExitSlot == null && lockedExitFilters > 0 && lockedExitFilters >= maxClosingLogicSlots)
                isExitLocked = true;

            if (lockedExitFilters > maxClosingLogicSlots)
                maxClosingLogicSlots = lockedExitFilters;

            for (int slot = 0; slot < strategyBest.Slots; slot++)
            {
                if (strategyBest.Slot[slot].SlotStatus != StrategySlotStatus.Linked) continue;
                if (strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                    isEntryLocked = isEntryLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isEntryLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    isEntryLocked = isEntryLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isEntryLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                    isExitLocked = isExitLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isExitLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    isExitLocked = isExitLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isExitLocked;
            }
        }

        /// <summary>
        ///     Shows if the slot has any parameters to generate.
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
        ///     Prepare available indicators for each slot.
        /// </summary>
        private void PrepareIndicatorLists()
        {
            // Clear lists
            entryIndicators.Clear();
            entryFilterIndicators.Clear();
            exitIndicators.Clear();
            exitIndicatorsWithFilters.Clear();
            exitFilterIndicators.Clear();

            // Copy all no banned indicators
            foreach (string indicator in IndicatorStore.OpenPointIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Open, indicator))
                    entryIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.OpenFilterIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.OpenFilter, indicator))
                    entryFilterIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.ClosePointIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Close, indicator))
                    exitIndicators.Add(indicator);
            foreach (string indicator in IndicatorStore.ClosingIndicatorsWithClosingFilters)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Close, indicator))
                    exitIndicatorsWithFilters.Add(indicator);
            foreach (string indicator in IndicatorStore.CloseFilterIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.CloseFilter, indicator))
                    exitFilterIndicators.Add(indicator);

            // Remove special cases
            bool isPeriodDayOrWeek = Data.Period == DataPeriods.day || Data.Period == DataPeriods.week;

            if (entryIndicators.Contains("Fibonacci"))
                entryIndicators.Remove("Fibonacci");
            if (entryIndicators.Contains("Day Opening") && isPeriodDayOrWeek)
                entryIndicators.Remove("Day Opening");
            if (entryIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                entryIndicators.Remove("Hourly High Low");
            if (entryIndicators.Contains("Entry Hour") && isPeriodDayOrWeek)
                entryIndicators.Remove("Entry Hour");

            if (entryFilterIndicators.Contains("Random Filter"))
                entryFilterIndicators.Remove("Random Filter");
            if (entryFilterIndicators.Contains("Data Bars Filter"))
                entryFilterIndicators.Remove("Data Bars Filter");
            if (entryFilterIndicators.Contains("Date Filter"))
                entryFilterIndicators.Remove("Date Filter");
            if (entryFilterIndicators.Contains("Long or Short"))
                entryFilterIndicators.Remove("Long or Short");
            if (entryFilterIndicators.Contains("Entry Time"))
                entryFilterIndicators.Remove("Entry Time");
            if (entryFilterIndicators.Contains("Lot Limiter"))
                entryFilterIndicators.Remove("Lot Limiter");
            if (entryFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                entryFilterIndicators.Remove("Hourly High Low");

            if (exitIndicators.Contains("Day Closing") && isPeriodDayOrWeek)
                exitIndicators.Remove("Day Closing");
            if (exitIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                exitIndicators.Remove("Hourly High Low");
            if (exitIndicators.Contains("Exit Hour") && isPeriodDayOrWeek)
                exitIndicators.Remove("Exit Hour");
            if (exitIndicators.Contains("Close and Reverse") &&
                strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                exitIndicators.Remove("Close and Reverse");

            if (exitIndicatorsWithFilters.Contains("Day Closing") && isPeriodDayOrWeek)
                exitIndicatorsWithFilters.Remove("Day Closing");
            if (exitIndicatorsWithFilters.Contains("Close and Reverse") &&
                strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                exitIndicatorsWithFilters.Remove("Close and Reverse");

            if (exitFilterIndicators.Contains("Random Filter"))
                exitFilterIndicators.Remove("Random Filter");
            if (exitFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                exitFilterIndicators.Remove("Hourly High Low");
        }

        /// <summary>
        ///     Checks if enough indicators are allowed
        /// </summary>
        private bool CheckAvailableIndicators()
        {
            if (!isEntryLocked && entryIndicators.Count == 0)
                return false;
            if (entryFilterIndicators.Count < maxOpeningLogicSlots - lockedEntryFilters)
                return false;
            if (!isExitLocked && exitIndicators.Count == 0)
                return false;
            if (!isExitLocked && exitIndicatorsWithFilters.Count == 0 && chbMaxClosingLogicSlots.Enabled &&
                nudMaxClosingLogicSlots.Value > 0)
                return false;
            if (lockedExitFilters > 0 && exitIndicatorsWithFilters.Count == 0)
                return false;
            if (chbMaxClosingLogicSlots.Enabled &&
                exitFilterIndicators.Count < nudMaxClosingLogicSlots.Value - lockedExitFilters)
                return false;
            if (!chbMaxClosingLogicSlots.Enabled &&
                exitFilterIndicators.Count < maxClosingLogicSlots - lockedExitFilters)
                return false;

            return true;
        }

        /// <summary>
        ///     Generates a strategy
        /// </summary>
        private void Generating(BackgroundWorker worker, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            var workTime = new TimeSpan(0, minutes, 0);
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
                else if (minutes > 0 && stopTime < DateTime.Now)
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
                    if (chbInitialOptimization.Checked)
                        PerformInitialOptimization(worker, isBetter);
                }

                if (minutes > 0)
                {
                    // Report progress as a percentage of the total task.
                    TimeSpan passedTime = DateTime.Now - startTime;
                    var percentComplete = (int) (100*passedTime.TotalSeconds/workTime.TotalSeconds);
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > progressPercent)
                    {
                        progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            } while (!isStopGenerating);
        }

        /// <summary>
        ///     Calculates the generated result
        /// </summary>
        private bool CalculateTheResult(bool isSaveEqualResult)
        {
            bool isBetter = false;
            cycles++;

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

                int balance = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);
                bool isLimitationsOk = IsLimitationsFulfilled();

                if (isLimitationsOk)
                {
                    if (bestBalance < balance ||
                        (bestBalance == balance && (isSaveEqualResult || Data.Strategy.Slots < strategyBest.Slots)))
                    {
                        strategyBest = Data.Strategy.Clone();
                        strategyBest.PropertiesStatus = Data.Strategy.PropertiesStatus;
                        for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                            strategyBest.Slot[slot].SlotStatus = Data.Strategy.Slot[slot].SlotStatus;

                        string description = GenerateDescription();
                        if (balance > bestBalance)
                            AddStrategyToGeneratorHistory(description);
                        else
                            UpdateStrategyInGeneratorHistory(description);
                        SetStrategyDescriptionButton();

                        bestBalance = balance;
                        isBetter = true;
                        isStartegyChanged = true;

                        RefreshSmallBalanceChart();
                        RefreshAccountStatistics();
                        RebuildStrategyLayout(strategyBest);
                        Top10AddStrategy();
                    }
                    else if (top10Field.IsNominated(balance))
                    {
                        Top10AddStrategy();
                    }
                }

                SetLabelCyclesText(cycles.ToString(CultureInfo.InvariantCulture));
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
        ///     Calculates an indicator and returns OK status.
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
                    "Slot type: <strong>" + slotType + "</strong><br />" +
                    "Indicator: <strong>" + indicator + "</strong>" +
                    "</p>" +
                    "<p>" +
                    message +
                    "</p>";

                const string caption = "Indicator Calculation Error";
                ReportIndicatorError(text, caption);
                indicatorBlackList.Add(indicator.IndicatorName);
                return false;
            }
#endif
        }

        /// <summary>
        ///     Restores the strategy from the best one
        /// </summary>
        private void RestoreFromBest()
        {
            Data.Strategy = strategyBest.Clone();
            Data.Strategy.PropertiesStatus = strategyBest.PropertiesStatus;
            for (int slot = 0; slot < strategyBest.Slots; slot++)
                Data.Strategy.Slot[slot].SlotStatus = strategyBest.Slot[slot].SlotStatus;

            RecalculateSlots();
        }

        /// <summary>
        ///     Check the strategy limitations
        /// </summary>
        private bool IsLimitationsFulfilled()
        {
            // The calculated strategy has higher profit
            // or the same profit but lower number of slots
            Backtester.CalculateAccountStats();

            // Limitation Max Ambiguous Bars
            if (chbAmbiguousBars.Checked && Backtester.AmbiguousBars > nudAmbiguousBars.Value)
                return false;

            // Limitation Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney
                                           ? Backtester.MaxMoneyEquityDrawdown
                                           : Backtester.MaxEquityDrawdown;
            if (chbMaxDrawdown.Checked && maxEquityDrawdown > (double) nudMaxDrawdown.Value)
                return false;

            // Limitation Max Equity percent drawdown
            if (chbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double) nudEquityPercent.Value)
                return false;

            // Limitation Min Trades
            if (chbMinTrades.Checked && Backtester.ExecutedOrders < nudMinTrades.Value)
                return false;

            // Limitation Max Trades
            if (chbMaxTrades.Checked && Backtester.ExecutedOrders > nudMaxTrades.Value)
                return false;

            // Limitation Win / Loss ratio
            if (chbWinLossRatio.Checked && Backtester.WinLossRatio < (double) nudWinLossRatio.Value)
                return false;

            // OOS Pattern filter
            if (chbOOSPatternFilter.Checked && chbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int oosBalance = Backtester.Balance(barOOS);
                var targetBalance = (int) (oosBalance*targetBalanceRatio);
                var minBalance = (int) (targetBalance*(1 - nudoosPatternPercent.Value/100));
                if (netBalance < oosBalance || netBalance < minBalance)
                    return false;
            }

            // Smooth Balance Line
            if (chbSmoothBalanceLines.Checked)
            {
                var checkPoints = (int) nudSmoothBalanceCheckPoints.Value;
                var maxPercentDeviation = (double) (nudSmoothBalancePercent.Value/100);

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
        ///     Generates a random strategy
        /// </summary>
        private void GenerateStrategySlots()
        {
            // Determines the number of slots
            int openFilters = random.Next(lockedEntryFilters, maxOpeningLogicSlots + 1);

            int closeFilters = 0;
            if (lockedExitSlot == null ||
                exitIndicatorsWithFilters.Contains(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName))
                closeFilters = random.Next(lockedExitFilters, maxClosingLogicSlots + 1);

            // Create a strategy
            Data.Strategy = new Strategy(openFilters, closeFilters)
                {
                    StrategyName = "Generated",
                    UseAccountPercentEntry = strategyBest.UseAccountPercentEntry,
                    MaxOpenLots = strategyBest.MaxOpenLots,
                    EntryLots = strategyBest.EntryLots,
                    AddingLots = strategyBest.AddingLots,
                    ReducingLots = strategyBest.ReducingLots
                };

            // Entry Slot
            int slot = 0;
            if (lockedEntrySlot != null)
            {
                Data.Strategy.Slot[slot] = lockedEntrySlot.Clone();
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            else
            {
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Entry filter slots
            for (int i = 0; i < lockedEntryFilters; i++)
            {
                slot++;
                Data.Strategy.Slot[slot] = aLockedEntryFilter[i].Clone();
                Data.Strategy.Slot[slot].SlotNumber = slot;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            for (int i = lockedEntryFilters; i < openFilters; i++)
            {
                slot++;
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Exit slot
            if (lockedExitSlot != null)
            {
                slot++;
                Data.Strategy.Slot[slot] = lockedExitSlot.Clone();
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
            if (
                IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(
                    Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName) && closeFilters > 0)
            {
                for (int i = 0; i < lockedExitFilters; i++)
                {
                    slot++;
                    Data.Strategy.Slot[slot] = aLockedExitFilter[i].Clone();
                    Data.Strategy.Slot[slot].SlotNumber = slot;
                    if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                        GenerateIndicatorParameters(slot);
                }
                for (int i = lockedExitFilters; i < closeFilters; i++)
                {
                    slot++;
                    GenerateIndicatorName(slot);
                    GenerateIndicatorParameters(slot);
                }
            }
        }

        /// <summary>
        ///     Calculate the indicator in the designated slot
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
                        indicatorName = entryIndicators[random.Next(entryIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.OpenFilter:
                    do
                    {
                        indicatorName = entryFilterIndicators[random.Next(entryFilterIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.Close:
                    do
                    {
                        indicatorName = Data.Strategy.CloseFilters > 0
                                            ? exitIndicatorsWithFilters[random.Next(exitIndicatorsWithFilters.Count)]
                                            : exitIndicators[random.Next(exitIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.CloseFilter:
                    do
                    {
                        indicatorName = exitFilterIndicators[random.Next(exitFilterIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                default:
                    indicatorName = "Error!";
                    break;
            }

            Data.Strategy.Slot[slot].IndicatorName = indicatorName;
        }

        /// <summary>
        ///     Calculate the indicator in the designated slot
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
                        list.Index = random.Next(list.ItemList.Length);
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
                        if (!chbUseDefaultIndicatorValues.Checked)
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

                            double value = minimum + step*random.Next((int) ((maximum - minimum)/step));
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
        ///     Generate random same and opposite signal action
        /// </summary>
        private void GenerateSameOppSignal()
        {
            if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.PropertiesStatus = strategyBest.PropertiesStatus;
                Data.Strategy.SameSignalAction = strategyBest.SameSignalAction;
                Data.Strategy.OppSignalAction = strategyBest.OppSignalAction;
            }
            else
            {
                Data.Strategy.SameSignalAction =
                    (SameDirSignalAction) Enum.GetValues(typeof (SameDirSignalAction)).GetValue(random.Next(3));
                Data.Strategy.OppSignalAction =
                    (OppositeDirSignalAction) Enum.GetValues(typeof (OppositeDirSignalAction)).GetValue(random.Next(4));

                if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                    Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
            }
        }

        /// <summary>
        ///     Generates the Permanent Stop Loss
        /// </summary>
        private void GeneratePermanentSL()
        {
            if (chbPreservePermSL.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentSL = strategyBest.UsePermanentSL;
                Data.Strategy.PermanentSLType = strategyBest.PermanentSLType;
                Data.Strategy.PermanentSL = strategyBest.PermanentSL;
            }
            else
            {
                bool usePermSL = random.Next(100) > 30;
                bool changePermSL = random.Next(100) > 50;
                Data.Strategy.UsePermanentSL = usePermSL;
                Data.Strategy.PermanentSLType = PermanentProtectionType.Relative;
                if (usePermSL && changePermSL)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentSL = multiplier*random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add   || 
                    //    Data.Strategy.SameSignalAction == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentSLType = PermanentProtectionType.Absolute;
                }
            }
        }

        /// <summary>
        ///     Generates the Permanent Take Profit
        /// </summary>
        private void GeneratePermanentTP()
        {
            if (chbPreservePermTP.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentTP = strategyBest.UsePermanentTP;
                Data.Strategy.PermanentTPType = strategyBest.PermanentTPType;
                Data.Strategy.PermanentTP = strategyBest.PermanentTP;
            }
            else
            {
                bool usePermTP = random.Next(100) > 30;
                bool changePermTP = random.Next(100) > 50;
                Data.Strategy.UsePermanentTP = usePermTP;
                Data.Strategy.PermanentTPType = PermanentProtectionType.Relative;
                if (usePermTP && changePermTP)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentTP = multiplier*random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add    ||
                    //    Data.Strategy.SameSignalAction  == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction   == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentTPType = PermanentProtectionType.Absolute;
                }
            }
        }

        /// <summary>
        ///     Generates Break Even stop.
        /// </summary>
        private void GenerateBreakEven()
        {
            if (chbPreserveBreakEven.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseBreakEven = strategyBest.UseBreakEven;
                Data.Strategy.BreakEven = strategyBest.BreakEven;
            }
            else
            {
                bool useBreakEven = random.Next(100) > 30;
                bool changeBreakEven = random.Next(100) > 50;
                Data.Strategy.UseBreakEven = useBreakEven;
                if (useBreakEven && changeBreakEven)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.BreakEven = multiplier*random.Next(5, 50);
                }
            }
        }

        private void GenerateMartingale()
        {
            if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseMartingale = strategyBest.UseMartingale;
                Data.Strategy.MartingaleMultiplier = strategyBest.MartingaleMultiplier;
            }
            else
            {
                Data.Strategy.UseMartingale = false;
                Data.Strategy.MartingaleMultiplier = 2.0;
            }
        }

        /// <summary>
        ///     Recalculate all the indicator slots
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