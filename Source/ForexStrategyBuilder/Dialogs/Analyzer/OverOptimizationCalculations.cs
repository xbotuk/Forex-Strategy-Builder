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
using System.IO;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    public sealed partial class OverOptimization
    {
        private int computedCycles; // Currently completed cycles.
        private int countStratParams;
        private int cycles; // Count of the cycles.
        private int deviationSteps;

        private string listParametersName;
        private List<string> paramNames;

        private string pathReportFile;

        private int progressPercent; // Reached progress in %.
        private OverOptimizationDataTable tableParameters;
        private OverOptimizationDataTable[] tableReport;
        private int tablesCount; // The count of data tables.

        /// <summary>
        ///     Counts the numeric parameters of the strategy.
        /// </summary>
        private void CountStrategyParams()
        {
            countStratParams = 0;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        countStratParams++;
        }

        /// <summary>
        ///     Sets table with parameter values.
        /// </summary>
        private void SetParametersValues(int percentDeviation, int countParam)
        {
            paramNames = new List<string>();
            listParametersName = "Index" + Configs.ColumnSeparator + "Parameter name" + Environment.NewLine;
            countStratParams = 0;
            cycles = 0;
            deviationSteps = 2*percentDeviation + 1;

            tableParameters = new OverOptimizationDataTable(percentDeviation, countParam)
                {Name = "Values of the Parameters"};

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && countStratParams < countParam)
                    {
                        NumericParam currentParam = Data.Strategy.Slot[slot].IndParam.NumParam[numParam];
                        double minVal = currentParam.Min;
                        double maxVal = currentParam.Max;
                        int point = currentParam.Point;
                        double originalValue = currentParam.Value;
                        double deltaStep = originalValue/100.0;

                        for (int p = 0; p < deviationSteps; p++)
                        {
                            int index = p - percentDeviation;
                            double value = originalValue + index*deltaStep;
                            value = Math.Round(value, point);

                            if (index == 0)
                                value = originalValue;
                            if (value < minVal)
                                value = minVal;
                            if (value > maxVal)
                                value = maxVal;

                            tableParameters.SetData(index, countStratParams, value);
                            cycles++;
                        }

                        paramNames.Add(currentParam.Caption);
                        listParametersName += (countStratParams + 1) + Configs.ColumnSeparator + currentParam.Caption +
                                              Environment.NewLine;
                        countStratParams++;
                    }

            for (int prm = countStratParams; prm < countParam; prm++)
                listParametersName += (prm + 1) + Environment.NewLine;
            listParametersName += Environment.NewLine;
        }

        /// <summary>
        ///     Calculates Data Tables.
        /// </summary>
        private void CalculateStatsTables(int percentDeviation, int countParam)
        {
            string unit = " " + Configs.AccountCurrency;

            var tableNames = new[]
                {
                    Language.T("Account balance") + unit,
                    Language.T("Profit per day") + unit,
                    Language.T("Maximum drawdown") + unit,
                    Language.T("Gross profit") + unit,
                    Language.T("Gross loss") + unit,
                    Language.T("Executed orders"),
                    Language.T("Traded lots"),
                    Language.T("Time in position") + " %",
                    Language.T("Sent orders"),
                    Language.T("Charged spread") + unit,
                    Language.T("Charged rollover") + unit,
                    Language.T("Winning trades"),
                    Language.T("Losing trades"),
                    Language.T("Win/loss ratio"),
                    Language.T("Max equity drawdown") + " %"
                };

            tablesCount = tableNames.Length;
            tableReport = new OverOptimizationDataTable[tablesCount];

            for (int t = 0; t < tableNames.Length; t++)
            {
                tableReport[t] = new OverOptimizationDataTable(percentDeviation, countParam) {Name = tableNames[t]};
            }

            int parNumber = 0;
            bool isBgWorkCanceled = false;
            for (int slot = 0; slot < Data.Strategy.Slots && !isBgWorkCanceled; slot++)
                for (int numParam = 0; numParam < 6 && !isBgWorkCanceled; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && parNumber < countParam)
                    {
                        for (int index = percentDeviation; index >= -percentDeviation && !isBgWorkCanceled; index--)
                        {
                            isBgWorkCanceled = bgWorker.CancellationPending;
                            Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = tableParameters.GetData(
                                index, parNumber);

                            CalculateIndicator(slot);
                            Backtester.Calculate();
                            Backtester.CalculateAccountStats();

                            var statValues = new[]
                                {
                                    Backtester.NetMoneyBalance,
                                    Backtester.MoneyProfitPerDay,
                                    Backtester.MaxMoneyDrawdown,
                                    Backtester.GrossMoneyProfit,
                                    Backtester.GrossMoneyLoss,
                                    Backtester.ExecutedOrders,
                                    Backtester.TradedLots,
                                    Backtester.TimeInPosition,
                                    Backtester.SentOrders,
                                    Backtester.TotalChargedMoneySpread,
                                    Backtester.TotalChargedMoneyRollOver,
                                    Backtester.WinningTrades,
                                    Backtester.LosingTrades,
                                    Backtester.WinLossRatio,
                                    Backtester.MoneyEquityPercentDrawdown
                                };

                            for (int tn = 0; tn < tablesCount; tn++)
                                tableReport[tn].SetData(index, parNumber, statValues[tn]);

                            // Report progress as a percentage of the total task.
                            computedCycles++;
                            int percentComplete = Math.Min(100*computedCycles/cycles, 100);
                            if (percentComplete > progressPercent)
                            {
                                progressPercent = percentComplete;
                                bgWorker.ReportProgress(percentComplete);
                            }
                        }

                        // Set default value
                        Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = tableParameters.GetData(0,
                                                                                                             parNumber);
                        CalculateIndicator(slot);
                        parNumber++;
                    }
        }

        /// <summary>
        ///     Calculates the indicator in the designated slot.
        /// </summary>
        private void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Data.Strategy.Slot[slot].IndParam;

            Indicator indicator = IndicatorManager.ConstructIndicator(ip.IndicatorName);
            indicator.Initialize(ip.SlotType);

            // List parameters
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric parameters
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check parameters
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(Data.DataSet);

            // Sets Data.Strategy
            Data.Strategy.Slot[slot].IndicatorName = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam = indicator.IndParam;
            Data.Strategy.Slot[slot].Component = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined = true;

            // Searches the indicators' components to determine the Data.FirstBar 
            Data.FirstBar = Data.Strategy.SetFirstBar();
        }

        /// <summary>
        ///     Generates the Over-optimization report.
        /// </summary>
        private string GenerateReport()
        {
            string report = listParametersName + tableParameters.DataToString();
            foreach (OverOptimizationDataTable table in tableReport)
                report += table.DataToString();

            return report;
        }

        /// <summary>
        ///     Saves the report in a file.
        /// </summary>
        private void SaveReport(string report)
        {
            string pathReport;
            string partilPath = Data.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilPath + "-Over-optimization_Report-" + reportIndex + ".csv";
            } while (File.Exists(pathReport));

            try
            {
                using (var outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(report);
                    pathReportFile = pathReport;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}