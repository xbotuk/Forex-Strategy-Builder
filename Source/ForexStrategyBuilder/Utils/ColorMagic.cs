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
using System.Drawing;

namespace ForexStrategyBuilder.Utils
{
    public static class ColorMagic
    {
        public static Color GetIntermediateColor(Color start, Color end, float factor)
        {
            var r = (int) Math.Round((end.R - start.R)*factor + start.R);
            var g = (int) Math.Round((end.G - start.G)*factor + start.G);
            var b = (int) Math.Round((end.B - start.B)*factor + start.B);
            return Color.FromArgb(r, g, b);
        }
    }
}