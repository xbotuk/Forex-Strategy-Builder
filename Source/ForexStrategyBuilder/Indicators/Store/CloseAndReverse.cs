//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    /// <summary>
    /// Close and Reverse
    /// </summary>
    public class CloseAndReverse : Indicator
    {
        public CloseAndReverse()
        {
            // General properties
            IndicatorName = "Close and Reverse";
            PossibleSlots = SlotTypes.Close;
        }

        /// <summary>
        /// Sets the default indicator parameters for the designated slot type.
        /// </summary>
        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            IndParam.IndicatorType = TypeOfIndicator.Additional;
            IndParam.ExecutionTime = ExecutionTime.CloseAndReverse;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[] {"Close all positions and open a new one in the opposite direction"};
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription()
        {
            ExitPointLongDescription = "and open a new short one, at the entry price, when a sell entry signal arises";
            ExitPointShortDescription = "and open a new long one, at the entry price, when a buy entry signal arises";
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            return IndicatorName;
        }
    }
}