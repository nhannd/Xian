using System;
using System.Collections.Generic;
using System.Text;
using ServiceModelEx;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorService
	{
		private class InternalSubscriptionManager : TransientSubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>
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
			LocalDataStoreService.Instance.Cancel(information);
		}

		public void ClearInactive()
		{
			LocalDataStoreService.Instance.ClearInactive();
		}

		public void Refresh()
		{
			LocalDataStoreService.Instance.RepublishAll();
		}

		#endregion

		#region ISubscriptionService Members

		public void Subscribe(string eventOperation)
		{
			_subscriptionManager.Subscribe(eventOperation);

			LocalDataStoreService.Instance.RepublishAll();
		}

		public void Unsubscribe(string eventOperation)
		{
			_subscriptionManager.Unsubscribe(eventOperation);
		}

		#endregion
	}
}
