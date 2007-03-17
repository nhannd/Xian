using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class LocalDataStoreServiceExtension : WcfShred
	{
		private readonly string _className;
		private readonly string _localDataStoreEndpointName;
		private readonly string _localDataStoreActivityMonitorEndpointName;

		public LocalDataStoreServiceExtension()
		{
			_className = this.GetType().ToString();
			_localDataStoreEndpointName = "LocalDataStore";
			_localDataStoreActivityMonitorEndpointName = "LocalDataStoreActivityMonitor";

			System.Diagnostics.Trace.WriteLine(_className + ": constructed");
		}

		public override void Start()
		{
			Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");

			StartNetPipeHost<LocalDataStoreServiceType, ILocalDataStoreService>(_localDataStoreEndpointName, "Local Data Store service");
			StartNetPipeHost<LocalDataStoreActivityMonitorServiceType, ILocalDataStoreActivityMonitorService>(_localDataStoreActivityMonitorEndpointName, "Local Data Store Activity Monitor service");
		}

		public override void Stop()
		{
			StopHost(_localDataStoreEndpointName);
			StopHost(_localDataStoreActivityMonitorEndpointName);

			Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
		}

		public override string GetDisplayName()
		{
			return "Local Data Store services";
		}

		public override string GetDescription()
		{
			return "This shred hosts the Local Data Store services";
		}
	}
}
