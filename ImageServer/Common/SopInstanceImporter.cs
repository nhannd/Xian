using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common
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

        public DicomSopProcessingResult Import(DicomMessageBase message, string sourceAE)
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
                    String basePath;
                    bool dupImage = false;
                    string extension = null;

                    processor.AddCommand(new CreateDirectoryCommand(path));

                    path = Path.Combine(path, studyLocation.PartitionFolder);
                    processor.AddCommand(new CreateDirectoryCommand(path));

                    path = Path.Combine(path, studyLocation.StudyFolder);
                    processor.AddCommand(new CreateDirectoryCommand(path));

                    path = Path.Combine(path, studyLocation.StudyInstanceUid);
                    processor.AddCommand(new CreateDirectoryCommand(path));

                    path = Path.Combine(path, seriesInstanceUid);
                    processor.AddCommand(new CreateDirectoryCommand(path));

                    basePath = path = Path.Combine(path, sopInstanceUid);
                    path += ".dcm";

                    if (File.Exists(path))
                    {
                        result.Duplicate = true;
                        
                        if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
                        {
                            result.DicomStatus = DicomStatuses.Success;

                            Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                            return result;
                        }
                        else if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
                        {
                            failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                            Platform.Log(LogLevel.Info, failureMessage);
                            SetError(result, DicomStatuses.DuplicateSOPInstance, failureMessage);
                            throw new ApplicationException("Duplicate SOP Instance received.");
                        }
                        else if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.AcceptLatest))
                        {
                            TransferSyntax studyStorageSyntax =
                                TransferSyntax.GetTransferSyntax(studyLocation.TransferSyntaxUid);
                            if (!studyStorageSyntax.UidString.Equals(message.TransferSyntax.UidString)
                                && (message.TransferSyntax.Encapsulated || studyStorageSyntax.Encapsulated))
                            {
                                failureMessage = String.Format( "Duplicate SOP received, but transfer syntax has changed, db syntax: {0}, message syntax: {1}",
                                        studyLocation.TransferSyntaxUid, message.TransferSyntax.Name);

                                Platform.Log(LogLevel.Info, failureMessage);
                                return result;
                            }
                            else
                            {
                                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, replacing previous file {0}", sopInstanceUid);
                                dupPath = path + "_dup_old";
                                processor.AddCommand(new RenameFileCommand(path, dupPath));
                                dupImage = true;
                            }
                        }
                        else if (_partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
                        {
                            Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, processing {0}", sopInstanceUid);
                            for (int i = 1; i < 999; i++)
                            {
                                extension = String.Format("dup{0}.dcm", i);
                                string newPath = basePath + "." + extension;
                                if (!File.Exists(newPath))
                                {
                                    dupImage = true;
                                    path = newPath;
                                    break;
                                }
                            }
                        }
                    }

                    DicomFile file = ConvertToDicomFile(message, path, sourceAE);
                    processor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

                    processor.AddCommand(new UpdateWorkQueueCommand(file, studyLocation, dupImage, extension));

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
    }
}
