// Strategy Optimizer - Math
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer - Math
    /// </summary>
    public partial class Optimizer : Form
    {
        /// <summary>
        /// Sets the parameters.
        /// </summary>
        void SetIndicatorParams()
        {
            protections = 0;
            parameters  = 0;

            CountParameters();
            CountPermanentProtections();
            CreateControls();
            CreateProtectionParameters();

            for (int prot = 0; prot < protections; prot++)
            {
                SetParametersValues(prot);
            }

            int param = protections;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                for (int numParam = 0; numParam < 6; numParam++)
                {
                    if (!Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled) continue;
                    aParameter[param] = new Parameter(OptimizerParameterType.Indicator, slot, numParam);
                    SetParametersValues(param);
                    param++;
                }
            }

            int totalHeight = ArrangeControls();

            if (parameters == 0)
            {
                pnlParams.Height = 50;
                lblNoParams.Visible = true;
            }
            else
            {
                pnlParams.Height = totalHeight;
            }

            btnOptimize.Focus();

            return;
        }

        /// <summary>
        /// Counts the strategy's numeric parameters and Indicators.
        /// </summary>
        private void CountParameters()
        {
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                    {
                        parameters++;
                    }
        }

        /// <summary>
        /// Counts Permanent SL, TP and Breakeven
        /// </summary>
        private void CountPermanentProtections()
        {
            if (Data.Strategy.UsePermanentSL)
            {
                parameters++;
                protections++;
            }
            if (Data.Strategy.UsePermanentTP)
            {
                parameters++;
                protections++;
            }
            if (Data.Strategy.UseBreakEven)
            {
                parameters++;
                protections++;
            }
        }

        /// <summary>
        /// Creates control arrays.
        /// </summary>
        private void CreateControls()
        {
            aParameter         = new Parameter[parameters];
            achbxParameterName = new CheckBox[parameters];
            alblParameterValue = new Label[parameters];
            anudParameterMin   = new NumericUpDown[parameters];
            anudParameterMax   = new NumericUpDown[parameters];
            anudParameterStep  = new NumericUpDown[parameters];
            alblIndicatorName  = new Label[parameters];

            for (int param = 0; param < parameters; param++)
            {
                alblIndicatorName[param]  = new Label();
                achbxParameterName[param] = new CheckBox();
                alblParameterValue[param] = new Label();
                anudParameterMin[param]   = new NumericUpDown();
                anudParameterMax[param]   = new NumericUpDown();
                anudParameterStep[param]  = new NumericUpDown();
            }
        }

        /// <summary>
        /// Initializes protection parameter.
        /// </summary>
        private void CreateProtectionParameters()
        {
            int par = 0;
            if (Data.Strategy.UsePermanentSL)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.PermanentSL, -1, 0);
                par++;
            }
            if (Data.Strategy.UsePermanentTP)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.PermanentTP, -1, 0);
                par++;
            }
            if (Data.Strategy.UseBreakEven)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.BreakEven, -1, 0);
            }
        }

        /// <summary>
        /// Sets values of optimizer controls.
        /// </summary>
        private void SetParametersValues(int param)
        {
            alblIndicatorName[param].Parent    = pnlParams;
            alblIndicatorName[param].Text      = aParameter[param].GroupName;
            alblIndicatorName[param].Font      = fontIndicator;
            alblIndicatorName[param].ForeColor = LayoutColors.ColorSlotIndicatorText;
            alblIndicatorName[param].BackColor = Color.Transparent;
            alblIndicatorName[param].Height    = 18;
            alblIndicatorName[param].Width     = 200;

            achbxParameterName[param].ForeColor = colorText;
            achbxParameterName[param].BackColor = Color.Transparent;
            achbxParameterName[param].Text      = aParameter[param].ParameterName;
            achbxParameterName[param].CheckedChanged += new EventHandler(Optimizer_CheckedChanged);
            achbxParameterName[param].Parent    = pnlParams;
            achbxParameterName[param].Width     = 140;
            achbxParameterName[param].TextAlign = ContentAlignment.MiddleLeft;

            int    point = aParameter[param].Point;
            double value = aParameter[param].Value;
            double min   = aParameter[param].Minimum;
            double max   = aParameter[param].Maximum;
            double step  = aParameter[param].Step;
            string stringFormat = "{0:F" + point.ToString() + "}";

            alblParameterValue[param].ForeColor = colorText;
            alblParameterValue[param].BackColor = Color.Transparent;
            alblParameterValue[param].Text      = string.Format(stringFormat, value);
            alblParameterValue[param].Parent    = pnlParams;
            alblParameterValue[param].Width     = 55;
            alblParameterValue[param].TextAlign = ContentAlignment.MiddleCenter;

            fontParamValueRegular = alblParameterValue[param].Font;
            fontParamValueBold = new Font(fontParamValueRegular, FontStyle.Bold);

            anudParameterMin[param].BeginInit();
            anudParameterMin[param].Minimum = (decimal) min;
            anudParameterMin[param].Maximum = (decimal) max;
            anudParameterMin[param].Value   = (decimal) Math.Round(Math.Max(value - 5*step, min), point);
            anudParameterMin[param].DecimalPlaces = point;
            anudParameterMin[param].Increment = (decimal) step;
            anudParameterMin[param].EndInit();
            anudParameterMin[param].Parent    = pnlParams;
            anudParameterMin[param].Width     = 70;
            anudParameterMin[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterMin[param], Language.T("Minimum value."));

            anudParameterMax[param].BeginInit();
            anudParameterMax[param].Minimum = (decimal) min;
            anudParameterMax[param].Maximum = (decimal) max;
            anudParameterMax[param].Value   = (decimal) Math.Round(Math.Min(value + 5*step, max), point);
            anudParameterMax[param].DecimalPlaces = point;
            anudParameterMax[param].Increment = (decimal) step;
            anudParameterMax[param].EndInit();
            anudParameterMax[param].Parent    = pnlParams;
            anudParameterMax[param].Width     = 70;
            anudParameterMax[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterMax[param], Language.T("Maximum value."));

            anudParameterStep[param].BeginInit();
            anudParameterStep[param].Minimum = (decimal) step;
            anudParameterStep[param].Maximum = (decimal) Math.Max(step, Math.Abs(max - min));
            anudParameterStep[param].Value   = (decimal) step;
            anudParameterStep[param].DecimalPlaces = point;
            anudParameterStep[param].Increment = (decimal) step;
            anudParameterStep[param].EndInit();
            anudParameterStep[param].Parent    = pnlParams;
            anudParameterStep[param].Width     = 70;
            anudParameterStep[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterStep[param], Language.T("Step of change."));
        }

        /// <summary>
        /// Arranges the controls
        /// </summary>
        int ArrangeControls()
        {
            const int vertMargin  = 3;
            const int horizMargin = 5;
            int vertPosition = vertMargin - scrollBar.Value;
            int totalHeight  = vertMargin;

            int slotNumber = int.MinValue;
            for (int param = 0; param < parameters; param++)
            {
                if (aParameter[param].SlotNumber != slotNumber)
                {
                    slotNumber = aParameter[param].SlotNumber;
                    alblIndicatorName[param].Location = new Point(horizMargin, vertPosition);
                    vertPosition += alblIndicatorName[param].Height + vertMargin;
                    totalHeight  += alblIndicatorName[param].Height + vertMargin;
                }
                else
                {
                    alblIndicatorName[param].Visible = false;
                }
                achbxParameterName[param].Location = new Point(horizMargin, vertPosition);
                alblParameterValue[param].Location = new Point(achbxParameterName[param].Right + horizMargin, vertPosition - 1);
                anudParameterMin[param].Location   = new Point(alblParameterValue[param].Right + horizMargin, vertPosition + 2);
                anudParameterMax[param].Location   = new Point(anudParameterMin[param].Right   + horizMargin, vertPosition + 2);
                anudParameterStep[param].Location  = new Point(anudParameterMax[param].Right   + horizMargin, vertPosition + 2);
                vertPosition += achbxParameterName[param].Height + vertMargin;
                totalHeight  += achbxParameterName[param].Height + vertMargin;
            }

            return totalHeight;
        }

        /// <summary>
        /// Sets parameters' Min / Max values
        /// </summary>
        void SetParamsMinMax(int deltaStep)
        {
            for (int param = 0; param < parameters; param++)
            {
                int    point = aParameter[param].Point;
                double value = aParameter[param].Value;
                double min   = aParameter[param].Minimum;
                double max   = aParameter[param].Maximum;
                double step  = aParameter[param].Step;

                anudParameterMin[param].Value = (decimal)Math.Round(Math.Max(value - deltaStep * step, min), point);
                anudParameterMax[param].Value = (decimal)Math.Round(Math.Min(value + deltaStep * step, max), point);
            }
        }

        /// <summary>
        /// Select / unselect parameters
        /// </summary>
        void SelectParameters(OptimizerButtons button)
        {
            for (int param = 0; param < parameters; param++)
            {
                if (button == OptimizerButtons.SelectAll)
                    achbxParameterName[param].Checked = true;
                else if (button == OptimizerButtons.SelectNone)
                    achbxParameterName[param].Checked = false;
                else if (button == OptimizerButtons.SelectRandom)
                    SelectRandomParameters();
            }
        }

        /// <summary>
        /// Selects parameters randomly.
        /// </summary>
        void SelectRandomParameters()
        {
            for (int param = 0; param < protections; param++)
            {
                achbxParameterName[param].Checked = random.Next(100) > 40;
            }

            int checkedChBoxes = 0;
            for (int param = protections; param < parameters; param++)
            {
                int slot = aParameter[param].SlotNumber;
                int numParam = aParameter[param].NumParam;

                achbxParameterName[param].Checked = false;

                if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption == "Level")
                {
                    if (Data.Strategy.Slot[slot].IndParam.ListParam[0].Text.Contains("Level"))
                    {
                        if (random.Next(100) > 20 && checkedChBoxes < 6)
                        {
                            achbxParameterName[param].Checked = true;
                            checkedChBoxes++;
                        }
                    }
                    else
                    {
                        achbxParameterName[param].Checked = false;
                    }
                }
                else
                {
                    if (random.Next(100) > 40 && checkedChBoxes < 6)
                    {
                        achbxParameterName[param].Checked = true;
                        checkedChBoxes++;
                    }
                }
            }
        }

        /// <summary>
        /// Optimize.
        /// </summary>
        void BtnOptimize_Click(object sender, EventArgs e)
        {
            if (isOptimizing)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            if (chbOptimizerWritesReport.Checked)
                InitReport();

            // Counts the checked parameters
            checkedParams = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    checkedParams++;

            // If there are no checked returns
            if (checkedParams < 1)
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            // Contains the checked parameters only
            aiChecked = new int[checkedParams];
            int indexChecked = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    aiChecked[indexChecked++] = i;

            SetNecessaryCycles();

            Cursor            = Cursors.WaitCursor;
            progressBar.Value = 1;
            progressPercent   = 0;
            computedCycles    = 0;
            isOptimizing      = true;
            btnCancel.Enabled = false;
            btnAccept.Enabled = false;
            btnOptimize.Text  = Language.T("Stop");

            for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                aOptimizerButtons[i].Enabled = false;

            foreach (Control control in pnlParams.Controls)
                if (control.GetType() != typeof(Label))
                    control.Enabled = false;

            foreach (Control control in pnlLimitations.Controls)
                control.Enabled = false;

            foreach (Control control in pnlSettings.Controls)
                control.Enabled = false;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();

            return;
        }

        /// <summary>
        /// Counts the necessary optimization cycles.
        /// </summary>
        void SetNecessaryCycles()
        {
            cycles = 0;
            for (int i = 0; i < checkedParams; i++)
            {
                double min  = (double)anudParameterMin[aiChecked[i]].Value;
                double max  = (double)anudParameterMax[aiChecked[i]].Value;
                double step = (double)anudParameterStep[aiChecked[i]].Value;

                for (double value = min; value <= max; value += step)
                    cycles += 1;
            }

            for (int i = 0; i < checkedParams - 1; i++)
            {
                for (int j = 0; j < checkedParams; j++)
                {
                    if (i >= j) continue;

                    double min1  = (double)anudParameterMin[aiChecked[i]].Value;
                    double max1  = (double)anudParameterMax[aiChecked[i]].Value;
                    double step1 = (double)anudParameterStep[aiChecked[i]].Value;
                    double min2  = (double)anudParameterMin[aiChecked[j]].Value;
                    double max2  = (double)anudParameterMax[aiChecked[j]].Value;
                    double step2 = (double)anudParameterStep[aiChecked[j]].Value;

                    for (double value1 = min1; value1 <= max1; value1 += step1)
                        for (double value2 = min2; value2 <= max2; value2 += step2)
                            cycles += 1;
                }
            }

            return;
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Optimize all Parameters
            OptimizeParams(worker, e);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();

            if (!e.Cancelled && Configs.PlaySounds)
                System.Media.SystemSounds.Exclamation.Play();

            isOptimizing           = false;
            btnCancel.Enabled      = true;
            btnAccept.Enabled      = true;
            btnOptimize.Text       = Language.T("Optimize");
            progressBar.Value      = 1;

            if (pnlParams.Visible)
                for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                    aOptimizerButtons[i].Enabled = true;

            for (int i = 0; i < parameters; i++)
                alblParameterValue[i].Text = GetParameterText(i);

            foreach (Control control in pnlParams.Controls)
                control.Enabled = true;

            foreach (Control control in pnlLimitations.Controls)
                control.Enabled = true;

            foreach (Control control in pnlSettings.Controls)
                control.Enabled = true;

            if (chbOptimizerWritesReport.Checked)
                SaveReport();

            Cursor = Cursors.Default;

            return;
        }

        /// <summary>
        /// Saves the Generator History
        /// </summary>
        void SetStrategyToGeneratorHistory()
        {
            Data.GeneratorHistory.Add(Data.Strategy.Clone());

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        /// Optimize all the checked parameters
        /// </summary>
        void OptimizeParams(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int bestBalance = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);

            // First Optimization Cycle
            for (int round = 0; round < checkedParams && isOptimizing; round++)
            {
                if (worker.CancellationPending) break;

                int param = aiChecked[round];

                double min  = (double)anudParameterMin[param].Value;
                double max  = (double)anudParameterMax[param].Value;
                double step = (double)anudParameterStep[param].Value;

                for (double value = min; value <= max; value += step)
                {
                    if (worker.CancellationPending) break;

                    aParameter[param].Value = value;
                    if (aParameter[param].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param].SlotNumber);

                    Backtester.Calculate();
                    Backtester.CalculateAccountStats();
                    if (chbOptimizerWritesReport.Checked)
                        FillInReport();

                    int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                    if (balance > bestBalance && IsLimitationsFulfilled())
                    {
                        bestBalance = balance;
                        aParameter[param].BestValue = value;
                        ShowParamBestValue(param);
                        smallBalanceChart.SetChartData();
                        smallBalanceChart.InitChart();
                        smallBalanceChart.Invalidate();
                        isStartegyChanged = true;
                        SetStrategyToGeneratorHistory();
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    int percentComplete = 100 * computedCycles / cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > progressPercent)
                    {
                        progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }

                aParameter[param].Value = aParameter[param].BestValue;
                if (aParameter[param].Type == OptimizerParameterType.Indicator)
                    CalculateIndicator(aParameter[param].SlotNumber);
                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (checkedParams < 2)
                return;

            // Counts the necessary round
            int rounds= 0;
            for (int i = 0; i < checkedParams - 1; i++)
            for (int j = 0; j < checkedParams    ; j++)
                if (i < j) rounds++;

            CoupleOfParams[] aCP     = new CoupleOfParams[rounds];
            CoupleOfParams[] aCPTemp = new CoupleOfParams[rounds];

            rounds = 0;
            for (int i = 0; i < checkedParams - 1; i++)
            for (int j = 0; j < checkedParams    ; j++)
                if (i < j)
                {
                    aCPTemp[rounds].Param1 = aiChecked[i];
                    aCPTemp[rounds].Param2 = aiChecked[j];
                    aCPTemp[rounds].IsPassed = false;
                    rounds++;
                }

            // Shaking the params
            for (int round = 0; round < rounds; round++)
            {
                int couple = 0;
                do
                {
                    couple = random.Next(rounds);
                } while (aCPTemp[couple].IsPassed);
                aCPTemp[couple].IsPassed = true;
                aCP[round] = aCPTemp[couple];
            }

            // The Optimization Cycle
            for (int round = 0; round < rounds; round++)
            {
                if (worker.CancellationPending) break;

                int  param1 = aCP[round].Param1;
                int  param2 = aCP[round].Param2;
                bool isOneIndicator = (aParameter[param1].Type == OptimizerParameterType.Indicator &&
                                       aParameter[param2].Type == OptimizerParameterType.Indicator &&
                                       aParameter[param1].IndParam.IndicatorName == aParameter[param2].IndParam.IndicatorName);

                double min1  = (double)anudParameterMin[param1].Value;
                double max1  = (double)anudParameterMax[param1].Value;
                double step1 = (double)anudParameterStep[param1].Value;

                double min2  = (double)anudParameterMin[param2].Value;
                double max2  = (double)anudParameterMax[param2].Value;
                double step2 = (double)anudParameterStep[param2].Value;

                for (double value1 = min1; value1 <= max1; value1 += step1)
                {
                    if (worker.CancellationPending) break;

                    if (!isOneIndicator)
                    {
                        aParameter[param1].Value = value1;
                        if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                            CalculateIndicator(aParameter[param1].SlotNumber);
                    }

                    for (double value2 = min2; value2 <= max2; value2 += step2)
                    {
                        if (worker.CancellationPending) break;

                        if (isOneIndicator)
                        {
                            aParameter[param1].Value = value1;
                            aParameter[param2].Value = value2;
                            if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(aParameter[param1].SlotNumber);
                        }
                        else
                        {
                            aParameter[param2].Value = value2;
                            if (aParameter[param2].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(aParameter[param2].SlotNumber);
                        }

                        // Calculates the Strategy
                        Backtester.Calculate();
                        Backtester.CalculateAccountStats();
                        if (chbOptimizerWritesReport.Checked)
                            FillInReport();

                        int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                        if (balance > bestBalance && IsLimitationsFulfilled())
                        {
                            bestBalance = balance;
                            aParameter[param1].BestValue = value1;
                            aParameter[param2].BestValue = value2;
                            ShowParamBestValue(param1);
                            ShowParamBestValue(param2);
                            smallBalanceChart.SetChartData();
                            smallBalanceChart.InitChart();
                            smallBalanceChart.Invalidate();
                            isStartegyChanged = true;
                            SetStrategyToGeneratorHistory();
                        }

                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        int percentComplete = 100 * computedCycles / cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > progressPercent)
                        {
                            progressPercent = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }
                }

                aParameter[param1].Value = aParameter[param1].BestValue;
                aParameter[param2].Value = aParameter[param2].BestValue;

                if (isOneIndicator)
                {
                    if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param1].SlotNumber);
                }
                else
                {
                    if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param1].SlotNumber);
                    if (aParameter[param2].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param2].SlotNumber);
                }

                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }

            return;
        }

        /// <summary>
        /// Calculates the indicator in the designated slot
        /// </summary>
        void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Data.Strategy.Slot[slot].IndParam;

            Indicator indicator = Indicator_Store.ConstructIndicator(ip.IndicatorName, ip.SlotType);

            // List params
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index   = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text    = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value   = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(ip.SlotType);

            // Sets Data.Strategy
            Data.Strategy.Slot[slot].IndicatorName  = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam       = indicator.IndParam;
            Data.Strategy.Slot[slot].Component      = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue      = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined      = true;

            // Search the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();

            return;
        }

        /// <summary>
        /// Calculates the Limitations Criteria
        /// </summary>
        bool IsLimitationsFulfilled()
        {
            // Limitation Max Ambiguous Bars
            if (chbAmbiguousBars.Checked && Backtester.AmbiguousBars > nudAmbiguousBars.Value)
                return false;

            // Limitation Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney ? Backtester.MaxMoneyEquityDrawdown : Backtester.MaxEquityDrawdown;
            if (chbMaxDrawdown.Checked && maxEquityDrawdown > (double)nudMaxDrawdown.Value)
                return false;

            // Limitation Max Equity percent drawdown
            if (chbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double)nudEquityPercent.Value)
                return false;

            // Limitation Min Trades
            if (chbMinTrades.Checked && Backtester.ExecutedOrders < nudMinTrades.Value)
                return false;

            // Limitation Max Trades
            if (chbMaxTrades.Checked && Backtester.ExecutedOrders > nudMaxTrades.Value)
                return false;

            // Limitation Win / Loss ratio
            if (chbWinLossRatio.Checked && Backtester.WinLossRatio < (double)nudWinLossRatio.Value)
                return false;

            // OOS Pattern filter
            if (chbOOSPatternFilter.Checked && chbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int OOSbalance = Backtester.Balance(barOOS);
                float targetBalanceRatio = 1 + (int)nudOutOfSample.Value / 100.0F;
                int targetBalance = (int)(OOSbalance * targetBalanceRatio);
                int minBalance = (int)(targetBalance * (1 - nudOOSPatternPercent.Value / 100));
                if (netBalance < OOSbalance || netBalance < minBalance)
                    return false;
            }

            // Smooth Balance Line
            if (chbSmoothBalanceLines.Checked)
            {
                int checkPoints = (int)nudSmoothBalanceCheckPoints.Value;
                double maxPercentDeviation = (double)(nudSmoothBalancePercent.Value / 100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Backtester.FirstBar;
                    int bar = Backtester.FirstBar + i * (Data.Bars - firstBar) / (checkPoints + 1);
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                    double minBalance = targetBalance * (1 - maxPercentDeviation);
                    double maxBalance = targetBalance * (1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                        return false;

                    if (Configs.AdditionalStatistics)
                    {
                        // Long balance line
                        netBalance = Backtester.NetLongMoneyBalance;
                        checkPointBalance = Backtester.LongMoneyBalance(bar);
                        startBalance = Backtester.LongMoneyBalance(firstBar);
                        targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                        minBalance = targetBalance * (1 - maxPercentDeviation);
                        maxBalance = targetBalance * (1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;

                        // Short balance line
                        netBalance = Backtester.NetShortMoneyBalance;
                        checkPointBalance = Backtester.ShortMoneyBalance(bar);
                        startBalance = Backtester.ShortMoneyBalance(firstBar);
                        targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                        minBalance = targetBalance * (1 - maxPercentDeviation);
                        maxBalance = targetBalance * (1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;
                    }
                }

            }

            return true;
        }

        delegate void ShowParamBestValueCallback(int param);
        /// <summary>
        /// Shows the best value of a parameter during optimization.
        /// </summary>
        void ShowParamBestValue(int param)
        {
            if (alblParameterValue[param].InvokeRequired)
            {
                BeginInvoke(new ShowParamBestValueCallback(ShowParamBestValue), new object[] { param });
            }
            else
            {

                alblParameterValue[param].Text = GetParameterText(param);
                alblParameterValue[param].Font = fontParamValueBold;
                if (aParameter[param].OldBestValue < aParameter[param].BestValue)
                    alblParameterValue[param].ForeColor = LayoutColors.ColorTradeLong;
                if (aParameter[param].OldBestValue > aParameter[param].BestValue)
                    alblParameterValue[param].ForeColor = LayoutColors.ColorTradeShort;

                Timer timer = new Timer();
                timer.Interval = 1500;
                timer.Tag      = param;
                timer.Tick    += new EventHandler(Timer_Tick);
                timer.Start();
            }
        }

        /// <summary>
        /// Generates the parameter text.
        /// </summary>
        string GetParameterText(int param)
        {
            string stringFormat = "{0:F" + aParameter[param].Point.ToString() + "}";
            string newText = string.Format(stringFormat, aParameter[param].BestValue);

            if (alblParameterValue[param].Text == newText)
                return newText;

            if (Math.Abs(aParameter[param].BestValue - (double)anudParameterMin[param].Value) < 0.000001)
                newText = "[" + newText;
            if (Math.Abs(aParameter[param].BestValue - (double)anudParameterMax[param].Value) < 0.000001)
                newText = newText + "]";

            return newText;
        }

        /// <summary>
        /// Recovers the font of a parameter value label.
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            int   param = (int)timer.Tag;

            alblParameterValue[param].ForeColor = LayoutColors.ColorControlText;
            alblParameterValue[param].Font = fontParamValueRegular;

            timer.Stop();
            timer.Dispose();
        }
    }
}
