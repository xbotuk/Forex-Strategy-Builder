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
            Strategy = Data.Strategy.Clone();

            IsPosition = new bool[Data.Bars];
            PositionLotsCount = new double[Data.Bars];
            PositionDirection = new PosDirection[Data.Bars];

            for (var bar = 0; bar < Data.Bars; bar++)
            {
                IsPosition[bar] = Backtester.IsPos(bar);
                PositionLotsCount[bar] = Backtester.SummaryLots(bar);
                PositionDirection[bar] = Backtester.SummaryDir(bar);
            }
        }

        // Calculated strategy data including 
        // all indicator parameters and values.
        public static Strategy Strategy { get; private set; }

        // Calculated account data
        public static bool[] IsPosition { get; private set; }
        public static double[] PositionLotsCount { get; private set; }
        public static PosDirection[] PositionDirection { get; private set; }

        // Shortcuts
        public static int FirstBar { get { return Strategy.FirstBar; } }
    }
}
