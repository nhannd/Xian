using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.IO;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreService : ILocalDataStoreService
	{
		private static LocalDataStoreService _instance;

		private bool _disabled;

		private DicomFileImporter _dicomFileImporter;
		private SentFileProcessor _sentFileProcessor;
		private ReceivedFileProcessor _receivedFileProcessor;
		private ImportProcessor _importProcessor;

		private string _storageFolder = null;
		private string _badFileFolder = null;
		private int _sendReceiveImportConcurrency;
		private int _databaseUpdateFrequencyMilliseconds;

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

		public void Initialize()
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

				_sentFileProcessor = new SentFileProcessor();
				_receivedFileProcessor = new ReceivedFileProcessor();
				_importProcessor = new ImportProcessor();

				_dicomFileImporter = new DicomFileImporter();
			}
			catch (Exception e)
			{
				Platform.Log(e);
				_disabled = true;
			}
		}

		public DicomFileImporter DicomFileImporter
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

		public int SendReceiveImportConcurrency
		{
			get { return _sendReceiveImportConcurrency; }
		}

		public int DatabaseUpdateFrequencyMilliseconds
		{
			get { return _databaseUpdateFrequencyMilliseconds; }
		}

		private void CheckDisabled()
		{
			if (_disabled)
				throw new Exception(SR.ExceptionServiceHasBeenDisabled);
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
			_dicomFileImporter.Start();

			_sentFileProcessor.Start();
			_receivedFileProcessor.Start();
			_importProcessor.Start();
		}

		public void Stop()
		{
			_receivedFileProcessor.Stop();
			_sentFileProcessor.Stop();
			_importProcessor.Stop();

			_dicomFileImporter.Stop();
		}

		public void RepublishAll()
		{
			_receivedFileProcessor.RepublishAll();
			_sentFileProcessor.RepublishAll();
			_importProcessor.RepublishAll();
		}
		
		#region ILocalDataStoreService Members

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			CheckDisabled();
			_receivedFileProcessor.ProcessReceivedFileInformation(receivedFileInformation);
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			CheckDisabled();
			_sentFileProcessor.ProcessSentFileInformation(sentFileInformation);
		}

		public Guid Import(FileImportRequest request)
		{
			CheckDisabled();
			return _importProcessor.Import(request);
		}

		public void Reindex()
		{
			CheckDisabled();
		}

		#endregion

		public void ClearInactive()
		{
			this._importProcessor.ClearInactive();
			this._sentFileProcessor.ClearInactive();
			this._receivedFileProcessor.ClearInactive();
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			this._importProcessor.Cancel(information);
			this._receivedFileProcessor.Cancel(information);
			this._sentFileProcessor.Cancel(information);
		}
	}
}
