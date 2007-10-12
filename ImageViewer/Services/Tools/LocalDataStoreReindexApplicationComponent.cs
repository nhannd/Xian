#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public class LocalDataStoreReindexApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(LocalDataStoreReindexApplicationComponentViewExtensionPoint))]
	public class LocalDataStoreReindexApplicationComponent : ApplicationComponent
	{
		private ReindexProgressItem _reindexProgress;

		private string _statusMessage;
		private int _totalProcessed;
		private int _totalToProcess;
		private int _failedSteps;
		private int _availableCount;
		private bool _cancelEnabled;

		public LocalDataStoreReindexApplicationComponent()
		{
			_reindexProgress = new ReindexProgressItem();
		}

		public string Title
		{
			get { return SR.TitleReindexLocalDataStore; }
		}

		private void OnConnected(object sender, EventArgs e)
		{
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			this.StatusMessage = SR.MessageActivityMonitorServiceUnavailable;
			this.TotalProcessed = 0;
			this.TotalToProcess = 0;
			this.AvailableCount = 0;
			this.FailedSteps = 0;
			this.CancelEnabled = false;
		}

		private void OnReindexProgressUpdate(object sender, ItemEventArgs<ReindexProgressItem> e)
		{
			_reindexProgress.CopyFrom(e.Item);

			this.StatusMessage = _reindexProgress.StatusMessage;
			this.TotalProcessed = _reindexProgress.TotalImportsProcessed;
			this.TotalToProcess = _reindexProgress.TotalFilesToImport;
			this.AvailableCount = _reindexProgress.NumberOfFilesCommittedToDataStore;
			this.FailedSteps = _reindexProgress.TotalDataStoreCommitFailures;
			this.CancelEnabled = (_reindexProgress.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel;
		}

		public override void Start()
		{
			base.Start();

			LocalDataStoreActivityMonitor.Instance.LostConnection += new EventHandler(OnLostConnection);
			LocalDataStoreActivityMonitor.Instance.Connected += new EventHandler(OnConnected);
			LocalDataStoreActivityMonitor.Instance.ReindexProgressUpdate += new EventHandler<ItemEventArgs<ReindexProgressItem>>(OnReindexProgressUpdate);

			if (!LocalDataStoreActivityMonitor.Instance.IsConnected)
				this.OnLostConnection(null, null);
		}

		public override void Stop()
		{
			base.Stop();

			LocalDataStoreActivityMonitor.Instance.LostConnection -= new EventHandler(OnLostConnection);
			LocalDataStoreActivityMonitor.Instance.Connected -= new EventHandler(OnConnected);
			LocalDataStoreActivityMonitor.Instance.ReindexProgressUpdate -= new EventHandler<ItemEventArgs<ReindexProgressItem>>(OnReindexProgressUpdate);
		}

		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			protected set
			{
				if (value == _statusMessage)
					return;

				_statusMessage = value;
				this.NotifyPropertyChanged("StatusMessage");
			}
		}

		public int TotalProcessed
		{
			get
			{
				return _totalProcessed;
			}
			protected set
			{
				if (value == _totalProcessed)
					return;

				_totalProcessed = value;
				this.NotifyPropertyChanged("TotalProcessed");
			}
		}

		public int TotalToProcess
		{
			get
			{
				return _totalToProcess;
			}
			protected set
			{
				if (value == _totalToProcess)
					return;

				_totalToProcess = value;
				this.NotifyPropertyChanged("TotalToProcess");
			}
		}

		public int AvailableCount
		{
			get
			{
				return _availableCount;
			}
			protected set
			{
				if (value == _availableCount)
					return;

				_availableCount = value;
				this.NotifyPropertyChanged("AvailableCount");
			}
		}

		public int FailedSteps
		{
			get
			{
				return _failedSteps;
			}
			protected set
			{
				if (value == _failedSteps)
					return;

				_failedSteps = value;
				this.NotifyPropertyChanged("FailedSteps");
			}
		}

		public bool CancelEnabled
		{
			get { return _cancelEnabled; }
			protected set
			{
				if (value == _cancelEnabled)
					return;

				_cancelEnabled = value;
				this.NotifyPropertyChanged("CancelEnabled");
			}
		}

		public void Cancel()
		{
			if (Platform.ShowMessageBox(SR.MessageConfirmCancelReindex, MessageBoxActions.YesNo) == DialogBoxAction.No)
				return;

			List<Guid> progressIdentifiers = new List<Guid>();
			progressIdentifiers.Add(_reindexProgress.Identifier);

			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = CancellationFlags.Cancel;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Instance.Cancel(cancelInformation);
		}
	}
}
