// Order class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Order
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Order()
        {
            OrdDir    = OrderDirection.None;
            OrdType   = OrderType.None;
            OrdCond   = OrderCondition.None;
            OrdStatus = OrderStatus.None;
            OrdSender = OrderSender.None;
            OrdOrigin = OrderOrigin.None;
            OrdNote   = "Not Defined";
        }

        /// <summary>
        /// The ID of the order.
        /// </summary>
        public int OrdNumb { get; set; }

        /// <summary>
        /// The amount of the order.
        /// </summary>
        public double OrdLots { get; set; }

        /// <summary>
        /// Zero or the ID number of the other order.
        /// </summary>
        public int OrdIF { get; set; }

        /// <summary>
        /// Zero or the ID number of the target position.
        /// </summary>
        public int OrdPos { get; set; }

        /// <summary>
        /// The order's price.
        /// </summary>
        public double OrdPrice { get; set; }

        /// <summary>
        /// The order's second price.
        /// </summary>
        public double OrdPrice2 { get; set; }

        /// <summary>
        /// The order's direction.
        /// </summary>
        public OrderDirection OrdDir { get; set; }

        /// <summary>
        /// The order's type.
        /// </summary>
        public OrderType OrdType { get; set; }

        /// <summary>
        /// The order's condition.
        /// </summary>
        public OrderCondition OrdCond { get; set; }

        /// <summary>
        /// The order's status.
        /// </summary>
        public OrderStatus OrdStatus { get; set; }

        /// <summary>
        /// The order's sender.
        /// </summary>
        public OrderSender OrdSender { get; set; }

        /// <summary>
        /// The order's origin.
        /// </summary>
        public OrderOrigin OrdOrigin { get; set; }

        /// <summary>
        /// The order's note.
        /// </summary>
        public string OrdNote { get; set; }

        /// <summary>
        /// Gets the order's icon.
        /// </summary>
        public Image OrderIcon
        {
            get
            {
                Image img = Properties.Resources.warning;

                switch (OrdStatus)
                {
                    case OrderStatus.Executed:
                        img = OrdDir == OrderDirection.Buy ? Properties.Resources.ord_buy : Properties.Resources.ord_sell;
                        break;
                    case OrderStatus.Cancelled:
                        img = OrdDir == OrderDirection.Buy ? Properties.Resources.ord_buy_cancel : Properties.Resources.ord_sell_cancel;
                        break;
                }

                return img;
            }
        }

        /// <summary>
        /// Makes a deep copy.
        /// </summary>
        public Order Copy()
        {
            var order = new Order
            {
                OrdDir = OrdDir,
                OrdType = OrdType,
                OrdCond = OrdCond,
                OrdStatus = OrdStatus,
                OrdSender = OrdSender,
                OrdOrigin = OrdOrigin,
                OrdNumb = OrdNumb,
                OrdIF = OrdIF,
                OrdPos = OrdPos,
                OrdLots = OrdLots,
                OrdPrice = OrdPrice,
                OrdPrice2 = OrdPrice2,
                OrdNote = OrdNote
            };

            return order;
        }

        /// <summary>
        /// Represents the position.
        /// </summary>
        public override string ToString()
        {
            string orderd = "";
            string nl = Environment.NewLine;

            orderd += "Number    " + (OrdNumb + 1) + nl;
            orderd += "Direction " + OrdDir        + nl;
            orderd += "Type      " + OrdType       + nl;
            orderd += "Condition " + OrdCond       + nl;
            orderd += "Status    " + OrdStatus     + nl;
            orderd += "Sender    " + OrdSender     + nl;
            orderd += "Origin    " + OrdOrigin     + nl;
            orderd += "If order  " + (OrdIF + 1)   + nl;
            orderd += "To pos    " + (OrdPos + 1)  + nl;
            orderd += "Lots      " + OrdLots       + nl;
            orderd += "Price     " + OrdPrice      + nl;
            orderd += "Price2    " + OrdPrice2     + nl;
            orderd += "Note      " + OrdNote       + nl;

            return orderd;
        }
    }
}
