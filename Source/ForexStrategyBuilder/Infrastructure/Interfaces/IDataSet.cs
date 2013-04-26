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
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Infrastructure.Interfaces
{
    public interface IDataSet
    {
        /// <summary>
        /// Instrument symbol.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Data period.
        /// </summary>
        DataPeriod Period { get; }

        /// <summary>
        /// Data identification
        /// </summary>
        string DataId { get; set; }

        /// <summary>
        /// Count of data bars.
        /// </summary>
        int Bars { get; }

        /// <summary>
        /// Time of bar opening.
        /// </summary>
        DateTime[] Time { get; }

        /// <summary>
        /// Open price of the bar.
        /// </summary>
        double[] Open { get; }

        /// <summary>
        /// Highest price of the bar.
        /// </summary>
        double[] High { get; }

        /// <summary>
        /// Lowest price of the bar.
        /// </summary>
        double[] Low { get; }

        /// <summary>
        /// Close price of the bar.
        /// </summary>
        double[] Close { get; }

        /// <summary>
        /// Tick volume of the bar.
        /// </summary>
        int[] Volume { get; }

        // Intrabar data
        bool IsIntrabarData { get; set; }
        int LoadedIntrabarPeriods { get; set; }
        DataPeriod[] IntrabarPeriods { get; set; }
        Bar[][] IntrabarData { get; set; }
        int[] IntrabarBars { get; set; }
        int[] Intrabar { get; set; }

        // Tick data
        bool IsTickData { get; set; }
        long Ticks { get; set; }
        double[][] TickData { get; set; }

        /// <summary>
        /// The properties of the instrument.
        /// </summary>
        IInstrumentProperties Properties { get; set; }

        /// <summary>
        /// Data parameters.
        /// </summary>
        IDataParams DataParams { get; set; }

        void SetDataBar(int bar, Bar data);
    }
}