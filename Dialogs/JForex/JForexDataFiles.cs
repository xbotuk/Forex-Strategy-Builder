// Forex Strategy Builder - JForexDataFiles
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.IO;

namespace Forex_Strategy_Builder.Dialogs.JForex
{
    public class JForexDataFiles
    {
        private readonly string _fileName;

        public JForexDataFiles(string filePath)
        {
            IsCorrect = false;
            FilePath = filePath;
            _fileName = Path.GetFileNameWithoutExtension(filePath);
            if (_fileName != null)
            {
                string[] fields = _fileName.Split(new[] {'_'});

                if (fields.Length != 5)
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
            FileTargetPath = Data.OfflineDataDir + Symbol + Period + (Period == 0 ? ".bin" : ".csv");
            IsCorrect = true;
        }

        public string FilePath { get; private set; }
        public string FileTargetPath { get; private set; }
        public string Symbol { get; private set; }
        public int Period { get; private set; }
        public bool IsCorrect { get; private set; }
    }
}