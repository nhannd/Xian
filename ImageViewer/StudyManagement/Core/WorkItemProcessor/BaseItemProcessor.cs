#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// <summary>
    /// Abstract base class for processing WorkItems.
    /// </summary>
    /// <typeparam name="TRequest">The request object for the work item.</typeparam>
    /// <typeparam name="TProgress">The progress object for the work item.</typeparam>
    public abstract class BaseItemProcessor<TRequest, TProgress> : IWorkItemProcessor
        where TProgress : WorkItemProgress, new()
        where TRequest : WorkItemRequest
    {
        #region Private Fields

        private const int MAX_DB_RETRY = 5;
        private string _name = "Work Item";
        private IList<WorkItemUid> _uidList;
        private volatile bool _cancelPending;
        private volatile bool _stopPending;
        private readonly object _syncRoot = new object();
        private StorageConfiguration _storageConfig;
        #endregion

        #region Properties

        /// <summary>
        /// The progress object for the WorkItem.  Note that all updates to the progress should
        /// be done through this object, and not through the <see cref="Proxy"/> property.
        /// </summary>
        public TProgress Progress
        {
            get { return Proxy.Progress as TProgress; }
        }

        /// <summary>
        /// The request object for the WorkItem.
        /// </summary>
        public TRequest Request
        {
            get { return Proxy.Request as TRequest; }
        }

        protected IList<WorkItemUid> WorkQueueUidList
        {
            get
            {
                if (_uidList == null)
                {
                    LoadUids();
                }
                return _uidList;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public WorkItemStatusProxy Proxy { get; set; }

        public StudyLocation Location { get; set; }

        protected bool CancelPending
        {
            get { return _cancelPending; }
        }

        protected bool StopPending
        {
            get { return _stopPending; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by the base to initialize the processor.
        /// </summary>
        public virtual bool Initialize(WorkItemStatusProxy proxy)
        {
            Proxy = proxy;
            if (proxy.Progress == null)
                proxy.Progress = new TProgress();
            else if (!(proxy.Progress is TProgress))
                proxy.Progress = new TProgress();

            if (Request == null)
                throw new ApplicationException(SR.InternalError);

            if (!string.IsNullOrEmpty(proxy.Item.StudyInstanceUid))
                Location = new StudyLocation(proxy.Item.StudyInstanceUid);

            return true;
        }

        public virtual void Cancel()
        {
            Proxy.Canceling();

            lock (_syncRoot)
                _cancelPending = true;
        }

        public virtual void Stop()
        {
            lock (_syncRoot)
                _stopPending = true;
        }

        public virtual bool CanStart(out string reason)
        {
         
            if (Proxy.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
            {
                var relatedList = FindRelatedWorkItems();
                if (relatedList != null)
                {
                    foreach (var relatedWorkItem in relatedList)
                    {
                        if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
                         || relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                         relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }
                reason = string.Empty;
                return !ReindexScheduled();
            }

            if (Proxy.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
            {
                var inProgressList = FindRelatedInProgressWorkItems();
                if (inProgressList != null)
                {
                    foreach (var relatedWorkItem in inProgressList)
                    {
                        // This check is done because it is possible that 2 entries for the
                        // Same study are entered into the queue, and they both don't run because
                        // of the other entry.  This will allow the one scheduled first to run.
                        if (relatedWorkItem.ScheduledTime < Proxy.Item.ScheduledTime)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                   relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }

                var relatedList = FindRelatedWorkItems();
                if (relatedList != null)
                {
                    foreach (var relatedWorkItem in relatedList)
                    {
                        if (relatedWorkItem.Request.ConcurrencyType != WorkItemConcurrency.NonStudy)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                   relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }
                reason = string.Empty;
                return !ReindexScheduled();
            }

            if (Proxy.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
            {
                var relatedList = FindRelatedWorkItems();
                if (relatedList != null)
                {
                    foreach (var relatedWorkItem in relatedList)
                    {
                        if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
                            || relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                   relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }

                reason = string.Empty;
                return !ReindexScheduled();
            }

            // WorkItemConcurrency.NonStudy entries can just run
            reason = string.Empty;
            return true;
        }

        public abstract void Process();

        public virtual void Delete()
        {
            Proxy.Delete();
        }

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Ensure minimum amount of space available in the local data store
        /// </summary>
        /// <exception cref="NotEnoughStorageException"/>
        protected void EnsureMaxUsedSpaceNotExceeded()
        {
            if (LocalStorageMonitor.IsMaxUsedSpaceExceeded)
            {
                Platform.Log(LogLevel.Error, "Not enough storage space. Max Used={0}%, Total Used={1} %",
                                    LocalStorageMonitor.MaximumUsedSpacePercent,
                                    LocalStorageMonitor.UsedSpacePercent);
                throw new NotEnoughStorageException();
            }
        }


        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        protected void LoadUids()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemUidBroker();

                _uidList = broker.GetWorkItemUidsForWorkItem(Proxy.Item.Oid);
            }
        }

        /// <summary>
        /// Routine for failing a work queue uid record.
        /// </summary>
        /// <param name="uid">The WorkItemUid entry to fail.</param>
        /// <param name="retry">A boolean value indicating whether a retry will be attempted later.</param>
        protected WorkItemUid FailWorkItemUid(WorkItemUid uid, bool retry)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemUidBroker();
                var sop = broker.GetWorkItemUid(uid.Oid);

                if (!sop.FailureCount.HasValue)
                    sop.FailureCount = 1;
                else
                    sop.FailureCount = (byte) (sop.FailureCount.Value + 1);

                if (sop.FailureCount > WorkItemServiceSettings.Default.RetryCount)
                    sop.Failed = true;

                context.Commit();
                return sop;
            }
        }

        /// <summary>
        /// Delete an entry in the <see cref="WorkItemUid"/> table.
        /// </summary>
        /// <param name="uid">The <see cref="WorkItemUid"/> entry to delete.</param>
        protected virtual WorkItemUid CompleteWorkItemUid(WorkItemUid uid)
        {
            // Must retry in case of db error.
            // Failure to do so may lead to orphaned WorkQueueUid and FileNotFoundException 
            // when the work queue is reset.
            int retryCount = 0;
            while (true)
            {
                try
                {
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetWorkItemUidBroker();
                        var sop = broker.GetWorkItemUid(uid.Oid);
                        sop.Complete = true;
                        context.Commit();
                        return sop;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex,
                                         "Error occurred when calling DeleteWorkQueueUid. Max db retry count has been reached.");
                            Proxy.Fail(
                                String.Format(
                                    "Error occurred when deleting WorkItemUid. Max db retry count has been reached."),
                                WorkItemFailureType.Fatal);
                            return uid;
                        }

                        Platform.Log(LogLevel.Error, ex,
                                     "Error occurred when calling DeleteWorkQueueUid(). Retry later. OID={0}", uid.Oid);
                        SleepForRetry();


                        // Service is stoping
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Termination Requested. DeleteWorkQueueUid() is now terminated.");
                            break;
                        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }
            return uid;
        }


        /// <summary>
        /// Put the workqueue thread to sleep for 2-3 minutes.
        /// </summary>
        /// <remarks>
        /// This method does not return until 2-3 minutes later or if the service is stoppping.
        /// </remarks>
        private void SleepForRetry()
        {
            var start = Platform.Time;
            var rand = new Random();
            while (!CancelPending)
            {
                // sleep, wake up every 1-3 sec and check if the service is stopping
                Thread.Sleep(rand.Next(1000, 3000));
                if (CancelPending)
                {
                    break;
                }

                // Sleep for 2-3 minutes
                DateTime now = Platform.Time;
                if (now - start > TimeSpan.FromMinutes(rand.Next(2, 3)))
                    break;
            }
        }


        /// <summary>
        /// Returns a list of related <see cref="WorkItem"/> with specified types and status (both are optional).
        /// and related to the given <see cref="WorkItem"/> 
        /// </summary>
        /// <returns></returns>
        protected IList<WorkItem> FindRelatedWorkItems( )
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                var prioritiesToBlock = new List<WorkItemPriorityEnum>();

                
                if (Request.Priority == WorkItemPriorityEnum.Stat)
                {
                    prioritiesToBlock.Add(WorkItemPriorityEnum.High);
                    prioritiesToBlock.Add(WorkItemPriorityEnum.Normal);
                }
                if (Request.Priority == WorkItemPriorityEnum.High)
                {
                    prioritiesToBlock.Add(WorkItemPriorityEnum.Normal);
                }
                var list = broker.GetPriorWorkItems(Proxy.Item.ScheduledTime,prioritiesToBlock, Proxy.Item.StudyInstanceUid);

                if (list == null)
                    return null;

                var newList = new List<WorkItem>();
                newList.AddRange(list);
                return newList;
            }
        }

        /// <summary>
        /// Returns a list of related <see cref="WorkItem"/> with specified types and status (both are optional).
        /// and related to the given <see cref="WorkItem"/> 
        /// </summary>
        /// <returns></returns>
        protected IList<WorkItem> FindRelatedInProgressWorkItems()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                var list = broker.GetWorkItems(null,WorkItemStatusEnum.InProgress, Proxy.Item.StudyInstanceUid);

                if (list == null)
                    return null;

                var newList = new List<WorkItem>();
                foreach (var item in list)
                {
                    if (item.Oid != Proxy.Item.Oid)
                        newList.Add(item);
                }
                return newList;
            }
        }

        /// <summary>
        /// Returns a list of related <see cref="WorkItem"/> with specified types and status (both are optional).
        /// and related to the given <see cref="WorkItem"/> 
        /// </summary>
        /// <returns></returns>
        protected bool ReindexScheduled()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                var list = broker.GetWorkItems(ReindexRequest.WorkItemTypeString, null, null);

                if (list == null)
                    return false;

                var newList = CollectionUtils.Select(list,
                                                     item => item.Status != WorkItemStatusEnum.Complete
                                                             && item.Status != WorkItemStatusEnum.Deleted
                                                             && item.Status != WorkItemStatusEnum.Canceled
                                                             && item.Status != WorkItemStatusEnum.Failed);

                if (Proxy.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                {
                    newList = CollectionUtils.Select(newList,
                                                     item => item.ScheduledTime < Proxy.Item.ScheduledTime && item.Status != WorkItemStatusEnum.InProgress);
                }

                return newList.Count > 0;
            }
        }

        /// <summary>
        /// Returns a list of related <see cref="WorkItem"/> with specified types and status (both are optional).
        /// and related to the given <see cref="WorkItem"/> 
        /// </summary>
        /// <returns></returns>
        protected bool InProgressWorkItems(out string reason)
        {
            reason = string.Empty;
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

                if (list == null)
                    return false;

                // remove the current item 
                var newList = CollectionUtils.Reject(list, item => item.Oid.Equals(Proxy.Item.Oid));

                foreach (var item in newList)
                {
                    reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                    break;
                }

                return newList.Count > 0;
            }
        }

        protected static DicomServerConfiguration GetServerConfiguration()
        {
            DicomServerConfiguration configuration = null;
            var request = new GetDicomServerConfigurationRequest();
            Platform.GetService<IDicomServerConfiguration>(s =>
                                                           configuration = s.GetConfiguration(request).Configuration);

            return configuration;
        }

        protected Study LoadRelatedStudy()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();           
                    
                if (!string.IsNullOrEmpty(Proxy.Item.StudyInstanceUid))
                    return broker.GetStudy(Proxy.Item.StudyInstanceUid);

                return null;
            }
        }

        #endregion
    }

    public class NotEnoughStorageException : Exception
    {
        public NotEnoughStorageException():base("Additional storage space is required")
        {
        }
    }
}
