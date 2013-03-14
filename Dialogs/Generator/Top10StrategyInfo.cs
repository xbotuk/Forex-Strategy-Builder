// Top10StrategyInfo Classes
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    public class Top10StrategyInfo
    {
        public Single Balance { get; set; }
        public Top10Slot Top10Slot { get; set; }
        public Strategy TheStrategy { get; set; }
    }
}