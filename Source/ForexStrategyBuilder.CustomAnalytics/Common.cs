// Strategy Generator Extension
// Part of Forex Strategy Builder (Custom.Types)
// Website http://forexsb.com/
// Copyright (c) 2006 - 2013 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Xml;

namespace Forex_Strategy_Builder.CustomAnalytics
{
    /// <summary>
    /// Position
    /// </summary>
    public struct Position
    {
        public int          PositionNumber;                 //  0
        public int          BarNumber;                      //  1
        public DateTime     BarOpeningTime;                 //  2
        public PosDirection Direction;                      //  3
        public float        Lots;                           //  4
        public Transaction  Transaction;                    //  5
        public float        OrderPrice;                     //  6
        public float        AveragePrice;                   //  7
        public float        ProfitLoss;                     //  8
        public float        FloatingProfitLoss;             //  9
        public float        Balance;                        // 10
        public float        Equity;                         // 11
    }

    /// <summary>
    /// Custom Analysis
    /// </summary>
    public struct CustomGeneratorAnalytics
    {
        // Custom Sorting Options
        public string SimpleSortOption;
        public string AdvancedSortOption;
        public string AdvancedSortOptionCompareTo;
        
        // Generator
        public string PathToConfigFile;
        public XmlDocument Template;
        public List<Bar> Bars;

        // Generator Accepted Strategy 
        public XmlDocument Strategy;
        public List<Position> Positions;
        
        // Generator Limitations Analysis
        public int Cycles;
        public int LimitationAmbiguousBars;
        public int LimitationMaxEquityDD;
        public int LimitationMaxEquityPercentDD;
        public int LimitationMinTrades;
        public int LimitationMaxTrades;
        public int LimitationWinLossRatio;
        public int LimitationOOSPatternFilter;
        public int LimitationSmoothBalanceLine;
        public int LimitationSmoothBalanceLineLong;
        public int LimitationSmoothBalanceLineShort;
        public int LimitationFailsNomination;
    }

    /// <summary>
    /// Bar structure
    /// </summary>
    public struct Bar
    {
        public DateTime Time;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public int Volume;
    }

    /// <summary>
    /// The type of transaction
    /// </summary>
    public enum Transaction
    {
        None,
        Open,
        Close,
        Add,
        Reduce,
        Reverse,
        Transfer
    }

    /// <summary>
    /// The positions' direction
    /// </summary>
    public enum PosDirection
    {
        None,
        Long,
        Short,
        Closed
    }

}
