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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Dicom.DataStore;
using System.Threading;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService : ILocalDataStoreService, IStudyStorageLocator
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

		private System.Threading.Timer _purgeTimer;
		private readonly object _purgeEventLock = new object();
		private event EventHandler _purgeEvent;

		private readonly object _startEventLock = new object();
		private event EventHandler _startEvent;

		private readonly object _stopEventLock = new object(); 
		private event EventHandler _stopEvent;

		private readonly object _republishEventLock = new object(); 
		private event EventHandler _republishEvent;

		private readonly object _clearInactiveEventLock = new object(); 
		private event EventHandler _clearInactiveEvent;

		private readonly object _cancelEventLock = new object(); 
		private event EventHandler<ItemEventArgs<CancelProgressItemInformation>> _cancelEvent;

		private readonly object _stateSyncLock = new object();
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

		public event EventHandler PurgeEvent
		{
			add
			{
				lock (_purgeEventLock)
				{
					_purgeEvent += value;
				}
			}
			remove
			{
				lock (_purgeEventLock)
				{
					_purgeEvent -= value;
				}
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

				DataAccessLayer.SetStudyStorageLocator(this);

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

				_dicomFileImporter = new DicomFileImporter();

				_sentFileProcessor = new SentFileProcessor(this);
				_receivedFileProcessor = new ReceivedFileProcessor(this);
				_importProcessor = new ImportProcessor(this);
				_reindexProcessor = new ReindexProcessor(this);
				_deletionProcessor = new InstanceDeletionProcessor(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				_disabled = true;
			}
		}

		private void OnPurge(object nothing)
		{
			lock (_stateSyncLock)
			{
				if (_state != ServiceState.Importing)
					return;
			}

			lock(_purgeEventLock)
			{
				if (_purgeTimer != null)
					EventsHelper.Fire(_purgeEvent, this, EventArgs.Empty);
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
				Platform.Log(LogLevel.Error, e);
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

			try
			{
				lock(_purgeEventLock)
				{
					_purgeTimer = new System.Threading.Timer(OnPurge, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
				}
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to start purge timer; old items will never be purged.");
			}
		}

		public void Stop()
		{
			CheckDisabled();

			lock (_stopEventLock)
			{
				EventsHelper.Fire(_stopEvent, this, EventArgs.Empty);
			}

			lock (_purgeEventLock)
			{
				try
				{
					if (_purgeTimer != null)
						_purgeTimer.Dispose();
				}
				finally
				{
					_purgeTimer = null;
				}
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

		#region IStudyStorageLocator Members

		public string GetStudyStorageDirectory(string studyInstanceUid)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

			string studyStorageDirectory = Path.Combine(StorageDirectory, studyInstanceUid);
			if (!Directory.Exists(studyStorageDirectory))
				Directory.CreateDirectory(studyStorageDirectory);

			return studyStorageDirectory;
		}

		#endregion
	}
}
