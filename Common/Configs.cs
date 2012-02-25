// Configs
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
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
        static XmlDocument _xmlConfig;
        static readonly string PathToConfigFile;
        static bool _isConfigLoaded;
        static bool _isResetActivated;
        private const string DateStringFormat = "yyyy-MM-dd HH:mm:ss";

        private const int MinBarsDefault = 300;
        private const int MaxBarsLimitDefault = 50000;
        private const int MaxIntraBarsDefault = 50000;
        private const int MaxEntryFiltersDefault = 4;
        private const int MaxExitFiltersDefault = 2;
        private const int SigmaModeMainChartDefault = 1;
        private const int SigmaModeSeparatedChartDefault = 5;

        private const string LanguageDefault = "English";
        private const bool ShowStartingTipDefault = true;
        private const int CurrentTipNumberDefault = -1;
        private const bool IsGradientViewDefault = true;
        private const string DataDirectoryDefault = "";
        private const string ColorSchemeDefault = "Light Blue";
        private const bool IsRememberLastStrDefault = true;
        private const string LastStrategyDefault = "";
        private const bool IsCheckForUpdatesDefault = true;
        private const bool IsCheckForNewBetaDefault = false;
        private const bool IsCheckDataDefault = true;
        private const bool IsFillDataGapsDefault = false;
        private const bool IsCutBadDataDefault = false;
        private const bool IsLoadCustIndDefault = true;
        private const bool IsShowCustIndDefault = false;
        private const int MaxBarsDefault = 20000;
        private static readonly DateTime DataStartTimeDefault = new DateTime(1990, 1, 1, 0, 0, 0);
        private static readonly DateTime DataEndTimeDefault = new DateTime(2020, 1, 1, 0, 0, 0);
        private const bool IsUseEndTimeDefault = false;
        private const bool IsUseStartTimeDefault = false;
        private const bool IsAccountInMoneyDefault = true;
        private const string AccountCurrencyDefault = "USD";
        private const int InitialAccountDefault = 10000;
        private const int LeverageDefault = 100;
        private const bool IsShowJournalDefault = true;
        private const bool IsJournalByBarsDefault = false;
        private const bool IsJournalShowTransfersDefault = false;
        private const bool IsAutoscanDefault = false;
        private const bool IsTradeUntilMarginCallDefault = true;
        private const bool IsAdditionalStatisticsDefault = false;
        private const bool IsUseLogicalGroupsDefault = false;
        private const bool IsPlaySoundsDefault = true;
        private const string GeneratorOptionsDefault = "";
        private const string OptimizerOptionsDefault = "";
        private const string ColumnSeparatorDefault = ",";
        private const string DecimalSeparatorDefault = ".";
        private const bool UseTickDataDefault = false;
        private const string JforexDataPathDefault = "";
        private const string Metatrader4DataPathDefault = "";
        private const int MarketClosingHourDefault = 21;
        private const int MarketOpeningHourDefault = 21;
        private const string ImportStartingDateDefault = "";
        private const string ImportEndingDateDefault = "";
        private const string BannedIndicatorsDefault = "";
        private const bool ShowPriceChartOnAccountChartDefault = false;
        private const bool AnalyzerHideFSBDefault = true;
        private const bool SendUsageStatsDefault = true;
        private const int MainScreenWidthDefault = 790;
        private const int MainScreenHeightDefault = 590;
        private const bool ShowStatusBarDefault = true;

        // Indicator Chart
        private const int IndicatorChartZoomDefault = 8;
        private const bool IsIndicatorChartInfoPanelDefault = true;
        private const bool IsIndicatorChartDynamicInfoDefault = true;
        private const bool IsIndicatorChartGridDefault = true;
        private const bool IsIndicatorChartCrossDefault = false;
        private const bool IsIndicatorChartVolumeDefault = false;
        private const bool IsIndicatorChartLotsDefault = true;
        private const bool IsIndicatorChartEntryExitPointsDefault = true;
        private const bool IsIndicatorChartCorrectedPositionPriceDefault = false;
        private const bool IsIndicatorChartBalanceEquityChartDefault = false;
        private const bool IsIndicatorChartFloatingPLChartDefault = false;
        private const bool IsIndicatorChartIndicatorsDefault = true;
        private const bool IsIndicatorChartAmbiguousMarkDefault = true;
        private const bool IsIndicatorChartTrueChartsDefault = false;
        private const bool IsIndicatorChartProtectionsDefault = false;

        // Balance Chart
        private const int BalanceChartZoomDefault = 8;
        private const bool IsBalanceChartInfoPanelDefault = true;
        private const bool IsBalanceChartDynamicInfoDefault = true;
        private const bool IsBalanceChartGridDefault = true;
        private const bool IsBalanceChartCrossDefault = false;
        private const bool IsBalanceChartVolumeDefault = false;
        private const bool IsBalanceChartLotsDefault = true;
        private const bool IsBalanceChartEntryExitPointsDefault = true;
        private const bool IsBalanceChartCorrectedPositionPriceDefault = true;
        private const bool IsBalanceChartBalanceEquityChartDefault = true;
        private const bool IsBalanceChartFloatingPLChartDefault = true;
        private const bool IsBalanceChartIndicatorsDefault = false;
        private const bool IsBalanceChartAmbiguousMarkDefault = true;
        private const bool IsBalanceChartTrueChartsDefault = false;
        private const bool IsBalanceChartProtectionsDefault = false;

        // ------------------------------------------------------------
        static int _iMinBars = MinBarsDefault;
        /// <summary>
        /// Minimum data bars
        /// </summary>
        public static int MinBars
        {
            get { return _iMinBars; }
            private set
            {
                _iMinBars = value;
                SetNode("config/MIN_BARS", value);
            }
        }
        // ------------------------------------------------------------
        static int _iMaxBarsLimit = MaxBarsLimitDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MaxBarsLimit
        {
            get { return _iMaxBarsLimit; }
            private set
            {
                _iMaxBarsLimit = value;
                SetNode("config/MAX_BARS", value);
            }
        }
        // ------------------------------------------------------------
        static int _iMaxIntraBars = MaxIntraBarsDefault;
        /// <summary>
        /// Maximum data intra bars
        /// </summary>
        public static int MaxIntraBars
        {
            get { return _iMaxIntraBars; }
            private set
            {
                _iMaxIntraBars = value;
                SetNode("config/MAX_INTRA_BARS", value);
            }
        }

        static int _iMaxEntryFilters = MaxEntryFiltersDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int MaxEntryFilters
        {
            get { return _iMaxEntryFilters; }
            set
            {
                _iMaxEntryFilters = value;
                SetNode("config/MAX_ENTRY_FILTERS", value);
            }
        }

        static int _iMaxExitFilters = MaxExitFiltersDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int MaxExitFilters
        {
            get { return _iMaxExitFilters; }
            set
            {
                _iMaxExitFilters = value;
                SetNode("config/MAX_EXIT_FILTERS", value);
            }
        }

        static int _iSigmaModeMainChart = SigmaModeMainChartDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int SigmaModeMainChart
        {
            get { return _iSigmaModeMainChart; }
            private set
            {
                _iSigmaModeMainChart = value;
                SetNode("config/SIGMA_MODE_MAIN_CHART", value);
            }
        }

        static int _iSigmaModeSeparatedChart = SigmaModeSeparatedChartDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int SigmaModeSeparatedChart
        {
            get { return _iSigmaModeSeparatedChart; }
            private set
            {
                _iSigmaModeSeparatedChart = value;
                SetNode("config/SIGMA_MODE_SEPARATED_CHART", value);
            }
        }

        // -------------------------------------------------------------
        static bool _isInstalled = true;
        private static bool IsInstalled
        {
            set
            {
                _isInstalled = value;
                SetNode("config/installed", value);
            }
        }
        // ------------------------------------------------------------
        // Program's Language
        static string _language = LanguageDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                SetNode("config/language", value);
            }
        }
        // -------------------------------------------------------------
        // Show starting Tips
        static bool _isShowStartingTip = ShowStartingTipDefault;
        /// <summary>
        /// Whether to show starting tips
        /// </summary>
        public static bool ShowStartingTip
        {
            get { return _isShowStartingTip; }
            set
            {
                _isShowStartingTip = value;
                SetNode("config/showStartingTip", value);
            }
        }
        // -------------------------------------------------------------
        // Current tip number
        static int _currentTipNumber = CurrentTipNumberDefault;
        /// <summary>
        /// Gets or sets the current starting tip number
        /// </summary>
        public static int CurrentTipNumber
        {
            get { return _currentTipNumber; }
            set
            {
                _currentTipNumber = value;
                SetNode("config/currentTipNumber", value);
            }
        }
        // -------------------------------------------------------------
        // Show Gradient View
        static bool _isGradientView = IsGradientViewDefault;
        /// <summary>
        /// Whether to show Gradient View
        /// </summary>
        public static bool GradientView
        {
            get { return _isGradientView; }
            set
            {
                _isGradientView = value;
                SetNode("config/gradientView", value);
            }
        }
        // ------------------------------------------------------------
        // Data directory
        static string _dataDirectory = DataDirectoryDefault;
        /// <summary>
        /// Data Directory
        /// </summary>
        public static string DataDirectory
        {
            set
            {
                _dataDirectory = value;
                SetNode("config/dataDirectory", value);
            }
        }
        // ------------------------------------------------------------
        // ColorScheme
        static string _colorScheme = ColorSchemeDefault;
        /// <summary>
        /// ColorScheme
        /// </summary>
        public static string ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                _colorScheme = value;
                SetNode("config/colorScheme", value);
            }
        }

        // ------------------------------------------------------------
        // Remember the Last Strategy
        static bool _isRememberLastStr = IsRememberLastStrDefault;
        /// <summary>
        /// Remember the Last Strategy
        /// </summary>
        public static bool RememberLastStr
        {
            get { return _isRememberLastStr; }
            set
            {
                _isRememberLastStr = value;
                SetNode("config/rememberLastStrategy", value);
            }
        }
        // ------------------------------------------------------------
        // Last Strategy
        static string _lastStrategy = LastStrategyDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string LastStrategy
        {
            get { return _lastStrategy; }
            set
            {
                _lastStrategy = value;
                SetNode("config/lastStrategy", value);
            }
        }
        // ------------------------------------------------------------
        // Check for new versions
        static bool _isCheckForUpdates = IsCheckForUpdatesDefault;
        /// <summary>
        /// Check for new versions at startup.
        /// </summary>
        public static bool CheckForUpdates
        {
            get { return _isCheckForUpdates; }
            set
            {
                _isCheckForUpdates = value;
                SetNode("config/checkForUpdates", value);
            }
        }
        // ------------------------------------------------------------
        // Check for new beta
        static bool _isCheckForNewBeta = IsCheckForNewBetaDefault;
        /// <summary>
        /// Check for new new beta at startup.
        /// </summary>
        public static bool CheckForNewBeta
        {
            get { return _isCheckForNewBeta; }
            set
            {
                _isCheckForNewBeta = value;
                SetNode("config/checkForNewBeta", value);
            }
        }
        // ------------------------------------------------------------
        // Check the Data
        static bool _isCheckData = IsCheckDataDefault;
        /// <summary>
        /// Whether to Check the Data
        /// </summary>
        public static bool CheckData
        {
            get { return _isCheckData; }
            set
            {
                _isCheckData = value;
                SetNode("config/checkData", value);
            }
        }
        // ------------------------------------------------------------
        // Fill In Data Gaps
        static bool _isFillDataGaps = IsFillDataGapsDefault;
        /// <summary>
        /// Whether to fill the data gaps in
        /// </summary>
        public static bool FillInDataGaps
        {
            get { return _isFillDataGaps; }
            set
            {
                _isFillDataGaps = value;
                SetNode("config/fillDataGaps", value);
            }
        }
        // ------------------------------------------------------------
        // Fill In Data Gaps
        static bool _isCutBadData = IsCutBadDataDefault;
        /// <summary>
        /// Whether to cut off bed data
        /// </summary>
        public static bool CutBadData
        {
            get { return _isCutBadData; }
            set
            {
                _isCutBadData = value;
                SetNode("config/cutBadData", value);
            }
        }
        // -------------------------------------------------------------
        // Whether to load custom indicators
        static bool _isLoadCustomIndicators = IsLoadCustIndDefault;
        /// <summary>
        /// Whether to load custom indicators at startup.
        /// </summary>
        public static bool LoadCustomIndicators
        {
            get { return _isLoadCustomIndicators; }
            set
            {
                _isLoadCustomIndicators = value;
                SetNode("config/loadCustomIndicators", value);
            }
        }
        // -------------------------------------------------------------
        // Whether to Show custom indicators
        static bool _isShowCustomIndicators = IsShowCustIndDefault;
        /// <summary>
        /// Whether to Show custom indicators at startup.
        /// </summary>
        public static bool ShowCustomIndicators
        {
            get { return _isShowCustomIndicators; }
            set
            {
                _isShowCustomIndicators = value;
                SetNode("config/showCustomIndicators", value);
            }
        }
        // -------------------------------------------------------------
        // Maximum data bars
        static int _maxBars = MaxBarsDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MaxBars
        {
            get { return _maxBars; }
            set
            {
                _maxBars = value;
                SetNode("config/dataMaxBars", value);
            }
        }

        static DateTime _dataStartTime = DataStartTimeDefault;
        /// <summary>
        /// Start time of market data.
        /// </summary>
        public static DateTime DataStartTime
        {
            get { return _dataStartTime; }
            set
            {
                _dataStartTime = value;
                SetNode("config/dataStartTime", _dataStartTime.ToString(DateStringFormat));
            }
        }

        static DateTime _dataEndTime = DataEndTimeDefault;
        /// <summary>
        /// End time of market data.
        /// </summary>
        public static DateTime DataEndTime
        {
            get { return _dataEndTime; }
            set
            {
                _dataEndTime = value;
                SetNode("config/dataEndTime", _dataEndTime.ToString(DateStringFormat));
            }
        }


        static bool _isUseStartTime = IsUseStartTimeDefault;
        /// <summary>
        /// Use start time
        /// </summary>
        public static bool UseStartTime
        {
            get { return _isUseStartTime; }
            set
            {
                _isUseStartTime = value;
                SetNode("config/dataUseStartTime", value);
            }
        }

        static bool _isUseEndTime = IsUseEndTimeDefault;
        /// <summary>
        /// Use ending time
        /// </summary>
        public static bool UseEndTime
        {
            get { return _isUseEndTime; }
            set
            {
                _isUseEndTime = value;
                SetNode("config/dataUseEndTime", value);
            }
        }

        static bool _isAccountInMoney = IsAccountInMoneyDefault;
        /// <summary>
        /// Whether to express the account in currency or in pips
        /// </summary>
        public static bool AccountInMoney
        {
            get { return _isAccountInMoney; }
            set
            {
                _isAccountInMoney = value;
                SetNode("config/account/accountInMoney", value);
            }
        }

        static string _accountCurrency = AccountCurrencyDefault;
        /// <summary>
        /// Account Currency
        /// </summary>
        public static string AccountCurrency
        {
            get { return _accountCurrency; }
            set
            {
                _accountCurrency = value;
                SetNode("config/account/accountCurrency", value);
            }
        }

        static int _initialAccount = InitialAccountDefault;
        /// <summary>
        /// Initial Account
        /// </summary>
        public static int InitialAccount
        {
            get { return _initialAccount; }
            set
            {
                _initialAccount = value;
                SetNode("config/account/initialAccount", value);
            }
        }

        static int _leverage = LeverageDefault;
        /// <summary>
        /// Leverage
        /// </summary>
        public static int Leverage
        {
            get { return _leverage; }
            set
            {
                _leverage = value;
                SetNode("config/account/leverage", value);
            }
        }

        static bool _isShowJournal = IsShowJournalDefault;
        /// <summary>
        /// Whether to show the journal
        /// </summary>
        public static bool ShowJournal
        {
            get { return _isShowJournal; }
            set
            {
                _isShowJournal = value;
                SetNode("config/showJournal", value);
            }
        }

        static bool _isJournalByBars = IsJournalByBarsDefault;
        /// <summary>
        /// Arrange the journal by bars
        /// </summary>
        public static bool JournalByBars
        {
            get { return _isJournalByBars; }
            set
            {
                _isJournalByBars = value;
                SetNode("config/journalByBars", value);
            }
        }

        static bool _isJournalShowTransfers = IsJournalShowTransfersDefault;
        /// <summary>
        /// Sets if journal shows transfers
        /// </summary>
        public static bool JournalShowTransfers
        {
            get { return _isJournalShowTransfers; }
            set
            {
                _isJournalShowTransfers = value;
                SetNode("config/journalShowTransfers", value);
            }
        }

        static bool _isAutoscan = IsAutoscanDefault;
        /// <summary>
        /// Perform auto scanning
        /// </summary>
        public static bool Autoscan
        {
            get { return _isAutoscan; }
            set
            {
                _isAutoscan = value;
                SetNode("config/autoscan", value);
            }
        }

        static bool _isTradeUntilMarginCall = IsTradeUntilMarginCallDefault;
        /// <summary>
        /// Close all trades after a Margin Call
        /// </summary>
        public static bool TradeUntilMarginCall
        {
            get { return _isTradeUntilMarginCall; }
            set
            {
                _isTradeUntilMarginCall = value;
                SetNode("config/tradeUntilMarginCall", value);
            }
        }

        static bool _isAdditionalStatistics = IsAdditionalStatisticsDefault;
        /// <summary>
        /// Calculates additional stats
        /// </summary>
        public static bool AdditionalStatistics
        {
            get { return _isAdditionalStatistics; }
            set
            {
                _isAdditionalStatistics = value;
                SetNode("config/additionalStatistics", value);
            }
        }

        static bool _isUseLogicalGroups = IsUseLogicalGroupsDefault;
        /// <summary>
        /// Logical groups for the entry / exit filters.
        /// </summary>
        public static bool UseLogicalGroups
        {
            get { return _isUseLogicalGroups; }
            set
            {
                _isUseLogicalGroups = value;
                SetNode("config/useLogicalGroups", value);
            }
        }

        static bool _isPlaySounds = IsPlaySoundsDefault;
        /// <summary>
        /// Sets if the program plays sounds on events.
        /// </summary>
        public static bool PlaySounds
        {
            get { return _isPlaySounds; }
            set
            {
                _isPlaySounds = value;
                SetNode("config/playSounds", value);
            }
        }

        static string _generatorOptions = GeneratorOptionsDefault;
        /// <summary>
        /// Generator options
        /// </summary>
        public static string GeneratorOptions
        {
            get { return _generatorOptions; }
            set
            {
                _generatorOptions = value;
                SetNode("config/generatorOptions", value);
            }
        }

        static string _optimizerOptions = OptimizerOptionsDefault;
        /// <summary>
        /// Optimizer options
        /// </summary>
        public static string OptimizerOptions
        {
            get { return _optimizerOptions; }
            set
            {
                _optimizerOptions = value;
                SetNode("config/optimizerOptions", value);
            }
        }

        static string _columnSeparator = ColumnSeparatorDefault;
        public static string ColumnSeparator
        {
            get { return _columnSeparator; }
            set
            {
                _columnSeparator = value;
                SetNode("config/columnSeparator", value);
            }
        }

        static string _decimalSeparator = DecimalSeparatorDefault;
        public static string DecimalSeparator
        {
            get { return _decimalSeparator; }
            set
            {
                _decimalSeparator = value;
                SetNode("config/decimalSeparator", value);
            }
        }

        static bool _useTickData = UseTickDataDefault;
        public static bool UseTickData
        {
            get { return _useTickData; }
            set
            {
                _useTickData = value;
                SetNode("config/useTickData", value);
            }
        }


        static string _jforexDataPath = JforexDataPathDefault;
        public static string JForexDataPath
        {
            get { return _jforexDataPath; }
            set
            {
                _jforexDataPath = value;
                SetNode("config/jforexDataPath", value);
            }
        }

        static string _metatrader4DataPath = Metatrader4DataPathDefault;
        public static string MetaTrader4DataPath
        {
            get { return _metatrader4DataPath; }
            set
            {
                _metatrader4DataPath = value;
                SetNode("config/metatrader4DataPath", value);
            }
        }

        static int _marketClosingHour = MarketClosingHourDefault;
        public static int MarketClosingHour
        {
            get { return _marketClosingHour; }
            set
            {
                _marketClosingHour = value;
                SetNode("config/marketClosingHour", value);
            }
        }

        static int _marketOpeningHour = MarketOpeningHourDefault;
        public static int MarketOpeningHour
        {
            get { return _marketOpeningHour; }
            set
            {
                _marketOpeningHour = value;
                SetNode("config/marketOpeningHour", value);
            }
        }

        static string _importStartingDate = ImportStartingDateDefault;
        public static string ImportStartingDate
        {
            get { return _importStartingDate; }
            set
            {
                _importStartingDate = value;
                SetNode("config/importStartingDate", value);
            }
        }

        static string _importEndingDate = ImportEndingDateDefault;
        public static string ImportEndingDate
        {
            get { return _importEndingDate; }
            set
            {
                _importEndingDate = value;
                SetNode("config/importEndingDate", value);
            }
        }

        static string _bannedIndicators = BannedIndicatorsDefault;
        public static string BannedIndicators
        {
            get { return _bannedIndicators; }
            set
            {
                _bannedIndicators = value;
                SetNode("config/bannedIndicators", value);
            }
        }

        static bool _showPriceChartOnAccountChart = ShowPriceChartOnAccountChartDefault;
        public static bool ShowPriceChartOnAccountChart
        {
            get { return _showPriceChartOnAccountChart; }
            set
            {
                _showPriceChartOnAccountChart = value;
                SetNode("config/showPriceChartOnAccountChart", value);
            }
        }

        static bool _analyzerHideFSB = AnalyzerHideFSBDefault;
        public static bool AnalyzerHideFSB
        {
            get { return _analyzerHideFSB; }
            set
            {
                _analyzerHideFSB = value;
                SetNode("config/analyzerHideFSB", value);
            }
        }

        static bool _sendUsageStats = SendUsageStatsDefault;
        public static bool SendUsageStats
        {
            get { return _sendUsageStats; }
            set
            {
                _sendUsageStats = value;
                SetNode("config/sendUsageStats", value);
            }
        }

        static int _mainScreenWidth = MainScreenWidthDefault;
        public static int MainScreenWidth
        {
            get { return _mainScreenWidth; }
            set
            {
                _mainScreenWidth = value;
                SetNode("config/mainScreenWidth", value);
            }
        }

        static int _mainScreenHeight = MainScreenHeightDefault;
        public static int MainScreenHeight
        {
            get { return _mainScreenHeight; }
            set
            {
                _mainScreenHeight = value;
                SetNode("config/mainScreenHeight", value);
            }
        }

        static bool _showStatusBar = ShowStatusBarDefault;
        public static bool ShowStatusBar
        {
            get { return _showStatusBar; }
            set
            {
                _showStatusBar = value;
                SetNode("config/showStatusBar", value);
            }
        }


        // -------------------------------------------------------------


        static int _indicatorChartZoom = IndicatorChartZoomDefault;
        public static int IndicatorChartZoom
        {
            get { return _indicatorChartZoom; }
            set
            {
                _indicatorChartZoom = value;
                SetNode("config/indicatorChart/zoom", value);
            }
        }

        static bool _isIndicatorChartInfoPanel = IsIndicatorChartInfoPanelDefault;
        public static bool IndicatorChartInfoPanel
        {
            get { return _isIndicatorChartInfoPanel; }
            set
            {
                _isIndicatorChartInfoPanel = value;
                SetNode("config/indicatorChart/infoPanel", value);
            }
        }

        static bool _isIndicatorChartDynamicInfo = IsIndicatorChartDynamicInfoDefault;
        public static bool IndicatorChartDynamicInfo
        {
            get { return _isIndicatorChartDynamicInfo; }
            set
            {
                _isIndicatorChartDynamicInfo = value;
                SetNode("config/indicatorChart/dynamicInfo", value);
            }
        }

        static bool _isIndicatorChartGrid = IsIndicatorChartGridDefault;
        public static bool IndicatorChartGrid
        {
            get { return _isIndicatorChartGrid; }
            set
            {
                _isIndicatorChartGrid = value;
                SetNode("config/indicatorChart/grid", value);
            }
        }

        static bool _isIndicatorChartCross = IsIndicatorChartCrossDefault;
        public static bool IndicatorChartCross
        {
            get { return _isIndicatorChartCross; }
            set
            {
                _isIndicatorChartCross = value;

                SetNode("config/indicatorChart/cross", value);
            }
        }

        static bool _isIndicatorChartVolume = IsIndicatorChartVolumeDefault;
        public static bool IndicatorChartVolume
        {
            get { return _isIndicatorChartVolume; }
            set
            {
                _isIndicatorChartVolume = value;
                SetNode("config/indicatorChart/volume", value);
            }
        }

        static bool _isIndicatorChartLots = IsIndicatorChartLotsDefault;
        public static bool IndicatorChartLots
        {
            get { return _isIndicatorChartLots; }
            set
            {
                _isIndicatorChartLots = value;
                SetNode("config/indicatorChart/lots", value);
            }
        }

        static bool _isIndicatorChartEntryExitPoints = IsIndicatorChartEntryExitPointsDefault;
        public static bool IndicatorChartEntryExitPoints
        {
            get { return _isIndicatorChartEntryExitPoints; }
            set
            {
                _isIndicatorChartEntryExitPoints = value;
                SetNode("config/indicatorChart/entryExitPoints", value);
            }
        }

        static bool _isIndicatorChartCorrectedPositionPrice = IsIndicatorChartCorrectedPositionPriceDefault;
        public static bool IndicatorChartCorrectedPositionPrice
        {
            get { return _isIndicatorChartCorrectedPositionPrice; }
            set
            {
                _isIndicatorChartCorrectedPositionPrice = value;
                SetNode("config/indicatorChart/correctedPositionPrice", value);
            }
        }

        static bool _isIndicatorChartBalanceEquityChart = IsIndicatorChartBalanceEquityChartDefault;
        public static bool IndicatorChartBalanceEquityChart
        {
            get { return _isIndicatorChartBalanceEquityChart; }
            set
            {
                _isIndicatorChartBalanceEquityChart = value;
                SetNode("config/indicatorChart/balanceEquityChart", value);
            }
        }

        static bool _isIndicatorChartFloatingPLChart = IsIndicatorChartFloatingPLChartDefault;
        public static bool IndicatorChartFloatingPLChart
        {
            get { return _isIndicatorChartFloatingPLChart; }
            set
            {
                _isIndicatorChartFloatingPLChart = value;
                SetNode("config/indicatorChart/floatingPLChart", value);
            }
        }

        static bool _isIndicatorChartIndicators = IsIndicatorChartIndicatorsDefault;
        public static bool IndicatorChartIndicators
        {
            get { return _isIndicatorChartIndicators; }
            set
            {
                _isIndicatorChartIndicators = value;

                SetNode("config/indicatorChart/indicators", value);
            }
        }

        static bool _isIndicatorChartAmbiguousMark = IsIndicatorChartAmbiguousMarkDefault;
        public static bool IndicatorChartAmbiguousMark
        {
            get { return _isIndicatorChartAmbiguousMark; }
            set
            {
                _isIndicatorChartAmbiguousMark = value;
                SetNode("config/indicatorChart/ambiguousMark", value);
            }
        }

        static bool _isIndicatorChartTrueCharts = IsIndicatorChartTrueChartsDefault;
        public static bool IndicatorChartTrueCharts
        {
            get { return _isIndicatorChartTrueCharts; }
            set
            {
                _isIndicatorChartTrueCharts = value;

                SetNode("config/indicatorChart/trueCharts", value);
            }
        }

        static bool _isIndicatorChartProtections = IsIndicatorChartProtectionsDefault;
        public static bool IndicatorChartProtections
        {
            get { return _isIndicatorChartProtections; }
            set
            {
                _isIndicatorChartProtections = value;
                SetNode("config/indicatorChart/protections", value);
            }
        }

        // -------------------------------------------------------------
        static int _balanceChartZoom = BalanceChartZoomDefault;
        public static int BalanceChartZoom
        {
            get { return _balanceChartZoom; }
            set
            {
                _balanceChartZoom = value;
                SetNode("config/balanceChart/zoom", value);
            }
        }

        static bool _isBalanceChartInfoPanel = IsBalanceChartInfoPanelDefault;
        public static bool BalanceChartInfoPanel
        {
            get { return _isBalanceChartInfoPanel; }
            set
            {
                _isBalanceChartInfoPanel = value;
                SetNode("config/balanceChart/infoPanel", value);
            }
        }

        static bool _isBalanceChartDynamicInfo = IsBalanceChartDynamicInfoDefault;
        public static bool BalanceChartDynamicInfo
        {
            get { return _isBalanceChartDynamicInfo; }
            set
            {
                _isBalanceChartDynamicInfo = value;
                SetNode("config/balanceChart/dynamicInfo", value);
            }
        }

        static bool _isBalanceChartGrid = IsBalanceChartGridDefault;
        public static bool BalanceChartGrid
        {
            get { return _isBalanceChartGrid; }
            set
            {
                _isBalanceChartGrid = value;
                SetNode("config/balanceChart/grid", value);
            }
        }

        static bool _isBalanceChartCross = IsBalanceChartCrossDefault;
        public static bool BalanceChartCross
        {
            get { return _isBalanceChartCross; }
            set
            {
                _isBalanceChartCross = value;
                SetNode("config/balanceChart/cross", value);
            }
        }

        static bool _isBalanceChartVolume = IsBalanceChartVolumeDefault;
        public static bool BalanceChartVolume
        {
            get { return _isBalanceChartVolume; }
            set
            {
                _isBalanceChartVolume = value;
                SetNode("config/balanceChart/volume", value);
            }
        }

        static bool _isBalanceChartLots = IsBalanceChartLotsDefault;
        public static bool BalanceChartLots
        {
            get { return _isBalanceChartLots; }
            set
            {
                _isBalanceChartLots = value;
                SetNode("config/balanceChart/lots", value);
            }
        }

        static bool _isBalanceChartEntryExitPoints = IsBalanceChartEntryExitPointsDefault;
        public static bool BalanceChartEntryExitPoints
        {
            get { return _isBalanceChartEntryExitPoints; }
            set
            {
                _isBalanceChartEntryExitPoints = value;
                SetNode("config/balanceChart/entryExitPoints", value);
            }
        }

        static bool _isBalanceChartCorrectedPositionPrice = IsBalanceChartCorrectedPositionPriceDefault;
        public static bool BalanceChartCorrectedPositionPrice
        {
            get { return _isBalanceChartCorrectedPositionPrice; }
            set
            {
                _isBalanceChartCorrectedPositionPrice = value;
                SetNode("config/balanceChart/correctedPositionPrice", value);
            }
        }

        static bool _isBalanceChartBalanceEquityChart = IsBalanceChartBalanceEquityChartDefault;
        public static bool BalanceChartBalanceEquityChart
        {
            get { return _isBalanceChartBalanceEquityChart; }
            set
            {
                _isBalanceChartBalanceEquityChart = value;
                SetNode("config/balanceChart/balanceEquityChart", value);
            }
        }

        static bool _isBalanceChartFloatingPLChart = IsBalanceChartFloatingPLChartDefault;
        public static bool BalanceChartFloatingPLChart
        {
            get { return _isBalanceChartFloatingPLChart; }
            set
            {
                _isBalanceChartFloatingPLChart = value;
                SetNode("config/balanceChart/floatingPLChart", value);
            }
        }

        static bool _isBalanceChartIndicators = IsBalanceChartIndicatorsDefault;
        public static bool BalanceChartIndicators
        {
            get { return _isBalanceChartIndicators; }
            set
            {
                _isBalanceChartIndicators = value;
                SetNode("config/balanceChart/indicators", value);
            }
        }

        static bool _isBalanceChartAmbiguousMark = IsBalanceChartAmbiguousMarkDefault;
        public static bool BalanceChartAmbiguousMark
        {
            get { return _isBalanceChartAmbiguousMark; }
            set
            {
                _isBalanceChartAmbiguousMark = value;
                SetNode("config/balanceChart/ambiguousMark", value);
            }
        }

        static bool _isBalanceChartTrueCharts = IsBalanceChartTrueChartsDefault;
        public static bool BalanceChartTrueCharts
        {
            get { return _isBalanceChartTrueCharts; }
            set
            {
                _isBalanceChartTrueCharts = value;
                SetNode("config/balanceChart/trueCharts", value);
            }
        }

        static bool _isBalanceChartProtections = IsBalanceChartProtectionsDefault;
        public static bool BalanceChartProtections
        {
            get { return _isBalanceChartProtections; }
            set
            {
                _isBalanceChartProtections = value;
                SetNode("config/balanceChart/protections", value);
            }
        }

