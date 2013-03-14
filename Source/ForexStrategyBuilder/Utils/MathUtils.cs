// FormState
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;

namespace Forex_Strategy_Builder.Utils
{
    public static class MathUtils
    {

        public static int[] ArrayResize(double[] input, int size)
        {
            var output = new int[size];

            var scale = input.Length / (double)size;
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                int count = 0;
                var startIndex = (int)Math.Floor(i * scale);
                var endIndex = (int)Math.Ceiling((i + 1) * scale);
                if (endIndex > input.Length)
                    endIndex = input.Length;

                for (int j = startIndex; j < endIndex; j++)
                {
                    sum += input[j];
                    count++;
                }

                output[i] = (int)((count == 0) ? input[startIndex] : Math.Round(sum / count));
            }

            return output;
        }

        public static string[] ArrayToStringArray(int[] input)
        {
            var output = new string[input.Length];
            for (int index = 0; index < input.Length; index++)
            {
                output[index] = input[index].ToString(CultureInfo.InvariantCulture);
            }
            return output;
        }
    }
}
