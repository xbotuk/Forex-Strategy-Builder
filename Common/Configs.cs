// Configs
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Win32;

namespace Forex_Strategy_Builder
{
    public static class Configs
    {
        static XmlDocument xmlConfig;
        static string pathToConfigFile;
        static bool   isConfigLoaded   = false;
        static bool   isResetActivated = false;
        static string dateStringFormat = "yyyy-MM-dd HH:mm:ss";

        static int    MIN_BARSDefault                   = 300;
        static int    MAX_BARSDefault                   = 50000;
        static int    MAX_INTRA_BARSDefault             = 50000;
        static int    MAX_ENTRY_FILTERSDefault          = 4;
        static int    MAX_EXIT_FILTERSDefault           = 2;
        static int    SIGMA_MODE_MAIN_CHARTDefault      = 1;
        static int    SIGMA_MODE_SEPARATED_CHARTDefault = 5;

        static string languageDefault               = "English";
        static bool   showStartingTipDefault        = true;
        static int    currentTipNumberDefault       = -1;
        static bool   isGradientViewDefault         = true;
        static string dataDirectoryDefault          = "";
        static string colorSchemeDefault            = "Light Blue";
        static bool   isRememberLastStrDefault      = true;
        static string lastStrategyDefault           = "";
        static bool   isCheckForUpdatesDefault      = true;
        static bool   isCheckForNewBetaDefault      = false;
        static bool   isCheckDataDefault            = true;
        static bool   isFillDataGapsDefault         = false;
        static bool   isCutBadDataDefault           = false;
        static bool   isLoadCustIndDefault          = true;
        static bool   isShowCustIndDefault          = false;
        static int    maxBarsDefault                = 20000;
        static DateTime dataStartTimeDefault        = new DateTime(1990,1,1,0,0,0);
        static DateTime dataEndTimeDefault          = new DateTime(2020,1,1,0,0,0);
        static bool   isUseEndTimeDefault           = false;
        static bool   isUseStartTimeDefault         = false;
        static bool   isAccountInMoneyDefault       = true;
        static string accountCurrencyDefault        = "USD";
        static int    initialAccountDefault         = 10000;
        static int    leverageDefault               = 100;
        static bool   isShowJournalDefault          = true;
        static bool   isJournalByBarsDefault        = false;
        static bool   isJournalShowTransfersDefault = false;
        static bool   isAutoscanDefault             = false;
        static bool   isTradeUntilMarginCallDefault = true;
        static bool   isAdditionalStatisticsDefault = false;
        static bool   isUseLogicalGroupsDefault     = false;
        static bool   isPlaySoundsDefault           = true;
        static string generatorOptionsDefault       = "";
        static string optimizerOptionsDefault       = "";
        static string columnSeparatorDefault        = ",";
        static string decimalSeparatorDefault       = ".";
        static bool   useTickDataDefault            = false;
        static string jforexDataPathDefault         = "";
        static string metatrader4DataPathDefault    = "";
        static int    marketClosingHourDefault      = 21;
        static int    marketOpeningHourDefault      = 21;
        static string importStartingDateDefault     = "";
        static string importEndingDateDefault       = "";
        static string bannedIndicatorsDefault       = "";
        static bool   showPriceChartOnAccountChartDefault = false;
        static bool   analyzerHideFSBDefault        = true;
        static bool   sendUsageStatsDefault         = true;
        static int    mainScreenWidthDefault        = 790;
        static int    mainScreenHeightDefault       = 590;
        static bool   showStatusBarDefault          = true;

        // Indicator Chart
        static int  indicatorChartZoomDefault                     = 8;
        static bool isIndicatorChartInfoPanelDefault              = true;
        static bool isIndicatorChartDynamicInfoDefault            = true;
        static bool isIndicatorChartGridDefault                   = true;
        static bool isIndicatorChartCrossDefault                  = false;
        static bool isIndicatorChartVolumeDefault                 = false;
        static bool isIndicatorChartLotsDefault                   = true;
        static bool isIndicatorChartEntryExitPointsDefault        = true;
        static bool isIndicatorChartCorrectedPositionPriceDefault = false;
        static bool isIndicatorChartBalanceEquityChartDefault     = false;
        static bool isIndicatorChartFloatingPLChartDefault        = false;
        static bool isIndicatorChartIndicatorsDefault             = true;
        static bool isIndicatorChartAmbiguousMarkDefault          = true;
        static bool isIndicatorChartTrueChartsDefault             = false;
        static bool isIndicatorChartProtectionsDefault            = false;

