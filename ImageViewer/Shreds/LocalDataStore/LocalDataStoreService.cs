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

		private SendReceiveImportQueue _sendReceiveImportQueue;
		private DicomFileImporter _dicomFileImporter;
		
		private string _storageFolder = null;
		private string _badFileFolder = null;
		private int _sendReceiveImportConcurrency;

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
						throw new Exception("The storage directory is inaccessible.  The service has been disabled.");
				}

				_badFileFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.Instance.BadFileFolder);
				if (_badFileFolder == null)
				{
					_badFileFolder = InitializeStorageFolder(LocalDataStoreServiceSettings.DefaultBadFileFolder);
					if (_badFileFolder == null)
						throw new Exception("The 'bad file' storage directory is inaccessible.  The service has been disabled.");
				}

				_sendReceiveImportConcurrency = LocalDataStoreServiceSettings.Instance.SendReceiveImportConcurrency;
				if (_sendReceiveImportConcurrency <= 0)
					_sendReceiveImportConcurrency = 1;

				if (_sendReceiveImportConcurrency >= 10)
					_sendReceiveImportConcurrency = 10;

				_dicomFileImporter = new DicomFileImporter();

				_sendReceiveImportQueue = new SendReceiveImportQueue();
			}
			catch (Exception e)
			{
				Platform.Log(e);
				_disabled = true;
			}
		}

		public SendReceiveImportQueue SendReceiveImportQueue
		{
			get { return _sendReceiveImportQueue; }
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

		private void CheckDisabled()
		{
			if (_disabled)
				throw new Exception("Unable to process request. The service has been disabled.");
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
			_sendReceiveImportQueue.Start();
		}

		public void Stop()
		{
			_sendReceiveImportQueue.Stop();
		}

		#region ILocalDataStoreService Members

		public void FilesReceived(StoreScpReceivedFilesInformation receivedFilesInformation)
		{
			CheckDisabled();
			_sendReceiveImportQueue.ProcessFilesReceived(receivedFilesInformation);
		}

		public void FilesSent(StoreScuSentFilesInformation sentFilesInformation)
		{
			CheckDisabled();
			//!!
		}

		public void Import(FileImportRequest request)
		{
			CheckDisabled();
			//!!
		}

		public void Reindex()
		{
			CheckDisabled();
			//!!
		}

		#endregion
	}
}
