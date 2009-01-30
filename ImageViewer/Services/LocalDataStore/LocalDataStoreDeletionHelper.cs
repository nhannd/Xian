#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	//TODO: should probably be an instantiable class.
	public static class LocalDataStoreDeletionHelper
	{
		[Serializable]
		public class UnableToConnectException : CommunicationException
		{
			internal UnableToConnectException(string message)
				: base(message)
			{
			}

			internal UnableToConnectException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}

		[Serializable]
		public class ConnectionLostException : CommunicationException
		{
			internal ConnectionLostException(string message)
				: base(message)
			{
			}

			internal ConnectionLostException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}

		public class DeletionProgressInformation
		{
			private TimeSpan _elapsedTime;
			private int _numberDeleted;
			private int _numberRemaining;
			private string _lastDeletedUid;
			private string _lastErrorMessage;

			internal DeletionProgressInformation()
			{ 
			}
			
			public TimeSpan ElapsedTime 
			{
				get { return _elapsedTime;  }
				internal set { _elapsedTime = value;  }
			}

			public int NumberDeleted
			{
				get { return _numberDeleted; }
				internal set { _numberDeleted = value; }
			}

			public int NumberRemaining
			{
				get { return _numberRemaining; }
				internal set { _numberRemaining = value; }
			}

			public string LastDeletedUid
			{
				get { return _lastDeletedUid; }
				internal set { _lastDeletedUid = value; }
			}

			public string LastErrorMessage
			{
				get { return _lastErrorMessage; }
				internal set { _lastErrorMessage = value; }
			}
		}

		public delegate bool DeletionProgressCallback(DeletionProgressInformation progressInformation);

		[CallbackBehavior(UseSynchronizationContext = false)]
		private class WaitDeleteInstancesCallback : ILocalDataStoreActivityMonitorServiceCallback
		{
			private object _waitLock = new object();
			private InstanceLevel _instanceLevel;
			private Dictionary<string, string> _waitDeleteInstanceUids;
			private int _startingNumber;
			
			private string _lastDeletedUid;
			private string _lastErrorMessage;

			private Exception _error;

			public WaitDeleteInstancesCallback(DeleteInstancesRequest request)
			{
				_instanceLevel = request.InstanceLevel;
				_waitDeleteInstanceUids = new Dictionary<string, string>();
				foreach(string instanceUid in request.InstanceUids)
					_waitDeleteInstanceUids[instanceUid] = instanceUid;

				_startingNumber = _waitDeleteInstanceUids.Count;
			}

			#region ILocalDataStoreActivityMonitorServiceCallback Members

			public void ReceiveProgressChanged(ReceiveProgressItem progressItem) { }
			public void SendProgressChanged(SendProgressItem progressItem) { }
			public void ImportProgressChanged(ImportProgressItem progressItem) { }
			public void ReindexProgressChanged(ReindexProgressItem progressItem) { }
			public void SopInstanceImported(ImportedSopInstanceInformation information) { }
			public void LocalDataStoreCleared() { }

			public void InstanceDeleted(DeletedInstanceInformation information)
			{
				if (information.InstanceLevel != _instanceLevel || String.IsNullOrEmpty(information.InstanceUid))
					return;

				lock (_waitLock)
				{ 
					_waitDeleteInstanceUids.Remove(information.InstanceUid);
					_lastDeletedUid = information.InstanceUid;
					_lastErrorMessage = information.ErrorMessage;

					Monitor.Pulse(_waitLock);
				}
			}

			#endregion

			public void OnLostConnection(object sender, EventArgs e)
			{
				lock (_waitLock)
				{
					_waitDeleteInstanceUids.Clear();
					_error = new ConnectionLostException("The connection to Local Data Store Activity Monitor has been lost.");
					Monitor.Pulse(_waitLock);
				}
			}

			public void Wait(int callbackPeriod, DeletionProgressCallback progressCallback)
			{
				DateTime startTime = Platform.Time;

				DeletionProgressInformation information = new DeletionProgressInformation();
 
				lock (_waitLock)
				{
					while(_waitDeleteInstanceUids.Count > 0)
					{
						Monitor.Wait(_waitLock, callbackPeriod);
						
						if (_error != null)
							throw _error;

						information.ElapsedTime = Platform.Time - startTime;
						information.NumberRemaining = _waitDeleteInstanceUids.Count;
						information.NumberDeleted = _startingNumber - information.NumberRemaining;
						information.LastDeletedUid = _lastDeletedUid;
						information.LastErrorMessage = _lastErrorMessage;

						if (!progressCallback(information))
							break;
					}						
				}
			}
		}

		private static void SendDeleteRequest(DeleteInstancesRequest request)
		{
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();

			try
			{
				client.Open();

				const int batchSize = 500;

				DeleteInstancesRequest batchRequest = new DeleteInstancesRequest();
				batchRequest.DeletePriority = request.DeletePriority;
				batchRequest.InstanceLevel = request.InstanceLevel;
				List<string> allUids = new List<string>(request.InstanceUids);
				List<string> batchUids = new List<string>();

				while (allUids.Count > 0)
				{
					batchUids.Add(allUids[0]);
					allUids.RemoveAt(0);

					if (batchUids.Count == batchSize || allUids.Count == 0)
					{
						batchRequest.InstanceUids = batchUids;
						client.DeleteInstances(batchRequest);
						batchUids.Clear();
					}
				}

				client.Close();
				
			}
			catch (Exception e)
			{
				client.Abort();
				throw new UnableToConnectException("Unable to connect to the Local Data Store service.", e);
			}
		}

		public static void DeleteInstancesNoWait(DeleteInstancesRequest request)
		{
			SendDeleteRequest(request);
		}

		public static void DeleteInstancesAndWait(DeleteInstancesRequest request, int callbackPeriod, DeletionProgressCallback progressCallback)
		{
			Platform.CheckPositive(callbackPeriod, "callbackPeriod");
			Platform.CheckForNullReference(progressCallback, "progressCallback");
 
			WaitDeleteInstancesCallback callback = new WaitDeleteInstancesCallback(request);
			LocalDataStoreActivityMonitorServiceClient monitor = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(callback));
			try
			{
				monitor.Open();
				monitor.Subscribe("InstanceDeleted");

				monitor.InnerChannel.Faulted += new EventHandler(callback.OnLostConnection);
				monitor.InnerChannel.Closed += new EventHandler(callback.OnLostConnection);
			}
			catch (Exception e)
			{
				monitor.Abort();
				throw new UnableToConnectException("Unable to connect to the Local Data Store Activity Monitor service.", e);
			}

			try
			{
				SendDeleteRequest(request);
				callback.Wait(callbackPeriod, progressCallback);
			}
			catch
			{
				throw;
			}
			finally
			{
				try
				{
					monitor.InnerChannel.Faulted -= new EventHandler(callback.OnLostConnection);
					monitor.InnerChannel.Closed -= new EventHandler(callback.OnLostConnection);

					monitor.Unsubscribe("InstanceDeleted");
					monitor.Close();
				}
				catch
				{
					// The call to callback.Wait will throw a "Lost Connection" exception,
					// so we'll just ignore this because an exception is already being thrown.
					monitor.Abort();
				}
			}
		}
	}
}
