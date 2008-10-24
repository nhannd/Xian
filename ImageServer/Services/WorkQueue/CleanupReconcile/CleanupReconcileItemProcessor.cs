using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupReconcile
{
    /// <summary>
    /// For processing 'CleanupReconcile' WorkQueue items.
    /// </summary>
    class CleanupReconcileItemProcessor : BaseItemProcessor
    {
        private ReconcileStudyWorkQueueData _reconcileQueueData;

        protected override bool CanStart()
        {
            return true;
        }

        protected override void ProcessItem(ClearCanvas.ImageServer.Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");

            if (!CanStart())
            {
                WorkQueueSettings settings = WorkQueueSettings.Instance;
                DateTime newScheduledTime = Platform.Time.Add(TimeSpan.FromMilliseconds(settings.WorkQueueQueryDelay));
                Platform.Log(LogLevel.Info, "Postpone CleanupReconcile entry until {0}. [GUID={1}]", newScheduledTime, item.GetKey());
                PostponeItem(item, newScheduledTime, newScheduledTime.Add(TimeSpan.FromSeconds(settings.WorkQueueExpireDelaySeconds)));
            }
            else
            {
                _reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);
                
                LoadUids(item);


                if (WorkQueueUidList.Count == 0)
                {
                    Complete();
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Reconcile Cleanup started. GUID={0}. StudyStorage={1}", WorkQueueItem.GetKey(), WorkQueueItem.StudyStorageKey);

                    ProcessUidList();

                    BatchComplete();
                }
            }
        }

        private void BatchComplete()
        {
            Platform.Log(LogLevel.Info, "Successfully complete Reconcile Cleanup. GUID={0}. {0} uids processed.", WorkQueueItem.GetKey(), WorkQueueUidList.Count);
            PostProcessing(WorkQueueItem, true, false, false);
        }

        private void Complete()
        {
            Platform.Log(LogLevel.Info, "Reconcile Cleanup is completed. GUID={0}.", WorkQueueItem.GetKey(), WorkQueueUidList.Count);
            PostProcessing(WorkQueueItem, false, true, true);
        }

        private void ProcessUidList()
        {
            Platform.CheckForNullReference(WorkQueueUidList, "WorkQueueUidList");

            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                ProcessUid(uid);
            }
        }

        private void ProcessUid(WorkQueueUid uid)
        {
            Platform.CheckForNullReference(uid, "uid");

            string imagePath = GetUidPath(uid);
            
            using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
            {
                
                // If the file for some reason doesn't exist, we just ignore it
                if (File.Exists(imagePath))
                {
                    Platform.Log(LogLevel.Info, "Deleting {0}", imagePath);
                    FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
                    processor.AddCommand(deleteFile);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "WARNING {0} is missing.", imagePath);
                }
                DeleteQueueUidCommand deleteUid = new DeleteQueueUidCommand(uid);
                processor.AddCommand(deleteUid);
                if (!processor.Execute())
                {
                    throw new Exception(String.Format("Unable to delete image {0}", uid.SopInstanceUid));
                }
            }

        }

        private string GetUidPath(WorkQueueUid sop)
        {
            string imagePath = Path.Combine(_reconcileQueueData.StoragePath, sop.SopInstanceUid + ".dcm");
            Debug.Assert(String.IsNullOrEmpty(imagePath)==false);

            return imagePath;
        }
    }
}
