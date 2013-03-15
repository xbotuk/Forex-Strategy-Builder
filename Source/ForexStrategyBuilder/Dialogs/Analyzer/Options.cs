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

namespace ForexStrategyBuilder.Dialogs.Analyzer
{
    public class Options : FancyPanel
    {
        public Options(string caption) : base(caption)
        {
            Color colorText = LayoutColors.ColorControlText;

            LblColumnSeparator = new Label();
            LblDecimalSeparator = new Label();
            CbxColumnSeparator = new ComboBox();
            CbxDecimalSeparator = new ComboBox();
            ChbHideFSB = new CheckBox();

            // Label Decimal Separator
            LblDecimalSeparator.Parent = this;
            LblDecimalSeparator.ForeColor = colorText;
            LblDecimalSeparator.BackColor = Color.Transparent;
            LblDecimalSeparator.AutoSize = true;
            LblDecimalSeparator.Text = Language.T("Report - decimal separator");

            // Label Column Separator
            LblColumnSeparator.Parent = this;
            LblColumnSeparator.ForeColor = colorText;
            LblColumnSeparator.BackColor = Color.Transparent;
            LblColumnSeparator.AutoSize = true;
            LblColumnSeparator.Text = Language.T("Report - column separator");

            // ComboBox Column Separator
            CbxDecimalSeparator.Parent = this;
            CbxDecimalSeparator.Items.AddRange(new object[] {Language.T("Dot") + " '.'", Language.T("Comma") + " ','"});
            CbxDecimalSeparator.SelectedIndex = Configs.DecimalSeparator == "." ? 0 : 1;
            CbxDecimalSeparator.SelectedIndexChanged += DecimalSeparatorChanged;

            // ComboBox Column Separator
            CbxColumnSeparator.Parent = this;
            CbxColumnSeparator.Items.AddRange(new object[]
                {
                    Language.T("Comma") + " ','", Language.T("Semicolon") + " ';'",
                    Language.T("Tab") + @" '\t'"
                });
            CbxColumnSeparator.SelectedIndex = Configs.ColumnSeparator == ","
                                                   ? 0
                                                   : Configs.ColumnSeparator == ";" ? 1 : 2;
            CbxColumnSeparator.SelectedIndexChanged += ColumnSeparator_Changed;

            // Hide FSB at startup.
            ChbHideFSB.Parent = this;
            ChbHideFSB.ForeColor = colorText;
            ChbHideFSB.BackColor = Color.Transparent;
            ChbHideFSB.Text = Language.T("Hide FSB when Analyzer starts");
            ChbHideFSB.Checked = Configs.AnalyzerHideFSB;
            ChbHideFSB.AutoSize = true;
            ChbHideFSB.CheckedChanged += HideFSBClick;

            Resize += PnlOptionsResize;
        }

        private Label LblColumnSeparator { get; set; }
        private Label LblDecimalSeparator { get; set; }

        private ComboBox CbxColumnSeparator { get; set; }
        private ComboBox CbxDecimalSeparator { get; set; }

        private CheckBox ChbHideFSB { get; set; }
        private Form FormFSB { get; set; }

        public Form SetParrentForm
        {
            set { FormFSB = value; }
        }

        private void PnlOptionsResize(object sender, EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            const int border = 2;

            LblDecimalSeparator.Location = new Point(btnHrzSpace + border, 0*buttonHeight + 2*space + 18);
            LblColumnSeparator.Location = new Point(btnHrzSpace + border, 1*buttonHeight + 3*space + 18);

            int maxLabelRight = LblDecimalSeparator.Right;
            if (LblColumnSeparator.Right > maxLabelRight) maxLabelRight = LblColumnSeparator.Right;
            int cbxLeft = maxLabelRight + btnHrzSpace;
            const int cbxWidth = 120;
            CbxDecimalSeparator.Size = new Size(cbxWidth, buttonHeight);
            CbxColumnSeparator.Size = new Size(cbxWidth, buttonHeight);
            CbxDecimalSeparator.Location = new Point(cbxLeft, 0*buttonHeight + 2*space + 16);
            CbxColumnSeparator.Location = new Point(cbxLeft, 1*buttonHeight + 3*space + 16);

            ChbHideFSB.Location = new Point(btnHrzSpace + border + 4, 2*buttonHeight + 4*space + 18);
        }

        private void ColumnSeparator_Changed(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;
            switch (comboBox.SelectedIndex)
            {
                case 0: // Comma
                    Configs.ColumnSeparator = ",";
                    break;
                case 1: // Semicolon
                    Configs.ColumnSeparator = ";";
                    break;
                case 2: // Tab
                    Configs.ColumnSeparator = "\t";
                    break;
            }
        }

        private void DecimalSeparatorChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;
            switch (comboBox.SelectedIndex)
            {
                case 0: // Dot
                    Configs.DecimalSeparator = ".";
                    break;
                case 1: // Comma
                    Configs.DecimalSeparator = ",";
                    break;
            }
        }

        /// <summary>
        ///     Toggles FSB visibility.
        /// </summary>
        private void HideFSBClick(object sender, EventArgs e)
        {
            if (FormFSB != null)
                FormFSB.Visible = !ChbHideFSB.Checked;

            Configs.AnalyzerHideFSB = ChbHideFSB.Checked;
        }

        /// <summary>
        ///     Shows FSB
        /// </summary>
        public void ShowFSB()
        {
            if (FormFSB != null)
                FormFSB.Visible = true;
        }

        /// <summary>
        ///     Shows or Hides FSB
        /// </summary>
        public void SetFSBVisiability()
        {
            if (FormFSB != null)
                FormFSB.Visible = !Configs.AnalyzerHideFSB;
        }
    }
}