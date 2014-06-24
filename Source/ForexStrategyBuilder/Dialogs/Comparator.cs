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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Media;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder
{
    internal sealed class Comparator : Form
    {
        private readonly BackgroundWorker bgWorker;
        private readonly Brush brushRandArea;
        private readonly int countMethods;
        private readonly bool isTradeUntilMC = Configs.TradeUntilMarginCall;
        private readonly Pen penBalance;
        private readonly Pen penNearest;
        private readonly Pen penOptimistic;
        private readonly Pen penPessimistic;
        private readonly Pen penRandBands;
        private readonly Pen penRandom;
        private readonly Pen penShortest;
        private float[] afBalance;
        private float[] afMaxRandom;
        private float[,] afMethods;
        private float[] afMinRandom;
        private float[,] afRandoms;
        private int checkedMethods;
        private bool isPaintChart;
        private bool isRandom;
        private bool isWorking; // It is true when the comparator is running
        private int lines;
        private float maximum;
        private float maximumRandom;
        private float minimum;
        private float minimumRandom;

        /// <summary>
        ///     Initialize the form and controls
        /// </summary>
        public Comparator()
        {
            PnlOptions = new Panel();
            PnlChart = new Panel();
            ProgressBar = new ProgressBar();
            LblAverageBalance = new Label();
            NumRandom = new NumericUpDown();
            LblRandomCycles = new Label();
            BtnCalculate = new Button();
            BtnClose = new Button();

            Text = Language.T("Comparator");
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            FormClosing += ActionsFormClosing;

            isPaintChart = false;

            //Button Calculate
            BtnCalculate.Parent = this;
            BtnCalculate.Name = "btnCalculate";
            BtnCalculate.Text = Language.T("Calculate");
            BtnCalculate.Click += BtnCalculateClick;
            BtnCalculate.UseVisualStyleBackColor = true;

            //Button Close
            BtnClose.Parent = this;
            BtnClose.Name = "btnClose";
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.OK;
            BtnClose.UseVisualStyleBackColor = true;

            // ProgressBar
            ProgressBar.Parent = this;
            ProgressBar.Minimum = 1;
            ProgressBar.Maximum = 100;
            ProgressBar.Step = 1;

            PnlChart.Parent = this;
            PnlChart.ForeColor = LayoutColors.ColorControlText;
            PnlChart.Paint += PnlChartPaint;

            PnlOptions.Parent = this;
            PnlOptions.ForeColor = LayoutColors.ColorControlText;
            PnlOptions.Paint += PnlOptionsPaint;

            countMethods = Enum.GetValues(typeof (InterpolationMethod)).Length;
            AchboxMethods = new CheckBox[countMethods];
            for (int i = 0; i < countMethods; i++)
            {
                AchboxMethods[i] = new CheckBox
                    {
                        Parent = PnlOptions,
                        Text = Language.T(Enum.GetNames(typeof (InterpolationMethod))[i]),
                        Tag = Enum.GetValues(typeof (InterpolationMethod)).GetValue(i),
                        Checked = true,
                        BackColor = Color.Transparent,
                        AutoSize = true
                    };
                AchboxMethods[i].CheckedChanged += ComparatorCheckedChanged;
            }

            // Label Average Balance
            LblAverageBalance.Parent = PnlOptions;
            LblAverageBalance.AutoSize = true;
            LblAverageBalance.Text = Language.T("Average balance");
            LblAverageBalance.ForeColor = LayoutColors.ColorControlText;
            LblAverageBalance.BackColor = Color.Transparent;
            LblAverageBalance.TextAlign = ContentAlignment.MiddleLeft;

            // NumUpDown random cycles
            NumRandom.BeginInit();
            NumRandom.Parent = this;
            NumRandom.Value = 25;
            NumRandom.Minimum = 3;
            NumRandom.Maximum = 100;
            NumRandom.TextAlign = HorizontalAlignment.Center;
            NumRandom.EndInit();

            // Label Random Cycles
            LblRandomCycles.Parent = this;
            LblRandomCycles.AutoSize = true;
            LblRandomCycles.ForeColor = LayoutColors.ColorControlText;
            LblRandomCycles.BackColor = Color.Transparent;
            LblRandomCycles.Text = Language.T("Random iterations");
            LblRandomCycles.TextAlign = ContentAlignment.MiddleLeft;

            // Colors
            penOptimistic = new Pen(LayoutColors.ComparatorChartOptimisticLine);
            penPessimistic = new Pen(LayoutColors.ComparatorChartPessimisticLine);
            penShortest = new Pen(LayoutColors.ComparatorChartShortestLine);
            penNearest = new Pen(LayoutColors.ComparatorChartNearestLine);
            penRandom = new Pen(LayoutColors.ComparatorChartRandomLine);
            penRandBands = new Pen(LayoutColors.ComparatorChartRandomBands);
            brushRandArea = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
            penBalance = new Pen(LayoutColors.ComparatorChartBalanceLine) {Width = 2};

            // BackGroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.ProgressChanged += BgWorkerProgressChanged;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            Configs.TradeUntilMarginCall = false;
        }

        private Panel PnlOptions { get; set; }
        private CheckBox[] AchboxMethods { get; set; }
        private Label LblAverageBalance { get; set; }
        private Panel PnlChart { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private NumericUpDown NumRandom { get; set; }
        private Label LblRandomCycles { get; set; }
        private Button BtnCalculate { get; set; }
        private Button BtnClose { get; set; }

        /// <summary>
        ///     Resizes the form
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Width = (int) (Data.HorizontalDlu*290);
            Height = (int) (Data.VerticalDlu*260);
        }

        /// <summary>
        ///     Arrange the controls
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            int controlZoneH = buttonHeight + 2*btnVertSpace;
            int controlZoneY = ClientSize.Height - controlZoneH;
            int buttonY = controlZoneY + btnVertSpace;

            PnlOptions.Size = new Size(ClientSize.Width - 2*space, 90);
            PnlOptions.Location = new Point(space, space);

            int positionX = (PnlOptions.ClientSize.Width - 10)/3;
            const int positionY = 27;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < countMethods)
                        AchboxMethods[num].Location = new Point(j*positionX + 40, i*30 + positionY);
                    else
                        LblAverageBalance.Location = new Point(j*positionX + 40, i*30 + positionY + 1);

                    num++;
                }
            }

            NumRandom.Size = new Size(50, buttonHeight);
            NumRandom.Location = new Point(btnHrzSpace, controlZoneY + (controlZoneH - NumRandom.Height)/2);
            LblRandomCycles.Location = new Point(NumRandom.Right + 5,
                                                 controlZoneY + (controlZoneH - LblRandomCycles.Height)/2);

            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, buttonY);

            BtnCalculate.Size = new Size(buttonWidth, buttonHeight);
            BtnCalculate.Location = new Point(BtnClose.Left - buttonWidth - btnHrzSpace, buttonY);

            ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDlu*9));
            ProgressBar.Location = new Point(space, BtnClose.Top - ProgressBar.Height - btnVertSpace);
            PnlChart.Size = new Size(ClientSize.Width - 2*space, ProgressBar.Top - PnlOptions.Bottom - 2*space);
            PnlChart.Location = new Point(space, PnlOptions.Bottom + space);
        }

        /// <summary>
        ///     Check whether the strategy have been changed.
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (isWorking)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
            }

            Configs.TradeUntilMarginCall = isTradeUntilMC;
        }

        /// <summary>
        ///     A check boxes status
        /// </summary>
        private void ComparatorCheckedChanged(object sender, EventArgs e)
        {
            var chbox = (CheckBox) sender;
            var interpMethod = (InterpolationMethod) chbox.Tag;

            if (interpMethod == InterpolationMethod.Random)
            {
                NumRandom.Enabled = chbox.Checked;
            }
        }

        /// <summary>
        ///     Calculate
        /// </summary>
        private void BtnCalculateClick(object sender, EventArgs e)
        {
            if (isWorking)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            ProgressBar.Value = 1;
            isWorking = true;
            BtnClose.Enabled = false;
            BtnCalculate.Text = Language.T("Stop");

            for (int m = 0; m < countMethods; m++)
            {
                AchboxMethods[m].Enabled = false;
            }
            NumRandom.Enabled = false;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            ProgressBar.Value = 1;

            // Optimize all Parameters
            if (Calculate(worker) == 0)
            {
                isPaintChart = true;
                PnlChart.Invalidate();
            }
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isWorking = false;
            BtnClose.Enabled = true;
            BtnCalculate.Text = Language.T("Calculate");

            for (int m = 0; m < countMethods; m++)
            {
                AchboxMethods[m].Enabled = true;
            }
            NumRandom.Enabled = true;

            Cursor = Cursors.Default;
            BtnClose.Focus();
        }

        /// <summary>
        ///     Calculates the balance lines
        /// </summary>
        private int Calculate(BackgroundWorker worker)
        {
            // Determine the number of lines
            // For each method per line
            // The random line shows the averaged values
            // Also we have two border lines for the random method
            // Plus the average balance line

            isRandom = false;
            minimum = float.MaxValue;
            maximum = float.MinValue;
            minimumRandom = float.MaxValue;
            maximumRandom = float.MinValue;
            var randomLines = (int) NumRandom.Value;

            checkedMethods = 0;
            lines = 1;
            for (int m = 0; m < countMethods; m++)
                if (AchboxMethods[m].Checked)
                {
                    checkedMethods++;
                    lines++;
                    if ((InterpolationMethod) AchboxMethods[m].Tag == InterpolationMethod.Random)
                        isRandom = true;
                }

            if (checkedMethods == 0 && Configs.PlaySounds)
            {
                SystemSounds.Hand.Play();
                return -1;
            }

            afBalance = new float[Data.Bars - Data.FirstBar];
            afMethods = new float[countMethods,Data.Bars - Data.FirstBar];
            if (isRandom)
            {
                afRandoms = new float[randomLines,Data.Bars - Data.FirstBar];
                afMinRandom = new float[Data.Bars - Data.FirstBar];
                afMaxRandom = new float[Data.Bars - Data.FirstBar];
            }

            // Progress parameters
            int computedCycles = 0;
            int cycles = lines + (isRandom ? randomLines : 0);
            int highestPercentageReached = 0;
            int percentComplete;

            // Calculates the lines
            for (int m = 0; m < countMethods; m++)
            {
                if (worker.CancellationPending) return -1;
                if (!AchboxMethods[m].Checked) continue;

                var method = (InterpolationMethod) AchboxMethods[m].Tag;

                if (method == InterpolationMethod.Random)
                {
                    for (int r = 0; r < randomLines; r++)
                    {
                        if (worker.CancellationPending) return -1;

                        Backtester.InterpolationMethod = method;
                        Backtester.Calculate();

                        if (Configs.AccountInMoney)
                            for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                                afRandoms[r, iBar] = (float) Backtester.MoneyBalance(iBar + Data.FirstBar);
                        else
                            for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                                afRandoms[r, iBar] = Backtester.Balance(iBar + Data.FirstBar);


                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        percentComplete = 100*computedCycles/cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > highestPercentageReached)
                        {
                            highestPercentageReached = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }

                    for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                    {
                        float randomSum = 0;
                        float minRandom = float.MaxValue;
                        float maxRandom = float.MinValue;
                        for (int r = 0; r < randomLines; r++)
                        {
                            float value = afRandoms[r, iBar];
                            randomSum += value;
                            minRandom = value < minRandom ? value : minRandom;
                            maxRandom = value > maxRandom ? value : maxRandom;
                        }
                        afMethods[m, iBar] = randomSum/randomLines;
                        afMinRandom[iBar] = minRandom;
                        afMaxRandom[iBar] = maxRandom;
                        minimumRandom = minRandom < minimumRandom ? minRandom : minimumRandom;
                        maximumRandom = maxRandom > maximumRandom ? maxRandom : maximumRandom;
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100*computedCycles/cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
                else
                {
                    Backtester.InterpolationMethod = method;
                    Backtester.Calculate();

                    if (Configs.AccountInMoney)
                        for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                            afMethods[m, iBar] = (float) Backtester.MoneyBalance(iBar + Data.FirstBar);
                    else
                        for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                            afMethods[m, iBar] = Backtester.Balance(iBar + Data.FirstBar);

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100*computedCycles/cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            }

            // Calculates the average balance, Min and Max
            for (int bar = 0; bar < Data.Bars - Data.FirstBar; bar++)
            {
                float sum = 0;
                for (int m = 0; m < countMethods; m++)
                {
                    if (!AchboxMethods[m].Checked) continue;

                    float value = afMethods[m, bar];
                    sum += value;
                    if (value < minimum)
                        minimum = value;
                    if (value > maximum)
                        maximum = value;
                }
                afBalance[bar] = sum/checkedMethods;
            }

            // Report progress as a percentage of the total task.
            computedCycles++;
            percentComplete = 100*computedCycles/cycles;
            percentComplete = percentComplete > 100 ? 100 : percentComplete;
            if (percentComplete > highestPercentageReached)
            {
                worker.ReportProgress(percentComplete);
            }

            return 0;
        }

        /// <summary>
        ///     Paints panel pnlOptions
        /// </summary>
        private void PnlOptionsPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            const int border = 2;

            // Chart Title
            string str = Language.T("Interpolation Methods");
            var font = new Font(Font.FontFamily, 9);
            var fCaptionHeight = (float) Math.Max(font.Height, 18);
            var rectfCaption = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            var stringFormatCaption = new StringFormat
                {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel background
            var rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2*border,
                                            pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);

            int positionX = (PnlOptions.ClientSize.Width - 10)/3;
            const int positionY = 35;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < countMethods)
                    {
                        var pt1 = new Point(j*positionX + 10, i*30 + positionY);
                        var pt2 = new Point(j*positionX + 30, i*30 + positionY);
                        var pen = new Pen(Color.Red);
                        switch ((InterpolationMethod) AchboxMethods[num].Tag)
                        {
                            case InterpolationMethod.Pessimistic:
                                pen = new Pen(LayoutColors.ComparatorChartPessimisticLine);
                                break;
                            case InterpolationMethod.Shortest:
                                pen = new Pen(LayoutColors.ComparatorChartShortestLine);
                                break;
                            case InterpolationMethod.Nearest:
                                pen = new Pen(LayoutColors.ComparatorChartNearestLine);
                                break;
                            case InterpolationMethod.Optimistic:
                                pen = new Pen(LayoutColors.ComparatorChartOptimisticLine);
                                break;
                            case InterpolationMethod.Random:
                                var pntRnd1 = new Point(j*positionX + 10, i*30 + positionY - 6);
                                var pntRnd2 = new Point(j*positionX + 30, i*30 + positionY - 6);
                                var pntRnd3 = new Point(j*positionX + 10, i*30 + positionY + 6);
                                var pntRnd4 = new Point(j*positionX + 30, i*30 + positionY + 6);
                                var penRnd = new Pen(LayoutColors.ComparatorChartRandomBands, 2);
                                Brush brushRnd = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
                                g.FillRectangle(brushRnd,
                                                new Rectangle(pntRnd1.X, pntRnd1.Y, pntRnd2.X - pntRnd1.X,
                                                              pntRnd4.Y - pntRnd2.Y));
                                g.DrawLine(penRnd, pntRnd1, pntRnd2);
                                g.DrawLine(penRnd, pntRnd3, pntRnd4);
                                pen = new Pen(LayoutColors.ComparatorChartRandomLine);
                                break;
                        }
                        pen.Width = 2;
                        g.DrawLine(pen, pt1, pt2);
                    }
                    else
                    {
                        var pt1 = new Point(j*positionX + 10, i*30 + positionY);
                        var pt2 = new Point(j*positionX + 30, i*30 + positionY);
                        var pen = new Pen(LayoutColors.ComparatorChartBalanceLine) {Width = 3};
                        g.DrawLine(pen, pt1, pt2);
                    }

                    num++;
                }
            }
        }

        /// <summary>
        ///     Paints the charts
        /// </summary>
        private void PnlChartPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            const int space = 5;
            const int border = 2;

            // Chart Title
            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("points")) + "]";
            string str = Language.T("Balance Chart") + unit;
            var font = new Font(Font.FontFamily, 9);
            var fCaptionHeight = (float) Math.Max(font.Height, 18);
            var rectfCaption = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            var stringFormatCaption = new StringFormat
                {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel background
            var rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2*border,
                                            pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);

            if (!isPaintChart)
            {
                if (Backtester.AmbiguousBars == 0)
                {
                    string sNote = Language.T("The Comparator is useful when the backtest shows ambiguous bars!");
                    var rectfNote = new RectangleF(0, 30, pnl.ClientSize.Width, Font.Height);
                    g.DrawString(sNote, Font, new SolidBrush(LayoutColors.ColorChartFore), rectfNote,
                                 stringFormatCaption);
                }
                return;
            }

            int bars = Data.Bars - Data.FirstBar;
            int max = (int) Math.Max(maximum, maximumRandom) + 1;
            int min = (int) Math.Min(minimum, minimumRandom) - 1;
            min = (int) Math.Floor(min/10f)*10;
            int yTop = (int) fCaptionHeight + 2*space + 1;
            int yBottom = (pnl.ClientSize.Height - 2*space - border);
            var labelWidth =
                (int)
                Math.Max(g.MeasureString(min.ToString(CultureInfo.InvariantCulture), Font).Width,
                         g.MeasureString(max.ToString(CultureInfo.InvariantCulture), Font).Width);
            labelWidth = Math.Max(labelWidth, 30);
            int xRight = pnl.ClientSize.Width - border - space - labelWidth;
            int xLeft = border + space;

            //
            // Grid
            //
            int cntLabels = Math.Max((yBottom - yTop)/20, 1);
            var delta = (float) Math.Max(Math.Round((max - min)/(float) cntLabels), 10);
            int step = (int) Math.Ceiling(delta/10)*10;
            cntLabels = (int) Math.Ceiling((max - min)/(float) step);
            max = min + cntLabels*step;
            float scaleY = (yBottom - yTop)/(cntLabels*(float) step);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            var penGrid = new Pen(LayoutColors.ColorChartGrid)
                {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            // Price labels
            for (int label = min; label <= max; label += step)
            {
                var labelY = (int) (yBottom - (label - min)*scaleY);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, border + space, labelY, xRight, labelY);
            }

            float xScale = (xRight - 2 * space - border) / (float)bars;

            if (isRandom)
            {
                // Draws the random area and Min Max lines
                var apntMinRandom = new PointF[bars];
                var apntMaxRandom = new PointF[bars];
                for (int iBar = 0; iBar < bars; iBar++)
                {
                    apntMinRandom[iBar].X = border + space + iBar*xScale;
                    apntMinRandom[iBar].Y = yBottom - (afMinRandom[iBar] - min)*scaleY;
                    apntMaxRandom[iBar].X = border + space + iBar*xScale;
                    apntMaxRandom[iBar].Y = yBottom - (afMaxRandom[iBar] - min)*scaleY;
                }
                apntMinRandom[0].Y = apntMaxRandom[0].Y;
                var path = new GraphicsPath();
                path.AddLines(apntMinRandom);
                path.AddLine(apntMinRandom[bars - 1], apntMaxRandom[bars - 1]);
                path.AddLines(apntMaxRandom);
                var region = new Region(path);
                g.FillRegion(brushRandArea, region);
                g.DrawLines(penRandBands, apntMinRandom);
                g.DrawLines(penRandBands, apntMaxRandom);
            }

            // Draws the lines
            for (int m = 0; m < countMethods; m++)
            {
                if (!AchboxMethods[m].Checked) continue;

                var apntLines = new PointF[bars];
                for (int iBar = 0; iBar < bars; iBar++)
                {
                    apntLines[iBar].X = border + space + iBar*xScale;
                    apntLines[iBar].Y = yBottom - (afMethods[m, iBar] - min)*scaleY;
                }

                var pen = new Pen(LayoutColors.ColorSignalRed);
                switch ((InterpolationMethod) AchboxMethods[m].Tag)
                {
                    case InterpolationMethod.Pessimistic:
                        pen = penPessimistic;
                        break;
                    case InterpolationMethod.Shortest:
                        pen = penShortest;
                        break;
                    case InterpolationMethod.Nearest:
                        pen = penNearest;
                        break;
                    case InterpolationMethod.Optimistic:
                        pen = penOptimistic;
                        break;
                    case InterpolationMethod.Random:
                        pen = penRandom;
                        break;
                }
                g.DrawLines(pen, apntLines);
            }

            // Draws the average balance
            var apntBalance = new PointF[bars];
            for (int bar = 0; bar < bars; bar++)
            {
                apntBalance[bar].X = border + space + bar*xScale;
                apntBalance[bar].Y = yBottom - (afBalance[bar] - min)*scaleY;
            }
            g.DrawLines(penBalance, apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yTop - space, xLeft - 1, yBottom);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft, yBottom, xRight, yBottom);

            // Balance label
            float fBalanceY = yBottom - (afBalance[bars - 1] - min)*scaleY;
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), xLeft, fBalanceY, xRight - space, fBalanceY);

            var szBalance = new Size(labelWidth + space, Font.Height + 2);
            var point = new Point(xRight - space + 2, (int) (fBalanceY - Font.Height/2.0 - 1));
            var rec = new Rectangle(point, szBalance);
            string sBalance = ((int) afBalance[bars - 1]).ToString(CultureInfo.InvariantCulture);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), rec);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), rec);
            g.DrawString(sBalance, Font, new SolidBrush(LayoutColors.ColorLabelText), rec, stringFormatCaption);

            // Scanning note
            var fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Configs.Autoscan && !Data.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, xLeft, fCaptionHeight - 2);
            else if (Backtester.IsScanPerformed)
                g.DrawString(Language.T("Scanned") + " MQ " + Data.ModellingQuality.ToString("N2") + "%", fontNote,
                             Brushes.LimeGreen, border + space, fCaptionHeight - 2);

            // Scanned bars
            if (Backtester.IsScanPerformed &&
                (Data.IntraBars != null && Data.IsIntrabarData ||
                 Data.Period == DataPeriod.M1 && Data.IsTickData && Configs.UseTickData))
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yBottom, xLeft - 1,
                           yBottom + 8);
                DataPeriod dataPeriod = Data.Period;
                Color color = Data.PeriodColor[Data.Period];
                int iFromBar = Data.FirstBar;
                for (int bar = Data.FirstBar; bar < Data.Bars; bar++)
                {
                    if (Data.IntraBarsPeriods[bar] != dataPeriod || bar == Data.Bars - 1)
                    {
                        int xStart = (int)((iFromBar - Data.FirstBar) * xScale) + xLeft;
                        int xEnd = (int)((bar - Data.FirstBar) * xScale) + xLeft;
                        iFromBar = bar;
                        dataPeriod = Data.IntraBarsPeriods[bar];
                        Data.GradientPaint(g, new RectangleF(xStart, yBottom + 3, xEnd - xStart + 2, 5), color, 60);
                        color = Data.PeriodColor[Data.IntraBarsPeriods[bar]];
                    }
                }

                // Tick Data
                if (Data.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    for (int b = 0; b < Data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                            lastBarWithTicks = b;
                    }
                    int xStart = (int)(firstBarWithTicks * xScale) + xLeft;
                    int xEnd = (int)((lastBarWithTicks - Data.FirstBar) * xScale) + xLeft;
                    if (xStart < xLeft)
                        xStart = xLeft;
                    if (xEnd < xStart)
                        xEnd = xStart;

                    Data.DrawCheckerBoard(g, Color.ForestGreen, new Rectangle(xStart, yBottom + 4, xEnd - xStart + 2, 3));
                }

            }
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}