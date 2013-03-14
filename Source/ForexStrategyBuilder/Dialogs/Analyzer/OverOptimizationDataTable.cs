// Strategy Analyzer - OverOptimizationDataTable class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.Globalization;
using System.Text;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    internal class OverOptimizationDataTable
    {
        private readonly string _columnSeparator = Configs.ColumnSeparator;
        private readonly int _countAllParams;
        private readonly double[,] _data;

        private readonly string _decimalSeparator = Configs.DecimalSeparator;
        private readonly int _percentDeviation;
        private readonly int _percentDeviationSteps;
        private int _countStrategyParams;
        private string _name;

        public OverOptimizationDataTable(int percentDeviation, int countParam)
        {
            _percentDeviation = percentDeviation;
            _percentDeviationSteps = 2*percentDeviation + 1;
            _countAllParams = countParam;
            _data = new double[_percentDeviationSteps,countParam];
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int CountDeviationSteps
        {
            get { return _percentDeviationSteps; }
        }

        public int CountParams
        {
            get { return _countAllParams; }
        }

        public int CountStrategyParams
        {
            get { return _countStrategyParams; }
        }

        /// <summary>
        /// Sets data
        /// </summary>
        /// <param name="indexDeviation">-10, -9, ... ,0, +1, +2, ... +10</param>
        /// <param name="indexParam">Parameter number</param>
        /// <param name="data">The value to be stored</param>
        public void SetData(int indexDeviation, int indexParam, double data)
        {
            _data[DevPosition(indexDeviation), indexParam] = data;
            if (_countStrategyParams < indexParam + 1)
                _countStrategyParams = indexParam + 1;
        }

        public double GetData(int indexDeviation, int parmIndex)
        {
            return _data[DevPosition(indexDeviation), parmIndex];
        }

        public string DataToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_name);
            sb.AppendLine(Language.T("Deviation"));
            for (int p = 0; p < _percentDeviationSteps; p++)
            {
                int index = p - _percentDeviation;
                sb.Append(index + _columnSeparator);
                for (int i = 0; i < _countAllParams; i++)
                    sb.Append(NumberToString(GetData(index, i)) + _columnSeparator);
                sb.AppendLine();
            }
            sb.Append(Language.T("Parameter") + _columnSeparator);
            for (int i = 1; i <= _countAllParams; i++)
                sb.Append(i + _columnSeparator);
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private int DevPosition(int indexDeviation)
        {
            return _percentDeviation + indexDeviation;
        }

        private string NumberToString(double value)
        {
            string strValue = value.ToString(CultureInfo.InvariantCulture);
            strValue = strValue.Replace(".", _decimalSeparator);
            strValue = strValue.Replace(",", _decimalSeparator);
            return strValue;
        }
    }
}