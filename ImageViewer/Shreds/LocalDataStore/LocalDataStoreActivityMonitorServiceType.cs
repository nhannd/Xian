using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class LocalDataStoreActivityMonitorServiceType : ILocalDataStoreActivityMonitorService
	{
		public LocalDataStoreActivityMonitorServiceType()
		{
			Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: LocalDataStoreActivityMonitorServiceType Constructor");
		}

		#region ILocalDataStoreActivityMonitorService Members

		public void Subscribe(string eventName)
		{
			LocalDataStoreActivityMonitor.Instance.Subscribe(eventName);
		}

		public void Unsubscribe(string eventName)
		{
			LocalDataStoreActivityMonitor.Instance.Unsubscribe(eventName);
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			LocalDataStoreActivityMonitor.Instance.Cancel(information);
		}

		public void ClearInactive()
		{
			LocalDataStoreActivityMonitor.Instance.ClearInactive();
		}

		public void Refresh()
		{
			LocalDataStoreActivityMonitor.Instance.Refresh();
		}

		#endregion
	}
}
