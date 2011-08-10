// Micro_Balance_Chart_Image class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Forex_Strategy_Builder
{
    public class Micro_Balance_Chart_Image
    {

        Bitmap chart;
        public Bitmap Chart { get { return chart; } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="width">Chart Width</param>
        /// <param name="height">Chart Height</param>
        public Micro_Balance_Chart_Image(int width, int height)
        {
            InitChart(width, height);
        }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        void InitChart(int width, int height)
        {
            chart = new Bitmap(width, height);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            int border = 1;
            int space  = 2;

            int XLeft, XRight, YTop, YBottom;
            float XScale, YScale;

            int bars;
            int chartBars;
            int maximum;
            int minimum;
            int firstBar;
            Pen penBorder;
            PointF[] apntBalance;
            PointF[] apntEquity;
            PointF[] apntLongBalance;
            PointF[] apntShortBalance;

            firstBar = Data.FirstBar;
            bars = Data.Bars;
            chartBars = Data.Bars - firstBar;
            int maxBalance = Configs.AccountInMoney ? (int)Backtester.MaxMoneyBalance : Backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int)Backtester.MinMoneyBalance : Backtester.MinBalance;
            int maxEquity  = Configs.AccountInMoney ? (int)Backtester.MaxMoneyEquity : Backtester.MaxEquity;
            int minEquity  = Configs.AccountInMoney ? (int)Backtester.MinMoneyEquity : Backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance  = Configs.AccountInMoney ? (int)Backtester.MaxLongMoneyBalance : Backtester.MaxLongBalance;
                int minLongBalance  = Configs.AccountInMoney ? (int)Backtester.MinLongMoneyBalance : Backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney ? (int)Backtester.MaxShortMoneyBalance : Backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney ? (int)Backtester.MinShortMoneyBalance : Backtester.MinShortBalance;
                int maxLSBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLSBalance = Math.Min(minLongBalance, minShortBalance);

                maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLSBalance) + 1;
                minimum = Math.Min(Math.Min(minBalance, minEquity), minLSBalance) - 1;
            }
            else
            {
                maximum = Math.Max(maxBalance, maxEquity) + 1;
                minimum = Math.Min(minBalance, minEquity) - 1;
            }

            YTop = border + space;
            YBottom = height - border - space;
            XLeft  = border;
            XRight = width - border - space;
            XScale = (XRight - XLeft) / (float)chartBars;
            YScale = (YBottom - YTop) / (float)(maximum - minimum);

            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            apntBalance = new PointF[chartBars];
            apntEquity  = new PointF[chartBars];
            apntLongBalance = new PointF[chartBars];
            apntShortBalance = new PointF[chartBars];

            int index = 0;
            for (int bar = firstBar; bar < bars; bar++)
            {
                apntBalance[index].X = XLeft + index * XScale;
                apntEquity[index].X  = XLeft + index * XScale;
                if (Configs.AccountInMoney)
                {
                    apntBalance[index].Y = (float)(YBottom - (Backtester.MoneyBalance(bar) - minimum) * YScale);
                    apntEquity[index].Y  = (float)(YBottom - (Backtester.MoneyEquity(bar) - minimum) * YScale);
                }
                else
                {
                    apntBalance[index].Y = YBottom - (Backtester.Balance(bar) - minimum) * YScale;
                    apntEquity[index].Y  = YBottom - (Backtester.Equity(bar) - minimum) * YScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    apntLongBalance[index].X = XLeft + index * XScale;
                    apntShortBalance[index].X = XLeft + index * XScale;
                    if (Configs.AccountInMoney)
                    {
                        apntLongBalance[index].Y = (float)(YBottom - (Backtester.LongMoneyBalance(bar) - minimum) * YScale);
                        apntShortBalance[index].Y = (float)(YBottom - (Backtester.ShortMoneyBalance(bar) - minimum) * YScale);
                    }
                    else
                    {
                        apntLongBalance[index].Y = YBottom - (Backtester.LongBalance(bar) - minimum) * YScale;
                        apntShortBalance[index].Y = YBottom - (Backtester.ShortBalance(bar) - minimum) * YScale;
                    }
                }

                index++;
            }

            Graphics g = Graphics.FromImage(chart);

            // Paints the background by gradient
            RectangleF rectField = new RectangleF(1, 1, width - 2, height - 2);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorChartBack), rectField);

            // Border
            g.DrawRectangle(penBorder, 0, 0, width - 1, height - 1);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red), apntShortBalance);
                g.DrawLines(new Pen(Color.Green), apntLongBalance);
            }

            // Draw the balance line
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);
        }
    }
}
