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

namespace ForexStrategyBuilder.Indicators.Store
{
    /// <summary>
    /// Stop Loss Indicator
    /// The implementation of logic is in Market.AnalyseClose(int iBar)
    /// </summary>
    public class StopLoss : Indicator
    {
        public StopLoss()
        {
            // General properties
            IndicatorName = "Stop Loss";
            PossibleSlots = SlotTypes.Close;
        }

        /// <summary>
        ///     Sets the default indicator parameters for the designated slot type.
        /// </summary>
        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            // Setting up the indicator parameters
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit at the Stop Loss level",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Stop Loss";
            IndParam.NumParam[0].Value   = 200;
            IndParam.NumParam[0].Min     = 5;
            IndParam.NumParam[0].Max     = 5000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The Stop value (in pips).";
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
            int iStopLoss = (int)IndParam.NumParam[0].Value;

            ExitPointLongDescription  = "when the market falls " + iStopLoss + " pips from the last entry price";
            ExitPointShortDescription = "when the market rises " + iStopLoss + " pips from the last entry price";
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            return IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")"; // Stop Loss
        }
    }
}
