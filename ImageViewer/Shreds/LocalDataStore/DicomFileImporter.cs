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
			string SeriesInstanceUid { set; }
			string SopInstanceUid { set; }
			string StudyDate { set; }
			string StudyDescription { set; }
			string StudyInstanceUid { set; }

			Study Study { get; set; }
			Series Series { get; set; }
			SopInstance SopInstance { get; set; }
		}

		public class FileImportInformation : IFileImportInformation
		{
			#region Private Fields

			#region Client Input

			private string _sourceFile;
			private BadFileBehaviour _badFileBehaviour;

			#endregion

			#region Results / Errors

			private ImportStage _completedStage;
			private Exception _error;
			private bool _badFile;
			private string _storedFile;

			#endregion

			#region Parsed File Information

			private string _studyInstanceUid;
			private string _seriesInstanceUid;
			private string _sopInstanceUid;

			private string _patientId;
			private string _patientsName;
			private string _studyDescription;
			private string _studyDate;

			#endregion

			#region Importer Specific Information

			private SopInstance _sopInstance;
			private Series _series;
			private Study _study;

			#endregion

			#endregion

			public FileImportInformation(string sourceFile)
				: this(sourceFile, BadFileBehaviour.Ignore)
			{
			}

			public FileImportInformation(string sourceFile, BadFileBehaviour badFileBehaviour)
			{
				_sourceFile = sourceFile;
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

			public string SeriesInstanceUid
			{
				get { return _seriesInstanceUid; }
			}

			public string SopInstanceUid
			{
				get { return _sopInstanceUid; }
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

			string IFileImportInformation.SeriesInstanceUid
			{
				set { _seriesInstanceUid = value; }
			}

			string IFileImportInformation.SopInstanceUid
			{
				set { _sopInstanceUid = value; }
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

		#region File Parse Thread Pool Job

		private class FileParseJob : IJob
		{
			private ImportJobInformation _importerData;

			public FileParseJob (ImportJobInformation importerData)
			{
				_importerData = importerData;
			}

			private FileParseJob()
			{ 
			}

			#region IJob Members

			public void Execute()
			{
				//parse the file.
				_importerData.Importer.ParseFile(_importerData.FileImportInformation, _importerData.FileImportJobStatusReportDelegate);

				if (!_importerData.FileImportInformation.Failed)
					_importerData.Importer.AddToImportQueue(_importerData);
			}

			#endregion
		}

		#endregion

		#region File Parse ThreadPool Job

		private class FileImportJob : IJob
		{
			private ImportJobInformation _importerData;

			public FileImportJob(ImportJobInformation importerData)
			{
				_importerData = importerData;
			}

			private FileImportJob()
			{
			}

			#region IJob Members

			public void Execute()
			{
				_importerData.Importer.MoveFile(_importerData.FileImportInformation, _importerData.FileImportJobStatusReportDelegate);

				if (!_importerData.FileImportInformation.Failed)
					_importerData.Importer.AddToDatabaseQueue(_importerData);
			}

			#endregion
		}

		#endregion

		#region Import Job Information

		private class ImportJobInformation
		{
			private DicomFileImporter _importer;
			private FileImportInformation _fileImportInformation;
			private FileImportJobStatusReportDelegate _fileImportJobStatusReportDelegate;

			public ImportJobInformation
				(
					DicomFileImporter importer,
					FileImportInformation fileImportInformation,
					FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate
				)
			{
				_importer = importer;
				_fileImportInformation = fileImportInformation;
				_fileImportJobStatusReportDelegate = fileImportJobStatusReportDelegate;
			}

			public DicomFileImporter Importer
			{
				get { return _importer; }
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

		private object _syncLock = new object();
		private EventWaitHandle _stopDatabaseQueueSignal;
		private List<ImportJobInformation> _databaseUpdateItems;
		private Thread _databaseUpdateThread;

		private SimpleThreadPool<FileParseJob> _parseFileThreadPool;
		private SimpleThreadPool<FileImportJob> _importFileThreadPool;
		
		public DicomFileImporter()
			: base()
		{
			_parseFileThreadPool = new SimpleThreadPool<FileParseJob>(LocalDataStoreService.Instance.SendReceiveImportConcurrency);
			_importFileThreadPool = new SimpleThreadPool<FileImportJob>(LocalDataStoreService.Instance.SendReceiveImportConcurrency);

			_stopDatabaseQueueSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
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
		
		private void ParseFile(FileImportInformation fileImportInformation, FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate)
		{
			DcmFileFormat file = new DcmFileFormat();
			//use this private interface to set the values as we go along.
			IFileImportInformation importerInformation = (IFileImportInformation)fileImportInformation;

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
						errorString.AppendFormat("Although the file format is recognized, it is not appropriate for insertion into the datastore ({0})", fileImportInformation.SourceFile);
					else
						errorString.AppendFormat("The file format is not recognized ({0})", fileImportInformation.SourceFile);

					if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Move)
					{
						string storedFile = String.Format("{0}{1}", LocalDataStoreService.Instance.BadFileFolder, System.IO.Path.GetRandomFileName());
						System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
						((IFileImportInformation)fileImportInformation).StoredFile = storedFile;

						errorString.AppendFormat("; the file has been moved to {0}", storedFile);
					}
					else if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Delete)
					{
						System.IO.File.Delete(fileImportInformation.SourceFile);
					
						errorString.AppendFormat("; the file has been deleted");
					}

					importerInformation.Error = new Exception(errorString.ToString());
					importerInformation.BadFile = true;
					fileImportJobStatusReportDelegate(fileImportInformation);
					return;
				}

				//parse the file and create study, series and sop objects for (potential) insertion into the datastore.
				importerInformation.Study = CreateNewStudy(metaInfo, dataset);
				importerInformation.Series = CreateNewSeries(metaInfo, dataset);
				importerInformation.SopInstance = CreateNewSopInstance(metaInfo, dataset);

				//!! lock the uri!!
				//UriBuilder storedFileUri = new UriBuilder();
				//storedFileUri.Scheme = "file";
				//storedFileUri.Path = storedFile;

				importerInformation.StudyInstanceUid = importerInformation.Study.StudyInstanceUid;
				importerInformation.SeriesInstanceUid = importerInformation.Series.SeriesInstanceUid;
				importerInformation.SopInstanceUid = importerInformation.SopInstance.SopInstanceUid;
				importerInformation.StudyDate = fileImportInformation.Study.StudyDate;
				importerInformation.PatientId = fileImportInformation.Study.PatientId;
				importerInformation.PatientsName = (fileImportInformation.Study.PatientsName == null) ? "" : fileImportInformation.Study.PatientsName.ToString();
				importerInformation.StudyDescription = fileImportInformation.Study.StudyDescription;

				//report the progress.
				importerInformation.CompletedStage = ImportStage.FileParsed;
				fileImportJobStatusReportDelegate(fileImportInformation);

				// keep the file object alive until the end of this scope block
				// otherwise, it'll be GC'd and metaInfo and dataset will be gone
				// as well, even though they are needed in the InsertSopInstance
				// and sub methods
				//GC.KeepAlive(file);
			}
			catch (Exception e)
			{
				importerInformation.Error = e;
				fileImportJobStatusReportDelegate(fileImportInformation);
			}
			finally
			{
				file.Dispose();
				file = null;
			}
		}

		private void MoveFile(FileImportInformation fileImportInformation, FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate)
		{
			IFileImportInformation importerInformation = (IFileImportInformation)fileImportInformation;

			try
			{
				string storedFile = String.Format("{0}{1}\\{2}\\{3}.dcm", LocalDataStoreService.Instance.StorageFolder,
																			fileImportInformation.StudyInstanceUid,
																			fileImportInformation.SeriesInstanceUid,
																			fileImportInformation.SopInstanceUid);

				string directoryName = System.IO.Path.GetDirectoryName(storedFile);
				if (!System.IO.Directory.Exists(directoryName))
					System.IO.Directory.CreateDirectory(directoryName);

				if (System.IO.File.Exists(storedFile))
					System.IO.File.Delete(storedFile);

				System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
				//System.IO.File.Copy(fileImportInformation.SourceFile, storedFile);

				AssignSopInstanceUri(fileImportInformation.SopInstance, storedFile);

				importerInformation.StoredFile = storedFile;
				importerInformation.CompletedStage = ImportStage.FileMoved;
			}
			catch (Exception e)
			{
				importerInformation.Error = e;
			}

			fileImportJobStatusReportDelegate(fileImportInformation);
		}

		#endregion

		public void Enqueue(FileImportInformation information, FileImportJobStatusReportDelegate statusReportDelegate)
		{
			AddToParseQueue(new ImportJobInformation(this, information, statusReportDelegate));
		}

		private void AddToParseQueue(ImportJobInformation jobInformation)
		{
			_parseFileThreadPool.Push(new FileParseJob(jobInformation));
		}

		private void AddToImportQueue(ImportJobInformation jobInformation)
		{
			_importFileThreadPool.Push(new FileImportJob(jobInformation));
		}

		#region Database Related Methods

		private void AddToDatabaseQueue(ImportJobInformation jobInformation)
		{
			lock (_syncLock)
			{
				_databaseUpdateItems.Add(jobInformation);
			}
		}

		private void UpdateDatabase(List<ImportJobInformation> items)
		{
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
					((IFileImportInformation)item.FileImportInformation).CompletedStage = ImportStage.CommittedToDataStore;
					string error = String.Format("Failed to commit a file to the datastore ({0}); This file will need to be imported manually.", item.FileImportInformation.StoredFile);
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
			while (!_stopDatabaseQueueSignal.WaitOne(waitTimeout, false))
			{
				List<ImportJobInformation> updates = new List<ImportJobInformation>();
				lock(_syncLock)
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
			}
		}

		#endregion

		#region Start/Stop Methods

		public void Start()
		{
			if (_databaseUpdateThread != null)
				throw new InvalidOperationException("The file importer has already been started.");

			_parseFileThreadPool.Start();

			_importFileThreadPool.Start();

			_stopDatabaseQueueSignal.Reset();
			ThreadStart threadStart = new ThreadStart(this.DatabaseUpdateThread);
			_databaseUpdateThread = new Thread(threadStart);
			_databaseUpdateThread.IsBackground = true;
			_databaseUpdateThread.Priority = ThreadPriority.BelowNormal;
			_databaseUpdateThread.Start();
		}

		public void Stop()
		{
			if (_databaseUpdateThread == null)
				throw new InvalidOperationException("The file importer is not running.");

			_parseFileThreadPool.Stop();

			_importFileThreadPool.Stop();
			
			_stopDatabaseQueueSignal.Set();
			_databaseUpdateThread.Join();
			_databaseUpdateThread = null;
		}

		#endregion
	}
}
