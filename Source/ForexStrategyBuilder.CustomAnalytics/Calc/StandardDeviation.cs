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

namespace ForexStrategyBuilder.CustomAnalytics
{
    /// <summary>
    /// Generator Custom Math
    /// </summary>
    public static partial class Calc
    {
        /// <summary>
        /// Standard Deviation
        /// </summary>
        public static double StandardDeviation(double[] data)
        {
            int items = data.GetUpperBound(0) - 1;
            if (items <= 0)
                return 1;

            var deviation = new double[items];
            double mean = ArithmeticMean(data);

            for (int i = 0; i < items; i++)
            {
                deviation[i] = Math.Pow((data[i] - mean), 2);
            }

            double devMean = ArithmeticMean(deviation);

            return Math.Sqrt(devMean);
        }
    }
}
