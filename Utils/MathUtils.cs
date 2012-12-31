using System;

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
                output[index] = input[index].ToString();
            }
            return output;
        }
    }
}
