// PositionCoordinates structure
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Keeps the coordinates of each position
    /// </summary>
    public struct PositionCoordinates
    {
        /// <summary>
        /// The bar number
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        /// The position number
        /// </summary>
        public int Pos { get; set; }
    }
}