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

namespace ForexStrategyBuilder.Indicators
{
    public partial class Indicator
    {
        /// <summary>
        ///     Calculates the base price.
        /// </summary>
        /// <param name="priceType">The base price type.</param>
        /// <returns>Base price.</returns>
        protected static double[] Price(BasePrice priceType)
        {
            var price = new double[Bars];

            switch (priceType)
            {
                case BasePrice.Open:
                    price = Open;
                    break;
                case BasePrice.High:
                    price = High;
                    break;
                case BasePrice.Low:
                    price = Low;
                    break;
                case BasePrice.Close:
                    price = Close;
                    break;
                case BasePrice.Median:
                    for (int bar = 0; bar < Bars; bar++)
                        price[bar] = (Low[bar] + High[bar])/2;
                    break;
                case BasePrice.Typical:
                    for (int bar = 0; bar < Bars; bar++)
                        price[bar] = (Low[bar] + High[bar] + Close[bar])/3;
                    break;
                case BasePrice.Weighted:
                    for (int bar = 0; bar < Bars; bar++)
                        price[bar] = (Low[bar] + High[bar] + 2*Close[bar])/4;
                    break;
            }
            return price;
        }

        /// <summary>
        ///     Calculates a Moving Average
        /// </summary>
        /// <param name="period">Period</param>
        /// <param name="shift">Shift</param>
        /// <param name="maMethod">Method of calculation</param>
        /// <param name="source">The array of source data</param>
        /// <returns>the Moving Average</returns>
        protected static double[] MovingAverage(int period, int shift, MAMethod maMethod, double[] source)
        {
            var movingAverage = new double[Bars];

            if (period <= 1 && shift == 0)
            {
                // There is no smoothing
                return source;
            }

            if (period > Bars || period + shift <= 0 || period + shift > Bars)
            {
                // Error in the parameters
                return null;
            }

            for (int bar = 0; bar < period + shift - 1; bar++)
            {
                movingAverage[bar] = 0;
            }

            double sum = 0;
            for (int bar = 0; bar < period; bar++)
            {
                sum += source[bar];
            }

            movingAverage[period + shift - 1] = sum/period;

            switch (maMethod)
            {
                case MAMethod.Simple:
                    for (int bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                    {
                        movingAverage[bar + shift] = movingAverage[bar + shift - 1] + source[bar]/period -
                                                     source[bar - period]/period;
                    }
                    break;
                case MAMethod.Exponential:
                    {
                        double pr = 2d/(period + 1);

                        for (int bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                        {
                            movingAverage[bar + shift] = source[bar]*pr + movingAverage[bar + shift - 1]*(1 - pr);
                        }
                    }
                    break;
                case MAMethod.Weighted:
                    {
                        double dWeight = period*(period + 1)/2d;

                        for (int bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                        {
                            sum = 0;
                            for (int i = 0; i < period; i++)
                            {
                                sum += source[bar - i]*(period - i);
                            }

                            movingAverage[bar + shift] = sum/dWeight;
                        }
                    }
                    break;
                case MAMethod.Smoothed:
                    for (int bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                    {
                        movingAverage[bar + shift] = (movingAverage[bar + shift - 1]*(period - 1) + source[bar])/period;
                    }
                    break;
            }

            for (int bar = Bars + shift; bar < Bars; bar++)
            {
                movingAverage[bar] = 0;
            }

            return movingAverage;
        }

        /// <summary>
        ///     Maximum error for comparing indicator values
        /// </summary>
        protected double Sigma()
        {
            int sigmaMode = SeparatedChart
                                ? Configs.SigmaModeSeparatedChart
                                : Configs.SigmaModeMainChart;

            double sigma;

            switch (sigmaMode)
            {
                case 0:
                    sigma = 0;
                    break;
                case 1:
                    sigma = Data.InstrProperties.Point*0.5;
                    break;
                case 2:
                    sigma = Data.InstrProperties.Point*0.05;
                    break;
                case 3:
                    sigma = Data.InstrProperties.Point*0.005;
                    break;
                case 4:
                    sigma = 0.00005;
                    break;
                case 5:
                    sigma = 0.000005;
                    break;
                case 6:
                    sigma = 0.0000005;
                    break;
                case 7:
                    sigma = 0.00000005;
                    break;
                case 8:
                    sigma = 0.000000005;
                    break;
                default:
                    sigma = 0;
                    break;
            }

            return sigma;
        }

        /// <summary>
        ///     Calculates the logic of an Oscillator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="prvs">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="levelLong">The Level value for a Long position.</param>
        /// <param name="levelShort">The Level value for a Short position.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everything is ok.</returns>
        protected void OscillatorLogic(int firstBar, int prvs, double[] adIndValue, double levelLong, double levelShort,
                                       ref IndicatorComp indCompLong, ref IndicatorComp indCompShort,
                                       IndicatorLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int currBar = bar - prvs;
                        int baseBar = currBar - 1;
                        bool isHigher = adIndValue[currBar] > adIndValue[baseBar];

                        if (!IsDiscreteValues) // Aroon oscillator uses IsDiscreteValues = true
                        {
                            bool isNoChange = true;
                            while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange &&
                                   baseBar > firstBar)
                            {
                                isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                                baseBar--;
                            }
                        }

                        indCompLong.Value[bar] = adIndValue[baseBar] < adIndValue[currBar] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = adIndValue[baseBar] > adIndValue[currBar] + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int currBar = bar - prvs;
                        int baseBar = currBar - 1;
                        bool isHigher = adIndValue[currBar] > adIndValue[baseBar];

                        if (!IsDiscreteValues) // Aroon oscillator uses IsDiscreteValues = true
                        {
                            bool isNoChange = true;
                            while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange &&
                                   baseBar > firstBar)
                            {
                                isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                                baseBar--;
                            }
                        }

                        indCompLong.Value[bar] = adIndValue[baseBar] > adIndValue[currBar] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = adIndValue[baseBar] < adIndValue[currBar] - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = adIndValue[bar - prvs] > levelLong + sigma ? 1 : 0;
                        indCompShort.Value[bar] = adIndValue[bar - prvs] < levelShort - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = adIndValue[bar - prvs] < levelLong - sigma ? 1 : 0;
                        indCompShort.Value[bar] = adIndValue[bar - prvs] > levelShort + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - prvs - 1;
                        while (Math.Abs(adIndValue[baseBar] - levelLong) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = (adIndValue[baseBar] < levelLong - sigma &&
                                                  adIndValue[bar - prvs] > levelLong + sigma)
                                                     ? 1
                                                     : 0;
                        indCompShort.Value[bar] = (adIndValue[baseBar] > levelShort + sigma &&
                                                   adIndValue[bar - prvs] < levelShort - sigma)
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - prvs - 1;
                        while (Math.Abs(adIndValue[baseBar] - levelLong) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = (adIndValue[baseBar] > levelLong + sigma &&
                                                  adIndValue[bar - prvs] < levelLong - sigma)
                                                     ? 1
                                                     : 0;
                        indCompShort.Value[bar] = (adIndValue[baseBar] < levelShort - sigma &&
                                                   adIndValue[bar - prvs] > levelShort + sigma)
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar - prvs;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        {
                            bar1--;
                        }

                        int iBar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[iBar2]) < sigma && iBar2 > firstBar)
                        {
                            iBar2--;
                        }

                        indCompLong.Value[bar] = (adIndValue[iBar2] > adIndValue[bar1] &&
                                                  adIndValue[bar1] < adIndValue[bar0] && bar1 == bar0 - 1)
                                                     ? 1
                                                     : 0;
                        indCompShort.Value[bar] = (adIndValue[iBar2] < adIndValue[bar1] &&
                                                   adIndValue[bar1] > adIndValue[bar0] && bar1 == bar0 - 1)
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar - prvs;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        {
                            bar1--;
                        }

                        int iBar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[iBar2]) < sigma && iBar2 > firstBar)
                        {
                            iBar2--;
                        }

                        indCompLong.Value[bar] = (adIndValue[iBar2] < adIndValue[bar1] &&
                                                  adIndValue[bar1] > adIndValue[bar0] && bar1 == bar0 - 1)
                                                     ? 1
                                                     : 0;
                        indCompShort.Value[bar] = (adIndValue[iBar2] > adIndValue[bar1] &&
                                                   adIndValue[bar1] < adIndValue[bar0] && bar1 == bar0 - 1)
                                                      ? 1
                                                      : 0;
                    }
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        ///     Calculates the logic of a No Direction Oscillator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="prvs">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="dLevel">The Level value.</param>
        /// <param name="indComp">Indicator component where to save the results.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everything is ok.</returns>
        protected void NoDirectionOscillatorLogic(int firstBar, int prvs, double[] adIndValue, double dLevel,
                                                  ref IndicatorComp indComp, IndicatorLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int currBar = bar - prvs;
                        int baseBar = currBar - 1;
                        bool isHigher = adIndValue[currBar] > adIndValue[baseBar];
                        bool isNoChange = true;

                        while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange &&
                               baseBar > firstBar)
                        {
                            isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                            baseBar--;
                        }

