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
    ///     Class Backtester
    /// </summary>
    public partial class Backtester
    {
        /// <summary>
        ///     Arranges the orders inside the bar.
        /// </summary>
        private static void BarInterpolation(int bar)
        {
            BacktestEval eval;
            double open = Open[bar];
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];
            double current = open;

            int reachedIntrabar = 0;
            int tradedIntrabar = -1;
            int reachedTick = 0;

            int lastPosBreakEven = -1;
            int lastPosActivatedBE = -1;

            do
            {
                Order orderHigher = null;
                Order orderLower = null;
                double priceHigher = high;
                double priceLower = low;
                bool isHigherPrice = false;
                bool isLowerPrice = false;
                bool isTopReachable = true;
                bool isBottomReachable = true;
                bool isClosingAmbiguity = false;
                bool isScanningResult = false;

                // Break Even
                int currentPosNumber = session[bar].Summary.PosNumb;
                if (Strategy.UseBreakEven && lastPosBreakEven != currentPosNumber && IsOpenPos(bar))
                {
                    if (lastPosActivatedBE != currentPosNumber && !session[bar].Summary.IsBreakEvenActivated)
                    {
                        SetBreakEvenActivation(bar);
                        lastPosActivatedBE = currentPosNumber;
                    }

                    if (SetBreakEvenExit(bar, current, lastPosBreakEven))
                        lastPosBreakEven = currentPosNumber;
                }

                // Setting the parameters
                for (int ord = 0; ord < session[bar].Orders; ord++)
                {
                    if (!CheckOrd(bar, ord)) continue;

                    Order order = session[bar].Order[ord];
                    var prices = new[] {order.OrdPrice, order.OrdPrice2};
                    foreach (double price in prices)
                    {
                        if (high + sigma <= price || price <= low - sigma)
                            continue;

                        if (isTopReachable)
                            isTopReachable = current > price + sigma;

                        if (isBottomReachable)
                            isBottomReachable = current < price - sigma;

                        if (price > current - sigma && price < priceHigher + sigma)
                        {
                            // New nearer Upper price
                            isHigherPrice = true;
                            priceHigher = price;
                            orderHigher = order;
                            isTopReachable = false;
                        }
                        else if (price < current && price > priceLower - sigma)
                        {
                            // New nearer Lower price
                            isLowerPrice = true;
                            priceLower = price;
                            orderLower = order;
                            isBottomReachable = false;
                        }
                    }
                }

                // Evaluate the bar
                if (!isLowerPrice && !isHigherPrice)
                {
                    // No more orders
                    eval = BacktestEval.None;
                }
                else if (isLowerPrice && isHigherPrice)
                {
                    // There are a higher and a lower order
                    eval = BacktestEval.Ambiguous;
                }
                else if (isHigherPrice && priceHigher - current < sigma)
                {
                    // There is an order at the current price
                    eval = BacktestEval.Correct;
                }
                else if (isLowerPrice && current - priceLower < sigma)
                {
                    // There is an order at the current price
                    eval = BacktestEval.Correct;
                }
                else
                {
                    // Check for a Closing Ambiguity
                    if (session[bar].IsBottomReached && session[bar].IsTopReached &&
                        current > close - sigma && close > priceLower)
                        isClosingAmbiguity = true;

                    else if (session[bar].IsBottomReached && session[bar].IsTopReached &&
                             current < close + sigma && close < priceHigher)
                        isClosingAmbiguity = true;

                    else if (session[bar].IsTopReached && isHigherPrice &&
                             current > close - sigma)
                        isClosingAmbiguity = true;

                    else if (session[bar].IsBottomReached && isLowerPrice &&
                             current < close + sigma)
                        isClosingAmbiguity = true;

                    eval = isClosingAmbiguity
                               ? BacktestEval.Ambiguous
                               : BacktestEval.Correct;
                }


                if (isScanning && Configs.UseTickData && IsTickData && TickData[bar] != null)
                {
                    isScanningResult = TickScanning(bar, eval,
                                                    ref current, ref reachedTick,
                                                    isTopReachable, isBottomReachable,
                                                    isHigherPrice, isLowerPrice,
                                                    priceHigher, priceLower,
                                                    orderHigher, orderLower,
                                                    isClosingAmbiguity);
                }

                if (isScanning && !isScanningResult && IntraBarsPeriods[bar] != Period)
                {
                    isScanningResult = IntrabarScanning(bar, eval, ref current,
                                                        ref reachedIntrabar, ref tradedIntrabar,
                                                        isTopReachable, isBottomReachable,
                                                        isHigherPrice, isLowerPrice,
                                                        priceHigher, priceLower,
                                                        orderHigher, orderLower,
                                                        isClosingAmbiguity);
                }

                if (isScanningResult) continue;

                switch (InterpolationMethod)
                {
                        // Calls a method
                    case InterpolationMethod.Optimistic:
                    case InterpolationMethod.Pessimistic:
                        OptimisticPessimisticMethod(bar, eval, ref current,
                                                    isTopReachable, isBottomReachable,
                                                    isHigherPrice, isLowerPrice,
                                                    priceHigher, priceLower,
                                                    orderHigher, orderLower,
                                                    isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Shortest:
                        ShortestMethod(bar, eval, ref current,
                                       isTopReachable, isBottomReachable,
                                       isHigherPrice, isLowerPrice,
                                       priceHigher, priceLower,
                                       orderHigher, orderLower,
                                       isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Nearest:
                        NearestMethod(bar, eval, ref current,
                                      isTopReachable, isBottomReachable,
                                      isHigherPrice, isLowerPrice,
                                      priceHigher, priceLower,
                                      orderHigher, orderLower,
                                      isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Random:
                        RandomMethod(bar, eval, ref current,
                                     isTopReachable, isBottomReachable,
                                     isHigherPrice, isLowerPrice,
                                     priceHigher, priceLower,
                                     orderHigher, orderLower,
                                     isClosingAmbiguity);
                        break;
                }
            } while (!(eval == BacktestEval.None && session[bar].IsTopReached && session[bar].IsBottomReached));
        }

        /// <summary>
        ///     Tick Scanning
        /// </summary>
        private static bool TickScanning(int bar, BacktestEval eval,
                                         ref double current, ref int reachedTick,
                                         bool isTopReachable, bool isBottomReachable,
                                         bool isHigherPrice, bool isLowerPrice,
                                         double priceHigher, double priceLower,
                                         Order orderHigher, Order orderLower,
                                         bool isClosingAmbiguity)
        {
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {
                // There isn't any orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] + sigma > high)
                        {
                            // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        if (TickData[bar][tick] - sigma < low)
                        {
                            // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Whether hit the Top
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] + sigma > high)
                        {
                            // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Whether hit the Bottom
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] - sigma < low)
                        {
                            // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top / bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the Bottom
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] + sigma > thePrice)
                        {
                            // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        if (TickData[bar][tick] - sigma < low)
                        {
                            // Bottom is reached
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] + sigma > high)
                        {
                            // The Top is reached
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        if (TickData[bar][tick] - sigma < thePrice)
                        {
                            // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Execute the order
                    double priceOld = TickData[bar][reachedTick];
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (priceOld - sigma < thePrice && TickData[bar][tick] + sigma > thePrice ||
                            priceOld + sigma > thePrice && TickData[bar][tick] - sigma < thePrice)
                        {
                            // Order reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the the first reached order
                    int tickCount = TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (TickData[bar][tick] + sigma > priceHigher)
                        {
                            // Upper order is reached
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        if (TickData[bar][tick] - sigma < priceLower)
                        {
                            // Lower order is reached
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Execute or exit the bar
                    var theOrder = new Order();
                    double thePrice = 0.0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        int tickCount = TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (TickData[bar][tick] + sigma > thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        int tickCount = TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (TickData[bar][tick] - sigma < thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {
                        // Execute the order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                    }
                    else
                    {
                        // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        session[bar].BacktestEval = BacktestEval.Correct;
                    }

                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        ///     Intrabar Scanning
        /// </summary>
        private static bool IntrabarScanning(int bar, BacktestEval eval, ref double current,
                                             ref int reachedIntrabar, ref int tradedIntrabar,
                                             bool isTopReachable, bool isBottomReachable,
                                             bool isHigherPrice, bool isLowerPrice,
                                             double priceHigher, double priceLower,
                                             Order orderHigher, Order orderLower,
                                             bool isClosingAmbiguity)
        {
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > high)
                        {
                            // Top found
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (IntraBarData[bar][intraBar].Low - sigma < low)
                        {
                            // Bottom found
                            if (isScanningResult)
                            {
                                // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Hit the Top
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                        }
                        else
                        {
                            // Hit the Bottom
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Whether hit the Top
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > high)
                        {
                            // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Whether hit the Bottom
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].Low - sigma < low)
                        {
                            // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top / bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > thePrice)
                        {
                            // The order is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (IntraBarData[bar][intraBar].Low - sigma < low)
                        {
                            // Bottom is reached
                            if (isScanningResult)
                            {
                                // The Order and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                        else
                        {
                            // Hit the Bottom
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > high)
                        {
                            // The Top is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (IntraBarData[bar][intraBar].Low - sigma < thePrice)
                        {
                            // The order is reached
                            if (isScanningResult)
                            {
                                // The Top and the order are into the same intrabar
                                isScanningResult = false;
                                break;
                            }

                            // The order is reachable downwards
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Hit the Top
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                        }
                        else
                        {
                            // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                    }
                }
                else
                {
                    // Execute the order
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > thePrice &&
                            IntraBarData[bar][intraBar].Low - sigma < thePrice)
                        {
                            // Order reached
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            isScanningResult = true;
                            tradedIntrabar = reachedIntrabar;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the the first reached order
                    bool executeUpper = false;
                    for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (IntraBarData[bar][intraBar].High + sigma > priceHigher)
                        {
                            // Upper order is reached
                            executeUpper = true;
                            isScanningResult = true;
                        }

                        if (IntraBarData[bar][intraBar].Low - sigma < priceLower)
                        {
                            // Lower order is reached
                            if (isScanningResult)
                            {
                                // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        Order theOrder;
                        double thePrice;
                        if (executeUpper)
                        {
                            theOrder = orderHigher;
                            thePrice = priceHigher;
                        }
                        else
                        {
                            theOrder = orderLower;
                            thePrice = priceLower;
                        }

                        if (tradedIntrabar == reachedIntrabar)
                        {
                            return false;
                        }
                        eval = BacktestEval.Correct;
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                }
                else
                {
                    // Execute or exit the bar
                    var theOrder = new Order();
                    double thePrice = 0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        for (int intraBar = reachedIntrabar; intraBar < IntraBarBars[bar]; intraBar++)
                        {
                            reachedIntrabar = intraBar;

                            if (IntraBarData[bar][intraBar].High + sigma > thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        for (int b = reachedIntrabar; b < IntraBarBars[bar]; b++)
                        {
                            reachedIntrabar = b;

                            if (IntraBarData[bar][b].Low - sigma < thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {
                        // Execute the order
                        if (tradedIntrabar == reachedIntrabar)
                        {
                            return false;
                        }
                        current = thePrice;
                        eval = BacktestEval.Correct;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                    else
                    {
                        // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        session[bar].BacktestEval = BacktestEval.Correct;
                    }
                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        ///     Random Execution Method
        /// </summary>
        private static void RandomMethod(int bar, BacktestEval eval, ref double current,
                                         bool isTopReachable, bool isBottomReachable,
                                         bool isHigherPrice, bool isLowerPrice,
                                         double priceHigher, double priceLower,
                                         Order orderHigher, Order orderLower,
                                         bool isClosingAmbiguity)
        {
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];
            var random = new Random();

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    var upRange = (int) Math.Round((high - current)/InstrProperties.Point);
                    var downRange = (int) Math.Round((current - low)/InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool isHitHigh;

                    if (upRange > downRange)
                        isHitHigh = rand > upRange;
                    else
                        isHitHigh = rand < downRange;

                    if (isHitHigh)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    var iUpRange = (int) Math.Round((thePrice - current)/InstrProperties.Point);
                    var iDnRange = (int) Math.Round((current - low)/InstrProperties.Point);
                    iUpRange = iUpRange < 0 ? 0 : iUpRange;
                    iDnRange = iDnRange < 0 ? 0 : iDnRange;
                    if (iDnRange + iDnRange == 0)
                    {
                        iUpRange = 1;
                        iDnRange = 1;
                    }
                    int rand = random.Next(iUpRange + iDnRange);
                    bool executeUpper;

                    if (iUpRange > iDnRange)
                        executeUpper = rand > iUpRange;
                    else
                        executeUpper = rand < iDnRange;

                    if (executeUpper)
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    var upRange = (int) Math.Round((high - current)/InstrProperties.Point);
                    var downRange = (int) Math.Round((current - thePrice)/InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool executeUpper;

                    if (upRange > downRange)
                        executeUpper = rand > upRange;
                    else
                        executeUpper = rand < downRange;

                    if (executeUpper)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the randomly chosen order
                    var upRange = (int) Math.Round((priceHigher - current)/InstrProperties.Point);
                    var downRange = (int) Math.Round((current - priceLower)/InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool executeUpper;

                    if (upRange > downRange)
                        executeUpper = rand > upRange;
                    else
                        executeUpper = rand < downRange;

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        var upRange = (int) Math.Round((priceHigher - current)/InstrProperties.Point);
                        var downRange = (int) Math.Round((close - current)/InstrProperties.Point);
                        upRange = upRange < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange = 1;
                            downRange = 0;
                        }
                        int rand = random.Next(upRange + downRange);

                        if (rand > upRange)
                        {
                            // Execute the order
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        var upRange = (int) Math.Round((current - close)/InstrProperties.Point);
                        var downRange = (int) Math.Round((current - priceLower)/InstrProperties.Point);
                        upRange = upRange < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange = 0;
                            downRange = 1;
                        }
                        int rand = random.Next(upRange + downRange);

                        if (rand > downRange)
                        {
                            // Execute the order
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Nearest order first Method
        /// </summary>
        private static void NearestMethod(int bar, BacktestEval eval, ref double current,
                                          bool isTopReachable, bool isBottomReachable,
                                          bool isHigherPrice, bool isLowerPrice,
                                          double priceHigher, double priceLower,
                                          Order orderHigher, Order orderLower,
                                          bool isClosingAmbiguity)
        {
            double open = Open[bar];
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (close < open)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    double upRange = thePrice - current;
                    double downRange = current - low;

                    if (upRange < downRange)
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the bottom
                    double upRange = high - current;
                    double downRange = current - thePrice;

                    if (upRange < downRange)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the nearest order
                    double upRange = priceHigher - current;
                    double downRange = current - priceLower;

                    Order theOrder;
                    double thePrice;
                    if (upRange < downRange)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Exit the bar
                    double orderRange = isHigherPrice ? priceHigher - current : current - priceLower;
                    double closeRange = Math.Abs(current - close);

                    if (orderRange < closeRange)
                    {
                        // Execute the order
                        Order theOrder;
                        double thePrice;
                        if (isHigherPrice)
                        {
                            theOrder = orderHigher;
                            thePrice = priceHigher;
                        }
                        else
                        {
                            theOrder = orderLower;
                            thePrice = priceLower;
                        }
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Cancel the order, go to Close
                        current = close;
                        session[bar].BacktestEval = BacktestEval.Ambiguous;
                        if (isHigherPrice)
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                        else if (isLowerPrice)
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                    }
                }
            }
        }

        /// <summary>
        ///     Shortest route inside the bar Method
        /// </summary>
        private static void ShortestMethod(int bar, BacktestEval eval, ref double current,
                                           bool isTopReachable, bool isBottomReachable,
                                           bool isHigherPrice, bool isLowerPrice,
                                           double priceHigher, double priceLower,
                                           Order orderHigher, Order orderLower,
                                           bool isClosingAmbiguity)
        {
            double open = Open[bar];
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];

            bool isGoUpward;
            if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                isGoUpward = open > close;
            else if (session[bar].IsTopReached && !session[bar].IsBottomReached)
                isGoUpward = false;
            else if (!session[bar].IsTopReached && session[bar].IsBottomReached)
                isGoUpward = true;
            else
                isGoUpward = open > close;

            if (isLowerPrice && current - priceLower < sigma)
                isGoUpward = false;
            if (isHigherPrice && priceHigher - current < sigma)
                isGoUpward = true;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (isGoUpward)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the top/bottom or execute
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable && !isGoUpward)
                {
                    // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
                else if (!session[bar].IsTopReached && isTopReachable && isGoUpward)
                {
                    // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the nearest order
                    Order theOrder;
                    double thePrice;

                    if (isGoUpward)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Exit the bar
                    current = close;
                    session[bar].BacktestEval = BacktestEval.Ambiguous;
                    if (isHigherPrice)
                        orderHigher.OrdStatus = OrderStatus.Cancelled;
                    else if (isLowerPrice)
                        orderLower.OrdStatus = OrderStatus.Cancelled;
                }
            }
        }

        /// <summary>
        ///     Optimistic / Pessimistic Method
        /// </summary>
        private static void OptimisticPessimisticMethod(int bar, BacktestEval eval, ref double current,
                                                        bool isTopReachable, bool isBottomReachable,
                                                        bool isHigherPrice, bool isLowerPrice,
                                                        double priceHigher, double priceLower,
                                                        Order orderHigher, Order orderLower,
                                                        bool isClosingAmbiguity)
        {
            double open = Open[bar];
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];

            bool isOptimistic = InterpolationMethod == InterpolationMethod.Optimistic;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (close < open)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                var theOrder = new Order();
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    bool goUpward;

                    if (current < low + sigma)
                        goUpward = false;
                    else if (thePrice - current < sigma)
                        goUpward = true;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = thePrice - current < current - low;

                    if (goUpward)
                    {
                        // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the top
                    bool goUpward;

                    if (current > high - sigma)
                        goUpward = true;
                    else if (current - thePrice < sigma)
                        goUpward = false;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = high - current < current - thePrice;

                    if (goUpward)
                    {
                        // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute one of both orders
                    bool executeUpper;

                    if (priceHigher - current < sigma)
                        executeUpper = true;
                    else if (current - priceLower < sigma)
                        executeUpper = false;
                    else if (session[bar].Summary.PosDir == PosDirection.Long)
                        executeUpper = isOptimistic;
                    else if (session[bar].Summary.PosDir == PosDirection.Short)
                        executeUpper = !isOptimistic;
                    else
                    {
                        if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Buy)
                            executeUpper = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell && orderLower.OrdDir == OrderDirection.Sell)
                            executeUpper = isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Sell)
                        {
                            if (current < close)
                                executeUpper = isOptimistic;
                            else
                                executeUpper = !isOptimistic;

                            if (Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                        else
                        {
                            if (current < close)
                                executeUpper = !isOptimistic;
                            else
                                executeUpper = isOptimistic;

                            if (Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                    }

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;

                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        bool toExecute = false;
                        if (session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = isOptimistic;
                        else if (session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell)
                            toExecute = isOptimistic;

                        if (toExecute)
                        {
                            // Execute
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        bool toExecute = false;

                        if (session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = !isOptimistic;
                        else if (session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Buy)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Sell)
                            toExecute = !isOptimistic;

                        if (toExecute)
                        {
                            // Execute
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }
        }
    }
}