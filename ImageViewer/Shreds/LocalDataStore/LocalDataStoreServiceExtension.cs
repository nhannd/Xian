#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ExtensionOf(typeof(ShredExtensionPoint), Enabled = false)]
	public class LocalDataStoreServiceExtension : WcfShred
	{
		private static readonly string _localDataStoreEndpointName = "LocalDataStore";
		private static readonly string _localDataStoreActivityMonitorEndpointName = "LocalDataStoreActivityMonitor";

		private bool _localDataStoreWCFInitialized;
		private bool _localDataStoreActivityMonitorWCFInitialized;

		public LocalDataStoreServiceExtension()
		{
			_localDataStoreWCFInitialized = false;
			_localDataStoreActivityMonitorWCFInitialized = false;
		}

		public override void Start()
		{
			try
			{
				LocalDataStoreService.Instance.Start();
				string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.LocalDataStore);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.LocalDataStore));
				return;
			}

			try
			{
				StartNetPipeHost<LocalDataStoreServiceType, ILocalDataStoreService>(_localDataStoreEndpointName, SR.LocalDataStore);
				_localDataStoreWCFInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.LocalDataStore);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.LocalDataStore));
			}

			try
			{
				StartNetPipeHost<LocalDataStoreActivityMonitorServiceType, ILocalDataStoreActivityMonitorService>(
					_localDataStoreActivityMonitorEndpointName, SR.LocalDataStoreActivityMonitor);
				_localDataStoreActivityMonitorWCFInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.LocalDataStoreActivityMonitor);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.LocalDataStoreActivityMonitor));
			}
		}

		public override void Stop()
		{
			if (_localDataStoreWCFInitialized)
			{
				try
				{
					StopHost(_localDataStoreEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.LocalDataStore));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			if (_localDataStoreActivityMonitorWCFInitialized)
			{
				try
				{
					StopHost(_localDataStoreActivityMonitorEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.LocalDataStoreActivityMonitor));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			try
			{
				LocalDataStoreService.Instance.Stop();
				Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.LocalDataStore));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		public override string GetDisplayName()
		{
			return SR.LocalDataStore;
		}

		public override string GetDescription()
		{
			return SR.LocalDataStoreDescription;
		}
	}
}
