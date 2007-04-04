using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class LocalDataStoreActivityMonitorServiceType : ILocalDataStoreActivityMonitorService
	{
		public LocalDataStoreActivityMonitorServiceType()
		{
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
				string message = String.Format("{0}\nDetail: {1}", SR.ExceptionFailedToAddSubscriber, e.Message);
				throw new LocalDataStoreFaultException(message);
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
				string message = String.Format("{0}\nDetail: {1}", SR.ExceptionFailedToAddSubscriber, e.Message);
				throw new LocalDataStoreFaultException(message);
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
				string message = SR.ExceptionCancellationOfAtLeastOneItemFailed;
				//this is a one-way operation, so you can't throw.
				Platform.Log(new Exception(message, e));

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
				string message = SR.ExceptionErrorWhileAttemptingToClearInactiveItems;
				//this is a one-way operation, so you can't throw.
				Platform.Log(new Exception(message, e));
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
				string message = SR.ExceptionErrorAttemptingToRefresh;
				//this is a one-way operation, so you can't throw.
				Platform.Log(new Exception(message, e));
			}
		}

		#endregion
	}
}
