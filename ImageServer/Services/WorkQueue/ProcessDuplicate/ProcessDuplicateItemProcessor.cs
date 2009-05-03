using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    class ProcessDuplicateItemProcessor : BaseItemProcessor
    {
        private const String DUPLICATE_SOP_EXTENSION = "dup";
        private WorkQueueProcessDuplicateSop _duplicateQueueEntry;

        protected String DuplicateFolder
        {
            get
            {
                if (_duplicateQueueEntry == null)
                {
                    _duplicateQueueEntry = new WorkQueueProcessDuplicateSop(WorkQueueItem);

                }

                return _duplicateQueueEntry.GetDuplicateSopFolder();
            }
        }

        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
                if (!LockStudyState(WorkQueueItem, QueueStudyStateEnum.ProcessingScheduled))
                {
                    Platform.Log(LogLevel.Debug,
                                 "ProcessDuplicate cannot start at this point. Study is being locked by another processor. Current state={0}",
                                 StorageLocation.QueueStudyStateEnum);
                    PostponeItem(WorkQueueItem);
                    return false;
                }
            }

            return true; // it is being locked by me
        }

        protected override void OnProcessItemBegin(ClearCanvas.ImageServer.Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);
            
            Platform.CheckForNullReference(Study, "Study doesn't exist");

        }

        protected override void ProcessItem(ClearCanvas.ImageServer.Model.WorkQueue item)
        {
            Platform.CheckMemberIsSet(StorageLocation, "StorageLocation");

            if (WorkQueueUidList.Count == 0)
            {
                // we are done. Just need to cleanup the duplicate folder
                DirectoryInfo duplicateFolder = new DirectoryInfo(DuplicateFolder);
                if (duplicateFolder.Exists)
                {
                    DirectoryUtility.DeleteIfEmpty(duplicateFolder.FullName);
                    DirectoryUtility.DeleteIfEmpty(duplicateFolder.Parent.FullName);
                }
                PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            }
            else
            {
                _duplicateQueueEntry = new WorkQueueProcessDuplicateSop(item);

                Platform.CheckTrue(Directory.Exists(DuplicateFolder), String.Format("Duplicate Folder {0} doesn't exist.", DuplicateFolder));

                foreach (WorkQueueUid uid in WorkQueueUidList)
                {
                    ProcessUid(uid);
                }

                PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                
            }

        }

        private void ProcessUid(WorkQueueUid uid)
        {
            
            switch(_duplicateQueueEntry.QueueData.Action)
            {
                case ProcessDuplicateAction.Overwrite:
                    OverwriteExistingInstance(uid);
                    break;

                case ProcessDuplicateAction.Delete:
                    DeleteDuplicate(uid);
                    break;

                default:
                    throw new NotSupportedException(
                        String.Format("Not supported action: {0}", _duplicateQueueEntry.QueueData.Action));
            }
        }

        private void DeleteDuplicate(WorkQueueUid uid)
        {
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Delete Received Duplicate"))
            {
                FileInfo duplicateFile = GetDuplicateSopFile(uid);

                processor.AddCommand(new DeleteFileCommand(duplicateFile.FullName));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Discard duplicate SOP {0} in {1}", uid.SopInstanceUid, duplicateFile.FullName);
                }
            }
        }

        private void OverwriteExistingInstance(WorkQueueUid uid)
        {
            String finalDestination = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
            bool overwrite = File.Exists(finalDestination);
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Move Duplicate Into Study Folder"))
            {
                FileInfo duplicateFile = GetDuplicateSopFile(uid);
                Platform.CheckTrue(duplicateFile.Exists, String.Format("Duplicate SOP doesn't exist at {0}", uid.SopInstanceUid));

                DicomFile file = new DicomFile(duplicateFile.FullName);
                file.Load();

                if (overwrite)
                    processor.AddCommand(new DeleteFileCommand(finalDestination));
                
                processor.AddCommand(new SaveDicomFileCommand(finalDestination, file, true, true));
                processor.AddCommand(new DeleteFileCommand(duplicateFile.FullName));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
                else
                {
                    if (overwrite)
                        Platform.Log(LogLevel.Info, "Replaced existing SOP {0} with duplicate {1}", uid.SopInstanceUid, duplicateFile.FullName);
                    else
                        Platform.Log(LogLevel.Info, "Added duplicate SOP {0} from {1}", uid.SopInstanceUid, duplicateFile.FullName);
                }
            }
            
        }

        private FileInfo GetDuplicateSopFile(WorkQueueUid uid)
        {
            String path = Path.Combine(DuplicateFolder, uid.SopInstanceUid);
            path += "." + DUPLICATE_SOP_EXTENSION;

            return new FileInfo(path);

        }
    }
}
