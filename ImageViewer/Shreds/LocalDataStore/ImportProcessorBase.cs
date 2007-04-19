using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common;
using System.IO;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private abstract class ImportProcessorBase
		{
			protected delegate void NotifyNoFilesToImportDelegate(FileImportJobInformation jobInformation);

			protected class FileImportInformation : DicomFileImporter.FileImportInformation
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

			protected class FileImportJobInformation
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
			}

			protected virtual void CancelJob(FileImportJobInformation jobInformation)
			{
				lock (jobInformation)
				{
					if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel))
						return;

					if (jobInformation.ProgressItem.Cancelled)
						return;

					if (jobInformation.ProgressItem.IsImportComplete())
						return;

					jobInformation.ProgressItem.Cancelled = true;
					jobInformation.ProgressItem.StatusMessage = SR.MessageCancelled;
					jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
					UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected virtual void ClearJob(FileImportJobInformation jobInformation)
			{
				lock (jobInformation)
				{
					if (!((jobInformation.ProgressItem.AllowedCancellationOperations & CancellationFlags.Clear) == CancellationFlags.Clear))
						return;

					if (!jobInformation.ProgressItem.Cancelled && !jobInformation.ProgressItem.IsImportComplete())
						return;

					jobInformation.ProgressItem.Removed = true;
					jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.None;
					UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
			{
				FileImportInformation information = (FileImportInformation)results;

				FileImportJobInformation jobInformation = information.FileImportJobInformation;

				ImportedSopInstanceInformation importedSopInformation = null;

				lock (jobInformation)
				{
					if (jobInformation.ProgressItem.Removed)
					{
						//forget it, just return.
						return;
					}

					jobInformation.ProgressItem.LastActive = DateTime.Now;

					bool updateProgress = false;

					if (results.Failed)
					{
						++jobInformation.ProgressItem.NumberOfFailedImports;
						Platform.Log(results.Error);

						if (jobInformation.ProgressItem.IsImportComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageWaitingForFilesToBecomeAvailable;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						}

						if (jobInformation.ProgressItem.IsComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageComplete;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						}

						updateProgress = true;

						AddNextFileToImportQueue(jobInformation);
					}
					else if (results.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
					{
						++jobInformation.ProgressItem.NumberOfFilesImported;

						if (jobInformation.ProgressItem.IsImportComplete())
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

						if (jobInformation.ProgressItem.IsComplete())
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
						UpdateProgress(jobInformation.ProgressItem);
				}
			}

			protected abstract void UpdateProgress(ImportProgressItem progressItem);

			protected void AddNextFileToImportQueue(FileImportJobInformation jobInformation)
			{
				lock (jobInformation)
				{
					if (jobInformation.ProgressItem.Removed)
						return;

					if (jobInformation.ProgressItem.Cancelled)
						return;

					string file = jobInformation.Dequeue();
					if (file == null)
						return;

					jobInformation.ProgressItem.StatusMessage = String.Format(SR.FormatProcessingFile, file);

					FileImportInformation fileImportInformation = new FileImportInformation(jobInformation, file, jobInformation.FileImportBehaviour, jobInformation.BadFileBehaviour);

					this.AddToImportQueue(fileImportInformation);
				}
			}

			protected abstract void AddToImportQueue(FileImportInformation fileImportInformation);

			protected abstract void NotifyNoFilesToImport(FileImportJobInformation jobInformation);

			protected virtual void Import(
				FileImportJobInformation jobInformation, 
				IList<string> filePaths, 
				IList<string> fileExtensions, 
				bool recursive)
			{
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

					foreach (string path in filePaths)
					{
                        if (Directory.Exists(path) == false)
                            continue;

						FileProcessor.Process(path, "",
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
									lock (jobInformation)
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
					}

					lock (jobInformation)
					{
						if (!jobInformation.ProgressItem.Cancelled)
						{
							if (jobInformation.NumberOfFilesInQueue == 0)
							{
								NotifyNoFilesToImport(jobInformation);
							}
							else
							{
								AddNextFileToImportQueue(jobInformation);
							}
						}

						UpdateProgress(jobInformation.ProgressItem);
					}
				};

				ThreadPool.QueueUserWorkItem(enumerateFilesToImport);
			}
		}
	}
}