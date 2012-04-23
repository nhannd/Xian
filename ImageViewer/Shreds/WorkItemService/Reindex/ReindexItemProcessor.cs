#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex
{
    public class ReindexItemProcessor : BaseItemProcessor<ReindexRequest, ReindexProgress>
    {
        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
                       
            return initResult;
        }

        public override void Process()
        {
            if (CancelPending)
            {
                Proxy.Cancel();
                return;
            }
            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            Progress.IsCancelable = false;
            Proxy.UpdateProgress();

            var processor = new ReindexUtility();

            processor.Initialize();

            try
            {
                PublishManager<IWorkItemActivityCallback>.Publish("StudiesCleared", WorkItemHelper.FromWorkItem(Proxy.Item));
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItem StudiesCleared status");
            }

            // Reset progress, in case of retry
            Progress.StudiesToProcess = processor.DatabaseStudiesToScan;
            Progress.StudyFoldersToProcess = processor.StudyFoldersToScan;
            Progress.StudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            processor.StudyFolderProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                             {
                                                 Progress.StudyFoldersProcessed++;
                                                 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                 Proxy.UpdateProgress();
                                             };

            processor.StudyDeletedEvent += delegate
                                               {
                                                   Progress.StudiesDeleted++;
                                                   Progress.StudiesProcessed++;
                                                   Proxy.Item.StudyInstanceUid = string.Empty;
                                                   Proxy.UpdateProgress();
                                               };

            processor.StudyProcessedEvent += delegate
                                                 {
                                                     Progress.StudiesProcessed++;
                                                     Proxy.Item.StudyInstanceUid = string.Empty;
                                                     Proxy.UpdateProgress();
                                                 };
            processor.Process();

            Progress.Complete = true;
            Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {            
            reason = string.Empty;
            return !InProgressWorkItems();
        }
    }
}
