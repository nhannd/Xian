using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.IO;
using ClearCanvas.Common;
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService : ILocalDataStoreService
	{
		public enum ServiceState
		{ 
			Importing = 0,
			Deleting,
			Reindexing
		}

		private static LocalDataStoreService _instance;

		private bool _disabled;

		private DicomFileImporter _dicomFileImporter;
		private SentFileProcessor _sentFileProcessor;
		private ReceivedFileProcessor _receivedFileProcessor;
		private ImportProcessor _importProcessor;
		private ReindexProcessor _reindexProcessor;
		private InstanceDeletionProcessor _deletionProcessor; 

		private string _storageDirectory = null;
		private string _badFileDirectory = null;
		private uint _sendReceiveImportConcurrency;
		private uint _databaseUpdateFrequencyMilliseconds;

		private object _startEventLock  = new object();
		private event EventHandler _startEvent;

		private object _stopEventLock = new object(); 
		private event EventHandler _stopEvent;

		private object _republishEventLock = new object(); 
		private event EventHandler _republishEvent;

		private object _clearInactiveEventLock = new object(); 
		private event EventHandler _clearInactiveEvent;

		private object _cancelEventLock = new object(); 
		private event EventHandler<ItemEventArgs<CancelProgressItemInformation>> _cancelEvent;

		private object _stateSyncLock = new object();
		private event EventHandler<ItemEventArgs<ServiceState>> _stateActivated;
		ServiceState _state;

		private LocalDataStoreService()
		{
			_disabled = false;
		}

		public static LocalDataStoreService Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LocalDataStoreService();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		public event EventHandler StartEvent
		{
			add
			{
				lock (_startEventLock)
				{
					_startEvent += value;
				}
			}
			remove
			{
				lock (_startEventLock)
				{
					_startEvent -= value; 
				}
			}
		}

		public event EventHandler StopEvent
		{
			add
			{
				lock (_stopEventLock)
				{
					_stopEvent += value;
				}
			}
			remove
			{
				lock (_stopEventLock)
				{
					_stopEvent -= value;
				}
			}
		}

		public event EventHandler RepublishEvent
		{
			add
			{
				lock (_republishEventLock)
				{
					_republishEvent += value;
				}
			}
			remove
			{
				lock (_republishEventLock)
				{
					_republishEvent -= value;
				}
			}
		}

		public event EventHandler ClearInactiveEvent
		{
			add
			{
				lock (_clearInactiveEventLock)
				{
					_clearInactiveEvent += value;
				}
			}
			remove
			{
				lock (_clearInactiveEventLock)
				{
					_clearInactiveEvent -= value;
				}
			}
		}

		public event EventHandler<ItemEventArgs<CancelProgressItemInformation>> CancelEvent
		{
			add
			{
				lock (_cancelEventLock)
				{
					_cancelEvent += value;
				}
			}
			remove
			{
				lock (_cancelEventLock)
				{
					_cancelEvent -= value;
				}
			}
		}

		private event EventHandler<ItemEventArgs<ServiceState>> StateActivated
		{
			add
			{
				lock (_stateSyncLock)
				{
					_stateActivated += value;
				}
			}
			remove
			{
				lock (_stateSyncLock)
				{
					_stateActivated -= value;
				}
			}
		}

		public void ReserveState(ServiceState newState)
		{
			lock (_stateSyncLock)
			{
				if (_state == newState)
					return;

				switch (newState)
				{
					case ServiceState.Reindexing:
						{
							if (_state == ServiceState.Deleting)
								throw new Exception(SR.ExceptionCannotReindexWhileDeletionIsInProgress);
						}
						break;	
					case ServiceState.Deleting:
						{
							if (_state == ServiceState.Reindexing)
								throw new Exception(SR.ExceptionCannotDeleteWhileReindexIsInProgress);
						}
						break;	
				}

				_state = newState;
			}
		}

		public void ActivateState()
		{
			lock (_stateSyncLock)
			{
				if (_state == ServiceState.Deleting)
					_dicomFileImporter.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.None);
				else if (_state == ServiceState.Reindexing)
					_dicomFileImporter.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Reindex);
				else
					_dicomFileImporter.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Import);

				EventsHelper.Fire(_stateActivated, this, new ItemEventArgs<ServiceState>(_state));
			}
		}

		public DicomFileImporter Importer
		{
			get { return _dicomFileImporter; }
		}

		public string StorageDirectory
		{
			get
			{
				if (!System.IO.Directory.Exists(_storageDirectory))
					System.IO.Directory.CreateDirectory(_storageDirectory);

				return _storageDirectory; 
			}
		}

		public string BadFileDirectory
		{
			get
			{
				if (!System.IO.Directory.Exists(_badFileDirectory))
					System.IO.Directory.CreateDirectory(_badFileDirectory);
				
				return _badFileDirectory; 
			}
		}

		public uint SendReceiveImportConcurrency
		{
			get { return _sendReceiveImportConcurrency; }
		}

		public uint DatabaseUpdateFrequencyMilliseconds
		{
			get { return _databaseUpdateFrequencyMilliseconds; }
		}

		private void CheckDisabled()
		{
			if (_disabled)
				throw new Exception(SR.ExceptionServiceHasBeenDisabled);
		}

		private void Initialize()
		{
			try
			{
				_storageDirectory = InitializeStorageDirectory(LocalDataStoreServiceSettings.Instance.StorageDirectory);
				if (_storageDirectory == null)
				{
					_storageDirectory = InitializeStorageDirectory(LocalDataStoreServiceSettings.DefaultStorageDirectory);
					if (_storageDirectory == null)
						throw new Exception(SR.ExceptionStorageDirectoryDoesNotExist);
				}

				_badFileDirectory = InitializeStorageDirectory(LocalDataStoreServiceSettings.Instance.BadFileDirectory);
				if (_badFileDirectory == null)
				{
					_badFileDirectory = InitializeStorageDirectory(LocalDataStoreServiceSettings.DefaultBadFileDirectory);
					if (_badFileDirectory == null)
						throw new Exception(SR.ExceptionBadFileStorageDirectoryDoesNotExist);
				}

				_sendReceiveImportConcurrency = LocalDataStoreServiceSettings.Instance.SendReceiveImportConcurrency;
				if (_sendReceiveImportConcurrency <= 0)
					_sendReceiveImportConcurrency = 1;

				if (_sendReceiveImportConcurrency >= 10)
					_sendReceiveImportConcurrency = 10;

				_databaseUpdateFrequencyMilliseconds = LocalDataStoreServiceSettings.Instance.DatabaseUpdateFrequencyMilliseconds;

				_dicomFileImporter = new DicomFileImporter(this);

				_sentFileProcessor = new SentFileProcessor(this);
				_receivedFileProcessor = new ReceivedFileProcessor(this);
				_importProcessor = new ImportProcessor(this);
				_reindexProcessor = new ReindexProcessor(this);
				_deletionProcessor = new InstanceDeletionProcessor(this);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				_disabled = true;
			}
		}

		private string InitializeStorageDirectory(string directoryPath)
		{
			string returnPath = null;

			try
			{
				directoryPath = Path.GetFullPath(directoryPath);

				string separator = Platform.PathSeparator.ToString();
				if (!directoryPath.EndsWith(separator))
					directoryPath += separator;

				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				returnPath = directoryPath;
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}

			return returnPath;
		}

		public void Start()
		{
			CheckDisabled();

			_dicomFileImporter.Start();

			lock (_startEventLock)
			{
				EventsHelper.Fire(_startEvent, this, EventArgs.Empty);
			}
		}

		public void Stop()
		{
			CheckDisabled();

			lock (_stopEventLock)
			{
				EventsHelper.Fire(_stopEvent, this, EventArgs.Empty);
			}

			_dicomFileImporter.Stop();
		}

		public void RepublishAll()
		{
			CheckDisabled();
			lock (_republishEventLock)
			{
				EventsHelper.Fire(_republishEvent, this, EventArgs.Empty);
			}
		}

		public void ClearInactive()
		{
			CheckDisabled();
			lock (_clearInactiveEventLock)
			{
				EventsHelper.Fire(_clearInactiveEvent, this, EventArgs.Empty);
			}
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			CheckDisabled();
			lock (_cancelEventLock)
			{
				EventsHelper.Fire(_cancelEvent, this, new ItemEventArgs<CancelProgressItemInformation>(information));
			}
		}

		#region ILocalDataStoreService Members

		public void RetrieveStarted(RetrieveStudyInformation information)
		{
			CheckDisabled();
			_receivedFileProcessor.RetrieveStarted(information);
		}

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			CheckDisabled();
			_receivedFileProcessor.ProcessReceivedFileInformation(receivedFileInformation);
		}

		public void ReceiveError(ReceiveErrorInformation errorInformation)
		{
			CheckDisabled();
			_receivedFileProcessor.ReceiveError(errorInformation);
		}

		public void SendStarted(SendStudyInformation information)
		{
			CheckDisabled();
			_sentFileProcessor.SendStarted(information);
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			CheckDisabled();
			_sentFileProcessor.ProcessSentFileInformation(sentFileInformation);
		}

		public void SendError(SendErrorInformation errorInformation)
		{
			CheckDisabled();
			_sentFileProcessor.SendError(errorInformation);
		}

		public LocalDataStoreServiceConfiguration GetConfiguration()
		{
			LocalDataStoreServiceConfiguration configuration = new LocalDataStoreServiceConfiguration();
			configuration.StorageDirectory = this.StorageDirectory;
			configuration.BadFileDirectory = this.BadFileDirectory;
			return configuration;
		}

		public void DeleteInstances(DeleteInstancesRequest request)
		{
			CheckDisabled();
			_deletionProcessor.DeleteInstances(request);
		}

		public Guid Import(FileImportRequest request)
		{
			CheckDisabled();
			return _importProcessor.Import(request);
		}

		public void Reindex()
		{
			CheckDisabled();
			_reindexProcessor.Reindex();
		}

		#endregion
	}
}
