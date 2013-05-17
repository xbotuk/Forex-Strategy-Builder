//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================using System;

using System;
using System.IO;

namespace ForexStrategyBuilder.Library
{
    public class LibRecord
    {
        public string Name { get; set; }
        public string SorcePath { get; set; }
        public string DllPath { get; set; }
        public DateTime SurceModificationTime { get; set; }
        public DateTime DllModificationTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                                 Name,
                                 Path.GetFileName(SorcePath),
                                 Path.GetFileName(DllPath),
                                 DllModificationTime.ToShortDateString());
        }
    }
}