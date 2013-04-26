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
using System.Drawing;
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators
{
    public class Volumes : Indicator
    {
        public Volumes()
        {
            // General properties
            IndicatorName = "Volumes";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChartMinValue = 0;
            SeparatedChart  = true;
        }

        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Volume rises",
                "The Volume falls",
                "The Volume is higher than the Level line",
                "The Volume is lower than the Level line",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Level";
            IndParam.NumParam[0].Value   = 1000;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 100000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A critical level (for the appropriate logic).";

            // The CheckBox parameters
            IndParam.CheckParam[0].Caption = "Use previous bar value";
            IndParam.CheckParam[0].Enabled = true;
            IndParam.CheckParam[0].ToolTip = "Use the indicator value from the previous bar.";

            return;
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Reading the parameters
            double dLevel = IndParam.NumParam[0].Value;
            int    iPrvs  = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            double[] adVolumes = new double[Bars];

            int iFirstBar = iPrvs + 1;

            for (int iBar = 0; iBar < Bars; iBar++)
            {
                adVolumes[iBar] = Volume[iBar];
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Volumes";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adVolumes;

            Component[1] = new IndicatorComp();
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = new double[Bars];

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            // Sets the Component's type
            if (SlotType == SlotTypes.OpenFilter)
            {
                Component[1].DataType = IndComponentType.AllowOpenLong;
                Component[1].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenShort;
                Component[2].CompName = "Is short entry allowed";
            }
            else if (SlotType == SlotTypes.CloseFilter)
            {
                Component[1].DataType = IndComponentType.ForceCloseLong;
                Component[1].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseShort;
                Component[2].CompName = "Close out short position";
            }

            // Calculation of the logic
            switch (IndParam.ListParam[0].Text)
            {
                case "The Volume rises":
                    for (int iBar = iPrvs + 1; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] > adVolumes[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] > adVolumes[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                    }
                    break;

                case "The Volume falls":
                    for (int iBar = iPrvs + 1; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] < adVolumes[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] < adVolumes[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Volume is higher than the Level line":
                    for (int iBar = iPrvs; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] > dLevel + Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] > dLevel + Sigma() ? 1 : 0;
                    }
                    SpecialValues = new double[1] { dLevel };
                    break;

                case "The Volume is lower than the Level line":
                    for (int iBar = iPrvs; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] < dLevel - Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] < dLevel - Sigma() ? 1 : 0;
                    }
                    SpecialValues = new double[1] { dLevel };
                    break;

                default:
                    break;
            }

            return;
        }

        public override void SetDescription()
        {
            string sLevelLong  = IndParam.NumParam[0].ValueToString;
            string sLevelShort = IndParam.NumParam[0].ValueToString;

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Volume rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Volume falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Volume is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The Volume is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level " + sLevelShort;
                    break;

                default:
                    break;
            }

            return;
        }

        public override string ToString()
        {
            return IndicatorName + (IndParam.CheckParam[0].Checked ? "*" : "");
        }
    }
}
 