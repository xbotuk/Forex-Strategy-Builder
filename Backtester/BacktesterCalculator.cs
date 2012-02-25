// Backtester - Calculator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester : Data
    {
        // Private fields
        private static int _totalPositions;
        private static Session[] _session;
        private static OrderCoordinates[] _ordCoord;
        private static PositionCoordinates[] _posCoord;
        private static StrategyPriceType _openStrPriceType;
        private static StrategyPriceType _closeStrPriceType;

        // Environment
        private static ExecutionTime _openTimeExec;
        private static ExecutionTime _closeTimeExec;
        private static bool _isScanning;
        private static double _maximumLots = 100;

        // Additional
        private static double _micron = InstrProperties.Point/2;
        private static DateTime _lastEntryTime;

        // Logical Groups
        private static Dictionary<string, bool> _groupsAllowLong;
        private static Dictionary<string, bool> _groupsAllowShort;
        private static List<string> _openingLogicGroups;
        private static List<string> _closingLogicGroups;

        // N Bars Exit indicator - Krog
        private static bool _hasNBarsExit;
        private static int _slotNBarsExit;

        // Enter Once indicator
        private static bool _hasEnterOnce;
        private static int _slotEnterOnce;

        // Martingale
        private static int _consecutiveLosses;

        /// <summary>
        /// Gets the maximum number of orders.
        /// </summary>
        private static int MaxOrders
        {
            get
            {
                int maxOrders = 6; // Entry - 2, Exit - 3, Exit Margin Call - 1
                if (Strategy.UsePermanentSL)
                    maxOrders += 3; // Exit Perm. S/L - 3
                if (Strategy.UsePermanentTP)
                    maxOrders += 3; // Exit Perm. T/P - 3
                if (Strategy.UseBreakEven)
                    maxOrders += 6; // Activation - 3, Exit - 3
                return maxOrders;
            }
        }

        /// <summary>
        /// Gets the maximum number of positions.
        /// </summary>
        private static int MaxPositions
        {
            // Transferred - 1, Transferred closing - 1, Opening - 2, Closing - 2
            get { return 6; }
        }

        /// <summary>
        /// Resets the variables and prepares the arrays
        /// </summary>
        private static void ResetStart()
        {
            MarginCallBar = 0;
            SentOrders = 0;
            _totalPositions = 0;
            IsScanPerformed = false;
            _micron = InstrProperties.Point/2d;
            _lastEntryTime = new DateTime();

            // Sets the maximum lots
            _maximumLots = 100;
            foreach (IndicatorSlot slot in Strategy.Slot)
                if (slot.IndicatorName == "Lot Limiter")
                    _maximumLots = (int) slot.IndParam.NumParam[0].Value;

            _maximumLots = Math.Min(_maximumLots, Strategy.MaxOpenLots);

            _session = new Session[Bars];
            for (int bar = 0; bar < Bars; bar++)
                _session[bar] = new Session(MaxPositions, MaxOrders);

            for (int bar = 0; bar < FirstBar; bar++)
            {
                _session[bar].Summary.MoneyBalance = Configs.InitialAccount;
                _session[bar].Summary.MoneyEquity = Configs.InitialAccount;
            }

            _ordCoord = new OrderCoordinates[Bars*MaxOrders];
            _posCoord = new PositionCoordinates[Bars*MaxPositions];

            _openTimeExec = Strategy.Slot[Strategy.OpenSlot].IndParam.ExecutionTime;
            _closeTimeExec = Strategy.Slot[Strategy.CloseSlot].IndParam.ExecutionTime;

            _openStrPriceType = StrategyPriceType.Unknown;
            if (_openTimeExec == ExecutionTime.AtBarOpening)
                _openStrPriceType = StrategyPriceType.Open;
            else if (_openTimeExec == ExecutionTime.AtBarClosing)
                _openStrPriceType = StrategyPriceType.Close;
            else
                _openStrPriceType = StrategyPriceType.Indicator;

            _closeStrPriceType = StrategyPriceType.Unknown;
            if (_closeTimeExec == ExecutionTime.AtBarOpening)
                _closeStrPriceType = StrategyPriceType.Open;
            else if (_closeTimeExec == ExecutionTime.AtBarClosing)
                _closeStrPriceType = StrategyPriceType.Close;
            else if (_closeTimeExec == ExecutionTime.CloseAndReverse)
                _closeStrPriceType = StrategyPriceType.CloseAndReverce;
            else
                _closeStrPriceType = StrategyPriceType.Indicator;

            if (Configs.UseLogicalGroups)
            {
                Strategy.Slot[Strategy.OpenSlot].LogicalGroup = "All";
                    // Allows calculation of open slot for each group.
                Strategy.Slot[Strategy.CloseSlot].LogicalGroup = "All";
                    // Allows calculation of close slot for each group.

                _groupsAllowLong = new Dictionary<string, bool>();
                _groupsAllowShort = new Dictionary<string, bool>();
                for (int slot = Strategy.OpenSlot; slot < Strategy.CloseSlot; slot++)
                {
                    if (!_groupsAllowLong.ContainsKey(Strategy.Slot[slot].LogicalGroup))
                        _groupsAllowLong.Add(Strategy.Slot[slot].LogicalGroup, false);
                    if (!_groupsAllowShort.ContainsKey(Strategy.Slot[slot].LogicalGroup))
                        _groupsAllowShort.Add(Strategy.Slot[slot].LogicalGroup, false);
                }

                // List of logical groups
                _openingLogicGroups = new List<string>();
                foreach (var kvp in _groupsAllowLong)
                    _openingLogicGroups.Add(kvp.Key);

                // Logical groups of the closing conditions.
                _closingLogicGroups = new List<string>();
                for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots; slot++)
                {
                    string group = Strategy.Slot[slot].LogicalGroup;
                    if (!_closingLogicGroups.Contains(group) && group != "all")
                        _closingLogicGroups.Add(group); // Adds all groups except "all"
                }

                if (_closingLogicGroups.Count == 0)
                    _closingLogicGroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.
            }

            // Search for N Bars
            _hasNBarsExit = false;
            _slotNBarsExit = -1;
            foreach (IndicatorSlot slot in Strategy.Slot)
                if (slot.IndicatorName == "N Bars Exit")
                {
                    _hasNBarsExit = true;
                    _slotNBarsExit = slot.SlotNumber;
                    break;
                }

            // Search for Enter Once indicator
            _hasEnterOnce = false;
            _slotEnterOnce = -1;
            foreach (IndicatorSlot slot in Strategy.Slot)
                if (slot.IndicatorName == "Enter Once")
                {
                    _hasEnterOnce = true;
                    _slotEnterOnce = slot.SlotNumber;
                    break;
                }

            // Martingale
            _consecutiveLosses = 0;
        }

        /// <summary>
        /// Resets the variables at the end of the test.
        /// </summary>
        private static void ResetStop()
        {
            if (!Configs.UseLogicalGroups) return;
            Strategy.Slot[Strategy.OpenSlot].LogicalGroup = ""; // Delete the group of open slot.
            Strategy.Slot[Strategy.CloseSlot].LogicalGroup = ""; // Delete the group of close slot.
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        private static void SetPosition(int bar, OrderDirection ordDir, double lots, double price, int ordNumb)
        {
            int sessionPosition;
            Position position;
            double pipsToMoneyRate = InstrProperties.Point*InstrProperties.LotSize/AccountExchangeRate(price);
            bool isAbsoluteSL = Strategy.UsePermanentSL && Strategy.PermanentSLType == PermanentProtectionType.Absolute;
            bool isAbsoluteTP = Strategy.UsePermanentTP && Strategy.PermanentTPType == PermanentProtectionType.Absolute;

            if (_session[bar].Positions == 0 || _session[bar].Summary.PosLots < 0.0001)
            {
                // Open new position when either we have not opened one or it has been closed
                if (ordDir == OrderDirection.Buy)
                {
                    // Opens a long position
                    sessionPosition = _session[bar].Positions;
                    position = _session[bar].Position[sessionPosition] = new Position();

                    position.Transaction = Transaction.Open;
                    position.PosDir = PosDirection.Long;
                    position.OpeningBar = bar;
                    position.FormOrdNumb = ordNumb;
                    position.FormOrdPrice = price;
                    position.PosNumb = _totalPositions;
                    position.PosLots = lots;
                    position.AbsoluteSL = isAbsoluteSL ? price - Strategy.PermanentSL*InstrProperties.Point : 0;
                    position.AbsoluteTP = isAbsoluteTP ? price + Strategy.PermanentTP*InstrProperties.Point : 0;
                    position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                    position.Spread = lots*InstrProperties.Spread;
                    position.Commission = Commission(lots, price, false);
                    position.Slippage = lots*InstrProperties.Slippage;
                    position.PosPrice = price +
                                        (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point;
                    position.FloatingPL = lots*(Close[bar] - position.PosPrice)/InstrProperties.Point;
                    position.ProfitLoss = 0;
                    position.Balance = PosFromNumb(_totalPositions - 1).Balance - position.Commission;
                    position.Equity = position.Balance + position.FloatingPL;

                    position.MoneySpread = lots*InstrProperties.Spread*pipsToMoneyRate;
                    position.MoneyCommission = CommissionInMoney(lots, price, false);
                    position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                    position.MoneyFloatingPL = lots*(Close[bar] - position.PosPrice)*InstrProperties.LotSize/
                                               AccountExchangeRate(price);
                    position.MoneyProfitLoss = 0;
                    position.MoneyBalance = PosFromNumb(_totalPositions - 1).MoneyBalance - position.MoneyCommission;
                    position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                    _posCoord[_totalPositions].Bar = bar;
                    _posCoord[_totalPositions].Pos = sessionPosition;
                    _session[bar].Positions++;
                    _totalPositions++;

                    return;
                }

                if (ordDir == OrderDirection.Sell)
                {
                    // Opens a short position
                    sessionPosition = _session[bar].Positions;
                    position = _session[bar].Position[sessionPosition] = new Position();

                    position.Transaction = Transaction.Open;
                    position.PosDir = PosDirection.Short;
                    position.OpeningBar = bar;
                    position.FormOrdNumb = ordNumb;
                    position.FormOrdPrice = price;
                    position.PosNumb = _totalPositions;
                    position.PosLots = lots;
                    position.AbsoluteSL = isAbsoluteSL ? price + Strategy.PermanentSL*InstrProperties.Point : 0;
                    position.AbsoluteTP = isAbsoluteTP ? price - Strategy.PermanentTP*InstrProperties.Point : 0;
                    position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                    position.Spread = lots*InstrProperties.Spread;
                    position.Commission = Commission(lots, price, false);
                    position.Slippage = lots*InstrProperties.Slippage;
                    position.PosPrice = price -
                                        (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point;
                    position.FloatingPL = lots*(position.PosPrice - Close[bar])/InstrProperties.Point;
                    position.ProfitLoss = 0;
                    position.Balance = PosFromNumb(_totalPositions - 1).Balance - position.Commission;
                    position.Equity = position.Balance + position.FloatingPL;

                    position.MoneySpread = lots*InstrProperties.Spread*pipsToMoneyRate;
                    position.MoneyCommission = CommissionInMoney(lots, price, false);
                    position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                    position.MoneyFloatingPL = lots*(position.PosPrice - Close[bar])*InstrProperties.LotSize/
                                               AccountExchangeRate(price);
                    position.MoneyProfitLoss = 0;
                    position.MoneyBalance = PosFromNumb(_totalPositions - 1).MoneyBalance - position.MoneyCommission;
                    position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                    _posCoord[_totalPositions].Bar = bar;
                    _posCoord[_totalPositions].Pos = sessionPosition;
                    _session[bar].Positions++;
                    _totalPositions++;

                    return;
                }
            }

            int sessionPosOld = _session[bar].Positions - 1;
            Position positionOld = _session[bar].Position[sessionPosOld];
            PosDirection posDirOld = positionOld.PosDir;
            double lotsOld = positionOld.PosLots;
            double priceOld = positionOld.PosPrice;
            double absoluteSL = positionOld.AbsoluteSL;
            double absoluteTP = positionOld.AbsoluteTP;
            double posBalanceOld = positionOld.Balance;
            double posEquityOld = positionOld.Equity;

            // KROG - keep for N Bars Exit
            int openingBarOld = positionOld.OpeningBar;

            sessionPosition = sessionPosOld + 1;
            position = _session[bar].Position[sessionPosition] = new Position();

            position.PosNumb = _totalPositions;
            position.FormOrdPrice = price;
            position.FormOrdNumb = ordNumb;
            position.Balance = posBalanceOld;
            position.Equity = posEquityOld;
            position.MoneyBalance = positionOld.MoneyBalance;
            position.MoneyEquity = positionOld.MoneyEquity;

            _posCoord[_totalPositions].Bar = bar;
            _posCoord[_totalPositions].Pos = sessionPosition;
            _session[bar].Positions++;
            _totalPositions++;

            // Closing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && Math.Abs(lotsOld - lots) < 0.001)
            {
                position.Transaction = Transaction.Close;
                position.OpeningBar = openingBarOld; // KROG -- for N Bars Exit
                position.PosDir = PosDirection.Closed;
                position.PosLots = 0;
                position.AbsoluteSL = 0;
                position.AbsoluteTP = 0;
                position.RequiredMargin = 0;

                position.Commission = Commission(lots, price, true);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = priceOld;
                position.FloatingPL = 0;
                position.ProfitLoss = lots*(price - priceOld)/InstrProperties.Point - position.Slippage;
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = 0;
                position.MoneyProfitLoss = lots*(price - priceOld)*InstrProperties.LotSize/AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance;

                _consecutiveLosses = position.ProfitLoss < 0 ? _consecutiveLosses + 1 : 0;
                return;
            }

            // Closing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && Math.Abs(lotsOld - lots) < 0.001)
            {
                position.Transaction = Transaction.Close;
                position.OpeningBar = openingBarOld;
                position.PosDir = PosDirection.Closed;
                position.PosLots = 0;
                position.AbsoluteSL = 0;
                position.AbsoluteTP = 0;
                position.RequiredMargin = 0;

                position.Commission = Commission(lots, price, true);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = priceOld;
                position.FloatingPL = 0;
                position.ProfitLoss = lots*(priceOld - price)/InstrProperties.Point - position.Slippage;
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = 0;
                position.MoneyProfitLoss = lots*(priceOld - price)*InstrProperties.LotSize/AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance;

                _consecutiveLosses = position.ProfitLoss < 0 ? _consecutiveLosses + 1 : 0;
                return;
            }

            // Adding to a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Buy)
            {
                position.Transaction = Transaction.Add;
                position.OpeningBar = openingBarOld;
                position.PosDir = PosDirection.Long;
                position.PosLots = NormalizeEntryLots(lotsOld + lots);
                position.AbsoluteSL = absoluteSL;
                position.AbsoluteTP = absoluteTP;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Spread = lots*InstrProperties.Spread;
                position.Commission = Commission(lots, price, false);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = (lotsOld*priceOld + lots*(price + (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point))/(lotsOld + lots);
                position.FloatingPL = (lotsOld + lots)*(Close[bar] - position.PosPrice)/InstrProperties.Point;
                position.ProfitLoss = 0;
                position.Balance -= position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneySpread = lots*InstrProperties.Spread*pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lots, price, false);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld + lots)*(Close[bar] - position.PosPrice)*InstrProperties.LotSize/AccountExchangeRate(price);
                position.MoneyProfitLoss = 0;
                position.MoneyBalance -= position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Adding to a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Sell)
            {
                position.Transaction = Transaction.Add;
                position.OpeningBar = openingBarOld;
                position.PosDir = PosDirection.Short;
                position.PosLots = NormalizeEntryLots(lotsOld + lots);
                position.AbsoluteSL = absoluteSL;
                position.AbsoluteTP = absoluteTP;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Spread = lots*InstrProperties.Spread;
                position.Commission = Commission(lots, price, false);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = (lotsOld*priceOld + lots*(price - (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point))/(lotsOld + lots);
                position.FloatingPL = (lotsOld + lots)*(position.PosPrice - Close[bar])/InstrProperties.Point;
                position.ProfitLoss = 0;
                position.Balance -= position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneySpread = lots*InstrProperties.Spread*pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lots, price, false);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld + lots)*(position.PosPrice - Close[bar])*InstrProperties.LotSize/AccountExchangeRate(price);
                position.MoneyProfitLoss = 0;
                position.MoneyBalance -= position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Reducing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && lotsOld > lots)
            {
                position.Transaction = Transaction.Reduce;
                position.OpeningBar = openingBarOld;
                position.PosDir = PosDirection.Long;
                position.PosLots = NormalizeEntryLots(lotsOld - lots);
                position.AbsoluteSL = absoluteSL;
                position.AbsoluteTP = absoluteTP;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Commission = Commission(lots, price, true);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = priceOld;
                position.FloatingPL = (lotsOld - lots)*(Close[bar] - priceOld)/InstrProperties.Point;
                position.ProfitLoss = lots*((price - priceOld)/InstrProperties.Point - InstrProperties.Slippage);
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld - lots)*(Close[bar] - priceOld)*InstrProperties.LotSize/AccountExchangeRate(price);
                position.MoneyProfitLoss = lots*(price - priceOld)*InstrProperties.LotSize/AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                _consecutiveLosses = 0;
                return;
            }

            // Reducing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && lotsOld > lots)
            {
                position.Transaction = Transaction.Reduce;
                position.OpeningBar = openingBarOld;
                position.PosDir = PosDirection.Short;
                position.PosLots = NormalizeEntryLots(lotsOld - lots);
                position.AbsoluteSL = absoluteSL;
                position.AbsoluteTP = absoluteTP;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Commission = Commission(lots, price, true);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = priceOld;
                position.FloatingPL = (lotsOld - lots)*(priceOld - Close[bar])/InstrProperties.Point;
                position.ProfitLoss = lots*((priceOld - price)/InstrProperties.Point - InstrProperties.Slippage);
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld - lots)*(priceOld - Close[bar])*InstrProperties.LotSize/
                                           AccountExchangeRate(price);
                position.MoneyProfitLoss = lots*(priceOld - price)*InstrProperties.LotSize/AccountExchangeRate(price) -
                                           position.MoneySlippage;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                _consecutiveLosses = 0;
                return;
            }

            // Reversing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && lotsOld < lots)
            {
                position.Transaction = Transaction.Reverse;
                position.PosDir = PosDirection.Short;
                position.PosLots = NormalizeEntryLots(lots - lotsOld);
                position.OpeningBar = bar;
                position.AbsoluteSL = isAbsoluteSL ? price + Strategy.PermanentSL*InstrProperties.Point : 0;
                position.AbsoluteTP = isAbsoluteTP ? price - Strategy.PermanentTP*InstrProperties.Point : 0;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Spread = (lots - lotsOld)*InstrProperties.Spread;
                position.Commission = Commission(lotsOld, price, true) + Commission(lots - lotsOld, price, false);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = price - (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point;
                position.FloatingPL = (lots - lotsOld)*(position.PosPrice - Close[bar])/InstrProperties.Point;
                position.ProfitLoss = lotsOld*((price - priceOld)/InstrProperties.Point - InstrProperties.Slippage);
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneySpread = (lots - lotsOld)*InstrProperties.Spread*pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lotsOld, price, true) + CommissionInMoney(lots - lotsOld, price, false);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lots - lotsOld)*(position.PosPrice - Close[bar])*InstrProperties.LotSize/AccountExchangeRate(price);
                position.MoneyProfitLoss = lotsOld*(price - priceOld)*InstrProperties.LotSize/AccountExchangeRate(price) - lotsOld*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                _consecutiveLosses = 0;
                return;
            }

            // Reversing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && lotsOld < lots)
            {
                position.Transaction = Transaction.Reverse;
                position.PosDir = PosDirection.Long;
                position.PosLots = NormalizeEntryLots(lots - lotsOld);
                position.OpeningBar = bar;
                position.AbsoluteSL = Strategy.UsePermanentSL ? price - Strategy.PermanentSL*InstrProperties.Point : 0;
                position.AbsoluteTP = Strategy.UsePermanentTP ? price + Strategy.PermanentTP*InstrProperties.Point : 0;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);

                position.Spread = (lots - lotsOld)*InstrProperties.Spread;
                position.Commission = Commission(lotsOld, price, true) + Commission(lots - lotsOld, price, false);
                position.Slippage = lots*InstrProperties.Slippage;
                position.PosPrice = price + (InstrProperties.Spread + InstrProperties.Slippage)*InstrProperties.Point;
                position.FloatingPL = (lots - lotsOld)*(Close[bar] - position.PosPrice)/InstrProperties.Point;
                position.ProfitLoss = lotsOld*((priceOld - price)/InstrProperties.Point - InstrProperties.Slippage);
                position.Balance += position.ProfitLoss - position.Commission;
                position.Equity = position.Balance + position.FloatingPL;

                position.MoneySpread = (lots - lotsOld)*InstrProperties.Spread*pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lotsOld, price, true) +
                                           CommissionInMoney(lots - lotsOld, price, false);
                position.MoneySlippage = lots*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyFloatingPL = (lots - lotsOld)*(Close[bar] - position.PosPrice)*InstrProperties.LotSize/
                                           AccountExchangeRate(price);
                position.MoneyProfitLoss = lotsOld*(priceOld - price)*InstrProperties.LotSize/AccountExchangeRate(price) -
                                           lotsOld*InstrProperties.Slippage*pipsToMoneyRate;
                position.MoneyBalance += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                _consecutiveLosses = 0;
            }
        }

        /// <summary>
        /// Checks all orders in the current bar and cancels the invalid ones.
        /// </summary>
        private static void CancelInvalidOrders(int bar)
        {
            // Cancelling the EOP orders
            for (int ord = 0; ord < _session[bar].Orders; ord++)
                if (_session[bar].Order[ord].OrdStatus == OrderStatus.Confirmed)
                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;

            // Cancelling the invalid IF orders
            for (int ord = 0; ord < _session[bar].Orders; ord++)
                if (_session[bar].Order[ord].OrdStatus == OrderStatus.Confirmed &&
                    _session[bar].Order[ord].OrdCond == OrderCondition.If &&
                    OrdFromNumb(_session[bar].Order[ord].OrdIF).OrdStatus != OrderStatus.Executed)
                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
        }

        /// <summary>
        /// Cancel all no executed entry orders
        /// </summary>
        private static void CancelNoexecutedEntryOrders(int bar)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                if (_session[bar].Order[ord].OrdSender == OrderSender.Open &&
                    _session[bar].Order[ord].OrdStatus != OrderStatus.Executed)
                {
                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                }
            }
        }

        /// <summary>
        /// Cancel all no executed exit orders
        /// </summary>
        private static void CancelNoexecutedExitOrders(int bar)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                if (_session[bar].Order[ord].OrdSender == OrderSender.Close &&
                    _session[bar].Order[ord].OrdStatus != OrderStatus.Executed)
                {
                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                }
            }
        }

        /// <summary>
        /// Executes an entry at the beginning of the bar
        /// </summary>
        private static void ExecuteEntryAtOpeningPrice(int bar)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                if (_session[bar].Order[ord].OrdSender == OrderSender.Open)
                {
                    ExecOrd(bar, _session[bar].Order[ord], Open[bar], BacktestEval.Correct);
                }
            }
        }

        /// <summary>
        /// Executes an entry at the closing of the bar
        /// </summary>
        private static void ExecuteEntryAtClosingPrice(int bar)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                if (_session[bar].Order[ord].OrdSender == OrderSender.Open)
                {
                    ExecOrd(bar, _session[bar].Order[ord], Close[bar], BacktestEval.Correct);
                }
            }
        }

        /// <summary>
        /// Executes an exit at the closing of the bar
        /// </summary>
        private static void ExecuteExitAtClosingPrice(int bar)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                if (_session[bar].Order[ord].OrdSender == OrderSender.Close && CheckOrd(bar, ord))
                {
                    ExecOrd(bar, _session[bar].Order[ord], Close[bar], BacktestEval.Correct);
                }
            }
        }

        /// <summary>
        /// Checks and perform actions in case of a Margin Call
        /// </summary>
        private static void MarginCallCheckAtBarClosing(int bar)
        {
            if (!Configs.TradeUntilMarginCall ||
                _session[bar].Summary.FreeMargin >= 0)
                return;

            if (_session[bar].Summary.PosDir == PosDirection.None ||
                _session[bar].Summary.PosDir == PosDirection.Closed)
                return;

            CancelNoexecutedExitOrders(bar);

            const int ifOrd = 0;
            int toPos = _session[bar].Summary.PosNumb;
            double lots = _session[bar].Summary.PosLots;
            string note = Language.T("Close due to a Margin Call");

            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                OrdSellMarket(bar, ifOrd, toPos, lots, Close[bar], OrderSender.Close, OrderOrigin.MarginCall, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                OrdBuyMarket(bar, ifOrd, toPos, lots, Close[bar], OrderSender.Close, OrderOrigin.MarginCall, note);
            }

            ExecuteExitAtClosingPrice(bar);

            // Margin Call bar
            if (MarginCallBar == 0)
                MarginCallBar = bar;
        }

        /// <summary>
        /// Checks the order
        /// </summary>
        /// <returns>True if the order is valid.</returns>
        private static bool CheckOrd(int bar, int iOrd)
        {
            if (_session[bar].Order[iOrd].OrdStatus != OrderStatus.Confirmed)
                return false;
            if (_session[bar].Order[iOrd].OrdCond != OrderCondition.If)
                return true;
            if (OrdFromNumb(_session[bar].Order[iOrd].OrdIF).OrdStatus == OrderStatus.Executed)
                return true;
            return false;
        }

        /// <summary>
        /// Tunes and Executes an order
        /// </summary>
        private static void ExecOrd(int bar, Order order, double price, BacktestEval testEval)
        {
            Position position = _session[bar].Summary;
            PosDirection posDir = position.PosDir;
            OrderDirection ordDir = order.OrdDir;
            WayPointType wayPointType = WayPointType.None;

            // Orders modification on a fly
            // Checks whether we are on the market
            if (posDir == PosDirection.Long || posDir == PosDirection.Short)
            {
                // We are on the market
                if (order.OrdSender == OrderSender.Open)
                {
                    // Entry orders
                    if (ordDir == OrderDirection.Buy && posDir == PosDirection.Long ||
                        ordDir == OrderDirection.Sell && posDir == PosDirection.Short)
                    {
                        // In case of a Same Dir Signal
                        switch (Strategy.SameSignalAction)
                        {
                            case SameDirSignalAction.Add:
                                order.OrdLots = TradingSize(Strategy.AddingLots, bar);
                                if (position.PosLots + TradingSize(Strategy.AddingLots, bar) <= _maximumLots)
                                {
                                    // Adding
                                    wayPointType = WayPointType.Add;
                                }
                                else
                                {
                                    // Cancel the Adding
                                    order.OrdStatus = OrderStatus.Cancelled;
                                    wayPointType = WayPointType.Cancel;
                                    FindCancelExitOrder(bar, order); // Canceling of its exit order
                                }
                                break;
                            case SameDirSignalAction.Winner:
                                order.OrdLots = TradingSize(Strategy.AddingLots, bar);
                                if (position.PosLots + TradingSize(Strategy.AddingLots, bar) <= _maximumLots &&
                                    (position.PosDir == PosDirection.Long && position.PosPrice < order.OrdPrice ||
                                     position.PosDir == PosDirection.Short && position.PosPrice > order.OrdPrice))
                                {
                                    // Adding
                                    wayPointType = WayPointType.Add;
                                }
                                else
                                {
                                    // Cancel the Adding
                                    order.OrdStatus = OrderStatus.Cancelled;
                                    wayPointType = WayPointType.Cancel;
                                    FindCancelExitOrder(bar, order); // Canceling of its exit order
                                }
                                break;
                            case SameDirSignalAction.Nothing:
                                order.OrdLots = TradingSize(Strategy.AddingLots, bar);
                                order.OrdStatus = OrderStatus.Cancelled;
                                wayPointType = WayPointType.Cancel;
                                FindCancelExitOrder(bar, order); // Canceling of its exit order
                                break;
                        }
                    }
                    else if (ordDir == OrderDirection.Buy && posDir == PosDirection.Short ||
                             ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                    {
                        // In case of an Opposite Dir Signal
                        switch (Strategy.OppSignalAction)
                        {
                            case OppositeDirSignalAction.Reduce:
                                if (position.PosLots > TradingSize(Strategy.ReducingLots, bar))
                                {
                                    // Reducing
                                    order.OrdLots = TradingSize(Strategy.ReducingLots, bar);
                                    wayPointType = WayPointType.Reduce;
                                }
                                else
                                {
                                    // Closing
                                    order.OrdLots = position.PosLots;
                                    wayPointType = WayPointType.Exit;
                                }
                                break;
                            case OppositeDirSignalAction.Close:
                                order.OrdLots = position.PosLots;
                                wayPointType = WayPointType.Exit;
                                break;
                            case OppositeDirSignalAction.Reverse:
                                order.OrdLots = position.PosLots + TradingSize(Strategy.EntryLots, bar);
                                wayPointType = WayPointType.Reverse;
                                break;
                            case OppositeDirSignalAction.Nothing:
                                order.OrdStatus = OrderStatus.Cancelled;
                                wayPointType = WayPointType.Cancel;
                                FindCancelExitOrder(bar, order); // Canceling of its exit order
                                break;
                        }
                    }
                }
                else
                {
                    // Exit orders
                    if (ordDir == OrderDirection.Buy && posDir == PosDirection.Short ||
                        ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                    {
                        // Check for Break Even Activation
                        if (order.OrdOrigin == OrderOrigin.BreakEvenActivation)
                        {
                            // This is a fictive order
                            order.OrdStatus = OrderStatus.Cancelled;
                            wayPointType = WayPointType.Cancel;
                        }
                        else
                        {
                            // The Close orders can only close the position
                            order.OrdLots = position.PosLots;
                            wayPointType = WayPointType.Exit;
                        }
                    }
                    else
                    {
                        // If the direction of the exit order is same as the position's direction
                        // the order have to be cancelled
                        order.OrdStatus = OrderStatus.Cancelled;
                        wayPointType = WayPointType.Cancel;
                    }
                }
            }
            else
            {
                // We are out of the market
                if (order.OrdSender == OrderSender.Open)
                {
                    // Open a new position
                    double entryAmount = TradingSize(Strategy.EntryLots, bar);
                    if (Strategy.UseMartingale && _consecutiveLosses > 0)
                    {
                        entryAmount = entryAmount*Math.Pow(Strategy.MartingaleMultiplier, _consecutiveLosses);
                        entryAmount = NormalizeEntryLots(entryAmount);
                    }
                    order.OrdLots = Math.Min(entryAmount, _maximumLots);
                    wayPointType = WayPointType.Entry;
                }
                else // if (order.OrdSender == OrderSender.Close)
                {
                    // The Close strategy cannot do anything
                    order.OrdStatus = OrderStatus.Cancelled;
                    wayPointType = WayPointType.Cancel;
                }
            }

            // Enter Once can cancel an entry order
            if (_hasEnterOnce && order.OrdSender == OrderSender.Open && order.OrdStatus == OrderStatus.Confirmed)
            {
                bool toCancel = false;
                switch (Strategy.Slot[_slotEnterOnce].IndParam.ListParam[0].Text)
                {
                    case "Enter no more than once a bar":
                        toCancel = Time[bar] == _lastEntryTime;
                        break;
                    case "Enter no more than once a day":
                        toCancel = Time[bar].DayOfYear == _lastEntryTime.DayOfYear;
                        break;
                    case "Enter no more than once a week":
                        int lastEntryWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            _lastEntryTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        int currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            Time[bar], CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        toCancel = lastEntryWeek == currentWeek;
                        break;
                    case "Enter no more than once a month":
                        toCancel = Time[bar].Month == _lastEntryTime.Month;
                        break;
                }

                if (toCancel)
                {
                    // Cancel the entry order
                    order.OrdStatus = OrderStatus.Cancelled;
                    wayPointType = WayPointType.Cancel;
                    FindCancelExitOrder(bar, order); // Canceling of its exit order
                }
                else
                    _lastEntryTime = Time[bar];
            }

            // Do not trade after Margin Call or after -1000000 Loss
            if (order.OrdSender == OrderSender.Open && order.OrdStatus == OrderStatus.Confirmed)
            {
                if (position.FreeMargin < -1000000 ||
                    Configs.TradeUntilMarginCall && RequiredMargin(order.OrdLots, bar) > position.FreeMargin)
                {
                    // Cancel the entry order
                    order.OrdStatus = OrderStatus.Cancelled;
                    wayPointType = WayPointType.Cancel;
                    FindCancelExitOrder(bar, order); // Canceling of its exit order
                }
            }

            // Executing the order
            if (order.OrdStatus == OrderStatus.Confirmed)
            {
                // Executes the order
                SetPosition(bar, ordDir, order.OrdLots, price, order.OrdNumb);
                order.OrdStatus = OrderStatus.Executed;

                // Set the evaluation
                switch (testEval)
                {
                    case BacktestEval.Error:
                        _session[bar].BacktestEval = BacktestEval.Error;
                        break;
                    case BacktestEval.None:
                        break;
                    case BacktestEval.Ambiguous:
                        _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        break;
                    case BacktestEval.Unknown:
                        if (_session[bar].BacktestEval != BacktestEval.Ambiguous)
                            _session[bar].BacktestEval = BacktestEval.Unknown;
                        break;
                    case BacktestEval.Correct:
                        if (_session[bar].BacktestEval == BacktestEval.None)
                            _session[bar].BacktestEval = BacktestEval.Correct;
                        break;
                }

                // If entry order closes or reverses the position the exit orders of the
                // initial position have to be cancelled
                if (order.OrdSender == OrderSender.Open &&
                    (_session[bar].Summary.Transaction == Transaction.Close ||
                     _session[bar].Summary.Transaction == Transaction.Reverse))
                {
                    int initialNumber = _session[bar].Position[_session[bar].Positions - 2].FormOrdNumb;
                    // If the position was opened during the current bar, we can find its exit order
                    bool isFound = false;
                    for (int ord = 0; ord < _session[bar].Orders; ord++)
                    {
                        if (_session[bar].Order[ord].OrdIF == initialNumber &&
                            _session[bar].Order[ord].OrdSender == OrderSender.Close)
                        {
                            _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                            isFound = true;
                            break;
                        }
                    }

                    // In case when the order is not found, this means that the position is transferred
                    // so its exit order is not conditional
                    if (!isFound)
                    {
                        for (int ord = 0; ord < _session[bar].Orders; ord++)
                        {
                            if (_session[bar].Order[ord].OrdSender == OrderSender.Close &&
                                _session[bar].Order[ord].OrdIF == 0)
                            {
                                _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                                break;
                            }
                        }
                    }

                    // Setting the exit order of the current position
                    switch (_session[bar].Summary.Transaction)
                    {
                        case Transaction.Close:
                            {
                                // In case of closing we have to cancel the exit order
                                int number = _session[bar].Summary.FormOrdNumb;
                                for (int ord = 0; ord < _session[bar].Orders; ord++)
                                {
                                    if (_session[bar].Order[ord].OrdIF != number) continue;
                                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                                    break;
                                }
                            }
                            break;
                        case Transaction.Reduce:
                            {
                                // In case of reducing we have to change the direction of the exit order
                                int number = _session[bar].Summary.FormOrdNumb;
                                for (int ord = 0; ord < _session[bar].Orders; ord++)
                                {
                                    if (_session[bar].Order[ord].OrdIF != number) continue;
                                    _session[bar].Order[ord].OrdDir = _session[bar].Summary.PosDir == PosDirection.Long 
                                                                         ? OrderDirection.Sell
                                                                         : OrderDirection.Buy;
                                    break;
                                }
                            }
                            break;
                    }
                }
            }

            _session[bar].SetWayPoint(price, wayPointType);
            if (order.OrdStatus == OrderStatus.Cancelled)
            {
                _session[bar].WayPoint[_session[bar].WayPoints - 1].OrdNumb = order.OrdNumb;
            }
        }

        /// <summary>
        /// Finds and cancels the exit order of an entry order
        /// </summary>
        private static void FindCancelExitOrder(int bar, Order order)
        {
            for (int ord = 0; ord < _session[bar].Orders; ord++)
                if (_session[bar].Order[ord].OrdIF == order.OrdNumb)
                {
                    _session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                    break;
                }
        }

        /// <summary>
        /// Transfers the orders and positions from the previous bar.
        /// </summary>
        private static void TransferFromPreviousBar(int bar)
        {
            // Check the previous bar for an open position
            if (_session[bar - 1].Summary.PosDir == PosDirection.Long ||
                _session[bar - 1].Summary.PosDir == PosDirection.Short)
            {
                // Yes, we have a position
                // We copy the position from the previous bar
                int sessionPosition = _session[bar].Positions;
                Position position = _session[bar].Position[sessionPosition] = _session[bar - 1].Summary.Copy();

                // How many days we transfer the positions with
                int days = Time[bar].DayOfYear - Time[bar - 1].DayOfYear;
                if (days < 0) days += 365;

                position.Rollover = 0;
                position.MoneyRollover = 0;

                if (days > 0)
                {
                    // Calculate the Rollover fee
                    double swapLongPips = 0;
                    double swapShortPips = 0;

                    if (InstrProperties.SwapType == CommissionType.pips)
                    {
                        swapLongPips = InstrProperties.SwapLong;
                        swapShortPips = InstrProperties.SwapShort;
                    }
                    else if (InstrProperties.SwapType == CommissionType.percents)
                    {
                        swapLongPips = (Close[bar - 1]/InstrProperties.Point)*(0.01*InstrProperties.SwapLong/365);
                        swapShortPips = (Close[bar - 1]/InstrProperties.Point)*(0.01*InstrProperties.SwapShort/365);
                    }
                    else if (InstrProperties.SwapType == CommissionType.money)
                    {
                        swapLongPips = InstrProperties.SwapLong/(InstrProperties.Point*InstrProperties.LotSize);
                        swapShortPips = InstrProperties.SwapShort/(InstrProperties.Point*InstrProperties.LotSize);
                    }

                    if (position.PosDir == PosDirection.Long)
                    {
                        position.PosPrice += InstrProperties.Point*days*swapLongPips;
                        position.Rollover = position.PosLots*days*swapLongPips;
                        position.MoneyRollover = position.PosLots*days*swapLongPips*InstrProperties.Point*
                                                 InstrProperties.LotSize/AccountExchangeRate(Close[bar - 1]);
                    }
                    else
                    {
                        position.PosPrice += InstrProperties.Point*days*swapShortPips;
                        position.Rollover = -position.PosLots*days*swapShortPips;
                        position.MoneyRollover = -position.PosLots*days*swapShortPips*InstrProperties.Point*
                                                 InstrProperties.LotSize/AccountExchangeRate(Close[bar - 1]);
                    }
                }

                if (position.PosDir == PosDirection.Long)
                {
                    position.FloatingPL = position.PosLots*(Close[bar] - position.PosPrice)/InstrProperties.Point;
                    position.MoneyFloatingPL = position.PosLots*(Close[bar] - position.PosPrice)*InstrProperties.LotSize/
                                               AccountExchangeRate(Close[bar]);
                }
                else
                {
                    position.FloatingPL = position.PosLots*(position.PosPrice - Close[bar])/InstrProperties.Point;
                    position.MoneyFloatingPL = position.PosLots*(position.PosPrice - Close[bar])*InstrProperties.LotSize/
                                               AccountExchangeRate(Close[bar]);
                }

                position.PosNumb = _totalPositions;
                position.Transaction = Transaction.Transfer;
                position.RequiredMargin = RequiredMargin(position.PosLots, bar);
                position.Spread = 0;
                position.Commission = 0;
                position.Slippage = 0;
                position.ProfitLoss = 0;
                position.Equity = position.Balance + position.FloatingPL;
                position.MoneySpread = 0;
                position.MoneyCommission = 0;
                position.MoneySlippage = 0;
                position.MoneyProfitLoss = 0;
                position.MoneyEquity = position.MoneyBalance + position.MoneyFloatingPL;

                _posCoord[_totalPositions].Bar = bar;
                _posCoord[_totalPositions].Pos = sessionPosition;
                _session[bar].Positions++;
                _totalPositions++;

                // Saves the Trailing Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop" &&
                    _session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*
                                       InstrProperties.Point;
                    double stop = position.FormOrdPrice +
                                  (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }

                // Saves the Trailing Stop Limit price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop Limit" &&
                    _session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*
                                       InstrProperties.Point;
                    double stop = position.FormOrdPrice +
                                  (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }

                // Saves the ATR Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "ATR Stop" &&
                    _session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                    double stop = position.FormOrdPrice +
                                  (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1] = stop;
                }

                // Saves the Account Percent Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Account Percent Stop" &&
                    _session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*
                                        MoneyBalance(bar - 1)/(100*position.PosLots);
                    double deltaStop = Math.Max(MoneyToPips(deltaMoney, bar), 5)*InstrProperties.Point;
                    double stop = position.FormOrdPrice +
                                  (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }
            }
            else
            {
                // When there is no position transfer the old balance and equity
                _session[bar].Summary.Balance = _session[bar - 1].Summary.Balance;
                _session[bar].Summary.Equity = _session[bar - 1].Summary.Equity;
                _session[bar].Summary.MoneyBalance = _session[bar - 1].Summary.MoneyBalance;
                _session[bar].Summary.MoneyEquity = _session[bar - 1].Summary.MoneyEquity;
            }

            // Transfer all confirmed orders
            for (int iOrd = 0; iOrd < _session[bar - 1].Orders; iOrd++)
                if (_session[bar - 1].Order[iOrd].OrdStatus == OrderStatus.Confirmed)
                {
                    int iSessionOrder = _session[bar].Orders;
                    Order order = _session[bar].Order[iSessionOrder] = _session[bar - 1].Order[iOrd].Copy();
                    _ordCoord[order.OrdNumb].Bar = bar;
                    _ordCoord[order.OrdNumb].Ord = iSessionOrder;
                    _session[bar].Orders++;
                }
        }

        /// <summary>
        /// Sets an entry order
        /// </summary>
        private static void SetEntryOrders(int bar, double price, PosDirection posDir, double lots)
        {
            if (lots < 0.005)
                return; // This is a manner of cancellation an order.

            const int ifOrder = 0;
            const int toPos = 0;
            string note = Language.T("Entry Order");

            if (posDir == PosDirection.Long)
            {
                if (_openStrPriceType == StrategyPriceType.Open || _openStrPriceType == StrategyPriceType.Close)
                    OrdBuyMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else if (price > Open[bar])
                    OrdBuyStop(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else if (price < Open[bar])
                    OrdBuyLimit(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else
                    OrdBuyMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
            }
            else
            {
                if (_openStrPriceType == StrategyPriceType.Open || _openStrPriceType == StrategyPriceType.Close)
                    OrdSellMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else if (price < Open[bar])
                    OrdSellStop(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else if (price > Open[bar])
                    OrdSellLimit(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
                else
                    OrdSellMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, note);
            }
        }

        /// <summary>
        /// Sets an exit order
        /// </summary>
        private static void SetExitOrders(int bar, double priceStopLong, double priceStopShort)
        {
            // When there is a Long Position we send a Stop Order to it
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                double lots = _session[bar].Summary.PosLots;
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                string note = Language.T("Exit Order to position") + " " + (toPos + 1);

                if (_closeStrPriceType == StrategyPriceType.Close)
                    OrdSellMarket(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                    // The Stop Price can't be higher from the bar's opening price
                else if (priceStopLong < Open[bar])
                    OrdSellStop(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellMarket(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // When there is a Short Position we send a Stop Order to it
            if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                double lots = _session[bar].Summary.PosLots;
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                string note = Language.T("Exit Order to position") + " " + (toPos + 1);

                if (_closeStrPriceType == StrategyPriceType.Close)
                    OrdBuyMarket(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                    // The Stop Price can't be lower from the bar's opening price
                else if (priceStopShort > Open[bar])
                    OrdBuyStop(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyMarket(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // We send IfOrder Order Stop for each Entry Order
            for (int iOrd = 0; iOrd < _session[bar].Orders; iOrd++)
            {
                Order entryOrder = _session[bar].Order[iOrd];
                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    string note = Language.T("Exit Order to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                        OrdSellStop(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                }
            }
        }

        /// <summary>
        /// Checks the slots for a permission to open a position.
        /// If there are no filters that forbid it, sets the entry orders.
        /// </summary>
        private static void AnalyseEntry(int bar)
        {
            // Do not send entry order when we are not on time
            if (_openTimeExec == ExecutionTime.AtBarOpening &&
                Strategy.Slot[Strategy.OpenSlot].Component[0].Value[bar] < 0.5)
                return;

            // Determining of the buy/sell entry price
            double openLongPrice = 0;
            double openShortPrice = 0;
            for (int comp = 0; comp < Strategy.Slot[Strategy.OpenSlot].Component.Length; comp++)
            {
                IndComponentType compType = Strategy.Slot[Strategy.OpenSlot].Component[comp].DataType;

                if (compType == IndComponentType.OpenLongPrice)
                    openLongPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    openShortPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    openLongPrice = openShortPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
            }

            // Decide whether to open
            bool canOpenLong = openLongPrice > InstrProperties.Point;
            bool canOpenShort = openShortPrice > InstrProperties.Point;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in _openingLogicGroups)
                {
                    bool groupOpenLong = canOpenLong;
                    bool groupOpenShort = canOpenShort;

                    EntryLogicConditions(bar, group, openLongPrice, openShortPrice, ref groupOpenLong,
                                         ref groupOpenShort);

                    _groupsAllowLong[group] = groupOpenLong;
                    _groupsAllowShort[group] = groupOpenShort;
                }

                bool groupLongEntry = false;
                foreach (var groupLong in _groupsAllowLong)
                    if ((_groupsAllowLong.Count > 1 && groupLong.Key != "All") || _groupsAllowLong.Count == 1)
                        groupLongEntry = groupLongEntry || groupLong.Value;

                bool groupShortEntry = false;
                foreach (var groupShort in _groupsAllowShort)
                    if ((_groupsAllowShort.Count > 1 && groupShort.Key != "All") || _groupsAllowShort.Count == 1)
                        groupShortEntry = groupShortEntry || groupShort.Value;

                canOpenLong = canOpenLong && groupLongEntry && _groupsAllowLong["All"];
                canOpenShort = canOpenShort && groupShortEntry && _groupsAllowShort["All"];
            }
            else
            {
                EntryLogicConditions(bar, "A", openLongPrice, openShortPrice, ref canOpenLong, ref canOpenShort);
            }

            if (canOpenLong && canOpenShort && Math.Abs(openLongPrice - openShortPrice) < _micron)
            {
                _session[bar].BacktestEval = BacktestEval.Ambiguous;
            }
            else
            {
                if (canOpenLong)
                    SetEntryOrders(bar, openLongPrice, PosDirection.Long, TradingSize(Strategy.EntryLots, bar));
                if (canOpenShort)
                    SetEntryOrders(bar, openShortPrice, PosDirection.Short, TradingSize(Strategy.EntryLots, bar));
            }
        }

        /// <summary>
        /// Checks if the opening logic conditions allow long or short entry.
        /// </summary>
        private static void EntryLogicConditions(int bar, string group, double buyPrice, double sellPrice, ref bool canOpenLong, ref bool canOpenShort)
        {
            for (int slot = 0; slot < Strategy.CloseSlot + 1; slot++)
            {
                if (Configs.UseLogicalGroups && Strategy.Slot[slot].LogicalGroup != group &&
                    Strategy.Slot[slot].LogicalGroup != "All")
                    continue;

                foreach (IndicatorComp component in Strategy.Slot[slot].Component)
                {
                    if (component.DataType == IndComponentType.AllowOpenLong && component.Value[bar] < 0.5)
                        canOpenLong = false;

                    if (component.DataType == IndComponentType.AllowOpenShort && component.Value[bar] < 0.5)
                        canOpenShort = false;

                    if (component.PosPriceDependence == PositionPriceDependence.None) continue;

                    double indVal = component.Value[bar - component.UsePreviousBar];
                    switch (component.PosPriceDependence)
                    {
                        case PositionPriceDependence.PriceBuyHigher:
                            canOpenLong = canOpenLong && buyPrice > indVal + _micron;
                            break;
                        case PositionPriceDependence.PriceBuyLower:
                            canOpenLong = canOpenLong && buyPrice < indVal - _micron;
                            break;
                        case PositionPriceDependence.PriceSellHigher:
                            canOpenShort = canOpenShort && sellPrice > indVal + _micron;
                            break;
                        case PositionPriceDependence.PriceSellLower:
                            canOpenShort = canOpenShort && sellPrice < indVal - _micron;
                            break;
                        case PositionPriceDependence.BuyHigherSellLower:
                            canOpenLong = canOpenLong && buyPrice > indVal + _micron;
                            canOpenShort = canOpenShort && sellPrice < indVal - _micron;
                            break;
                        case PositionPriceDependence.BuyLowerSelHigher:
                            canOpenLong = canOpenLong && buyPrice < indVal - _micron;
                            canOpenShort = canOpenShort && sellPrice > indVal + _micron;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the close orders for the indicated bar.
        /// </summary>
        private static void AnalyseExit(int bar)
        {
            if (_closeTimeExec == ExecutionTime.AtBarClosing &&
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] < 0.001)
                return;

            switch (Strategy.Slot[Strategy.CloseSlot].IndicatorName)
            {
                case "Account Percent Stop":
                    AnalyseAccountPercentStopExit(bar);
                    return;
                case "ATR Stop":
                    AnalyseATRStopExit(bar);
                    return;
                case "Stop Limit":
                    AnalyseStopLimitExit(bar);
                    return;
                case "Stop Loss":
                    AnalyseStopLossExit(bar);
                    return;
                case "Take Profit":
                    AnalyseTakeProfitExit(bar);
                    return;
                case "Trailing Stop":
                    AnalyseTrailingStopExit(bar);
                    return;
                case "Trailing Stop Limit":
                    AnalyseTrailingStopLimitExit(bar);
                    return;
            }

            #region Indicator "N Bars Exit"  // KROG 

            // check have N Bars exit close filter
            if (_hasNBarsExit)
            {
                // check there is a position open
                if (_session[bar].Summary.PosDir == PosDirection.Long ||
                    _session[bar].Summary.PosDir == PosDirection.Short)
                {
                    var nExit = (int) Strategy.Slot[_slotNBarsExit].IndParam.NumParam[0].Value;
                    int posDuration = bar - _session[bar].Summary.OpeningBar;

                    // check if N Bars should close; else set Component Value to 0
                    Strategy.Slot[_slotNBarsExit].Component[0].Value[bar] = posDuration >= nExit ? 1 : 0;
                }
                else // if no position, set to zero
                    Strategy.Slot[_slotNBarsExit].Component[0].Value[bar] = 0;
            }

            #endregion

            // Searching the components to find the exit price for a long position.
            double priceExitLong = 0;
            foreach (IndicatorComp indComp in Strategy.Slot[Strategy.CloseSlot].Component)
                if (indComp.DataType == IndComponentType.CloseLongPrice ||
                    indComp.DataType == IndComponentType.ClosePrice ||
                    indComp.DataType == IndComponentType.OpenClosePrice)
                {
                    priceExitLong = indComp.Value[bar];
                    break;
                }

            // Searching the components to find the exit price for a short position.
            double priceExitShort = 0;
            foreach (IndicatorComp indComp in Strategy.Slot[Strategy.CloseSlot].Component)
                if (indComp.DataType == IndComponentType.CloseShortPrice ||
                    indComp.DataType == IndComponentType.ClosePrice ||
                    indComp.DataType == IndComponentType.OpenClosePrice)
                {
                    priceExitShort = indComp.Value[bar];
                    break;
                }

            if (Strategy.CloseFilters == 0)
            {
                SetExitOrders(bar, priceExitLong, priceExitShort);
                return;
            }

            // If we do not have a position we do not have anything to close.
            if (_session[bar].Summary.PosDir != PosDirection.Long &&
                _session[bar].Summary.PosDir != PosDirection.Short)
                return;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in _closingLogicGroups)
                {
                    bool isGroupAllowExit = false;
                    for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots; slot++)
                    {
                        if (Strategy.Slot[slot].LogicalGroup == group || Strategy.Slot[slot].LogicalGroup == "all")
                        {
                            bool isSlotAllowExit = false;
                            foreach (IndicatorComp component in Strategy.Slot[slot].Component)
                            {
                                // We are searching the components for a permission to close the position.
                                if (component.Value[bar] < 0.001)
                                    continue;

                                if (component.DataType == IndComponentType.ForceClose ||
                                    component.DataType == IndComponentType.ForceCloseLong && _session[bar].Summary.PosDir == PosDirection.Long ||
                                    component.DataType == IndComponentType.ForceCloseShort && _session[bar].Summary.PosDir == PosDirection.Short)
                                {
                                    isSlotAllowExit = true;
                                    break;
                                }
                            }

                            if (!isSlotAllowExit)
                            {
                                isGroupAllowExit = false;
                                break;
                            }
                            isGroupAllowExit = true;
                        }
                    }

                    if (isGroupAllowExit)
                    {
                        SetExitOrders(bar, priceExitLong, priceExitShort);
                        break;
                    }
                }
            }
            else
            {
                bool stopSearching = false;
                for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots && !stopSearching; slot++)
                {
                    for (int comp = 0; comp < Strategy.Slot[slot].Component.Length && !stopSearching; comp++)
                    {
                        // We are searching the components for a permission to close the position.
                        if (Strategy.Slot[slot].Component[comp].Value[bar] < 0.001)
                            continue;

                        IndComponentType compDataType = Strategy.Slot[slot].Component[comp].DataType;

                        if (compDataType == IndComponentType.ForceClose ||
                            compDataType == IndComponentType.ForceCloseLong && _session[bar].Summary.PosDir == PosDirection.Long || 
                            compDataType == IndComponentType.ForceCloseShort && _session[bar].Summary.PosDir == PosDirection.Short)
                        {
                            SetExitOrders(bar, priceExitLong, priceExitShort);
                            stopSearching = true;
                        }
                    }
                }
            }
        }

        private static void AnalyseATRStopExit(int bar)
        {
            double deltaStop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar];

            // If there is a transferred position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1];

                if (stop > Open[bar])
                    stop = Open[bar];

                string note = Language.T("ATR Stop to position") + " " + (toPos + 1);
                OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar] = stop;
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1];

                if (stop < Open[bar])
                    stop = Open[bar];

                string note = Language.T("ATR Stop to position") + " " + (toPos + 1);
                OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar] = stop;
            }

            // If there is a new position, sends an ATR Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("ATR Stop to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("ATR Stop to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Open orders, sends an IfOrder ATR Stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    string note = Language.T("ATR Stop to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseStopLimitExit(int bar)
        {
            // If there is a position, sends a StopLimit Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice;
                double dLimit = _session[bar].Summary.FormOrdPrice;
                stop -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;
                dLimit += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value*InstrProperties.Point;
                string note = Language.T("Stop Limit to position") + " " + (toPos + 1);

                if (Open[bar] > dLimit && dLimit > Close[bar - 1] || Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice;
                double dLimit = _session[bar].Summary.FormOrdPrice;
                stop += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;
                dLimit -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value*InstrProperties.Point;
                string note = Language.T("Stop Limit to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1] || Close[bar - 1] > dLimit && dLimit > Open[bar])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int iOrd = 0; iOrd < _session[bar].Orders; iOrd++)
            {
                Order entryOrder = _session[bar].Order[iOrd];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    double dLimit = entryOrder.OrdPrice;
                    string note = Language.T("Stop Limit to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;
                        dLimit += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value*InstrProperties.Point;
                        OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;
                        dLimit -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value*InstrProperties.Point;
                        OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseStopLossExit(int bar)
        {
            // The stop is exactly n pips below the entry point (also when add, reduce, reverse)
            double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;

            // If there is a position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("Stop Loss to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("Stop Loss to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    string note = Language.T("Stop Loss to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseTakeProfitExit(int bar)
        {
            double dDeltaLimit = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;

            // If there is a position, sends a Limit Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double dLimit = _session[bar].Summary.FormOrdPrice + dDeltaLimit;
                string note = Language.T("Take Profit to position") + " " + (toPos + 1);

                if (Open[bar] > dLimit && dLimit > Close[bar - 1])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double dLimit = _session[bar].Summary.FormOrdPrice - dDeltaLimit;
                string note = Language.T("Take Profit to position") + " " + (toPos + 1);

                if (Close[bar - 1] > dLimit && dLimit > Open[bar])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Open orders, sends an IfOrder Limit for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double dLimit = entryOrder.OrdPrice;
                    string note = Language.T("Take Profit to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        dLimit += dDeltaLimit;
                        OrdSellLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        dLimit -= dDeltaLimit;
                        OrdBuyLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseTrailingStopExit(int bar)
        {
            double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;

            // If there is a transferred position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                // When the position is modified after the previous bar high
                // we do not modify the Trailing Stop
                int wayPointOrder = 0;
                int wayPointHigh = 0;
                for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                {
                    if (WayPoint(bar - 1, wayPoint).OrdNumb == _session[bar - 1].Summary.FormOrdNumb)
                        wayPointOrder = wayPoint;
                    if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.High)
                        wayPointHigh = wayPoint;
                }

                if (wayPointOrder < wayPointHigh && stop < High[bar - 1] - deltaStop)
                    stop = High[bar - 1] - deltaStop;

                if (stop > Open[bar])
                    stop = Open[bar];

                string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                // When the position is modified after the previous bar low
                // we do not modify the Trailing Stop
                int wayPointOrder = 0;
                int wayPointLow = 0;
                for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                {
                    if (WayPoint(bar - 1, wayPoint).OrdNumb == _session[bar - 1].Summary.FormOrdNumb)
                        wayPointOrder = wayPoint;
                    if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.Low)
                        wayPointLow = wayPoint;
                }

                if (wayPointOrder < wayPointLow && stop > Low[bar - 1] + deltaStop)
                    stop = Low[bar - 1] + deltaStop;

                if (stop < Open[bar])
                    stop = Open[bar];

                string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }

            // If there is a new position, sends a Trailing Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Open orders, sends an IfOrder Trailing Stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    string note = Language.T("Trailing Stop to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseTrailingStopLimitExit(int bar)
        {
            double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*InstrProperties.Point;
            double dDeltaLimit = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value*InstrProperties.Point;

            // If there is a transferred position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                double dLimit = _session[bar].Summary.FormOrdPrice + dDeltaLimit;

                // When the position is modified after the previous bar high
                // we do not modify the Trailing Stop
                int wayPointOrder = 0;
                int iWayPointHigh = 0;
                for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                {
                    if (WayPoint(bar - 1, wayPoint).OrdNumb == _session[bar - 1].Summary.FormOrdNumb)
                        wayPointOrder = wayPoint;
                    if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.High)
                        iWayPointHigh = wayPoint;
                }

                if (wayPointOrder < iWayPointHigh && stop < High[bar - 1] - deltaStop)
                    stop = High[bar - 1] - deltaStop;

                if (stop > Open[bar])
                    stop = Open[bar];

                string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                if (Open[bar] > dLimit && dLimit > Close[bar - 1] || Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close,
                                     OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                double dLimit = _session[bar].Summary.FormOrdPrice - dDeltaLimit;

                // When the position is modified after the previous bar low
                // we do not modify the Trailing Stop
                int wayPointOrder = 0;
                int wayPointLow = 0;
                for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                {
                    if (WayPoint(bar - 1, wayPoint).OrdNumb == _session[bar - 1].Summary.FormOrdNumb)
                        wayPointOrder = wayPoint;
                    if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.Low)
                        wayPointLow = wayPoint;
                }

                if (wayPointOrder < wayPointLow && stop > Low[bar - 1] + deltaStop)
                    stop = Low[bar - 1] + deltaStop;

                if (stop < Open[bar])
                    stop = Open[bar];

                string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                if (Open[bar] > stop && stop > Close[bar - 1] || Close[bar - 1] > dLimit && dLimit > Open[bar])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }

            // If there is a new position, sends a Trailing Stop Limit Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice - deltaStop;
                double dLimit = _session[bar].Summary.FormOrdPrice + dDeltaLimit;
                string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = _session[bar].Summary.FormOrdPrice + deltaStop;
                double dLimit = _session[bar].Summary.FormOrdPrice - dDeltaLimit;
                string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Open orders, sends an IfOrder Trailing Stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    double dLimit = entryOrder.OrdPrice;
                    string note = Language.T("Trailing Stop Limit to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= deltaStop;
                        dLimit += dDeltaLimit;
                        OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += deltaStop;
                        dLimit -= dDeltaLimit;
                        OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        private static void AnalyseAccountPercentStopExit(int bar)
        {
            // If there is a transferred position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                if (stop > Open[bar])
                    stop = Open[bar];

                string note = Language.T("Stop order to position") + " " + (toPos + 1);
                OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction == Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                if (stop < Open[bar])
                    stop = Open[bar];

                string note = Language.T("Stop order to position") + " " + (toPos + 1);
                OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
            }


            // If there is a new position, sends a Stop Order for it.
            if (_session[bar].Summary.PosDir == PosDirection.Long &&
                _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*MoneyBalance(bar)/
                                    (100*lots);
                double deltaStop = Math.Max(MoneyToPips(deltaMoney, bar), 5)*InstrProperties.Point;
                double stop = _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("Stop order to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short &&
                     _session[bar].Summary.Transaction != Transaction.Transfer)
            {
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*
                                    MoneyBalance(bar)/(100*lots);
                double deltaStop = Math.Max(MoneyToPips(deltaMoney, bar), 5)*InstrProperties.Point;
                double stop = _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("Stop order to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // If there are Open orders, sends an if order for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value*MoneyBalance(bar)/
                                        (100*lots);
                    double deltaStop = -Math.Max(MoneyToPips(deltaMoney, bar), 5)*InstrProperties.Point;
                    string note = Language.T("Stop Order to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop -= deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                    else
                    {
                        stop += deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    }
                }
            }
        }

        /// <summary>
        /// Sets Permanent Stop Loss close orders for the indicated bar
        /// </summary>
        private static void AnalysePermanentSLExit(int bar)
        {
            double deltaStop = Strategy.PermanentSL*InstrProperties.Point;

            // If there is a position, sends a Stop Order for it
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                // Sets Permanent S/L for a Long Position
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.PermanentSLType == PermanentProtectionType.Absolute
                                  ? _session[bar].Summary.AbsoluteSL
                                  : _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("Permanent S/L to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                // Sets Permanent S/L for a Short Position
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.PermanentSLType == PermanentProtectionType.Absolute
                                  ? _session[bar].Summary.AbsoluteSL
                                  : _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("Permanent S/L to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double ordPrice = entryOrder.OrdPrice;
                    string note = Language.T("Permanent S/L to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                        OrdSellStop(bar, ifOrder, toPos, lots, ordPrice - deltaStop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, ordPrice + deltaStop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                }
            }
        }

        /// <summary>
        /// Sets Permanent Take Profit close orders for the indicated bar
        /// </summary>
        private static void AnalysePermanentTPExit(int bar)
        {
            double deltaStop = Strategy.PermanentTP*InstrProperties.Point;

            // If there is a position, sends a Stop Order for it
            if (_session[bar].Summary.PosDir == PosDirection.Long)
            {
                // Sets Permanent T/P for a Long Position
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.PermanentTPType == PermanentProtectionType.Absolute
                                  ? _session[bar].Summary.AbsoluteTP
                                  : _session[bar].Summary.FormOrdPrice + deltaStop;
                string note = Language.T("Permanent T/P to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
            }
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
            {
                // Sets Permanent T/P for a Short Position
                const int ifOrder = 0;
                int toPos = _session[bar].Summary.PosNumb;
                double lots = _session[bar].Summary.PosLots;
                double stop = Strategy.PermanentTPType == PermanentProtectionType.Absolute
                                  ? _session[bar].Summary.AbsoluteTP
                                  : _session[bar].Summary.FormOrdPrice - deltaStop;
                string note = Language.T("Permanent T/P to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int ord = 0; ord < _session[bar].Orders; ord++)
            {
                Order entryOrder = _session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int ifOrder = entryOrder.OrdNumb;
                    const int toPos = 0;
                    double lots = entryOrder.OrdLots;
                    double stop = entryOrder.OrdPrice;
                    string note = Language.T("Permanent T/P to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop += deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                    }
                    else
                    {
                        stop -= deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                    }
                }
            }
        }

        /// <summary>
        /// Sets Break Even close order for the current position.
        /// </summary>
        private static bool SetBreakEvenExit(int bar, double price, int lastPosBreakEven)
        {
            // First cancel no executed Break Even exits if any.
            if (_session[bar].Summary.PosNumb > lastPosBreakEven)
            {
                for (int ord = 0; ord < Orders(bar); ord++)
                {
                    Order order = OrdFromNumb(OrdNumb(bar, ord));
                    if (order.OrdOrigin == OrderOrigin.BreakEven && order.OrdStatus != OrderStatus.Executed)
                    {
                        order.OrdStatus = OrderStatus.Cancelled;
                        _session[bar].Summary.IsBreakEvenActivated = false;
                    }
                }
            }

            double targetBreakEven = Strategy.BreakEven*InstrProperties.Point;

            // Check if Break Even has to be activated (if position has profit).
            if (!_session[bar].Summary.IsBreakEvenActivated)
            {
                if (_session[bar].Summary.PosDir == PosDirection.Long &&
                    price >= _session[bar].Summary.PosPrice + targetBreakEven ||
                    _session[bar].Summary.PosDir == PosDirection.Short &&
                    price <= _session[bar].Summary.PosPrice - targetBreakEven)
                    _session[bar].Summary.IsBreakEvenActivated = true;
                else
                    return false;
            }

            // Set Break Even to the current position.
            const int ifOrder = 0;
            int toPos = _session[bar].Summary.PosNumb;
            double lots = _session[bar].Summary.PosLots;
            double stop = _session[bar].Summary.PosPrice;
            string note = Language.T("Break Even to position") + " " + (toPos + 1);

            if (_session[bar].Summary.PosDir == PosDirection.Long)
                OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.BreakEven, note);
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
                OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.BreakEven, note);

            return true;
        }

        /// <summary>
        /// Calculates at what price to activate Break Even and sends a fictive order.
        /// </summary>
        private static void SetBreakEvenActivation(int bar)
        {
            double price = 0;
            double targetBreakEven = Strategy.BreakEven*InstrProperties.Point;

            if (_session[bar].Summary.PosDir == PosDirection.Long)
                price = _session[bar].Summary.PosPrice + targetBreakEven;

            if (_session[bar].Summary.PosDir == PosDirection.Short)
                price = _session[bar].Summary.PosPrice - targetBreakEven;

            const int ifOrder = 0;
            int toPos = _session[bar].Summary.PosNumb;
            const double lots = 0;
            string note = Language.T("Break Even activation to position") + " " + (toPos + 1);

            if (_session[bar].Summary.PosDir == PosDirection.Long)
                OrdSellStop(bar, ifOrder, toPos, lots, price, OrderSender.Close, OrderOrigin.BreakEvenActivation, note);
            else if (_session[bar].Summary.PosDir == PosDirection.Short)
                OrdBuyStop(bar, ifOrder, toPos, lots, price, OrderSender.Close, OrderOrigin.BreakEvenActivation, note);
        }

        /// <summary>
        /// The main calculating cycle
        /// </summary>
        private static void Calculation()
        {
            ResetStart();

            if (_closeStrPriceType == StrategyPriceType.CloseAndReverce)
            {
                // Close and Reverse.
                switch (_openStrPriceType)
                {
                    case StrategyPriceType.Open:
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            ExecuteEntryAtOpeningPrice(bar);
                            CancelNoexecutedEntryOrders(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            CancelNoexecutedExitOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                        break;
                    case StrategyPriceType.Close:
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            AnalyseEntry(bar);
                            ExecuteEntryAtClosingPrice(bar);
                            CancelInvalidOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                        break;
                    default:
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            CancelInvalidOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                        break;
                }

                ResetStop();

                return;
            }

            switch (_openStrPriceType)
            {
                case StrategyPriceType.Open:
                    if (_closeStrPriceType == StrategyPriceType.Close)
                    {
                        // Opening price - Closing price
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            ExecuteEntryAtOpeningPrice(bar);
                            CancelNoexecutedEntryOrders(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            CancelNoexecutedExitOrders(bar);
                            AnalyseExit(bar);
                            ExecuteExitAtClosingPrice(bar);
                            CancelNoexecutedExitOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                    }
                    else
                    {
                        // Opening price - Indicator
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            ExecuteEntryAtOpeningPrice(bar);
                            CancelNoexecutedEntryOrders(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            AnalyseExit(bar);
                            BarInterpolation(bar);
                            CancelInvalidOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                    }
                    break;
                case StrategyPriceType.Close:
                    if (_closeStrPriceType == StrategyPriceType.Close)
                    {
                        // Closing price - Closing price
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            AnalyseEntry(bar - 1);
                            ExecuteEntryAtClosingPrice(bar - 1);
                            CancelNoexecutedEntryOrders(bar - 1);
                            MarginCallCheckAtBarClosing(bar - 1);
                            _session[bar - 1].SetWayPoint(Close[bar - 1], WayPointType.Close);
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            CancelNoexecutedExitOrders(bar);
                            AnalyseExit(bar);
                            ExecuteExitAtClosingPrice(bar);
                            CancelNoexecutedExitOrders(bar);
                        }
                    }
                    else
                    {
                        // Closing price - Indicator
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            AnalyseEntry(bar - 1);
                            ExecuteEntryAtClosingPrice(bar - 1);
                            CancelNoexecutedEntryOrders(bar - 1);
                            MarginCallCheckAtBarClosing(bar - 1);
                            _session[bar - 1].SetWayPoint(Close[bar - 1], WayPointType.Close);
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            AnalyseExit(bar);
                            BarInterpolation(bar);
                            CancelInvalidOrders(bar);
                        }
                    }
                    break;
                default:
                    if (_closeStrPriceType == StrategyPriceType.Close)
                    {
                        // Indicator - Closing price
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            BarInterpolation(bar);
                            CancelInvalidOrders(bar);
                            AnalyseExit(bar);
                            ExecuteExitAtClosingPrice(bar);
                            CancelNoexecutedExitOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                    }
                    else
                    {
                        // Indicator - Indicator
                        for (int bar = FirstBar; bar < Bars; bar++)
                        {
                            TransferFromPreviousBar(bar);
                            if (FastCalculating(bar)) break;
                            _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                            AnalyseEntry(bar);
                            if (Strategy.UsePermanentSL)
                                AnalysePermanentSLExit(bar);
                            if (Strategy.UsePermanentTP)
                                AnalysePermanentTPExit(bar);
                            AnalyseExit(bar);
                            BarInterpolation(bar);
                            CancelInvalidOrders(bar);
                            MarginCallCheckAtBarClosing(bar);
                            _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Performs an intrabar scanning.
        /// </summary>
        public static void Scan()
        {
            _isScanning = true;
            Calculation();
            IsScanPerformed = true;
            CalculateAccountStats();
            _isScanning = false;
        }

        /// <summary>
        /// Calculate strategy
        /// </summary>
        public static void Calculate()
        {
            if (Configs.Autoscan && (IsIntrabarData || Configs.UseTickData && IsTickData))
                Scan();
            else
                Calculation();
        }

        /// <summary>
        /// Calculate statistics
        /// </summary>
        private static bool FastCalculating(int startBar)
        {
            bool isFastCalc = false;

            if (_session[startBar].Positions != 0)
                return false;

            if (Configs.TradeUntilMarginCall &&
                _session[startBar].Summary.FreeMargin <
                RequiredMargin(TradingSize(Strategy.EntryLots, startBar), startBar) ||
                _session[startBar].Summary.FreeMargin < -1000000)
            {
                for (int bar = startBar; bar < Bars; bar++)
                {
                    _session[bar].Summary.Balance = _session[bar - 1].Summary.Balance;
                    _session[bar].Summary.Equity = _session[bar - 1].Summary.Equity;
                    _session[bar].Summary.MoneyBalance = _session[bar - 1].Summary.MoneyBalance;
                    _session[bar].Summary.MoneyEquity = _session[bar - 1].Summary.MoneyEquity;

                    _session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                    ArrangeBarsHighLow(bar);
                    _session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                }

                isFastCalc = true;
            }

            return isFastCalc;
        }

        /// <summary>
        /// Arranges the order of hitting the bar's Top and Bottom.
        /// </summary>
        private static void ArrangeBarsHighLow(int bar)
        {
            double low = Low[bar];
            double high = High[bar];
            bool isTopFirst = false;
            bool isOrderFound = false;

            if (_isScanning && IntraBarsPeriods[bar] != Period)
            {
                for (int b = 0; b < IntraBarBars[bar]; b++)
                {
                    if (IntraBarData[bar][b].High + _micron > high)
                    {
                        // Top found
                        isTopFirst = true;
                        isOrderFound = true;
                    }

                    if (IntraBarData[bar][b].Low - _micron < low)
                    {
                        // Bottom found
                        if (isOrderFound)
                        {
                            // Top and Bottom into the same intrabar
                            isOrderFound = false;
                            break;
                        }
                        isOrderFound = true;
                    }

                    if (isOrderFound)
                        break;
                }
            }

            if (!_isScanning || !isOrderFound)
            {
                isTopFirst = Open[bar] > Close[bar];
            }

            if (isTopFirst)
            {
                _session[bar].SetWayPoint(high, WayPointType.High);
                _session[bar].SetWayPoint(low, WayPointType.Low);
            }
            else
            {
                _session[bar].SetWayPoint(low, WayPointType.Low);
                _session[bar].SetWayPoint(high, WayPointType.High);
            }
        }
    }
}