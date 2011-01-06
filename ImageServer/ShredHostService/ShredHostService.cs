#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.ShredHostService
{
    public partial class ShredHostService : ServiceBase
    {
        private static Assembly _assembly;
        private static Type _shredHostType;

        internal static void InternalStart()
        {
            Platform.Log(LogLevel.Info, "Starting Server Shred Host Service...");

            ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
                                 SR.AlertComponentName, AlertTypeCodes.Starting,
                                 null, TimeSpan.Zero,
                                 SR.AlertShredHostServiceStarting);

            // the default startup path is in the system folder
            // we need to change this to be able to scan for plugins and to log
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.SetCurrentDirectory(startupPath);

            // we choose to dynamically load the ShredHost dll so that we can bypass
            // the requirement that the ShredHost dll be Strong Name signed, i.e.
            // if we were to reference it directly in the the project at design time
            // ClearCanvas.Server.ShredHost.dll would also need to be Strong Name signed
            _assembly = Assembly.Load("ClearCanvas.Server.ShredHost");
            _shredHostType = _assembly.GetType("ClearCanvas.Server.ShredHost.ShredHost");
            _shredHostType.InvokeMember("Start", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,null, null, new object[] { });
        }

        internal static void InternalStop()
        {
            _shredHostType.InvokeMember("Stop", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
                null, null, new object[] { });


            ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
                                 SR.AlertComponentName, AlertTypeCodes.Stopped,
                                 null, TimeSpan.Zero,
                                 SR.AlertShredHostServiceStopped);
        }

        public ShredHostService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            InternalStart();
        }

        protected override void OnStop()
        {
            InternalStop();
        }
    }
}
