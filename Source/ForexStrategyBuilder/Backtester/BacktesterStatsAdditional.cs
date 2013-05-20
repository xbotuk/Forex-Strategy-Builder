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
    ///     Class Backtester - Additional statistics
    /// </summary>
    public partial class Backtester
    {
        private static double[] longBalance;
        private static double[] shortBalance;
        private static double[] longMoneyBalance;
        private static double[] shortMoneyBalance;
        private static double maxLongBalance;
        private static double minLongBalance;
        private static double maxShortBalance;
        private static double minShortBalance;

        private static DateTime maxLongBalanceDate;
        private static DateTime minLongBalanceDate;
        private static DateTime maxShortBalanceDate;
        private static DateTime minShortBalanceDate;
        private static DateTime maxLongMoneyBalanceDate;
        private static DateTime minLongMoneyBalanceDate;
        private static DateTime maxShortMoneyBalanceDate;
        private static DateTime minShortMoneyBalanceDate;

        private static double grossLongProfit;
        private static double grossLongLoss;
        private static double grossShortProfit;
        private static double grossShortLoss;
        private static double grossLongMoneyProfit;
        private static double grossLongMoneyLoss;
        private static double grossShortMoneyProfit;
        private static double grossShortMoneyLoss;

        private static double maxLongDrawdown;
        private static double maxShortDrawdown;
        private static double maxLongMoneyDrawdown;
        private static double maxShortMoneyDrawdown;
        private static double maxLongMoneyDrawdownPercent;
        private static double maxShortMoneyDrawdownPercent;
        private static DateTime maxLongDrawdownDate;
        private static DateTime maxShortDrawdownDate;
        private static DateTime maxLongMoneyDrawdownDate;
        private static DateTime maxShortMoneyDrawdownDate;

        private static int barsWithLongPos;
        private static int barsWithShortPos;
        private static int barsWithPos;

        private static int winningLongTrades;
        private static int winningShortTrades;
        private static int losingLongTrades;
        private static int losingShortTrades;
        private static int totalLongTrades;
        private static int totalShortTrades;

        private static double maxLongWin;
        private static double maxShortWin;
        private static double maxLongMoneyWin;
        private static double maxShortMoneyWin;
        private static double maxLongLoss;
        private static double maxShortLoss;
        private static double maxLongMoneyLoss;
        private static double maxShortMoneyLoss;

        private static double ahprLong;
        private static double ahprShort;
        private static double ghprLong;
        private static double ghprShort;
        private static double sharpeRatioLong;
        private static double sharpeRatioShort;

        public static double AnnualizedProfit { get; private set; }
        public static double AnnualizedProfitPercent { get; private set; }
        public static double AvrgHoldingPeriodRet { get; private set; }
        public static double GeomHoldingPeriodRet { get; private set; }
        public static double ProfitFactor { get; private set; }
        public static double SharpeRatio { get; private set; }

        /// <summary>
        ///     Gets the Additional Stats Parameter Name.
        /// </summary>
        public static string[] AdditionalStatsParamName { get; private set; }

        /// <summary>
        ///     Gets the Additional Stats Value Long + Short.
        /// </summary>
        public static string[] AdditionalStatsValueTotal { get; private set; }

        /// <summary>
        ///     Gets the Additional Stats Value Long.
        /// </summary>
        public static string[] AdditionalStatsValueLong { get; private set; }

        /// <summary>
        ///     Gets the Additional Stats Value Short.
        /// </summary>
        public static string[] AdditionalStatsValueShort { get; private set; }

        /// <summary>
        ///     Gets the long balance in points.
        /// </summary>
        public static int NetLongBalance
        {
            get { return (int) Math.Round(longBalance[Bars - 1]); }
        }

        /// <summary>
        ///     Gets the short balance in points.
        /// </summary>
        public static int NetShortBalance
        {
            get { return (int) Math.Round(shortBalance[Bars - 1]); }
        }

        /// <summary>
        ///     Gets the max long balance in points.
        /// </summary>
        public static int MaxLongBalance
        {
            get { return (int) Math.Round(maxLongBalance); }
        }

        /// <summary>
        ///     Gets the max short balance in points.
        /// </summary>
        public static int MaxShortBalance
        {
            get { return (int) Math.Round(maxShortBalance); }
        }

        /// <summary>
        ///     Gets the min long balance in points.
        /// </summary>
        public static int MinLongBalance
        {
            get { return (int) Math.Round(minLongBalance); }
        }

        /// <summary>
        ///     Gets the min short balance in points.
        /// </summary>
        public static int MinShortBalance
        {
            get { return (int) Math.Round(minShortBalance); }
        }

        /// <summary>
        ///     Gets the long balance in money
        /// </summary>
        public static double NetLongMoneyBalance
        {
            get { return longMoneyBalance[Bars - 1]; }
        }

        /// <summary>
        ///     Gets the short balance in money.
        /// </summary>
        public static double NetShortMoneyBalance
        {
            get { return shortMoneyBalance[Bars - 1]; }
        }

        /// <summary>
        ///     Gets the max long balance in money.
        /// </summary>
        public static double MaxLongMoneyBalance { get; private set; }

        /// <summary>
        /// Gets deviation percent between red and green balances.
        /// </summary>
        public static double RedGreenBalanceDev
        {
            get { return  100 * Math.Abs(NetLongMoneyBalance - NetShortMoneyBalance) / NetMoneyBalance; }
        }

        /// <summary>
        ///     Gets the max short balance in money.
        /// </summary>
        public static double MaxShortMoneyBalance { get; private set; }

        /// <summary>
        ///     Gets the min long balance in money.
        /// </summary>
        public static double MinLongMoneyBalance { get; private set; }

        /// <summary>
        ///     Gets the min short balance in money.
        /// </summary>
        public static double MinShortMoneyBalance { get; private set; }

        /// <summary>
        ///     Returns the long balance at the end of the bar in points.
        /// </summary>
        public static int LongBalance(int bar)
        {
            return (int) Math.Round(longBalance[bar]);
        }

        /// <summary>
        ///     Returns the short balance at the end of the bar in points.
        /// </summary>
        public static int ShortBalance(int bar)
        {
            return (int) Math.Round(shortBalance[bar]);
        }

        /// <summary>
        ///     Returns the long balance at the end of the bar in money.
        /// </summary>
        public static double LongMoneyBalance(int bar)
        {
            return longMoneyBalance[bar];
        }

        /// <summary>
        ///     Returns the short balance at the end of the bar in money.
        /// </summary>
        public static double ShortMoneyBalance(int bar)
        {
            return shortMoneyBalance[bar];
        }

        /// <summary>
        ///     Calculates the values of the stats parameters.
        /// </summary>
        private static void CalculateAdditionalStats()
        {
            longBalance = new double[Bars];
            shortBalance = new double[Bars];
            longMoneyBalance = new double[Bars];
            shortMoneyBalance = new double[Bars];

            MaxLongMoneyBalance = Configs.InitialAccount;
            MinLongMoneyBalance = Configs.InitialAccount;
            MaxShortMoneyBalance = Configs.InitialAccount;
            MinShortMoneyBalance = Configs.InitialAccount;
            maxLongBalance = 0;
            minLongBalance = 0;
            maxShortBalance = 0;
            minShortBalance = 0;

            maxLongBalanceDate = Time[0];
            minLongBalanceDate = Time[0];
            maxShortBalanceDate = Time[0];
            minShortBalanceDate = Time[0];
            maxLongMoneyBalanceDate = Time[0];
            minLongMoneyBalanceDate = Time[0];
            maxShortMoneyBalanceDate = Time[0];
            minShortMoneyBalanceDate = Time[0];
            maxLongDrawdownDate = Time[0];
            maxShortDrawdownDate = Time[0];
            maxLongMoneyDrawdownDate = Time[0];
            maxShortMoneyDrawdownDate = Time[0];

            grossLongProfit = 0;
            grossLongLoss = 0;
            grossShortProfit = 0;
            grossShortLoss = 0;
            grossLongMoneyProfit = 0;
            grossLongMoneyLoss = 0;
            grossShortMoneyProfit = 0;
            grossShortMoneyLoss = 0;

            maxLongDrawdown = 0;
            maxShortDrawdown = 0;
            maxLongMoneyDrawdown = 0;
            maxShortMoneyDrawdown = 0;
            maxShortDrawdown = 0;
            maxLongMoneyDrawdown = 0;
            maxShortMoneyDrawdown = 0;
            maxLongMoneyDrawdownPercent = 0;
            maxShortMoneyDrawdownPercent = 0;

            barsWithPos = 0;
            barsWithLongPos = 0;
            barsWithShortPos = 0;

            winningLongTrades = 0;
            winningShortTrades = 0;
            losingLongTrades = 0;
            losingShortTrades = 0;

            totalLongTrades = 0;
            totalShortTrades = 0;

            maxLongWin = 0;
            maxShortWin = 0;
            maxLongMoneyWin = 0;
            maxShortMoneyWin = 0;
            maxLongLoss = 0;
            maxShortLoss = 0;
            maxLongMoneyLoss = 0;
            maxShortMoneyLoss = 0;

            for (int bar = 0; bar < FirstBar; bar++)
            {
                longBalance[bar] = 0;
                shortBalance[bar] = 0;
                longMoneyBalance[bar] = Configs.InitialAccount;
                shortMoneyBalance[bar] = Configs.InitialAccount;
            }

            for (int bar = FirstBar; bar < Bars; bar++)
            {
                longBalance[bar] = longBalance[bar - 1];
                shortBalance[bar] = shortBalance[bar - 1];
                longMoneyBalance[bar] = longMoneyBalance[bar - 1];
                shortMoneyBalance[bar] = shortMoneyBalance[bar - 1];

                bool isLong = false;
                bool isShort = false;
                for (int pos = 0; pos < Positions(bar); pos++)
                {
                    if (PosDir(bar, pos) == PosDirection.Long)
                        isLong = true;

                    if (PosDir(bar, pos) == PosDirection.Short)
                        isShort = true;

                    double positionProfitLoss = PosProfitLoss(bar, pos);
                    double positionMoneyProfitLoss = PosMoneyProfitLoss(bar, pos);

                    if (PosTransaction(bar, pos) == Transaction.Close ||
                        PosTransaction(bar, pos) == Transaction.Reduce ||
                        PosTransaction(bar, pos) == Transaction.Reverse)
                    {
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Sell)
                        {
                            // Closing long position
                            longBalance[bar] += positionProfitLoss;
                            longMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                grossLongProfit += positionProfitLoss;
                                grossLongMoneyProfit += positionMoneyProfitLoss;
                                winningLongTrades++;
                                if (positionProfitLoss > maxLongWin)
                                    maxLongWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > maxLongMoneyWin)
                                    maxLongMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                grossLongLoss += positionProfitLoss;
                                grossLongMoneyLoss += positionMoneyProfitLoss;
                                losingLongTrades++;
                                if (positionProfitLoss < maxLongLoss)
                                    maxLongLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < maxLongMoneyLoss)
                                    maxLongMoneyLoss = positionMoneyProfitLoss;
                            }

                            totalLongTrades++;
                        }
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Buy)
                        {
                            // Closing short position
                            shortBalance[bar] += positionProfitLoss;
                            shortMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                grossShortProfit += positionProfitLoss;
                                grossShortMoneyProfit += positionMoneyProfitLoss;
                                winningShortTrades++;
                                if (positionProfitLoss > maxShortWin)
                                    maxShortWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > maxShortMoneyWin)
                                    maxShortMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                grossShortLoss += positionProfitLoss;
                                grossShortMoneyLoss += positionMoneyProfitLoss;
                                losingShortTrades++;
                                if (positionProfitLoss < maxShortLoss)
                                    maxShortLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < maxShortMoneyLoss)
                                    maxShortMoneyLoss = positionMoneyProfitLoss;
                            }

                            totalShortTrades++;
                        }
                    }
                }

                barsWithPos += (isLong || isShort) ? 1 : 0;
                barsWithLongPos += isLong ? 1 : 0;
                barsWithShortPos += isShort ? 1 : 0;

                if (maxLongBalance < longBalance[bar])
                {
                    maxLongBalance = longBalance[bar];
                    maxLongBalanceDate = Time[bar];
                }
                if (minLongBalance > longBalance[bar])
                {
                    minLongBalance = longBalance[bar];
                    minLongBalanceDate = Time[bar];
                }
                if (maxShortBalance < shortBalance[bar])
                {
                    maxShortBalance = shortBalance[bar];
                    maxShortBalanceDate = Time[bar];
                }
                if (minShortBalance > shortBalance[bar])
                {
                    minShortBalance = shortBalance[bar];
                    minShortBalanceDate = Time[bar];
                }
                if (MaxLongMoneyBalance < longMoneyBalance[bar])
                {
                    MaxLongMoneyBalance = longMoneyBalance[bar];
                    maxLongMoneyBalanceDate = Time[bar];
                }
                if (MinLongMoneyBalance > longMoneyBalance[bar])
                {
                    MinLongMoneyBalance = longMoneyBalance[bar];
                    minLongMoneyBalanceDate = Time[bar];
                }
                if (MaxShortMoneyBalance < shortMoneyBalance[bar])
                {
                    MaxShortMoneyBalance = shortMoneyBalance[bar];
                    maxShortMoneyBalanceDate = Time[bar];
                }
                if (MinShortMoneyBalance > shortMoneyBalance[bar])
                {
                    MinShortMoneyBalance = shortMoneyBalance[bar];
                    minShortMoneyBalanceDate = Time[bar];
                }

                // Maximum Drawdown
                if (maxLongBalance - longBalance[bar] > maxLongDrawdown)
                {
                    maxLongDrawdown = maxLongBalance - longBalance[bar];
                    maxLongDrawdownDate = Time[bar];
                }

                if (MaxLongMoneyBalance - longMoneyBalance[bar] > maxLongMoneyDrawdown)
                {
                    maxLongMoneyDrawdown = MaxLongMoneyBalance - longMoneyBalance[bar];
                    maxLongMoneyDrawdownPercent = 100*maxLongMoneyDrawdown/MaxLongMoneyBalance;
                    maxLongMoneyDrawdownDate = Time[bar];
                }

                if (maxShortBalance - shortBalance[bar] > maxShortDrawdown)
                {
                    maxShortDrawdown = maxShortBalance - shortBalance[bar];
                    maxShortDrawdownDate = Time[bar];
                }

                if (MaxShortMoneyBalance - shortMoneyBalance[bar] > maxShortMoneyDrawdown)
                {
                    maxShortMoneyDrawdown = MaxShortMoneyBalance - shortMoneyBalance[bar];
                    maxShortMoneyDrawdownPercent = 100*maxShortMoneyDrawdown/MaxShortMoneyBalance;
                    maxShortMoneyDrawdownDate = Time[bar];
                }
            }

            // Holding period returns
            AvrgHoldingPeriodRet = 0;
            ahprLong = 0;
            ahprShort = 0;

            var hpr = new double[totalTrades];
            var hprLong = new double[totalLongTrades];
            var hprShort = new double[totalShortTrades];

            double totalHPR = 0;
            double totalHPRLong = 0;
            double totalHPRShort = 0;

            double startBalance = Configs.InitialAccount;
            double startBalanceLong = Configs.InitialAccount;
            double startBalanceShort = Configs.InitialAccount;

            int count = 0;
            int countL = 0;
            int countS = 0;

            for (int pos = 0; pos < PositionsTotal; pos++)
            {
                // Charged fees
                Position position = PosFromNumb(pos);
                // Winning losing trades.
                if (position.Transaction == Transaction.Close ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Sell)
                    {
                        // Closing long position
                        hprLong[countL] = 1 + position.MoneyProfitLoss/startBalanceLong;
                        totalHPRLong += hprLong[countL];
                        countL++;
                        startBalanceLong += position.MoneyProfitLoss;
                    }
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Buy)
                    {
                        // Closing short position
                        hprShort[countS] = 1 + position.MoneyProfitLoss/startBalanceShort;
                        totalHPRShort += hprShort[countS];
                        countS++;
                        startBalanceShort += position.MoneyProfitLoss;
                    }
                    hpr[count] = 1 + position.MoneyProfitLoss/startBalance;
                    totalHPR += hpr[count];
                    count++;
                    startBalance += position.MoneyProfitLoss;
                }
            }

            double averageHPR = totalHPR/totalTrades;
            double averageHPRLong = totalHPRLong/totalLongTrades;
            double averageHPRShort = totalHPRShort/totalShortTrades;

            AvrgHoldingPeriodRet = 100*(averageHPR - 1);
            ahprLong = 100*(averageHPRLong - 1);
            ahprShort = 100*(averageHPRShort - 1);

            GeomHoldingPeriodRet = 100*(Math.Pow((NetMoneyBalance/Configs.InitialAccount), (1f/totalTrades)) - 1);
            ghprLong = 100*(Math.Pow((NetLongMoneyBalance/Configs.InitialAccount), (1f/totalLongTrades)) - 1);
            ghprShort = 100*(Math.Pow((NetShortMoneyBalance/Configs.InitialAccount), (1f/totalShortTrades)) - 1);

            // Sharpe Ratio
            SharpeRatio = 0;
            sharpeRatioLong = 0;
            sharpeRatioShort = 0;

            double sumPow = 0;
            double sumPowLong = 0;
            double sumPowShort = 0;

            for (int i = 0; i < totalTrades; i++)
                sumPow += Math.Pow((hpr[i] - averageHPR), 2);
            for (int i = 0; i < totalLongTrades; i++)
                sumPowLong += Math.Pow((hprLong[i] - averageHPRLong), 2);
            for (int i = 0; i < totalShortTrades; i++)
                sumPowShort += Math.Pow((hprShort[i] - averageHPRShort), 2);

            double stDev = Math.Sqrt(sumPow/(totalTrades - 1));
            double stDevLong = Math.Sqrt(sumPowLong/(totalLongTrades - 1));
            double stDevShort = Math.Sqrt(sumPowShort/(totalShortTrades - 1));

            SharpeRatio = Math.Abs(stDev - 0) < micron ? 0 : (averageHPR - 1)/stDev;
            sharpeRatioLong = Math.Abs(stDevLong - 0) < micron ? 0 : (averageHPRLong - 1) / stDevLong;
            sharpeRatioShort = Math.Abs(stDevShort - 0) < micron ? 0 : (averageHPRShort - 1) / stDevShort;

            // Annualized Profit
            AnnualizedProfit = (365f/Time[Bars - 1].Subtract(Time[0]).Days)*(NetMoneyBalance - Configs.InitialAccount);
            AnnualizedProfitPercent = 100*AnnualizedProfit/Configs.InitialAccount;

            ProfitFactor = Math.Abs(GrossMoneyLoss - 0) < micron ? GrossMoneyProfit : GrossMoneyProfit/GrossMoneyLoss;

            if (Configs.AccountInMoney)
                SetAdditionalMoneyStats();
            else
                SetAdditionalStats();
        }

        /// <summary>
        ///     Sets the additional stats in points.
        /// </summary>
        private static void SetAdditionalStats()
        {
            string unit = " " + Language.T("points");

            AdditionalStatsParamName = new[]
                {
                    Language.T("Initial account"),
                    Language.T("Account balance"),
                    Language.T("Net profit"),
                    Language.T("Gross profit"),
                    Language.T("Gross loss"),
                    Language.T("Profit factor"),
                    Language.T("Annualized profit"),
                    Language.T("Minimum account"),
                    Language.T("Minimum account date"),
                    Language.T("Maximum account"),
                    Language.T("Maximum account date"),
                    Language.T("Absolute drawdown"),
                    Language.T("Maximum drawdown"),
                    Language.T("Maximum drawdown date"),
                    Language.T("Historical bars"),
                    Language.T("Tested bars"),
                    Language.T("Bars with trades"),
                    Language.T("Bars with trades") + " %",
                    Language.T("Number of trades"),
                    Language.T("Winning trades"),
                    Language.T("Losing trades"),
                    Language.T("Win/loss ratio"),
                    Language.T("Maximum profit"),
                    Language.T("Average profit"),
                    Language.T("Maximum loss"),
                    Language.T("Average loss"),
                    Language.T("Expected payoff")
                };

            int totalWinTrades = winningLongTrades + winningShortTrades;
            int totalLossTrades = losingLongTrades + losingShortTrades;
            int trades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
                {
                    "0" + unit,
                    NetBalance + unit,
                    NetBalance + unit,
                    Math.Round(grossProfit) + unit,
                    Math.Round(grossLoss) + unit,
                    (Math.Abs(grossLoss - 0) < 0.00001 ? "N/A" : Math.Abs(grossProfit/grossLoss).ToString("F2")),
                    Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetBalance)) + unit,
                    MinBalance + unit,
                    minBalanceDate.ToShortDateString(),
                    MaxBalance + unit,
                    maxBalanceDate.ToShortDateString(),
                    Math.Abs(MinBalance) + unit,
                    Math.Round(maxDrawdown) + unit,
                    maxDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithPos/(Bars - FirstBar)).ToString("F2") + "%",
                    trades.ToString(CultureInfo.InvariantCulture),
                    totalWinTrades.ToString(CultureInfo.InvariantCulture),
                    totalLossTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                    Math.Round(Math.Max(maxLongWin, maxShortWin)) + unit,
                    Math.Round(grossProfit/totalWinTrades) + unit,
                    Math.Round(Math.Min(maxLongLoss, maxShortLoss)) + unit,
                    Math.Round(grossLoss/totalLossTrades) + unit,
                    (1f*NetBalance/trades).ToString("F2") + unit
                };

            AdditionalStatsValueLong = new[]
                {
                    "0" + unit,
                    NetLongBalance + unit,
                    NetLongBalance + unit,
                    Math.Round(grossLongProfit) + unit,
                    Math.Round(grossLongLoss) + unit,
                    (Math.Abs(grossLongLoss - 0) < 0.00001
                         ? "N/A"
                         : Math.Abs(grossLongProfit/grossLongLoss).ToString("F2")),
                    Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetLongBalance)) + unit,
                    MinLongBalance + unit,
                    minLongBalanceDate.ToShortDateString(),
                    MaxLongBalance + unit,
                    maxLongBalanceDate.ToShortDateString(),
                    Math.Round(Math.Abs(minLongBalance)) + unit,
                    Math.Round(maxLongDrawdown) + unit,
                    maxLongDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithLongPos/(Bars - FirstBar)).ToString("F2") + "%",
                    totalLongTrades.ToString(CultureInfo.InvariantCulture),
                    winningLongTrades.ToString(CultureInfo.InvariantCulture),
                    losingLongTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*winningLongTrades/(winningLongTrades + losingLongTrades)).ToString("F2"),
                    Math.Round(maxLongWin) + unit,
                    Math.Round(grossLongProfit/winningLongTrades) + unit,
                    Math.Round(maxLongLoss) + unit,
                    Math.Round(grossLongLoss/losingLongTrades) + unit,
                    (1f*NetLongBalance/(winningLongTrades + losingLongTrades)).ToString("F2") + unit
                };

            AdditionalStatsValueShort = new[]
                {
                    "0" + unit,
                    NetShortBalance + unit,
                    NetShortBalance + unit,
                    Math.Round(grossShortProfit) + unit,
                    Math.Round(grossShortLoss) + unit,
                    (Math.Abs(grossShortLoss - 0) < 0.00001
                         ? "N/A"
                         : Math.Abs(grossShortProfit/grossShortLoss).ToString("F2")),
                    Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetShortBalance)) + unit,
                    MinShortBalance + unit,
                    minShortBalanceDate.ToShortDateString(),
                    MaxShortBalance + unit,
                    maxShortBalanceDate.ToShortDateString(),
                    Math.Round(Math.Abs(minShortBalance)) + unit,
                    Math.Round(maxShortDrawdown) + unit,
                    maxShortDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithShortPos/(Bars - FirstBar)).ToString("F2") + "%",
                    totalShortTrades.ToString(CultureInfo.InvariantCulture),
                    winningShortTrades.ToString(CultureInfo.InvariantCulture),
                    losingShortTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*winningShortTrades/(winningShortTrades + losingShortTrades)).ToString("F2"),
                    Math.Round(maxShortWin) + unit,
                    Math.Round(grossShortProfit/winningShortTrades) + unit,
                    Math.Round(maxShortLoss) + unit,
                    Math.Round(grossShortLoss/losingShortTrades) + unit,
                    (1f*NetShortBalance/(winningShortTrades + losingShortTrades)).ToString("F2") + unit
                };
        }

        /// <summary>
        ///     Sets the additional stats in Money.
        /// </summary>
        private static void SetAdditionalMoneyStats()
        {
            string unit = " " + Configs.AccountCurrency;

            AdditionalStatsParamName = new[]
                {
                    Language.T("Initial account"),
                    Language.T("Account balance"),
                    Language.T("Net profit"),
                    Language.T("Net profit") + " %",
                    Language.T("Gross profit"),
                    Language.T("Gross loss"),
                    Language.T("Profit factor"),
                    Language.T("Annualized profit"),
                    Language.T("Annualized profit") + " %",
                    Language.T("Minimum account"),
                    Language.T("Minimum account date"),
                    Language.T("Maximum account"),
                    Language.T("Maximum account date"),
                    Language.T("Absolute drawdown"),
                    Language.T("Maximum drawdown"),
                    Language.T("Maximum drawdown") + " %",
                    Language.T("Maximum drawdown date"),
                    Language.T("Historical bars"),
                    Language.T("Tested bars"),
                    Language.T("Bars with trades"),
                    Language.T("Bars with trades") + " %",
                    Language.T("Number of trades"),
                    Language.T("Winning trades"),
                    Language.T("Losing trades"),
                    Language.T("Win/loss ratio"),
                    Language.T("Maximum profit"),
                    Language.T("Average profit"),
                    Language.T("Maximum loss"),
                    Language.T("Average loss"),
                    Language.T("Expected payoff"),
                    Language.T("Average holding period returns"),
                    Language.T("Geometric holding period returns"),
                    Language.T("Sharpe ratio")
                };

            int totalWinTrades = winningLongTrades + winningShortTrades;
            int totalLossTrades = losingLongTrades + losingShortTrades;
            int trades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
                {
                    Configs.InitialAccount.ToString("F2") + unit,
                    NetMoneyBalance.ToString("F2") + unit,
                    (NetMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                    (100*((NetMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                    GrossMoneyProfit.ToString("F2") + unit,
                    GrossMoneyLoss.ToString("F2") + unit,
                    ProfitFactor.ToString("F2"),
                    AnnualizedProfit.ToString("F2") + unit,
                    AnnualizedProfitPercent.ToString("F2") + "%",
                    MinMoneyBalance.ToString("F2") + unit,
                    minMoneyBalanceDate.ToShortDateString(),
                    MaxMoneyBalance.ToString("F2") + unit,
                    maxMoneyBalanceDate.ToShortDateString(),
                    (Configs.InitialAccount - MinMoneyBalance).ToString("F2") + unit,
                    MaxMoneyDrawdown.ToString("F2") + unit,
                    maxMoneyDrawdownPercent.ToString("F2") + "%",
                    maxMoneyDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithPos/(Bars - FirstBar)).ToString("F2") + "%",
                    trades.ToString(CultureInfo.InvariantCulture),
                    totalWinTrades.ToString(CultureInfo.InvariantCulture),
                    totalLossTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                    Math.Max(maxLongMoneyWin, maxShortMoneyWin).ToString("F2") + unit,
                    (GrossMoneyProfit/totalWinTrades).ToString("F2") + unit,
                    Math.Min(maxLongMoneyLoss, maxShortMoneyLoss).ToString("F2") + unit,
                    (GrossMoneyLoss/totalLossTrades).ToString("F2") + unit,
                    (1f*(NetMoneyBalance - Configs.InitialAccount)/trades).ToString("F2") + unit,
                    AvrgHoldingPeriodRet.ToString("F2") + "%",
                    GeomHoldingPeriodRet.ToString("F2") + "%",
                    SharpeRatio.ToString("F2")
                };

            AdditionalStatsValueLong = new[]
                {
                    Configs.InitialAccount.ToString("F2") + unit,
                    NetLongMoneyBalance.ToString("F2") + unit,
                    (NetLongMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                    (100*((NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                    grossLongMoneyProfit.ToString("F2") + unit,
                    grossLongMoneyLoss.ToString("F2") + unit,
                    (Math.Abs(grossLongMoneyLoss - 0) < micron
                         ? grossLongMoneyProfit
                         : Math.Abs(grossLongMoneyProfit/grossLongMoneyLoss)).ToString("F2"),
                    ((365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                     (NetLongMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                    (100*(365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                     (NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                    MinLongMoneyBalance.ToString("F2") + unit,
                    minLongMoneyBalanceDate.ToShortDateString(),
                    MaxLongMoneyBalance.ToString("F2") + unit,
                    maxLongMoneyBalanceDate.ToShortDateString(),
                    (Configs.InitialAccount - MinLongMoneyBalance).ToString("F2") + unit,
                    maxLongMoneyDrawdown.ToString("F2") + unit,
                    maxLongMoneyDrawdownPercent.ToString("F2") + "%",
                    maxLongMoneyDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithLongPos/(Bars - FirstBar)).ToString("F2") + "%",
                    totalLongTrades.ToString(CultureInfo.InvariantCulture),
                    winningLongTrades.ToString(CultureInfo.InvariantCulture),
                    losingLongTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*winningLongTrades/(winningLongTrades + losingLongTrades)).ToString("F2"),
                    maxLongMoneyWin.ToString("F2") + unit,
                    (grossLongMoneyProfit/winningLongTrades).ToString("F2") + unit,
                    maxLongMoneyLoss.ToString("F2") + unit,
                    (grossLongMoneyLoss/losingLongTrades).ToString("F2") + unit,
                    (1f*(NetLongMoneyBalance - Configs.InitialAccount)/
                     (winningLongTrades + losingLongTrades)).ToString("F2") + unit,
                    ahprLong.ToString("F2") + "%",
                    ghprLong.ToString("F2") + "%",
                    sharpeRatioLong.ToString("F2")
                };

            AdditionalStatsValueShort = new[]
                {
                    Configs.InitialAccount.ToString("F2") + unit,
                    NetShortMoneyBalance.ToString("F2") + unit,
                    (NetShortMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                    (100*((NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                    grossShortMoneyProfit.ToString("F2") + unit,
                    grossShortMoneyLoss.ToString("F2") + unit,
                    (Math.Abs(grossShortMoneyLoss - 0) < micron
                         ? grossShortMoneyProfit
                         : Math.Abs(grossShortMoneyProfit/grossShortMoneyLoss)).ToString("F2"),
                    ((365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                     (NetShortMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                    (100*(365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                     (NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                    MinShortMoneyBalance.ToString("F2") + unit,
                    minShortMoneyBalanceDate.ToShortDateString(),
                    MaxShortMoneyBalance.ToString("F2") + unit,
                    maxShortMoneyBalanceDate.ToShortDateString(),
                    (Configs.InitialAccount - MinShortMoneyBalance).ToString("F2") + unit,
                    maxShortMoneyDrawdown.ToString("F2") + unit,
                    maxShortMoneyDrawdownPercent.ToString("F2") + "%",
                    maxShortMoneyDrawdownDate.ToShortDateString(),
                    Bars.ToString(CultureInfo.InvariantCulture),
                    (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                    barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                    (100f*barsWithShortPos/(Bars - FirstBar)).ToString("F2") + "%",
                    totalShortTrades.ToString(CultureInfo.InvariantCulture),
                    winningShortTrades.ToString(CultureInfo.InvariantCulture),
                    losingShortTrades.ToString(CultureInfo.InvariantCulture),
                    (1f*winningShortTrades/(winningShortTrades + losingShortTrades)).ToString("F2"),
                    maxShortMoneyWin.ToString("F2") + unit,
                    (grossShortMoneyProfit/winningShortTrades).ToString("F2") + unit,
                    maxShortMoneyLoss.ToString("F2") + unit,
                    (grossShortMoneyLoss/losingShortTrades).ToString("F2") + unit,
                    (1f*(NetShortMoneyBalance - Configs.InitialAccount)/(winningShortTrades + losingShortTrades))
                        .ToString("F2") + unit,
                    ahprShort.ToString("F2") + "%",
                    ghprShort.ToString("F2") + "%",
                    sharpeRatioShort.ToString("F2")
                };
        }
    }
}