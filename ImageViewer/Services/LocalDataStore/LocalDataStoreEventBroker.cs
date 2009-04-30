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
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public interface ILocalDataStoreEventBroker : IDisposable
	{
		event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate;
		event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate;
		event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate;
		event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate;
		event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> SopInstanceImported;
		event EventHandler<ItemEventArgs<DeletedInstanceInformation>> InstanceDeleted;
		event EventHandler LocalDataStoreCleared;
		event EventHandler LostConnection;
		event EventHandler Connected;

	}

	internal class LocalDataStoreEventBroker : ILocalDataStoreEventBroker
	{
		private readonly SynchronizationContext _synchronizationContext;
		private bool _disposed = false;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler<ItemEventArgs<DeletedInstanceInformation>> _instanceDeleted;
		private event EventHandler _localDataStoreCleared;
		private event EventHandler _lostConnection;
		private event EventHandler _connected;

		public LocalDataStoreEventBroker(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;
		}

		#region ILocalDataStoreActivityMonitorProxy Members

		public event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate
		{
			add
			{
				CheckIsDisposed();
				if (_sendProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.SendProgressUpdate += OnSendProgressUpdate;

				_sendProgressUpdate += value;
			}
			remove
			{
				CheckIsDisposed();
				_sendProgressUpdate -= value;

				if (_sendProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.SendProgressUpdate -= OnSendProgressUpdate;
			}
		}

		public event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate
		{
			add
			{
				CheckIsDisposed();
				if (_receiveProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate += OnReceiveProgressUpdate;

				_receiveProgressUpdate += value;
			}
			remove
			{
				CheckIsDisposed();
				_receiveProgressUpdate -= value;

				if (_receiveProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate -= OnReceiveProgressUpdate;
			}
		}

		public event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate
		{
			add
			{
				CheckIsDisposed();
				if (_receiveProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ImportProgressUpdate += OnImportProgressUpdate;

				_importProgressUpdate += value;
			}
			remove
			{
				CheckIsDisposed();
				_importProgressUpdate -= value;

				if (_importProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ImportProgressUpdate -= OnImportProgressUpdate;
			}
		}

		public event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate
		{
			add
			{
				CheckIsDisposed();
				if (_reindexProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ReindexProgressUpdate += OnReindexProgressUpdate;

				_reindexProgressUpdate += value;
			}
			remove
			{
				CheckIsDisposed();
				_reindexProgressUpdate -= value;

				if (_reindexProgressUpdate == null)
					LocalDataStoreActivityMonitor.Instance.ReindexProgressUpdate -= OnReindexProgressUpdate;
			}
		}

		public event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> SopInstanceImported
		{
			add
			{
				CheckIsDisposed();
				if (_sopInstanceImported == null)
					LocalDataStoreActivityMonitor.Instance.SopInstanceImported += OnSopInstanceImported;

				_sopInstanceImported += value;
			}
			remove
			{
				CheckIsDisposed();
				_sopInstanceImported -= value;

				if (_sopInstanceImported == null)
					LocalDataStoreActivityMonitor.Instance.SopInstanceImported -= OnSopInstanceImported;
			}
		}

		public event EventHandler<ItemEventArgs<DeletedInstanceInformation>> InstanceDeleted
		{
			add
			{
				CheckIsDisposed();
				if (_instanceDeleted == null)
					LocalDataStoreActivityMonitor.Instance.InstanceDeleted += OnInstanceDeleted;

				_instanceDeleted += value;
			}
			remove
			{
				CheckIsDisposed();
				_instanceDeleted -= value;

				if (_instanceDeleted == null)
					LocalDataStoreActivityMonitor.Instance.InstanceDeleted -= OnInstanceDeleted;
			}
		}

		public event EventHandler LocalDataStoreCleared
		{
			add
			{
				CheckIsDisposed();
				if (_localDataStoreCleared == null)
					LocalDataStoreActivityMonitor.Instance.LocalDataStoreCleared += OnLocalDataStoreCleared;

				_localDataStoreCleared += value;
			}
			remove
			{
				CheckIsDisposed();
				_localDataStoreCleared -= value;

				if (_localDataStoreCleared == null)
					LocalDataStoreActivityMonitor.Instance.LocalDataStoreCleared -= OnLocalDataStoreCleared;
			}
		}

		public event EventHandler LostConnection
		{
			add
			{
				CheckIsDisposed();
				if (_lostConnection == null)
					LocalDataStoreActivityMonitor.Instance.LostConnection += OnLostConnection;

				_lostConnection += value;
			}
			remove
			{
				CheckIsDisposed();
				_lostConnection -= value;

				if (_lostConnection == null)
					LocalDataStoreActivityMonitor.Instance.LostConnection -= OnLostConnection;
			}
		}

		public event EventHandler Connected
		{
			add
			{
				CheckIsDisposed();
				if (_connected == null)
					LocalDataStoreActivityMonitor.Instance.Connected += OnConnected;

				_connected += value;
			}
			remove
			{
				CheckIsDisposed();
				_connected -= value;

				if (_connected == null)
					LocalDataStoreActivityMonitor.Instance.Connected -= OnConnected;
			}
		}

		#endregion

		#region Private Methods

		private void OnSendProgressUpdate(object sender, ItemEventArgs<SendProgressItem> e)
		{
			FireEvent(_sendProgressUpdate, e);
		}

		private void OnReceiveProgressUpdate(object sender, ItemEventArgs<ReceiveProgressItem> e)
		{
			FireEvent(_receiveProgressUpdate, e);
		}

		private void OnImportProgressUpdate(object sender, ItemEventArgs<ImportProgressItem> e)
		{
			FireEvent(_importProgressUpdate, e);
		}

		private void OnReindexProgressUpdate(object sender, ItemEventArgs<ReindexProgressItem> e)
		{
			FireEvent(_reindexProgressUpdate, e);
		}

		private void OnSopInstanceImported(object sender, ItemEventArgs<ImportedSopInstanceInformation> e)
		{
			FireEvent(_sopInstanceImported, e);
		}

		private void OnInstanceDeleted(object sender, ItemEventArgs<DeletedInstanceInformation> e)
		{
			FireEvent(_instanceDeleted, e);
		}

		private void OnLocalDataStoreCleared(object sender, EventArgs e)
		{
			FireEvent(_localDataStoreCleared, e);
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			FireEvent(_lostConnection, e);
		}

		private void OnConnected(object sender, EventArgs e)
		{
			FireEvent(_connected, e);
		}

		private void FireEvent(Delegate del, EventArgs e)
		{
			if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
			{
				_synchronizationContext.Post(delegate { EventsHelper.Fire(del, this, e); }, null);
			}
			else
			{
				EventsHelper.Fire(del, this, e);
			}
		}

		private void UnsubscribeAll()
		{
			LocalDataStoreActivityMonitor.Instance.SendProgressUpdate -= OnSendProgressUpdate;
			LocalDataStoreActivityMonitor.Instance.ReceiveProgressUpdate -= OnReceiveProgressUpdate;
			LocalDataStoreActivityMonitor.Instance.ImportProgressUpdate -= OnImportProgressUpdate;
			LocalDataStoreActivityMonitor.Instance.ReindexProgressUpdate -= OnReindexProgressUpdate;
			LocalDataStoreActivityMonitor.Instance.SopInstanceImported -= OnSopInstanceImported;
			LocalDataStoreActivityMonitor.Instance.InstanceDeleted -= OnInstanceDeleted;
			LocalDataStoreActivityMonitor.Instance.LocalDataStoreCleared -= OnLocalDataStoreCleared;
			LocalDataStoreActivityMonitor.Instance.LostConnection -= OnLostConnection;
			LocalDataStoreActivityMonitor.Instance.Connected -= OnConnected;
		}

		private void CheckIsDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("The object has already been disposed.");
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				UnsubscribeAll();
			}
		}

		#endregion
	}
}
