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
    /// Generator Custom Math
    /// </summary>
    public static partial class Calc
    {
        /// <summary>
        /// System Quality Number = Squareroot(N) * Average (of the N Profit&Loss) / Std dev (of the N Profit&Loss) 
        /// </summary>
        /// <returns>Van Tharpe SQN</returns>
        public static float SystemQualityNumber(ref List<Position> positions)
        {
            // Build the Trades List Curve
            var trades = new List<double>();
            foreach (Position pos in positions)
                trades.Add(pos.ProfitLoss);

            // System Quality Number
            return (float)(Math.Sqrt(trades.Count) * ArithmeticMean(trades.ToArray()) / StandardDeviation(trades.ToArray()));
        }
    }
}
