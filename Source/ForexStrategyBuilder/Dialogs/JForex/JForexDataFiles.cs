//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.IO;

namespace ForexStrategyBuilder.Dialogs.JForex
{
    public class JForexDataFiles
    {
        private readonly string fileName;

        public JForexDataFiles(string filePath, string targetPath)
        {
            IsCorrect = false;
            FilePath = filePath;
            fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName != null)
            {
                string[] fields = fileName.Split(new[] {'_'});

                if (fields.Length < 4)
                    return;

                switch (fields[1])
                {
                    case "Ticks":
                        Period = 0;
                        break;
                    case "1 Min":
                        Period = 1;
                        break;
                    case "5 Mins":
                        Period = 5;
                        break;
                    case "15 Mins":
                        Period = 15;
                        break;
                    case "30 Mins":
                        Period = 30;
                        break;
                    case "Hourly":
                        Period = 60;
                        break;
                    case "4 Hours":
                        Period = 240;
                        break;
                    case "Daily":
                        Period = 1440;
                        break;
                    case "Weekly":
                        Period = 10080;
                        break;
                    default:
                        return;
                }

                Symbol = fields[0];
            }
            FileTargetPath = Path.Combine(targetPath, Symbol + Period + (Period == 0 ? ".bin" : ".csv"));
            IsCorrect = true;
        }

        public string FilePath { get; private set; }
        public string FileTargetPath { get; private set; }
        public string Symbol { get; private set; }
        public int Period { get; private set; }
        public bool IsCorrect { get; private set; }
    }
}