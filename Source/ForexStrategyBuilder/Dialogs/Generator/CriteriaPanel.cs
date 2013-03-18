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
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    /// <summary>
    ///     Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        private Panel pnlCriteriaControls;

        private CheckBox chbAmbiguousBars;
        private CheckBox chbMaxDrawdown;
        private CheckBox chbEquityPercent;
        private CheckBox chbMaxTrades;
        private CheckBox chbMinTrades;
        private CheckBox chbWinLossRatio;
        private CheckBox chbOOSPatternFilter;
        private CheckBox chbSmoothBalanceLines;
        private CheckBox chbMinSharpeRatio;
        private CheckBox chbMinProfitPerDay;
        private CheckBox chbMaxRedGreenDeviation;


        private NumericUpDown nudAmbiguousBars;
        private NumericUpDown nudEquityPercent;
        private NumericUpDown nudMaxClosingLogicSlots;
        private NumericUpDown nudMaxDrawdown;
        private NumericUpDown nudMaxTrades;
        private NumericUpDown nudMinTrades;
        private NumericUpDown nudSmoothBalanceCheckPoints;
        private NumericUpDown nudSmoothBalancePercent;
        private NumericUpDown nudWinLossRatio;
        private NumericUpDown nudoosPatternPercent;
        private NumericUpDown nudMinSharpeRatio;
        private NumericUpDown nudMinProfitPerDay;
        private NumericUpDown nudMaxRedGreenDeviation;

        /// <summary>
        ///     Sets controls in Criteria panel
        /// </summary>
        private void SetCriteriaPanel()
        {
            pnlCriteriaControls = new Panel
                {
                    Height = 400,
                    BackColor = LayoutColors.ColorControlBack,
                    ForeColor = LayoutColors.ColorControlText,
                    Margin = new Padding(0)
                };
            criteriaPanel.Parent = pnlCriteriaBase;
            criteriaPanel.Padding = new Padding(0);
            criteriaPanel.Margin = new Padding(0);
            criteriaPanel.ClearControls();
            criteriaPanel.AddControl(pnlCriteriaControls);
            criteriaPanel.SetControls();

            chbAmbiguousBars = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of ambiguous bars"),
                    Checked = true,
                    AutoSize = true
                };

            nudAmbiguousBars = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudAmbiguousBars.BeginInit();
            nudAmbiguousBars.Minimum = 0;
            nudAmbiguousBars.Maximum = 100;
            nudAmbiguousBars.Increment = 1;
            nudAmbiguousBars.Value = 10;
            nudAmbiguousBars.EndInit();

            chbMinProfitPerDay = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum Profit Per Day") + " [" + Configs.AccountCurrency + "]",
                    Checked = true,
                    AutoSize = true
                };

            nudMinProfitPerDay = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudMinProfitPerDay.BeginInit();
            nudMinProfitPerDay.Minimum = 1;
            nudMinProfitPerDay.Maximum = 500;
            nudMinProfitPerDay.Increment =1;
            nudMinProfitPerDay.DecimalPlaces = 1;
            nudMinProfitPerDay.Value = 1;
            nudMinProfitPerDay.EndInit();

            chbMaxDrawdown = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum equity drawdown") + " [" +
                           (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxDrawdown = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudMaxDrawdown.BeginInit();
            nudMaxDrawdown.Minimum = 0;
            nudMaxDrawdown.Maximum = Configs.InitialAccount;
            nudMaxDrawdown.Increment = 10;
            nudMaxDrawdown.Value = (decimal) Math.Round(Configs.InitialAccount/4.0);
            nudMaxDrawdown.EndInit();

            chbEquityPercent = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text =
                        Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency +
                        "]",
                    Checked = true,
                    AutoSize = true
                };

            nudEquityPercent = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudEquityPercent.BeginInit();
            nudEquityPercent.Minimum = 1;
            nudEquityPercent.Maximum = 100;
            nudEquityPercent.Increment = 1;
            nudEquityPercent.Value = 25;
            nudEquityPercent.EndInit();

            chbMinTrades = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum number of trades"),
                    Checked = true,
                    AutoSize = true
                };

            nudMinTrades = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudMinTrades.BeginInit();
            nudMinTrades.Minimum = 10;
            nudMinTrades.Maximum = 1000;
            nudMinTrades.Increment = 10;
            nudMinTrades.Value = 100;
            nudMinTrades.EndInit();

            chbMaxTrades = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of trades"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxTrades = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudMaxTrades.BeginInit();
            nudMaxTrades.Minimum = 10;
            nudMaxTrades.Maximum = 10000;
            nudMaxTrades.Increment = 10;
            nudMaxTrades.Value = 1000;
            nudMaxTrades.EndInit();

            chbWinLossRatio = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum win / loss trades ratio"),
                    Checked = false,
                    AutoSize = true
                };

            nudWinLossRatio = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudWinLossRatio.BeginInit();
            nudWinLossRatio.Minimum = 0.10M;
            nudWinLossRatio.Maximum = 1;
            nudWinLossRatio.Increment = 0.01M;
            nudWinLossRatio.Value = 0.30M;
            nudWinLossRatio.DecimalPlaces = 2;
            nudWinLossRatio.EndInit();

            chbMinSharpeRatio = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum Sharpe ratio"),
                    Checked = false,
                    AutoSize = true
                };

            nudMinSharpeRatio = new NumericUpDown {Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center};
            nudMinSharpeRatio.BeginInit();
            nudMinSharpeRatio.Minimum = 0.1M;
            nudMinSharpeRatio.Maximum = 10.0M;
            nudMinSharpeRatio.Increment = 0.1M;
            nudMinSharpeRatio.Value = 0.2M;
            nudMinSharpeRatio.DecimalPlaces = 1;
            nudMinSharpeRatio.EndInit();

            chbMaxRedGreenDeviation = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Max long/short balance deviation"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxRedGreenDeviation = new NumericUpDown { Parent = pnlCriteriaControls, TextAlign = HorizontalAlignment.Center };
            nudMaxRedGreenDeviation.BeginInit();
            nudMaxRedGreenDeviation.Minimum = 1;
            nudMaxRedGreenDeviation.Maximum = 100;
            nudMaxRedGreenDeviation.Value = 40;
            nudMaxRedGreenDeviation.EndInit();
            toolTip.SetToolTip(nudMaxRedGreenDeviation, Language.T("Deviation percent."));

            chbOOSPatternFilter = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Filter bad OOS performance"),
                    Checked = false,
                    AutoSize = true
                };

            nudoosPatternPercent = new NumericUpDown
                {
                    Parent = pnlCriteriaControls,
                    TextAlign = HorizontalAlignment.Center
                };
            nudoosPatternPercent.BeginInit();
            nudoosPatternPercent.Minimum = 1;
            nudoosPatternPercent.Maximum = 50;
            nudoosPatternPercent.Value = 20;
            nudoosPatternPercent.EndInit();
            toolTip.SetToolTip(nudoosPatternPercent, Language.T("Deviation percent."));

            chbSmoothBalanceLines = new CheckBox
                {
                    Parent = pnlCriteriaControls,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Filter non-linear balance pattern"),
                    Checked = false,
                    AutoSize = true
                };

            nudSmoothBalancePercent = new NumericUpDown
                {
                    Parent = pnlCriteriaControls,
                    TextAlign = HorizontalAlignment.Center
                };
            nudSmoothBalancePercent.BeginInit();
            nudSmoothBalancePercent.Minimum = 1;
            nudSmoothBalancePercent.Maximum = 50;
            nudSmoothBalancePercent.Value = 20;
            nudSmoothBalancePercent.EndInit();
            toolTip.SetToolTip(nudSmoothBalancePercent, Language.T("Deviation percent."));

            nudSmoothBalanceCheckPoints = new NumericUpDown
                {
                    Parent = pnlCriteriaControls,
                    TextAlign = HorizontalAlignment.Center
                };
            nudSmoothBalanceCheckPoints.BeginInit();
            nudSmoothBalanceCheckPoints.Minimum = 1;
            nudSmoothBalanceCheckPoints.Maximum = 50;
            nudSmoothBalanceCheckPoints.Value = 1;
            nudSmoothBalanceCheckPoints.EndInit();
            toolTip.SetToolTip(nudSmoothBalanceCheckPoints, Language.T("Check points count."));

            pnlCriteriaControls.Resize += pnlCriteriaControls_Resize;
        }

        private void pnlCriteriaControls_Resize(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int border = btnHrzSpace;
            const int nudWidth = 55;

            // chbAmbiguousBars
            chbAmbiguousBars.Location = new Point(border + 2, 6);

            // nudAmbiguousBars
            nudAmbiguousBars.Width = nudWidth;
            nudAmbiguousBars.Location = new Point(panel.ClientSize.Width - nudWidth - border - 2,
                                                  chbAmbiguousBars.Top - 1);

            // Min Profit Per Day
            chbMinProfitPerDay.Location = new Point(border + 2, chbAmbiguousBars.Bottom + border + 4);
            nudMinProfitPerDay.Width = nudWidth;
            nudMinProfitPerDay.Location = new Point(nudAmbiguousBars.Left, chbMinProfitPerDay.Top - 1);

            // MaxDrawdown
            chbMaxDrawdown.Location = new Point(border + 2, chbMinProfitPerDay.Bottom + border + 4);
            nudMaxDrawdown.Width = nudWidth;
            nudMaxDrawdown.Location = new Point(nudAmbiguousBars.Left, chbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            chbEquityPercent.Location = new Point(border + 2, nudMaxDrawdown.Bottom + border + 4);
            nudEquityPercent.Width = nudWidth;
            nudEquityPercent.Location = new Point(nudAmbiguousBars.Left, chbEquityPercent.Top - 1);

            // MinTrades
            chbMinTrades.Location = new Point(border + 2, chbEquityPercent.Bottom + border + 4);
            nudMinTrades.Width = nudWidth;
            nudMinTrades.Location = new Point(nudAmbiguousBars.Left, chbMinTrades.Top - 1);

            // MaxTrades
            chbMaxTrades.Location = new Point(border + 2, chbMinTrades.Bottom + border + 4);
            nudMaxTrades.Width = nudWidth;
            nudMaxTrades.Location = new Point(nudAmbiguousBars.Left, chbMaxTrades.Top - 1);

            // Win/Loss Ratios
            chbWinLossRatio.Location = new Point(border + 2, chbMaxTrades.Bottom + border + 4);
            nudWinLossRatio.Width = nudWidth;
            nudWinLossRatio.Location = new Point(nudAmbiguousBars.Left, chbWinLossRatio.Top - 1);

            // Sharpe Ratios
            chbMinSharpeRatio.Location = new Point(border + 2, chbWinLossRatio.Bottom + border + 4);
            nudMinSharpeRatio.Width = nudWidth;
            nudMinSharpeRatio.Location = new Point(nudAmbiguousBars.Left, chbMinSharpeRatio.Top - 1);

            // Red/Green Deviation
            chbMaxRedGreenDeviation.Location = new Point(border + 2, chbMinSharpeRatio.Bottom + border + 4);
            nudMaxRedGreenDeviation.Width = nudWidth;
            nudMaxRedGreenDeviation.Location = new Point(nudAmbiguousBars.Left, chbMaxRedGreenDeviation.Top - 1);

            // OOS Pattern Filter
            chbOOSPatternFilter.Location = new Point(border + 2, chbMaxRedGreenDeviation.Bottom + border + 4);
            nudoosPatternPercent.Width = nudWidth;
            nudoosPatternPercent.Location = new Point(nudAmbiguousBars.Left, chbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            chbSmoothBalanceLines.Location = new Point(border + 2, chbOOSPatternFilter.Bottom + border + 4);
            nudSmoothBalancePercent.Width = nudWidth;
            nudSmoothBalancePercent.Location = new Point(nudAmbiguousBars.Left, chbSmoothBalanceLines.Top - 1);
            nudSmoothBalanceCheckPoints.Width = nudWidth;
            nudSmoothBalanceCheckPoints.Location = new Point(nudSmoothBalancePercent.Left - nudWidth - border,
                                                             chbSmoothBalanceLines.Top - 1);
        }

        /// <summary>
        ///     Check the strategy criteria
        /// </summary>
        private bool IsCriteriaFulfilled()
        {
            // The calculated strategy has higher profit
            // or the same profit but lower number of slots
            Backtester.CalculateAccountStats();

            // Criterion Max Ambiguous Bars
            if (chbAmbiguousBars.Checked && Backtester.AmbiguousBars > nudAmbiguousBars.Value)
            {
                customAnalytics.CriterionAmbiguousBars++;
                return false;
            }

            // Criterion Min Profit per Day
            if (chbMinProfitPerDay.Checked && Backtester.MoneyProfitPerDay < (double) nudMinProfitPerDay.Value)
            {
                customAnalytics.CriterionProfitPerDay++;
                return false;
            }

            // Criterion Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney
                                           ? Backtester.MaxMoneyEquityDrawdown
                                           : Backtester.MaxEquityDrawdown;
            if (chbMaxDrawdown.Checked && maxEquityDrawdown > (double) nudMaxDrawdown.Value)
            {
                customAnalytics.CriterionMaxEquityDD++;
                return false;
            }

            // Criterion Max Equity percent drawdown
            if (chbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double) nudEquityPercent.Value)
            {
                customAnalytics.CriterionMaxEquityPercentDD++;
                return false;
            }

            // Criterion Min Trades
            if (chbMinTrades.Checked && Backtester.ExecutedOrders < nudMinTrades.Value)
            {
                customAnalytics.CriterionMinTrades++;
                return false;
            }

            // Criterion Max Trades
            if (chbMaxTrades.Checked && Backtester.ExecutedOrders > nudMaxTrades.Value)
            {
                customAnalytics.CriterionMaxTrades++;
                return false;
            }

            // Criterion Win / Loss ratio
            if (chbWinLossRatio.Checked && Backtester.WinLossRatio < (double) nudWinLossRatio.Value)
            {
                customAnalytics.CriterionWinLossRatio++;
                return false;
            }

            // Criterion Minimum Sharpe ratio
            if (chbMinSharpeRatio.Checked && Backtester.SharpeRatio < (double) nudMinSharpeRatio.Value)
            {
                customAnalytics.CriterionSharpeRatio++;
                return false;
            }

            // Criterion Red/Green Deviation
            if (chbMaxRedGreenDeviation.Checked)
            {
                var maxDev = (double) nudMaxRedGreenDeviation.Value;
                var delta = 100*Math.Abs(Backtester.NetLongMoneyBalance - Backtester.NetShortMoneyBalance)/
                            Backtester.NetMoneyBalance;

                if (delta > maxDev)
                {
                    customAnalytics.CriterionSharpeRatio++;
                    return false;
                }
            }

            // OOS Pattern filter
            if (chbOOSPatternFilter.Checked && chbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int oosBalance = Backtester.Balance(barOOS);
                var targetBalance = (int) (oosBalance*targetBalanceRatio);
                var minBalance = (int) (targetBalance*(1 - nudoosPatternPercent.Value/100));
                if (netBalance < oosBalance || netBalance < minBalance)
                {
                    customAnalytics.CriterionOOSPatternFilter++;
                    return false;
                }
            }

            // Smooth Balance Line
            if (chbSmoothBalanceLines.Checked)
            {
                var checkPoints = (int) nudSmoothBalanceCheckPoints.Value;
                var maxPercentDeviation = (double) (nudSmoothBalancePercent.Value/100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Data.FirstBar;
                    int bar = Data.FirstBar + i*(Data.Bars - firstBar)/(checkPoints + 1);
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    double minBalance = targetBalance*(1 - maxPercentDeviation);
                    double maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        customAnalytics.CriterionSmoothBalanceLine++;
                        return false;
                    }

                    // Long balance line
                    netBalance = Backtester.NetLongMoneyBalance;
                    checkPointBalance = Backtester.LongMoneyBalance(bar);
                    startBalance = Backtester.LongMoneyBalance(firstBar);
                    targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    minBalance = targetBalance*(1 - maxPercentDeviation);
                    maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        customAnalytics.CriterionSmoothBalanceLineLong++;
                        return false;
                    }

                    // Short balance line
                    netBalance = Backtester.NetShortMoneyBalance;
                    checkPointBalance = Backtester.ShortMoneyBalance(bar);
                    startBalance = Backtester.ShortMoneyBalance(firstBar);
                    targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    minBalance = targetBalance*(1 - maxPercentDeviation);
                    maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        customAnalytics.CriterionSmoothBalanceLineShort++;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}