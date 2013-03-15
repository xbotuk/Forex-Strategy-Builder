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

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Backtester Class
    /// </summary>
    public partial class Backtester
    {
        /// <summary>
        ///     Sets a new order Buy Market.
        /// </summary>
        private static void OrdBuyMarket(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                         OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Buy;
            order.OrdType = OrderType.Market;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Buy Stop.
        /// </summary>
        private static void OrdBuyStop(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                       OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Buy;
            order.OrdType = OrderType.Stop;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Buy Limit.
        /// </summary>
        private static void OrdBuyLimit(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                        OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Buy;
            order.OrdType = OrderType.Limit;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Buy Stop Limit.
        /// </summary>
        private static void OrdBuyStopLimit(int bar, int orderIf, int toPos, double lots, double price1, double price2,
                                            OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Buy;
            order.OrdType = OrderType.StopLimit;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price1, InstrProperties.Digits);
            order.OrdPrice2 = Math.Round(price2, InstrProperties.Digits);
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Sell Market.
        /// </summary>
        private static void OrdSellMarket(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                          OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Sell;
            order.OrdType = OrderType.Market;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Sell Stop.
        /// </summary>
        private static void OrdSellStop(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                        OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Sell;
            order.OrdType = OrderType.Stop;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Sell Limit.
        /// </summary>
        private static void OrdSellLimit(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender,
                                         OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Sell;
            order.OrdType = OrderType.Limit;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price, InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        ///     Sets a new order Sell Stop Limit.
        /// </summary>
        private static void OrdSellStopLimit(int bar, int orderIf, int toPos, double lots, double price1, double price2,
                                             OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = session[bar].Orders;
            Order order = session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb = SentOrders;
            order.OrdDir = OrderDirection.Sell;
            order.OrdType = OrderType.StopLimit;
            order.OrdCond = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIf = orderIf;
            order.OrdPos = toPos;
            order.OrdLots = lots;
            order.OrdPrice = Math.Round(price1, InstrProperties.Digits);
            order.OrdPrice2 = Math.Round(price2, InstrProperties.Digits);
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote = note;

            ordCoord[SentOrders].Bar = bar;
            ordCoord[SentOrders].Ord = sessionOrder;
            session[bar].Orders++;
            SentOrders++;
        }
    }
}