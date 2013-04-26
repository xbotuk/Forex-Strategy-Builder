//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ForexStrategyBuilder.Indicators
{
    /// <summary>
    ///     Class IndicatorSlot.
    /// </summary>
    public class IndicatorSlot
    {
        /// <summary>
        ///     The default constructor.
        /// </summary>
        public IndicatorSlot()
        {
            SlotNumber = 0;
            SlotType = SlotTypes.NotDefined;
            LogicalGroup = "";
            IsDefined = false;
            SlotStatus = StrategySlotStatus.Open;
            IndicatorName = "Not defined";
            IndParam = new IndicatorParam();
            SeparatedChart = false;
            Component = new IndicatorComp[] {};
            SpecValue = new double[] {};
            MinValue = double.MaxValue;
            MaxValue = double.MinValue;
        }

        /// <summary>
        ///     Gets or sets the number of the slot.
        /// </summary>
        public int SlotNumber { get; set; }

        /// <summary>
        ///     Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get; set; }

        /// <summary>
        ///     Gets or sets the logical group of the slot.
        /// </summary>
        public string LogicalGroup { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is a locked slot (Generator)
        /// </summary>
        public StrategySlotStatus SlotStatus { get; set; }

        /// <summary>
        ///     Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get; set; }

        /// <summary>
        ///     Gets or sets the indicator parameters.
        /// </summary>
        public IndicatorParam IndParam { get; set; }

        /// <summary>
        ///     If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get; set; }

        /// <summary>
        ///     Gets or sets an indicator component.
        /// </summary>
        public IndicatorComp[] Component { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's special values.
        /// </summary>
        public double[] SpecValue { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's min value.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's max value.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        ///     Returns a copy
        /// </summary>
        public IndicatorSlot Clone()
        {
            var slot = new IndicatorSlot
                {
                    SlotNumber = SlotNumber,
                    SlotType = SlotType,
                    SlotStatus = SlotStatus,
                    LogicalGroup = LogicalGroup,
                    IsDefined = IsDefined,
                    IndicatorName = IndicatorName,
                    SeparatedChart = SeparatedChart,
                    MinValue = MinValue,
                    MaxValue = MaxValue,
                    IndParam = IndParam.Clone(),
                    SpecValue = new double[SpecValue.Length]
                };

            SpecValue.CopyTo(slot.SpecValue, 0);

            slot.Component = new IndicatorComp[Component.Length];
            for (int i = 0; i < Component.Length; i++)
                slot.Component[i] = Component[i].Clone();

            return slot;
        }
    }
}