// Workspace form.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// This is the base application form.
    /// </summary>
    public class Workspace : Form
    {
        protected const int Gap = 4;
        protected Panel PanelAccount { get; private set; }
        protected Panel PanelJournal { get; private set; }
        protected Panel PanelMarket { get; private set; }
        protected Panel PanelStrategy { get; private set; }
        protected Panel PanelWorkspace { get; private set; }
        protected StatusStrip StatusBarStrip { get; private set; }
        protected ToolStrip ToolStripAccount { get; private set; }
        protected ToolStrip ToolStripMarket { get; private set; }
        protected ToolStrip ToolStripStrategy { get; private set; }
        private Panel PanelAccountBase { get; set; }
        private Panel PanelDataBase { get; set; }
        private Panel PanelJournalBase { get; set; }
        private Panel PanelMarketBase { get; set; }
        private Panel PanelStrategyBase { get; set; }
        private Splitter SplitterJournal { get; set; }
        private FileSystemWatcher _strategyDirWatcher;

        /// <summary>
        /// The default constructor
        /// </summary>
        protected Workspace()
        {
            Graphics g = CreateGraphics();
            SetGraphicalMeasures(g);
            g.Dispose();

            CreateControls();
            InitializeControls();
        }

        private void SetGraphicalMeasures(Graphics g)
        {
            SizeF sizeString = g.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", Font);
            Data.HorizontalDLU = (sizeString.Width / 62) / 4;
            Data.VerticalDLU = sizeString.Height / 8;
        }

        private void CreateControls()
        {
            MainMenuStrip = new MenuStrip();
            StatusBarStrip = new StatusStrip();

            PanelAccount = new Panel();
            PanelJournal = new Panel();
            PanelMarket = new Panel();
            PanelStrategy = new Panel();
            PanelWorkspace = new Panel();

            ToolStripMarket = new ToolStrip();
            ToolStripStrategy = new ToolStrip();
            ToolStripAccount = new ToolStrip();

            PanelDataBase = new Panel();
            PanelMarketBase = new Panel();
            PanelStrategyBase = new Panel();
            PanelAccountBase = new Panel();
            PanelJournalBase = new Panel();
            SplitterJournal = new Splitter();

            _strategyDirWatcher = new FileSystemWatcher();
        }

        private void InitializeControls()
        {
            // Panel Workspace
            PanelWorkspace.Parent = this;
            PanelWorkspace.Dock = DockStyle.Fill;
            PanelWorkspace.BackColor = LayoutColors.ColorFormBack;
            PanelWorkspace.AllowDrop = true;
            PanelWorkspace.DragEnter += WorkspaceDragEnter;
            PanelWorkspace.DragDrop += WorkspaceDragDrop;

            // Main menu
            MainMenuStrip.Parent = this;
            MainMenuStrip.Dock = DockStyle.Top;

            // Status bar
            StatusBarStrip.Parent = this;
            StatusBarStrip.Dock = DockStyle.Bottom;

            // Panel Journal Base
            PanelJournalBase.Parent = PanelWorkspace;
            PanelJournalBase.Dock = DockStyle.Fill;
            PanelJournalBase.Padding = new Padding(Gap, 0, Gap, Gap);

            // Splitter Journal
            SplitterJournal.Parent = PanelWorkspace;
            SplitterJournal.Dock = DockStyle.Top;
            SplitterJournal.Height = Gap;

            // Panel Data Base
            PanelDataBase.Parent = PanelWorkspace;
            PanelDataBase.Dock = DockStyle.Top;
            PanelDataBase.MinimumSize = new Size(300, 200);

            // Panel Account Base
            PanelAccountBase.Parent = PanelDataBase;
            PanelAccountBase.Dock = DockStyle.Fill;
            PanelAccountBase.MinimumSize = new Size(100, 100);

            // Splitter Strategy / Account
            new Splitter {Parent = PanelDataBase, Dock = DockStyle.Left, Width = Gap};

            // Panel pnlStrategyBase
            PanelStrategyBase.Parent = PanelDataBase;
            PanelStrategyBase.Dock = DockStyle.Left;
            PanelStrategyBase.MinimumSize = new Size(100, 100);

            // Splitter Market / Strategy
            new Splitter {Parent = PanelDataBase, Dock = DockStyle.Left, Width = Gap};

            // Panel Market Base
            PanelMarketBase.Parent = PanelDataBase;
            PanelMarketBase.Dock = DockStyle.Left;
            PanelMarketBase.MinimumSize = new Size(100, 100);

            // Market panel
            PanelMarket.Parent = PanelMarketBase;
            PanelMarket.Dock = DockStyle.Fill;
            PanelMarket.Padding = new Padding(Gap, Gap, 0, 0);
            ToolStripMarket.Parent = PanelMarketBase;
            ToolStripMarket.Dock = DockStyle.Top;

            // Strategy panel
            PanelStrategy.Parent = PanelStrategyBase;
            PanelStrategy.Dock = DockStyle.Fill;
            PanelStrategy.Padding = new Padding(0, Gap, 0, 0);
            ToolStripStrategy.Parent = PanelStrategyBase;
            ToolStripStrategy.Dock = DockStyle.Top;

            // Account panel
            PanelAccount.Parent = PanelAccountBase;
            PanelAccount.Dock = DockStyle.Fill;
            PanelAccount.Padding = new Padding(0, Gap, Gap, 0);
            ToolStripAccount.Parent = PanelAccountBase;
            ToolStripAccount.Dock = DockStyle.Top;

            // Journal panel
            PanelJournal.Parent = PanelJournalBase;
            PanelJournal.Dock = DockStyle.Fill;

            // Strategy Directory FileSystemWatcher
            _strategyDirWatcher.Path = Data.StrategyDir;
            _strategyDirWatcher.Created += StrategyDirWatcherCreated;
        }

        /// <summary>
        /// Calculates the size of base panels.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            StatusBarStrip.Visible = Configs.ShowStatusBar;

            base.OnResize(e);

            PanelJournalBase.Visible = Configs.ShowJournal;
            PanelDataBase.Height = Configs.ShowJournal ? (int) (PanelWorkspace.ClientSize.Height*0.630) : PanelWorkspace.ClientSize.Height - Gap;
            SplitterJournal.Enabled = Configs.ShowJournal;
            PanelMarketBase.Width = PanelDataBase.ClientSize.Width/3;
            PanelStrategyBase.Width = PanelDataBase.ClientSize.Width/3;
        }

        private void WorkspaceDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void WorkspaceDragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop, true);
            string filePath = files[0];
            LoadDroppedStrategy(filePath);
        }

        protected virtual void LoadDroppedStrategy(string filePath)
        {
        }

        /// <summary>
        /// Set the status of the Strategy Directory FileSystemWatcher
        /// </summary>
        internal void SetStrategyDirWatcher()
        {
            _strategyDirWatcher.EnableRaisingEvents = Configs.StrategyDirWatch;
        }

        /// <summary>
        /// Strategy Directory FileSystemWatcher Event Handler
        /// </summary>
        private void StrategyDirWatcherCreated(object sender, FileSystemEventArgs e)
        {
            _strategyDirWatcher.EnableRaisingEvents = false;
            Thread.Sleep(1000);
            LoadDroppedStrategy(e.FullPath);
            _strategyDirWatcher.EnableRaisingEvents = Configs.StrategyDirWatch;
        }
    }
}