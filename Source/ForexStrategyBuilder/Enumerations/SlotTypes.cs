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

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Describes the type of the strategy slots.
    /// </summary>
    [Flags]
    public enum SlotTypes : short
    {
        NotDefined  = 0,
        Open        = 1, // Opening Point of the Position
        OpenFilter  = 2, // Opening Logic Condition
        Close       = 4, // Closing Point of the Position
        CloseFilter = 8  // Closing Logic Condition
    }
}
