// Enumerations
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Describes the type of the strategy slots.
    /// </summary>
    [FlagsAttribute]
    public enum SlotTypes : short
    {
        NotDefined  = 0,
        Open        = 1, // Opening Point of the Position
        OpenFilter  = 2, // Opening Logic Condition
        Close       = 4, // Closing Point of the Position
        CloseFilter = 8  // Closing Logic Condition
    }
}
