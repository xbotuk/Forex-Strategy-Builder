// CommandLineParser
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2013 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.
//
// Contributed by Adam Burgess
//

namespace Forex_Strategy_Builder
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