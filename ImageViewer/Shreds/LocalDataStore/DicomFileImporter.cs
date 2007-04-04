using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class DicomFileImporter : DicomImageStoreBase
	{
		public enum DedicatedImportQueue
		{ 
			Default = 0,
			Reindex
		}

		public enum ImportStage
		{
			NotStarted = 0,
			FileParsed,
			FileMoved,
			CommittedToDataStore
		}

		private interface IFileImportInformation
		{
			ImportStage CompletedStage { set; }
			Exception Error { set; }
			bool BadFile { set; }
			string StoredFile { set; }

			string PatientId { set; }
			string PatientsName { set; }
			string StudyDate { set; }
			string StudyDescription { set; }
			string StudyInstanceUid { set; }
			string SeriesInstanceUid { set; }
			string SopInstanceUid { set; }

			Study Study { get; set; }
			Series Series { get; set; }
			SopInstance SopInstance { get; set; }
		}

		public class FileImportInformation : IFileImportInformation
		{
			#region Private Fields

			#region Client Input

			private string _sourceFile;
			private FileImportBehaviour _importBehaviour;
			private BadFileBehaviour _badFileBehaviour;

			#endregion

			#region Results / Errors

			private ImportStage _completedStage;
			private Exception _error;
			private bool _badFile;
			private string _storedFile;

			#endregion

			#region Parsed File Information

			private string _patientId;
			private string _patientsName;
			private string _studyDescription;
			private string _studyDate;

			private string _studyInstanceUid;
			private string _seriesInstanceUid;
			private string _sopInstanceUid;

			#endregion

			#region Importer Specific Information

			private Study _study;
			private Series _series;
			private SopInstance _sopInstance;

			#endregion

			#endregion

			public FileImportInformation(string sourceFile)
				: this(sourceFile, FileImportBehaviour.Move, BadFileBehaviour.Ignore)
			{
			}

			public FileImportInformation(string sourceFile, FileImportBehaviour importBehaviour, BadFileBehaviour badFileBehaviour)
			{
				_sourceFile = sourceFile;
				_importBehaviour = importBehaviour;
				_badFileBehaviour = badFileBehaviour;
			}

			private FileImportInformation()
			{
			}

			public string SourceFile
			{
				get { return _sourceFile; }
			}

			public BadFileBehaviour BadFileBehaviour
			{
				get { return _badFileBehaviour; }
			}

			public FileImportBehaviour ImportBehaviour
			{
				get { return _importBehaviour; }
			}

			public ImportStage CompletedStage
			{
				get { return _completedStage; }
			}

			public bool Failed
			{
				get { return _error != null; }
			}

			public Exception Error
			{
				get { return _error; }
			}

			public bool BadFile
			{
				get { return _badFile; }
			}

			public string StoredFile
			{
				get { return _storedFile; }
			}

			public string PatientId
			{
				get { return _patientId; }
			}

			public string PatientsName
			{
				get { return _patientsName; }
			}

			public string StudyDate
			{
				get { return _studyDate; }
			}

			public string StudyDescription
			{
				get { return _studyDescription; }
			}

			public string StudyInstanceUid
			{
				get { return _studyInstanceUid; }
			}

			public string SeriesInstanceUid
			{
				get { return _seriesInstanceUid; }
			}

			public string SopInstanceUid
			{
				get { return _sopInstanceUid; }
			}

			#region IDicomFileImporterInformation Members

			ImportStage IFileImportInformation.CompletedStage
			{
				set { _completedStage = value; }
			}

			Exception IFileImportInformation.Error
			{
				set { _error = value; }
			}

			bool IFileImportInformation.BadFile
			{
				set { _badFile = value; }
			}

			string IFileImportInformation.StoredFile
			{
				set { _storedFile = value; }
			}

			string IFileImportInformation.PatientId
			{
				set { _patientId = value; }
			}

			string IFileImportInformation.PatientsName
			{
				set { _patientsName = value; }
			}

			string IFileImportInformation.StudyDate
			{
				set { _studyDate = value; }
			}

			string IFileImportInformation.StudyDescription
			{
				set { _studyDescription = value; }
			}

			string IFileImportInformation.StudyInstanceUid
			{
				set { _studyInstanceUid = value; }
			}

			string IFileImportInformation.SeriesInstanceUid
			{
				set { _seriesInstanceUid = value; }
			}

			string IFileImportInformation.SopInstanceUid
			{
				set { _sopInstanceUid = value; }
			}

			public Study Study
			{
				get { return _study; }
				set { _study = value; }
			}

			public Series Series
			{
				get { return _series; }
				set { _series = value; }
			}

			public SopInstance SopInstance
			{
				get { return _sopInstance; }
				set { _sopInstance = value; }
			}

			#endregion
		}

		internal delegate void FileImportJobStatusReportDelegate(FileImportInformation results);

		#region Import Job Information

		private class ImportJobInformation
		{
			private FileImportInformation _fileImportInformation;
			private FileImportJobStatusReportDelegate _fileImportJobStatusReportDelegate;
			private DedicatedImportQueue _destinationImportQueue;

			public ImportJobInformation
				(
					FileImportInformation fileImportInformation,
					FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate,
					DedicatedImportQueue destinationImportQueue
				)
			{
				_fileImportInformation = fileImportInformation;
				_fileImportJobStatusReportDelegate = fileImportJobStatusReportDelegate;
				_destinationImportQueue = destinationImportQueue;
			}

			public DedicatedImportQueue DestinationImportQueue
			{
				get { return _destinationImportQueue; }
			}

			public FileImportInformation FileImportInformation
			{
				get { return _fileImportInformation; }
			}

			public FileImportJobStatusReportDelegate FileImportJobStatusReportDelegate
			{
				get { return _fileImportJobStatusReportDelegate; }
			}
		}

		#endregion

		private class ParseFileThreadPool : SimpleThreadPoolBase<ImportJobInformation>
		{
			DicomFileImporter _parent;

			public ParseFileThreadPool(DicomFileImporter parent)
				: base(LocalDataStoreService.Instance.SendReceiveImportConcurrency)
			{
				_parent = parent;
			}

			protected override void ProcessItem(ImportJobInformation jobInformation)
			{
				_parent.ParseFile(jobInformation);

				if (!jobInformation.FileImportInformation.Failed)
					_parent.AddToImportQueue(jobInformation);
			}
		};

		private class ImportFileThreadPool : SimpleThreadPoolBase<ImportJobInformation>
		{
			DicomFileImporter _parent;

			public ImportFileThreadPool(DicomFileImporter parent)
				: base(LocalDataStoreService.Instance.SendReceiveImportConcurrency)
			{
				_parent = parent;
				this.AllowInactiveAdd = true;
			}

			protected override void ProcessItem(ImportJobInformation jobInformation)
			{
				_parent.MoveFile(jobInformation);

				if (!jobInformation.FileImportInformation.Failed)
					_parent.AddToDatabaseQueue(jobInformation);
			}
		};

		private LocalDataStoreService _parent;

		private bool _stopDatabaseQueue = false;
		private object _databaseQueueWait = new object();

		private Thread _databaseUpdateThread;
		private object _databaseUpdateItemsLock = new object();
		private List<ImportJobInformation> _databaseUpdateItems;

		private ParseFileThreadPool _parseFileThreadPool;

		private DedicatedImportQueue _activeImportThreadPool;
		private Dictionary<DedicatedImportQueue, ImportFileThreadPool> _importThreadPools;

		private object _importThreadPoolSwitchSyncLock = new object();
		private event EventHandler<ItemEventArgs<DedicatedImportQueue>> _importThreadPoolSwitched;

		public DicomFileImporter(LocalDataStoreService parent)
			: base()
		{
			_parent = parent;
			_parseFileThreadPool = new ParseFileThreadPool(this);
			
			_importThreadPools = new Dictionary<DedicatedImportQueue, ImportFileThreadPool>();
			_importThreadPools.Add(DedicatedImportQueue.Default, new ImportFileThreadPool(this));
			_importThreadPools.Add(DedicatedImportQueue.Reindex, new ImportFileThreadPool(this));
			_activeImportThreadPool = DedicatedImportQueue.Default;

			_databaseUpdateItems = new List<ImportJobInformation>();
		}

		#region File Parse / Import Methods

		private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
		{
			StringBuilder stringValue = new StringBuilder(1024);
			OFCondition cond;
			cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
			if (cond.good())
			{
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == stringValue.ToString())
					return false;
			}

			return true;
		}

		private void ParseFile(ImportJobInformation jobInformation)
		{
			FileImportInformation fileImportInformation = jobInformation.FileImportInformation;
			FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate = jobInformation.FileImportJobStatusReportDelegate;

			DcmFileFormat file = new DcmFileFormat();
			//use this private interface to set the values as we go along.
			IFileImportInformation setImportInformation = (IFileImportInformation)fileImportInformation;

			try
			{
				OFCondition condition = file.loadFile(fileImportInformation.SourceFile);
				bool good = condition.good();

				DcmMetaInfo metaInfo = file.getMetaInfo();
				DcmDataset dataset = file.getDataset();

				bool processable = ConfirmProcessableFile(metaInfo, dataset);

				if (!condition.good() || !processable)
				{
					file.Dispose();
					file = null;

					StringBuilder errorString = new StringBuilder("");
					if (!processable)
						errorString.AppendFormat(SR.FormatFileFormatNotAppropriateForDatastore, fileImportInformation.SourceFile);
					else
						errorString.AppendFormat(SR.FormatFileFormatNotRecognized, fileImportInformation.SourceFile);

					if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Move)
					{
						string storedFile = String.Format("{0}{1}", LocalDataStoreService.Instance.BadFileFolder, System.IO.Path.GetRandomFileName());
						System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
						((IFileImportInformation)fileImportInformation).StoredFile = storedFile;

						errorString.AppendFormat(SR.FormatFileHasBeenMoved, storedFile);
					}
					else if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Delete)
					{
						System.IO.File.Delete(fileImportInformation.SourceFile);

						errorString.AppendFormat(SR.MessageFileHasBeenDeleted);
					}

					setImportInformation.Error = new Exception(errorString.ToString());
					setImportInformation.BadFile = true;
					fileImportJobStatusReportDelegate(fileImportInformation);
					return;
				}

				//parse the file and create study, series and sop objects for (potential) insertion into the datastore.
				setImportInformation.Study = CreateNewStudy(metaInfo, dataset);
				setImportInformation.Series = CreateNewSeries(metaInfo, dataset);
				setImportInformation.SopInstance = CreateNewSopInstance(metaInfo, dataset);

				//!! lock the uri!!
				//UriBuilder storedFileUri = new UriBuilder();
				//storedFileUri.Scheme = "file";
				//storedFileUri.Path = storedFile;

				setImportInformation.StudyInstanceUid = setImportInformation.Study.StudyInstanceUid;
				setImportInformation.SeriesInstanceUid = setImportInformation.Series.SeriesInstanceUid;
				setImportInformation.SopInstanceUid = setImportInformation.SopInstance.SopInstanceUid;
				setImportInformation.StudyDate = fileImportInformation.Study.StudyDate;
				setImportInformation.PatientId = fileImportInformation.Study.PatientId;
				setImportInformation.PatientsName = (fileImportInformation.Study.PatientsName == null) ? "" : fileImportInformation.Study.PatientsName.ToString();
				setImportInformation.StudyDescription = fileImportInformation.Study.StudyDescription;

				//report the progress.
				setImportInformation.CompletedStage = ImportStage.FileParsed;
				fileImportJobStatusReportDelegate(fileImportInformation);

				// keep the file object alive until the end of this scope block
				// otherwise, it'll be GC'd and metaInfo and dataset will be gone
				// as well, even though they are needed in the InsertSopInstance
				// and sub methods
				//GC.KeepAlive(file);
			}
			catch (Exception e)
			{
				setImportInformation.Error = e;
				fileImportJobStatusReportDelegate(fileImportInformation);
			}
			finally
			{
				if (file != null)
				{
					file.Dispose();
					file = null;
				}
			}
		}

		private void MoveFile(ImportJobInformation jobInformation)
		{
			FileImportInformation fileImportInformation = jobInformation.FileImportInformation;
			FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate = jobInformation.FileImportJobStatusReportDelegate;

			IFileImportInformation importerInformation = (IFileImportInformation)fileImportInformation;

			try
			{
				string storedFile = String.Format("{0}{1}\\{2}\\{3}.dcm", LocalDataStoreService.Instance.StorageFolder,
																			fileImportInformation.StudyInstanceUid,
																			fileImportInformation.SeriesInstanceUid,
																			fileImportInformation.SopInstanceUid);

				UriBuilder sourceUri = new UriBuilder();
				sourceUri.Scheme = "file";
				sourceUri.Path = fileImportInformation.SourceFile;

				UriBuilder storedUri = new UriBuilder();
				storedUri.Scheme = "file";
				storedUri.Path = storedFile;

				if (storedUri.Uri.AbsolutePath != sourceUri.Uri.AbsolutePath)
				{
					string directoryName = System.IO.Path.GetDirectoryName(storedFile);
					if (!System.IO.Directory.Exists(directoryName))
						System.IO.Directory.CreateDirectory(directoryName);

					if (fileImportInformation.ImportBehaviour == FileImportBehaviour.Copy)
					{
						System.IO.File.Copy(fileImportInformation.SourceFile, storedFile, true);
					}
					else
					{
						if (System.IO.File.Exists(storedFile))
							System.IO.File.Delete(storedFile);
						
						System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
					}
				}

				AssignSopInstanceUri(fileImportInformation.SopInstance, storedFile);

				importerInformation.StoredFile = storedFile;
				importerInformation.CompletedStage = ImportStage.FileMoved;
			}
			catch (Exception e)
			{
				importerInformation.Error = e;

				//!!retry?
			}

			fileImportJobStatusReportDelegate(fileImportInformation);
		}

		#endregion

		public void Enqueue(FileImportInformation information, FileImportJobStatusReportDelegate statusReportDelegate, DedicatedImportQueue queue)
		{
			AddToParseQueue(new ImportJobInformation(information, statusReportDelegate, queue));
		}

		private void AddToParseQueue(ImportJobInformation jobInformation)
		{
			_parseFileThreadPool.Enqueue(jobInformation);
		}

		private void AddToImportQueue(ImportJobInformation jobInformation)
		{
			_importThreadPools[jobInformation.DestinationImportQueue].Enqueue(jobInformation);
		}

		#region Database Related Methods

		private void AddToDatabaseQueue(ImportJobInformation jobInformation)
		{
			lock (_databaseUpdateItemsLock)
			{
				_databaseUpdateItems.Add(jobInformation);
			}
		}

		private void UpdateDatabase(List<ImportJobInformation> items)
		{
			if (items.Count == 0)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			IDataStoreReader dataStoreReader = SingleSessionDataAccessLayer.GetIDataStoreReader();

			try
			{
				foreach (ImportJobInformation item in items)
				{
					Study study = null;
					if (this.StudyCache.ContainsKey(item.FileImportInformation.StudyInstanceUid))
						study = this.StudyCache[item.FileImportInformation.StudyInstanceUid];

					if (null == study)
					{
						study = dataStoreReader.GetStudy(new Uid(item.FileImportInformation.StudyInstanceUid)) as Study;
						if (null == study)
						{
							study = item.FileImportInformation.Study;
							this.StudyCache.Add(item.FileImportInformation.StudyInstanceUid, study);
						}
						else
						{
							// the study was found in the data store
							this.StudyCache.Add(item.FileImportInformation.StudyInstanceUid, study);

							// since Study-Series is not lazy initialized, all the series
							// should be loaded. Let's add them to the cache
							foreach (ISeries existingSeries in study.GetSeries())
							{
								this.SeriesCache.Add(existingSeries.GetSeriesInstanceUid().ToString(), existingSeries as Series);
							}
						}
					}

					Series series = null;
					if (this.SeriesCache.ContainsKey(item.FileImportInformation.SeriesInstanceUid))
						series = this.SeriesCache[item.FileImportInformation.SeriesInstanceUid];

					if (null == series)
					{
						series = item.FileImportInformation.Series;
						this.SeriesCache.Add(item.FileImportInformation.SeriesInstanceUid, series);
					}

					series.AddSopInstance(item.FileImportInformation.SopInstance);
					study.AddSeries(series);
				}

				foreach (KeyValuePair<string, Study> pair in this.StudyCache)
				{
					SingleSessionDataAccessLayer.GetIDataStoreWriter().StoreStudy(pair.Value);
				}

				foreach (ImportJobInformation item in items)
				{
					((IFileImportInformation)item.FileImportInformation).CompletedStage = ImportStage.CommittedToDataStore;
					item.FileImportJobStatusReportDelegate(item.FileImportInformation);
				}
			}
			catch (Exception e)
			{
				Platform.Log(e);

				foreach (ImportJobInformation item in items)
				{
					string error = String.Format(SR.FormatFailedToCommitToDatastore, item.FileImportInformation.StoredFile);
					((IFileImportInformation)item.FileImportInformation).Error = new Exception(error);
					item.FileImportJobStatusReportDelegate(item.FileImportInformation);
				}
			}


			this.SeriesCache.Clear();
			this.StudyCache.Clear();

			SingleSessionDataAccessLayer.SqliteWorkaround();

			clock.Stop();
			Trace.WriteLine(String.Format("Update took {0} seconds", clock.Seconds));
		}

		private void DatabaseUpdateThread()
		{
			int waitTimeout = LocalDataStoreService.Instance.DatabaseUpdateFrequencyMilliseconds;

			while (true)
			{
				lock (_databaseQueueWait)
				{
					if (!_stopDatabaseQueue)
						Monitor.Wait(_databaseQueueWait, waitTimeout);
				}
				
				List<ImportJobInformation> updates = new List<ImportJobInformation>();
				lock (_databaseUpdateItemsLock)
				{
					updates.AddRange(_databaseUpdateItems);
					_databaseUpdateItems.Clear();
				}

				try
				{
					UpdateDatabase(updates);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				lock (_databaseQueueWait)
				{
					if (_stopDatabaseQueue)
						break;
				}
			}
		}

		#endregion

		public event EventHandler<ItemEventArgs<DedicatedImportQueue>> ImportThreadPoolSwitched
		{
			add
			{
				lock (_importThreadPoolSwitchSyncLock)
				{
					_importThreadPoolSwitched += value;
				}
			}
			remove 
			{
				lock (_importThreadPoolSwitchSyncLock)
				{
					_importThreadPoolSwitched -= value;
				}
			}
		}

		#region Start/Stop Methods

		public void Start()
		{
			if (_databaseUpdateThread != null)
				throw new InvalidOperationException(SR.ExceptionImporterAlreadyStarted);

			_parseFileThreadPool.Start();
			_importThreadPools[DedicatedImportQueue.Default].Start();

			_stopDatabaseQueue = false;

			ThreadStart threadStart = new ThreadStart(this.DatabaseUpdateThread);
			_databaseUpdateThread = new Thread(threadStart);
			_databaseUpdateThread.IsBackground = true;
			_databaseUpdateThread.Priority = ThreadPriority.AboveNormal;
			_databaseUpdateThread.Start();
		}

		public void Stop()
		{
			if (_databaseUpdateThread == null)
				throw new InvalidOperationException(SR.ExceptionImporterNotStarted);

			_parseFileThreadPool.Stop();

			foreach (ImportFileThreadPool pool in _importThreadPools.Values)
				pool.Stop();

			lock (_databaseQueueWait)
			{
				_stopDatabaseQueue = true;
				Monitor.Pulse(_databaseQueueWait);
			}

			_databaseUpdateThread.Join();
			_databaseUpdateThread = null;
		}

		public void ActivateImportQueue(DedicatedImportQueue queue)
		{
			lock (_importThreadPoolSwitchSyncLock)
			{
				if (_activeImportThreadPool == queue)
					return;

				_importThreadPools[_activeImportThreadPool].Stop();
				_activeImportThreadPool = queue;
				_importThreadPools[_activeImportThreadPool].Start();

				EventsHelper.Fire(_importThreadPoolSwitched, this, new ItemEventArgs<DedicatedImportQueue>(_activeImportThreadPool));
			}
		}

		#endregion
	}
}
