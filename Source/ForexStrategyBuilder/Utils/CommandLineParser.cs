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
    public class CommandLineParser
    {
        /// <summary>
        ///     Extract Value from a Command Line Parameter
        /// </summary>
        /// <param name="command">Command Line Parameter</param>
        /// <returns>Parameter Value</returns>
        public static string GetValue(string command)
        {
            try
            {
                return
                    command.Substring(command.IndexOf('=') + 1, command.Length - command.IndexOf('=') - 1)
                           .Replace("\"", @"\")
                           .Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}