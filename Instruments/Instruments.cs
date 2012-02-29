// Instruments class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    public static class Instruments
    {
        private static readonly string PathToInstrumentsFile;
        private static readonly XmlDocument XMLInstruments;
        private static Dictionary<String, InstrumentProperties> _dictInstrument;
        private static bool _isReset;

        /// <summary>
        /// Public constructor.
        /// </summary>
        static Instruments()
        {
            XMLInstruments = new XmlDocument();
            PathToInstrumentsFile = Data.ProgramDir + Path.DirectorySeparatorChar + "System" +
                                    Path.DirectorySeparatorChar + "instruments.xml";
        }

        /// <summary>
        /// Gets the symbols list.
        /// </summary>
        public static string[] SymbolList
        {
            get
            {
                var asSymbols = new string[_dictInstrument.Count];
                _dictInstrument.Keys.CopyTo(asSymbols, 0);
                return asSymbols;
            }
        }

        /// <summary>
        /// Gets or sets the instruments list.
        /// </summary>
        public static Dictionary<String, InstrumentProperties> InstrumentList
        {
            get { return _dictInstrument; }
        }

        /// <summary>
        /// Loads the instruments file.
        /// </summary>
        public static void LoadInstruments()
        {
            try
            {
                XMLInstruments.Load(PathToInstrumentsFile);
            }
            catch (FileNotFoundException)
            {
                XMLInstruments.LoadXml(Resources.instruments);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Load Instruments");
                XMLInstruments.LoadXml(Resources.instruments);
            }

            try
            {
                ParseInstruments();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parse Instruments");
            }
        }

        /// <summary>
        /// Resets the instruments file.
        /// </summary>
        public static void ResetInstruments()
        {
            XMLInstruments.LoadXml(Resources.instruments);
            ParseInstruments();
            SaveInstruments();
            _isReset = true;
        }

        /// <summary>
        /// Saves the config file.
        /// </summary>
        public static void SaveInstruments()
        {
            if (_isReset) return;

            try
            {
                GenerateXMLFile().Save(PathToInstrumentsFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Save Instruments");
            }
        }

        /// <summary>
        /// Parses the instruments file.
        /// </summary>
        private static void ParseInstruments()
        {
            int instrumentsCount = XMLInstruments.GetElementsByTagName("instrument").Count;
            _dictInstrument = new Dictionary<string, InstrumentProperties>(instrumentsCount);

            try
            {
                foreach (XmlNode nodeInstr in XMLInstruments.GetElementsByTagName("instrument"))
                {
                    string symbol = nodeInstr.SelectSingleNode("symbol").InnerText;
                    var instrType = (InstrumetType) Enum.Parse(typeof (InstrumetType), nodeInstr.SelectSingleNode("instrumentType").InnerText);
                    var instrProp = new InstrumentProperties(symbol, instrType)
                    {
                        Comment = nodeInstr.SelectSingleNode("comment").InnerText,
                        Digits = int.Parse(nodeInstr.SelectSingleNode("digits").InnerText),
                        LotSize = int.Parse(nodeInstr.SelectSingleNode("contractSize").InnerText),
                        Spread = StringToFloat(nodeInstr.SelectSingleNode("spread").InnerText),
                        SwapType = (CommissionType) Enum.Parse(typeof (CommissionType), nodeInstr.SelectSingleNode("swapType").InnerText),
                        SwapLong = StringToFloat(nodeInstr.SelectSingleNode("swapLong").InnerText),
                        SwapShort = StringToFloat(nodeInstr.SelectSingleNode("swapShort").InnerText),
                        CommissionType = (CommissionType) Enum.Parse(typeof (CommissionType), nodeInstr.SelectSingleNode("commissionType").InnerText),
                        CommissionScope = (CommissionScope) Enum.Parse(typeof (CommissionScope), nodeInstr.SelectSingleNode("commissionScope").InnerText),
                        CommissionTime = (CommissionTime) Enum.Parse(typeof (CommissionTime), nodeInstr.SelectSingleNode("commissionTime").InnerText),
                        Commission = StringToFloat(nodeInstr.SelectSingleNode("commission").InnerText),
                        Slippage = int.Parse(nodeInstr.SelectSingleNode("slippage").InnerText),
                        PriceIn = nodeInstr.SelectSingleNode("priceIn").InnerText,
                        RateToUSD = StringToFloat(nodeInstr.SelectSingleNode("rateToUSD").InnerText),
                        RateToEUR = StringToFloat(nodeInstr.SelectSingleNode("rateToEUR").InnerText),
                        BaseFileName = nodeInstr.SelectSingleNode("baseFileName").InnerText
                    };
                    _dictInstrument.Add(symbol, instrProp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parsing Instruments");
            }
        }

        /// <summary>
        /// Generates instrument.xml file.
        /// </summary>
        private static XmlDocument GenerateXMLFile()
        {
            // Create the XmlDocument.
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<fsb></fsb>");

            // Create the XML declaration.
            XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);

            // Add new node to the document.
            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(xmldecl, root);

            foreach (var kvp in _dictInstrument)
            {
                InstrumentProperties instrProp = kvp.Value;

                // Creates an instrument element.
                XmlElement instrument = xmlDoc.CreateElement("instrument");

                XmlElement element = xmlDoc.CreateElement("symbol");
                element.InnerText = instrProp.Symbol;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("instrumentType");
                element.InnerText = instrProp.InstrType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("comment");
                element.InnerText = instrProp.Comment;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("digits");
                element.InnerText = instrProp.Digits.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("contractSize");
                element.InnerText = instrProp.LotSize.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("spread");
                element.InnerText = instrProp.Spread.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapType");
                element.InnerText = instrProp.SwapType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapLong");
                element.InnerText = instrProp.SwapLong.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapShort");
                element.InnerText = instrProp.SwapShort.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionType");
                element.InnerText = instrProp.CommissionType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionScope");
                element.InnerText = instrProp.CommissionScope.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionTime");
                element.InnerText = instrProp.CommissionTime.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commission");
                element.InnerText = instrProp.Commission.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("slippage");
                element.InnerText = instrProp.Slippage.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("priceIn");
                element.InnerText = instrProp.PriceIn;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToUSD");
                element.InnerText = instrProp.RateToUSD.ToString("F4");
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToEUR");
                element.InnerText = instrProp.RateToEUR.ToString("F4");
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("baseFileName");
                element.InnerText = instrProp.BaseFileName;
                instrument.AppendChild(element);

                if (xmlDoc.DocumentElement != null) xmlDoc.DocumentElement.AppendChild(instrument);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Parses string values to float.
        /// </summary>
        private static float StringToFloat(string input)
        {
            float output = 0;
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            input = input.Replace(",", decimalSeparator);
            input = input.Replace(".", decimalSeparator);

            try
            {
                output = float.Parse(input);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parsing Instruments");
            }

            return output;
        }
    }
}