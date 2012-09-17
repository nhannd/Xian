#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.ServiceProcess;

namespace ClearCanvas.Server.ShredHostService
{
	public partial class ShredHostService : ServiceBase
	{
		private const int _serviceOperationTimeout = 3*60*1000;

		internal static void InternalStart()
		{
			// the default startup path is in the system folder
			// we need to change this to be able to scan for plugins and to log
			string startupPath = AppDomain.CurrentDomain.BaseDirectory;
			Directory.SetCurrentDirectory(startupPath);
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
			RequestAdditionalTime(_serviceOperationTimeout);

			InternalStart();
		}

		protected override void OnStop()
		{
			RequestAdditionalTime(_serviceOperationTimeout);

			InternalStop();
		}
	}
}