using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class LocalDataStoreActivityMonitorServiceType : ILocalDataStoreActivityMonitorService, IDisposable
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
				Platform.Log(LogLevel.Error, e);
				string message = String.Format("{0}\nDetail: {1}", SR.ExceptionFailedToAddSubscriber, e.Message);
				//in the unlikely event of an exception, throw a FaultException, so that the client channel doesn't get closed.
				throw new FaultException(message);
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
				Platform.Log(LogLevel.Error, e); 
				string message = String.Format("{0}\nDetail: {1}", SR.ExceptionFailedToRemoveSubscriber, e.Message);
				//in the unlikely event of an exception, throw a FaultException, so that the client channel doesn't get closed.
				throw new FaultException(message);
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
				Platform.Log(LogLevel.Error, new Exception(message, e));

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
				Platform.Log(LogLevel.Error, new Exception(message, e));
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
				Platform.Log(LogLevel.Error, new Exception(message, e));
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			string message = "Local Data Store Activity Monitor session object disposed";
			Console.WriteLine(message);
			Platform.Log(LogLevel.Info, message);
		}

		#endregion
	}
}
