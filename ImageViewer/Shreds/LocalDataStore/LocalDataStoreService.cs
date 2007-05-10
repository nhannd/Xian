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
		private static LocalDataStoreService _instance;

		private bool _disabled;

		private DicomFileImporter _dicomFileImporter;
		private SentFileProcessor _sentFileProcessor;
		private ReceivedFileProcessor _receivedFileProcessor;
		private ImportProcessor _importProcessor;
		private ReindexProcessor _reindexProcessor;

		private string _storageFolder = null;
		private string _badFileFolder = null;
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

		public DicomFileImporter Importer
		{
			get { return _dicomFileImporter; }
		}

		public string StorageFolder
		{
			get { return _storageFolder; }
		}

		public string BadFileFolder
		{
			get { return _badFileFolder; }
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
				_storageFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.Instance.StorageFolder);
				if (_storageFolder == null)
				{
					_storageFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.DefaultStorageFolder);
					if (_storageFolder == null)
						throw new Exception(SR.ExceptionStorageDirectoryDoesNotExist);
				}

				_badFileFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.Instance.BadFileFolder);
				if (_badFileFolder == null)
				{
					_badFileFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.DefaultBadFileFolder);
					if (_badFileFolder == null)
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
			}
			catch (Exception e)
			{
				Platform.Log(e);
				_disabled = true;
			}
		}

		private string InitializeStorageFolder(string folderPath)
		{
			string returnPath = null;

			try
			{
				if (!folderPath.EndsWith("\\"))
					folderPath += "\\";

				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

				returnPath = folderPath;
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
