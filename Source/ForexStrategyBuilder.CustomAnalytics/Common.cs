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
using System.Collections.Generic;
using System.Xml;

namespace ForexStrategyBuilder.CustomAnalytics
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
        
        // Generator Criterions Analysis
        public int Cycles;
        public int CriterionAmbiguousBars;
        public int CriterionMaxEquityDD;
        public int CriterionMaxEquityPercentDD;
        public int CriterionMinTrades;
        public int CriterionMaxTrades;
        public int CriterionWinLossRatio;
        public int CriterionOOSPatternFilter;
        public int CriterionSmoothBalanceLine;
        public int CriterionSmoothBalanceLineLong;
        public int CriterionSmoothBalanceLineShort;
        public int CriterionFailsNomination;
        public int CriterionSharpeRatio;
        public int CriterionProfitPerDay;
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
