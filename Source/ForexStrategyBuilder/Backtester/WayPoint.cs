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
    public class WayPoint
    {
        public WayPoint(double price, WayPointType wpType)
        {
            Price = price;
            WpType = wpType;
            OrdNumb = -1;
            PosNumb = -1;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public WayPoint(double price, WayPointType wpType, int ordNumb, int posNumb)
        {
            Price = price;
            WpType = wpType;

            if (wpType == WayPointType.Open || wpType == WayPointType.High ||
                wpType == WayPointType.Low || wpType == WayPointType.Close ||
                wpType == WayPointType.None)
                OrdNumb = -1;
            else
                OrdNumb = ordNumb;

            if (Backtester.PosFromNumb(posNumb).PosDir == PosDirection.None ||
                Backtester.PosFromNumb(posNumb).PosDir == PosDirection.Closed &&
                wpType != WayPointType.Exit && wpType != WayPointType.Reduce)
                PosNumb = -1;
            else
                PosNumb = posNumb;
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        private WayPoint()
        {
        }

        /// <summary>
        ///     Gets or sets the waypoint price
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        ///     Gets or sets the waypoint type
        /// </summary>
        public WayPointType WpType { get; private set; }

        /// <summary>
        ///     Gets or sets the waypoint order number
        /// </summary>
        public int OrdNumb { get; set; }

        /// <summary>
        ///     Gets or sets the waypoint position number
        /// </summary>
        public int PosNumb { get; private set; }

        /// <summary>
        ///     Shows the WayPointType as a string.
        /// </summary>
        public static string WpTypeToString(WayPointType wpType)
        {
            string output;

            switch (wpType)
            {
                case WayPointType.None:
                    output = "None";
                    break;
                case WayPointType.Open:
                    output = "Bar open";
                    break;
                case WayPointType.High:
                    output = "Bar high";
                    break;
                case WayPointType.Low:
                    output = "Bar low";
                    break;
                case WayPointType.Close:
                    output = "Bar close";
                    break;
                case WayPointType.Entry:
                    output = "Entry point";
                    break;
                case WayPointType.Exit:
                    output = "Exit point";
                    break;
                case WayPointType.Add:
                    output = "Adding point";
                    break;
                case WayPointType.Reduce:
                    output = "Reducing point";
                    break;
                case WayPointType.Reverse:
                    output = "Reversing point";
                    break;
                case WayPointType.Cancel:
                    output = "Cancelled order";
                    break;
                default:
                    output = "Error";
                    break;
            }

            return output;
        }

        public WayPoint Copy()
        {
            var wayPoint = new WayPoint
                {
                    Price = Price,
                    WpType = WpType,
                    OrdNumb = OrdNumb,
                    PosNumb = PosNumb
                };
            return wayPoint;
        }
    }
}