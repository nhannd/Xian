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
using System.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class LocalDataStoreEventPublisher
	{
		private class PublishThreadPool : BlockingThreadPool<object>
		{
			public PublishThreadPool()
				: base(1, false)
			{
			}

			private static void Publish(object item)
			{
				LocalDataStoreServiceClient proxy = new LocalDataStoreServiceClient();

				try
				{
					proxy.Open();

					if (item is StoreScpReceivedFileInformation)
					{
						proxy.FileReceived(item as StoreScpReceivedFileInformation);
					}
					else if (item is StoreScuSentFileInformation)
					{
						proxy.FileSent(item as StoreScuSentFileInformation);
					}
					if (item is RetrieveStudyInformation)
					{
						proxy.RetrieveStarted(item as RetrieveStudyInformation);
					}
					else if (item is SendStudyInformation)
					{
						proxy.SendStarted(item as SendStudyInformation);
					}
					else if (item is ReceiveErrorInformation)
					{
						proxy.ReceiveError(item as ReceiveErrorInformation);
					}
					else if (item is SendErrorInformation)
					{
						proxy.SendError(item as SendErrorInformation);
					}

					proxy.Close();
				}
				catch (Exception e)
				{
					proxy.Abort();
					LogPublishError(item, e);
				}
			}

			private static void LogPublishError(object item, Exception e)
			{
				if (item is RetrieveStudyInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify the local data store service that a retrieve operation has begun.");
				}
				else if (item is SendStudyInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify the local data store service that a send operation has begun.");
				}
				else if (item is StoreScpReceivedFileInformation)
				{
					StoreScpReceivedFileInformation receivedFileInformation = (StoreScpReceivedFileInformation)item;
					String message = String.Format(
						"Failed to notify local data store service that a file has been received ({0})." +
						"  The file must be imported manually.", receivedFileInformation.FileName);

					Platform.Log(LogLevel.Warn, e, message);
				}
				else if (item is StoreScuSentFileInformation)
				{
					StoreScuSentFileInformation sentFileInformation = (StoreScuSentFileInformation)item;
					string message =
						String.Format("Failed to notify local data store service that a file has been sent ({0}).",
						sentFileInformation.FileName);

					Platform.Log(LogLevel.Warn, e, message);
				}
				else if (item is SendErrorInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify local data store service that a send error occurred.");
				}
				else if (item is ReceiveErrorInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify local data store service that a receive error occurred.");
				}
			}

			protected override void ProcessItem(object item)
			{
				Publish(item);
			}
		}

		public static readonly LocalDataStoreEventPublisher Instance = new LocalDataStoreEventPublisher();

		private readonly PublishThreadPool _pool;

		private LocalDataStoreEventPublisher()
		{
			_pool = new PublishThreadPool();
		}

		private void AddToQueue(object item)
		{
			_pool.Enqueue(item);
		}

		public void Start()
		{
			_pool.Start();
		}

		public void Stop()
		{
			_pool.Stop();
		}

		public void FileSent(StoreScuSentFileInformation info)
		{
			AddToQueue(info);
		}

		public void FileReceived(StoreScpReceivedFileInformation info)
		{
			AddToQueue(info);
		}

		public void SendStarted(SendStudyInformation info)
		{
			AddToQueue(info);
		}

		public void RetrieveStarted(RetrieveStudyInformation info)
		{
			AddToQueue(info);
		}

		public void SendError(SendErrorInformation info)
		{
			AddToQueue(info);
		}

		public void ReceiveError(ReceiveErrorInformation info)
		{
			AddToQueue(info);
		}
	}
}
