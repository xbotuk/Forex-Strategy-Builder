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
    ///     Keeps the coordinates of each order
    /// </summary>
    public struct OrderCoordinates
    {
        /// <summary>
        ///     The bar number
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        ///     The order number
        /// </summary>
        public int Ord { get; set; }
    }
}