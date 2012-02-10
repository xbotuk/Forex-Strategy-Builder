// Strategy Optimizer - Report
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer
    /// </summary>
    public partial class Optimizer : Form
    {
        /// <summary>
        /// Prepares the report string.
        /// </summary>
        void InitReport()
        {
            // Prepare report
            sbReport = new StringBuilder();
            sbReport.Append(
                "Net Balance"             + "," +
                "Max Drawdown"            + "," +
                "Gross Profit"            + "," +
                "Gross Loss"              + "," +
                "Executed Orders"         + "," +
                "Traded Lots"             + "," +
                "Time in Position"        + "," +
                "Sent Orders"             + "," +
                "Total Charged Spread"    + "," +
                "Total Charged Rollover"  + "," +
                "Win / Loss Ratio"        + "," +
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
        ///  Fills a line to the report.
        /// </summary>
        void FillInReport()
        {
            sbReport.Append(
                Backtester.NetMoneyBalance.ToString("F2")            + "," +
                Backtester.MaxMoneyDrawdown.ToString("F2")           + "," +
                Backtester.GrossMoneyProfit.ToString("F2")           + "," +
                Backtester.GrossMoneyLoss.ToString("F2")             + "," +
                Backtester.ExecutedOrders.ToString()                 + "," +
                Backtester.TradedLots.ToString()                     + "," +
                Backtester.TimeInPosition.ToString()                 + "," +
                Backtester.SentOrders.ToString()                     + "," +
                Backtester.TotalChargedMoneySpread.ToString("F2")    + "," +
                Backtester.TotalChargedMoneyRollOver.ToString("F2")  + "," +
                Backtester.WinLossRatio.ToString("F2")               + "," +
                Backtester.MoneyEquityPercentDrawdown.ToString("F2") + ",");

            if (Data.Strategy.UsePermanentSL)
                sbReport.Append(Data.Strategy.PermanentSL.ToString() + ",");
            if (Data.Strategy.UsePermanentTP)
                sbReport.Append(Data.Strategy.PermanentTP.ToString() + ",");
            if (Data.Strategy.UseBreakEven)
                sbReport.Append(Data.Strategy.BreakEven.ToString() + ",");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        sbReport.Append(Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value.ToString() + ",");

            sbReport.AppendLine();
        }

        /// <summary>
        /// Saves the report in a file.
        /// </summary>
        void SaveReport()
        {
            string pathReport  = Data.StrategyPath.Replace(".xml", ".csv");
            string partilaPath = Data.StrategyPath.Replace(".xml", "");
            int    reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilaPath + "-report-" + reportIndex.ToString() + ".csv";

            } while (File.Exists(pathReport));

            try
            {
                using (StreamWriter outfile = new StreamWriter(pathReport))
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
