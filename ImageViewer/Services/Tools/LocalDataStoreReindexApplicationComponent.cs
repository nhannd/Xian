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
