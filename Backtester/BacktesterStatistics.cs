// Backtester class - Statistics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester
    {
        static double _maxBalance;
        static double _minBalance;
        static double _maxDrawdown;
        static double _maxEquity;
        static double _minEquity;
        static double _maxEquityDrawdown;
        static double _grossProfit;
        static double _grossLoss;
        static double _minMoneyBalance;
        static int[] _balanceDrawdown;
        static int[] _equityDrawdown;

        static int    _executedOrders;
        static int    _barsInPosition;
        static int    _totalTrades;

        static DateTime _maxBalanceDate;
        static DateTime _minBalanceDate;
        static DateTime _maxMoneyBalanceDate;
        static DateTime _minMoneyBalanceDate;
        static double   _maxMoneyDrawdownPercent;
        static DateTime _maxDrawdownDate;
        static DateTime _maxMoneyDrawdownDate;

        static Backtester()
        {
            InterpolationMethod = InterpolationMethod.Pessimistic;
            AccountStatsFlags = new bool[0];
            AccountStatsValue = new string[0];
            AccountStatsParam = new string[0];
        }

        /// <summary>
        /// Gets the account balance in pips
        /// </summary>
        public static int NetBalance { get { return Balance(Bars - 1); } }

        /// <summary>
        /// Gets the max balance in pips
        /// </summary>
        public static int MaxBalance { get { return (int)Math.Round(_maxBalance); } }

        /// <summary>
        /// Gets the min balance in pips
        /// </summary>
        public static int MinBalance { get { return (int)Math.Round(_minBalance); } }

        /// <summary>
        /// Gets the account balance in currency
        /// </summary>
        public static double NetMoneyBalance { get { return MoneyBalance(Bars - 1); } }

        /// <summary>
        /// Gets the max balance in currency
        /// </summary>
        public static double MaxMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min balance in currency
        /// </summary>
        public static double MinMoneyBalance { get { return _minMoneyBalance; } }

        /// <summary>
        /// Gets the max equity
        /// </summary>
        public static int MaxEquity { get { return (int)Math.Round(_maxEquity); } }

        /// <summary>
        /// Gets the min equity in pips
        /// </summary>
        public static int MinEquity { get { return (int)Math.Round(_minEquity); } }

        /// <summary>
        /// Gets the max Equity in currency
        /// </summary>
        public static double MaxMoneyEquity { get; private set; }

        /// <summary>
        /// Gets the min Equity in currency
        /// </summary>
        public static double MinMoneyEquity { get; private set; }

        /// <summary>
        /// Gets the maximum drawdown in the account bill
        /// </summary>
        public static int MaxDrawdown { get { return (int)Math.Round(_maxDrawdown); } }

        /// <summary>
        /// Gets the maximum equity drawdown in the account bill
        /// </summary>
        public static int MaxEquityDrawdown { get { return (int)Math.Round(_maxEquityDrawdown); } }

        /// <summary>
        /// Gets the maximum money drawdown
        /// </summary>
        public static double MaxMoneyDrawdown { get; private set; }

        /// <summary>
        /// Gets the maximum money equity drawdown
        /// </summary>
        public static double MaxMoneyEquityDrawdown { get; private set; }

        /// <summary>
        /// The total earned pips
        /// </summary>
        public static int GrossProfit { get { return (int)Math.Round(_grossProfit); } }

        /// <summary>
        /// The total earned money
        /// </summary>
        public static double GrossMoneyProfit { get; private set; }

        /// <summary>
        /// The total lost pips
        /// </summary>
        public static int GrossLoss { get { return (int)Math.Round(_grossLoss); } }

        /// <summary>
        /// The total lost money
        /// </summary>
        public static double GrossMoneyLoss { get; private set; }

        /// <summary>
        /// Gets the count of executed orders
        /// </summary>
        public static int ExecutedOrders { get { return _executedOrders; } }

        /// <summary>
        /// Gets the count of lots have been traded
        /// </summary>
        public static double TradedLots { get; private set; }

        /// <summary>
        /// Gets the time in position in percents
        /// </summary>
        public static int TimeInPosition { get { return (int)Math.Round(100f * _barsInPosition / (Bars - FirstBar)); } }

        /// <summary>
        /// Gets the count of sent orders
        /// </summary>
        public static int SentOrders { get; private set; }

        /// <summary>
        /// Gets the Charged Spread
        /// </summary>
        public static double TotalChargedSpread { get; private set; }

        /// <summary>
        /// Gets the Charged Spread in currency
        /// </summary>
        public static double TotalChargedMoneySpread { get; private set; }

        /// <summary>
        /// Gets the Charged RollOver
        /// </summary>
        public static double TotalChargedRollOver { get; private set; }

        /// <summary>
        /// Gets the Charged RollOver in currency
        /// </summary>
        public static double TotalChargedMoneyRollOver { get; private set; }

        /// <summary>
        /// Gets the Charged Slippage
        /// </summary>
        public static double TotalChargedSlippage { get; private set; }

        /// <summary>
        /// Gets the Charged Slippage in currency
        /// </summary>
        public static double TotalChargedMoneySlippage { get; private set; }

        /// <summary>
        /// Gets the Charged Commission
        /// </summary>
        public static double TotalChargedCommission { get; private set; }

        /// <summary>
        /// Gets the Charged Commission in currency
        /// </summary>
        public static double TotalChargedMoneyCommission { get; private set; }

        /// <summary>
        /// Winning Trades
        /// </summary>
        public static int WinningTrades { get; private set; }

        /// <summary>
        /// Losing Trades
        /// </summary>
        public static int LosingTrades { get; private set; }

        /// <summary>
        /// Win / Loss ratio
        /// </summary>
        public static double WinLossRatio { get; private set; }

        /// <summary>
        /// Money Equity Percent Drawdown
        /// </summary>
        public static double MoneyEquityPercentDrawdown { get; private set; }

        /// <summary>
        /// Equity Percent Drawdown
        /// </summary>
        public static double EquityPercentDrawdown { get; private set; }

        /// <summary>
        /// Returns the ambiguous calculated bars
        /// </summary>
        public static int AmbiguousBars { get; private set; }

        /// <summary>
        /// Was the intrabar scanning performed
        /// </summary>
        public static bool IsScanPerformed { get; private set; }

        /// <summary>
        /// Margin Call Bar
        /// </summary>
        public static int MarginCallBar { get; private set; }

        /// <summary>
        /// Gets the number of days tested.
        /// </summary>
        public static int TestedDays { get; private set; }

        /// <summary>
        /// Gets the profit per tested day.
        /// </summary>
        public static int ProfitPerDay { get { return TestedDays > 0 ? Balance(Bars - 1) / TestedDays : 0; } }

        /// <summary>
        /// Gets the profit per tested day in currency.
        /// </summary>
        public static double MoneyProfitPerDay { get { return TestedDays > 0 ? (MoneyBalance(Bars - 1) - Configs.InitialAccount) / TestedDays : 0; } }

        /// <summary>
        /// Gets the account stats parameters
        /// </summary>
        public static string[] AccountStatsParam { get; private set; }

        /// <summary>
        /// Gets the account stats values
        /// </summary>
        public static string[] AccountStatsValue { get; private set; }

        /// <summary>
        /// Gets the account stats flags
        /// </summary>
        public static bool[] AccountStatsFlags { get; private set; }

        /// <summary>
        /// Returns the Balance Drawdown in pips
        /// </summary>
        public static int BalanceDrawdown(int bar)
        {
            return _balanceDrawdown[bar];
        }

        /// <summary>
        /// Returns the Equity Drawdown in pips
        /// </summary>
        public static int EquityDrawdown(int bar)
        {
            return _equityDrawdown[bar];
        }

        /// <summary>
        /// Returns the Balance Drawdown in currency
        /// </summary>
        public static double MoneyBalanceDrawdown(int bar)
        {
            return _balanceDrawdown[bar] * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Returns the Equity Drawdown in currency.
        /// </summary>
        public static double MoneyEquityDrawdown(int bar)
        {
            return _equityDrawdown[bar] * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Calculates the account statistics.
        /// </summary>
        public static void CalculateAccountStats()
        {
            _maxBalance = 0;
            _minBalance = 0;
            _maxEquity  = 0;
            _minEquity  = 0;
            _maxEquityDrawdown = 0;
            _maxDrawdown       = 0;

            MaxMoneyBalance = Configs.InitialAccount;
            _minMoneyBalance = Configs.InitialAccount;
            MaxMoneyEquity  = Configs.InitialAccount;
            MinMoneyEquity  = Configs.InitialAccount;
            MaxMoneyEquityDrawdown = 0;
            MaxMoneyDrawdown       = 0;

            _barsInPosition    = 0;
            _grossProfit       = 0;
            _grossLoss         = 0;
            GrossMoneyProfit  = 0;
            GrossMoneyLoss    = 0;
            TotalChargedSpread     = 0;
            TotalChargedRollOver   = 0;
            TotalChargedCommission = 0;
            TotalChargedSlippage   = 0;
            TotalChargedMoneySpread     = 0;
            TotalChargedMoneyRollOver   = 0;
            TotalChargedMoneyCommission = 0;
            TotalChargedMoneySlippage   = 0;
            AmbiguousBars     = 0;
            _balanceDrawdown  = new int[Bars];
            _equityDrawdown   = new int[Bars];

            _maxBalanceDate       = Time[0];
            _minBalanceDate       = Time[0];
            _maxMoneyBalanceDate  = Time[0];
            _minMoneyBalanceDate  = Time[0];
            _maxDrawdownDate      = Time[0];
            _maxMoneyDrawdownDate = Time[0];

            EquityPercentDrawdown      = 100;
            _maxMoneyDrawdownPercent    = 0;
            MoneyEquityPercentDrawdown = 0;
            WinLossRatio               = 0;

            WinningTrades = 0;
            LosingTrades  = 0;
            _totalTrades   = 0;
            TestedDays    = 0;

            for (int bar = FirstBar; bar < Bars; bar++)
            {
                // Balance
                double balance = _session[bar].Summary.Balance;
                if (balance > _maxBalance)
                {
                    _maxBalance = balance;
                    _maxBalanceDate = Time[bar];
                }
                if (balance < _minBalance)
                {
                    _minBalance = balance;
                    _minBalanceDate = Time[bar];
                }

                // Money Balance
                double moneyBalance = _session[bar].Summary.MoneyBalance;
                if (moneyBalance > MaxMoneyBalance)
                {
                    MaxMoneyBalance = moneyBalance;
                    _maxMoneyBalanceDate = Time[bar];
                }
                if (moneyBalance < _minMoneyBalance)
                {
                    _minMoneyBalance = moneyBalance;
                    _minMoneyBalanceDate = Time[bar];
                }

                // Equity
                double equity = _session[bar].Summary.Equity;
                if (equity > _maxEquity) _maxEquity = equity;
                if (equity < _minEquity) _minEquity = equity;

                // Money Equity
                double moneyEquity = _session[bar].Summary.MoneyEquity;
                if (moneyEquity > MaxMoneyEquity) MaxMoneyEquity = moneyEquity;
                if (moneyEquity < MinMoneyEquity) MinMoneyEquity = moneyEquity;

                // Maximum Drawdown
                if (_maxBalance - balance > _maxDrawdown)
                {
                    _maxDrawdown = _maxBalance - balance;
                    _maxDrawdownDate = Time[bar];
                }

                // Maximum Equity Drawdown
                if (_maxEquity - equity > _maxEquityDrawdown)
                {
                    _maxEquityDrawdown = _maxEquity - equity;

                    // In percents
                    if (_maxEquity > 0)
                        EquityPercentDrawdown = 100 * (_maxEquityDrawdown / _maxEquity);
                }

                // Maximum Money Drawdown
                if (MaxMoneyBalance - MoneyBalance(bar) > MaxMoneyDrawdown)
                {
                    MaxMoneyDrawdown        = MaxMoneyBalance - MoneyBalance(bar);
                    _maxMoneyDrawdownPercent = 100 * (MaxMoneyDrawdown / MaxMoneyBalance);
                    _maxMoneyDrawdownDate    = Time[bar];
                }

                // Maximum Money Equity Drawdown
                if (MaxMoneyEquity - MoneyEquity(bar) > MaxMoneyEquityDrawdown)
                {
                    MaxMoneyEquityDrawdown = MaxMoneyEquity - MoneyEquity(bar);

                    // Maximum Money Equity Drawdown in percents
                    if (100 * MaxMoneyEquityDrawdown / MaxMoneyEquity > MoneyEquityPercentDrawdown)
                        MoneyEquityPercentDrawdown = 100 * (MaxMoneyEquityDrawdown / MaxMoneyEquity);
                }

                // Drawdown
                _balanceDrawdown[bar] = (int)Math.Round((_maxBalance - balance));
                _equityDrawdown[bar]  = (int)Math.Round((_maxEquity  - equity));

                // Bars in position
                if (_session[bar].Positions > 0)
                    _barsInPosition++;

                // Bar interpolation evaluation
                if (_session[bar].BacktestEval == BacktestEval.Ambiguous)
                {
                    AmbiguousBars++;
                }

                // Margin Call bar
                if (!Configs.TradeUntilMarginCall && MarginCallBar == 0 && _session[bar].Summary.FreeMargin < 0)
                    MarginCallBar = bar;
            }

            for (int pos = 0; pos < PositionsTotal; pos++)
            {   // Charged fees
                Position position = PosFromNumb(pos);
                TotalChargedSpread          += position.Spread;
                TotalChargedRollOver        += position.Rollover;
                TotalChargedCommission      += position.Commission;
                TotalChargedSlippage        += position.Slippage;
                TotalChargedMoneySpread     += position.MoneySpread;
                TotalChargedMoneyRollOver   += position.MoneyRollover;
                TotalChargedMoneyCommission += position.MoneyCommission;
                TotalChargedMoneySlippage   += position.MoneySlippage;

                // Winning losing trades.
                if (position.Transaction == Transaction.Close  ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (position.ProfitLoss > 0)
                    {
                        _grossProfit      += position.ProfitLoss;
                        GrossMoneyProfit += position.MoneyProfitLoss;
                        WinningTrades++;
                    }
                    else if (position.ProfitLoss < 0)
                    {
                        _grossLoss      += position.ProfitLoss;
                        GrossMoneyLoss += position.MoneyProfitLoss;
                        LosingTrades++;
                    }
                    _totalTrades++;
                }
            }

            WinLossRatio = WinningTrades / Math.Max((LosingTrades + WinningTrades), 1.0);

            _executedOrders = 0;
            TradedLots = 0;
            for (int ord = 0; ord < SentOrders; ord++)
            {
                if (OrdFromNumb(ord).OrdStatus == OrderStatus.Executed)
                {
                    _executedOrders++;
                    TradedLots += OrdFromNumb(ord).OrdLots;
                }
            }

            TestedDays = (Time[Bars - 1] - Time[FirstBar]).Days;
            if (TestedDays < 1)
                TestedDays = 1;

            if (Configs.AccountInMoney)
                GenerateAccountStatsInMoney();
            else
                GenerateAccountStats();

            if (Configs.AdditionalStatistics)
            {
                CalculateAdditionalStats();

                if (Configs.AccountInMoney)
                    SetAdditionalMoneyStats();
                else
                    SetAdditionalStats();
            }
        }

        /// <summary>
        /// Generate the Account Statistics in currency.
        /// </summary>
        static void GenerateAccountStatsInMoney()
        {
            AccountStatsParam = new[]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
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

            AccountStatsValue = new string[28];
            AccountStatsValue[0]  = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[1]  = InterpolationMethodShortToString();
            AccountStatsValue[2]  = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[3]  = MoneyProfitPerDay.ToString("F2") + unit;
            AccountStatsValue[4]  = (Bars - FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[5]  = Configs.InitialAccount.ToString("F2") + unit;
            AccountStatsValue[6]  = NetMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[7]  = MinMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[8]  = MaxMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[9]  = MaxMoneyDrawdown.ToString("F2") + unit;
            AccountStatsValue[10] = MaxMoneyEquityDrawdown.ToString("F2") + unit;
            AccountStatsValue[11] = MoneyEquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[12] = GrossMoneyProfit.ToString("F2") + unit;
            AccountStatsValue[13] = GrossMoneyLoss.ToString("F2")   + unit;
            AccountStatsValue[14] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[15] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[16] = TradedLots.ToString("F2");
            AccountStatsValue[17] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[18] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[19] = WinLossRatio.ToString("F2");
            AccountStatsValue[20] = TimeInPosition + " %";
            AccountStatsValue[21] = TotalChargedMoneySpread.ToString("F2")     + unit;
            AccountStatsValue[22] = TotalChargedMoneyRollOver.ToString("F2")   + unit;
            AccountStatsValue[23] = TotalChargedMoneyCommission.ToString("F2") + unit;
            AccountStatsValue[24] = TotalChargedMoneySlippage.ToString("F2")   + unit;
            AccountStatsValue[25] = (TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;
            AccountStatsValue[26] = (NetMoneyBalance + TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;

            if (InstrProperties.PriceIn == Configs.AccountCurrency)
                AccountStatsValue[27] = Language.T("Not used");
            else if (InstrProperties.InstrType == InstrumetType.Forex && Symbol.StartsWith(Configs.AccountCurrency))
                AccountStatsValue[27] = Language.T("Deal price");
            else if (Configs.AccountCurrency == "USD")
                AccountStatsValue[27] = InstrProperties.RateToUSD.ToString("F4");
            else if (Configs.AccountCurrency == "EUR")
                AccountStatsValue[27] = InstrProperties.RateToEUR.ToString("F4");

            AccountStatsFlags = new bool[28];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[6] = NetMoneyBalance < Configs.InitialAccount;
            AccountStatsFlags[9] = MaxDrawdown > Configs.InitialAccount / 2;
        }

        /// <summary>
        /// Generate the Account Statistics in pips.
        /// </summary>
        static void GenerateAccountStats()
        {
            AccountStatsParam = new[]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
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
            AccountStatsValue = new string[26];
            AccountStatsValue[0]  = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[1]  = InterpolationMethodShortToString();
            AccountStatsValue[2]  = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[3]  = ProfitPerDay + unit;
            AccountStatsValue[4]  = (Bars - FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[5]  = NetBalance  + unit;
            AccountStatsValue[6]  = MinBalance  + unit;
            AccountStatsValue[7]  = MaxBalance  + unit;
            AccountStatsValue[8]  = MaxDrawdown + unit;
            AccountStatsValue[9]  = MaxEquityDrawdown + unit;
            AccountStatsValue[10] = EquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[11] = GrossProfit + unit;
            AccountStatsValue[12] = GrossLoss   + unit;
            AccountStatsValue[13] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[14] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[15] = TradedLots.ToString("F2");
            AccountStatsValue[16] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[17] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[18] = ((float)WinningTrades/(WinningTrades + LosingTrades)).ToString("F2");
            AccountStatsValue[19] = TimeInPosition + " %";
            AccountStatsValue[20] = Math.Round(TotalChargedSpread) + unit;
            AccountStatsValue[21] = Math.Round(TotalChargedRollOver) + unit;
            AccountStatsValue[22] = Math.Round(TotalChargedCommission) + unit;
            AccountStatsValue[23] = TotalChargedSlippage.ToString("F2") + unit;
            AccountStatsValue[24] = Math.Round(TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;
            AccountStatsValue[25] = Math.Round(NetBalance + TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;

            AccountStatsFlags = new bool[26];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[5] = NetBalance < 0;
            AccountStatsFlags[8] = MaxDrawdown > 500;
        }

        /// <summary>
        /// Calculates the required margin.
        /// </summary>
        public static double RequiredMargin(double lots, int bar)
        {
            double amount = lots * InstrProperties.LotSize;
            double exchangeRate = Close[bar] / AccountExchangeRate(Close[bar]);
            double requiredMargin = amount * exchangeRate / Configs.Leverage;

            return requiredMargin;
        }

        /// <summary>
        /// Calculates the trading size in normalized lots.
        /// </summary>
        public static double TradingSize(double size, int bar)
        {
            if (Strategy.UseAccountPercentEntry)
            {
                double maxMargin = _session[bar].Summary.MoneyEquity * size / 100.0;
                double exchangeRate = Close[bar] / AccountExchangeRate(Close[bar]);
                size = maxMargin * Configs.Leverage / (exchangeRate * InstrProperties.LotSize);
            }

            size = NormalizeEntryLots(size);

            return size;
        }

        /// <summary>
        /// Normalizes an entry order's size.
        /// </summary>
        static double NormalizeEntryLots(double lots)
        {
            const double minlot = 0.01;
            double maxlot  = Strategy.MaxOpenLots;
            const double lotstep = 0.01;

            if (lots <= 0)
                return (0);

            var steps = (int)Math.Round((lots - minlot) / lotstep);
            lots = minlot + steps * lotstep;

            if (lots <= minlot)
                return (minlot);

            if (lots >= maxlot)
                return (maxlot);

            return lots;
        }

        /// <summary>
        /// Account Exchange Rate.
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
        /// Calculates the commission in pips.
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
                commission = (price / InstrProperties.Point) * (InstrProperties.Commission / 100);
                return commission;
            }

            else if (InstrProperties.CommissionType == CommissionType.money)
                commission = InstrProperties.Commission / (InstrProperties.Point * InstrProperties.LotSize);

            if (InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        /// Calculates the commission in currency.
        /// </summary>
        public static double CommissionInMoney(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (InstrProperties.Commission < 0.00001)
                return 0;

            if (InstrProperties.CommissionTime == CommissionTime.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (InstrProperties.CommissionType == CommissionType.pips)
                commission = InstrProperties.Commission * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(price);

            else if (InstrProperties.CommissionType == CommissionType.percents)
            {
                commission = lots * InstrProperties.LotSize * price * (InstrProperties.Commission / 100) / AccountExchangeRate(price);
                return commission;
            }

            else if (InstrProperties.CommissionType == CommissionType.money)
                commission = InstrProperties.Commission / AccountExchangeRate(price);

            if (InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        /// Calculates the rollover fee in currency.
        /// </summary>
        public static double RolloverInMoney(PosDirection posDir, double lots, int daysRollover, double price)
        {
            double point   = InstrProperties.Point;
            int    lotSize = InstrProperties.LotSize;
            double swapLongPips  = 0; // Swap long in pips
            double swapShortPips = 0; // Swap short in pips
            if (InstrProperties.SwapType == CommissionType.pips)
            {
                swapLongPips  = InstrProperties.SwapLong;
                swapShortPips = InstrProperties.SwapShort;
            }
            else if (InstrProperties.SwapType == CommissionType.percents)
            {
                swapLongPips  = (price / point) * (0.01 * InstrProperties.SwapLong / 365);
                swapShortPips = (price / point) * (0.01 * InstrProperties.SwapShort / 365);
            }
            else if (InstrProperties.SwapType == CommissionType.money)
            {
                swapLongPips  = InstrProperties.SwapLong  / (point * lotSize);
                swapShortPips = InstrProperties.SwapShort / (point * lotSize);
            }

            double rollover = lots * lotSize * (posDir == PosDirection.Long ? swapLongPips : -swapShortPips) * point * daysRollover / AccountExchangeRate(price);

            return rollover;
        }

        /// <summary>
        /// Converts pips to money.
        /// </summary>
        public static double PipsToMoney(double pips, int bar)
        {
            return pips * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Converts money to pips.
        /// </summary>
        public static double MoneyToPips(double money, int bar)
        {
            return money * AccountExchangeRate(Close[bar]) / (InstrProperties.Point * InstrProperties.LotSize);
        }
    }
}
