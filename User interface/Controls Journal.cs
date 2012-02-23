// Forex Strategy Builder - Journal controls.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls Journal: Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private Panel _journalRight; // Parent panel for Order and Position panels
        private Journal_Positions _journalByPositions;
        private Journal_Bars _journalByBars;
        private Journal_Ord  _journalOrder;
        private Journal_Pos  _journalPosition;
        private Splitter _splitter;
        private int _selectedBarNumber;
        int _widthJournalOld;

        /// <summary>
        /// Initializes the controls in panel pnlJournal.
        /// </summary>
        void InitializeJournal()
        {
            // Journal Right
            _journalRight = new Panel {Parent = pnlJournal, Dock = DockStyle.Fill};

            // Journal Orders
            _journalOrder = new Journal_Ord {Parent = _journalRight, Dock = DockStyle.Fill, Cursor = Cursors.Hand};
            _journalOrder.Click += PnlJournalMouseClick;
            _journalOrder.PopUpContextMenu.Items.AddRange(GetJournalContextMenuItems());
            _journalOrder.IsContextButtonVisible = true;
            toolTip.SetToolTip(_journalOrder, Language.T("Click to view Bar Explorer."));

            new Splitter {Parent = _journalRight, Dock = DockStyle.Bottom, Height = space};

            // Journal Position
            _journalPosition = new Journal_Pos {Parent = _journalRight, Dock = DockStyle.Bottom, Cursor = Cursors.Hand};
            _journalPosition.Click += PnlJournalMouseClick;
            toolTip.SetToolTip(_journalPosition, Language.T("Click to view Bar Explorer."));

            _splitter = new Splitter {Parent = pnlJournal, Dock = DockStyle.Left, Width = space};

            // Journal by Bars
            _journalByBars = new Journal_Bars { Name = "JournalByBars", Parent = pnlJournal, Dock = DockStyle.Left };
            _journalByBars.SelectedBarChange += PnlJournalSelectedBarChange;
            _journalByBars.MouseDoubleClick  += PnlJournalMouseDoubleClick;
            toolTip.SetToolTip(_journalByBars, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));

            // Journal by Positions
            _journalByPositions = new Journal_Positions {Name = "JournalByPositions", Parent = pnlJournal, Dock = DockStyle.Fill};
            _journalByPositions.PopUpContextMenu.Items.AddRange(GetJournalContextMenuItems());
            _journalByPositions.IsContextButtonVisible = true;
            _journalByPositions.SelectedBarChange += PnlJournalSelectedBarChange;
            _journalByPositions.MouseDoubleClick  += PnlJournalMouseDoubleClick;
            toolTip.SetToolTip(_journalByPositions, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));

            pnlJournal.Resize += PnlJournalResize;

            _journalRight.Visible  = Configs.JournalByBars;
            _journalByBars.Visible = Configs.JournalByBars;
            _splitter.Visible      = Configs.JournalByBars;
            _journalByPositions.Visible = !Configs.JournalByBars;
            _journalByPositions.ShowTransfers = Configs.JournalShowTransfers;
        }

        private ToolStripItem[] GetJournalContextMenuItems()
        {
            var menuStripJournalByPosWithoutTransfers = new ToolStripMenuItem
            {
                Image = Properties.Resources.pos_buy,
                Text = Language.T("Journal by Positions") + " " + Language.T("without Transfers")
            };
            menuStripJournalByPosWithoutTransfers.Click += ContextMenuJournalByPosWithoutTransfersClick;

            var menuStripJournalByPositions = new ToolStripMenuItem
            {
                Image = Properties.Resources.pos_transfer_long,
                Text = Language.T("Journal by Positions")
            };
            menuStripJournalByPositions.Click += ContextMenuJournalByPositionsClick;

            var menuStripJournalByBars = new ToolStripMenuItem
            {
                Image = Properties.Resources.pos_square,
                Text = Language.T("Journal by Bars")
            };
            menuStripJournalByBars.Click += ContextMenuJournalByBarsClick;

            var menuStripBarExplorer = new ToolStripMenuItem
            {
                Image = Properties.Resources.bar_explorer,
                Text = Language.T("Show Bar Explorer")
            };
            menuStripBarExplorer.Click += ContextMenuBarExplorerClick;

            var menuStripCloseJournal = new ToolStripMenuItem
            {
                Image = Properties.Resources.close_button,
                Text = Language.T("Close Journal")
            };
            menuStripCloseJournal.Click += ContextMenuCloseJournalClick;

            var itemCollection = new ToolStripItem[]
            {
                menuStripJournalByPosWithoutTransfers,
                menuStripJournalByPositions,
                menuStripJournalByBars,
                new ToolStripSeparator(),
                menuStripBarExplorer,
                new ToolStripSeparator(),
                menuStripCloseJournal
            };

            return itemCollection;
        }

        /// <summary>
        /// Sets the journal data
        /// </summary>
        protected void SetupJournal()
        {
            if (!Configs.ShowJournal) return;
            if (Configs.JournalByBars)
            {
                _journalByBars.SetUpJournal();
                _journalByBars.Invalidate();
                _selectedBarNumber = _journalByBars.SelectedBar;
                _journalOrder.SelectedBar = _selectedBarNumber;
                _journalOrder.SetUpJournal();
                _journalOrder.Invalidate();
                _journalPosition.SelectedBar = _selectedBarNumber;
                _journalPosition.SetUpJournal();
                _journalPosition.Invalidate();
            }
            else
            {
                _journalByPositions.ShowTransfers = Configs.JournalShowTransfers;
                _journalByPositions.SetUpJournal();
                _journalByPositions.Invalidate();
                _selectedBarNumber = _journalByBars.SelectedBar;
            }
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        void PnlJournalResize(object sender, EventArgs e)
        {
            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                if(_widthJournalOld != pnlJournal.Width)
                    _journalByBars.Width = 2 * ClientSize.Width / 3;
                _journalPosition.Height = (pnlJournal.ClientSize.Height - space) / 2;
            }
            _widthJournalOld = pnlJournal.Width;
        }

        /// <summary>
        /// Sets the selected bar number
        /// </summary>
        void PnlJournalSelectedBarChange(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            if (panel.Name == "JournalByBars")
            {
                _selectedBarNumber = _journalByBars.SelectedBar;
                _journalOrder.SelectedBar = _selectedBarNumber;
                _journalOrder.SetUpJournal();
                _journalOrder.Invalidate();
                _journalPosition.SelectedBar = _selectedBarNumber;
                _journalPosition.SetUpJournal();
                _journalPosition.Invalidate();
            }
            else if (panel.Name == "JournalByPositions")
            {
                _selectedBarNumber = _journalByPositions.SelectedBar;
            }
        }

        void PnlJournalMouseClick(object sender, EventArgs e)
        {
            ShowBarExplorer();
        }

        void PnlJournalMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowBarExplorer();
        }

        void ContextMenuBarExplorerClick(object sender, EventArgs e)
        {
            ShowBarExplorer();
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        private void ShowBarExplorer()
        {
            var barExplorer = new Bar_Explorer(_selectedBarNumber);
            barExplorer.ShowDialog();
        }

        void ContextMenuJournalByPosWithoutTransfersClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = false;
            Configs.JournalShowTransfers = false;
            miJournalByPosWithoutTransfers.Checked = true;
            miJournalByPos.Checked = false;
            miJournalByBars.Checked = false;
            ResetJournal();
        }

        void ContextMenuJournalByPositionsClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = false;
            Configs.JournalShowTransfers = true;
            miJournalByPosWithoutTransfers.Checked = false;
            miJournalByPos.Checked = true;
            miJournalByBars.Checked = false;
            ResetJournal();
        }

        void ContextMenuJournalByBarsClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = true;
            miJournalByPosWithoutTransfers.Checked = false;
            miJournalByPos.Checked = false;
            miJournalByBars.Checked = true;
            ResetJournal();
        }

        void ContextMenuCloseJournalClick(object sender, EventArgs e)
        {
            miJournalByPosWithoutTransfers.Checked = false;
            miJournalByPos.Checked = false;
            miJournalByBars.Checked = false;
            Configs.ShowJournal = false;
            OnResize(EventArgs.Empty);
        }

        /// <summary>
        /// Resets the journal layout.
        /// </summary>
        protected void ResetJournal()
        {
            SetupJournal();

            _journalRight.Visible  = Configs.JournalByBars;
            _splitter.Visible      = Configs.JournalByBars;
            _journalByBars.Visible = Configs.JournalByBars;
            _journalByPositions.Visible = !Configs.JournalByBars;
            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                _journalByBars.Width    = 2 * ClientSize.Width / 3;
                _journalPosition.Height = pnlJournal.ClientSize.Height / 2;
            }

            OnResize(EventArgs.Empty);
        }
    }
}