                        indComp.Value[bar] = adIndValue[baseBar] < adIndValue[currBar] - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int currBar = bar - prvs;
                        int baseBar = currBar - 1;
                        bool isHigher = adIndValue[currBar] > adIndValue[baseBar];
                        bool isNoChange = true;

                        while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange &&
                               baseBar > firstBar)
                        {
                            isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                            baseBar--;
                        }

                        indComp.Value[bar] = adIndValue[baseBar] > adIndValue[currBar] + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indComp.Value[bar] = adIndValue[bar - prvs] > dLevel + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indComp.Value[bar] = adIndValue[bar - prvs] < dLevel - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - prvs - 1;
                        while (Math.Abs(adIndValue[baseBar] - dLevel) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indComp.Value[bar] = (adIndValue[baseBar] < dLevel - sigma &&
                                              adIndValue[bar - prvs] > dLevel + sigma)
                                                 ? 1
                                                 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - prvs - 1;
                        while (Math.Abs(adIndValue[baseBar] - dLevel) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indComp.Value[bar] = (adIndValue[baseBar] > dLevel + sigma &&
                                              adIndValue[bar - prvs] < dLevel - sigma)
                                                 ? 1
                                                 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar - prvs;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        {
                            bar1--;
                        }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[bar2]) < sigma && bar2 > firstBar)
                        {
                            bar2--;
                        }

