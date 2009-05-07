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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    class ProcessDuplicateItemProcessor : BaseItemProcessor
    {
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

        protected override void OnProcessItemBegin(Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);
            
            Platform.CheckForNullReference(Study, "Study doesn't exist");

        }

        protected override void ProcessItem(Model.WorkQueue item)
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
                processor.AddCommand(new OpValidatation(WorkQueueItem, uid));

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
            string baseDir = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
            baseDir = Path.Combine(baseDir, "Reconcile");
            baseDir = Path.Combine(baseDir, WorkQueueItem.GroupID);

            String path = Path.Combine(baseDir, uid.RelativePath);

            return new FileInfo(path);

        }
    }

    internal class OpValidatation : ServerCommand
    {
        private readonly Model.WorkQueue _item;
        private readonly WorkQueueUid _uid;
        private readonly IList<StudyStorageLocation> _locations;

        public OpValidatation(Model.WorkQueue item, WorkQueueUid uid)
            :base("Validation Command", true)
        {
            _item = item;
            _uid = uid;
            _locations = _item.LoadStudyLocations(ExecutionContext.Current.ReadContext);
        }

        private String GetDuplicateSopFile()
        {
            _item.LoadStudy(ExecutionContext.ReadContext);

            string baseDir = Path.Combine(_locations[0].FilesystemPath, _locations[0].PartitionFolder);
            baseDir = Path.Combine(baseDir, "Reconcile");
            baseDir = Path.Combine(baseDir, _item.GroupID);

            String path = Path.Combine(baseDir, _uid.RelativePath);

            return path;

        }

        protected override void OnExecute()
        {
            // duplicate file is deleted
            String dupPath = GetDuplicateSopFile();
            String replacedSopPath = _locations[0].GetSopInstancePath(_uid.SeriesInstanceUid, _uid.SopInstanceUid);

            Platform.CheckTrue(!File.Exists(dupPath), "Duplicate sop was not deleted.");
            Platform.CheckTrue(File.Exists(replacedSopPath), "Replaced sop is not in the study folder.");

            #if DEBUG
            #region MORE EXTENSIVE CHECK
            // can load the file without problem
            DicomFile file = new DicomFile(replacedSopPath);
            file.Load();
            #endregion
            #endif
        }

        protected override void OnUndo()
        {
            // NO-OP
        }
    }
}
