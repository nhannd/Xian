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

            var processor = new ReindexProcessor();

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
            Progress.NumberOfStudiesToProcess = processor.DatabaseStudiesToScan + processor.StudyFoldersToScan;
            Progress.NumberOfStudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;

            Proxy.UpdateProgress();

            processor.StudyFolderProcessedEvent += delegate(object sender, ReindexProcessor.StudyEventArgs e)
                                             {
                                                 Progress.StudyFoldersProcessed++;
                                                 Progress.StudiesProcessed++;
                                                 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                 Proxy.UpdateProgress();
                                             };

            processor.StudyDeletedEvent += delegate
                                               {
                                                   Progress.NumberOfStudiesDeleted++;
                                                   Progress.StudiesProcessed++;
                                                   Proxy.Item.StudyInstanceUid = string.Empty;
                                                   Proxy.UpdateProgress();
                                               };

            processor.StudyProcessedEvent += delegate(object sender, ReindexProcessor.StudyEventArgs e)
                                                 {
                                                     Progress.StudiesProcessed++;
                                                     Proxy.Item.StudyInstanceUid = string.Empty;
                                                     Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                     Proxy.UpdateProgress();
                                                 };
            processor.Process();
            
            Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {            
            reason = string.Empty;
            return !InProgressWorkItems();
        }
    }
}
