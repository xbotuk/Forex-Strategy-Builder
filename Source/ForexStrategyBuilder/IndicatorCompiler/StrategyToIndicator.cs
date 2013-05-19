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
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public static class StrategyToIndicator
    {
        public static void ExportStrategyToIndicator()
        {
            var sbLong = new StringBuilder();
            var sbShort = new StringBuilder();

            for (int bar = Data.FirstBar; bar < Data.Bars; bar++)
                for (int pos = 0; pos < Backtester.Positions(bar); pos++)
                {
                    if (Backtester.PosDir(bar, pos) == PosDirection.Long)
                        sbLong.AppendLine("				\"" + Data.Time[bar] + "\",");

                    if (Backtester.PosDir(bar, pos) == PosDirection.Short)
                        sbShort.AppendLine("				\"" + Data.Time[bar] + "\",");
                }

            string strategy = Resources.StrategyToIndicator;
            strategy = strategy.Replace("#MODIFIED#", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#INSTRUMENT#", Data.Symbol);
            strategy = strategy.Replace("#BASEPERIOD#", Data.DataPeriodToString(Data.Period));
            strategy = strategy.Replace("#STARTDATE#", Data.Time[Data.FirstBar].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#ENDDATE#", Data.Time[Data.Bars - 1].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#PERIODMINUTES#", ((int) Data.Period).ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#LISTLONG#", sbLong.ToString());
            strategy = strategy.Replace("#LISTSHORT#", sbShort.ToString());

            var savedlg = new SaveFileDialog
                {
                    InitialDirectory = Data.SourceFolder,
                    AddExtension = true,
                    Title = Language.T("Custom Indicators"),
                    Filter = Language.T("Custom Indicators") + " (*.cs)|*.cs"
                };

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                strategy = strategy.Replace("#INDICATORNAME#", Path.GetFileNameWithoutExtension(savedlg.FileName));
                var sw = new StreamWriter(savedlg.FileName);
                try
                {
                    sw.Write(strategy);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Language.T("Custom Indicators"));
                }
                finally
                {
                    sw.Close();
                }
            }
        }
    }
}