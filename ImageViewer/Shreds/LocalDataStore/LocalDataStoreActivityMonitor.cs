using System;
using System.Collections.Generic;
using System.Text;
using ServiceModelEx;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityMonitor : SubscriptionManager<ILocalDataStoreActivityMonitorService>, ILocalDataStoreActivityMonitorService
	{
		private static LocalDataStoreActivityMonitor _instance;

		private LocalDataStoreActivityMonitor()
		{ 
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
			base.Subscribe(eventOperation);
		}

		public void Unsubscribe(string eventOperation)
		{
			base.Unsubscribe(eventOperation);
		}

		#endregion
	}
}
