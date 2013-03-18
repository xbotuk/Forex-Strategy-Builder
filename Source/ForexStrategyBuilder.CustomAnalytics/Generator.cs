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
using System.Collections.Generic;

namespace ForexStrategyBuilder.CustomAnalytics
{
    /// <summary>
    ///     Custom Analytics
    /// </summary>
    public static class Generator
    {
        /// <summary>
        ///     Are Analytics Enabled
        /// </summary>
        public static bool IsAnalyticsEnabled
        {
            get { return true; }
        }

        /// <summary>
        ///     Is Full Bar Data needed by Analytics
        /// </summary>
        public static bool IsFullBarDataNeeded
        {
            get { return false; }
        }

        #region Simple Custom Sorting

        /// <summary>
        ///     Returns a list of supported simple custom generator sorting options
        /// </summary>
        public static IEnumerable<string> GetSimpleCustomSortingOptions()
        {
            var options = new List<string> {"System Quality Number"};

            return options;
        }

        /// <summary>
        ///     Returns the Simple Custom Sorting Value for the Generator
        /// </summary>
        /// <param name="cga">CustomGeneratorAnalysis Data</param>
        /// <param name="value">Simple Custom Sorting Value</param>
        /// <param name="displayName">Simple Custom Sorting Option Display Name</param>
        public static void GetSimpleCustomSortingValue(ref CustomGeneratorAnalytics cga,
                                                       out float value,
                                                       out string displayName)
        {
            // Set the initial output values 
            displayName = cga.SimpleSortOption;
            value = 0;

            // Perform simple custom sorting calculations
            switch (cga.SimpleSortOption)
            {
                case "System Quality Number":
                    value = Calc.SystemQualityNumber(ref cga.Positions);
                    break;
            }
        }

        #endregion

        #region Advanced Custom Sorting

        /// <summary>
        ///     Returns a list of supported simple custom generator sorting options
        /// </summary>
        public static IEnumerable<string> GetAdvancedCustomSortingOptions()
        {
            var options = new List<string> {"Correlation"};

            return options;
        }

        /// <summary>
        ///     Returns the Advanced Custom Sorting Value for the Generator
        /// </summary>
        /// <param name="cga">CustomGeneratorAnalysis Data</param>
        /// <param name="value">Simple Custom Sorting Value</param>
        /// <param name="displayName">Advanced Custom Sorting Option Display Name</param>
        public static void GetAdvancedCustomSortingValue(ref CustomGeneratorAnalytics cga,
                                                         out float value,
                                                         out string displayName)
        {
            // Set the initial output values 
            displayName = cga.AdvancedSortOption;
            value = 0;

            // Build the comparison curve
            var curve = new List<double>();
            switch (cga.AdvancedSortOptionCompareTo)
            {
                case "Balance":
                    foreach (Position pos in cga.Positions)
                        if (pos.Transaction != Transaction.Transfer)
                            curve.Add(pos.Balance);
                    break;
                case "Balance (with Transfers)":
                    foreach (Position pos in cga.Positions)
                        curve.Add(pos.Balance);
                    break;
                case "Equity":
                    foreach (Position pos in cga.Positions)
                        if (pos.Transaction != Transaction.Transfer)
                            curve.Add(pos.Equity);
                    break;
                case "Equity (with Transfers)":
                    foreach (Position pos in cga.Positions)
                        curve.Add(pos.Equity);
                    break;
            }

            // Perform advanced custom sorting calculations
            const double epsilon = 0.000001;
            switch (cga.AdvancedSortOption)
            {
                case "Correlation":
                    value = (float) Calc.Correlation(curve);
                    if (Math.Abs(value - 1) < epsilon)
                        value = 0;
                    break;
            }
        }

        #endregion
    }
}