        // Balance Chart
        static int  balanceChartZoomDefault                     = 8;
        static bool isBalanceChartInfoPanelDefault              = true;
        static bool isBalanceChartDynamicInfoDefault            = true;
        static bool isBalanceChartGridDefault                   = true;
        static bool isBalanceChartCrossDefault                  = false;
        static bool isBalanceChartVolumeDefault                 = false;
        static bool isBalanceChartLotsDefault                   = true;
        static bool isBalanceChartEntryExitPointsDefault        = true;
        static bool isBalanceChartCorrectedPositionPriceDefault = true;
        static bool isBalanceChartBalanceEquityChartDefault     = true;
        static bool isBalanceChartFloatingPLChartDefault        = true;
        static bool isBalanceChartIndicatorsDefault             = false;
        static bool isBalanceChartAmbiguousMarkDefault          = true;
        static bool isBalanceChartTrueChartsDefault             = false;
        static bool isBalanceChartProtectionsDefault            = false;

// ------------------------------------------------------------
        static int iMIN_BARS = MIN_BARSDefault;
        /// <summary>
        /// Minimum data bars
        /// </summary>
        public static int MIN_BARS
        {
            get { return iMIN_BARS; }
            set
            {
                iMIN_BARS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MIN_BARS").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        static int iMAX_BARS = MAX_BARSDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MAX_BARS
        {
            get { return iMAX_BARS; }
            set
            {
                iMAX_BARS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_BARS").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        static int iMAX_INTRA_BARS = MAX_INTRA_BARSDefault;
        /// <summary>
        /// Maximum data intra bars
        /// </summary>
        public static int MAX_INTRA_BARS
        {
            get { return iMAX_INTRA_BARS; }
            set
            {
                iMAX_INTRA_BARS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_INTRA_BARS").Item(0).InnerText = value.ToString();
            }
        }

        static int iMAX_ENTRY_FILTERS = MAX_ENTRY_FILTERSDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int MAX_ENTRY_FILTERS
        {
            get { return iMAX_ENTRY_FILTERS; }
            set
            {
                iMAX_ENTRY_FILTERS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_ENTRY_FILTERS").Item(0).InnerText = value.ToString();
            }
        }

        static int iMAX_EXIT_FILTERS = MAX_EXIT_FILTERSDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int MAX_EXIT_FILTERS
        {
            get { return iMAX_EXIT_FILTERS; }
            set
            {
                iMAX_EXIT_FILTERS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_EXIT_FILTERS").Item(0).InnerText = value.ToString();
            }
        }

        static int iSIGMA_MODE_MAIN_CHART = SIGMA_MODE_MAIN_CHARTDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int SIGMA_MODE_MAIN_CHART
        {
            get { return iSIGMA_MODE_MAIN_CHART; }
            set
            {
                iSIGMA_MODE_MAIN_CHART = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/SIGMA_MODE_MAIN_CHART").Item(0).InnerText = value.ToString();
            }
        }

        static int iSIGMA_MODE_SEPARATED_CHART = SIGMA_MODE_SEPARATED_CHARTDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int SIGMA_MODE_SEPARATED_CHART
        {
            get { return iSIGMA_MODE_SEPARATED_CHART; }
            set
            {
                iSIGMA_MODE_SEPARATED_CHART = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/SIGMA_MODE_SEPARATED_CHART").Item(0).InnerText = value.ToString();
            }
        }

// -------------------------------------------------------------
        static bool isInstalled = true;
        public static bool IsInstalled
        {
            get { return isInstalled; }
            set
            {
                isInstalled = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/installed").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Program's Language
        static string language = languageDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string Language
        {
            get { return language; }
            set
            {
                language = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/language").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
        // Show starting Tips
        static bool isShowStartingTip = showStartingTipDefault;
        /// <summary>
        /// Whether to show starting tips
        /// </summary>
        public static bool ShowStartingTip
        {
            get { return isShowStartingTip; }
            set
            {
                isShowStartingTip = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showStartingTip").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
        // Current tip number
        static int currentTipNumber = currentTipNumberDefault;
        /// <summary>
        /// Gets or sets the current starting tip number
        /// </summary>
        public static int CurrentTipNumber
        {
            get { return currentTipNumber; }
            set
            {
                currentTipNumber = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/currentTipNumber").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
        // Show Gradient View
        static bool isGradientView = isGradientViewDefault;
        /// <summary>
        /// Whether to show Gradient View
        /// </summary>
        public static bool GradientView
        {
            get { return isGradientView; }
            set
            {
                isGradientView = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/gradientView").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Data directory
        static string dataDirectory = dataDirectoryDefault;
        /// <summary>
        /// Data Directory
        /// </summary>
        public static string DataDirectory
        {
            get { return dataDirectory; }
            set
            {
                dataDirectory = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataDirectory").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // ColorScheme
        static string colorScheme = colorSchemeDefault;
        /// <summary>
        /// ColorScheme
        /// </summary>
        public static string ColorScheme
        {
            get { return colorScheme; }
            set
            {
                colorScheme = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/colorScheme").Item(0).InnerText = value.ToString();
            }
        }

// ------------------------------------------------------------
        // Remember the Last Strategy
        static bool isRememberLastStr = isRememberLastStrDefault;
        /// <summary>
        /// Remember the Last Strategy
        /// </summary>
        public static bool RememberLastStr
        {
            get { return isRememberLastStr; }
            set
            {
                isRememberLastStr = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/rememberLastStrategy").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Last Strategy
        static string lastStrategy = lastStrategyDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string LastStrategy
        {
            get { return lastStrategy; }
            set
            {
                lastStrategy = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/lastStrategy").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Check for new versions
        static bool isCheckForUpdates = isCheckForUpdatesDefault;
        /// <summary>
        /// Check for new versions at startup.
        /// </summary>
        public static bool CheckForUpdates
        {
            get { return isCheckForUpdates; }
            set
            {
                isCheckForUpdates = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/checkForUpdates").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Check for new beta
        static bool isCheckForNewBeta = isCheckForNewBetaDefault;
        /// <summary>
        /// Check for new new beta at startup.
        /// </summary>
        public static bool CheckForNewBeta
        {
            get { return isCheckForNewBeta; }
            set
            {
                isCheckForNewBeta = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/checkForNewBeta").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Check the Data
        static bool isCheckData = isCheckDataDefault;
        /// <summary>
        /// Whether to Check the Data
        /// </summary>
        public static bool CheckData
        {
            get { return isCheckData; }
            set
            {
                isCheckData = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/checkData").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Fill In Data Gaps
        static bool isFillDataGaps = isFillDataGapsDefault;
        /// <summary>
        /// Whether to fill the data gaps in
        /// </summary>
        public static bool FillInDataGaps
        {
            get { return isFillDataGaps; }
            set
            {
                isFillDataGaps = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/fillDataGaps").Item(0).InnerText = value.ToString();
            }
        }
// ------------------------------------------------------------
        // Fill In Data Gaps
        static bool isCutBadData = isCutBadDataDefault;
        /// <summary>
        /// Whether to cut off bed data
        /// </summary>
        public static bool CutBadData
        {
            get { return isCutBadData; }
            set
            {
                isCutBadData = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/cutBadData").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
        // Whether to load custom indicators
        static bool isLoadCustomIndicators = isLoadCustIndDefault;
        /// <summary>
        /// Whether to load custom indicators at startup.
        /// </summary>
        public static bool LoadCustomIndicators
        {
            get { return isLoadCustomIndicators; }
            set
            {
                isLoadCustomIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/loadCustomIndicators").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
         // Whether to Show custom indicators
        static bool isShowCustomIndicators = isShowCustIndDefault;
        /// <summary>
        /// Whether to Show custom indicators at startup.
        /// </summary>
        public static bool ShowCustomIndicators
        {
            get { return isShowCustomIndicators; }
            set
            {
                isShowCustomIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showCustomIndicators").Item(0).InnerText = value.ToString();
            }
        }
// -------------------------------------------------------------
       // Maximum data bars
        static int maxBars = maxBarsDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MaxBars
        {
            get { return maxBars; }
            set
            {
                maxBars = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataMaxBars").Item(0).InnerText = value.ToString();
            }
        }

        static DateTime dataStartTime = dataStartTimeDefault;
        /// <summary>
        /// Start time of market data.
        /// </summary>
        public static DateTime DataStartTime
        {
            get { return dataStartTime; }
            set
            {
                dataStartTime = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataStartTime").Item(0).InnerText = dataStartTime.ToString(dateStringFormat);
            }
        }

        static DateTime dataEndTime = dataEndTimeDefault;
        /// <summary>
        /// End time of market data.
        /// </summary>
        public static DateTime DataEndTime
        {
            get { return dataEndTime; }
            set
            {
                dataEndTime = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataEndTime").Item(0).InnerText = dataEndTime.ToString(dateStringFormat);
            }
        }


        static bool isUseStartTime = isUseStartTimeDefault;
        /// <summary>
        /// Use start time
        /// </summary>
        public static bool UseStartTime
        {
            get { return isUseStartTime; }
            set
            {
                isUseStartTime = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataUseStartTime").Item(0).InnerText = value.ToString();
            }
        }

        static bool isUseEndTime = isUseEndTimeDefault;
        /// <summary>
        /// Use ending time
        /// </summary>
        public static bool UseEndTime
        {
            get { return isUseEndTime; }
            set
            {
                isUseEndTime = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/dataUseEndTime").Item(0).InnerText = value.ToString();
            }
        }

        static bool isAccountInMoney = isAccountInMoneyDefault;
        /// <summary>
        /// Whether to express the account in currency or in pips
        /// </summary>
        public static bool AccountInMoney
        {
            get { return isAccountInMoney; }
            set
            {
                isAccountInMoney = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/account/accountInMoney").Item(0).InnerText = value.ToString();
            }
        }

        static string accountCurrency = accountCurrencyDefault;
        /// <summary>
        /// Account Currency
        /// </summary>
        public static string AccountCurrency
        {
            get { return accountCurrency; }
            set
            {
                accountCurrency = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/account/accountCurrency").Item(0).InnerText = value.ToString();
            }
        }

        static int initialAccount = initialAccountDefault;
        /// <summary>
        /// Initial Account
        /// </summary>
        public static int InitialAccount
        {
            get { return initialAccount; }
            set
            {
                initialAccount = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/account/initialAccount").Item(0).InnerText = value.ToString();
            }
        }

        static int leverage = leverageDefault;
        /// <summary>
        /// Leverage
        /// </summary>
        public static int Leverage
        {
            get { return leverage; }
            set
            {
                leverage = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/account/leverage").Item(0).InnerText = value.ToString();
            }
        }

        static bool isShowJournal = isShowJournalDefault;
        /// <summary>
        /// Whether to show the journal
        /// </summary>
        public static bool ShowJournal
        {
            get { return isShowJournal; }
            set
            {
                isShowJournal = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showJournal").Item(0).InnerText = value.ToString();
            }
        }

        static bool isJournalByBars = isJournalByBarsDefault;
        /// <summary>
        /// Arrange the journal by bars
        /// </summary>
        public static bool JournalByBars
        {
            get { return isJournalByBars; }
            set
            {
                isJournalByBars = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/journalByBars").Item(0).InnerText = value.ToString();
            }
        }

        static bool isJournalShowTransfers = isJournalShowTransfersDefault;
        /// <summary>
        /// Sets if journal shows transfers
        /// </summary>
        public static bool JournalShowTransfers
        {
            get { return isJournalShowTransfers; }
            set
            {
                isJournalShowTransfers = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/journalShowTransfers").Item(0).InnerText = value.ToString();
            }
        }

        static bool isAutoscan = isAutoscanDefault;
        /// <summary>
        /// Perform auto scanning
        /// </summary>
        public static bool Autoscan
        {
            get { return isAutoscan; }
            set
            {
                isAutoscan = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/autoscan").Item(0).InnerText = value.ToString();
            }
        }

        static bool isTradeUntilMarginCall = isTradeUntilMarginCallDefault;
        /// <summary>
        /// Close all trades after a Margin Call
        /// </summary>
        public static bool TradeUntilMarginCall
        {
            get { return isTradeUntilMarginCall; }
            set
            {
                isTradeUntilMarginCall = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/tradeUntilMarginCall").Item(0).InnerText = value.ToString();
            }
        }

        static bool isAdditionalStatistics = isAdditionalStatisticsDefault;
        /// <summary>
        /// Calculates additional stats
        /// </summary>
        public static bool AdditionalStatistics
        {
            get { return isAdditionalStatistics; }
            set
            {
                isAdditionalStatistics = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/additionalStatistics").Item(0).InnerText = value.ToString();
            }
        }

        static bool isUseLogicalGroups = isUseLogicalGroupsDefault;
        /// <summary>
        /// Logical groups for the entry / exit filters.
        /// </summary>
        public static bool UseLogicalGroups
        {
            get { return isUseLogicalGroups; }
            set
            {
                isUseLogicalGroups = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/useLogicalGroups").Item(0).InnerText = value.ToString();
            }
        }

        static bool isPlaySounds = isPlaySoundsDefault;
        /// <summary>
        /// Sets if the program plays sounds on events.
        /// </summary>
        public static bool PlaySounds
        {
            get { return isPlaySounds; }
            set
            {
                isPlaySounds = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/playSounds").Item(0).InnerText = value.ToString();
            }
        }

        static string generatorOptions = generatorOptionsDefault;
        /// <summary>
        /// Generator options
        /// </summary>
        public static string GeneratorOptions
        {
            get { return generatorOptions; }
            set
            {
                generatorOptions = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/generatorOptions").Item(0).InnerText = value.ToString();
            }
        }

        static string optimizerOptions = optimizerOptionsDefault;
        /// <summary>
        /// Optimizer options
        /// </summary>
        public static string OptimizerOptions
        {
            get { return optimizerOptions; }
            set
            {
                optimizerOptions = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/optimizerOptions").Item(0).InnerText = value.ToString();
            }
        }

        static string columnSeparator = columnSeparatorDefault;
        public static string ColumnSeparator
        {
            get { return columnSeparator; }
            set
            {
                columnSeparator = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/columnSeparator").Item(0).InnerText = value.ToString();
            }
        }

        static string decimalSeparator = decimalSeparatorDefault;
        public static string DecimalSeparator
        {
            get { return decimalSeparator; }
            set
            {
                decimalSeparator = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/decimalSeparator").Item(0).InnerText = value.ToString();
            }
        }

        static bool useTickData = useTickDataDefault;
        public static bool UseTickData
        {
            get { return useTickData; }
            set
            {
                useTickData = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/useTickData").Item(0).InnerText = value.ToString();
            }
        }


        static string jforexDataPath = jforexDataPathDefault;
        public static string JForexDataPath
        {
            get { return jforexDataPath; }
            set
            {
                jforexDataPath = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/jforexDataPath").Item(0).InnerText = value.ToString();
            }
        }

        static string metatrader4DataPath = metatrader4DataPathDefault;
        public static string MetaTrader4DataPath
        {
            get { return metatrader4DataPath; }
            set
            {
                metatrader4DataPath = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/metatrader4DataPath").Item(0).InnerText = value.ToString();
            }
        }

        static int marketClosingHour = marketClosingHourDefault;
        public static int MarketClosingHour
        {
            get { return marketClosingHour; }
            set
            {
                marketClosingHour = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/marketClosingHour").Item(0).InnerText = value.ToString();
            }
        }

        static int marketOpeningHour = marketOpeningHourDefault;
        public static int MarketOpeningHour
        {
            get { return marketOpeningHour; }
            set
            {
                marketOpeningHour = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/marketOpeningHour").Item(0).InnerText = value.ToString();
            }
        }

        static string importStartingDate = importStartingDateDefault;
        public static string ImportStartingDate
        {
            get { return importStartingDate; }
            set
            {
                importStartingDate = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/importStartingDate").Item(0).InnerText = value.ToString();
            }
        }

        static string importEndingDate = importEndingDateDefault;
        public static string ImportEndingDate
        {
            get { return importEndingDate; }
            set
            {
                importEndingDate = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/importEndingDate").Item(0).InnerText = value.ToString();
            }
        }

        static string bannedIndicators = bannedIndicatorsDefault;
        public static string BannedIndicators
        {
            get { return bannedIndicators; }
            set
            {
                bannedIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/bannedIndicators").Item(0).InnerText = value.ToString();
            }
        }

        static bool showPriceChartOnAccountChart = showPriceChartOnAccountChartDefault;
        public static bool ShowPriceChartOnAccountChart
        {
            get { return showPriceChartOnAccountChart; }
            set
            {
                showPriceChartOnAccountChart = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showPriceChartOnAccountChart").Item(0).InnerText = value.ToString();
            }
        }

        static bool analyzerHideFSB = analyzerHideFSBDefault;
        public static bool AnalyzerHideFSB
        {
            get { return analyzerHideFSB; }
            set
            {
                analyzerHideFSB = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/analyzerHideFSB").Item(0).InnerText = value.ToString();
            }
        }

        static bool sendUsageStats = sendUsageStatsDefault;
        public static bool SendUsageStats
        {
            get { return sendUsageStats; }
            set
            {
                sendUsageStats = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/sendUsageStats").Item(0).InnerText = value.ToString();
            }
        }

        static int mainScreenWidth = mainScreenWidthDefault;
        public static int MainScreenWidth
        {
            get { return mainScreenWidth; }
            set
            {
                mainScreenWidth = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/mainScreenWidth").Item(0).InnerText = value.ToString();
            }
        }

        static int mainScreenHeight = mainScreenHeightDefault;
        public static int MainScreenHeight
        {
            get { return mainScreenHeight; }
            set
            {
                mainScreenHeight = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/mainScreenHeight").Item(0).InnerText = value.ToString();
            }
        }

        static bool showStatusBar = showStatusBarDefault;
        public static bool ShowStatusBar
        {
            get { return showStatusBar; }
            set
            {
                showStatusBar = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showStatusBar").Item(0).InnerText = value.ToString();
            }
        }


// -------------------------------------------------------------


        static int indicatorChartZoom = indicatorChartZoomDefault;
        public static int IndicatorChartZoom
        {
            get { return indicatorChartZoom; }
            set
            {
                indicatorChartZoom = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/zoom").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartInfoPanel = isIndicatorChartInfoPanelDefault;
        public static bool IndicatorChartInfoPanel
        {
            get { return isIndicatorChartInfoPanel; }
            set
            {
                isIndicatorChartInfoPanel = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/infoPanel").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartDynamicInfo = isIndicatorChartDynamicInfoDefault;
        public static bool IndicatorChartDynamicInfo
        {
            get { return isIndicatorChartDynamicInfo; }
            set
            {
                isIndicatorChartDynamicInfo = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/dynamicInfo").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartGrid = isIndicatorChartGridDefault;
        public static bool IndicatorChartGrid
        {
            get { return isIndicatorChartGrid; }
            set
            {
                isIndicatorChartGrid = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/grid").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartCross = isIndicatorChartCrossDefault;
        public static bool IndicatorChartCross
        {
            get { return isIndicatorChartCross; }
            set
            {
                isIndicatorChartCross = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/cross").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartVolume = isIndicatorChartVolumeDefault;
        public static bool IndicatorChartVolume
        {
            get { return isIndicatorChartVolume; }
            set
            {
                isIndicatorChartVolume = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/volume").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartLots = isIndicatorChartLotsDefault;
        public static bool IndicatorChartLots
        {
            get { return isIndicatorChartLots; }
            set
            {
                isIndicatorChartLots = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/lots").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartEntryExitPoints = isIndicatorChartEntryExitPointsDefault;
        public static bool IndicatorChartEntryExitPoints
        {
            get { return isIndicatorChartEntryExitPoints; }
            set
            {
                isIndicatorChartEntryExitPoints = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/entryExitPoints").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartCorrectedPositionPrice = isIndicatorChartCorrectedPositionPriceDefault;
        public static bool IndicatorChartCorrectedPositionPrice
        {
            get { return isIndicatorChartCorrectedPositionPrice; }
            set
            {
                isIndicatorChartCorrectedPositionPrice = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/correctedPositionPrice").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartBalanceEquityChart = isIndicatorChartBalanceEquityChartDefault;
        public static bool IndicatorChartBalanceEquityChart
        {
            get { return isIndicatorChartBalanceEquityChart; }
            set
            {
                isIndicatorChartBalanceEquityChart = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/balanceEquityChart").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartFloatingPLChart = isIndicatorChartFloatingPLChartDefault;
        public static bool IndicatorChartFloatingPLChart
        {
            get { return isIndicatorChartFloatingPLChart; }
            set
            {
                isIndicatorChartFloatingPLChart = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/floatingPLChart").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartIndicators = isIndicatorChartIndicatorsDefault;
        public static bool IndicatorChartIndicators
        {
            get { return isIndicatorChartIndicators; }
            set
            {
                isIndicatorChartIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/indicators").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartAmbiguousMark = isIndicatorChartAmbiguousMarkDefault;
        public static bool IndicatorChartAmbiguousMark
        {
            get { return isIndicatorChartAmbiguousMark; }
            set
            {
                isIndicatorChartAmbiguousMark = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/ambiguousMark").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartTrueCharts = isIndicatorChartTrueChartsDefault;
        public static bool IndicatorChartTrueCharts
        {
            get { return isIndicatorChartTrueCharts; }
            set
            {
                isIndicatorChartTrueCharts = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/trueCharts").Item(0).InnerText = value.ToString();
            }
        }

        static bool isIndicatorChartProtections = isIndicatorChartProtectionsDefault;
        public static bool IndicatorChartProtections
        {
            get { return isIndicatorChartProtections; }
            set
            {
                isIndicatorChartProtections = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/indicatorChart/protections").Item(0).InnerText = value.ToString();
            }
        }

// -------------------------------------------------------------
        static int balanceChartZoom = balanceChartZoomDefault;
        public static int BalanceChartZoom
        {
            get { return balanceChartZoom; }
            set
            {
                balanceChartZoom = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/zoom").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartInfoPanel = isBalanceChartInfoPanelDefault;
        public static bool BalanceChartInfoPanel
        {
            get { return isBalanceChartInfoPanel; }
            set
            {
                isBalanceChartInfoPanel = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/infoPanel").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartDynamicInfo = isBalanceChartDynamicInfoDefault;
        public static bool BalanceChartDynamicInfo
        {
            get { return isBalanceChartDynamicInfo; }
            set
            {
                isBalanceChartDynamicInfo = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/dynamicInfo").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartGrid = isBalanceChartGridDefault;
        public static bool BalanceChartGrid
        {
            get { return isBalanceChartGrid; }
            set
            {
                isBalanceChartGrid = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/grid").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartCross = isBalanceChartCrossDefault;
        public static bool BalanceChartCross
        {
            get { return isBalanceChartCross; }
            set
            {
                isBalanceChartCross = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/cross").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartVolume = isBalanceChartVolumeDefault;
        public static bool BalanceChartVolume
        {
            get { return isBalanceChartVolume; }
            set
            {
                isBalanceChartVolume = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/volume").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartLots = isBalanceChartLotsDefault;
        public static bool BalanceChartLots
        {
            get { return isBalanceChartLots; }
            set
            {
                isBalanceChartLots = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/lots").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartEntryExitPoints = isBalanceChartEntryExitPointsDefault;
        public static bool BalanceChartEntryExitPoints
        {
            get { return isBalanceChartEntryExitPoints; }
            set
            {
                isBalanceChartEntryExitPoints = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/entryExitPoints").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartCorrectedPositionPrice = isBalanceChartCorrectedPositionPriceDefault;
        public static bool BalanceChartCorrectedPositionPrice
        {
            get { return isBalanceChartCorrectedPositionPrice; }
            set
            {
                isBalanceChartCorrectedPositionPrice = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/correctedPositionPrice").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartBalanceEquityChart = isBalanceChartBalanceEquityChartDefault;
        public static bool BalanceChartBalanceEquityChart
        {
            get { return isBalanceChartBalanceEquityChart; }
            set
            {
                isBalanceChartBalanceEquityChart = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/balanceEquityChart").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartFloatingPLChart = isBalanceChartFloatingPLChartDefault;
        public static bool BalanceChartFloatingPLChart
        {
            get { return isBalanceChartFloatingPLChart; }
            set
            {
                isBalanceChartFloatingPLChart = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/floatingPLChart").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartIndicators = isBalanceChartIndicatorsDefault;
        public static bool BalanceChartIndicators
        {
            get { return isBalanceChartIndicators; }
            set
            {
                isBalanceChartIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/indicators").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartAmbiguousMark = isBalanceChartAmbiguousMarkDefault;
        public static bool BalanceChartAmbiguousMark
        {
            get { return isBalanceChartAmbiguousMark; }
            set
            {
                isBalanceChartAmbiguousMark = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/ambiguousMark").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartTrueCharts = isBalanceChartTrueChartsDefault;
        public static bool BalanceChartTrueCharts
        {
            get { return isBalanceChartTrueCharts; }
            set
            {
                isBalanceChartTrueCharts = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/trueCharts").Item(0).InnerText = value.ToString();
            }
        }

        static bool isBalanceChartProtections = isBalanceChartProtectionsDefault;
        public static bool BalanceChartProtections
        {
            get { return isBalanceChartProtections; }
            set
            {
                isBalanceChartProtections = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/balanceChart/protections").Item(0).InnerText = value.ToString();
            }
        }

// ----------------------------------------------------------------------

        /// <summary>
        /// Public constructor
        /// </summary>
        static Configs()
        {
            xmlConfig = new XmlDocument();
            pathToConfigFile = Path.Combine(Data.SystemDir, "config.xml");

            return;
        }

        /// <summary>
        /// Sets the parameters to its default value
        /// </summary>
        public static void ResetParams()
        {
            if (!isConfigLoaded)
                return;

            MIN_BARS                   = MIN_BARSDefault;
            MAX_BARS                   = MAX_BARSDefault;
            MAX_INTRA_BARS             = MAX_INTRA_BARSDefault;
            MAX_ENTRY_FILTERS          = MAX_ENTRY_FILTERSDefault;
            MAX_EXIT_FILTERS           = MAX_EXIT_FILTERSDefault;
            SIGMA_MODE_MAIN_CHART      = SIGMA_MODE_MAIN_CHARTDefault;
            SIGMA_MODE_SEPARATED_CHART = SIGMA_MODE_SEPARATED_CHARTDefault;

            Language                   = languageDefault;
            ShowStartingTip            = showStartingTipDefault;
            CurrentTipNumber           = currentTipNumberDefault;
            DataDirectory              = dataDirectoryDefault;
            ColorScheme                = colorSchemeDefault;
            RememberLastStr            = isRememberLastStrDefault;
            LastStrategy               = lastStrategyDefault;
            CheckForUpdates            = isCheckForUpdatesDefault;
            CheckForNewBeta            = isCheckForNewBetaDefault;
            CheckData                  = isCheckDataDefault;
            FillInDataGaps             = isFillDataGapsDefault;
            CutBadData                 = isCutBadDataDefault;
            LoadCustomIndicators       = isLoadCustIndDefault;
            ShowCustomIndicators       = isShowCustIndDefault;
            MaxBars                    = maxBarsDefault;
            DataStartTime              = dataStartTimeDefault;
            DataEndTime                = dataEndTimeDefault;
            UseEndTime                 = isUseEndTimeDefault;
            UseStartTime               = isUseStartTimeDefault;
            AccountInMoney             = isAccountInMoneyDefault;
            AccountCurrency            = accountCurrencyDefault;
            InitialAccount             = initialAccountDefault;
            Leverage                   = leverageDefault;
            ShowJournal                = isShowJournalDefault;
            JournalByBars              = isJournalByBarsDefault;
            Autoscan                   = isAutoscanDefault;
            TradeUntilMarginCall       = isTradeUntilMarginCallDefault;
            AdditionalStatistics       = isAdditionalStatisticsDefault;
            UseLogicalGroups           = isUseLogicalGroupsDefault;
            PlaySounds                 = isPlaySoundsDefault;
            GeneratorOptions           = generatorOptionsDefault;
            OptimizerOptions           = optimizerOptionsDefault;
            ColumnSeparator            = columnSeparatorDefault;
            DecimalSeparator           = decimalSeparatorDefault;
            UseTickData                = useTickDataDefault;
            JForexDataPath             = jforexDataPathDefault;
            MetaTrader4DataPath        = metatrader4DataPathDefault;
            MarketClosingHour          = marketClosingHourDefault;
            MarketOpeningHour          = marketOpeningHourDefault;
            ImportStartingDate         = importStartingDateDefault;
            ImportEndingDate           = importEndingDateDefault;
            BannedIndicators           = bannedIndicatorsDefault;
            ShowPriceChartOnAccountChart = showPriceChartOnAccountChartDefault;
            AnalyzerHideFSB            = analyzerHideFSBDefault;
            MainScreenWidth            = mainScreenWidthDefault;
            MainScreenHeight           = mainScreenHeightDefault;
            ShowStatusBar              = showStatusBarDefault;

            // Indicator Chart
            IndicatorChartZoom                   = indicatorChartZoomDefault;
            IndicatorChartInfoPanel              = isIndicatorChartInfoPanelDefault;
            IndicatorChartDynamicInfo            = isIndicatorChartDynamicInfoDefault;
            IndicatorChartGrid                   = isIndicatorChartGridDefault;
            IndicatorChartCross                  = isIndicatorChartCrossDefault;
            IndicatorChartVolume                 = isIndicatorChartVolumeDefault;
            IndicatorChartLots                   = isIndicatorChartLotsDefault;
            IndicatorChartEntryExitPoints        = isIndicatorChartEntryExitPointsDefault;
            IndicatorChartCorrectedPositionPrice = isIndicatorChartCorrectedPositionPriceDefault;
            IndicatorChartBalanceEquityChart     = isIndicatorChartBalanceEquityChartDefault;
            IndicatorChartFloatingPLChart        = isIndicatorChartFloatingPLChartDefault;
            IndicatorChartIndicators             = isIndicatorChartIndicatorsDefault;
            IndicatorChartAmbiguousMark          = isIndicatorChartAmbiguousMarkDefault;
            IndicatorChartTrueCharts             = isIndicatorChartTrueChartsDefault;
            IndicatorChartProtections            = isIndicatorChartProtectionsDefault;

            // Balance Chart
            BalanceChartZoom                   = balanceChartZoomDefault;
            BalanceChartInfoPanel              = isBalanceChartInfoPanelDefault;
            BalanceChartDynamicInfo            = isBalanceChartDynamicInfoDefault;
            BalanceChartGrid                   = isBalanceChartGridDefault;
            BalanceChartCross                  = isBalanceChartCrossDefault;
            BalanceChartVolume                 = isBalanceChartVolumeDefault;
            BalanceChartLots                   = isBalanceChartLotsDefault;
            BalanceChartEntryExitPoints        = isBalanceChartEntryExitPointsDefault;
            BalanceChartCorrectedPositionPrice = isBalanceChartCorrectedPositionPriceDefault;
            BalanceChartBalanceEquityChart     = isBalanceChartBalanceEquityChartDefault;
            BalanceChartFloatingPLChart        = isBalanceChartFloatingPLChartDefault;
            BalanceChartIndicators             = isBalanceChartIndicatorsDefault;
            BalanceChartAmbiguousMark          = isBalanceChartAmbiguousMarkDefault;
            BalanceChartTrueCharts             = isBalanceChartTrueChartsDefault;
            BalanceChartProtections            = isBalanceChartProtectionsDefault;

            SaveConfigs();
            isResetActivated = true;

            return;
        }

        /// <summary>
        /// Parses the config file
        /// </summary>
        static void ParseConfigs()
        {
            // Constants
            iMIN_BARS                    = ParseNode("config/MIN_BARS", MIN_BARSDefault);
            iMAX_BARS                    = ParseNode("config/MAX_BARS", MAX_BARSDefault);
            iMAX_INTRA_BARS              = ParseNode("config/MAX_INTRA_BARS", MAX_INTRA_BARSDefault);
            iMAX_ENTRY_FILTERS           = ParseNode("config/MAX_ENTRY_FILTERS", MAX_ENTRY_FILTERSDefault);
            iMAX_EXIT_FILTERS            = ParseNode("config/MAX_EXIT_FILTERS", MAX_EXIT_FILTERSDefault);
            iSIGMA_MODE_MAIN_CHART       = ParseNode("config/SIGMA_MODE_MAIN_CHART", SIGMA_MODE_MAIN_CHARTDefault);
            iSIGMA_MODE_SEPARATED_CHART  = ParseNode("config/SIGMA_MODE_SEPARATED_CHART", SIGMA_MODE_SEPARATED_CHARTDefault);

            isInstalled                  = ParseNode("config/installed", false);
            language                     = ParseNode("config/language", languageDefault);
            isShowStartingTip            = ParseNode("config/showStartingTip", showStartingTipDefault);
            currentTipNumber             = ParseNode("config/currentTipNumber", currentTipNumberDefault);
            isGradientView               = ParseNode("config/gradientView", isGradientViewDefault);
            isShowJournal                = ParseNode("config/showJournal", isShowJournalDefault);
            isJournalByBars              = ParseNode("config/journalByBars", isJournalByBarsDefault);
            isJournalShowTransfers       = ParseNode("config/journalShowTransfers", isJournalShowTransfersDefault);
            isAutoscan                   = ParseNode("config/autoscan", isAutoscanDefault);
            isTradeUntilMarginCall       = ParseNode("config/tradeUntilMarginCall", isTradeUntilMarginCallDefault);
            isAdditionalStatistics       = ParseNode("config/additionalStatistics", isAdditionalStatisticsDefault);
            isUseLogicalGroups           = ParseNode("config/useLogicalGroups", isUseLogicalGroupsDefault);
            dataDirectory                = ParseNode("config/dataDirectory", dataDirectoryDefault);
            colorScheme                  = ParseNode("config/colorScheme", colorSchemeDefault);
            isRememberLastStr            = ParseNode("config/rememberLastStrategy", isRememberLastStrDefault);
            lastStrategy                 = ParseNode("config/lastStrategy", lastStrategyDefault);
            isCheckForUpdates            = ParseNode("config/checkForUpdates", isCheckForUpdatesDefault);
            isCheckForNewBeta            = ParseNode("config/checkForNewBeta", isCheckForNewBetaDefault);
            isCheckData                  = ParseNode("config/checkData", isCheckDataDefault);
            isFillDataGaps               = ParseNode("config/fillDataGaps", isFillDataGapsDefault);
            isCutBadData                 = ParseNode("config/cutBadData", isCutBadDataDefault);
            isLoadCustomIndicators       = ParseNode("config/loadCustomIndicators", isLoadCustIndDefault);
            isShowCustomIndicators       = ParseNode("config/showCustomIndicators", isShowCustIndDefault);
            isPlaySounds                 = ParseNode("config/playSounds", isPlaySoundsDefault);
            generatorOptions             = ParseNode("config/generatorOptions", generatorOptionsDefault);
            optimizerOptions             = ParseNode("config/optimizerOptions", optimizerOptionsDefault);
            columnSeparator              = ParseNode("config/columnSeparator", columnSeparatorDefault);
            decimalSeparator             = ParseNode("config/decimalSeparator", decimalSeparatorDefault);
            useTickData                  = ParseNode("config/useTickData",  useTickDataDefault);
            jforexDataPath               = ParseNode("config/jforexDataPath",  jforexDataPathDefault);
            metatrader4DataPath          = ParseNode("config/metatrader4DataPath", metatrader4DataPathDefault);
            marketClosingHour            = ParseNode("config/marketClosingHour", marketClosingHourDefault);
            marketOpeningHour            = ParseNode("config/marketOpeningHour", marketOpeningHourDefault);
            importStartingDate           = ParseNode("config/importStartingDate", importStartingDateDefault);
            importEndingDate             = ParseNode("config/importEndingDate", importEndingDateDefault); 
            bannedIndicators             = ParseNode("config/bannedIndicators", bannedIndicatorsDefault);
            showPriceChartOnAccountChart = ParseNode("config/showPriceChartOnAccountChart", showPriceChartOnAccountChartDefault);
            analyzerHideFSB              = ParseNode("config/analyzerHideFSB", analyzerHideFSBDefault);
            sendUsageStats               = ParseNode("config/sendUsageStats", sendUsageStatsDefault);
            mainScreenWidth              = ParseNode("config/mainScreenWidth", mainScreenWidthDefault);
            mainScreenHeight             = ParseNode("config/mainScreenHeight", mainScreenHeightDefault);
            showStatusBar                = ParseNode("config/showStatusBar", showStatusBarDefault);

            // Data Horizon
            maxBars                      = ParseNode("config/dataMaxBars", maxBarsDefault);
            dataStartTime                = ParseNode("config/dataStartTime", dataStartTimeDefault);
            dataEndTime                  = ParseNode("config/dataEndTime", dataEndTimeDefault);
            isUseStartTime               = ParseNode("config/dataUseStartTime", isUseStartTimeDefault);
            isUseEndTime                 = ParseNode("config/dataUseEndTime", isUseEndTimeDefault);

            // Account
            isAccountInMoney             = ParseNode("config/account/accountInMoney", isAccountInMoneyDefault);
            accountCurrency              = ParseNode("config/account/accountCurrency", accountCurrencyDefault);
            initialAccount               = ParseNode("config/account/initialAccount", initialAccountDefault);
            leverage                     = ParseNode("config/account/leverage", leverageDefault);

            // Indicator Chart
            indicatorChartZoom                     = ParseNode("config/indicatorChart/zoom", indicatorChartZoomDefault);
            isIndicatorChartInfoPanel              = ParseNode("config/indicatorChart/infoPanel", isIndicatorChartInfoPanelDefault);
            isIndicatorChartDynamicInfo            = ParseNode("config/indicatorChart/dynamicInfo", isIndicatorChartDynamicInfoDefault);
            isIndicatorChartGrid                   = ParseNode("config/indicatorChart/grid", isIndicatorChartGridDefault);
            isIndicatorChartCross                  = ParseNode("config/indicatorChart/cross", isIndicatorChartCrossDefault);
            isIndicatorChartVolume                 = ParseNode("config/indicatorChart/volume", isIndicatorChartVolumeDefault);
            isIndicatorChartLots                   = ParseNode("config/indicatorChart/lots", isIndicatorChartLotsDefault);
            isIndicatorChartEntryExitPoints        = ParseNode("config/indicatorChart/entryExitPoints", isIndicatorChartEntryExitPointsDefault);
            isIndicatorChartCorrectedPositionPrice = ParseNode("config/indicatorChart/correctedPositionPrice", isIndicatorChartCorrectedPositionPriceDefault);
            isIndicatorChartBalanceEquityChart     = ParseNode("config/indicatorChart/balanceEquityChart", isIndicatorChartBalanceEquityChartDefault);
            isIndicatorChartFloatingPLChart        = ParseNode("config/indicatorChart/floatingPLChart", isIndicatorChartFloatingPLChartDefault);
            isIndicatorChartIndicators             = ParseNode("config/indicatorChart/indicators", isIndicatorChartIndicatorsDefault);
            isIndicatorChartAmbiguousMark          = ParseNode("config/indicatorChart/ambiguousMark", isIndicatorChartAmbiguousMarkDefault);
            isIndicatorChartTrueCharts             = ParseNode("config/indicatorChart/trueCharts", isIndicatorChartTrueChartsDefault);
            isIndicatorChartProtections            = ParseNode("config/indicatorChart/protections", isIndicatorChartProtectionsDefault);

            // Balance Chart
            balanceChartZoom                     = ParseNode("config/balanceChart/zoom", balanceChartZoomDefault);
            isBalanceChartInfoPanel              = ParseNode("config/balanceChart/infoPanel", isBalanceChartInfoPanelDefault);
            isBalanceChartDynamicInfo            = ParseNode("config/balanceChart/dynamicInfo", isBalanceChartDynamicInfoDefault);
            isBalanceChartGrid                   = ParseNode("config/balanceChart/grid", isBalanceChartGridDefault);
            isBalanceChartCross                  = ParseNode("config/balanceChart/cross", isBalanceChartCrossDefault);
            isBalanceChartVolume                 = ParseNode("config/balanceChart/volume", isBalanceChartVolumeDefault);
            isBalanceChartLots                   = ParseNode("config/balanceChart/lots", isBalanceChartLotsDefault);
            isBalanceChartEntryExitPoints        = ParseNode("config/balanceChart/entryExitPoints", isBalanceChartEntryExitPointsDefault);
            isBalanceChartCorrectedPositionPrice = ParseNode("config/balanceChart/correctedPositionPrice", isBalanceChartCorrectedPositionPriceDefault);
            isBalanceChartBalanceEquityChart     = ParseNode("config/balanceChart/balanceEquityChart", isBalanceChartBalanceEquityChartDefault);
            isBalanceChartFloatingPLChart        = ParseNode("config/balanceChart/floatingPLChart", isBalanceChartFloatingPLChartDefault);
            isBalanceChartIndicators             = ParseNode("config/balanceChart/indicators", isBalanceChartIndicatorsDefault);
            isBalanceChartAmbiguousMark          = ParseNode("config/balanceChart/ambiguousMark", isBalanceChartAmbiguousMarkDefault);
            isBalanceChartTrueCharts             = ParseNode("config/balanceChart/trueCharts", isBalanceChartTrueChartsDefault);
            isBalanceChartProtections            = ParseNode("config/balanceChart/protections", isBalanceChartProtectionsDefault);

            return;
        }

        /// <summary>
        /// Sets parameters after loading config file.
        /// </summary>
        static void ConfigAfterLoading()
        {
            if (maxBars > iMAX_BARS)
                maxBars = iMAX_BARS;

            if (!isInstalled)
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey("Software\\Forex Software\\Forex Strategy Builder");
                SendUsageStats = (regKey.GetValue("UsageStats") == null || regKey.GetValue("UsageStats").ToString() == "0");
                IsInstalled = true;
            }

            if (dataDirectory != "" && Directory.Exists(dataDirectory))
            {
                Data.OfflineDataDir = dataDirectory;
            }
        }

        /// <summary>
        /// Parses an integer parameter.
        /// </summary>
        static int ParseNode(string node, int defaultValue)
        {
            int value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                    value = int.Parse(list.Item(0).InnerText);
                else
                    CreateElement(node, defaultValue.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a string parameter.
        /// </summary>
        static string ParseNode(string node, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                    value = list.Item(0).InnerText;
                else
                    CreateElement(node, defaultValue.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a boolean parameter.
        /// </summary>
        static bool ParseNode(string node, bool defaultValue)
        {
            bool value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                    value = bool.Parse(list.Item(0).InnerText);
                else
                    CreateElement(node, defaultValue.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a DateTime parameter.
        /// </summary>
        static DateTime ParseNode(string node, DateTime defaultValue)
        {
            DateTime value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                    value = DateTime.ParseExact(list.Item(0).InnerText, dateStringFormat, new DateTimeFormatInfo());
                else
                    CreateElement(node, defaultValue.ToString(dateStringFormat));
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Creates a XML element.
        /// </summary>
        static void CreateElement(string node, string value)
        {
            XmlElement newElem = xmlConfig.CreateElement(node.Replace("config/", ""));
            newElem.InnerText = value;
            xmlConfig.DocumentElement.AppendChild(newElem);
        }

        /// <summary>
        /// Loads the config file
        /// </summary>
        public static void LoadConfigs()
        {
            try
            {
                if (!File.Exists(pathToConfigFile))
                {
                    xmlConfig = new XmlDocument();
                    xmlConfig.InnerXml = Properties.Resources.config;
                }
                else
                {
                    xmlConfig.Load(pathToConfigFile);
                }
                ParseConfigs();
                isConfigLoaded = true;
                ConfigAfterLoading();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return;
        }

        /// <summary>
        /// Saves the config file
        /// </summary>
        public static void SaveConfigs()
        {
            if (isResetActivated || !isConfigLoaded) return;

            try
            {
                xmlConfig.Save(pathToConfigFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Configs");
            }

            return;
        }
    }
}
