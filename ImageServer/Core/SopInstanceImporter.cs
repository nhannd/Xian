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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core
{
	public class DicomSopProcessingResult
	{
		public String AccessionNumber;
		public String StudyInstanceUid;
		public String SeriesInstanceUid;
		public String SopInstanceUid;
		public bool Sussessful;
		public String ErrorMessage;
		public DicomStatus DicomStatus;
		/// <summary>
		/// Indicates whether the sop being processed is a duplicate.
		/// </summary>
		/// <remarks>
		/// The result of the processing depends on the duplicate policy used.
		/// </remarks>
		public bool Duplicate;
	}

	public class SopInstanceImporter
	{
		private readonly ServerPartition _partition;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        
		public SopInstanceImporter(String partitionAE)
			:this(ServerPartitionMonitor.Instance.GetPartition(partitionAE))
		{
		}

		public SopInstanceImporter(ServerPartition partition)
		{
			Platform.CheckForNullReference(partition, "partition");
			_partition = partition;
		}

		private DicomSopProcessingResult Import(DicomMessageBase message, string sourceAE, string sourceId, string receiverId)
		{
			Platform.CheckForNullReference(message, "message");
			String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			String accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);

			DicomSopProcessingResult result = new DicomSopProcessingResult();
			result.Sussessful = true; // assumed for now 
			result.StudyInstanceUid = studyInstanceUid;
			result.SeriesInstanceUid = seriesInstanceUid;
			result.SopInstanceUid = sopInstanceUid;
			result.AccessionNumber = accessionNumber;
            
			// Use the command processor for rollback capabilities.
			using (ServerCommandProcessor processor =
				new ServerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
			{
				try
				{
					string failureMessage;
                    
					if (studyInstanceUid.Length > 64 || seriesInstanceUid.Length > 64 || sopInstanceUid.Length > 64)
					{
						if (studyInstanceUid.Length > 64)
							failureMessage = string.Format("Study Instance UID is > 64 bytes in C-STORE Message: {0}", studyInstanceUid);
						else if (seriesInstanceUid.Length > 64)
							failureMessage = string.Format("Series Instance UID is > 64 bytes in C-STORE Message: {0}", seriesInstanceUid);
						else
							failureMessage = string.Format("SOP Instance UID is > 64 bytes in C-STORE Message: {0}", sopInstanceUid);

						SetError(result, DicomStatuses.AttributeValueOutOfRange, failureMessage);
						Platform.Log(LogLevel.Error, failureMessage);
						throw new ApplicationException(failureMessage);
					}

					StudyStorageLocation studyLocation = StorageHelper.GetStudyStorageLocation(message, _partition);
					if (studyLocation == null)
					{
						StudyStorage storage = StudyStorage.Load(_partition.Key, studyInstanceUid);
                        
						if (storage != null)
						{
							failureMessage = String.Format("Study {0} on partition {1} is in a Nearline state, can't accept new images.  Inserting Restore Request for Study.", studyInstanceUid, _partition.Description);
							Platform.Log(LogLevel.Error, failureMessage);
							if (!InsertRestore(storage.Key))
							{
								Platform.Log(LogLevel.Warn, "Unable to insert Restore Request for Study");
							}

							SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
							throw new ApplicationException(failureMessage);
						}


						failureMessage = String.Format("Unable to process image, no writeable storage location: {0}", sopInstanceUid);
						Platform.Log(LogLevel.Error, failureMessage); 
						SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
						throw new ApplicationException("No writeable storage location.");
					}


					if (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle)
					    && (!studyLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled)))
					{
						failureMessage = String.Format("Study {0} on partition {1} is being processed: {2}, can't accept new images.",
						                               studyLocation.StudyInstanceUid, _partition.Description, studyLocation.QueueStudyStateEnum.Description);
						Platform.Log(LogLevel.Error, failureMessage);
						SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
						throw new ApplicationException(failureMessage);
					}
					else if (studyLocation.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
					{
						ArchiveStudyStorage archiveLocation = StudyStorageLocation.GetArchiveLocation(studyLocation.Key);
						if (archiveLocation != null && archiveLocation.ServerTransferSyntax.Lossless)
						{
							result.DicomStatus = DicomStatuses.StorageStorageOutOfResources;
							failureMessage = String.Format("Study {0} on partition {1} can't accept new images due to lossy compression of the study.  Restoring study.",
							                               studyLocation.StudyInstanceUid, _partition.Description);
							Platform.Log(LogLevel.Error, failureMessage);
							if (!InsertRestore(studyLocation.Key))
							{
								Platform.Log(LogLevel.Warn, "Unable to insert Restore Request for Study");
							}

							SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
							throw new ApplicationException(failureMessage);
						}
					}

					String path = studyLocation.FilesystemPath;
					String dupPath = null;
					bool dupImage = false;
					string extension = null;
					String finalDest = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
					DicomFile file = ConvertToDicomFile(message, finalDest, sourceAE);
                        
					if (File.Exists(finalDest))
					{
						DuplicateSopHandler handler = new DuplicateSopHandler(sourceId, receiverId, processor, _partition, studyLocation);
						result = handler.Handle(file);
					}
					else
					{
						processor.AddCommand(new CreateDirectoryCommand(path));

						path = Path.Combine(path, studyLocation.PartitionFolder);
						processor.AddCommand(new CreateDirectoryCommand(path));

						path = Path.Combine(path, studyLocation.StudyFolder);
						processor.AddCommand(new CreateDirectoryCommand(path));

						path = Path.Combine(path, studyLocation.StudyInstanceUid);
						processor.AddCommand(new CreateDirectoryCommand(path));

						path = Path.Combine(path, seriesInstanceUid);
						processor.AddCommand(new CreateDirectoryCommand(path));

						path += ".dcm";

						processor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

						processor.AddCommand(new UpdateWorkQueueCommand(file, studyLocation, dupImage, extension));

					}
                    
					if (processor.Execute())
					{
						result.DicomStatus = DicomStatuses.Success;
						if (dupPath != null)
						{
							try
							{
								// Don't want to rollback here, just leave the file "as is"
								File.Delete(dupPath);
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Warn, e, "Unexpectedly unable to remove duplicate file: {0}", dupPath);
							}
						}
					}
					else
					{
						failureMessage = String.Format("Failure processing message: {0}. Sending failure status.", processor.FailureReason);
						Platform.Log(LogLevel.Error, failureMessage);
						SetError(result, DicomStatuses.ProcessingFailure, failureMessage);
						// processor already rolled back
						return result;
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
					processor.Rollback();
					result.Sussessful = false;
					if (result.DicomStatus==null /* not yet set */)
						result.DicomStatus = DicomStatuses.ProcessingFailure;
					result.ErrorMessage = e.Message;
					return result;
				}

			}
            
			return result;
		}


		private static void SetError(DicomSopProcessingResult result, DicomStatus status, String message)
		{
			result.Sussessful = false;
			result.DicomStatus = status;
			result.ErrorMessage = message;
		}

		private bool InsertRestore(ServerEntityKey studyStorageKey)
		{
			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IInsertRestoreQueue broker = updateContext.GetBroker<IInsertRestoreQueue>();

				InsertRestoreQueueParameters parms = new InsertRestoreQueueParameters();
				parms.StudyStorageKey = studyStorageKey;

				RestoreQueue queue = broker.FindOne(parms);

				if (queue == null)
					return false;

				updateContext.Commit();
			}

			return true;
		}

		private static DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAE)
		{
			// This routine sets some of the group 0x0002 elements.
			DicomFile file;
			if (message is DicomFile)
			{
				file = message as DicomFile;
			}
			else if (message is DicomMessage)
			{
				file = new DicomFile(message as DicomMessage, filename);
			}
			else
			{
				throw new NotSupportedException(String.Format("Cannot convert {0} to DicomFile", message.GetType()));
			}

			file.SourceApplicationEntityTitle = sourceAE;
			if (message.TransferSyntax.Encapsulated)
				file.TransferSyntax = message.TransferSyntax;
			else
				file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			return file;
		}

		public DicomSopProcessingResult ImportFromNetworkStream(DicomMessage message, ServerAssociationParameters association)
		{
			Platform.CheckForNullReference(message, "message");
			Platform.CheckForNullReference(association, "association");
			String sourceId =
				String.Format("AE:{0}@{1}:{2}", association.CallingAE, association.RemoteEndPoint.Address,
				              association.RemoteEndPoint.Port);

			String receiverId =
				String.Format("AE:{0}@{1}:{2}", association.CalledAE, association.LocalEndPoint.Address,
				              association.LocalEndPoint.Port);

			return Import(message, association.CallingAE, sourceId, receiverId);
		}

		public DicomSopProcessingResult ImportFromFilesystemStream(DicomFile file)
		{
			Platform.CheckForNullReference(file, "file");
			FileInfo fileInfo = new FileInfo(file.Filename);

			String receiverId = ServiceTools.ServerInstanceId;
			return Import(file, "DirectoryImport", fileInfo.Directory.FullName, receiverId);
		}
	}
}