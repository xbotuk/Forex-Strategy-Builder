//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Drawing;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Accumulation Distribution Indicator
    /// </summary>
    public class AccumulationDistribution : Indicator
    {
        /// <summary>
        ///     Sets the default indicator parameters for the designated slot type
        /// </summary>
        public AccumulationDistribution(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Accumulation Distribution";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam {IndicatorName = IndicatorName, SlotType = slotType};

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[]
                {
                    "The AD rises",
                    "The AD falls",
                    "The AD changes its direction upward",
                    "The AD changes its direction downward"
                };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The CheckBox parameters
            IndParam.CheckParam[0].Caption = "Use previous bar value";
            IndParam.CheckParam[0].Checked = PrepareUsePrevBarValueCheckBox(slotType);
            IndParam.CheckParam[0].Enabled = true;
            IndParam.CheckParam[0].ToolTip = "Use the indicator value from the previous bar.";
        }

        /// <summary>
        ///     Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iPrvs = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            const int iFirstBar = 3;

            var adAd = new double[Bars];

            adAd[0] = (Close[0] - Low[0]) - (High[0] - Close[0]);
            if ((High[0] - Low[0]) > 0)
            {
                adAd[0] = adAd[0]/(High[0] - Low[0])*Volume[0];
            }
            else
            {
                adAd[0] = 0;
            }

            for (int iBar = 1; iBar < Bars; iBar++)
            {
                double dDelta = 0;
                double dRange = High[iBar] - Low[iBar];

                if (dRange > 0)
                {
                    dDelta = Volume[iBar]*(2*Close[iBar] - High[iBar] - Low[iBar])/dRange;
                }

                adAd[iBar] = adAd[iBar - 1] + dDelta;
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp
                {
                    CompName = "Accumulation Distribution",
                    DataType = IndComponentType.IndicatorValue,
                    ChartType = IndChartType.Line,
                    ChartColor = Color.Blue,
                    FirstBar = iFirstBar,
                    Value = adAd
                };

            Component[1] = new IndicatorComp
                {
                    ChartType = IndChartType.NoChart,
                    FirstBar = iFirstBar,
                    Value = new double[Bars]
                };

            Component[2] = new IndicatorComp
                {
                    ChartType = IndChartType.NoChart,
                    FirstBar = iFirstBar,
                    Value = new double[Bars]
                };

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[1].DataType = IndComponentType.AllowOpenLong;
                Component[1].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenShort;
                Component[2].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[1].DataType = IndComponentType.ForceCloseLong;
                Component[1].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseShort;
                Component[2].CompName = "Close out short position";
            }

            // Calculation of the logic
            var indLogic = IndicatorLogic.It_does_not_act_as_a_filter;

            switch (IndParam.ListParam[0].Text)
            {
                case "The AD rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The AD falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The AD changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The AD changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adAd, 0, 0, ref Component[1], ref Component[2], indLogic);
        }

        /// <summary>
        ///     Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription = "the " + ToString() + " ";
            ExitFilterShortDescription = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The AD rises":
                    EntryFilterLongDescription += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription += "rises";
                    ExitFilterShortDescription += "falls";
                    break;

                case "The AD falls":
                    EntryFilterLongDescription += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription += "falls";
                    ExitFilterShortDescription += "rises";
                    break;

                case "The AD changes its direction upward":
                    EntryFilterLongDescription += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription += "changes its direction upward";
                    ExitFilterShortDescription += "changes its direction downward";
                    break;

                case "The AD changes its direction downward":
                    EntryFilterLongDescription += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription += "changes its direction downward";
                    ExitFilterShortDescription += "changes its direction upward";
                    break;
            }
        }

        /// <summary>
        ///     Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + (IndParam.CheckParam[0].Checked ? "*" : "");

            return sString;
        }
    }
}