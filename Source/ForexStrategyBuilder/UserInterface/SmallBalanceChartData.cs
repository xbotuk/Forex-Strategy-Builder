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

namespace ForexStrategyBuilder
{
    public class SmallBalanceChartData
    {
        public int[] Balance { get; set; }
        public int[] Equity { get; set; }
        public int[] LongBalance { get; set; }
        public double[] LongMoneyBalance { get; set; }
        public double[] MoneyBalance { get; set; }
        public double[] MoneyEquity { get; set; }
        public int[] ShortBalance { get; set; }
        public double[] ShortMoneyBalance { get; set; }
        public float NetBalance { get; set; }
        public double[] ClosePrice { get; set; }
        public int MarginCallBar { get; set; }
        public int FirstBar { get; set; }
        public int Bars { get; set; }
        public double DataMinPrice { get; set; }
        public double DataMaxPrice { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string ModellingQuolity { get; set; }
        public DateTime DataTimeBarOOS { get; set; }
    }
}