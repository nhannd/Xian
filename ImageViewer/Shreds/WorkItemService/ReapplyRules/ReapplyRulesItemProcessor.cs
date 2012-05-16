#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.ReapplyRules
{
    internal class ReapplyRulesItemProcessor : BaseItemProcessor<ReapplyRulesRequest, ReapplyRulesProgress>
    {
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

            var processor = new ReapplyRulesUtility(Request);

            processor.Initialize();


            // Reset progress, in case of retry
            Progress.StudiesToProcess = processor.DatabaseStudiesToScan;
            Progress.StudiesProcessed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            processor.StudyProcessedEvent += delegate
                                                 {
                                                     Progress.StudiesProcessed++;
                                                     Proxy.UpdateProgress();
                                                 };
            processor.Process();

            Progress.Complete = true;
            Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {            
            reason = string.Empty;
            return !ReindexScheduled();
        }
    }
}
