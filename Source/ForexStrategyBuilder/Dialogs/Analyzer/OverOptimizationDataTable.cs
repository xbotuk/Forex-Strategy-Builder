//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Globalization;
using System.Text;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    internal class OverOptimizationDataTable
    {
        private readonly string columnSeparator = Configs.ColumnSeparator;
        private readonly int countAllParams;
        private readonly double[,] data;

        private readonly string decimalSeparator = Configs.DecimalSeparator;
        private readonly int percentDeviation;
        private readonly int percentDeviationSteps;
        private int countStrategyParams;
        private string name;

        public OverOptimizationDataTable(int deviation, int countParam)
        {
            percentDeviation = deviation;
            percentDeviationSteps = 2*percentDeviation + 1;
            countAllParams = countParam;
            data = new double[percentDeviationSteps,countParam];
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CountDeviationSteps
        {
            get { return percentDeviationSteps; }
        }

        public int CountStrategyParams
        {
            get { return countStrategyParams; }
        }

        /// <summary>
        ///     Sets data
        /// </summary>
        /// <param name="indexDeviation">-10, -9, ... ,0, +1, +2, ... +10</param>
        /// <param name="indexParam">Parameter number</param>
        /// <param name="value">The value to be stored</param>
        public void SetData(int indexDeviation, int indexParam, double value)
        {
            data[DevPosition(indexDeviation), indexParam] = value;
            if (countStrategyParams < indexParam + 1)
                countStrategyParams = indexParam + 1;
        }

        public double GetData(int indexDeviation, int parmIndex)
        {
            return data[DevPosition(indexDeviation), parmIndex];
        }

        public string DataToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(name);
            sb.AppendLine(Language.T("Deviation"));
            for (int p = 0; p < percentDeviationSteps; p++)
            {
                int index = p - percentDeviation;
                sb.Append(index + columnSeparator);
                for (int i = 0; i < countAllParams; i++)
                    sb.Append(NumberToString(GetData(index, i)) + columnSeparator);
                sb.AppendLine();
            }
            sb.Append(Language.T("Parameter") + columnSeparator);
            for (int i = 1; i <= countAllParams; i++)
                sb.Append(i + columnSeparator);
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private int DevPosition(int indexDeviation)
        {
            return percentDeviation + indexDeviation;
        }

        private string NumberToString(double value)
        {
            string strValue = value.ToString(CultureInfo.InvariantCulture);
            strValue = strValue.Replace(".", decimalSeparator);
            strValue = strValue.Replace(",", decimalSeparator);
            return strValue;
        }
    }
}