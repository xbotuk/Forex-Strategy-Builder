// IndicatorStore Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.using System;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public static class IndicatorStore
    {
        private static readonly Dictionary<string, Indicator> OriginalIndicators = new Dictionary<string, Indicator>();

        // Stores the custom indicators
        private static readonly SortableDictionary<string, Indicator> CustomIndicators = new SortableDictionary<string, Indicator>();

        // Stores all the indicators
        private static readonly SortableDictionary<string, Indicator> AllIndicators = new SortableDictionary<string, Indicator>();

        /// <summary>
        /// Constructor.
        /// </summary>
        static IndicatorStore()
        {
            ClosingIndicatorsWithClosingFilters = new List<string>();
            CloseFilterIndicators = new List<string>();
            OpenFilterIndicators = new List<string>();
            ClosePointIndicators = new List<string>();
            OpenPointIndicators = new List<string>();
            AddOriginalIndicators();

            foreach (var record in OriginalIndicators)
                AllIndicators.Add(record.Key, record.Value);
        }

        /// <summary>
        /// Gets the names of all the original indicators
        /// </summary>
        public static IEnumerable<string> OriginalIndicatorNames
        {
            get { return new List<string>(OriginalIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all custom indicators
        /// </summary>
        public static List<string> CustomIndicatorNames
        {
            get { return new List<string>(CustomIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all indicators.
        /// </summary>
        public static List<string> IndicatorNames
        {
            get { return new List<string>(AllIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all Opening Point indicators.
        /// </summary>
        public static List<string> OpenPointIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Closing Point indicators.
        /// </summary>
        public static List<string> ClosePointIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Opening Filter indicators.
        /// </summary>
        public static List<string> OpenFilterIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Closing Filter indicators.
        /// </summary>
        public static List<string> CloseFilterIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all losing Point indicators that allow use of Closing Filter indicators.
        /// </summary>
        public static List<string> ClosingIndicatorsWithClosingFilters { get; private set; }

        /// <summary>
        /// Adds all indicators to the store.
        /// </summary>
        private static void AddOriginalIndicators()
        {
            OriginalIndicators.Add("Accelerator Oscillator", new Accelerator_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Account Percent Stop", new Account_Percent_Stop(SlotTypes.NotDefined));
            OriginalIndicators.Add("Accumulation Distribution", new Accumulation_Distribution(SlotTypes.NotDefined));
            OriginalIndicators.Add("ADX", new ADX(SlotTypes.NotDefined));
            OriginalIndicators.Add("Alligator", new Alligator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Aroon Histogram", new Aroon_Histogram(SlotTypes.NotDefined));
            OriginalIndicators.Add("ATR MA Oscillator", new ATR_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("ATR Stop", new ATR_Stop(SlotTypes.NotDefined));
            OriginalIndicators.Add("Average True Range", new Average_True_Range(SlotTypes.NotDefined));
            OriginalIndicators.Add("Awesome Oscillator", new Awesome_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Balance of Power", new Balance_of_Power(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Closing", new Bar_Closing(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Opening", new Bar_Opening(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Range", new Bar_Range(SlotTypes.NotDefined));
            OriginalIndicators.Add("BBP MA Oscillator", new BBP_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bears Power", new Bears_Power(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bollinger Bands", new Bollinger_Bands(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bulls Bears Power", new Bulls_Bears_Power(SlotTypes.NotDefined));
            OriginalIndicators.Add("Bulls Power", new Bulls_Power(SlotTypes.NotDefined));
            OriginalIndicators.Add("CCI MA Oscillator", new CCI_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Close and Reverse", new Close_and_Reverse(SlotTypes.NotDefined));
            OriginalIndicators.Add("Commodity Channel Index", new Commodity_Channel_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Cumulative Sum", new Cumulative_Sum(SlotTypes.NotDefined));
            OriginalIndicators.Add("Data Bars Filter", new DataBarsFilter(SlotTypes.NotDefined));
            OriginalIndicators.Add("Date Filter", new DateFilter(SlotTypes.NotDefined));
            OriginalIndicators.Add("Day Closing", new Day_Closing(SlotTypes.NotDefined));
            OriginalIndicators.Add("Day of Week", new Day_of_Week(SlotTypes.NotDefined));
            OriginalIndicators.Add("Day Opening", new Day_Opening(SlotTypes.NotDefined));
            OriginalIndicators.Add("DeMarker", new DeMarker(SlotTypes.NotDefined));
            OriginalIndicators.Add("Detrended Oscillator", new Detrended_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Directional Indicators", new Directional_Indicators(SlotTypes.NotDefined));
            OriginalIndicators.Add("Donchian Channel", new Donchian_Channel(SlotTypes.NotDefined));
            OriginalIndicators.Add("Ease of Movement", new Ease_of_Movement(SlotTypes.NotDefined));
            OriginalIndicators.Add("Enter Once", new Enter_Once(SlotTypes.NotDefined));
            OriginalIndicators.Add("Entry Hour", new Entry_Hour(SlotTypes.NotDefined));
            OriginalIndicators.Add("Entry Time", new Entry_Time(SlotTypes.NotDefined));
            OriginalIndicators.Add("Envelopes", new Envelopes(SlotTypes.NotDefined));
            OriginalIndicators.Add("Exit Hour", new Exit_Hour(SlotTypes.NotDefined));
            OriginalIndicators.Add("Fisher Transform", new Fisher_Transform(SlotTypes.NotDefined));
            OriginalIndicators.Add("Force Index", new Force_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Fractal", new Fractal(SlotTypes.NotDefined));
            OriginalIndicators.Add("Heiken Ashi", new Heiken_Ashi(SlotTypes.NotDefined));
            OriginalIndicators.Add("Hourly High Low", new Hourly_High_Low(SlotTypes.NotDefined));
            OriginalIndicators.Add("Ichimoku Kinko Hyo", new Ichimoku_Kinko_Hyo(SlotTypes.NotDefined));
            OriginalIndicators.Add("Inside Bar", new Inside_Bar(SlotTypes.NotDefined));
            OriginalIndicators.Add("Keltner Channel", new Keltner_Channel(SlotTypes.NotDefined));
            OriginalIndicators.Add("Long or Short", new Long_or_Short(SlotTypes.NotDefined));
            OriginalIndicators.Add("Lot Limiter", new Lot_Limiter(SlotTypes.NotDefined));
            OriginalIndicators.Add("MA Oscillator", new MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("MACD Histogram", new MACD_Histogram(SlotTypes.NotDefined));
            OriginalIndicators.Add("MACD", new MACD(SlotTypes.NotDefined));
            OriginalIndicators.Add("Market Facilitation Index", new Market_Facilitation_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Momentum MA Oscillator", new Momentum_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Momentum", new Momentum(SlotTypes.NotDefined));
            OriginalIndicators.Add("Money Flow Index", new Money_Flow_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Money Flow", new Money_Flow(SlotTypes.NotDefined));
            OriginalIndicators.Add("Moving Average", new Moving_Average(SlotTypes.NotDefined));
            OriginalIndicators.Add("Moving Averages Crossover", new Moving_Averages_Crossover(SlotTypes.NotDefined));
            OriginalIndicators.Add("N Bars Exit", new N_Bars_Exit(SlotTypes.NotDefined));
            OriginalIndicators.Add("Narrow Range", new Narrow_Range(SlotTypes.NotDefined));
            OriginalIndicators.Add("OBOS MA Oscillator", new OBOS_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("On Balance Volume", new On_Balance_Volume(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of ATR", new Oscillator_of_ATR(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of BBP", new Oscillator_of_BBP(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of CCI", new Oscillator_of_CCI(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of MACD", new Oscillator_of_MACD(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of Momentum", new Oscillator_of_Momentum(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of OBOS", new Oscillator_of_OBOS(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of ROC", new Oscillator_of_ROC(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of RSI", new Oscillator_of_RSI(SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of Trix", new Oscillator_of_Trix(SlotTypes.NotDefined));
            OriginalIndicators.Add("Overbought Oversold Index", new Overbought_Oversold_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Parabolic SAR", new Parabolic_SAR(SlotTypes.NotDefined));
            OriginalIndicators.Add("Percent Change", new Percent_Change(SlotTypes.NotDefined));
            OriginalIndicators.Add("Pivot Points", new Pivot_Points(SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous Bar Closing", new Previous_Bar_Closing(SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous Bar Opening", new Previous_Bar_Opening(SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous High Low", new Previous_High_Low(SlotTypes.NotDefined));
            OriginalIndicators.Add("Price Move", new Price_Move(SlotTypes.NotDefined));
            OriginalIndicators.Add("Price Oscillator", new Price_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Random Filter", new Random_Filter(SlotTypes.NotDefined));
            OriginalIndicators.Add("Rate of Change", new Rate_of_Change(SlotTypes.NotDefined));
            OriginalIndicators.Add("Relative Vigor Index", new Relative_Vigor_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("ROC MA Oscillator", new ROC_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Ross Hook", new Ross_Hook(SlotTypes.NotDefined));
            OriginalIndicators.Add("Round Number", new Round_Number(SlotTypes.NotDefined));
            OriginalIndicators.Add("RSI MA Oscillator", new RSI_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("RSI", new RSI(SlotTypes.NotDefined));
            OriginalIndicators.Add("Standard Deviation", new Standard_Deviation(SlotTypes.NotDefined));
            OriginalIndicators.Add("Starc Bands", new Starc_Bands(SlotTypes.NotDefined));
            OriginalIndicators.Add("Steady Bands", new Steady_Bands(SlotTypes.NotDefined));
            OriginalIndicators.Add("Stochastics", new Stochastics(SlotTypes.NotDefined));
            OriginalIndicators.Add("Stop Limit", new Stop_Limit(SlotTypes.NotDefined));
            OriginalIndicators.Add("Stop Loss", new Stop_Loss(SlotTypes.NotDefined));
            OriginalIndicators.Add("Take Profit", new Take_Profit(SlotTypes.NotDefined));
            OriginalIndicators.Add("Top Bottom Price", new Top_Bottom_Price(SlotTypes.NotDefined));
            OriginalIndicators.Add("Trailing Stop Limit", new Trailing_Stop_Limit(SlotTypes.NotDefined));
            OriginalIndicators.Add("Trailing Stop", new Trailing_Stop(SlotTypes.NotDefined));
            OriginalIndicators.Add("Trix Index", new Trix_Index(SlotTypes.NotDefined));
            OriginalIndicators.Add("Trix MA Oscillator", new Trix_MA_Oscillator(SlotTypes.NotDefined));
            OriginalIndicators.Add("Week Closing", new Week_Closing(SlotTypes.NotDefined));
            OriginalIndicators.Add("Williams' Percent Range", new Williams_Percent_Range(SlotTypes.NotDefined));
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> GetIndicatorNames(SlotTypes slotType)
        {
            var list = new List<string>();

            foreach (var record in AllIndicators)
                if (record.Value.TestPossibleSlot(slotType))
                    list.Add(record.Value.IndicatorName);

            return list;
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> ListIndicatorNames(SlotTypes slotType)
        {
            switch (slotType)
            {
                case SlotTypes.NotDefined:
                    break;
                case SlotTypes.Open:
                    return OpenPointIndicators;
                case SlotTypes.OpenFilter:
                    return OpenFilterIndicators;
                case SlotTypes.Close:
                    return ClosePointIndicators;
                case SlotTypes.CloseFilter:
                    return CloseFilterIndicators;
            }

            return null;
        }

        /// <summary>
        /// Resets the custom indicators in the custom indicators list.
        /// </summary>
        public static void ResetCustomIndicators(IEnumerable<Indicator> indicatorList)
        {
            CustomIndicators.Clear();

            if (indicatorList == null)
                return;

            foreach (Indicator indicator in indicatorList)
                if (!CustomIndicators.ContainsKey(indicator.IndicatorName))
                    CustomIndicators.Add(indicator.IndicatorName, indicator);

            CustomIndicators.Sort();
        }

        /// <summary>
        /// Clears the indicator list and adds to it the original indicators.
        /// </summary>
        public static void CombineAllIndicators()
        {
            AllIndicators.Clear();

            foreach (var record in OriginalIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);

            foreach (var record in CustomIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);

            AllIndicators.Sort();

            OpenPointIndicators = GetIndicatorNames(SlotTypes.Open);
            ClosePointIndicators = GetIndicatorNames(SlotTypes.Close);
            OpenFilterIndicators = GetIndicatorNames(SlotTypes.OpenFilter);
            CloseFilterIndicators = GetIndicatorNames(SlotTypes.CloseFilter);

            foreach (string indicatorName in ClosePointIndicators)
            {
                Indicator indicator = ConstructIndicator(indicatorName, SlotTypes.Close);
                if (indicator.AllowClosingFilters)
                    ClosingIndicatorsWithClosingFilters.Add(indicatorName);
            }
        }

        /// <summary>
        /// Constructs an indicator with specified name and slot type.
        /// </summary>
        public static Indicator ConstructIndicator(string indicatorName, SlotTypes slotType)
        {
            if (!AllIndicators.ContainsKey(indicatorName))
            {
                MessageBox.Show("There is no indicator named: " + indicatorName);
                return null;
            }

            Type indicatorType = AllIndicators[indicatorName].GetType();
            var parameterType = new[] {slotType.GetType()};
            ConstructorInfo constructorInfo = indicatorType.GetConstructor(parameterType);
            if (constructorInfo != null)
                return (Indicator) constructorInfo.Invoke(new object[] {slotType});

            MessageBox.Show("Error with indicator named: " + indicatorName);
            return null;
        }
    }
}