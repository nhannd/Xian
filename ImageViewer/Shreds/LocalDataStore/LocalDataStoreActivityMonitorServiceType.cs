using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class LocalDataStoreActivityMonitorServiceType : ILocalDataStoreActivityMonitorService
	{
		public LocalDataStoreActivityMonitorServiceType()
		{
			Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: LocalDataStoreActivityMonitorServiceType Constructor");
		}

		#region ILocalDataStoreActivityMonitorService Members

		public void Subscribe(string eventName)
		{
			try
			{
				LocalDataStoreActivityMonitor.Instance.Subscribe(eventName);
			}
			catch (Exception e)
			{
				string message = "Failed to add subscriber to the Local DataStore Activity Monitor service.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void Unsubscribe(string eventName)
		{
			try
			{
				LocalDataStoreActivityMonitor.Instance.Unsubscribe(eventName);
			}
			catch (Exception e)
			{
				string message = "Failed to remove subscriber from the Local DataStore Activity Monitor service.";
				throw new LocalDataStoreFaultException(message, e);
			}

		}

		public void Cancel(CancelProgressItemInformation information)
		{
			try
			{		
				LocalDataStoreActivityMonitor.Instance.Cancel(information);
			}
			catch (Exception e)
			{
				string message = "Cancellation of at least one of the specified items has failed.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void ClearInactive()
		{
			try
			{
				LocalDataStoreActivityMonitor.Instance.ClearInactive();
			}
			catch (Exception e)
			{
				string message = "An error occurred while attempting to clear the inactive items.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void Refresh()
		{
			try
			{
				LocalDataStoreActivityMonitor.Instance.Refresh();
			}
			catch (Exception e)
			{
				string message = "An error occurred while attempting to refresh the Local Data Store activities.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		#endregion
	}
}
