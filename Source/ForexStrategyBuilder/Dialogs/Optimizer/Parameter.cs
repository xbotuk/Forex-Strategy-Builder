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

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     Provide links to the Parameter's fields
    /// </summary>
    public class Parameter
    {
        private double bestValue;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Parameter(OptimizerParameterType type, int slotNumber, int paramNumber)
        {
            Type = type;
            SlotNumber = slotNumber;
            NumParam = paramNumber;
            bestValue = Value;
            OldBestValue = Value;
        }

        /// <summary>
        ///     Type of the parameter
        /// </summary>
        public OptimizerParameterType Type { get; private set; }

        /// <summary>
        ///     The number of the indicator slot
        /// </summary>
        public int SlotNumber { get; private set; }

        /// <summary>
        ///     The number of NumericParam
        /// </summary>
        public int NumParam { get; private set; }

        /// <summary>
        ///     The IndicatorParameters
        /// </summary>
        public IndicatorParam IndParam
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[SlotNumber].IndParam
                           : null;
            }
        }

        /// <summary>
        ///     Parameter group name
        /// </summary>
        public string GroupName
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[SlotNumber].IndicatorName
                           : Language.T("Permanent Protection");
            }
        }

        /// <summary>
        ///     The name of the parameter
        /// </summary>
        public string ParameterName
        {
            get
            {
                string name = string.Empty;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        name = Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Caption;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        name = Language.T("Permanent Stop Loss");
                        break;
                    case OptimizerParameterType.PermanentTP:
                        name = Language.T("Permanent Take Profit");
                        break;
                    case OptimizerParameterType.BreakEven:
                        name = Language.T("Break Even");
                        break;
                }
                return name;
            }
        }

        /// <summary>
        ///     The current value of the parameter
        /// </summary>
        public double Value
        {
            get
            {
                double value = 0.0;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        value = Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Value;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        value = Data.Strategy.PermanentSL;
                        break;
                    case OptimizerParameterType.PermanentTP:
                        value = Data.Strategy.PermanentTP;
                        break;
                    case OptimizerParameterType.BreakEven:
                        value = Data.Strategy.BreakEven;
                        break;
                }
                return value;
            }
            set
            {
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Value = value;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        Data.Strategy.PermanentSL = (int) value;
                        break;
                    case OptimizerParameterType.PermanentTP:
                        Data.Strategy.PermanentTP = (int) value;
                        break;
                    case OptimizerParameterType.BreakEven:
                        Data.Strategy.BreakEven = (int) value;
                        break;
                }
            }
        }

        /// <summary>
        ///     The minimum value of the parameter
        /// </summary>
        public double Minimum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Min
                           : 5;
            }
        }

        /// <summary>
        ///     The maximum value of the parameter
        /// </summary>
        public double Maximum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Max
                           : 5000;
            }
        }

        /// <summary>
        ///     The number of significant digits
        /// </summary>
        public int Point
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Point
                           : 0;
            }
        }

        /// <summary>
        ///     The maximum value of the parameter
        /// </summary>
        public double Step
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Math.Round(Math.Pow(10, -Point), Point)
                           : (Data.InstrProperties.IsFiveDigits ? 10 : 1);
            }
        }

        /// <summary>
        ///     The best value of the parameter
        /// </summary>
        public double BestValue
        {
            get { return bestValue; }
            set
            {
                OldBestValue = bestValue;
                bestValue = value;
            }
        }

        /// <summary>
        ///     The previous best value of the parameter
        /// </summary>
        public double OldBestValue { get; private set; }
    }
}