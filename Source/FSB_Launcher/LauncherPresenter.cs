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
using System.IO;
using FSB_Launcher.Interfaces;

namespace FSB_Launcher
{
    public class LauncherPresenter : ILauncherPresenter
    {
        private readonly IIoManager ioManager;
        private readonly ITimeHelper timeHelper;
        private ILauncherForm view;

        public LauncherPresenter(IIoManager ioManager,
                                 ITimeHelper timeHelper)
        {
            if (ioManager == null) throw new ArgumentNullException("ioManager");
            if (timeHelper == null) throw new ArgumentNullException("timeHelper");
            this.ioManager = ioManager;
            this.timeHelper = timeHelper;

            timeHelper.CountDownElapsed += TimeHelper_CountDownElapsed;
        }

        public void SetView(ILauncherForm launcherForm)
        {
            if (launcherForm == null) throw new ArgumentNullException("launcherForm");
            view = launcherForm;
        }

        public void Proceede()
        {
            timeHelper.StartCountDown(4);
            StartApplication();
        }

        public void ManageIncomingMassage(string messageText)
        {
            view.UpdateStatus(messageText);
        }

        public void VisitWebsite()
        {
            ioManager.VisitWebLink(@"http:\\forexsb.com\");
        }

        private void TimeHelper_CountDownElapsed(object sender, EventArgs e)
        {
            view.Close();
        }

        private void StartApplication()
        {
            view.UpdateStatus("Starting Forex Strategy Builder...");

            string path = Path.Combine(ioManager.CurrentDirectory, @"Forex Strategy Builder.exe");
            if (ioManager.FileExists(path))
                ioManager.RunFile(path, "");
            else
                view.UpdateStatus("Error: cannot find FSB!");
        }
    }
}