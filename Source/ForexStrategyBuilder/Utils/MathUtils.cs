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
using System.Globalization;

namespace ForexStrategyBuilder.Utils
{
    public static class MathUtils
    {
        public static int[] ArrayResize(double[] input, int size)
        {
            var output = new int[size];

            double scale = input.Length/(double) size;
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                int count = 0;
                var startIndex = (int) Math.Floor(i*scale);
                var endIndex = (int) Math.Ceiling((i + 1)*scale);
                if (endIndex > input.Length)
                    endIndex = input.Length;

                for (int j = startIndex; j < endIndex; j++)
                {
                    sum += input[j];
                    count++;
                }

                output[i] = (int) ((count == 0) ? input[startIndex] : Math.Round(sum/count));
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