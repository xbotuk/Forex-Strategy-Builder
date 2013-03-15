//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Drawing;
using System.Windows.Forms;
using FSB_Launcher.Helpers;
using FSB_Launcher.Interfaces;

namespace FSB_Launcher
{
    public sealed partial class LauncherForm : Form, ILauncherForm
    {
        private readonly ILauncherPresenter presenter;
        private const int WmCopydata = 0x4A;

        private Size? mouseGrabOffset;

        public LauncherForm()
        {
            InitializeComponent();

            Color backColor = ColorTranslator.FromHtml("#007049");
            Color foreColor = ColorTranslator.FromHtml("#E3DEEB");

            BackColor = backColor;
            tbxOutput.BackColor = backColor;

            lblApplicationName.ForeColor = foreColor;
            lblCompany.ForeColor = foreColor;
            linkWebsite.LinkColor = foreColor;
            tbxOutput.ForeColor = foreColor;
        }

        public LauncherForm(ILauncherPresenter presenter)
            : this()
        {
            this.presenter = presenter;
        }

        public void UpdateStatus(string record)
        {
            tbxOutput.Invoke((MethodInvoker) (() => tbxOutput.Items.Add(record)));
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WmCopydata)
            {
                var dataStruct = (CopyDataStruct)message.GetLParam(typeof(CopyDataStruct));
                presenter.ManageIncomingMassage(dataStruct.LpData);
            }

            base.WndProc(ref message);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseGrabOffset = new Size(e.Location);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseGrabOffset = null;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseGrabOffset.HasValue)
                Location = Cursor.Position - mouseGrabOffset.Value;
            base.OnMouseMove(e);
        }

        private void FormLauncher_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void LinkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            presenter.VisitWebsite();
        }
    }
}