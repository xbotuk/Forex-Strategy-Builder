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
    ///     Generator Custom Math
    /// </summary>
    public static partial class Calc
    {
        /// <summary>
        ///     Correlation
        /// </summary>
        public static double Correlation(List<double> balance)
        {
            // Build the Comparison Curve
            double max = Double.MinValue;
            foreach (double val in balance)
                if (val > max) max = val;

            var comparison = new List<double>();
            for (int i = 0; i <= balance.Count - 1; i++)
                comparison.Add(max/balance.Count*(i + 1));

            // Correlation Calculation
            double[] p = balance.ToArray();
            double[] q = comparison.ToArray();

            return PearsonCorrelation(p, q);
        }
    }
}