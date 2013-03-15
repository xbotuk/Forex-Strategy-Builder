//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : MenuAndStatusBar
    {
        /// <summary>
        ///     The default constructor.
        /// </summary>
        protected Controls()
        {
            InitializeMarket();
            InitializeStrategy();
            InitializeAccount();
            InitializeJournal();
        }
    }
}