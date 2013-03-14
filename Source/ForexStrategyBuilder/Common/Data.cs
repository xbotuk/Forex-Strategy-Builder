// Data class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        static Data()
        {
            Icon = Resources.Icon;
            PointChar = '.';
            DFS = "dd.MM";
            DF = "dd.MM.yy";
            AutoUsePrvBarValue = true;
            FirstBar = 40;
            PeriodColor = new Dictionary<DataPeriods, Color>();
            AdditionalFolder = "Additional" + Path.DirectorySeparatorChar;
            SourceFolder = "Custom Indicators" + Path.DirectorySeparatorChar;
            DefaultStrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            ColorDir = "Colors" + Path.DirectorySeparatorChar;
            LanguageDir = "Languages" + Path.DirectorySeparatorChar;
            SystemDir = "System" + Path.DirectorySeparatorChar;
            ProgramDir = "";
            ProgramName = "Forex Strategy Builder";
            IsProgramRC = false;
            IsProgramBeta = false;
            LoadedSavedStrategy = "";
            StrategyName = "New.xml";
            StrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            OfflineDocsDir = "Docs" + Path.DirectorySeparatorChar;
            DefaultOfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            OfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            Debug = false;
            IsData = false;
            IsResult = false;
            IsStrategyChanged = false;
            StackStrategy = new Stack<Strategy>();
            GeneratorHistory = new List<Strategy>();
            IsIntrabarData = false;

            // Program's Major, Minor, Version and Build numbers must be <= 99.
            ProgramVersion = Application.ProductVersion;
            string[] version = ProgramVersion.Split('.');
            ProgramID = 1000000*int.Parse(version[0]) +
                        10000*int.Parse(version[1]) +
                        100*int.Parse(version[2]) +
                        1*int.Parse(version[3]);

            if (int.Parse(version[1])%2 != 0)
                IsProgramBeta = true;

            Strategy.GenerateNew();
        }

        /// <summary>
        /// Gets the program name.
        /// </summary>
        public static string ProgramName { get; private set; }

        /// <summary>
        /// Programs icon.
        /// </summary>
        public static Icon Icon { get; private set; }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        public static string ProgramVersion { get; private set; }

        /// <summary>
        /// Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get; private set; }

        /// <summary>
        /// Gets the program Release Candidate.
        /// </summary>
        public static bool IsProgramRC { get; private set; }

        /// <summary>
        /// Gets the program ID
        /// </summary>
        public static int ProgramID { get; private set; }

        /// <summary>
        /// Gets the program current working directory.
        /// </summary>
        public static string ProgramDir { get; private set; }

        /// <summary>
        /// Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get; private set; }

        /// <summary>
        /// Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get; private set; }

        /// <summary>
        /// Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get; private set; }

        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        public static string OfflineDataDir { get; set; }

        /// <summary>
        /// Gets the default data directory.
        /// </summary>
        public static string DefaultOfflineDataDir { get; private set; }

        /// <summary>
        /// Gets or sets the docs directory.
        /// </summary>
        private static string OfflineDocsDir { get; set; }

        /// <summary>
        /// Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get; private set; }

        /// <summary>
        /// Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get; set; }

        /// <summary>
        /// Gets or sets the strategy name with extension.
        /// </summary>
        public static string StrategyName { get; set; }

        /// <summary>
        /// Gets the current strategy full path.
        /// </summary>
        public static string StrategyPath
        {
            get { return Path.Combine(StrategyDir, StrategyName); }
        }

        /// <summary>
        /// Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get; private set; }

        /// <summary>
        /// Gets or sets the Additional  folder
        /// </summary>
        public static string AdditionalFolder { get; private set; }

        /// <summary>
        /// Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get; set; }

        /// <summary>
        /// The current strategy.
        /// </summary>
        public static Strategy Strategy { get; set; }

        /// <summary>
        /// The current instrument.
        /// </summary>
        public static InstrumentProperties InstrProperties { get; set; }

        /// <summary>
        /// The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get; private set; }

        /// <summary>
        /// The Generator History
        /// </summary>
        public static List<Strategy> GeneratorHistory { get; private set; }

        /// <summary>
        /// The Generator History current strategy
        /// </summary>
        public static int GenHistoryIndex { get; set; }

        /// <summary>
        /// The scanner colors
        /// </summary>
        public static Dictionary<DataPeriods, Color> PeriodColor { get; private set; }

        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool Debug { get; set; }

        public static string Symbol
        {
            get { return InstrProperties.Symbol; }
        }

        public static DataPeriods Period { get; set; }

        public static string PeriodString
        {
            get { return DataPeriodToString(Period); }
        }

        public static DateTime Update { get; set; }
        public static int Bars { get; set; }

        public static bool IsData { get; set; }
        public static bool IsResult { get; set; }
        public static bool IsStrategyChanged { get; set; }
        public static int FirstBar { get; set; }

        /// <summary>
        /// Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get; set; }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public static string FF
        {
            get { return "F" + InstrProperties.Digits; }
        }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        public static string DF { get; private set; }

        /// <summary>
        /// Gets the short date format.
        /// </summary>
        public static string DFS { get; private set; }

        /// <summary>
        /// Gets the point character
        /// </summary>
        public static char PointChar { get; private set; }

        /// <summary>
        /// Relative font height
        /// </summary>
        public static float VerticalDLU { get; set; }

        /// <summary>
        /// Relative font width
        /// </summary>
        public static float HorizontalDLU { get; set; }

        /// <summary>
        /// Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            if (DateTimeFormatInfo.CurrentInfo != null)
                DF = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            if (DF == "dd/MM yyyy") DF = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
            DF = DF.Replace(" ", ""); // Fixes the Slovenian issue
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                char[] acDS = DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
                string[] asSS = DF.Split(acDS, 3);
                asSS[0] = asSS[0].Substring(0, 1) + asSS[0].Substring(0, 1);
                asSS[1] = asSS[1].Substring(0, 1) + asSS[1].Substring(0, 1);
                asSS[2] = asSS[2].Substring(0, 1) + asSS[2].Substring(0, 1);
                DF = asSS[0] + acDS[0] + asSS[1] + acDS[0] + asSS[2];

                if (asSS[0].ToUpper() == "YY")
                    DFS = asSS[1] + acDS[0] + asSS[2];
                else if (asSS[1].ToUpper() == "YY")
                    DFS = asSS[0] + acDS[0] + asSS[2];
                else
                    DFS = asSS[0] + acDS[0] + asSS[1];
            }

            // Point character
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            PointChar = cultureInfo.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            ProgramDir = Directory.GetCurrentDirectory();
            DefaultOfflineDataDir = Path.Combine(ProgramDir, OfflineDataDir);
            OfflineDataDir = DefaultOfflineDataDir;
            OfflineDocsDir = Path.Combine(ProgramDir, OfflineDocsDir);
            StrategyDir = Path.Combine(ProgramDir, DefaultStrategyDir);
            SourceFolder = Path.Combine(ProgramDir, SourceFolder);
            SystemDir = Path.Combine(ProgramDir, SystemDir);
            LanguageDir = Path.Combine(SystemDir, LanguageDir);
            ColorDir = Path.Combine(SystemDir, ColorDir);

            // Scanner colors
            PeriodColor.Add(DataPeriods.min1, Color.Yellow);
            PeriodColor.Add(DataPeriods.min5, Color.Lime);
            PeriodColor.Add(DataPeriods.min15, Color.Green);
            PeriodColor.Add(DataPeriods.min30, Color.Orange);
            PeriodColor.Add(DataPeriods.hour1, Color.DarkSalmon);
            PeriodColor.Add(DataPeriods.hour4, Color.Peru);
            PeriodColor.Add(DataPeriods.day, Color.Red);
            PeriodColor.Add(DataPeriods.week, Color.DarkViolet);
        }

        #region Market data arrays

        public static DateTime[] Time { get; set; }
        public static double[] Open { get; set; }
        public static double[] High { get; set; }
        public static double[] Low { get; set; }
        public static double[] Close { get; set; }
        public static int[] Volume { get; set; }

        #endregion

        #region Intrabar Scanner

        public static Bar[][] IntraBarData { get; set; }
        public static int[] IntraBarBars { get; set; }
        public static bool IsIntrabarData { get; set; }
        public static int[] IntraBars { get; set; }

        /// <summary>
        /// Number of intrabar periods that have been loaded
        /// </summary>
        public static int LoadedIntraBarPeriods { get; set; }

        public static DataPeriods[] IntraBarsPeriods { get; set; }

        // Tick data
        //static Dictionary<DateTime, double[]> tickData;
        //public static Dictionary<DateTime, double[]> TickData { get { return tickData; } set { tickData = value; } }
        public static double[][] TickData { get; set; }
        public static bool IsTickData { get; set; }
        public static long Ticks { get; set; }


        /// <summary>
        /// Calculates the Modelling Quality
        /// </summary>
        public static double ModellingQuality
        {
            get
            {
                if (!Backtester.IsScanPerformed)
                    return 0;

                int startGen = 0;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] < Period)
                    {
                        startGen = i;
                        break;
                    }

                int startGenM1 = Bars - 1;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] == DataPeriods.min1)
                    {
                        startGenM1 = i;
                        break;
                    }

                double modellingQuality = (0.25 * (startGen - FirstBar) + 0.5 * (startGenM1 - startGen) +
                                           0.9 * (Bars - startGenM1)) / (Bars - FirstBar) * 100;

                return modellingQuality;
            }
        }

        #endregion

        #region Market statistics

        // Statistical information for the instrument data
        public static double MinPrice { get; set; }
        public static double MaxPrice { get; set; }
        public static int DaysOff { get; set; }
        public static int AverageGap { private get; set; }
        public static int MaxGap { private get; set; }
        public static int AverageHighLow { private get; set; }
        public static int MaxHighLow { private get; set; }
        public static int AverageCloseOpen { private get; set; }
        public static int MaxCloseOpen { private get; set; }
        public static bool DataCut { get; set; }

        /// <summary>
        /// Gets the market stats parameters
        /// </summary>
        public static string[] MarketStatsParam { get; private set; }

        /// <summary>
        /// Gets the market stats values
        /// </summary>
        public static string[] MarketStatsValue { get; private set; }

        /// <summary>
        /// Gets the market stats flags
        /// </summary>
        public static bool[] MarketStatsFlag { get; private set; }

        /// <summary>
        /// Initializes the stats names
        /// </summary>
        public static void InitMarketStatistic()
        {
            MarketStatsParam = new[]
            {
                Language.T("Symbol"),
                Language.T("Period"),
                Language.T("Number of bars"),
                Language.T("Date of updating"),
                Language.T("Time of updating"),
                Language.T("Date of beginning"),
                Language.T("Time of beginning"),
                Language.T("Minimum price"),
                Language.T("Maximum price"),
                Language.T("Average Gap"),
                Language.T("Maximum Gap"),
                Language.T("Average High-Low"),
                Language.T("Maximum High-Low"),
                Language.T("Average Close-Open"),
                Language.T("Maximum Close-Open"),
                Language.T("Maximum days off"),
                Language.T("Maximum data bars"),
                Language.T("No data older than"),
                Language.T("No data newer than"),
                Language.T("Fill In Data Gaps"),
                Language.T("Cut Off Bad Data")
            };

            MarketStatsValue = new string[21];
            MarketStatsFlag = new bool[21];
        }

        /// <summary>
        /// Generate the Market Statistics
        /// </summary>
        public static void GenerateMarketStats()
        {
            MarketStatsValue[0] = Symbol;
            MarketStatsValue[1] = DataPeriodToString(Period);
            MarketStatsValue[2] = Bars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[3] = Update.ToString(DF);
            MarketStatsValue[4] = Update.ToString("HH:mm");
            MarketStatsValue[5] = Time[0].ToString(DF);
            MarketStatsValue[6] = Time[0].ToString("HH:mm");
            MarketStatsValue[7] = MinPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[8] = MaxPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[9] = AverageGap + " " + Language.T("pips");
            MarketStatsValue[10] = MaxGap + " " + Language.T("pips");
            MarketStatsValue[11] = AverageHighLow + " " + Language.T("pips");
            MarketStatsValue[12] = MaxHighLow + " " + Language.T("pips");
            MarketStatsValue[13] = AverageCloseOpen + " " + Language.T("pips");
            MarketStatsValue[14] = MaxCloseOpen + " " + Language.T("pips");
            MarketStatsValue[15] = DaysOff.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[16] = Configs.MaxBars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[17] = Configs.UseStartTime ? Configs.DataStartTime.ToShortDateString() : Language.T("No limits");
            MarketStatsValue[18] = Configs.UseEndTime ? Configs.DataEndTime.ToShortDateString() : Language.T("No limits");
            MarketStatsValue[19] = Configs.FillInDataGaps ? Language.T("Accomplished") : Language.T("Switched off");
            MarketStatsValue[20] = Configs.CutBadData ? Language.T("Accomplished") : Language.T("Switched off");
        }

        #endregion

        #region Usage stats

        private static readonly DateTime FSBStartTime = DateTime.Now;
        public static int GeneratorStarts { get; set; }
        public static int OptimizerStarts { get; set; }
        public static int SavedStrategies { get; set; }

        #endregion

        private static string[] _asStrategyIndicators;

        /// <summary>
        /// Sets the indicator names
        /// </summary>
        public static void SetStrategyIndicators()
        {
            _asStrategyIndicators = new string[Strategy.Slots];
            for (int i = 0; i < Strategy.Slots; i++)
                _asStrategyIndicators[i] = Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        /// It tells if the strategy description is relevant.
        /// </summary>
        public static bool IsStrDescriptionRelevant()
        {
            bool isStrategyIndicatorsChanged = Strategy.Slots != _asStrategyIndicators.Length;

            if (isStrategyIndicatorsChanged == false)
            {
                for (int i = 0; i < Strategy.Slots; i++)
                    if (_asStrategyIndicators[i] != Strategy.Slot[i].IndicatorName)
                        isStrategyIndicatorsChanged = true;
            }

            return !isStrategyIndicatorsChanged;
        }

        /// <summary>
        /// Collects usage statistics and sends them if it's allowed.
        /// </summary>
        public static void SendStats()
        {
            const string fileURL = "http://forexsb.com/ustats/set-fsb.php";

            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up) continue;
                mac = nic.GetPhysicalAddress().ToString();
                break;
            }

            string parameters = string.Empty;

            if (Configs.SendUsageStats)
            {
                parameters =
                    "?mac=" + mac +
                    "&reg=" + RegionInfo.CurrentRegion.EnglishName +
                    "&time=" + (int) (DateTime.Now - FSBStartTime).TotalSeconds +
                    "&gen=" + GeneratorStarts +
                    "&opt=" + OptimizerStarts +
                    "&str=" + SavedStrategies;
            }

            try
            {
                var webClient = new WebClient();
                Stream data = webClient.OpenRead(fileURL + parameters);
                if (data != null) data.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriods dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriods.min1:
                    return "1 " + Language.T("Minute");
                case DataPeriods.min5:
                    return "5 " + Language.T("Minutes");
                case DataPeriods.min15:
                    return "15 " + Language.T("Minutes");
                case DataPeriods.min30:
                    return "30 " + Language.T("Minutes");
                case DataPeriods.hour1:
                    return "1 " + Language.T("Hour");
                case DataPeriods.hour4:
                    return "4 " + Language.T("Hours");
                case DataPeriods.day:
                    return "1 " + Language.T("Day");
                case DataPeriods.week:
                    return "1 " + Language.T("Week");
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Paints a rectangle with gradient
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = GetGradientColor(color, +depth);
                Color color2 = GetGradientColor(color, -depth);
                var rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                var linearGradientBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(linearGradientBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }
        }

        /// <summary>
        /// Color change
        /// </summary>
        public static Color GetGradientColor(Color baseColor, int depth)
        {
            if (!Configs.GradientView)
                return baseColor;

            int r = Math.Max(Math.Min(baseColor.R + depth, 255), 0);
            int g = Math.Max(Math.Min(baseColor.G + depth, 255), 0);
            int b = Math.Max(Math.Min(baseColor.B + depth, 255), 0);

            return Color.FromArgb(r, g, b);
        }
    }
}