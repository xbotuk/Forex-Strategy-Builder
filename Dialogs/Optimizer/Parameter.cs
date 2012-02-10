// Parameter class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// Provide links to the Parameter's fields
    /// </summary>
    public class Parameter
    {
        private readonly int _paramNumber;
        private readonly int _slotNumber;
        private readonly OptimizerParameterType _type;
        private double _bestValue;
        private double _oldBestValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public Parameter(OptimizerParameterType type, int slotNumber, int paramNumber)
        {
            _type = type;
            _slotNumber = slotNumber;
            _paramNumber = paramNumber;
            _bestValue = Value;
            _oldBestValue = Value;
        }

        /// <summary>
        /// Type of the parameter
        /// </summary>
        public OptimizerParameterType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The number of the indicator slot
        /// </summary>
        public int SlotNumber
        {
            get { return _slotNumber; }
        }

        /// <summary>
        /// The number of NumericParam
        /// </summary>
        public int NumParam
        {
            get { return _paramNumber; }
        }

        /// <summary>
        /// The IndicatorParameters
        /// </summary>
        public IndicatorParam IndParam
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[_slotNumber].IndParam
                           : null;
            }
        }


        /// <summary>
        /// Parameter group name
        /// </summary>
        public string GroupName
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[_slotNumber].IndicatorName
                           : Language.T("Permanent Protection");
            }
        }

        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName
        {
            get
            {
                string name = string.Empty;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        name = Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Caption;
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
                    default:
                        break;
                }
                return name;
            }
        }

        /// <summary>
        /// The current value of the parameter
        /// </summary>
        public double Value
        {
            get
            {
                double value = 0.0;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        value = Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Value;
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
                    default:
                        break;
                }
                return value;
            }
            set
            {
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Value = value;
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
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// The minimum value of the parameter
        /// </summary>
        public double Minimum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Min
                           : 5;
            }
        }

        /// <summary>
        /// The maximum value of the parameter
        /// </summary>
        public double Maximum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Max
                           : 5000;
            }
        }

        /// <summary>
        /// The number of significant digits
        /// </summary>
        public int Point
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Point
                           : 0;
            }
        }

        /// <summary>
        /// The maximum value of the parameter
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
        /// The best value of the parameter
        /// </summary>
        public double BestValue
        {
            get { return _bestValue; }
            set
            {
                _oldBestValue = _bestValue;
                _bestValue = value;
            }
        }

        /// <summary>
        /// The previous best value of the parameter
        /// </summary>
        public double OldBestValue
        {
            get { return _oldBestValue; }
        }
    }
}