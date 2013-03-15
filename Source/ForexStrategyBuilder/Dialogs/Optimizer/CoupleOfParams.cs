//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     The numbers of the parameters into the couple
    /// </summary>
    public struct CoupleOfParams
    {
        public int Param1 { get; set; }
        public int Param2 { get; set; }
        public bool IsPassed { get; set; }
    }
}