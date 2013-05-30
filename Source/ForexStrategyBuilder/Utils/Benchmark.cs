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
using System.Diagnostics;
using System.Text;
using ForexStrategyBuilder.Common;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Utils
{
    public class Benchmark
    {
        private string accountCurrencyDefault;
        private string dataFile;
        private int initialAccountDefault;
        private InstrumentProperties instrProperties;
        private InstrumentProperties instrPropertiesDefault;
        private bool isAccountInCurrencyDefault;
        private bool isAutoscanDefault;
        private bool isCheckDataDefault;
        private bool isCutBadDataDefault;
        private bool isCutSatSunDataDefault;
        private bool isFillDataGapsDefault;
        private bool isTradeUntilMarginCallDefault;
        private bool isUseEndTimeDefault;
        private bool isUseLogicalGroupsDefault;
        private bool isUseStartTimeDefault;
        private bool isUseTickDataDefault;
        private int leverageDefault;
        private int maxBarsDefault;
        private DataPeriod period;
        private DataPeriod periodDefault;
        private Strategy strategyDefault;
        private string symbol;
        private string symbolDefault;

        public void InitializeBenchmark()
        {
            StoreConfigs();
            SetConfigs();
            GenerateDataFile();
            LoadDataFile();
        }

        public double RunLoadFilesTest(int runs)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < runs; i++)
            {
                LoadDataFile();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public double RunStrategyTest(int runs)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < runs; i++)
            {
                GenerateStrategy();
                for (int j = 0; j < 10; j++)
                    CalculateStrategy();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        private void CalculateStrategy()
        {
            foreach (IndicatorSlot indSlot in Data.Strategy.Slot)
            {
                string indicatorName = indSlot.IndicatorName;
                SlotTypes slotType = indSlot.SlotType;
                Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
                indicator.Initialize(slotType);
                indicator.IndParam = indSlot.IndParam;
                indicator.Calculate(Data.DataSet);

                indSlot.IndicatorName = indicator.IndicatorName;
                indSlot.IndParam = indicator.IndParam;
                indSlot.Component = indicator.Component;
                indSlot.SeparatedChart = indicator.SeparatedChart;
                indSlot.SpecValue = indicator.SpecialValues;
                indSlot.MinValue = indicator.SeparatedChartMinValue;
                indSlot.MaxValue = indicator.SeparatedChartMaxValue;
                indSlot.IsDefined = true;
            }

            Data.FirstBar = Data.Strategy.SetFirstBar();
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
            Data.IsResult = true;
            StatsBuffer.UpdateStatsBuffer();
        }

        private void GenerateStrategy()
        {
            var strategy = new Strategy(4, 2)
                {
                    SameSignalAction = SameDirSignalAction.Add,
                    OppSignalAction = OppositeDirSignalAction.Nothing,
                    PermanentSL = 500,
                    UsePermanentSL = true,
                    PermanentTP = 500,
                    UsePermanentTP = true,
                    BreakEven = 500,
                    UseBreakEven = true,
                    EntryLots = 0.1,
                    AddingLots = 0.2,
                    UseMartingale = true,
                    MartingaleMultiplier = 2
                };

            Indicator indicator = IndicatorManager.ConstructIndicator("Bar Opening");
            indicator.Initialize(SlotTypes.Open);
            strategy.Slot[0].IndicatorName = indicator.IndicatorName;
            strategy.Slot[0].IndParam = indicator.IndParam.Clone();
            strategy.Slot[0].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("Moving Average");
            indicator.Initialize(SlotTypes.OpenFilter);
            strategy.Slot[1].IndicatorName = indicator.IndicatorName;
            strategy.Slot[1].IndParam = indicator.IndParam.Clone();
            strategy.Slot[1].LogicalGroup = "A";
            strategy.Slot[1].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("MACD");
            indicator.Initialize(SlotTypes.OpenFilter);
            strategy.Slot[2].IndicatorName = indicator.IndicatorName;
            strategy.Slot[2].IndParam = indicator.IndParam.Clone();
            strategy.Slot[2].LogicalGroup = "B";
            strategy.Slot[2].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("RSI");
            indicator.Initialize(SlotTypes.OpenFilter);
            strategy.Slot[3].IndicatorName = indicator.IndicatorName;
            strategy.Slot[3].IndParam = indicator.IndParam.Clone();
            strategy.Slot[3].LogicalGroup = "C";
            strategy.Slot[3].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("ADX");
            indicator.Initialize(SlotTypes.OpenFilter);
            strategy.Slot[4].IndicatorName = indicator.IndicatorName;
            strategy.Slot[4].IndParam = indicator.IndParam.Clone();
            strategy.Slot[4].LogicalGroup = "D";
            strategy.Slot[4].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("Bar Closing");
            indicator.Initialize(SlotTypes.Close);
            strategy.Slot[5].IndicatorName = indicator.IndicatorName;
            strategy.Slot[5].IndParam = indicator.IndParam.Clone();
            strategy.Slot[5].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("Moving Average");
            indicator.Initialize(SlotTypes.CloseFilter);
            strategy.Slot[6].IndicatorName = indicator.IndicatorName;
            strategy.Slot[6].IndParam = indicator.IndParam.Clone();
            strategy.Slot[6].LogicalGroup = "a";
            strategy.Slot[6].IsDefined = true;

            indicator = IndicatorManager.ConstructIndicator("RSI");
            indicator.Initialize(SlotTypes.CloseFilter);
            strategy.Slot[7].IndicatorName = indicator.IndicatorName;
            strategy.Slot[7].IndParam = indicator.IndParam.Clone();
            strategy.Slot[7].LogicalGroup = "b";
            strategy.Slot[7].IsDefined = true;

            Data.Strategy = strategy;
        }

        private void GenerateDataFile()
        {
            symbol = "GBPUSD";
            instrProperties = new InstrumentProperties(symbol, InstrumetType.Forex)
                {
                    Commission = 0,
                    CommissionScope = CommissionScope.deal,
                    CommissionTime = CommissionTime.openclose,
                    Digits = 5,
                    LotSize = 10000,
                    RateToEUR = 2.3456,
                    RateToUSD = 2.3456,
                    Slippage = 1,
                    Spread = 6,
                    SwapLong = 0.25,
                    SwapShort = 0.25,
                    SwapUnit = ChargeUnit.Points
                };

            int maxBars = 2*Configs.MaxBars;
            var sb = new StringBuilder(maxBars);
            var time = new DateTime(2000, 1, 1, 0, 0, 0);
            double open = 1.12345;
            int volume = 300;
            period = DataPeriod.H4;
            for (int i = 0; i < maxBars; i++)
            {
                time = time.AddMinutes((int) DataPeriod.H1);
                int multiplier = (time.DayOfYear%2 == 0) ? 1 : -1;
                open = open + multiplier*0.00005;
                double high = open + 0.00025;
                double low = open - 0.00025;
                double close = open + 0.00005;
                volume = volume + multiplier*5;

                sb.AppendLine(string.Format("{0} {1} {2} {3} {4} {5}",
                                            time.ToString("yyyy-MM-dd HH:mm:ss"),
                                            open.ToString("F5"),
                                            high.ToString("F5"),
                                            low.ToString("F5"),
                                            close.ToString("F5"),
                                            volume));
            }

            dataFile = sb.ToString();
        }

        private void LoadDataFile()
        {
            //  Makes an instance of class Instrument
            var instrument = new Instrument(instrProperties, (int) period)
                {
                    DataDir = Data.OfflineDataDir,
                    MaxBars = Configs.MaxBars,
                    StartTime = Configs.DataStartTime,
                    EndTime = Configs.DataEndTime,
                    UseStartTime = Configs.UseStartTime,
                    UseEndTime = Configs.UseEndTime
                };

            instrument.LoadResourceData(dataFile, period);
            Data.InstrProperties = instrProperties.Clone();

            Data.Bars = instrument.Bars;
            Data.Period = period;
            Data.Update = instrument.Update;

            Data.Time = new DateTime[Data.Bars];
            Data.Open = new double[Data.Bars];
            Data.High = new double[Data.Bars];
            Data.Low = new double[Data.Bars];
            Data.Close = new double[Data.Bars];
            Data.Volume = new int[Data.Bars];

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                Data.Open[bar] = instrument.Open(bar);
                Data.High[bar] = instrument.High(bar);
                Data.Low[bar] = instrument.Low(bar);
                Data.Close[bar] = instrument.Close(bar);
                Data.Time[bar] = instrument.Time(bar);
                Data.Volume[bar] = instrument.Volume(bar);
            }

            Data.MinPrice = instrument.MinPrice;
            Data.MaxPrice = instrument.MaxPrice;
            Data.DaysOff = instrument.DaysOff;
            Data.AverageGap = instrument.AverageGap;
            Data.MaxGap = instrument.MaxGap;
            Data.AverageHighLow = instrument.AverageHighLow;
            Data.MaxHighLow = instrument.MaxHighLow;
            Data.AverageCloseOpen = instrument.AverageCloseOpen;
            Data.MaxCloseOpen = instrument.MaxCloseOpen;
            Data.DataCut = instrument.Cut;
            Data.IsIntrabarData = false;
            Data.IsTickData = false;
            Data.IsData = true;
            Data.IsResult = false;

            Data.GenerateMarketStats();
        }


        public void FinishBenchmark()
        {
            ResetConfigs();
        }


        private void StoreConfigs()
        {
            maxBarsDefault = Configs.MaxBars;
            isCheckDataDefault = Configs.CheckData;
            isFillDataGapsDefault = Configs.FillInDataGaps;
            isCutBadDataDefault = Configs.CutBadData;
            isCutSatSunDataDefault = Configs.CutSatSunData;
            isUseEndTimeDefault = Configs.UseEndTime;
            isUseStartTimeDefault = Configs.UseStartTime;
            isUseTickDataDefault = Configs.UseTickData;
            isAutoscanDefault = Configs.Autoscan;

            isAccountInCurrencyDefault = Configs.AccountInMoney;
            accountCurrencyDefault = Configs.AccountCurrency;
            initialAccountDefault = Configs.InitialAccount;
            leverageDefault = Configs.Leverage;
            isTradeUntilMarginCallDefault = Configs.TradeUntilMarginCall;
            isUseLogicalGroupsDefault = Configs.UseLogicalGroups;

            symbolDefault = Data.Symbol;
            periodDefault = Data.Period;
            instrPropertiesDefault = Instruments.InstrumentList[symbolDefault].Clone();

            strategyDefault = Data.Strategy.Clone();
        }

        private void SetConfigs()
        {
            Configs.MaxBars = 50000;
            Configs.CheckData = true;
            Configs.FillInDataGaps = false;
            Configs.CutBadData = false;
            Configs.CutSatSunData = true;
            Configs.UseEndTime = false;
            Configs.UseStartTime = false;
            Configs.UseTickData = false;
            Configs.Autoscan = false;

            Configs.AccountInMoney = true;
            Configs.AccountCurrency = "EUR";
            Configs.InitialAccount = 100000;
            Configs.Leverage = 100;
            Configs.TradeUntilMarginCall = false;
            Configs.UseLogicalGroups = true;
        }

        private void ResetConfigs()
        {
            Configs.MaxBars = maxBarsDefault;
            Configs.CheckData = isCheckDataDefault;
            Configs.FillInDataGaps = isFillDataGapsDefault;
            Configs.CutBadData = isCutBadDataDefault;
            Configs.CutSatSunData = isCutSatSunDataDefault;
            Configs.UseEndTime = isUseEndTimeDefault;
            Configs.UseStartTime = isUseStartTimeDefault;
            Configs.UseTickData = isUseTickDataDefault;
            Configs.Autoscan = isAutoscanDefault;

            Configs.AccountInMoney = isAccountInCurrencyDefault;
            Configs.AccountCurrency = accountCurrencyDefault;
            Configs.InitialAccount = initialAccountDefault;
            Configs.Leverage = leverageDefault;
            Configs.TradeUntilMarginCall = isTradeUntilMarginCallDefault;
            Configs.UseLogicalGroups = isUseLogicalGroupsDefault;

            Data.Period = periodDefault;
            Instruments.InstrumentList[symbolDefault] = instrPropertiesDefault.Clone();
            Data.Strategy = strategyDefault.Clone();
        }
    }
}