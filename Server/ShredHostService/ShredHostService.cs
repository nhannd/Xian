using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHostService
{
    public partial class ShredHostService : ServiceBase
    {
        public ShredHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // the default startup path is in the system folder
            // we need to change this to be able to scan for plugins and to log
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.SetCurrentDirectory(startupPath);

            ShredHost.ShredHost.Start();
        }

        protected override void OnStop()
        {
            ShredHost.ShredHost.Stop();
        }
    }
}
