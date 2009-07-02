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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
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

    public class SopInstanceImporterContext
    {
        public String ContextID; 
        public string SourceAE; 
        public DicomMessageBase Message;

        public string GetDuplicateStorageFolder()
        {
            return ContextID;
        }

        public string SeriesInstanceUid
        {
            get
            {
                return Message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            }
        }

        public string SopInstanceUid
        {
            get
            {
                return Message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            }
        }
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

        public DicomSopProcessingResult Import(SopInstanceImporterContext context)
		{
            Platform.CheckForNullReference(context, "context");
            string failureMessage;
            DicomMessageBase message;
            message = context.Message;
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
            using (ServerCommandProcessor commandProcessor =
                new ServerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
            {
                try
                {
                    
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

                    StudyStorageLocation studyLocation = StorageHelper.GetWritableStudyStorageLocation(message, _partition);
                    if (studyLocation == null)
                    {
                        StudyStorage storage = StudyStorage.Load(_partition.Key, studyInstanceUid);
                        
                        if (storage != null)
                        {
                            if (storage.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
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
                            else
                            {
                                // study is not nearline but the location is not writable
                                failureMessage = String.Format("Unable to process image, study storage location is not writeable: {0}.", sopInstanceUid);
                                Platform.Log(LogLevel.Error, failureMessage);
                                SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
                                throw new ApplicationException("No writeable storage location.");
                            }
                        }
                        else
                        {
                            failureMessage = String.Format("Unable to process image, no writeable storage location: {0}", sopInstanceUid);
                            Platform.Log(LogLevel.Error, failureMessage);
                            SetError(result, DicomStatuses.StorageStorageOutOfResources, failureMessage);
                            throw new ApplicationException("No writeable storage location.");
                        }
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
                    DicomFile file = ConvertToDicomFile(message, finalDest, context.SourceAE);

                    if (HasUnprocessedCopy(context, studyLocation))
                    {
                        failureMessage = string.Format("Another copy of the SOP Instance was received but has not been processed: {0}", sopInstanceUid);
                        SetError(result, DicomStatuses.DuplicateSOPInstance, failureMessage);
                        Platform.Log(LogLevel.Info, failureMessage);
                    }
                    else
                    {
                        if (File.Exists(finalDest))
                        {
                            Study study = studyLocation.Study ??
                                          studyLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);
                            if (study != null)
                                Platform.Log(LogLevel.Info, "Received duplicate SOP {0} (A#:{1} StudyUid:{2}  Patient: {3}  ID:{4})", 
                                            sopInstanceUid,
                                            study.AccessionNumber, study.StudyInstanceUid,            
                                            study.PatientsName, study.PatientId);
                            else
                                Platform.Log(LogLevel.Info,
                                             "Received duplicate SOP {0} (StudyUid:{1}). Existing files haven't been processed.",
                                             sopInstanceUid, studyLocation.StudyInstanceUid);

                            DuplicateSopProcessor dupProcessor =
                                new DuplicateSopProcessor(commandProcessor, _partition, studyLocation);
                            result = dupProcessor.Process(context.SourceAE, context.ContextID, file);
                        }
                        else
                        {
                            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

                            path = Path.Combine(path, studyLocation.PartitionFolder);
                            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

                            path = Path.Combine(path, studyLocation.StudyFolder);
                            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

                            path = Path.Combine(path, studyLocation.StudyInstanceUid);
                            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

                            path = Path.Combine(path, seriesInstanceUid);
                            commandProcessor.AddCommand(new CreateDirectoryCommand(path));

                            path = Path.Combine(path, sopInstanceUid);
                            path += ".dcm";

                            commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true, true));

                            commandProcessor.AddCommand(
                                new UpdateWorkQueueCommand(file, studyLocation, dupImage, extension));

                            #region SPECIAL CODE FOR TESTING
                            if (ClearCanvas.ImageServer.Common.Diagnostics.Settings.SimulateFileCorruption)
                            {
                                commandProcessor.AddCommand(new CorruptDicomFileCommand(path));
                            }
                            #endregion
                        }

                        if (commandProcessor.Execute())
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
                                    Platform.Log(LogLevel.Warn, e, "Unexpectedly unable to remove duplicate file: {0}",
                                                 dupPath);
                                }
                            }
                        }
                        else
                        {
                            failureMessage =
                                String.Format("Failure processing message: {0}. Sending failure status.",
                                              commandProcessor.FailureReason);
                            Platform.Log(LogLevel.Error, failureMessage);
                            SetError(result, DicomStatuses.ProcessingFailure, failureMessage);
                            // processor already rolled back
                            return result;
                        }
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
                    commandProcessor.Rollback();
                    result.Sussessful = false;
                    if (result.DicomStatus == null /* not yet set */)
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

		static private DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAE)
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

        static private bool HasUnprocessedCopy(SopInstanceImporterContext context, StudyStorageLocation storage)
        {
            IList<WorkQueue> workQueues = FindWorkQueueEntries(storage, null);
            foreach (WorkQueue queue in workQueues)
            {
                if (queue.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReconcileStudy))
                {
                    ReconcileStudyWorkQueue entry = new ReconcileStudyWorkQueue(queue);
                    ReconcileStudyWorkQueueData queueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(entry.Data);
                    if (queueData != null)
                    {
                        String path = entry.GetSopPath(context.SeriesInstanceUid, context.SopInstanceUid);
                        if (File.Exists(path))
                            return true;
                    }

                }

            }

            IList<StudyIntegrityQueue> list = FindSIQEntries(storage, null);
            if (list == null || list.Count == 0) return false;

            foreach (StudyIntegrityQueue entry in list)
            {
                if (entry.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.Duplicate))
                {
                    DuplicateSopReceivedQueue duplicateEntry = new DuplicateSopReceivedQueue(entry);
                    String path = duplicateEntry.GetSopPath(context.SeriesInstanceUid, context.SopInstanceUid);
                    if (File.Exists(path))
                        return true;
                }
                else if (entry.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.InconsistentData))
                {
                    InconsistentDataSIQEntry duplicateEntry = new InconsistentDataSIQEntry(entry);
                    String path = duplicateEntry.GetSopPath(context.SeriesInstanceUid, context.SopInstanceUid);
                    if (File.Exists(path))
                        return true;
                }
            }

            return false;
        }

        static public IList<StudyIntegrityQueue> FindSIQEntries(StudyStorageLocation study, Predicate<StudyIntegrityQueue> filter)
        {
            using (ExecutionContext scope = new ExecutionContext())
            {
                IStudyIntegrityQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(study.GetKey());
                criteria.InsertTime.SortDesc(0);
                IList<StudyIntegrityQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }

        static public IList<WorkQueue> FindWorkQueueEntries(StudyStorageLocation study, Predicate<WorkQueue> filter)
        {
            using (ExecutionContext scope = new ExecutionContext())
            {
                IWorkQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(study.GetKey());
                criteria.InsertTime.SortDesc(0);
                IList<WorkQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }
    
	}

    internal class CorruptDicomFileCommand : ServerCommand
    {
        private string _path;

        public CorruptDicomFileCommand(string path):base("Corrupt file", false)
        {
            _path = path;
        }

        protected override void OnExecute()
        {
            Random rand = new Random();

            if (ClearCanvas.ImageServer.Common.Diagnostics.Settings.SimulateFileCorruption)
            {
                ClearCanvas.ImageServer.Common.Diagnostics.RandomError.Generate(
                    rand.Next() % 2 == 0,
                    String.Format("Corrupting the file {0}", _path),
                    delegate()
                    {
                        FileInfo f = new FileInfo(_path);
                        long size = rand.Next(0, (int) f.Length/2);
                        if (size <= 0)
                        {
                            FileStream s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
                            s.Flush();
                            s.Close();
                        }
                        else
                        {
                            FileStream s = FileStreamOpener.OpenForRead(_path, FileMode.Open);
                            byte[] buffer = new byte[size];
                            int bytesRead = s.Read(buffer, 0, buffer.Length);
                            s.Close();

                            s = FileStreamOpener.OpenForSoleUpdate(_path, FileMode.Truncate);
                            s.Write(buffer, 0, bytesRead);
                            s.Flush();
                            s.Close();
                        }
                        
                    }
                );
            }
        }

        protected override void OnUndo()
        {
            
        }
    }
}