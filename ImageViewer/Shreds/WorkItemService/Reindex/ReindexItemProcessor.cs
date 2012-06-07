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
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex
{
    internal class ReindexItemProcessor : BaseItemProcessor<ReindexRequest, ReindexProgress>
    {
        #region Private Members

        private ReindexUtility _reindexUtility;

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

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
            base.Cancel();

            if (_reindexUtility != null)
                _reindexUtility.Cancel();            
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
            Progress.Complete = false;
            Progress.StudiesToProcess = 0;
            Progress.StudyFoldersToProcess = 0;
            Progress.StudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;

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
            Progress.StudiesFailed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            _reindexUtility.StudyFolderProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                             {
                                                                 Progress.StudyFoldersProcessed++;
                                                                 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                                 Proxy.UpdateProgress();
                                                             };

            _reindexUtility.StudyDeletedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                     {
                                                         Progress.StudiesDeleted++;
                                                         Progress.StudiesProcessed++;
                                                         Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                         Proxy.UpdateProgress();
                                                     };

            _reindexUtility.StudyProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                       {
                                                           Progress.StudiesProcessed++;
                                                           Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                           Proxy.UpdateProgress();
                                                       };

            _reindexUtility.StudiesRestoredEvent += delegate(object sender, ReindexUtility.StudiesEventArgs e)
                                                      {
                                                          foreach (var studyInstanceUid in e.StudyInstanceUids)
                                                          {
                                                              Proxy.Item.StudyInstanceUid = studyInstanceUid;
                                                              Proxy.UpdateProgress();
                                                          }
                                                      };

            _reindexUtility.StudyReindexFailedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                           {
                                                               Progress.StudiesFailed++;
                                                               Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                               if (!string.IsNullOrEmpty(e.Message))
                                                                   Progress.StatusDetails = e.Message;
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
                if (Progress.StudiesFailed > 0)
                    Proxy.Fail(Progress.StatusDetails,WorkItemFailureType.Fatal);                
                else
                    Proxy.Complete();
            }           
        }

        public override bool CanStart(out string reason)
        {                        
            if (ScheduledAheadInsertItems(out reason))
            {
                return false;
            }

            return !InProgressWorkItems(out reason);
        }

        #endregion


        #region Private Methods

        protected bool ScheduledAheadInsertItems(out string reason)
        {
            reason = string.Empty;

            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();             
                var list = broker.GetPriorWorkItems(Proxy.Item.ScheduledTime, null, null);

                if (list == null)
                    return false;
                foreach (var item in list)
                {
                    if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                    {
                        reason = string.Format("Waiting for: {0}",
                                                       item.Request.ActivityDescription);                         
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

    }
}