// ----------------------------------------------------------------------

        /// <summary>
        /// Public constructor
        /// </summary>
        static Configs()
        {
            _xmlConfig = new XmlDocument();
            PathToConfigFile = Path.Combine(Data.SystemDir, "config.xml");
        }

        private static void SetNode(string node, object value)
        {
            if (!_isConfigLoaded) return;
            var xmlNodeList = _xmlConfig.SelectNodes(node);
            if (xmlNodeList == null) return;
            var xmlNode = xmlNodeList.Item(0);
            if (xmlNode != null)
                xmlNode.InnerText = value.ToString();
        }

        /// <summary>
        /// Sets the parameters to its default value
        /// </summary>
        public static void ResetParams()
        {
            if (!_isConfigLoaded)
                return;

            MinBars                   = MinBarsDefault;
            MaxBarsLimit                   = MaxBarsLimitDefault;
            MaxIntraBars             = MaxIntraBarsDefault;
            MaxEntryFilters          = MaxEntryFiltersDefault;
            MaxExitFilters           = MaxExitFiltersDefault;
            SigmaModeMainChart      = SigmaModeMainChartDefault;
            SigmaModeSeparatedChart = SigmaModeSeparatedChartDefault;

            Language                   = LanguageDefault;
            ShowStartingTip            = ShowStartingTipDefault;
            CurrentTipNumber           = CurrentTipNumberDefault;
            DataDirectory              = DataDirectoryDefault;
            ColorScheme                = ColorSchemeDefault;
            RememberLastStr            = IsRememberLastStrDefault;
            LastStrategy               = LastStrategyDefault;
            CheckForUpdates            = IsCheckForUpdatesDefault;
            CheckForNewBeta            = IsCheckForNewBetaDefault;
            CheckData                  = IsCheckDataDefault;
            FillInDataGaps             = IsFillDataGapsDefault;
            CutBadData                 = IsCutBadDataDefault;
            LoadCustomIndicators       = IsLoadCustIndDefault;
            ShowCustomIndicators       = IsShowCustIndDefault;
            MaxBars                    = MaxBarsDefault;
            DataStartTime              = DataStartTimeDefault;
            DataEndTime                = DataEndTimeDefault;
            UseEndTime                 = IsUseEndTimeDefault;
            UseStartTime               = IsUseStartTimeDefault;
            AccountInMoney             = IsAccountInMoneyDefault;
            AccountCurrency            = AccountCurrencyDefault;
            InitialAccount             = InitialAccountDefault;
            Leverage                   = LeverageDefault;
            ShowJournal                = IsShowJournalDefault;
            JournalByBars              = IsJournalByBarsDefault;
            Autoscan                   = IsAutoscanDefault;
            TradeUntilMarginCall       = IsTradeUntilMarginCallDefault;
            AdditionalStatistics       = IsAdditionalStatisticsDefault;
            UseLogicalGroups           = IsUseLogicalGroupsDefault;
            PlaySounds                 = IsPlaySoundsDefault;
            GeneratorOptions           = GeneratorOptionsDefault;
            OptimizerOptions           = OptimizerOptionsDefault;
            ColumnSeparator            = ColumnSeparatorDefault;
            DecimalSeparator           = DecimalSeparatorDefault;
            UseTickData                = UseTickDataDefault;
            JForexDataPath             = JforexDataPathDefault;
            MetaTrader4DataPath        = Metatrader4DataPathDefault;
            MarketClosingHour          = MarketClosingHourDefault;
            MarketOpeningHour          = MarketOpeningHourDefault;
            ImportStartingDate         = ImportStartingDateDefault;
            ImportEndingDate           = ImportEndingDateDefault;
            BannedIndicators           = BannedIndicatorsDefault;
            ShowPriceChartOnAccountChart = ShowPriceChartOnAccountChartDefault;
            AnalyzerHideFSB            = AnalyzerHideFSBDefault;
            MainScreenWidth            = MainScreenWidthDefault;
            MainScreenHeight           = MainScreenHeightDefault;
            ShowStatusBar              = ShowStatusBarDefault;

            // Indicator Chart
            IndicatorChartZoom                   = IndicatorChartZoomDefault;
            IndicatorChartInfoPanel              = IsIndicatorChartInfoPanelDefault;
            IndicatorChartDynamicInfo            = IsIndicatorChartDynamicInfoDefault;
            IndicatorChartGrid                   = IsIndicatorChartGridDefault;
            IndicatorChartCross                  = IsIndicatorChartCrossDefault;
            IndicatorChartVolume                 = IsIndicatorChartVolumeDefault;
            IndicatorChartLots                   = IsIndicatorChartLotsDefault;
            IndicatorChartEntryExitPoints        = IsIndicatorChartEntryExitPointsDefault;
            IndicatorChartCorrectedPositionPrice = IsIndicatorChartCorrectedPositionPriceDefault;
            IndicatorChartBalanceEquityChart     = IsIndicatorChartBalanceEquityChartDefault;
            IndicatorChartFloatingPLChart        = IsIndicatorChartFloatingPLChartDefault;
            IndicatorChartIndicators             = IsIndicatorChartIndicatorsDefault;
            IndicatorChartAmbiguousMark          = IsIndicatorChartAmbiguousMarkDefault;
            IndicatorChartTrueCharts             = IsIndicatorChartTrueChartsDefault;
            IndicatorChartProtections            = IsIndicatorChartProtectionsDefault;

            // Balance Chart
            BalanceChartZoom                   = BalanceChartZoomDefault;
            BalanceChartInfoPanel              = IsBalanceChartInfoPanelDefault;
            BalanceChartDynamicInfo            = IsBalanceChartDynamicInfoDefault;
            BalanceChartGrid                   = IsBalanceChartGridDefault;
            BalanceChartCross                  = IsBalanceChartCrossDefault;
            BalanceChartVolume                 = IsBalanceChartVolumeDefault;
            BalanceChartLots                   = IsBalanceChartLotsDefault;
            BalanceChartEntryExitPoints        = IsBalanceChartEntryExitPointsDefault;
            BalanceChartCorrectedPositionPrice = IsBalanceChartCorrectedPositionPriceDefault;
            BalanceChartBalanceEquityChart     = IsBalanceChartBalanceEquityChartDefault;
            BalanceChartFloatingPLChart        = IsBalanceChartFloatingPLChartDefault;
            BalanceChartIndicators             = IsBalanceChartIndicatorsDefault;
            BalanceChartAmbiguousMark          = IsBalanceChartAmbiguousMarkDefault;
            BalanceChartTrueCharts             = IsBalanceChartTrueChartsDefault;
            BalanceChartProtections            = IsBalanceChartProtectionsDefault;

            SaveConfigs();
            _isResetActivated = true;
        }

        /// <summary>
        /// Parses the config file
        /// </summary>
        static void ParseConfigs()
        {
            // Constants
            _iMinBars                  = ParseNode("config/MIN_BARS", MinBarsDefault);
            _iMaxBarsLimit             = ParseNode("config/MAX_BARS", MaxBarsLimitDefault);
            _iMaxIntraBars             = ParseNode("config/MAX_INTRA_BARS", MaxIntraBarsDefault);
            _iMaxEntryFilters          = ParseNode("config/MAX_ENTRY_FILTERS", MaxEntryFiltersDefault);
            _iMaxExitFilters           = ParseNode("config/MAX_EXIT_FILTERS", MaxExitFiltersDefault);
            _iSigmaModeMainChart       = ParseNode("config/SIGMA_MODE_MAIN_CHART", SigmaModeMainChartDefault);
            _iSigmaModeSeparatedChart  = ParseNode("config/SIGMA_MODE_SEPARATED_CHART", SigmaModeSeparatedChartDefault);

            _isInstalled                  = ParseNode("config/installed", false);
            _language                     = ParseNode("config/language", LanguageDefault);
            _isShowStartingTip            = ParseNode("config/showStartingTip", ShowStartingTipDefault);
            _currentTipNumber             = ParseNode("config/currentTipNumber", CurrentTipNumberDefault);
            _isGradientView               = ParseNode("config/gradientView", IsGradientViewDefault);
            _isShowJournal                = ParseNode("config/showJournal", IsShowJournalDefault);
            _isJournalByBars              = ParseNode("config/journalByBars", IsJournalByBarsDefault);
            _isJournalShowTransfers       = ParseNode("config/journalShowTransfers", IsJournalShowTransfersDefault);
            _isAutoscan                   = ParseNode("config/autoscan", IsAutoscanDefault);
            _isTradeUntilMarginCall       = ParseNode("config/tradeUntilMarginCall", IsTradeUntilMarginCallDefault);
            _isAdditionalStatistics       = ParseNode("config/additionalStatistics", IsAdditionalStatisticsDefault);
            _isUseLogicalGroups           = ParseNode("config/useLogicalGroups", IsUseLogicalGroupsDefault);
            _dataDirectory                = ParseNode("config/dataDirectory", DataDirectoryDefault);
            _colorScheme                  = ParseNode("config/colorScheme", ColorSchemeDefault);
            _isRememberLastStr            = ParseNode("config/rememberLastStrategy", IsRememberLastStrDefault);
            _lastStrategy                 = ParseNode("config/lastStrategy", LastStrategyDefault);
            _isCheckForUpdates            = ParseNode("config/checkForUpdates", IsCheckForUpdatesDefault);
            _isCheckForNewBeta            = ParseNode("config/checkForNewBeta", IsCheckForNewBetaDefault);
            _isCheckData                  = ParseNode("config/checkData", IsCheckDataDefault);
            _isFillDataGaps               = ParseNode("config/fillDataGaps", IsFillDataGapsDefault);
            _isCutBadData                 = ParseNode("config/cutBadData", IsCutBadDataDefault);
            _isLoadCustomIndicators       = ParseNode("config/loadCustomIndicators", IsLoadCustIndDefault);
            _isShowCustomIndicators       = ParseNode("config/showCustomIndicators", IsShowCustIndDefault);
            _isPlaySounds                 = ParseNode("config/playSounds", IsPlaySoundsDefault);
            _generatorOptions             = ParseNode("config/generatorOptions", GeneratorOptionsDefault);
            _optimizerOptions             = ParseNode("config/optimizerOptions", OptimizerOptionsDefault);
            _columnSeparator              = ParseNode("config/columnSeparator", ColumnSeparatorDefault);
            _decimalSeparator             = ParseNode("config/decimalSeparator", DecimalSeparatorDefault);
            _useTickData                  = ParseNode("config/useTickData",  UseTickDataDefault);
            _jforexDataPath               = ParseNode("config/jforexDataPath",  JforexDataPathDefault);
            _metatrader4DataPath          = ParseNode("config/metatrader4DataPath", Metatrader4DataPathDefault);
            _marketClosingHour            = ParseNode("config/marketClosingHour", MarketClosingHourDefault);
            _marketOpeningHour            = ParseNode("config/marketOpeningHour", MarketOpeningHourDefault);
            _importStartingDate           = ParseNode("config/importStartingDate", ImportStartingDateDefault);
            _importEndingDate             = ParseNode("config/importEndingDate", ImportEndingDateDefault); 
            _bannedIndicators             = ParseNode("config/bannedIndicators", BannedIndicatorsDefault);
            _showPriceChartOnAccountChart = ParseNode("config/showPriceChartOnAccountChart", ShowPriceChartOnAccountChartDefault);
            _analyzerHideFSB              = ParseNode("config/analyzerHideFSB", AnalyzerHideFSBDefault);
            _sendUsageStats               = ParseNode("config/sendUsageStats", SendUsageStatsDefault);
            _mainScreenWidth              = ParseNode("config/mainScreenWidth", MainScreenWidthDefault);
            _mainScreenHeight             = ParseNode("config/mainScreenHeight", MainScreenHeightDefault);
            _showStatusBar                = ParseNode("config/showStatusBar", ShowStatusBarDefault);

            // Data Horizon
            _maxBars                      = ParseNode("config/dataMaxBars", MaxBarsDefault);
            _dataStartTime                = ParseNode("config/dataStartTime", DataStartTimeDefault);
            _dataEndTime                  = ParseNode("config/dataEndTime", DataEndTimeDefault);
            _isUseStartTime               = ParseNode("config/dataUseStartTime", IsUseStartTimeDefault);
            _isUseEndTime                 = ParseNode("config/dataUseEndTime", IsUseEndTimeDefault);

            // Account
            _isAccountInMoney             = ParseNode("config/account/accountInMoney", IsAccountInMoneyDefault);
            _accountCurrency              = ParseNode("config/account/accountCurrency", AccountCurrencyDefault);
            _initialAccount               = ParseNode("config/account/initialAccount", InitialAccountDefault);
            _leverage                     = ParseNode("config/account/leverage", LeverageDefault);

            // Indicator Chart
            _indicatorChartZoom                     = ParseNode("config/indicatorChart/zoom", IndicatorChartZoomDefault);
            _isIndicatorChartInfoPanel              = ParseNode("config/indicatorChart/infoPanel", IsIndicatorChartInfoPanelDefault);
            _isIndicatorChartDynamicInfo            = ParseNode("config/indicatorChart/dynamicInfo", IsIndicatorChartDynamicInfoDefault);
            _isIndicatorChartGrid                   = ParseNode("config/indicatorChart/grid", IsIndicatorChartGridDefault);
            _isIndicatorChartCross                  = ParseNode("config/indicatorChart/cross", IsIndicatorChartCrossDefault);
            _isIndicatorChartVolume                 = ParseNode("config/indicatorChart/volume", IsIndicatorChartVolumeDefault);
            _isIndicatorChartLots                   = ParseNode("config/indicatorChart/lots", IsIndicatorChartLotsDefault);
            _isIndicatorChartEntryExitPoints        = ParseNode("config/indicatorChart/entryExitPoints", IsIndicatorChartEntryExitPointsDefault);
            _isIndicatorChartCorrectedPositionPrice = ParseNode("config/indicatorChart/correctedPositionPrice", IsIndicatorChartCorrectedPositionPriceDefault);
            _isIndicatorChartBalanceEquityChart     = ParseNode("config/indicatorChart/balanceEquityChart", IsIndicatorChartBalanceEquityChartDefault);
            _isIndicatorChartFloatingPLChart        = ParseNode("config/indicatorChart/floatingPLChart", IsIndicatorChartFloatingPLChartDefault);
            _isIndicatorChartIndicators             = ParseNode("config/indicatorChart/indicators", IsIndicatorChartIndicatorsDefault);
            _isIndicatorChartAmbiguousMark          = ParseNode("config/indicatorChart/ambiguousMark", IsIndicatorChartAmbiguousMarkDefault);
            _isIndicatorChartTrueCharts             = ParseNode("config/indicatorChart/trueCharts", IsIndicatorChartTrueChartsDefault);
            _isIndicatorChartProtections            = ParseNode("config/indicatorChart/protections", IsIndicatorChartProtectionsDefault);

            // Balance Chart
            _balanceChartZoom                     = ParseNode("config/balanceChart/zoom", BalanceChartZoomDefault);
            _isBalanceChartInfoPanel              = ParseNode("config/balanceChart/infoPanel", IsBalanceChartInfoPanelDefault);
            _isBalanceChartDynamicInfo            = ParseNode("config/balanceChart/dynamicInfo", IsBalanceChartDynamicInfoDefault);
            _isBalanceChartGrid                   = ParseNode("config/balanceChart/grid", IsBalanceChartGridDefault);
            _isBalanceChartCross                  = ParseNode("config/balanceChart/cross", IsBalanceChartCrossDefault);
            _isBalanceChartVolume                 = ParseNode("config/balanceChart/volume", IsBalanceChartVolumeDefault);
            _isBalanceChartLots                   = ParseNode("config/balanceChart/lots", IsBalanceChartLotsDefault);
            _isBalanceChartEntryExitPoints        = ParseNode("config/balanceChart/entryExitPoints", IsBalanceChartEntryExitPointsDefault);
            _isBalanceChartCorrectedPositionPrice = ParseNode("config/balanceChart/correctedPositionPrice", IsBalanceChartCorrectedPositionPriceDefault);
            _isBalanceChartBalanceEquityChart     = ParseNode("config/balanceChart/balanceEquityChart", IsBalanceChartBalanceEquityChartDefault);
            _isBalanceChartFloatingPLChart        = ParseNode("config/balanceChart/floatingPLChart", IsBalanceChartFloatingPLChartDefault);
            _isBalanceChartIndicators             = ParseNode("config/balanceChart/indicators", IsBalanceChartIndicatorsDefault);
            _isBalanceChartAmbiguousMark          = ParseNode("config/balanceChart/ambiguousMark", IsBalanceChartAmbiguousMarkDefault);
            _isBalanceChartTrueCharts             = ParseNode("config/balanceChart/trueCharts", IsBalanceChartTrueChartsDefault);
            _isBalanceChartProtections            = ParseNode("config/balanceChart/protections", IsBalanceChartProtectionsDefault);
        }

        /// <summary>
        /// Sets parameters after loading config file.
        /// </summary>
        static void ConfigAfterLoading()
        {
            if (_maxBars > _iMaxBarsLimit)
                _maxBars = _iMaxBarsLimit;

            if (!_isInstalled)
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey("Software\\Forex Software\\Forex Strategy Builder");
                if (regKey != null)
                    SendUsageStats = (regKey.GetValue("UsageStats") == null || regKey.GetValue("UsageStats").ToString() == "0");
                IsInstalled = true;
            }

            if (_dataDirectory != "" && Directory.Exists(_dataDirectory))
            {
                Data.OfflineDataDir = _dataDirectory;
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
                XmlNodeList list = _xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = int.Parse(xmlNode.InnerText);
                }
                else
                    CreateElement(node, defaultValue.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
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
                XmlNodeList list = _xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = xmlNode.InnerText;
                }
                else
                    CreateElement(node, defaultValue);
            }
            catch (Exception ex)
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
                XmlNodeList list = _xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = bool.Parse(xmlNode.InnerText);
                }
                else
                    CreateElement(node, defaultValue.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
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
                XmlNodeList list = _xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null)
                        value = DateTime.ParseExact(xmlNode.InnerText, DateStringFormat, new DateTimeFormatInfo());
                }
                else
                    CreateElement(node, defaultValue.ToString(DateStringFormat));
            }
            catch (Exception ex)
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
            XmlElement newElem = _xmlConfig.CreateElement(node.Replace("config/", ""));
            newElem.InnerText = value;
            if (_xmlConfig.DocumentElement != null)
                _xmlConfig.DocumentElement.AppendChild(newElem);
        }

        /// <summary>
        /// Loads the config file
        /// </summary>
        public static void LoadConfigs()
        {
            try
            {
                if (!File.Exists(PathToConfigFile))
                {
                    _xmlConfig = new XmlDocument {InnerXml = Properties.Resources.config};
                }
                else
                {
                    _xmlConfig.Load(PathToConfigFile);
                }
                ParseConfigs();
                _isConfigLoaded = true;
                ConfigAfterLoading();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }
        }

        /// <summary>
        /// Saves the config file
        /// </summary>
        public static void SaveConfigs()
        {
            if (_isResetActivated || !_isConfigLoaded) return;

            try
            {
                _xmlConfig.Save(PathToConfigFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Configs");
            }
        }
    }
}
