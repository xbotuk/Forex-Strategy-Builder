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
    ///     Keeps the coordinates of each position
    /// </summary>
    public struct PositionCoordinates
    {
        /// <summary>
        ///     The bar number
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        ///     The position number
        /// </summary>
        public int Pos { get; set; }
    }
}