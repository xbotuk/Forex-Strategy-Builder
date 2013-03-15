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
    /// <summary>
    ///     Contains the instrument properties.
    /// </summary>
    public class InstrumentProperties
    {
        private int digits;

        /// <summary>
        ///     Constructor
        /// </summary>
        public InstrumentProperties(string symbol, InstrumetType instrType)
        {
            if (instrType == InstrumetType.Forex)
            {
                SetDefaultForexParams(symbol, instrType);
            }
            else
            {
                SetDefaultIndexParams(symbol, instrType);
            }
        }

        public string Symbol { get; set; }
        public string Comment { get; set; }
        public string PriceIn { get; set; }
        public string BaseFileName { get; set; }
        public int LotSize { get; set; }
        public int Slippage { get; set; }
        public double Spread { get; set; }
        public double SwapLong { get; set; }
        public double SwapShort { get; set; }
        public double Commission { get; set; }
        public double RateToUSD { get; set; }
        public double RateToEUR { get; set; }
        public InstrumetType InstrType { get; set; }
        public CommissionType SwapType { get; set; }
        public CommissionType CommissionType { get; set; }
        public CommissionScope CommissionScope { get; set; }
        public CommissionTime CommissionTime { get; set; }
        public double Point { get; private set; }
        public double Pip { get; private set; }
        public bool IsFiveDigits { get; private set; }

        public int Digits
        {
            get { return digits; }
            set
            {
                digits = value;
                Point = 1/Math.Pow(10, digits);
                IsFiveDigits = (digits == 3 || digits == 5);
                Pip = IsFiveDigits ? 10*Point : Point;
            }
        }

        /// <summary>
        ///     Gets the Commission type as a string
        /// </summary>
        public string CommissionTypeToString
        {
            get
            {
                switch (CommissionType)
                {
                    case CommissionType.pips:
                        return Language.T("pips");
                    case CommissionType.percents:
                        return Language.T("percent");
                    default:
                        return Language.T("money");
                }
            }
        }

        /// <summary>
        ///     Gets the Commission Scope as a string
        /// </summary>
        public string CommissionScopeToString
        {
            get { return Language.T(CommissionScope == CommissionScope.lot ? "per lot" : "per deal"); }
        }

        /// <summary>
        ///     Gets the Commission Time as a string
        /// </summary>
        public string CommissionTimeToString
        {
            get { return Language.T(CommissionTime == CommissionTime.open ? "at opening" : "at opening and closing"); }
        }

        private void SetDefaultIndexParams(string symbol, InstrumetType instrType)
        {
            Symbol = symbol;
            InstrType = instrType;
            Comment = symbol + " " + instrType;
            Digits = 2;
            LotSize = 100;
            Spread = 4;
            SwapType = CommissionType.percents;
            SwapLong = -5;
            SwapShort = -1;
            CommissionType = CommissionType.percents;
            CommissionScope = CommissionScope.deal;
            CommissionTime = CommissionTime.openclose;
            Commission = 0.25f;
            Slippage = 0;
            PriceIn = "USD";
            RateToUSD = 1;
            RateToEUR = 1;
            BaseFileName = symbol;
        }

        private void SetDefaultForexParams(string symbol, InstrumetType instrType)
        {
            Symbol = symbol;
            InstrType = instrType;
            Comment = symbol.Substring(0, 3) + " vs " + symbol.Substring(3, 3);
            Digits = (symbol.Contains("JPY") ? 3 : 5);
            LotSize = 100000;
            Spread = 20;
            SwapType = CommissionType.pips;
            SwapLong = 2;
            SwapShort = -2;
            CommissionType = CommissionType.pips;
            CommissionScope = CommissionScope.lot;
            CommissionTime = CommissionTime.openclose;
            Commission = 0;
            Slippage = 0;
            PriceIn = symbol.Substring(3, 3);
            RateToUSD = (symbol.Contains("JPY") ? 100 : 1);
            RateToEUR = (symbol.Contains("JPY") ? 100 : 1);
            BaseFileName = symbol;
        }

        /// <summary>
        ///     Clones the Instrument_Properties.
        /// </summary>
        public InstrumentProperties Clone()
        {
            var copy = new InstrumentProperties(Symbol, InstrType)
                {
                    Symbol = Symbol,
                    InstrType = InstrType,
                    Comment = Comment,
                    Digits = Digits,
                    LotSize = LotSize,
                    Spread = Spread,
                    SwapType = SwapType,
                    SwapLong = SwapLong,
                    SwapShort = SwapShort,
                    CommissionType = CommissionType,
                    CommissionScope = CommissionScope,
                    CommissionTime = CommissionTime,
                    Commission = Commission,
                    PriceIn = PriceIn,
                    Slippage = Slippage,
                    RateToEUR = RateToEUR,
                    RateToUSD = RateToUSD,
                    BaseFileName = BaseFileName
                };
            return copy;
        }
    }
}