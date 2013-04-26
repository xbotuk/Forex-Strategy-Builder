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
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Infrastructure.Entities
{
    public class DataSet : IDataSet
    {
        public string Symbol { get; private set; }
        public DataPeriod Period { get; private set; }
        public string DataId { get; set; }
        public int Bars { get; private set; }
        public DateTime[] Time { get; private set; }
        public double[] Open { get; private set; }
        public double[] High { get; private set; }
        public double[] Low { get; private set; }
        public double[] Close { get; private set; }
        public int[] Volume { get; private set; }
        public bool IsIntrabarData { get; set; }
        public int LoadedIntrabarPeriods { get; set; }
        public DataPeriod[] IntrabarPeriods { get; set; }
        public Bar[][] IntrabarData { get; set; }
        public int[] IntrabarBars { get; set; }
        public int[] Intrabar { get; set; }
        public bool IsTickData { get; set; }
        public long Ticks { get; set; }
        public double[][] TickData { get; set; }
        public IInstrumentProperties Properties { get; set; }
        public IDataParams DataParams { get; set; }

        public void SetDataBar(int bar, Bar data)
        {
        }
    }
}