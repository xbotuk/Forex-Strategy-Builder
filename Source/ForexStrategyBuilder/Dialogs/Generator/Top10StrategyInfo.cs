//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ForexStrategyBuilder.Dialogs.Generator
{
    public class Top10StrategyInfo
    {
        public float Value { get; set; }
        public int Balance { get; set; }
        public Top10Slot Top10Slot { get; set; }
        public Strategy TheStrategy { get; set; }
    }
}