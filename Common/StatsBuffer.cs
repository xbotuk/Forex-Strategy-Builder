// StatsBuffer class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder.Common
{
    /// <summary>
    /// This class stores raw data used as a source for 
    /// calculation of user interface panels.
    /// </summary>
    public static class StatsBuffer
    {
        /// <summary>
        /// Forces the buffer to collect data.
        /// </summary>
        public static void UpdateStatsBuffer()
        {
            UpdateStartegyStats();
            UpdateAccountStats();

            if (Configs.AdditionalStatistics)
                UpdateAdditionalAccountStats();
        }

        private static void UpdateStartegyStats()
        {
            Strategy = Data.Strategy.Clone();
        }

        private static void UpdateAccountStats()
        {
            IsPosition = new bool[Data.Bars];
            PositionLotsCount = new double[Data.Bars];
            PositionDirection = new PosDirection[Data.Bars];

            Balance = new int[Data.Bars];
            Equity = new int[Data.Bars];
            MoneyBalance = new double[Data.Bars];
            MoneyEquity = new double[Data.Bars];

            for (var bar = 0; bar < Data.Bars; bar++)
            {
                IsPosition[bar] = Backtester.IsPos(bar);
                PositionLotsCount[bar] = Backtester.SummaryLots(bar);
                PositionDirection[bar] = Backtester.SummaryDir(bar);

                Balance[bar] = Backtester.Balance(bar);
                Equity[bar]  = Backtester.Equity(bar);
                MoneyBalance[bar] = Backtester.MoneyBalance(bar);
                MoneyEquity[bar]  = Backtester.MoneyEquity(bar);
            }

            MarginCallBar = Backtester.MarginCallBar;
            IsScanPerformed = Backtester.IsScanPerformed;
            ExecutedOrders = Backtester.ExecutedOrders;
            NetBalance = Backtester.NetBalance;
            NetMoneyBalance = Backtester.NetMoneyBalance;

            MaxBalance = Backtester.MaxBalance;
            MinBalance = Backtester.MinBalance;
            MaxEquity = Backtester.MaxEquity;
            MinEquity = Backtester.MinEquity;

            MaxMoneyBalance = Backtester.MaxMoneyBalance;
            MinMoneyBalance = Backtester.MinMoneyBalance;
            MaxMoneyEquity = Backtester.MaxMoneyEquity;
            MinMoneyEquity = Backtester.MinMoneyEquity;
        }

        private static void UpdateAdditionalAccountStats()
        {
            LongBalance = new int[Data.Bars];
            ShortBalance = new int[Data.Bars];
            LongMoneyBalance = new double[Data.Bars];
            ShortMoneyBalance = new double[Data.Bars];

            for (var bar = 0; bar < Data.Bars; bar++)
            {
                LongBalance[bar]  = Backtester.LongBalance(bar);
                ShortBalance[bar] = Backtester.ShortBalance(bar);
                LongMoneyBalance[bar]  = Backtester.LongMoneyBalance(bar);
                ShortMoneyBalance[bar] = Backtester.ShortMoneyBalance(bar);
            }

            MaxLongBalance = Backtester.MaxLongBalance;
            MinLongBalance = Backtester.MinLongBalance;
            MaxShortBalance = Backtester.MaxShortBalance;
            MinShortBalance = Backtester.MinShortBalance;

            MaxLongMoneyBalance = Backtester.MaxLongMoneyBalance;
            MinLongMoneyBalance = Backtester.MinLongMoneyBalance;
            MaxShortMoneyBalance = Backtester.MaxShortMoneyBalance;
            MinShortMoneyBalance = Backtester.MinShortMoneyBalance;
        }

        // Calculated strategy data including all indicator parameters and values.
        public static Strategy Strategy { get; private set; }
        public static int FirstBar { get { return Strategy.FirstBar; } }

        // Positions
        public static bool[] IsPosition { get; private set; }
        public static double[] PositionLotsCount { get; private set; }
        public static PosDirection[] PositionDirection { get; private set; }

        public static int MarginCallBar { get; private set; }
        public static bool IsScanPerformed { get; private set; }
        public static int ExecutedOrders { get; private set; }

        // Balance and equity in points.
        public static int[] Balance { get; private set; }
        public static int[] Equity { get; private set; }
        public static int NetBalance { get; private set; }
        public static int MaxBalance { get; private set; }
        public static int MinBalance { get; private set; }
        public static int MaxEquity { get; private set; }
        public static int MinEquity { get; private set; }

        // Balance and equity in currency.
        public static double[] MoneyBalance { get; private set; }
        public static double[] MoneyEquity { get; private set; }
        public static double NetMoneyBalance { get; private set; }
        public static double MaxMoneyBalance { get; private set; }
        public static double MinMoneyBalance { get; private set; }
        public static double MaxMoneyEquity { get; private set; }
        public static double MinMoneyEquity { get; private set; }

        // Additional stats in points.
        public static int[] LongBalance { get; private set; }
        public static int[] ShortBalance { get; private set; }
        public static int MaxLongBalance { get; private set; }
        public static int MinLongBalance { get; private set; }
        public static int MaxShortBalance { get; private set; }
        public static int MinShortBalance { get; private set; }

        // Additional stats in currency.
        public static double[] LongMoneyBalance { get; private set; }
        public static double[] ShortMoneyBalance { get; private set; }
        public static double MaxLongMoneyBalance { get; private set; }
        public static double MinLongMoneyBalance { get; private set; }
        public static double MaxShortMoneyBalance { get; private set; }
        public static double MinShortMoneyBalance { get; private set; }
    }
}
