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
using System.Globalization;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Backtester
    /// </summary>
    public partial class Backtester
    {
        private static double maxBalance;
        private static double minBalance;
        private static double maxDrawdown;
        private static double maxEquity;
        private static double minEquity;
        private static double maxEquityDrawdown;
        private static double grossProfit;
        private static double grossLoss;
        private static double minMoneyBalance;
        private static int[] balanceDrawdown;
        private static int[] equityDrawdown;

        private static int barsInPosition;
        private static int totalTrades;

        private static DateTime maxBalanceDate;
        private static DateTime minBalanceDate;
        private static DateTime maxMoneyBalanceDate;
        private static DateTime minMoneyBalanceDate;
        private static double maxMoneyDrawdownPercent;
        private static DateTime maxDrawdownDate;
        private static DateTime maxMoneyDrawdownDate;

        static Backtester()
        {
            AdditionalStatsValueShort = new string[0];
            AdditionalStatsValueLong = new string[0];
            AdditionalStatsValueTotal = new string[0];
            AdditionalStatsParamName = new string[0];
            InterpolationMethod = InterpolationMethod.Pessimistic;
            AccountStatsFlags = new bool[0];
            AccountStatsValue = new string[0];
            AccountStatsParam = new string[0];
        }

        /// <summary>
        ///     Gets the account balance in pips
        /// </summary>
        public static int NetBalance
        {
            get { return Balance(Bars - 1); }
        }

        /// <summary>
        ///     Gets the max balance in pips
        /// </summary>
        public static int MaxBalance
        {
            get { return (int) Math.Round(maxBalance); }
        }

        /// <summary>
        ///     Gets the min balance in pips
        /// </summary>
        public static int MinBalance
        {
            get { return (int) Math.Round(minBalance); }
        }

        /// <summary>
        ///     Gets the account balance in currency
        /// </summary>
        public static double NetMoneyBalance
        {
            get { return MoneyBalance(Bars - 1); }
        }

        /// <summary>
        ///     Gets the max balance in currency
        /// </summary>
        public static double MaxMoneyBalance { get; private set; }

        /// <summary>
        ///     Gets the min balance in currency
        /// </summary>
        public static double MinMoneyBalance
        {
            get { return minMoneyBalance; }
        }

        /// <summary>
        ///     Gets the max equity
        /// </summary>
        public static int MaxEquity
        {
            get { return (int) Math.Round(maxEquity); }
        }

        /// <summary>
        ///     Gets the min equity in pips
        /// </summary>
        public static int MinEquity
        {
            get { return (int) Math.Round(minEquity); }
        }

        /// <summary>
        ///     Gets the max Equity in currency
        /// </summary>
        public static double MaxMoneyEquity { get; private set; }

        /// <summary>
        ///     Gets the min Equity in currency
        /// </summary>
        public static double MinMoneyEquity { get; private set; }

        /// <summary>
        ///     Gets the maximum drawdown in the account bill
        /// </summary>
        public static int MaxDrawdown
        {
            get { return (int) Math.Round(maxDrawdown); }
        }

        /// <summary>
        ///     Gets the maximum equity drawdown in the account bill
        /// </summary>
        public static int MaxEquityDrawdown
        {
            get { return (int) Math.Round(maxEquityDrawdown); }
        }

        /// <summary>
        ///     Gets the maximum money drawdown
        /// </summary>
        public static double MaxMoneyDrawdown { get; private set; }

        /// <summary>
        ///     Gets the maximum money equity drawdown
        /// </summary>
        public static double MaxMoneyEquityDrawdown { get; private set; }

        /// <summary>
        ///     The total earned pips
        /// </summary>
        public static int GrossProfit
        {
            get { return (int) Math.Round(grossProfit); }
        }

        /// <summary>
        ///     The total earned money
        /// </summary>
        public static double GrossMoneyProfit { get; private set; }

        /// <summary>
        ///     The total lost pips
        /// </summary>
        public static int GrossLoss
        {
            get { return (int) Math.Round(grossLoss); }
        }

        /// <summary>
        ///     The total lost money
        /// </summary>
        public static double GrossMoneyLoss { get; private set; }

        /// <summary>
        ///     Gets the count of executed orders
        /// </summary>
        public static int ExecutedOrders { get; private set; }

        /// <summary>
        ///     Gets the count of lots have been traded
        /// </summary>
        public static double TradedLots { get; private set; }

        /// <summary>
        ///     Gets the time in position in percents
        /// </summary>
        public static int TimeInPosition
        {
            get { return (int) Math.Round(100f*barsInPosition/(Bars - FirstBar)); }
        }

        /// <summary>
        ///     Gets the count of max consecutve losses.
        /// </summary>
        public static int MaxConsecutiveLosses { get; private set; }

        /// <summary>
        ///     Gets the count of sent orders
        /// </summary>
        public static int SentOrders { get; private set; }

        /// <summary>
        ///     Gets the Charged Spread
        /// </summary>
        public static double TotalChargedSpread { get; private set; }

        /// <summary>
        ///     Gets the Charged Spread in currency
        /// </summary>
        public static double TotalChargedMoneySpread { get; private set; }

        /// <summary>
        ///     Gets the Charged RollOver
        /// </summary>
        public static double TotalChargedRollOver { get; private set; }

        /// <summary>
        ///     Gets the Charged RollOver in currency
        /// </summary>
        public static double TotalChargedMoneyRollOver { get; private set; }

        /// <summary>
        ///     Gets the Charged Slippage
        /// </summary>
        public static double TotalChargedSlippage { get; private set; }

        /// <summary>
        ///     Gets the Charged Slippage in currency
        /// </summary>
        public static double TotalChargedMoneySlippage { get; private set; }

        /// <summary>
        ///     Gets the Charged Commission
        /// </summary>
        public static double TotalChargedCommission { get; private set; }

        /// <summary>
        ///     Gets the Charged Commission in currency
        /// </summary>
        public static double TotalChargedMoneyCommission { get; private set; }

        /// <summary>
        ///     Winning Trades
        /// </summary>
        public static int WinningTrades { get; private set; }

        /// <summary>
        ///     Losing Trades
        /// </summary>
        public static int LosingTrades { get; private set; }

        /// <summary>
        ///     Win / Loss ratio
        /// </summary>
        public static double WinLossRatio { get; private set; }

        /// <summary>
        ///     Money Equity Percent Drawdown
        /// </summary>
        public static double MoneyEquityPercentDrawdown { get; private set; }

        /// <summary>
        ///     Equity Percent Drawdown
        /// </summary>
        public static double EquityPercentDrawdown { get; private set; }

        /// <summary>
        ///     Returns the ambiguous calculated bars
        /// </summary>
        public static int AmbiguousBars { get; private set; }

        /// <summary>
        ///     Was the intrabar scanning performed
        /// </summary>
        public static bool IsScanPerformed { get; private set; }

        /// <summary>
        ///     Margin Call Bar
        /// </summary>
        public static int MarginCallBar { get; private set; }

        /// <summary>
        ///     Gets the number of days tested.
        /// </summary>
        public static int TestedDays { get; private set; }

        /// <summary>
        ///     Gets the profit per tested day.
        /// </summary>
        public static int ProfitPerDay
        {
            get { return TestedDays > 0 ? Balance(Bars - 1)/TestedDays : 0; }
        }

        /// <summary>
        ///     Gets the profit per tested day in currency.
        /// </summary>
        public static double MoneyProfitPerDay
        {
            get { return TestedDays > 0 ? (MoneyBalance(Bars - 1) - Configs.InitialAccount)/TestedDays : 0; }
        }

        /// <summary>
        ///     Gets the account stats parameters
        /// </summary>
        public static string[] AccountStatsParam { get; private set; }

        /// <summary>
        ///     Gets the account stats values
        /// </summary>
        public static string[] AccountStatsValue { get; private set; }

        /// <summary>
        ///     Gets the account stats flags
        /// </summary>
        public static bool[] AccountStatsFlags { get; private set; }

        /// <summary>
        ///     Returns the Balance Drawdown in pips
        /// </summary>
        public static int BalanceDrawdown(int bar)
        {
            return balanceDrawdown[bar];
        }

        /// <summary>
        ///     Returns the Equity Drawdown in pips
        /// </summary>
        public static int EquityDrawdown(int bar)
        {
            return equityDrawdown[bar];
        }

        /// <summary>
        ///     Returns the Balance Drawdown in currency
        /// </summary>
        public static double MoneyBalanceDrawdown(int bar)
        {
            return balanceDrawdown[bar]*InstrProperties.Point*InstrProperties.LotSize/AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        ///     Returns the Equity Drawdown in currency.
        /// </summary>
        public static double MoneyEquityDrawdown(int bar)
        {
            return equityDrawdown[bar]*InstrProperties.Point*InstrProperties.LotSize/AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        ///     Calculates the account statistics.
        /// </summary>
        public static void CalculateAccountStats()
        {
            maxBalance = 0;
            minBalance = 0;
            maxEquity = 0;
            minEquity = 0;
            maxEquityDrawdown = 0;
            maxDrawdown = 0;
            MaxConsecutiveLosses = 0;

            MaxMoneyBalance = Configs.InitialAccount;
            minMoneyBalance = Configs.InitialAccount;
            MaxMoneyEquity = Configs.InitialAccount;
            MinMoneyEquity = Configs.InitialAccount;
            MaxMoneyEquityDrawdown = 0;
            MaxMoneyDrawdown = 0;

            barsInPosition = 0;
            grossProfit = 0;
            grossLoss = 0;
            GrossMoneyProfit = 0;
            GrossMoneyLoss = 0;
            TotalChargedSpread = 0;
            TotalChargedRollOver = 0;
            TotalChargedCommission = 0;
            TotalChargedSlippage = 0;
            TotalChargedMoneySpread = 0;
            TotalChargedMoneyRollOver = 0;
            TotalChargedMoneyCommission = 0;
            TotalChargedMoneySlippage = 0;
            AmbiguousBars = 0;
            balanceDrawdown = new int[Bars];
            equityDrawdown = new int[Bars];

            maxBalanceDate = Time[0];
            minBalanceDate = Time[0];
            maxMoneyBalanceDate = Time[0];
            minMoneyBalanceDate = Time[0];
            maxDrawdownDate = Time[0];
            maxMoneyDrawdownDate = Time[0];

            EquityPercentDrawdown = 100;
            maxMoneyDrawdownPercent = 0;
            MoneyEquityPercentDrawdown = 0;
            WinLossRatio = 0;

            WinningTrades = 0;
            LosingTrades = 0;
            totalTrades = 0;
            TestedDays = 0;

            for (int bar = FirstBar; bar < Bars; bar++)
            {
                // Balance
                double balance = session[bar].Summary.Balance;
                if (balance > maxBalance)
                {
                    maxBalance = balance;
                    maxBalanceDate = Time[bar];
                }
                if (balance < minBalance)
                {
                    minBalance = balance;
                    minBalanceDate = Time[bar];
                }

                // Money Balance
                double moneyBalance = session[bar].Summary.MoneyBalance;
                if (moneyBalance > MaxMoneyBalance)
                {
                    MaxMoneyBalance = moneyBalance;
                    maxMoneyBalanceDate = Time[bar];
                }
                if (moneyBalance < minMoneyBalance)
                {
                    minMoneyBalance = moneyBalance;
                    minMoneyBalanceDate = Time[bar];
                }

                // Equity
                double equity = session[bar].Summary.Equity;
                if (equity > maxEquity) maxEquity = equity;
                if (equity < minEquity) minEquity = equity;

                // Money Equity
                double moneyEquity = session[bar].Summary.MoneyEquity;
                if (moneyEquity > MaxMoneyEquity) MaxMoneyEquity = moneyEquity;
                if (moneyEquity < MinMoneyEquity) MinMoneyEquity = moneyEquity;

                // Maximum Drawdown
                if (maxBalance - balance > maxDrawdown)
                {
                    maxDrawdown = maxBalance - balance;
                    maxDrawdownDate = Time[bar];
                }

                // Maximum Equity Drawdown
                if (maxEquity - equity > maxEquityDrawdown)
                {
                    maxEquityDrawdown = maxEquity - equity;

                    // In percents
                    if (maxEquity > 0)
                        EquityPercentDrawdown = 100*(maxEquityDrawdown/maxEquity);
                }

                // Maximum Money Drawdown
                if (MaxMoneyBalance - MoneyBalance(bar) > MaxMoneyDrawdown)
                {
                    MaxMoneyDrawdown = MaxMoneyBalance - MoneyBalance(bar);
                    maxMoneyDrawdownPercent = 100*(MaxMoneyDrawdown/MaxMoneyBalance);
                    maxMoneyDrawdownDate = Time[bar];
                }

                // Maximum Money Equity Drawdown
                if (MaxMoneyEquity - MoneyEquity(bar) > MaxMoneyEquityDrawdown)
                {
                    MaxMoneyEquityDrawdown = MaxMoneyEquity - MoneyEquity(bar);

                    // Maximum Money Equity Drawdown in percents
                    if (100*MaxMoneyEquityDrawdown/MaxMoneyEquity > MoneyEquityPercentDrawdown)
                        MoneyEquityPercentDrawdown = 100*(MaxMoneyEquityDrawdown/MaxMoneyEquity);
                }

                // Drawdown
                balanceDrawdown[bar] = (int) Math.Round((maxBalance - balance));
                equityDrawdown[bar] = (int) Math.Round((maxEquity - equity));

                // Bars in position
                if (session[bar].Positions > 0)
                    barsInPosition++;

                // Bar interpolation evaluation
                if (session[bar].BacktestEval == BacktestEval.Ambiguous)
                {
                    AmbiguousBars++;
                }

                // Margin Call bar
                if (!Configs.TradeUntilMarginCall && MarginCallBar == 0 && session[bar].Summary.FreeMargin < 0)
                    MarginCallBar = bar;
            }

            for (int pos = 0; pos < PositionsTotal; pos++)
            {
                // Charged fees
                Position position = PosFromNumb(pos);
                TotalChargedSpread += position.Spread;
                TotalChargedRollOver += position.Rollover;
                TotalChargedCommission += position.Commission;
                TotalChargedSlippage += position.Slippage;
                TotalChargedMoneySpread += position.MoneySpread;
                TotalChargedMoneyRollOver += position.MoneyRollover;
                TotalChargedMoneyCommission += position.MoneyCommission;
                TotalChargedMoneySlippage += position.MoneySlippage;

                // Winning losing trades.
                if (position.Transaction == Transaction.Close ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (position.ProfitLoss > 0)
                    {
                        grossProfit += position.ProfitLoss;
                        GrossMoneyProfit += position.MoneyProfitLoss;
                        WinningTrades++;
                    }
                    else if (position.ProfitLoss < 0)
                    {
                        grossLoss += position.ProfitLoss;
                        GrossMoneyLoss += position.MoneyProfitLoss;
                        LosingTrades++;
                    }
                    totalTrades++;
                }
            }

            WinLossRatio = WinningTrades/Math.Max((LosingTrades + WinningTrades), 1.0);

            ExecutedOrders = 0;
            TradedLots = 0;
            for (int ord = 0; ord < SentOrders; ord++)
            {
                if (OrdFromNumb(ord).OrdStatus == OrderStatus.Executed)
                {
                    ExecutedOrders++;
                    TradedLots += OrdFromNumb(ord).OrdLots;
                }
            }

            TestedDays = (Time[Bars - 1] - Time[FirstBar]).Days;
            if (TestedDays < 1)
                TestedDays = 1;

            // Max consecutive loses
            int sum = 0;
            for (int pos = 0; pos < PositionsTotal; pos++)
            {
                Position position = PosFromNumb(pos);

                if (position.Transaction != Transaction.Close &&
                    position.Transaction != Transaction.Reduce &&
                    position.Transaction != Transaction.Reverse)
                    continue; // There is no profit/loss taken.

                if (position.ProfitLoss < micron)
                    sum++;
                else if (position.ProfitLoss > micron)
                    sum = 0;

                if (sum > MaxConsecutiveLosses)
                    MaxConsecutiveLosses = sum;
            }

            CalculateAdditionalStats();

            if (Configs.AccountInMoney)
                GenerateAccountStatsInMoney();
            else
                GenerateAccountStats();

        }

        /// <summary>
        ///     Generate the Account Statistics in currency.
        /// </summary>
        private static void GenerateAccountStatsInMoney()
        {
            AccountStatsParam = new[]
                {
                    Language.T("Intrabar scanning"),
                    Language.T("Interpolation method"),
                    Language.T("Ambiguous bars"),
                    Language.T("Profit per day"),
                    Language.T("Sharpe ratio"),
                    Language.T("Max consecutive losses"),
                    Language.T("Tested bars"),
                    Language.T("Initial account"),
                    Language.T("Account balance"),
                    Language.T("Minimum account"),
                    Language.T("Maximum account"),
                    Language.T("Maximum drawdown"),
                    Language.T("Max equity drawdown"),
                    Language.T("Max equity drawdown"),
                    Language.T("Gross profit"),
                    Language.T("Gross loss"),
                    Language.T("Sent orders"),
                    Language.T("Executed orders"),
                    Language.T("Traded lots"),
                    Language.T("Winning trades"),
                    Language.T("Losing trades"),
                    Language.T("Win/loss ratio"),
                    Language.T("Time in position"),
                    Language.T("Charged spread"),
                    Language.T("Charged rollover"),
                    Language.T("Charged commission"),
                    Language.T("Charged slippage"),
                    Language.T("Total charges"),
                    Language.T("Balance without charges"),
                    Language.T("Account exchange rate")
                };

            string unit = " " + Configs.AccountCurrency;

            AccountStatsValue = new string[AccountStatsParam.Length];
            int i = 0;
            AccountStatsValue[i++] = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[i++] = InterpolationMethodShortToString();
            AccountStatsValue[i++] = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = MoneyProfitPerDay.ToString("F2") + unit;
            AccountStatsValue[i++] = SharpeRatio.ToString("F2");
            AccountStatsValue[i++] = MaxConsecutiveLosses.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = (Bars - FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = Configs.InitialAccount.ToString("F2") + unit;
            AccountStatsValue[i++] = NetMoneyBalance.ToString("F2") + unit;
            AccountStatsValue[i++] = MinMoneyBalance.ToString("F2") + unit;
            AccountStatsValue[i++] = MaxMoneyBalance.ToString("F2") + unit;
            AccountStatsValue[i++] = MaxMoneyDrawdown.ToString("F2") + unit;
            AccountStatsValue[i++] = MaxMoneyEquityDrawdown.ToString("F2") + unit;
            AccountStatsValue[i++] = MoneyEquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[i++] = GrossMoneyProfit.ToString("F2") + unit;
            AccountStatsValue[i++] = GrossMoneyLoss.ToString("F2") + unit;
            AccountStatsValue[i++] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = TradedLots.ToString("F2");
            AccountStatsValue[i++] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = WinLossRatio.ToString("F2");
            AccountStatsValue[i++] = TimeInPosition + " %";
            AccountStatsValue[i++] = TotalChargedMoneySpread.ToString("F2") + unit;
            AccountStatsValue[i++] = TotalChargedMoneyRollOver.ToString("F2") + unit;
            AccountStatsValue[i++] = TotalChargedMoneyCommission.ToString("F2") + unit;
            AccountStatsValue[i++] = TotalChargedMoneySlippage.ToString("F2") + unit;
            AccountStatsValue[i++] =
                (TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission +
                 TotalChargedMoneySlippage).ToString("F2") + unit;
            AccountStatsValue[i++] =
                (NetMoneyBalance + TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission +
                 TotalChargedMoneySlippage).ToString("F2") + unit;

            if (InstrProperties.PriceIn == Configs.AccountCurrency)
                AccountStatsValue[i++] = Language.T("Not used");
            else if (InstrProperties.InstrType == InstrumetType.Forex && Symbol.StartsWith(Configs.AccountCurrency))
                AccountStatsValue[i++] = Language.T("Deal price");
            else if (Configs.AccountCurrency == "USD")
                AccountStatsValue[i++] = InstrProperties.RateToUSD.ToString("F4");
            else if (Configs.AccountCurrency == "EUR")
                AccountStatsValue[i++] = InstrProperties.RateToEUR.ToString("F4");

            AccountStatsFlags = new bool[AccountStatsParam.Length];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[5] = MaxConsecutiveLosses > 6;
            AccountStatsFlags[8] = NetMoneyBalance < Configs.InitialAccount;
            AccountStatsFlags[11] = MaxDrawdown > Configs.InitialAccount/2;
        }

        /// <summary>
        ///     Generate the Account Statistics in pips.
        /// </summary>
        private static void GenerateAccountStats()
        {
            AccountStatsParam = new[]
                {
                    Language.T("Intrabar scanning"),
                    Language.T("Interpolation method"),
                    Language.T("Ambiguous bars"),
                    Language.T("Profit per day"),
                    Language.T("Sharpe ratio"),
                    Language.T("Max consecutive losses"),
                    Language.T("Tested bars"),
                    Language.T("Account balance"),
                    Language.T("Minimum account"),
                    Language.T("Maximum account"),
                    Language.T("Maximum drawdown"),
                    Language.T("Max equity drawdown"),
                    Language.T("Max equity drawdown"),
                    Language.T("Gross profit"),
                    Language.T("Gross loss"),
                    Language.T("Sent orders"),
                    Language.T("Executed orders"),
                    Language.T("Traded lots"),
                    Language.T("Winning trades"),
                    Language.T("Losing trades"),
                    Language.T("Win/loss ratio"),
                    Language.T("Time in position"),
                    Language.T("Charged spread"),
                    Language.T("Charged rollover"),
                    Language.T("Charged commission"),
                    Language.T("Charged slippage"),
                    Language.T("Total charges"),
                    Language.T("Balance without charges")
                };

            string unit = " " + Language.T("pips");
            AccountStatsValue = new string[AccountStatsParam.Length];
            int i = 0;
            AccountStatsValue[i++] = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[i++] = InterpolationMethodShortToString();
            AccountStatsValue[i++] = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = ProfitPerDay + unit;
            AccountStatsValue[i++] = SharpeRatio.ToString("F2");
            AccountStatsValue[i++] = MaxConsecutiveLosses.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = (Bars - FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = NetBalance + unit;
            AccountStatsValue[i++] = MinBalance + unit;
            AccountStatsValue[i++] = MaxBalance + unit;
            AccountStatsValue[i++] = MaxDrawdown + unit;
            AccountStatsValue[i++] = MaxEquityDrawdown + unit;
            AccountStatsValue[i++] = EquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[i++] = GrossProfit + unit;
            AccountStatsValue[i++] = GrossLoss + unit;
            AccountStatsValue[i++] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = TradedLots.ToString("F2");
            AccountStatsValue[i++] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[i++] = ((float)WinningTrades / (WinningTrades + LosingTrades)).ToString("F2");
            AccountStatsValue[i++] = TimeInPosition + " %";
            AccountStatsValue[i++] = Math.Round(TotalChargedSpread) + unit;
            AccountStatsValue[i++] = Math.Round(TotalChargedRollOver) + unit;
            AccountStatsValue[i++] = Math.Round(TotalChargedCommission) + unit;
            AccountStatsValue[i++] = TotalChargedSlippage.ToString("F2") + unit;
            AccountStatsValue[i++] = Math.Round(TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;
            AccountStatsValue[i++] = Math.Round(NetBalance + TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;

            AccountStatsFlags = new bool[AccountStatsParam.Length];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[5] = MaxConsecutiveLosses > 6;
            AccountStatsFlags[7] = NetBalance < 0;
            AccountStatsFlags[10] = MaxDrawdown > 500;
        }

        /// <summary>
        ///     Calculates the required margin.
        /// </summary>
        public static double RequiredMargin(double lots, int bar)
        {
            double amount = lots*InstrProperties.LotSize;
            double exchangeRate = Close[bar]/AccountExchangeRate(Close[bar]);
            double requiredMargin = amount*exchangeRate/Configs.Leverage;

            return requiredMargin;
        }

        /// <summary>
        ///     Calculates the trading size in normalized lots.
        /// </summary>
        public static double TradingSize(double size, int bar)
        {
            if (Strategy.UseAccountPercentEntry)
            {
                double maxMargin = session[bar].Summary.MoneyEquity*size/100.0;
                double exchangeRate = Close[bar]/AccountExchangeRate(Close[bar]);
                size = maxMargin*Configs.Leverage/(exchangeRate*InstrProperties.LotSize);
            }

            return NormalizeEntryLots(size);
        }

        /// <summary>
        ///     Normalizes an entry order's size.
        /// </summary>
        private static double NormalizeEntryLots(double lots)
        {
            const double minlot = 0.01;
            double maxlot = Strategy.MaxOpenLots;
            const double lotstep = 0.01;

            if (lots <= 0)
                return (0);

            var steps = (int) Math.Round((lots - minlot)/lotstep);
            lots = minlot + steps*lotstep;

            if (lots <= minlot)
                return (minlot);

            if (lots >= maxlot)
                return (maxlot);

            return lots;
        }

        /// <summary>
        ///     Account Exchange Rate.
        /// </summary>
        public static double AccountExchangeRate(double price)
        {
            double exchangeRate = 0;

            if (InstrProperties.PriceIn == Configs.AccountCurrency)
                exchangeRate = 1;
            else if (InstrProperties.InstrType == InstrumetType.Forex && Symbol.StartsWith(Configs.AccountCurrency))
                exchangeRate = price;
            else if (Configs.AccountCurrency == "USD")
                exchangeRate = InstrProperties.RateToUSD;
            else if (Configs.AccountCurrency == "EUR")
                exchangeRate = InstrProperties.RateToEUR;

            return exchangeRate;
        }

        /// <summary>
        ///     Calculates the commission in pips.
        /// </summary>
        public static double Commission(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (InstrProperties.Commission < 0.00001)
                return 0;

            if (InstrProperties.CommissionTime == CommissionTime.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (InstrProperties.CommissionType == CommissionType.pips)
                commission = InstrProperties.Commission;

            else if (InstrProperties.CommissionType == CommissionType.percents)
            {
                commission = (price/InstrProperties.Point)*(InstrProperties.Commission/100);
                return commission;
            }

            else if (InstrProperties.CommissionType == CommissionType.money)
                commission = InstrProperties.Commission/(InstrProperties.Point*InstrProperties.LotSize);

            if (InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        ///     Calculates the commission in currency.
        /// </summary>
        public static double CommissionInMoney(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (InstrProperties.Commission < 0.00001)
                return 0;

            if (InstrProperties.CommissionTime == CommissionTime.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (InstrProperties.CommissionType == CommissionType.pips)
                commission = InstrProperties.Commission*InstrProperties.Point*InstrProperties.LotSize/
                             AccountExchangeRate(price);

            else if (InstrProperties.CommissionType == CommissionType.percents)
            {
                commission = lots*InstrProperties.LotSize*price*(InstrProperties.Commission/100)/
                             AccountExchangeRate(price);
                return commission;
            }

            else if (InstrProperties.CommissionType == CommissionType.money)
                commission = InstrProperties.Commission/AccountExchangeRate(price);

            if (InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        ///     Calculates the rollover fee in currency.
        /// </summary>
        public static double RolloverInMoney(PosDirection posDir, double lots, int daysRollover, double price)
        {
            double point = InstrProperties.Point;
            int lotSize = InstrProperties.LotSize;
            double swapLongPips = 0; // Swap long in pips
            double swapShortPips = 0; // Swap short in pips
            if (InstrProperties.SwapType == CommissionType.pips)
            {
                swapLongPips = InstrProperties.SwapLong;
                swapShortPips = InstrProperties.SwapShort;
            }
            else if (InstrProperties.SwapType == CommissionType.percents)
            {
                swapLongPips = (price/point)*(0.01*InstrProperties.SwapLong/365);
                swapShortPips = (price/point)*(0.01*InstrProperties.SwapShort/365);
            }
            else if (InstrProperties.SwapType == CommissionType.money)
            {
                swapLongPips = InstrProperties.SwapLong/(point*lotSize);
                swapShortPips = InstrProperties.SwapShort/(point*lotSize);
            }

            double rollover = lots*lotSize*(posDir == PosDirection.Long ? swapLongPips : -swapShortPips)*point*
                              daysRollover/AccountExchangeRate(price);

            return rollover;
        }

        /// <summary>
        ///     Converts pips to money.
        /// </summary>
        public static double PipsToMoney(double pips, int bar)
        {
            return pips*InstrProperties.Point*InstrProperties.LotSize/AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        ///     Converts money to pips.
        /// </summary>
        public static double MoneyToPips(double money, int bar)
        {
            return money*AccountExchangeRate(Close[bar])/(InstrProperties.Point*InstrProperties.LotSize);
        }
    }
}