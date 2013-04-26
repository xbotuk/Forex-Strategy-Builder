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
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    /// <summary>
    ///     Day Closing Indicator
    /// </summary>
    public class DayClosing : Indicator
    {
        /// <summary>
        ///     Sets the default indicator parameters for the designated slot type
        /// </summary>
        public DayClosing()
        {
            // General properties
            IndicatorName = "Day Closing";
            PossibleSlots = SlotTypes.Close;
            AllowClosingFilters = true;
        }

        /// <summary>
        ///     Sets the default indicator parameters for the designated slot type.
        /// </summary>
        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            IndParam.IndicatorType = TypeOfIndicator.DateTime;
            IndParam.ExecutionTime = ExecutionTime.AtBarClosing;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[] {"Exit the market at the end of the day"};
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[1].ToolTip = "The execution price of all exit orders.";

            IndParam.ListParam[1].Caption = "Base price";
            IndParam.ListParam[1].ItemList = new[] {"Close"};
            IndParam.ListParam[1].Index = 0;
            IndParam.ListParam[1].Text = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Exit price of the position.";
        }

        /// <summary>
        ///     Calculates the indicator's components
        /// </summary>
        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Calculation
            var adClosePrice = new double[Bars];

            for (int iBar = 1; iBar < Bars; iBar++)
                if (Time[iBar - 1].Day != Time[iBar].Day)
                    adClosePrice[iBar - 1] = Close[iBar - 1];

            // Check the last bar
            TimeSpan tsBarClosing = Time[Bars - 1].TimeOfDay.Add(new TimeSpan(0, (int) Period, 0));
            var tsDayClosing = new TimeSpan(24, 0, 0);
            if (tsBarClosing == tsDayClosing)
                adClosePrice[Bars - 1] = Close[Bars - 1];

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp
                {
                    CompName = "Closing price of the day",
                    DataType = IndComponentType.ClosePrice,
                    ChartType = IndChartType.NoChart,
                    FirstBar = 2,
                    Value = adClosePrice
                };
        }

        /// <summary>
        ///     Sets the indicator logic description
        /// </summary>
        public override void SetDescription()
        {
            ExitPointLongDescription = "at the end of the day";
            ExitPointShortDescription = "at the end of the day";
        }

        /// <summary>
        ///     Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName;

            return sString;
        }
    }
}