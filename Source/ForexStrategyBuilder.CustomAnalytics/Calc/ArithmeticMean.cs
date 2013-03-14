// Strategy Generator Extension
// Part of Forex Strategy Builder (Custom.Generator)
// Website http://forexsb.com/
// Copyright (c) 2006 - 2013 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder.CustomAnalytics
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