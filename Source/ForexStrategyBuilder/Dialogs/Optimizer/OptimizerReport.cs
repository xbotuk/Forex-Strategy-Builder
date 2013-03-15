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
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     The Optimizer
    /// </summary>
    public sealed partial class Optimizer
    {
        /// <summary>
        ///     Prepares the report string.
        /// </summary>
        private void InitReport()
        {
            // Prepare report
            sbReport = new StringBuilder();
            sbReport.Append(
                "Net Balance" + "," +
                "Max Drawdown" + "," +
                "Gross Profit" + "," +
                "Gross Loss" + "," +
                "Executed Orders" + "," +
                "Traded Lots" + "," +
                "Time in Position" + "," +
                "Sent Orders" + "," +
                "Total Charged Spread" + "," +
                "Total Charged Rollover" + "," +
                "Win / Loss Ratio" + "," +
                "Equity Percent Drawdown" + ",");

            if (Data.Strategy.UsePermanentSL)
                sbReport.Append("Permanent SL" + ",");
            if (Data.Strategy.UsePermanentTP)
                sbReport.Append("Permanent TP" + ",");
            if (Data.Strategy.UseBreakEven)
                sbReport.Append("Break Even" + ",");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        sbReport.Append(Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption + ",");

            sbReport.AppendLine();
        }

        /// <summary>
        ///     Fills a line to the report.
        /// </summary>
        private void FillInReport()
        {
            sbReport.Append(
                Backtester.NetMoneyBalance.ToString("F2") + "," +
                Backtester.MaxMoneyDrawdown.ToString("F2") + "," +
                Backtester.GrossMoneyProfit.ToString("F2") + "," +
                Backtester.GrossMoneyLoss.ToString("F2") + "," +
                Backtester.ExecutedOrders + "," +
                Backtester.TradedLots + "," +
                Backtester.TimeInPosition + "," +
                Backtester.SentOrders + "," +
                Backtester.TotalChargedMoneySpread.ToString("F2") + "," +
                Backtester.TotalChargedMoneyRollOver.ToString("F2") + "," +
                Backtester.WinLossRatio.ToString("F2") + "," +
                Backtester.MoneyEquityPercentDrawdown.ToString("F2") + ",");

            if (Data.Strategy.UsePermanentSL)
                sbReport.Append(Data.Strategy.PermanentSL + ",");
            if (Data.Strategy.UsePermanentTP)
                sbReport.Append(Data.Strategy.PermanentTP + ",");
            if (Data.Strategy.UseBreakEven)
                sbReport.Append(Data.Strategy.BreakEven + ",");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        sbReport.Append(Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value + ",");

            sbReport.AppendLine();
        }

        /// <summary>
        ///     Saves the report in a file.
        /// </summary>
        private void SaveReport()
        {
            string pathReport;
            string partilaPath = Data.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilaPath + "-Report-" + reportIndex + ".csv";
            } while (File.Exists(pathReport));

            try
            {
                using (var outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(sbReport.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}