// Strategy Generator - Optimization
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        /// <summary>
        /// Initial Optimization
        /// </summary>
        void PerformInitialOptimization(BackgroundWorker worker, bool isBetter)
        {
            bool secondChance = (_random.Next(100) < 10 && Backtester.NetBalance > 500);
            int maxCycles = isBetter ? 3 : 1;

            if (isBetter || secondChance)
            {
                for (int cycle = 0; cycle < maxCycles; cycle++)
                {
                    // Change parameters
                    ChangeNumericParameters(worker);

                    // Change Permanent Stop Loss
                    ChangePermanentSL(worker);

                    // Change Permanent Take Profit
                    ChangePermanentTP(worker);

                    // Change BreakEven
                    ChangeBreakEven(worker);
                }

                // Remove needless filters
                RemoveNeedlessFilters(worker);

                // Tries to clear the Same / Opposite Signals
                NormalizeSameOppositeSignalBehaviour(worker);

                // Remove Permanent Stop Loss
                if (!ChbPreservPermSL.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && Data.Strategy.UsePermanentSL && !worker.CancellationPending)
                    RemovePermanentSL();

                // Remove Permanent Take Profit
                if (!ChbPreservPermTP.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && Data.Strategy.UsePermanentTP && !worker.CancellationPending)
                    RemovePermanentTP();

                // Remove Break Even
                if (!ChbPreservBreakEven.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && Data.Strategy.UseBreakEven && !worker.CancellationPending)
                    RemoveBreakEven();

                // Reduce the value of numeric parameters
                if (!ChbUseDefaultIndicatorValues.Checked)
                    ReduceTheValuesOfNumericParams(worker);
           }
        }

        /// <summary>
        /// Tries to clear the Same / Opposite Signals
        /// </summary>
        void NormalizeSameOppositeSignalBehaviour(BackgroundWorker worker)
        {
            if (_strategyBest.PropertiesStatus != StrategySlotStatus.Open) return;

            if (Data.Strategy.SameSignalAction != SameDirSignalAction.Nothing)
            {
                if (!worker.CancellationPending)
                {
                    Data.Strategy.SameSignalAction = SameDirSignalAction.Nothing;
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }

            if (Data.Strategy.OppSignalAction != OppositeDirSignalAction.Nothing &&
                Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName != "Close and Reverse")
            {
                if (!worker.CancellationPending)
                {
                    Data.Strategy.OppSignalAction = OppositeDirSignalAction.Nothing;
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }
        }

        /// <summary>
        /// Removes the excessive filter.
        /// </summary>
        void RemoveNeedlessFilters(BackgroundWorker worker)
        {
            for (int slot = 1; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked || Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    continue;

                if (Data.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter || Data.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter)
                {
                    if (worker.CancellationPending) break;

                    Data.Strategy.RemoveFilter(slot);
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }
        }

        /// <summary>
        /// Change Numeric Parameters
        /// </summary>
        void ChangeNumericParameters(BackgroundWorker worker)
        {
            bool isDoAgain;
            int repeats = 0;
            do
            {
                isDoAgain = repeats < 4;
                repeats++;
                for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                {
                    if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;
                    if (worker.CancellationPending) break;

                    GenerateIndicatorParameters(slot);
                    RecalculateSlots();
                    isDoAgain = CalculateTheResult(false);
                    if (!isDoAgain)
                        RestoreFromBest();
                }
            } while (isDoAgain);
        }

        /// <summary>
        /// Change Permanent Stop Loss
        /// </summary>
        void ChangePermanentSL(BackgroundWorker worker)
        {
            bool isDoAgain;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservPermSL.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                    break;

                int oldPermSL = Data.Strategy.PermanentSL;
                Data.Strategy.UsePermanentSL = true;
                int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                Data.Strategy.PermanentSL = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    Data.Strategy.PermanentSL = oldPermSL;
            } while (isDoAgain);
        }

        /// <summary>
        /// Remove Permanent Stop Loss
        /// </summary>
        void RemovePermanentSL()
        {
            int oldPermSL = Data.Strategy.PermanentSL;
            Data.Strategy.UsePermanentSL = false;
            Data.Strategy.PermanentSL    = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            Data.Strategy.UsePermanentSL = true;
            Data.Strategy.PermanentSL = oldPermSL;
        }

        /// <summary>
        ///  Change Permanent Take Profit
        /// </summary>
        void ChangePermanentTP(BackgroundWorker worker)
        {
            bool isDoAgain;
            int  multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservPermTP.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked || !Data.Strategy.UsePermanentTP)
                    break;

                int oldPermTP = Data.Strategy.PermanentTP;
                Data.Strategy.UsePermanentTP = true;
                Data.Strategy.PermanentTP = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    Data.Strategy.PermanentTP = oldPermTP;
            } while (isDoAgain);
        }

        /// <summary>
        /// Removes the Permanent Take Profit
        /// </summary>
        void RemovePermanentTP()
        {
            int oldPermTP = Data.Strategy.PermanentTP;
            Data.Strategy.UsePermanentTP = false;
            Data.Strategy.PermanentTP = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            Data.Strategy.UsePermanentTP = true;
            Data.Strategy.PermanentTP = oldPermTP;
        }

        /// <summary>
        /// Change Break Even
        /// </summary>
        void ChangeBreakEven(BackgroundWorker worker)
        {
            bool isDoAgain;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservBreakEven.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked || !Data.Strategy.UseBreakEven)
                    break;

                int oldBreakEven = Data.Strategy.BreakEven;
                Data.Strategy.UseBreakEven = true;
                int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                Data.Strategy.BreakEven = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    Data.Strategy.BreakEven = oldBreakEven;
            } while (isDoAgain);
        }

        /// <summary>
        /// Removes the Break Even
        /// </summary>
        void RemoveBreakEven()
        {
            int oldBreakEven = Data.Strategy.BreakEven;
            Data.Strategy.UseBreakEven = false;
            Data.Strategy.BreakEven = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            Data.Strategy.UseBreakEven = true;
            Data.Strategy.BreakEven = oldBreakEven;
        }

        /// <summary>
        /// Normalizes the numeric parameters.
        /// </summary>
        void ReduceTheValuesOfNumericParams(BackgroundWorker worker)
        {
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (_bestBalance < 500) break;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;

                // Numeric parameters
                for (int param = 0; param < 6; param++)
                {
                    if (!Data.Strategy.Slot[slot].IndParam.NumParam[param].Enabled) continue;

                    bool isDoAgain;
                    do
                    {
                        if (worker.CancellationPending) break;

                        IndicatorSlot indSlot = Data.Strategy.Slot[slot];
                        NumericParam num = Data.Strategy.Slot[slot].IndParam.NumParam[param];
                        if (num.Caption == "Level" && !indSlot.IndParam.ListParam[0].Text.Contains("Level")) break;

                        Indicator indicator = IndicatorStore.ConstructIndicator(indSlot.IndicatorName, indSlot.SlotType);
                        double defaultValue = indicator.IndParam.NumParam[param].Value;

                        double numOldValue = num.Value;
                        if (Math.Abs(num.Value - defaultValue) < 0.00001) break;

                        double value = num.Value;
                        double delta = (defaultValue - value) * 3 / 4;
                        value += delta;
                        value = Math.Round(value, num.Point);

                        if (Math.Abs(value - numOldValue) < value) break;

                        num.Value = value;

                        RecalculateSlots();
                        isDoAgain = CalculateTheResult(true);
                        if (!isDoAgain) RestoreFromBest();

                    } while (isDoAgain);
                }
            }
        }
    }
}
