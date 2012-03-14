#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private abstract class ImportProcessorBase
		{
			protected class FileImportInformation : DicomFileImporter.FileImportInformation
			{
				private readonly FileImportJobInformation _fileImportJobInformation;

				public FileImportInformation(FileImportJobInformation jobInformation, string file, FileImportBehaviour importBehaviour, BadFileBehaviour badFileBehaviour)
					: base(file, importBehaviour, badFileBehaviour, jobInformation.UserIdentityContext)
				{
					_fileImportJobInformation = jobInformation;
				}

				public FileImportJobInformation FileImportJobInformation
				{
					get { return _fileImportJobInformation; }
				}

				public void DisableAuditing()
				{
					base.Audit = false;
				}
			}

			protected class FileImportJobInformation
			{
				public readonly object SyncRoot = new object();

				private readonly ImportProgressItem _progressItem;
				private readonly Queue<string> _filesToImport;
				private readonly BadFileBehaviour _badFileBehaviour;
				private readonly FileImportBehaviour _fileImportBehaviour;
				private UserIdentityContext _userIdentityContext;

				public FileImportJobInformation(ImportProgressItem progressItem, FileImportBehaviour fileImportBehaviour, BadFileBehaviour badFileBehaviour)
					: this(progressItem, fileImportBehaviour, badFileBehaviour, null) {}

				public FileImportJobInformation(ImportProgressItem progressItem, FileImportBehaviour fileImportBehaviour, BadFileBehaviour badFileBehaviour, UserIdentityContext userIdentityContext)
				{
					_fileImportBehaviour = fileImportBehaviour;
					_badFileBehaviour = badFileBehaviour;
					_progressItem = progressItem;
					_userIdentityContext = userIdentityContext ?? new UserIdentityContext();
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

				public UserIdentityContext UserIdentityContext
				{
					get { return _userIdentityContext; }
				}

				public int NumberOfFilesInQueue
				{
					get { return _filesToImport.Count; }
				}

				public void Enqueue(string file)
				{
					_filesToImport.Enqueue(file);
				}

				public string Dequeue()
				{
					if (_filesToImport.Count == 0)
						return null;

					return _filesToImport.Dequeue();
				}

				public void DisposeClientUserContext()
				{
					if (_userIdentityContext != null)
					{
						_userIdentityContext.Dispose();
						_userIdentityContext = null;
					}
				}
			}

			private void AddNextFileToImportQueue(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (jobInformation.ProgressItem.Removed)
					{
						jobInformation.DisposeClientUserContext();
						return;
					}

					if (jobInformation.ProgressItem.Cancelled)
					{
						jobInformation.DisposeClientUserContext();
						return;
					}

					string file = jobInformation.Dequeue();
					if (file == null)
						return;

					jobInformation.ProgressItem.StatusMessage = String.Format(SR.FormatProcessingFile, file);
					UpdateProgress(jobInformation.ProgressItem);

					FileImportInformation fileImportInformation =
						new FileImportInformation(jobInformation, file, jobInformation.FileImportBehaviour,
						                          jobInformation.BadFileBehaviour);

					if(this.GetType() == typeof(ReindexProcessor))
						fileImportInformation.DisableAuditing();

					AddToImportQueue(fileImportInformation);
				}
			}

			protected bool CanCancelJob(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel))
						return false;

					if (jobInformation.ProgressItem.Cancelled)
						return false;

					if (jobInformation.ProgressItem.TotalFilesToImport > 0 && jobInformation.ProgressItem.IsImportComplete())
						return false;
				}

				return true;
			}

			protected void CancelJob(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (!CanCancelJob(jobInformation))
						return;

					jobInformation.ProgressItem.Cancelled = true;
					jobInformation.ProgressItem.StatusMessage = SR.MessageCancelled;
					jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
					if (jobInformation.ProgressItem.IsImportComplete())
						jobInformation.DisposeClientUserContext();

					UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected bool CanClearJob(FileImportJobInformation jobInformation)
			{ 
				lock (jobInformation.SyncRoot)
				{
					if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Clear) == CancellationFlags.Clear))
						return false;

					if (!jobInformation.ProgressItem.Cancelled && jobInformation.ProgressItem.TotalFilesToImport > 0 && !jobInformation.ProgressItem.IsImportComplete())
						return false;
				}

				return true;
			}

			protected virtual void ClearJob(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (!CanClearJob(jobInformation))
						return;

					jobInformation.ProgressItem.Removed = true;
					jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.None;
					jobInformation.DisposeClientUserContext();

					UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
			{
				FileImportInformation information = (FileImportInformation)results;

				FileImportJobInformation jobInformation = information.FileImportJobInformation;

				ImportedSopInstanceInformation importedSopInformation = null;

				lock (jobInformation.SyncRoot)
				{
					if (jobInformation.ProgressItem.Removed)
					{
						//forget it, just return.
						jobInformation.DisposeClientUserContext();
						return;
					}

					//TODO (Time Review): Change this back to use Platform.Time once we've resolved
					//the exception throwing issue.
					jobInformation.ProgressItem.LastActive = DateTime.Now;

					if (results.Failed)
					{
						//When a failure is reported, it is because the 'next' stage failed, hence the stage 
						//that was attempted did not get completed.  This is why the logic below is the way it is.
						switch (results.CompletedStage)
						{ 
							case DicomFileImporter.ImportStage.NotStarted:
								++jobInformation.ProgressItem.NumberOfParseFailures;
								break;
							case DicomFileImporter.ImportStage.FileParsed:
								++jobInformation.ProgressItem.NumberOfImportFailures;
								break;
							case DicomFileImporter.ImportStage.FileMoved:
								++jobInformation.ProgressItem.NumberOfDataStoreCommitFailures;
								break;
						}

						Platform.Log(LogLevel.Error, results.Error);

						if (jobInformation.ProgressItem.IsImportComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageWaitingForFilesToBecomeAvailable;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
							jobInformation.DisposeClientUserContext();
						}

						if (jobInformation.ProgressItem.IsComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageCompleteWithFailures;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
							jobInformation.DisposeClientUserContext();
						}

						AddNextFileToImportQueue(jobInformation);
					}
					else if (results.CompletedStage == DicomFileImporter.ImportStage.FileParsed)
					{
						++jobInformation.ProgressItem.NumberOfFilesParsed;
					}
					else if (results.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
					{
						++jobInformation.ProgressItem.NumberOfFilesImported;

						if (jobInformation.ProgressItem.IsImportComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageWaitingForFilesToBecomeAvailable;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
							jobInformation.DisposeClientUserContext();
						}

						AddNextFileToImportQueue(jobInformation);
					}
					else if (results.CompletedStage == DicomFileImporter.ImportStage.CommittedToDataStore)
					{
						++jobInformation.ProgressItem.NumberOfFilesCommittedToDataStore;

						if (jobInformation.ProgressItem.IsComplete())
						{
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
							jobInformation.DisposeClientUserContext();
							
							if (jobInformation.ProgressItem.TotalDataStoreCommitFailures == 0)
								jobInformation.ProgressItem.StatusMessage = SR.MessageComplete;
							else
								jobInformation.ProgressItem.StatusMessage = SR.MessageCompleteWithFailures;
						}

						importedSopInformation = new ImportedSopInstanceInformation();
						importedSopInformation.StudyInstanceUid = results.StudyInstanceUid;
						importedSopInformation.SeriesInstanceUid = results.SeriesInstanceUid;
						importedSopInformation.SopInstanceUid = results.SopInstanceUid;
						importedSopInformation.SopInstanceFileName = results.FileName;
					}

					if (importedSopInformation != null)
						LocalDataStoreActivityPublisher.Instance.SopInstanceImported(importedSopInformation);

					UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected abstract void AddToImportQueue(FileImportInformation fileImportInformation);

			protected abstract void UpdateProgress(ImportProgressItem progressItem);

			protected abstract void OnNoFilesToImport(FileImportJobInformation jobInformation);

			protected virtual void Import(
				FileImportJobInformation jobInformation, 
				IList<string> filePaths, 
				IList<string> fileExtensions, 
				bool recursive)
			{
				// no impersonation of the job's user identity context is necessary inside the task delegate since
				// the .NET thread pool automatically captures the current execution context for you, which includes
				// the impersonated client credentials if you're calling this from the import service implementation
				WaitCallback enumerateFilesToImport = delegate(object nothing)
				{
					List<string> extensions = new List<string>();
					if (fileExtensions.Count > 0)
					{
						foreach (string extension in fileExtensions)
						{
							if (String.IsNullOrEmpty(extension))
								continue;

							string addExtension = extension;
							if (addExtension[0] != '.')
								addExtension = String.Format(".{0}", extension);

							extensions.Add(extension);
						}
					}

					bool cancelled = false;

					foreach (string path in filePaths)
					{
						cancelled = FileProcessor.Process(path, "",
							delegate(string file, out bool cancel)
							{
								cancel = false;

								bool enqueue = false;
								foreach (string extension in extensions)
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
									lock (jobInformation.SyncRoot)
									{
										if (jobInformation.ProgressItem.Cancelled)
										{
											cancel = true;
											return;
										}

										jobInformation.ProgressItem.StatusMessage = String.Format(SR.FormatEnumeratingFile, file);
										jobInformation.Enqueue(file);
										++jobInformation.ProgressItem.TotalFilesToImport;

										UpdateProgress(jobInformation.ProgressItem);
									}
								}

							}, recursive);

						if (cancelled)
							break;
					}

					if (!cancelled)
					{
						//it's ok to read this property unsynchronized because this is the only thread that is adding to the queue for the particular job.
						if (jobInformation.NumberOfFilesInQueue == 0)
						{
							OnNoFilesToImport(jobInformation);
							jobInformation.DisposeClientUserContext();
						}
						else
						{
							AddNextFileToImportQueue(jobInformation);
						}
					}
				};

				ThreadPool.QueueUserWorkItem(enumerateFilesToImport);
			}
		}
	}
}