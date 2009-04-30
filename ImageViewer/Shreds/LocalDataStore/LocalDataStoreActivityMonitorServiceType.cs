#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

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
