// Strategy Generator Extension
// Part of Forex Strategy Builder (Custom.Generator)
// Website http://forexsb.com/
// Copyright (c) 2006 - 2013 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

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
