using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		public sealed class DicomFileImporter
		{
			public enum DedicatedImportQueue
			{
				None = 0,
				Import,
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

				DicomAttributeCollection MetaInfo { get; set; }
				DicomAttributeCollection DataSet { get; set; }
			}

			public class FileImportInformation : IFileImportInformation
			{
				#region Private Fields

				#region Client Input

				private readonly string _sourceFile;
				private readonly FileImportBehaviour _importBehaviour;
				private readonly BadFileBehaviour _badFileBehaviour;

				#endregion

				#region Results / Errors

				private ImportStage _completedStage;
				private Exception _error;
				private bool _badFile;
				private string _storedFile;

				#endregion

				#region Parsed File Information

				private DicomAttributeCollection _metaInfo;
				private DicomAttributeCollection _dataSet;

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
					get
					{
						if (_dataSet == null)
							return "";
						
						return _dataSet[DicomTags.PatientId] ?? "";
					}
				}

				public string PatientsName
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.PatientsName] ?? "";
					}
				}

				public string StudyDate
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.StudyDate] ?? "";
					}
				}

				public string StudyDescription
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.StudyDescription] ?? "";
					}
				}

				public string StudyInstanceUid
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.StudyInstanceUid] ?? "";
					}

				}

				public string SeriesInstanceUid
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.SeriesInstanceUid] ?? "";
					}
				}

				public string SopInstanceUid
				{
					get
					{
						if (_dataSet == null)
							return "";

						return _dataSet[DicomTags.SopInstanceUid] ?? "";
					}
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

				DicomAttributeCollection IFileImportInformation.MetaInfo
				{
					get { return _metaInfo; }
					set { _metaInfo = value; }
				}

				DicomAttributeCollection IFileImportInformation.DataSet
				{
					get { return _dataSet; }
					set { _dataSet = value; }
				}

				#endregion
			}

			internal delegate void FileImportJobStatusReportDelegate(FileImportInformation results);

			#region Import Job Information

			private class ImportJobInformation
			{
				private readonly FileImportInformation _fileImportInformation;
				private readonly FileImportJobStatusReportDelegate _fileImportJobStatusReportDelegate;
				private readonly DedicatedImportQueue _destinationImportQueue;

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

			private class ParseFileThreadPool : BlockingThreadPool<ImportJobInformation>
			{
				readonly DicomFileImporter _parent;

				public ParseFileThreadPool(DicomFileImporter parent)
					: base((int)LocalDataStoreService.Instance.SendReceiveImportConcurrency, true)
				{
					_parent = parent;
				}

				protected override void ProcessItem(ImportJobInformation jobInformation)
				{
					_parent.ParseFile(jobInformation);

					if (!jobInformation.FileImportInformation.Failed)
						_parent.AddToImportQueue(jobInformation);
				}
			}

			private class ImportFileThreadPool : BlockingThreadPool<ImportJobInformation>
			{
				readonly DicomFileImporter _parent;

				public ImportFileThreadPool(DicomFileImporter parent)
					: base((int)LocalDataStoreService.Instance.SendReceiveImportConcurrency, true)
				{
					_parent = parent;
				}

				protected override void ProcessItem(ImportJobInformation jobInformation)
				{
					_parent.MoveFile(jobInformation);

					if (!jobInformation.FileImportInformation.Failed)
						_parent.AddToDatabaseQueue(jobInformation);
				}
			}

			/// <summary>
			/// An unsophisticated way to provide a mutually exclusive lock on a file that is about to get
			/// written to (or deleted or copied over) by any one of the threads in the import thread pool.
			/// This is really only for the (very unlikely) case where you 'receive' the same image at the same
			/// time from multiple sources.  For example, you could be importing from a directory at the same time
			/// as you are retrieving a study, at the same time some other server is sending to you, and identical
			/// images could be coming in from all 3 places.
			/// </summary>
			private class UriLock
			{
				private readonly Uri _uri;
				private int _lockCount;

				public UriLock(Uri uri)
				{
					_uri = uri;
					_lockCount = 0;
				}

				public Uri Uri { get { return _uri; } }
				public int LockCount { get { return _lockCount; } }
				public void IncrementLockCount() { ++_lockCount; }
				public void DecrementLockCount() { --_lockCount; }
			}

			private LocalDataStoreService _parent;

			private readonly object _databaseThreadLock = new object();
			private bool _stopDatabaseThread = false;
			private Thread _databaseUpdateThread;

			private readonly object _databaseUpdateItemsLock = new object();
			private readonly List<ImportJobInformation> _databaseUpdateItems;
			
			private readonly ParseFileThreadPool _parseFileThreadPool;

			private DedicatedImportQueue _activeImportThreadPool;
			private readonly Dictionary<DedicatedImportQueue, ImportFileThreadPool> _importThreadPools;

			private readonly object _importThreadPoolSwitchSyncLock = new object();
			private event EventHandler<ItemEventArgs<DedicatedImportQueue>> _importThreadPoolSwitched;

			private readonly object _uriLocksSync = new object();
			private readonly List<UriLock> _uriLocks;

			public DicomFileImporter(LocalDataStoreService parent)
				: base()
			{
				_parent = parent;
				_parseFileThreadPool = new ParseFileThreadPool(this);

				_importThreadPools = new Dictionary<DedicatedImportQueue, ImportFileThreadPool>();
				_importThreadPools.Add(DedicatedImportQueue.Import, new ImportFileThreadPool(this));
				_importThreadPools.Add(DedicatedImportQueue.Reindex, new ImportFileThreadPool(this));
				_activeImportThreadPool = DedicatedImportQueue.Import;

				_databaseUpdateItems = new List<ImportJobInformation>();

				_uriLocks = new List<UriLock>();
			}

			#region Uri Locking
			
			private Uri GetLockUri(string filePath)
			{
				lock (_uriLocksSync)
				{
					UriBuilder searchUri = new UriBuilder();
					searchUri.Scheme = "file";
					searchUri.Path = filePath;

					UriLock uriLock = _uriLocks.Find(delegate(UriLock test) { return test.Uri.AbsoluteUri == searchUri.Uri.AbsoluteUri; });
					if (uriLock == null)
					{
						uriLock = new UriLock(searchUri.Uri);
						_uriLocks.Add(uriLock);
						uriLock.IncrementLockCount();
					}
					else
					{
						uriLock.IncrementLockCount();
					}

					return uriLock.Uri;
				}
			}

			private void ReleaseLockUri(Uri releaseUri)
			{
				lock (_uriLocksSync)
				{
					UriLock uriLock = _uriLocks.Find(delegate(UriLock test) { return test.Uri == releaseUri; });
					if (uriLock != null)
					{
						uriLock.DecrementLockCount();

						if (0 == uriLock.LockCount)
							_uriLocks.Remove(uriLock);
					}
				}
			}

			#endregion

			#region File Parse / Import Methods

			private bool ConfirmProcessableFile(DicomAttributeCollection metaInfo, DicomAttributeCollection dataset)
			{
				DicomAttribute attribute = metaInfo[DicomTags.MediaStorageSopClassUid];
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == attribute.ToString())
					return false;

				return true;
			}

			private void ParseFile(ImportJobInformation jobInformation)
			{
				FileImportInformation fileImportInformation = jobInformation.FileImportInformation;
				FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate = jobInformation.FileImportJobStatusReportDelegate;

				DicomFile dicomFile = new DicomFile(fileImportInformation.SourceFile);

				//use this private interface to set the values as we go along.
				IFileImportInformation setImportInformation = (IFileImportInformation)fileImportInformation;

				try
				{
					Exception exception = null;
					bool processable = true;

					try
					{
						dicomFile.Load(DicomTags.PixelData, DicomReadOptions.Default);

						processable = ConfirmProcessableFile(dicomFile.MetaInfo, dicomFile.DataSet);

						if (processable)
						{
							//parse the file and create study, series and sop objects for (potential) insertion into the datastore.
							setImportInformation.MetaInfo = dicomFile.MetaInfo;
							setImportInformation.DataSet = dicomFile.DataSet;
							ValidateDicomData(dicomFile.MetaInfo, dicomFile.DataSet);
						}
					}
					catch (Exception e)
					{
						exception = e;
					}

					if (!processable || exception != null)
					{
						StringBuilder errorString = new StringBuilder(512);
						if (!processable)
							errorString.AppendFormat(SR.FormatFileFormatNotAppropriateForDatastore, fileImportInformation.SourceFile);
						else
							errorString.AppendFormat(SR.FormatFileFormatNotRecognized, fileImportInformation.SourceFile);

						if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Move)
						{
							string storedFile = String.Format("{0}{1}", LocalDataStoreService.Instance.BadFileDirectory, System.IO.Path.GetRandomFileName());
							System.IO.File.Move(fileImportInformation.SourceFile, storedFile);
							((IFileImportInformation)fileImportInformation).StoredFile = storedFile;

							errorString.AppendFormat(SR.FormatFileHasBeenMoved, storedFile);
						}
						else if (fileImportInformation.BadFileBehaviour == BadFileBehaviour.Delete)
						{
							System.IO.File.Delete(fileImportInformation.SourceFile);

							errorString.AppendFormat(SR.MessageFileHasBeenDeleted);
						}

						if (exception == null)
							setImportInformation.Error = new Exception(errorString.ToString());
						else
							setImportInformation.Error = new Exception(errorString.ToString(), exception);

						setImportInformation.BadFile = true;
						fileImportJobStatusReportDelegate(fileImportInformation);
						return;
					}

					//report the progress.
					setImportInformation.CompletedStage = ImportStage.FileParsed;
					fileImportJobStatusReportDelegate(fileImportInformation);
				}
				catch (Exception e)
				{
					setImportInformation.Error = new Exception(SR.ExceptionFailedToParseFile, e);
					fileImportJobStatusReportDelegate(fileImportInformation);
				}
			}

			private void ValidateDicomData(DicomAttributeCollection metaInfo, DicomAttributeCollection dataSet)
			{
				Platform.CheckForNullReference(metaInfo, "metaInfo");
				Platform.CheckForNullReference(dataSet, "dataSet");

				DicomValidator.ValidateStudyInstanceUID(dataSet[DicomTags.StudyInstanceUid]);
				DicomValidator.ValidateSeriesInstanceUID(dataSet[DicomTags.SeriesInstanceUid]);
				DicomValidator.ValidateSOPInstanceUID(dataSet[DicomTags.SopInstanceUid]);
				DicomValidator.ValidateTransferSyntaxUID(metaInfo[DicomTags.TransferSyntaxUid]);
			}

			private void MoveFile(ImportJobInformation jobInformation)
			{
				FileImportInformation fileImportInformation = jobInformation.FileImportInformation;
				FileImportJobStatusReportDelegate fileImportJobStatusReportDelegate = jobInformation.FileImportJobStatusReportDelegate;

				IFileImportInformation importerInformation = (IFileImportInformation)fileImportInformation;

				try
				{
					UriBuilder sourceUri = new UriBuilder();
					sourceUri.Scheme = "file";
					sourceUri.Path = fileImportInformation.SourceFile;

					string storedFile = String.Format("{0}{1}\\{2}.dcm", LocalDataStoreService.Instance.StorageDirectory,
																				fileImportInformation.StudyInstanceUid,
																				fileImportInformation.SopInstanceUid);

					Uri storedUri = GetLockUri(storedFile);

					try
					{
						if (storedUri.AbsolutePath != sourceUri.Uri.AbsolutePath)
						{
							//by locking on the Uri, two threads will only ever block each other if the file they are attempting
							//to write to is exactly the same one, which will happen very infrequently.

							lock (storedUri)
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
					}
					}
					finally
					{
						ReleaseLockUri(storedUri);
					}

					importerInformation.StoredFile = storedFile;
					importerInformation.CompletedStage = ImportStage.FileMoved;
				}
				catch (Exception e)
				{
					importerInformation.Error = new Exception(SR.ExceptionFailedToImportFile, e);
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

			private void StartDatabaseThread()
			{
				lock (_databaseThreadLock)
				{
					if (_databaseUpdateThread != null)
						return;

					_stopDatabaseThread = false;
					ThreadStart threadStart = new ThreadStart(this.DatabaseUpdateThread);
					_databaseUpdateThread = new Thread(threadStart);
					_databaseUpdateThread.IsBackground = true;
					_databaseUpdateThread.Priority = ThreadPriority.BelowNormal;
					_databaseUpdateThread.Start();
					Monitor.Wait(_databaseThreadLock); //wait for the signal that the thread has started.
				}
			}

			private void StopDatabaseThread()
			{
				lock (_databaseThreadLock)
				{
					if (_databaseUpdateThread == null)
						return;

					_stopDatabaseThread = true;
					Monitor.Pulse(_databaseThreadLock);
					Monitor.Wait(_databaseThreadLock); //wait for the signal that the thread has ended.

					_databaseUpdateThread.Join();
					_databaseUpdateThread = null;
				}
			}

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

#if DEBUG
				CodeClock clock = new CodeClock();
				clock.Start();
#endif
				try
				{
					using (IDicomPersistentStore store = DataAccessLayer.GetIDicomPersistentStore())
					{
						foreach (ImportJobInformation item in items)
						{
							IFileImportInformation info = (IFileImportInformation)item.FileImportInformation;
							store.UpdateSopInstance(info.MetaInfo, info.DataSet, item.FileImportInformation.StoredFile);
						}

						store.Commit();
					}

					foreach (ImportJobInformation item in items)
					{
						((IFileImportInformation)item.FileImportInformation).CompletedStage = ImportStage.CommittedToDataStore;
						item.FileImportJobStatusReportDelegate(item.FileImportInformation);
					}
				}
				catch (Exception e)
				{
					foreach (ImportJobInformation item in items)
					{
						string error = String.Format(SR.FormatFailedToCommitToDatastore, item.FileImportInformation.StoredFile);
						((IFileImportInformation)item.FileImportInformation).Error = new Exception(error, e);
						item.FileImportJobStatusReportDelegate(item.FileImportInformation);
					}
				}
#if DEBUG
				clock.Stop();
				Console.WriteLine(String.Format("Update took {0} seconds", clock.Seconds));
#endif
			}

			private void DatabaseUpdateThread()
			{
				uint waitTimeout = LocalDataStoreService.Instance.DatabaseUpdateFrequencyMilliseconds;

				lock (_databaseThreadLock)
				{
					Monitor.Pulse(_databaseThreadLock); //signal that the thread has started.

					while (true)
					{
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
							Platform.Log(LogLevel.Error, e);
						}

						if (_stopDatabaseThread)
							break;

						//putting the wait here is important because it will update the database
						//one last time before exiting in case there was any data left to insert.
						Monitor.Wait(_databaseThreadLock, (int)waitTimeout);
					}

					Monitor.Pulse(_databaseThreadLock); //signal that the thread is exiting.
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
				_parseFileThreadPool.Start();
				_importThreadPools[_activeImportThreadPool].Start();

				StartDatabaseThread();
			}

			public void Stop()
			{
				_parseFileThreadPool.Stop();

				foreach (ImportFileThreadPool pool in _importThreadPools.Values)
					pool.Stop();

				StopDatabaseThread();
			}

			#endregion

			public void ActivateImportQueue(DedicatedImportQueue queue)
			{
				lock (_importThreadPoolSwitchSyncLock)
				{
					if (_activeImportThreadPool == queue)
						return;

					if (_activeImportThreadPool != DedicatedImportQueue.None)
					{
						_importThreadPools[_activeImportThreadPool].Stop();
						StopDatabaseThread();
					}

					_activeImportThreadPool = queue;

					if (_activeImportThreadPool != DedicatedImportQueue.None)
					{
						StartDatabaseThread();
						_importThreadPools[_activeImportThreadPool].Start();
					}

					EventsHelper.Fire(_importThreadPoolSwitched, this, new ItemEventArgs<DedicatedImportQueue>(_activeImportThreadPool));
				}
			}
		}
	}
}