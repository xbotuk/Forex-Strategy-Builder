// Strategy Generator Extension
// Part of Forex Strategy Builder (Custom.Generator)
// Website http://forexsb.com/
// Copyright (c) 2006 - 2013 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.CustomAnalytics
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
