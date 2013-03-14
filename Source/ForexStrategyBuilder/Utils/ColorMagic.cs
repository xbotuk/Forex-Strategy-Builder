// ColorMagic class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder.Utils
{
    public static class ColorMagic
    {
        public static Color GetIntermediateColor(Color start, Color end, float factor)
        {
            var r = (int)Math.Round((end.R - start.R) * factor + start.R);
            var g = (int)Math.Round((end.G - start.G) * factor + start.G);
            var b = (int)Math.Round((end.B - start.B) * factor + start.B);
            return Color.FromArgb(r, g, b);
        }
    }
}
