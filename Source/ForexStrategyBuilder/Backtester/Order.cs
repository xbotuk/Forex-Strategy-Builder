//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Drawing;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Order
    /// </summary>
    public class Order
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Order()
        {
            OrdDir = OrderDirection.None;
            OrdType = OrderType.None;
            OrdCond = OrderCondition.None;
            OrdStatus = OrderStatus.None;
            OrdSender = OrderSender.None;
            OrdOrigin = OrderOrigin.None;
            OrdNote = "Not Defined";
        }

        /// <summary>
        ///     The ID of the order.
        /// </summary>
        public int OrdNumb { get; set; }

        /// <summary>
        ///     The amount of the order.
        /// </summary>
        public double OrdLots { get; set; }

        /// <summary>
        ///     Zero or the ID number of the other order.
        /// </summary>
        public int OrdIf { get; set; }

        /// <summary>
        ///     Zero or the ID number of the target position.
        /// </summary>
        public int OrdPos { get; set; }

        /// <summary>
        ///     The order's price.
        /// </summary>
        public double OrdPrice { get; set; }

        /// <summary>
        ///     The order's second price.
        /// </summary>
        public double OrdPrice2 { get; set; }

        /// <summary>
        ///     The order's direction.
        /// </summary>
        public OrderDirection OrdDir { get; set; }

        /// <summary>
        ///     The order's type.
        /// </summary>
        public OrderType OrdType { get; set; }

        /// <summary>
        ///     The order's condition.
        /// </summary>
        public OrderCondition OrdCond { get; set; }

        /// <summary>
        ///     The order's status.
        /// </summary>
        public OrderStatus OrdStatus { get; set; }

        /// <summary>
        ///     The order's sender.
        /// </summary>
        public OrderSender OrdSender { get; set; }

        /// <summary>
        ///     The order's origin.
        /// </summary>
        public OrderOrigin OrdOrigin { get; set; }

        /// <summary>
        ///     The order's note.
        /// </summary>
        public string OrdNote { get; set; }

        /// <summary>
        ///     Gets the order's icon.
        /// </summary>
        public OrderIcons OrderIcon
        {
            get
            {
                var icon = OrderIcons.BuyCancel;

                switch (OrdStatus)
                {
                    case OrderStatus.Executed:
                        icon = OrdDir == OrderDirection.Buy ? OrderIcons.Buy : OrderIcons.Sell;
                        break;
                    case OrderStatus.Cancelled:
                        icon = OrdDir == OrderDirection.Buy ? OrderIcons.BuyCancel : OrderIcons.SellCancel;
                        break;
                }

                return icon;
            }
        }

        /// <summary>
        ///     Gets the order's icon
        /// </summary>
        public static Image OrderIconImage(OrderIcons icon)
        {
            Image img = Resources.pos_square;

            switch (icon)
            {
                case OrderIcons.Buy:
                    img = Resources.ord_buy;
                    break;
                case OrderIcons.Sell:
                    img = Resources.ord_sell;
                    break;
                case OrderIcons.BuyCancel:
                    img = Resources.ord_buy_cancel;
                    break;
                case OrderIcons.SellCancel:
                    img = Resources.ord_sell_cancel;
                    break;
            }

            return img;
        }

        /// <summary>
        ///     Makes a deep copy.
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
                    OrdIf = OrdIf,
                    OrdPos = OrdPos,
                    OrdLots = OrdLots,
                    OrdPrice = OrdPrice,
                    OrdPrice2 = OrdPrice2,
                    OrdNote = OrdNote
                };

            return order;
        }

        /// <summary>
        ///     Represents the position.
        /// </summary>
        public override string ToString()
        {
            string orderd = "";
            string nl = Environment.NewLine;

            orderd += "Number    " + (OrdNumb + 1) + nl;
            orderd += "Direction " + OrdDir + nl;
            orderd += "Type      " + OrdType + nl;
            orderd += "Condition " + OrdCond + nl;
            orderd += "Status    " + OrdStatus + nl;
            orderd += "Sender    " + OrdSender + nl;
            orderd += "Origin    " + OrdOrigin + nl;
            orderd += "If order  " + (OrdIf + 1) + nl;
            orderd += "To pos    " + (OrdPos + 1) + nl;
            orderd += "Lots      " + OrdLots + nl;
            orderd += "Price     " + OrdPrice + nl;
            orderd += "Price2    " + OrdPrice2 + nl;
            orderd += "Note      " + OrdNote + nl;

            return orderd;
        }
    }
}