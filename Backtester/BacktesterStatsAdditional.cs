// Backtester - Additional Statistics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester - Additional statistics
    /// </summary>
    public partial class Backtester
    {
        private static double[] _longBalance;
        private static double[] _shortBalance;
        private static double[] _longMoneyBalance;
        private static double[] _shortMoneyBalance;
        private static double _maxLongBalance;
        private static double _minLongBalance;
        private static double _maxShortBalance;
        private static double _minShortBalance;

        private static DateTime _maxLongBalanceDate;
        private static DateTime _minLongBalanceDate;
        private static DateTime _maxShortBalanceDate;
        private static DateTime _minShortBalanceDate;
        private static DateTime _maxLongMoneyBalanceDate;
        private static DateTime _minLongMoneyBalanceDate;
        private static DateTime _maxShortMoneyBalanceDate;
        private static DateTime _minShortMoneyBalanceDate;

        private static double _grossLongProfit;
        private static double _grossLongLoss;
        private static double _grossShortProfit;
        private static double _grossShortLoss;
        private static double _grossLongMoneyProfit;
        private static double _grossLongMoneyLoss;
        private static double _grossShortMoneyProfit;
        private static double _grossShortMoneyLoss;

        private static double _maxLongDrawdown;
        private static double _maxShortDrawdown;
        private static double _maxLongMoneyDrawdown;
        private static double _maxShortMoneyDrawdown;
        private static double _maxLongMoneyDrawdownPercent;
        private static double _maxShortMoneyDrawdownPercent;
        private static DateTime _maxLongDrawdownDate;
        private static DateTime _maxShortDrawdownDate;
        private static DateTime _maxLongMoneyDrawdownDate;
        private static DateTime _maxShortMoneyDrawdownDate;

        private static int _barsWithLongPos;
        private static int _barsWithShortPos;
        private static int _barsWithPos;

        private static int _winningLongTrades;
        private static int _winningShortTrades;
        private static int _losingLongTrades;
        private static int _losingShortTrades;
        private static int _totalLongTrades;
        private static int _totalShortTrades;

        private static double _maxLongWin;
        private static double _maxShortWin;
        private static double _maxLongMoneyWin;
        private static double _maxShortMoneyWin;
        private static double _maxLongLoss;
        private static double _maxShortLoss;
        private static double _maxLongMoneyLoss;
        private static double _maxShortMoneyLoss;

        private static double _ahpr;
        private static double _ahprLong;
        private static double _ahprShort;
        private static double _ghpr;
        private static double _ghprLong;
        private static double _ghprShort;
        private static double _sharpeRatio;
        private static double _sharpeRatioLong;
        private static double _sharpeRatioShort;

        /// <summary>
        /// Gets the Additional Stats Parameter Name.
        /// </summary>
        public static string[] AdditionalStatsParamName { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Long + Short.
        /// </summary>
        public static string[] AdditionalStatsValueTotal { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Long.
        /// </summary>
        public static string[] AdditionalStatsValueLong { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Short.
        /// </summary>
        public static string[] AdditionalStatsValueShort { get; private set; }

        /// <summary>
        /// Gets the long balance in pips.
        /// </summary>
        public static int NetLongBalance
        {
            get { return (int) Math.Round(_longBalance[Bars - 1]); }
        }

        /// <summary>
        /// Gets the short balance in pips.
        /// </summary>
        public static int NetShortBalance
        {
            get { return (int) Math.Round(_shortBalance[Bars - 1]); }
        }

        /// <summary>
        /// Gets the max long balance in pips.
        /// </summary>
        public static int MaxLongBalance
        {
            get { return (int) Math.Round(_maxLongBalance); }
        }

        /// <summary>
        /// Gets the max short balance in pips.
        /// </summary>
        public static int MaxShortBalance
        {
            get { return (int) Math.Round(_maxShortBalance); }
        }

        /// <summary>
        /// Gets the min long balance in pips.
        /// </summary>
        public static int MinLongBalance
        {
            get { return (int) Math.Round(_minLongBalance); }
        }

        /// <summary>
        /// Gets the min short balance in pips.
        /// </summary>
        public static int MinShortBalance
        {
            get { return (int) Math.Round(_minShortBalance); }
        }

        /// <summary>
        /// Gets the long balance in money
        /// </summary>
        public static double NetLongMoneyBalance
        {
            get { return _longMoneyBalance[Bars - 1]; }
        }

        /// <summary>
        /// Gets the short balance in money.
        /// </summary>
        public static double NetShortMoneyBalance
        {
            get { return _shortMoneyBalance[Bars - 1]; }
        }

        /// <summary>
        /// Gets the max long balance in money.
        /// </summary>
        public static double MaxLongMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the max short balance in money.
        /// </summary>
        public static double MaxShortMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min long balance in money.
        /// </summary>
        public static double MinLongMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min short balance in money.
        /// </summary>
        public static double MinShortMoneyBalance { get; private set; }

        /// <summary>
        /// Returns the long balance at the end of the bar in pips.
        /// </summary>
        public static int LongBalance(int bar)
        {
            return (int) Math.Round(_longBalance[bar]);
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in pips.
        /// </summary>
        public static int ShortBalance(int bar)
        {
            return (int) Math.Round(_shortBalance[bar]);
        }

        /// <summary>
        /// Returns the long balance at the end of the bar in money.
        /// </summary>
        public static double LongMoneyBalance(int bar)
        {
            return _longMoneyBalance[bar];
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in money.
        /// </summary>
        public static double ShortMoneyBalance(int bar)
        {
            return _shortMoneyBalance[bar];
        }

        /// <summary>
        /// Calculates the values of the stats parameters.
        /// </summary>
        private static void CalculateAdditionalStats()
        {
            _longBalance = new double[Bars];
            _shortBalance = new double[Bars];
            _longMoneyBalance = new double[Bars];
            _shortMoneyBalance = new double[Bars];

            MaxLongMoneyBalance = Configs.InitialAccount;
            MinLongMoneyBalance = Configs.InitialAccount;
            MaxShortMoneyBalance = Configs.InitialAccount;
            MinShortMoneyBalance = Configs.InitialAccount;
            _maxLongBalance = 0;
            _minLongBalance = 0;
            _maxShortBalance = 0;
            _minShortBalance = 0;

            _maxLongBalanceDate = Time[0];
            _minLongBalanceDate = Time[0];
            _maxShortBalanceDate = Time[0];
            _minShortBalanceDate = Time[0];
            _maxLongMoneyBalanceDate = Time[0];
            _minLongMoneyBalanceDate = Time[0];
            _maxShortMoneyBalanceDate = Time[0];
            _minShortMoneyBalanceDate = Time[0];
            _maxLongDrawdownDate = Time[0];
            _maxShortDrawdownDate = Time[0];
            _maxLongMoneyDrawdownDate = Time[0];
            _maxShortMoneyDrawdownDate = Time[0];

            _grossLongProfit = 0;
            _grossLongLoss = 0;
            _grossShortProfit = 0;
            _grossShortLoss = 0;
            _grossLongMoneyProfit = 0;
            _grossLongMoneyLoss = 0;
            _grossShortMoneyProfit = 0;
            _grossShortMoneyLoss = 0;

            _maxLongDrawdown = 0;
            _maxShortDrawdown = 0;
            _maxLongMoneyDrawdown = 0;
            _maxShortMoneyDrawdown = 0;
            _maxShortDrawdown = 0;
            _maxLongMoneyDrawdown = 0;
            _maxShortMoneyDrawdown = 0;
            _maxLongMoneyDrawdownPercent = 0;
            _maxShortMoneyDrawdownPercent = 0;

            _barsWithPos = 0;
            _barsWithLongPos = 0;
            _barsWithShortPos = 0;

            _winningLongTrades = 0;
            _winningShortTrades = 0;
            _losingLongTrades = 0;
            _losingShortTrades = 0;

            _totalLongTrades = 0;
            _totalShortTrades = 0;

            _maxLongWin = 0;
            _maxShortWin = 0;
            _maxLongMoneyWin = 0;
            _maxShortMoneyWin = 0;
            _maxLongLoss = 0;
            _maxShortLoss = 0;
            _maxLongMoneyLoss = 0;
            _maxShortMoneyLoss = 0;

            for (int bar = 0; bar < FirstBar; bar++)
            {
                _longBalance[bar] = 0;
                _shortBalance[bar] = 0;
                _longMoneyBalance[bar] = Configs.InitialAccount;
                _shortMoneyBalance[bar] = Configs.InitialAccount;
            }

            for (int bar = FirstBar; bar < Bars; bar++)
            {
                _longBalance[bar] = _longBalance[bar - 1];
                _shortBalance[bar] = _shortBalance[bar - 1];
                _longMoneyBalance[bar] = _longMoneyBalance[bar - 1];
                _shortMoneyBalance[bar] = _shortMoneyBalance[bar - 1];

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
                            _longBalance[bar] += positionProfitLoss;
                            _longMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                _grossLongProfit += positionProfitLoss;
                                _grossLongMoneyProfit += positionMoneyProfitLoss;
                                _winningLongTrades++;
                                if (positionProfitLoss > _maxLongWin)
                                    _maxLongWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > _maxLongMoneyWin)
                                    _maxLongMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                _grossLongLoss += positionProfitLoss;
                                _grossLongMoneyLoss += positionMoneyProfitLoss;
                                _losingLongTrades++;
                                if (positionProfitLoss < _maxLongLoss)
                                    _maxLongLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < _maxLongMoneyLoss)
                                    _maxLongMoneyLoss = positionMoneyProfitLoss;
                            }

                            _totalLongTrades++;
                        }
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Buy)
                        {
                            // Closing short position
                            _shortBalance[bar] += positionProfitLoss;
                            _shortMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                _grossShortProfit += positionProfitLoss;
                                _grossShortMoneyProfit += positionMoneyProfitLoss;
                                _winningShortTrades++;
                                if (positionProfitLoss > _maxShortWin)
                                    _maxShortWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > _maxShortMoneyWin)
                                    _maxShortMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                _grossShortLoss += positionProfitLoss;
                                _grossShortMoneyLoss += positionMoneyProfitLoss;
                                _losingShortTrades++;
                                if (positionProfitLoss < _maxShortLoss)
                                    _maxShortLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < _maxShortMoneyLoss)
                                    _maxShortMoneyLoss = positionMoneyProfitLoss;
                            }

                            _totalShortTrades++;
                        }
                    }
                }

                _barsWithPos += (isLong || isShort) ? 1 : 0;
                _barsWithLongPos += isLong ? 1 : 0;
                _barsWithShortPos += isShort ? 1 : 0;

                if (_maxLongBalance < _longBalance[bar])
                {
                    _maxLongBalance = _longBalance[bar];
                    _maxLongBalanceDate = Time[bar];
                }
                if (_minLongBalance > _longBalance[bar])
                {
                    _minLongBalance = _longBalance[bar];
                    _minLongBalanceDate = Time[bar];
                }
                if (_maxShortBalance < _shortBalance[bar])
                {
                    _maxShortBalance = _shortBalance[bar];
                    _maxShortBalanceDate = Time[bar];
                }
                if (_minShortBalance > _shortBalance[bar])
                {
                    _minShortBalance = _shortBalance[bar];
                    _minShortBalanceDate = Time[bar];
                }
                if (MaxLongMoneyBalance < _longMoneyBalance[bar])
                {
                    MaxLongMoneyBalance = _longMoneyBalance[bar];
                    _maxLongMoneyBalanceDate = Time[bar];
                }
                if (MinLongMoneyBalance > _longMoneyBalance[bar])
                {
                    MinLongMoneyBalance = _longMoneyBalance[bar];
                    _minLongMoneyBalanceDate = Time[bar];
                }
                if (MaxShortMoneyBalance < _shortMoneyBalance[bar])
                {
                    MaxShortMoneyBalance = _shortMoneyBalance[bar];
                    _maxShortMoneyBalanceDate = Time[bar];
                }
                if (MinShortMoneyBalance > _shortMoneyBalance[bar])
                {
                    MinShortMoneyBalance = _shortMoneyBalance[bar];
                    _minShortMoneyBalanceDate = Time[bar];
                }

                // Maximum Drawdown
                if (_maxLongBalance - _longBalance[bar] > _maxLongDrawdown)
                {
                    _maxLongDrawdown = _maxLongBalance - _longBalance[bar];
                    _maxLongDrawdownDate = Time[bar];
                }

                if (MaxLongMoneyBalance - _longMoneyBalance[bar] > _maxLongMoneyDrawdown)
                {
                    _maxLongMoneyDrawdown = MaxLongMoneyBalance - _longMoneyBalance[bar];
                    _maxLongMoneyDrawdownPercent = 100*_maxLongMoneyDrawdown/MaxLongMoneyBalance;
                    _maxLongMoneyDrawdownDate = Time[bar];
                }

                if (_maxShortBalance - _shortBalance[bar] > _maxShortDrawdown)
                {
                    _maxShortDrawdown = _maxShortBalance - _shortBalance[bar];
                    _maxShortDrawdownDate = Time[bar];
                }

                if (MaxShortMoneyBalance - _shortMoneyBalance[bar] > _maxShortMoneyDrawdown)
                {
                    _maxShortMoneyDrawdown = MaxShortMoneyBalance - _shortMoneyBalance[bar];
                    _maxShortMoneyDrawdownPercent = 100*_maxShortMoneyDrawdown/MaxShortMoneyBalance;
                    _maxShortMoneyDrawdownDate = Time[bar];
                }
            }

            // Holding period returns
            _ahpr = 0;
            _ahprLong = 0;
            _ahprShort = 0;

            var hpr = new double[_totalTrades];
            var hprLong = new double[_totalLongTrades];
            var hprShort = new double[_totalShortTrades];

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

            double averageHPR = totalHPR/_totalTrades;
            double averageHPRLong = totalHPRLong/_totalLongTrades;
            double averageHPRShort = totalHPRShort/_totalShortTrades;

            _ahpr = 100*(averageHPR - 1);
            _ahprLong = 100*(averageHPRLong - 1);
            _ahprShort = 100*(averageHPRShort - 1);

            _ghpr = 100*(Math.Pow((NetMoneyBalance/Configs.InitialAccount), (1f/_totalTrades)) - 1);
            _ghprLong = 100*(Math.Pow((NetLongMoneyBalance/Configs.InitialAccount), (1f/_totalLongTrades)) - 1);
            _ghprShort = 100*(Math.Pow((NetShortMoneyBalance/Configs.InitialAccount), (1f/_totalShortTrades)) - 1);

            // Sharpe Ratio
            _sharpeRatio = 0;
            _sharpeRatioLong = 0;
            _sharpeRatioShort = 0;

            double sumPow = 0;
            double sumPowLong = 0;
            double sumPowShort = 0;

            for (int i = 0; i < _totalTrades; i++)
                sumPow += Math.Pow((hpr[i] - averageHPR), 2);
            for (int i = 0; i < _totalLongTrades; i++)
                sumPowLong += Math.Pow((hprLong[i] - averageHPRLong), 2);
            for (int i = 0; i < _totalShortTrades; i++)
                sumPowShort += Math.Pow((hprShort[i] - averageHPRShort), 2);

            double stDev = Math.Sqrt(sumPow/(_totalTrades - 1));
            double stDevLong = Math.Sqrt(sumPowLong/(_totalLongTrades - 1));
            double stDevShort = Math.Sqrt(sumPowShort/(_totalShortTrades - 1));

            _sharpeRatio = (averageHPR - 1)/stDev;
            _sharpeRatioLong = (averageHPRLong - 1)/stDevLong;
            _sharpeRatioShort = (averageHPRShort - 1)/stDevShort;
        }

        /// <summary>
        /// Sets the additional stats in pips.
        /// </summary>
        private static void SetAdditionalStats()
        {
            string unit = " " + Language.T("pips");

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

            int totalWinTrades = _winningLongTrades + _winningShortTrades;
            int totalLossTrades = _losingLongTrades + _losingShortTrades;
            int totalTrades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
            {
                "0" + unit,
                NetBalance + unit,
                NetBalance + unit,
                Math.Round(_grossProfit) + unit,
                Math.Round(_grossLoss) + unit,
                (Math.Abs(_grossLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossProfit/_grossLoss).ToString("F2")),
                Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetBalance)) + unit,
                MinBalance + unit,
                _minBalanceDate.ToShortDateString(),
                MaxBalance + unit,
                _maxBalanceDate.ToShortDateString(),
                Math.Abs(MinBalance) + unit,
                Math.Round(_maxDrawdown) + unit,
                _maxDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithPos/(Bars - FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(CultureInfo.InvariantCulture),
                totalWinTrades.ToString(CultureInfo.InvariantCulture),
                totalLossTrades.ToString(CultureInfo.InvariantCulture),
                (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Round(Math.Max(_maxLongWin, _maxShortWin)) + unit,
                Math.Round(_grossProfit/totalWinTrades) + unit,
                Math.Round(Math.Min(_maxLongLoss, _maxShortLoss)) + unit,
                Math.Round(_grossLoss/totalLossTrades) + unit,
                (1f*NetBalance/totalTrades).ToString("F2") + unit
            };

            AdditionalStatsValueLong = new[]
            {
                "0" + unit,
                NetLongBalance + unit,
                NetLongBalance + unit,
                Math.Round(_grossLongProfit) + unit,
                Math.Round(_grossLongLoss) + unit,
                (Math.Abs(_grossLongLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossLongProfit/_grossLongLoss).ToString("F2")),
                Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetLongBalance)) + unit,
                MinLongBalance + unit,
                _minLongBalanceDate.ToShortDateString(),
                MaxLongBalance + unit,
                _maxLongBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(_minLongBalance)) + unit,
                Math.Round(_maxLongDrawdown) + unit,
                _maxLongDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithLongPos/(Bars - FirstBar)).ToString("F2") + "%",
                _totalLongTrades.ToString(CultureInfo.InvariantCulture),
                _winningLongTrades.ToString(CultureInfo.InvariantCulture),
                _losingLongTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningLongTrades/(_winningLongTrades + _losingLongTrades)).ToString("F2"),
                Math.Round(_maxLongWin) + unit,
                Math.Round(_grossLongProfit/_winningLongTrades) + unit,
                Math.Round(_maxLongLoss) + unit,
                Math.Round(_grossLongLoss/_losingLongTrades) + unit,
                (1f*NetLongBalance/(_winningLongTrades + _losingLongTrades)).ToString("F2") + unit
            };

            AdditionalStatsValueShort = new[]
            {
                "0" + unit,
                NetShortBalance + unit,
                NetShortBalance + unit,
                Math.Round(_grossShortProfit) + unit,
                Math.Round(_grossShortLoss) + unit,
                (Math.Abs(_grossShortLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossShortProfit/_grossShortLoss).ToString("F2")),
                Math.Round(((365f/Time[Bars - 1].Subtract(Time[0]).Days)*NetShortBalance)) + unit,
                MinShortBalance + unit,
                _minShortBalanceDate.ToShortDateString(),
                MaxShortBalance + unit,
                _maxShortBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(_minShortBalance)) + unit,
                Math.Round(_maxShortDrawdown) + unit,
                _maxShortDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithShortPos/(Bars - FirstBar)).ToString("F2") + "%",
                _totalShortTrades.ToString(CultureInfo.InvariantCulture),
                _winningShortTrades.ToString(CultureInfo.InvariantCulture),
                _losingShortTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningShortTrades/(_winningShortTrades + _losingShortTrades)).ToString("F2"),
                Math.Round(_maxShortWin) + unit,
                Math.Round(_grossShortProfit/_winningShortTrades) + unit,
                Math.Round(_maxShortLoss) + unit,
                Math.Round(_grossShortLoss/_losingShortTrades) + unit,
                (1f*NetShortBalance/(_winningShortTrades + _losingShortTrades)).ToString("F2") + unit
            };
        }

        /// <summary>
        /// Sets the additional stats in Money.
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

            int totalWinTrades = _winningLongTrades + _winningShortTrades;
            int totalLossTrades = _losingLongTrades + _losingShortTrades;
            int totalTrades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetMoneyBalance.ToString("F2") + unit,
                (NetMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                GrossMoneyProfit.ToString("F2") + unit,
                GrossMoneyLoss.ToString("F2") + unit,
                (Math.Abs(GrossMoneyLoss - 0) < 0.00001 ? "N/A" : Math.Abs(GrossMoneyProfit/GrossMoneyLoss).ToString("F2")),
                ((365f/Time[Bars - 1].Subtract(Time[0]).Days)*(NetMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/Time[Bars - 1].Subtract(Time[0]).Days)*(NetMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinMoneyBalance.ToString("F2") + unit,
                _minMoneyBalanceDate.ToShortDateString(),
                MaxMoneyBalance.ToString("F2") + unit,
                _maxMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinMoneyBalance).ToString("F2") + unit,
                MaxMoneyDrawdown.ToString("F2") + unit,
                _maxMoneyDrawdownPercent.ToString("F2") + "%",
                _maxMoneyDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithPos/(Bars - FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(CultureInfo.InvariantCulture),
                totalWinTrades.ToString(CultureInfo.InvariantCulture),
                totalLossTrades.ToString(CultureInfo.InvariantCulture),
                (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Max(_maxLongMoneyWin, _maxShortMoneyWin).ToString("F2") + unit,
                (GrossMoneyProfit/totalWinTrades).ToString("F2") + unit,
                Math.Min(_maxLongMoneyLoss, _maxShortMoneyLoss).ToString("F2") + unit,
                (GrossMoneyLoss/totalLossTrades).ToString("F2") + unit,
                (1f*(NetMoneyBalance - Configs.InitialAccount)/totalTrades).ToString("F2") + unit,
                _ahpr.ToString("F2") + "%",
                _ghpr.ToString("F2") + "%",
                _sharpeRatio.ToString("F2")
            };

            AdditionalStatsValueLong = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetLongMoneyBalance.ToString("F2") + unit,
                (NetLongMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                _grossLongMoneyProfit.ToString("F2") + unit,
                _grossLongMoneyLoss.ToString("F2") + unit,
                (Math.Abs(_grossLongMoneyLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossLongMoneyProfit/_grossLongMoneyLoss).ToString("F2")),
                ((365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                (NetLongMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/Time[Bars - 1].Subtract(Time[0]).Days)*
                (NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinLongMoneyBalance.ToString("F2") + unit,
                _minLongMoneyBalanceDate.ToShortDateString(),
                MaxLongMoneyBalance.ToString("F2") + unit,
                _maxLongMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinLongMoneyBalance).ToString("F2") + unit,
                _maxLongMoneyDrawdown.ToString("F2") + unit,
                _maxLongMoneyDrawdownPercent.ToString("F2") + "%",
                _maxLongMoneyDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithLongPos/(Bars - FirstBar)).ToString("F2") + "%",
                _totalLongTrades.ToString(CultureInfo.InvariantCulture),
                _winningLongTrades.ToString(CultureInfo.InvariantCulture),
                _losingLongTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningLongTrades/(_winningLongTrades + _losingLongTrades)).ToString("F2"),
                _maxLongMoneyWin.ToString("F2") + unit,
                (_grossLongMoneyProfit/_winningLongTrades).ToString("F2") + unit,
                _maxLongMoneyLoss.ToString("F2") + unit,
                (_grossLongMoneyLoss/_losingLongTrades).ToString("F2") + unit,
                (1f*(NetLongMoneyBalance - Configs.InitialAccount)/
                (_winningLongTrades + _losingLongTrades)).ToString("F2") + unit,
                _ahprLong.ToString("F2") + "%",
                _ghprLong.ToString("F2") + "%",
                _sharpeRatioLong.ToString("F2")
            };

            AdditionalStatsValueShort = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetShortMoneyBalance.ToString("F2") + unit,
                (NetShortMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                _grossShortMoneyProfit.ToString("F2") + unit,
                _grossShortMoneyLoss.ToString("F2") + unit,
                (Math.Abs(_grossShortMoneyLoss - 0) < 0.0001 ? "N/A" : Math.Abs(_grossShortMoneyProfit/_grossShortMoneyLoss).ToString("F2")),
                ((365f/Time[Bars - 1].Subtract(Time[0]).Days)*(NetShortMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/Time[Bars - 1].Subtract(Time[0]).Days)*(NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinShortMoneyBalance.ToString("F2") + unit,
                _minShortMoneyBalanceDate.ToShortDateString(),
                MaxShortMoneyBalance.ToString("F2") + unit,
                _maxShortMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinShortMoneyBalance).ToString("F2") + unit,
                _maxShortMoneyDrawdown.ToString("F2") + unit,
                _maxShortMoneyDrawdownPercent.ToString("F2") + "%",
                _maxShortMoneyDrawdownDate.ToShortDateString(),
                Bars.ToString(CultureInfo.InvariantCulture),
                (Bars - FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithShortPos/(Bars - FirstBar)).ToString("F2") + "%",
                _totalShortTrades.ToString(CultureInfo.InvariantCulture),
                _winningShortTrades.ToString(CultureInfo.InvariantCulture),
                _losingShortTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningShortTrades/(_winningShortTrades + _losingShortTrades)).ToString("F2"),
                _maxShortMoneyWin.ToString("F2") + unit,
                (_grossShortMoneyProfit/_winningShortTrades).ToString("F2") + unit,
                _maxShortMoneyLoss.ToString("F2") + unit,
                (_grossShortMoneyLoss/_losingShortTrades).ToString("F2") + unit,
                (1f*(NetShortMoneyBalance - Configs.InitialAccount)/(_winningShortTrades + _losingShortTrades)).ToString("F2") + unit,
                _ahprShort.ToString("F2") + "%",
                _ghprShort.ToString("F2") + "%",
                _sharpeRatioShort.ToString("F2")
            };
        }
    }
}