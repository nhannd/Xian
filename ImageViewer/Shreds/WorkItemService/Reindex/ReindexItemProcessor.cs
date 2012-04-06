#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;

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

            // Reset progress, in case of retry
            Progress.NumberOfStudiesToProcess = processor.DatabaseStudiesToScan + processor.StudyFoldersToScan;
            Progress.NumberOfStudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;

            Proxy.UpdateProgress();

            processor.StudyFolderProcessedEvent += delegate
                                             {
                                                 Progress.StudyFoldersProcessed++;
                                                 Proxy.UpdateProgress();
                                             };

            processor.StudyDeletedEvent += delegate
                                               {
                                                   Progress.NumberOfStudiesDeleted++;
                                                   Proxy.UpdateProgress();
                                               };

            processor.StudyProcessedEvent += delegate
                                                 {
                                                     Progress.StudiesProcessed++;
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
