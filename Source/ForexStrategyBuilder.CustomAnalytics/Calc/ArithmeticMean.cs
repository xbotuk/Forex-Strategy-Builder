//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ForexStrategyBuilder.CustomAnalytics
{
    /// <summary>
    ///     Generator Custom Math
    /// </summary>
    public static partial class Calc
    {
        /// <summary>
        ///     Arithmetic Mean
        /// </summary>
        public static double ArithmeticMean(double[] data)
        {
            double sum = 0.0;
            int items = data.GetUpperBound(0) - 1;
            if (items <= 0)
                return 0;

            for (int i = 0; i < items; i++)
            {
                sum += data[i];
            }

            return sum/items;
        }
    }
}