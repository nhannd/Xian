#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public sealed class LocalDataStoreActivityMonitorServiceType : ILocalDataStoreActivityMonitorService, IDisposable
	{
		private readonly ILocalDataStoreActivityMonitorServiceCallback _callback;

		public LocalDataStoreActivityMonitorServiceType()
		{
			_callback = OperationContext.Current.GetCallbackChannel<ILocalDataStoreActivityMonitorServiceCallback>();
		}

		#region ILocalDataStoreActivityMonitorService Members

		public void Subscribe(string eventName)
		{
			try
			{
				SubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>.Subscribe(_callback, eventName);
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
				SubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>.Unsubscribe(_callback, eventName);
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
				LocalDataStoreService.Instance.Cancel(information);
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
				LocalDataStoreService.Instance.ClearInactive();
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
				LocalDataStoreService.Instance.RepublishAll();
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
			try
			{
				SubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>.Unsubscribe(_callback, null);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