                        indComp.Value[bar] = (adIndValue[bar2] > adIndValue[bar1] && adIndValue[bar1] < adIndValue[bar0] &&
                                              bar1 == bar0 - 1)
                                                 ? 1
                                                 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar - prvs;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        {
                            bar1--;
                        }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[bar2]) < sigma && bar2 > firstBar)
                        {
                            bar2--;
                        }

                        indComp.Value[bar] = (adIndValue[bar2] < adIndValue[bar1] && adIndValue[bar1] > adIndValue[bar0] &&
                                              bar1 == bar0 - 1)
                                                 ? 1
                                                 : 0;
                    }
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        ///     Calculates the logic of a band indicator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="prvs">To use the previous bar or not.</param>
        /// <param name="adUpperBand">The Upper band values.</param>
        /// <param name="adLowerBand">The Lower band values.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        protected void BandIndicatorLogic(int firstBar, int prvs, double[] adUpperBand, double[] adLowerBand,
                                          ref IndicatorComp indCompLong, ref IndicatorComp indCompShort,
                                          BandIndLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case BandIndLogic.The_bar_opens_below_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Open[bar] < adUpperBand[bar - prvs] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] > adLowerBand[bar - prvs] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Open[bar] > adUpperBand[bar - prvs] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] < adLowerBand[bar - prvs] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Open[bar] < adLowerBand[bar - prvs] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] > adUpperBand[bar - prvs] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Open[bar] > adLowerBand[bar - prvs] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] < adUpperBand[bar - prvs] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Upper_Band_after_opening_above_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = Open[bar] < adUpperBand[bar - prvs] - sigma &&
                                                 Open[baseBar] > adUpperBand[baseBar - prvs] + sigma
                                                     ? 1
                                                     : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompShort.Value[bar] = Open[bar] > adLowerBand[bar - prvs] + sigma &&
                                                  Open[baseBar] < adLowerBand[baseBar - prvs] - sigma
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band_after_opening_below_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = Open[bar] > adUpperBand[bar - prvs] + sigma &&
                                                 Open[baseBar] < adUpperBand[baseBar - prvs] - sigma
                                                     ? 1
                                                     : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompShort.Value[bar] = Open[bar] < adLowerBand[bar - prvs] - sigma &&
                                                  Open[baseBar] > adLowerBand[baseBar - prvs] + sigma
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band_after_opening_above_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = Open[bar] < adLowerBand[bar - prvs] - sigma &&
                                                 Open[baseBar] > adLowerBand[baseBar - prvs] + sigma
                                                     ? 1
                                                     : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompShort.Value[bar] = Open[bar] > adUpperBand[bar - prvs] + sigma &&
                                                  Open[baseBar] < adUpperBand[baseBar - prvs] - sigma
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band_after_opening_below_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompLong.Value[bar] = Open[bar] > adLowerBand[bar - prvs] + sigma &&
                                                 Open[baseBar] < adLowerBand[baseBar - prvs] - sigma
                                                     ? 1
                                                     : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - prvs]) < sigma && baseBar > firstBar)
                        {
                            baseBar--;
                        }

                        indCompShort.Value[bar] = Open[bar] < adUpperBand[bar - prvs] - sigma &&
                                                  Open[baseBar] > adUpperBand[baseBar - prvs] + sigma
                                                      ? 1
                                                      : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Close[bar] < adUpperBand[bar - prvs] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] > adLowerBand[bar - prvs] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Close[bar] > adUpperBand[bar - prvs] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] < adLowerBand[bar - prvs] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Close[bar] < adLowerBand[bar - prvs] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] > adUpperBand[bar - prvs] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar] = Close[bar] > adLowerBand[bar - prvs] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] < adUpperBand[bar - prvs] - sigma ? 1 : 0;
                    }
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "Indicator rises".
        /// </summary>
        protected void IndicatorRisesLogic(int firstBar, int prvs, double[] adIndValue, ref IndicatorComp indCompLong,
                                           ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                int baseBar = currBar - 1;
                bool isNoChange = true;
                bool isHigher = adIndValue[currBar] > adIndValue[baseBar];

                while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                {
                    isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                    baseBar--;
                }

                indCompLong.Value[bar] = adIndValue[currBar] > adIndValue[baseBar] + sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currBar] < adIndValue[baseBar] - sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "Indicator falls"
        /// </summary>
        protected void IndicatorFallsLogic(int firstBar, int prvs, double[] adIndValue, ref IndicatorComp indCompLong,
                                           ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                int baseBar = currBar - 1;
                bool isNoChange = true;
                bool isLower = adIndValue[currBar] < adIndValue[baseBar];

                while (Math.Abs(adIndValue[currBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                {
                    isNoChange = (isLower == (adIndValue[baseBar + 1] < adIndValue[baseBar]));
                    baseBar--;
                }

                indCompLong.Value[bar] = adIndValue[currBar] < adIndValue[baseBar] - sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currBar] > adIndValue[baseBar] + sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The Indicator is higher than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsHigherThanAnotherIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                                  double[] adAnotherIndValue,
                                                                  ref IndicatorComp indCompLong,
                                                                  ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                indCompLong.Value[bar] = adIndValue[currBar] > adAnotherIndValue[currBar] + sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currBar] < adAnotherIndValue[currBar] - sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The Indicator is lower than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsLowerThanAnotherIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                                 double[] adAnotherIndValue,
                                                                 ref IndicatorComp indCompLong,
                                                                 ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                indCompLong.Value[bar] = adIndValue[currBar] < adAnotherIndValue[currBar] - sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currBar] > adAnotherIndValue[currBar] + sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The Indicator crosses AnotherIndicator upward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorUpwardLogic(int firstBar, int prvs, double[] adIndValue,
                                                                   double[] adAnotherIndValue,
                                                                   ref IndicatorComp indCompLong,
                                                                   ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                int baseBar = currBar - 1;
                while (Math.Abs(adIndValue[baseBar] - adAnotherIndValue[baseBar]) < sigma && baseBar > firstBar)
                {
                    baseBar--;
                }

                indCompLong.Value[bar] = adIndValue[currBar] > adAnotherIndValue[currBar] + sigma &&
                                         adIndValue[baseBar] < adAnotherIndValue[baseBar] - sigma
                                             ? 1
                                             : 0;
                indCompShort.Value[bar] = adIndValue[currBar] < adAnotherIndValue[currBar] - sigma &&
                                          adIndValue[baseBar] > adAnotherIndValue[baseBar] + sigma
                                              ? 1
                                              : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The Indicator crosses AnotherIndicator downward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorDownwardLogic(int firstBar, int prvs, double[] adIndValue,
                                                                     double[] adAnotherIndValue,
                                                                     ref IndicatorComp indCompLong,
                                                                     ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currBar = bar - prvs;
                int baseBar = currBar - 1;
                while (Math.Abs(adIndValue[baseBar] - adAnotherIndValue[baseBar]) < sigma && baseBar > firstBar)
                {
                    baseBar--;
                }

                indCompLong.Value[bar] = adIndValue[currBar] < adAnotherIndValue[currBar] - sigma &&
                                         adIndValue[baseBar] > adAnotherIndValue[baseBar] + sigma
                                             ? 1
                                             : 0;
                indCompShort.Value[bar] = adIndValue[currBar] > adAnotherIndValue[currBar] + sigma &&
                                          adIndValue[baseBar] < adAnotherIndValue[baseBar] - sigma
                                              ? 1
                                              : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar opens above the Indicator"
        /// </summary>
        protected void BarOpensAboveIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                   ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar] = Open[bar] > adIndValue[bar - prvs] + sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] < adIndValue[bar - prvs] - sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar opens below the Indicator"
        /// </summary>
        protected void BarOpensBelowIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                   ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar] = Open[bar] < adIndValue[bar - prvs] - sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] > adIndValue[bar - prvs] + sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar opens above the Indicator after opening below it"
        /// </summary>
        protected void BarOpensAboveIndicatorAfterOpeningBelowLogic(int firstBar, int prvs, double[] adIndValue,
                                                                    ref IndicatorComp indCompLong,
                                                                    ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int baseBar = bar - 1;
                while (Math.Abs(Open[baseBar] - adIndValue[baseBar - prvs]) < sigma && baseBar > firstBar)
                {
                    baseBar--;
                }

                indCompLong.Value[bar] = Open[bar] > adIndValue[bar - prvs] + sigma &&
                                         Open[baseBar] < adIndValue[baseBar - prvs] - sigma
                                             ? 1
                                             : 0;
                indCompShort.Value[bar] = Open[bar] < adIndValue[bar - prvs] - sigma &&
                                          Open[baseBar] > adIndValue[baseBar - prvs] + sigma
                                              ? 1
                                              : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar opens below the Indicator after opening above it"
        /// </summary>
        protected void BarOpensBelowIndicatorAfterOpeningAboveLogic(int firstBar, int prvs, double[] adIndValue,
                                                                    ref IndicatorComp indCompLong,
                                                                    ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int baseBar = bar - 1;
                while (Math.Abs(Open[baseBar] - adIndValue[baseBar - prvs]) < sigma && baseBar > firstBar)
                {
                    baseBar--;
                }

                indCompLong.Value[bar] = Open[bar] < adIndValue[bar - prvs] - sigma &&
                                         Open[baseBar] > adIndValue[baseBar - prvs] + sigma
                                             ? 1
                                             : 0;
                indCompShort.Value[bar] = Open[bar] > adIndValue[bar - prvs] + sigma &&
                                          Open[baseBar] < adIndValue[baseBar - prvs] - sigma
                                              ? 1
                                              : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar closes above the Indicator"
        /// </summary>
        protected void BarClosesAboveIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                    ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar] = Close[bar] > adIndValue[bar - prvs] + sigma ? 1 : 0;
                indCompShort.Value[bar] = Close[bar] < adIndValue[bar - prvs] - sigma ? 1 : 0;
            }
        }

        /// <summary>
        ///     Returns signals for the logic rule "The bar closes below the Indicator"
        /// </summary>
        protected void BarClosesBelowIndicatorLogic(int firstBar, int prvs, double[] adIndValue,
                                                    ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar] = Close[bar] < adIndValue[bar - prvs] - sigma ? 1 : 0;
                indCompShort.Value[bar] = Close[bar] > adIndValue[bar - prvs] + sigma ? 1 : 0;
            }
        }
    }
}
