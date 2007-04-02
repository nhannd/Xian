using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.IO;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class ImportQueue
	{
		private class FileImportInformation : DicomFileImporter.FileImportInformation
		{
			private FileImportJobInformation _fileImportJobInformation;

			public FileImportInformation(FileImportJobInformation jobInformation, string file, FileImportBehaviour importBehaviour, BadFileBehaviour badFileBehaviour)
				: base(file, importBehaviour, badFileBehaviour)
			{
				_fileImportJobInformation = jobInformation;
			}

			public FileImportJobInformation FileImportJobInformation
			{
				get { return _fileImportJobInformation; }
			}
		}

		private class FileImportJobInformation
		{
			private ImportProgressItem _progressItem;
			private Queue<string> _filesToImport;
			private BadFileBehaviour _badFileBehaviour;
			private FileImportBehaviour _fileImportBehaviour;

			public FileImportJobInformation(ImportProgressItem progressItem, FileImportBehaviour fileImportBehaviour, BadFileBehaviour badFileBehaviour)
			{
				_fileImportBehaviour = fileImportBehaviour;
				_badFileBehaviour = badFileBehaviour;
				_progressItem = progressItem;
				_filesToImport = new Queue<string>();
			}

			public ImportProgressItem ProgressItem
			{
				get { return _progressItem; }
			}

			public FileImportBehaviour FileImportBehaviour
			{
				get { return _fileImportBehaviour; }
			}

			public BadFileBehaviour BadFileBehaviour
			{
				get { return _badFileBehaviour; }
			}

			public void Add(string file)
			{
				_filesToImport.Enqueue(file);
			}

			public string NextFile()
			{
				if (_filesToImport.Count == 0)
					return null;

				return _filesToImport.Dequeue();
			}
		}

		private object _importJobsLock = new object();
		private List<FileImportJobInformation> _importJobs;

		public ImportQueue()
		{
			_importJobs = new List<FileImportJobInformation>();
		}

		private void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
		{
			FileImportInformation information = (FileImportInformation)results;

			FileImportJobInformation jobInformation = information.FileImportJobInformation;

			ImportedSopInstanceInformation importedSopInformation = null;

			lock (jobInformation)
			{
				jobInformation.ProgressItem.LastActive = DateTime.Now;

				if (jobInformation.ProgressItem.Removed)
				{ 
					//forget it, just return.
					return;
				}
				
				bool updateProgress = false;

				if (results.Failed)
				{
					++jobInformation.ProgressItem.NumberOfFailedImports;
					Platform.Log(results.Error);

					updateProgress = true;
					
					AddNextFileToImportQueue(jobInformation);
				}
				else if (results.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
				{
					++jobInformation.ProgressItem.NumberOfFilesImported;

					if (jobInformation.ProgressItem.TotalFilesToImport == (jobInformation.ProgressItem.NumberOfFilesImported + jobInformation.ProgressItem.NumberOfFailedImports))
					{
						jobInformation.ProgressItem.StatusMessage = SR.MessageWaitingForFilesToBecomeAvailable; 
						jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
					}
					
					updateProgress = true;

					AddNextFileToImportQueue(jobInformation);
				}
				else if (results.CompletedStage == DicomFileImporter.ImportStage.CommittedToDataStore)
				{
					++jobInformation.ProgressItem.NumberOfFilesCommittedToDataStore;

					if (jobInformation.ProgressItem.TotalFilesToImport == (jobInformation.ProgressItem.NumberOfFilesCommittedToDataStore + jobInformation.ProgressItem.NumberOfFailedImports))
					{
						jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						jobInformation.ProgressItem.StatusMessage = SR.MessageComplete;
					}

					updateProgress = true;

					importedSopInformation = new ImportedSopInstanceInformation();
					importedSopInformation.StudyInstanceUid = results.StudyInstanceUid;
					importedSopInformation.SeriesInstanceUid = results.SeriesInstanceUid;
					importedSopInformation.SopInstanceUid = results.SopInstanceUid;
					importedSopInformation.SopInstanceFileName = results.StoredFile;
				}

				if (importedSopInformation != null)
					LocalDataStoreActivityPublisher.Instance.SopInstanceImported(importedSopInformation);

				if (updateProgress)
					LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(jobInformation.ProgressItem.Clone());
			}
		}

		private void ValidateRequest(FileImportRequest request)
		{
			if (request.FilePaths == null)
				throw new ArgumentNullException(SR.ExceptionNoFilesHaveBeenSpecifiedToImport);

			int paths = 0;
			int badPaths = 0;

			foreach (string path in request.FilePaths)
			{
				if (Directory.Exists(path) || File.Exists(path))
					++paths;
				else
					++badPaths;
			}

			if (paths == 0)
				throw new ArgumentNullException(SR.ExceptionNoValidFilesHaveBeenSpecifiedToImport);
		}

		private void AddNextFileToImportQueue(FileImportJobInformation jobInformation)
		{
			lock (jobInformation)
			{
				if (jobInformation.ProgressItem.Cancelled)
					return;

				string file = jobInformation.NextFile();
				if (file == null)
					return;

				jobInformation.ProgressItem.StatusMessage = String.Format(SR.FormatImportingFile, file);

				FileImportInformation fileImportInformation = new FileImportInformation(jobInformation, file, jobInformation.FileImportBehaviour, jobInformation.BadFileBehaviour);
				LocalDataStoreService.Instance.DicomFileImporter.Enqueue(fileImportInformation, this.ProcessFileImportResults);			
			}
		}

		public Guid Import(FileImportRequest request)
		{
			ValidateRequest(request);

			ImportProgressItem progressItem = new ImportProgressItem();

			progressItem.Identifier = Guid.NewGuid();
			progressItem.AllowedCancellationOperations = CancellationFlags.Cancel;
			progressItem.StartTime = DateTime.Now;
			progressItem.LastActive = progressItem.StartTime;

			progressItem.NumberOfFailedImports = 0;
			progressItem.NumberOfFilesImported = 0;
			progressItem.TotalFilesToImport = 0;
			progressItem.StatusMessage = SR.MessagePending;

			FileImportJobInformation jobInformation = new FileImportJobInformation(progressItem, request.FileImportBehaviour, request.BadFileBehaviour);

			lock (_importJobsLock)
			{
				_importJobs.Add(jobInformation);
			}

			lock(jobInformation)
			{
				LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
			}

			WaitCallback enumerateFilesToImport = delegate(object nothing)
			{
				List<string> extensions = new List<string>();
				if (request.FileExtensions != null)
				{
					foreach (string extension in request.FileExtensions)
					{
						if (String.IsNullOrEmpty(extension))
							continue;

						string addExtension = extension;
						if (addExtension[0] != '.')
							addExtension = String.Format(".{0}", extension);

						extensions.Add(extension);
					}
				}

				lock (jobInformation)
				{
					LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
				}

				int numberOfPaths = 0;
				string firstPath = "";
				foreach (string path in request.FilePaths)
				{
					if (firstPath == "")
						firstPath = path;

					++numberOfPaths;

					FileProcessor.Process(path, "", 
						delegate(string file)
						{
							bool enqueue = false;
							foreach(string extension in extensions)
							{
								if (file.EndsWith(extension))
								{	
									enqueue = true;
									break;
								}
							}
							
							enqueue = enqueue || extensions.Count == 0;

							if (enqueue)
							{
								lock (jobInformation)
								{
									if (jobInformation.ProgressItem.Cancelled)
										return;

									jobInformation.ProgressItem.StatusMessage = String.Format(SR.FormatAddingFile, file);

									jobInformation.Add(file);
									++progressItem.TotalFilesToImport;
									LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
								}
							}
							
						}, request.Recursive);
				}

				if (numberOfPaths > 1)
				{
					progressItem.Description = String.Format(SR.FormatMultipleFilesDescription, firstPath);
				}
				else
				{
					progressItem.Description = firstPath;
				}

				AddNextFileToImportQueue(jobInformation);

				lock (jobInformation)
				{
					LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
				}
			};

			ThreadPool.QueueUserWorkItem(enumerateFilesToImport);

			return progressItem.Identifier;
		}

		private void CancelJob(FileImportJobInformation jobInformation)
		{
			lock (jobInformation)
			{
				if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel))
					return;

				if (jobInformation.ProgressItem.Cancelled)
					return;

				jobInformation.ProgressItem.Cancelled = true;
				jobInformation.ProgressItem.StatusMessage = SR.MessageCancelled;
				jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
				LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(jobInformation.ProgressItem.Clone());
			}
		}

		private void ClearJob(FileImportJobInformation jobInformation)
		{
			lock (jobInformation)
			{
				if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Clear) == CancellationFlags.Clear))
					return;
				
				jobInformation.ProgressItem.Removed = true;
				jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.None;
				LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(jobInformation.ProgressItem.Clone());
			}

			lock (_importJobsLock)
			{
				if (_importJobs.Contains(jobInformation))
					_importJobs.Remove(jobInformation);
			}
		}

		public void Start()
		{
			//nothing to do.
		}

		public void Stop()
		{
			//nothing to do.
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			if (information.ProgressItemIdentifiers == null)
				return;

			if ((information.CancellationFlags & CancellationFlags.Cancel) == CancellationFlags.Cancel)
			{
				foreach (Guid guid in information.ProgressItemIdentifiers)
				{
					FileImportJobInformation jobInformation = null;
					lock (_importJobsLock)
					{
						jobInformation = _importJobs.Find(delegate(FileImportJobInformation test) { return test.ProgressItem.Identifier.Equals(guid); });
					}

					if (jobInformation != null)
					{
						CancelJob(jobInformation);
					}
				}
			}

			if ((information.CancellationFlags & CancellationFlags.Clear) == CancellationFlags.Clear)
			{
				foreach (Guid guid in information.ProgressItemIdentifiers)
				{
					FileImportJobInformation jobInformation = null;
					lock (_importJobsLock)
					{
						jobInformation = _importJobs.Find(delegate(FileImportJobInformation test) { return test.ProgressItem.Identifier.Equals(guid); });
					}

					if (jobInformation != null)
					{
						ClearJob(jobInformation);
					}
				}
			}
		}

		public void ClearInactive()
		{
			List<FileImportJobInformation> clearJobs = new List<FileImportJobInformation>();
			lock (_importJobsLock)
			{
				clearJobs.AddRange(_importJobs);
			}

			foreach (FileImportJobInformation jobInformation in clearJobs)
			{
				ClearJob(jobInformation);
			}
		}

		public void RepublishAll()
		{
			lock (_importJobsLock)
			{
				foreach (FileImportJobInformation jobInformation in _importJobs)
				{
					lock (jobInformation)
					{
						LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(jobInformation.ProgressItem.Clone());
					}
				}
			}
		}
	}
}
