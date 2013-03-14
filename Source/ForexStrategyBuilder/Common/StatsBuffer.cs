// StatsBuffer class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.Common
{
    /// <summary>
    ///     This class stores raw data used as a source for
    ///     calculation of user interface panels.
    /// </summary>
    public static class StatsBuffer
    {
        private static Session[] session;
        private static OrderCoordinates[] ordCoord;
        private static PositionCoordinates[] posCoord;

        /// <summary>
        ///     Gets a value showing if StatsBuffer has valid parameters.
        /// </summary>
        public static bool IsStatsBufferValid
        {
            get { return session != null && ordCoord != null && posCoord != null; }
        }

        /// <summary>
        ///     A copy of the strategy with calculated indicators.
        /// </summary>
        public static Strategy Strategy { get; private set; }

        /// <summary>
        ///     Shortcut to the strategy's first bar.
        /// </summary>
        public static int FirstBar
        {
            get { return Strategy.FirstBar; }
        }

        /// <summary>
        ///     Gets the position coordinates.
        /// </summary>
        public static PositionCoordinates[] PosCoordinates
        {
            get { return posCoord; }
        }

        /// <summary>
        ///     Gets the total number of the positions.
        /// </summary>
        public static int PositionsTotal { get; private set; }

        /// <summary>
        ///     Forces the buffer to collect data.
        /// </summary>
        public static void UpdateStatsBuffer()
        {
            Strategy = Data.Strategy.Clone();
            PositionsTotal = Backtester.PositionsTotal;
            session = Backtester.GetAllSessionsCopy();
            posCoord = Backtester.GetPosCoordinateCopy();
            ordCoord = Backtester.GetOrdCoordinateCopy();
        }

        /// <summary>
        ///     Checks whether we have a position. "Closed" is also a position.
        /// </summary>
        public static bool IsPos(int bar)
        {
            PosDirection dir = session[bar].Summary.PosDir;
            return dir == PosDirection.Long
                   || dir == PosDirection.Short
                   || dir == PosDirection.Closed;
        }

        /// <summary>
        ///     Returns the position's Profit Loss in pips.
        /// </summary>
        public static int ProfitLoss(int bar)
        {
            return (int) Math.Round(session[bar].Summary.ProfitLoss);
        }

        /// <summary>
        ///     Returns the bar end Profit Loss in currency.
        /// </summary>
        public static double MoneyProfitLoss(int bar)
        {
            return session[bar].Summary.MoneyProfitLoss;
        }

        /// <summary>
        ///     Returns the Floating Profit Loss at the end of the bar in pips
        /// </summary>
        public static int FloatingPL(int bar)
        {
            return (int) Math.Round(session[bar].Summary.FloatingPL);
        }

        /// <summary>
        ///     Returns the bar end Floating Profit Loss in currency
        /// </summary>
        public static double MoneyFloatingPL(int bar)
        {
            return session[bar].Summary.MoneyFloatingPL;
        }

        /// <summary>
        ///     Position lots at the end of the bar.
        /// </summary>
        public static double SummaryLots(int bar)
        {
            return session[bar].Summary.PosLots;
        }

        /// <summary>
        ///     Position direction at the end of the bar
        /// </summary>
        public static PosDirection SummaryDir(int bar)
        {
            return session[bar].Summary.PosDir;
        }

        /// <summary>
        ///     Position amount at the end of the bar.
        /// </summary>
        public static int SummaryAmount(int bar)
        {
            return (int) Math.Round(session[bar].Summary.PosLots*Data.InstrProperties.LotSize);
        }

        /// <summary>
        ///     The last transaction for the bar.
        /// </summary>
        public static Transaction SummaryTrans(int bar)
        {
            return session[bar].Summary.Transaction;
        }

        /// <summary>
        ///     Position price at the end of the bar.
        /// </summary>
        public static double SummaryPrice(int bar)
        {
            return session[bar].Summary.PosPrice;
        }

        /// <summary>
        ///     Returns the Required Margin at the end of the bar
        /// </summary>
        public static double SummaryRequiredMargin(int bar)
        {
            return session[bar].Summary.RequiredMargin;
        }

        /// <summary>
        ///     Returns the Free Margin at the end of the bar
        /// </summary>
        public static double SummaryFreeMargin(int bar)
        {
            return session[bar].Summary.FreeMargin;
        }

        /// <summary>
        ///     The position's Icon
        /// </summary>
        public static PositionIcons SummaryPositionIcon(int bar)
        {
            return session[bar].Summary.PositionIcon;
        }

        /// <summary>
        ///     Returns the backtest safety evaluation
        /// </summary>
        public static string BackTestEvalToString(int bar)
        {
            return bar < FirstBar || session[bar].BacktestEval == BacktestEval.None
                       ? ""
                       : session[bar].BacktestEval.ToString();
        }

        /// <summary>
        ///     Returns the account balance at the end of the bar in pips
        /// </summary>
        public static int Balance(int bar)
        {
            return (int) Math.Round(session[bar].Summary.Balance);
        }

        /// <summary>
        ///     Returns the equity at the end of the bar in pips
        /// </summary>
        public static int Equity(int bar)
        {
            return (int) Math.Round(session[bar].Summary.Equity);
        }

        /// <summary>
        ///     Returns the account balance in currency
        /// </summary>
        public static double MoneyBalance(int bar)
        {
            return session[bar].Summary.MoneyBalance;
        }

        /// <summary>
        ///     Returns the current bill in currency.
        /// </summary>
        public static double MoneyEquity(int bar)
        {
            return session[bar].Summary.MoneyEquity;
        }

        /// <summary>
        ///     Returns the number of orders for the designated bar
        /// </summary>
        public static int Orders(int bar)
        {
            return session[bar].Orders;
        }

        /// <summary>
        ///     Bar's way points count.
        /// </summary>
        public static int WayPoints(int bar)
        {
            return session[bar].WayPoints;
        }

        /// <summary>
        ///     Way point
        /// </summary>
        public static WayPoint WayPoint(int bar, int wayPointNumber)
        {
            return session[bar].WayPoint[wayPointNumber];
        }

        /// <summary>
        ///     Returns the Order Number
        /// </summary>
        public static int OrdNumb(int bar, int ord)
        {
            return session[bar].Order[ord].OrdNumb;
        }

        /// <summary>
        ///     Returns the order with the corresponding number
        /// </summary>
        public static Order OrdFromNumb(int ordNumber)
        {
            if (ordNumber < 0) ordNumber = 0;
            return session[ordCoord[ordNumber].Bar].Order[ordCoord[ordNumber].Ord];
        }

        /// <summary>
        ///     Number of the positions during de session.
        /// </summary>
        public static int Positions(int bar)
        {
            return session[bar].Positions;
        }

        /// <summary>
        ///     The position Profit Loss
        /// </summary>
        public static double PosProfitLoss(int bar, int pos)
        {
            return session[bar].Position[pos].ProfitLoss;
        }

        /// <summary>
        ///     The position Floating P/L
        /// </summary>
        public static double PosFloatingPL(int bar, int pos)
        {
            return session[bar].Position[pos].FloatingPL;
        }

        /// <summary>
        ///     The position Profit Loss in currency
        /// </summary>
        public static double PosMoneyProfitLoss(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyProfitLoss;
        }

        /// <summary>
        ///     The position Floating Profit Loss in currency
        /// </summary>
        public static double PosMoneyFloatingPL(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyFloatingPL;
        }

        /// <summary>
        ///     The position's corrected price
        /// </summary>
        public static double PosPrice(int bar, int pos)
        {
            return session[bar].Position[pos].PosPrice;
        }

        /// <summary>
        ///     The position's Transaction
        /// </summary>
        public static Transaction PosTransaction(int bar, int pos)
        {
            return session[bar].Position[pos].Transaction;
        }

        /// <summary>
        ///     The position forming order number
        /// </summary>
        public static int PosOrdNumb(int bar, int pos)
        {
            return session[bar].Position[pos].FormOrdNumb;
        }

        /// <summary>
        ///     The number of the position
        /// </summary>
        public static int PosNumb(int bar, int pos)
        {
            return session[bar].Position[pos].PosNumb;
        }

        /// <summary>
        ///     Returns the position with the required number
        /// </summary>
        public static Position PosFromNumb(int posNumber)
        {
            if (posNumber < 0) posNumber = 0;
            return session[posCoord[posNumber].Bar].Position[posCoord[posNumber].Pos];
        }

        /// <summary>
        ///     Last Position's number.
        /// </summary>
        public static int SummaryPosNumb(int bar)
        {
            return session[bar].Summary.PosNumb;
        }

        /// <summary>
        ///     The position direction
        /// </summary>
        public static PosDirection PosDir(int bar, int pos)
        {
            return session[bar].Position[pos].PosDir;
        }

        /// <summary>
        ///     The position lots
        /// </summary>
        public static double PosLots(int bar, int pos)
        {
            return session[bar].Position[pos].PosLots;
        }

        /// <summary>
        ///     The position forming order price
        /// </summary>
        public static double PosOrdPrice(int bar, int pos)
        {
            return session[bar].Position[pos].FormOrdPrice;
        }

        /// <summary>
        ///     The position's Icon
        /// </summary>
        public static PositionIcons PosIcon(int bar, int pos)
        {
            return session[bar].Position[pos].PositionIcon;
        }
    }
}