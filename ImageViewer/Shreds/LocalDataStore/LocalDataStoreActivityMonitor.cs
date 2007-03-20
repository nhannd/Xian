using System;
using System.Collections.Generic;
using System.Text;
using ServiceModelEx;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorService
	{
		private class InternalSubscriptionManager : SubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>
		{
			public InternalSubscriptionManager()
			{ 
			}
		}

		private static LocalDataStoreActivityMonitor _instance;

		private InternalSubscriptionManager _subscriptionManager;

		private LocalDataStoreActivityMonitor()
		{
			_subscriptionManager = new InternalSubscriptionManager();
		}

		public static LocalDataStoreActivityMonitor Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LocalDataStoreActivityMonitor();

				return _instance;
			}
		}

		#region ILocalDataStoreActivityMonitorService Members

		public void Cancel(CancelProgressItemInformation information)
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		public void ClearInactive()
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		public void Refresh()
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		#endregion

		#region ISubscriptionService Members

		public void Subscribe(string eventOperation)
		{
			_subscriptionManager.Subscribe(eventOperation);
		}

		public void Unsubscribe(string eventOperation)
		{
			_subscriptionManager.Unsubscribe(eventOperation);
		}

		#endregion
	}
}
