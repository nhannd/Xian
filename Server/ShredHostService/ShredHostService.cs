using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Reflection;

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

            // we choose to dynamically load the ShredHost dll so that we can bypass
            // the requirement that the ShredHost dll be Strong Name signed, i.e.
            // if we were to reference it directly in the the project at design time
            // ClearCanvas.Server.ShredHost.dll would also need to be Strong Name signed
            _assembly = Assembly.Load("ClearCanvas.Server.ShredHost");
            _shredHostType = _assembly.GetType("ClearCanvas.Server.ShredHost.ShredHost");
            _shredHostType.InvokeMember("Start", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
                null, null, new object[] { });
        }

        protected override void OnStop()
        {
            _shredHostType.InvokeMember("Stop", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
                null, null, new object[] { });
        }

        private Assembly _assembly;
        private Type _shredHostType;
    }
}
