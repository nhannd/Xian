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
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

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
					: base(file, importBehaviour, badFileBehaviour)
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

			private void AddNextFileToImportQueue(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (jobInformation.ProgressItem.Removed)
						return;

					if (jobInformation.ProgressItem.Cancelled)
						return;

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
						return;
					}

					jobInformation.ProgressItem.LastActive = Platform.Time;

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
						}

						if (jobInformation.ProgressItem.IsComplete())
						{
							jobInformation.ProgressItem.StatusMessage = SR.MessageCompleteWithFailures;
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
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
						}

						AddNextFileToImportQueue(jobInformation);
					}
					else if (results.CompletedStage == DicomFileImporter.ImportStage.CommittedToDataStore)
					{
						++jobInformation.ProgressItem.NumberOfFilesCommittedToDataStore;

						if (jobInformation.ProgressItem.IsComplete())
						{
							jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
							
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