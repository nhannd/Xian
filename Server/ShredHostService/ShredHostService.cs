#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceProcess;

namespace ClearCanvas.Server.ShredHostService
{
    public partial class ShredHostService : ServiceBase
    {
		internal static void InternalStart()
		{
			// the default startup path is in the system folder
			// we need to change this to be able to scan for plugins and to log
			string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
			System.IO.Directory.SetCurrentDirectory(startupPath);
			ShredHost.ShredHost.Start();
		}

		internal static void InternalStop()
		{
			ShredHost.ShredHost.Stop();
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
