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
using ClearCanvas.ImageViewer.StudyManagement.Core;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex
{
    internal class ReindexItemProcessor : BaseItemProcessor<ReindexRequest, ReindexProgress>
    {
        private ReindexUtility _reindexUtility;

        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
                       
            return initResult;
        }

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the ReindexUtility's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            if (_reindexUtility != null)
                _reindexUtility.Cancel();
            
            base.Cancel();
        }

        /// <summary>
        /// Override of Stop() routine.
        /// </summary>
        /// <remarks>
        /// The Stop must be override to call the ReindexUtility's Cancel routine.
        /// </remarks>
        public override void Stop()
        {
            if (_reindexUtility != null)
                _reindexUtility.Cancel();

            base.Stop();
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

            Progress.IsCancelable = true;
            Proxy.UpdateProgress();

            _reindexUtility = new ReindexUtility();

            _reindexUtility.Initialize();

            try
            {
                WorkItemPublishSubscribeHelper.PublishStudiesCleared();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItem StudiesCleared status");
            }

            // Reset progress, in case of retry
            Progress.StudiesToProcess = _reindexUtility.DatabaseStudiesToScan;
            Progress.StudyFoldersToProcess = _reindexUtility.StudyFoldersToScan;
            Progress.StudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            _reindexUtility.StudyFolderProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                             {
                                                 Progress.StudyFoldersProcessed++;
                                                 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                 Proxy.UpdateProgress();
                                             };

            _reindexUtility.StudyDeletedEvent += delegate
                                               {
                                                   Progress.StudiesDeleted++;
                                                   Progress.StudiesProcessed++;
                                                   Proxy.Item.StudyInstanceUid = string.Empty;
                                                   Proxy.UpdateProgress();
                                               };

            _reindexUtility.StudyProcessedEvent += delegate
                                                 {
                                                     Progress.StudiesProcessed++;
                                                     Proxy.Item.StudyInstanceUid = string.Empty;
                                                     Proxy.UpdateProgress();
                                                 };
            _reindexUtility.Process();

            if (StopPending)
            {
                Progress.Complete = true;
                Proxy.Postpone();
            }
            else if (CancelPending)
            {
                Progress.Complete = true;
                Proxy.Cancel();
            }
            else
            {
                Progress.Complete = true;
                Proxy.Complete();
            }           
        }

        public override bool CanStart(out string reason)
        {            
            reason = string.Empty;
            return !InProgressWorkItems();
        }
    }
}
