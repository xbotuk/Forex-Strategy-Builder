namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// Provide links to the Parameter's fields
    /// </summary>
    public class Parameter
    {
        int    _slotNumber;
        int    _paramNumber;
        double _bestValue;

        /// <summary>
        /// The number of the indicator slot
        /// </summary>
        public int SlotNumber
        {
            get { return _slotNumber; }
            set { _slotNumber = value; }
        }

        /// <summary>
        /// The number of NumericParam
        /// </summary>
        public int NumParam
        {
            get { return _paramNumber; }
            set { _paramNumber = value; }
        }

        /// <summary>
        /// The IndicatorParameters
        /// </summary>
        public IndicatorParam IP
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam; }
        }

        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Caption; }
        }

        /// <summary>
        /// The current value of the parameter
        /// </summary>
        public double Value
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Value; }
            set { Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Value = value; }
        }

        /// <summary>
        /// The minimum value of the parameter
        /// </summary>
        public double Minimum
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Min; }
        }

        /// <summary>
        /// The maximum value of the parameter
        /// </summary>
        public double Maximum
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Max; }
        }

        /// <summary>
        /// The number of significant digits
        /// </summary>
        public int Point
        {
            get { return Data.Strategy.Slot[_slotNumber].IndParam.NumParam[_paramNumber].Point; }
        }

        /// <summary>
        /// The best value of the parameter
        /// </summary>
        public double BestValue
        {
            get { return _bestValue; }
            set { _bestValue = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Parameter(int slotNumber, int paramNumber)
        {
            _slotNumber  = slotNumber;
            _paramNumber = paramNumber;
            _bestValue = Value;
        }
    }
}