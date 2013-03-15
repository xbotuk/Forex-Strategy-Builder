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
    public class Session
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Session(int maxPos, int maxOrd)
        {
            MaxPos = maxPos;
            MaxOrd = maxOrd;
            MaxWayPoints = maxOrd + 4;
            Positions = 0;
            Orders = 0;
            WayPoints = 0;
            Position = new Position[MaxPos];
            Order = new Order[MaxOrd];
            WayPoint = new WayPoint[MaxWayPoints];
            Position[0] = new Position();
            BacktestEval = BacktestEval.None;
            IsTopReached = false;
            IsBottomReached = false;
        }

        /// <summary>
        ///     The number of positions
        /// </summary>
        public int Positions { get; set; }

        /// <summary>
        ///     The positions during the session
        /// </summary>
        public Position[] Position { get; private set; }

        /// <summary>
        ///     The number of orders
        /// </summary>
        public int Orders { get; set; }

        /// <summary>
        ///     The orders during the session
        /// </summary>
        public Order[] Order { get; private set; }

        /// <summary>
        ///     The position at the end of the session
        /// </summary>
        public Position Summary
        {
            get { return Positions == 0 ? Position[0] : Position[Positions - 1]; }
        }

        /// <summary>
        ///     The backtest's evaluation
        /// </summary>
        public BacktestEval BacktestEval { get; set; }

        /// <summary>
        ///     The count of interpolating steps
        /// </summary>
        public int WayPoints { get; private set; }

        /// <summary>
        ///     The price route
        /// </summary>
        public WayPoint[] WayPoint { get; private set; }

        /// <summary>
        ///     Is the top of the bar was reached
        /// </summary>
        public bool IsTopReached { get; set; }

        /// <summary>
        ///     Is the bottom of the bar was reached
        /// </summary>
        public bool IsBottomReached { get; set; }

        private int MaxPos { get; set; }
        private int MaxOrd { get; set; }
        private int MaxWayPoints { get; set; }

        /// <summary>
        ///     Sets a Way Point
        /// </summary>
        public void SetWayPoint(double price, WayPointType type)
        {
            if (Positions > 0)
                WayPoint[WayPoints] = new WayPoint(price, type, Summary.FormOrdNumb, Summary.PosNumb);
            else
                WayPoint[WayPoints] = new WayPoint(price, type);

            WayPoints++;
        }

        public Session Copy()
        {
            var session = new Session(MaxPos, MaxOrd)
                {
                    Positions = Positions,
                    Orders = Orders,
                    BacktestEval = BacktestEval,
                    WayPoints = WayPoints,
                    IsTopReached = IsTopReached,
                    IsBottomReached = IsBottomReached
                };

            for (int i = 0; i < Positions; i++)
                session.Position[i] = Position[i].Copy();
            for (int i = 0; i < Orders; i++)
                session.Order[i] = Order[i].Copy();
            for (int i = 0; i < WayPoints; i++)
                session.WayPoint[i] = WayPoint[i].Copy();

            return session;
        }
    }
}