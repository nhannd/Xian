using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.Auditing;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public enum RunningState
	{
		Unknown,
		Running,
		NotRunning
	}

	public interface IReindexLocalDataStore : INotifyPropertyChanged, IDisposable
	{
		void Start();
		void Cancel();

		RunningState RunningState { get; }
		bool CanCancel { get; }
		bool CanStart { get; }

		bool Canceled { get; }
		int TotalProcessed { get; }
		int TotalToProcess { get; }
		int AvailableCount { get; }
		int FailedSteps { get; }
		string StatusMessage { get; }
	}

	public class ReindexLocalDataStoreFacade : IReindexLocalDataStore
	{
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;
		private readonly ReindexProgressItem _reindexProgress;

		private RunningState _runningState;
		private bool _canCancel;
		private bool _canStart;
		private string _statusMessage;
		private int _totalProcessed;
		private int _totalToProcess;
		private int _failedSteps;
		private int _availableCount;
		private bool _canceled;
		private event PropertyChangedEventHandler _propertyChanged;

		public ReindexLocalDataStoreFacade()
		{
			_reindexProgress = new ReindexProgressItem();

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.LostConnection += OnLostConnection;
			_localDataStoreEventBroker.ReindexProgressUpdate += OnReindexProgressUpdate;

			StatusMessage = SR.MessageReindexStatusUnknown;
		}

		public void Dispose()
		{
			Dispose(true);
		}
        
        protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _localDataStoreEventBroker == null)
				return;

			_localDataStoreEventBroker.LostConnection -= OnLostConnection;
			_localDataStoreEventBroker.ReindexProgressUpdate -= OnReindexProgressUpdate;
			_localDataStoreEventBroker.Dispose();
			_localDataStoreEventBroker = null;
		}

		private void Reset(bool connectionExists)
		{
			TotalProcessed = 0;
			TotalToProcess = 0;
			AvailableCount = 0;
			FailedSteps = 0;

			CanCancel = false;
			CanStart = connectionExists;
			RunningState = RunningState.Unknown;
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			StatusMessage = SR.MessageActivityMonitorServiceUnavailable;
			Reset(false);
		}

		private void OnReindexFailed()
		{
			StatusMessage = SR.MessageFailedToStartReindex;
			Reset(true);
		}

		private void OnReindexProgressUpdate(object sender, ItemEventArgs<ReindexProgressItem> e)
		{
			_reindexProgress.CopyFrom(e.Item);

			StatusMessage = _reindexProgress.StatusMessage;
			TotalProcessed = _reindexProgress.TotalImportsProcessed;
			TotalToProcess = _reindexProgress.TotalFilesToImport;
			AvailableCount = _reindexProgress.NumberOfFilesCommittedToDataStore;
			FailedSteps = _reindexProgress.TotalDataStoreCommitFailures;

			CanCancel = (_reindexProgress.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel;
			CanStart = (_reindexProgress.AllowedCancellationOperations & CancellationFlags.Clear) == CancellationFlags.Clear && (_reindexProgress.IsComplete() || 
				(_reindexProgress.Cancelled && _reindexProgress.TotalImportsProcessed == _reindexProgress.TotalDataStoreCommitsProcessed));

			RunningState = CanStart ? RunningState.NotRunning : RunningState.Running;
		}

		public void Start()
		{
			if (!CanStart)
				return;

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();
				client.Reindex();
				client.Close();

				CanStart = false;
				CanCancel = false;

				AuditHelper.LogImportStudies(new AuditedInstances(), EventSource.CurrentUser, EventResult.Success);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				OnLostConnection(null, null);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				client.Abort();
				OnReindexFailed();
			}
		}

		public void Cancel()
		{
			if (!CanCancel)
				return;

			List<Guid> progressIdentifiers = new List<Guid>();
			progressIdentifiers.Add(_reindexProgress.Identifier);

			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = CancellationFlags.Cancel;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Cancel(cancelInformation);
		}

		public RunningState RunningState
		{
			get { return _runningState; }
			set
			{
				if (_runningState == value)
					return;

				_runningState = value;
				NotifyPropertyChanged("RunningState");
			}
		}
		
		public bool CanCancel
		{
			get { return _canCancel; }
			private set
			{
				if (_canCancel== value)
					return;

				_canCancel= value;
				NotifyPropertyChanged("CanCancel");
			}
		}

		public bool CanStart
		{
			get { return _canStart; }
			private set
			{
				_canStart = value;
				NotifyPropertyChanged("CanStart");
			}
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
				NotifyPropertyChanged("StatusMessage");
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
				NotifyPropertyChanged("TotalProcessed");
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
				NotifyPropertyChanged("TotalToProcess");
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
				NotifyPropertyChanged("AvailableCount");
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
				NotifyPropertyChanged("FailedSteps");
			}
		}

		public bool Canceled
		{
			get { return _canceled; }
			private set
			{
				if (_canceled == value)
					return;

				_canceled = value;
				NotifyPropertyChanged("Canceled");
			}
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion
	}
}
