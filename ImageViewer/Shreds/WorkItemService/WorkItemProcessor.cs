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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    /// <summary>
    /// Engine for acquiring WorkItems and finding plugins to process them.
    /// </summary>
    sealed public class WorkItemProcessor : QueueProcessor
    {
        #region Members

        private readonly Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory> _extensions = new Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory>();
		private readonly WorkItemThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly string _name;

        #endregion

		#region Constructor
        private WorkItemProcessor(int numberStatThreads, int numberNormalThreads, string name)
        {
            _name = name;
            _threadStop = new ManualResetEvent(false);

			_threadPool = new WorkItemThreadPool(numberStatThreads,numberNormalThreads)
			                  {
			                      ThreadPoolName = name + " Pool"
			                  };

			var ep = new WorkItemProcessorFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Fatal, "No WorkItemFactory Extensions found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    var factory = obj as IWorkItemProcessorFactory;
                    if (factory != null)
                    {
                        WorkItemTypeEnum type = factory.GetWorkQueueType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
	    }
		#endregion

        #region Public Properties

        public static WorkItemProcessor Instance { get; private set; }

        /// <summary>
        /// The Thread Name.
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }

        #endregion

        #region Public Static Method
        public static void CreateProcessor(int numberStatThreads, int numberNormalThreads, string name)
        {
            if (Instance != null) throw new ApplicationException("Processor already created!");

            Instance = new WorkItemProcessor(numberStatThreads, numberNormalThreads, name);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Signal the processor to stop sleeping and check for Shutdown or new WorkItem
        /// </summary>
        public void SignalThread()
        {
            _threadStop.Set();
        }

        /// <summary>
        /// Stop the WorkItem processor
		/// </summary>
		public override void RequestStop()
		{
            base.RequestStop();

			_threadStop.Set();

			if (_threadPool.Active)
				_threadPool.Stop();
		}

		/// <summary>
		/// The processing thread.
		/// </summary>
		/// <remarks>
        /// This method queries the database for WorkItem entries to work on, and then uses
		/// a thread pool to process the entries.
		/// </remarks>
        protected override void RunCore()
        {
            // Reset any in progress WorkItems if we crashed while processing.
		    ResetInProgressWorkItems();

		    if (!_threadPool.Active)
		        _threadPool.Start();

		    Platform.Log(LogLevel.Info, "WorkItem Processor running...");

            while (true)
            {
                if (StopRequested)
                    return;

                List<WorkItem> list = null;
                if (_threadPool.StatThreadsAvailable > 0)
                {
                    list = GetWorkItems(_threadPool.StatThreadsAvailable, true);
                }
                
                if ((list == null || list.Count == 0) && _threadPool.NormalThreadsAvailable > 0)
                {
                    list = GetWorkItems(_threadPool.NormalThreadsAvailable, false);
                }

                if ((list == null || list.Count == 0) && _threadPool.NormalThreadsAvailable > 0)
                {
                    list = GetWorkItemsToDelete(_threadPool.NormalThreadsAvailable);
                }
                
                if ( list == null)
                {
                    /* No threads available, wait for one to complete. */
                    if (_threadStop.WaitOne(5000, false))
                        _threadStop.Reset();
                    continue;
                }
                if  (list.Count == 0)
                {
                    // No result found 
                    if (_threadStop.WaitOne(5000, false))
                        _threadStop.Reset(); 
                    continue;
                }

                try
                {
                    foreach (var item in list)
                    {
                        if (!_extensions.ContainsKey(item.Type))
                        {
                            Platform.Log(LogLevel.Error,
                                         "No extensions loaded for WorkItem item type: {0}.  Failing item.",
                                         item.Type);

                            //Just fail the WorkQueue item, not much else we can do
                            var proxy = new WorkItemStatusProxy(item);
                            proxy.Fail("No plugin to handle WorkItem type: " + item.Type, WorkItemFailureType.Fatal);
                            continue;
                        }

                        try
                        {
                            IWorkItemProcessorFactory factory = _extensions[item.Type];
                            IWorkItemProcessor processor = factory.GetItemProcessor();

                            // Enqueue the actual processing of the item to the thread pool.  
                            _threadPool.Enqueue(processor, item, ExecuteProcessor);
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkItem processor.");
                            var proxy = new WorkItemStatusProxy(item);
                            proxy.Fail("No plugin to handle WorkItem type: " + item.Type, WorkItemFailureType.Fatal);                   
                        }
                    }
                }
                catch (Exception e)
                {
                    // Wait for only 3 seconds
                    Platform.Log(LogLevel.Error, e, "Exception occured when processing WorkItem item.");
                    _threadStop.WaitOne(3000, false);
                }
            }
		}

        /// <summary>
        /// Cancel a current running WorkItem
        /// </summary>
        /// <param name="workItemOid"></param>
        public void Cancel(long workItemOid)
        {
            _threadPool.Cancel(workItemOid);
        }
        #endregion

		#region Private Methods
		/// <summary>
		/// The actual delegate 
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="queueItem"></param>
        private void ExecuteProcessor(IWorkItemProcessor processor, WorkItem queueItem)
		{
		    var proxy = new WorkItemStatusProxy(queueItem);

			try
			{
			    Platform.Log(proxy.LogLevel, "Starting processing of {0} WorkItem for OID {1}", queueItem.Type, queueItem.Oid);

                if (proxy.Item.Status == WorkItemStatusEnum.Deleted || proxy.Item.Status == WorkItemStatusEnum.DeleteInProgress)
                {
                    if (!processor.Initialize(proxy))
                    {
                        Platform.Log(LogLevel.Error, "Unable to intialize WorkItem processor for: {0}.  Directly deleting.", proxy.Request.ActivityType);
                        proxy.Delete();
                        return;
                    }

                    // Delete the entry
                    processor.Delete();
                    return;
                }

			    string failureDescription;
                if (!processor.Initialize(proxy))
                {
                	proxy.Postpone();
                	return;
                }

                if (processor.CanStart(out failureDescription))
                {
                    processor.Process();
                }
                else
                {
                    proxy.Progress.StatusDetails = failureDescription;
                    proxy.Postpone();
                }
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
				             "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (Oid: {1})",
				             queueItem.Type,
				             queueItem.Oid);
				String error = e.InnerException != null ? e.InnerException.Message : e.Message;

				proxy.Fail(error, WorkItemFailureType.NonFatal);
			}
			finally
			{
                // Signal the parent thread, so it can query again
                _threadStop.Set();

                // Cleanup the processor
                processor.Dispose();
                Platform.Log(proxy.LogLevel, "Done processing of {0} WorkItem for OID {1} and status {2}", proxy.Item.Type, proxy.Item.Oid, proxy.Item.Status);
			}
		}

        /// <summary>
        /// Method for getting next <see cref="StudyManagement.Storage.WorkItem"/> entry.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="stat">Search for stat items.</param>
		/// <remarks>
		/// </remarks>
		/// <returns>
		/// A <see cref="StudyManagement.Storage.WorkItem"/> entry if found, or else null;
		/// </returns>
        private List<WorkItem> GetWorkItems(int count, bool stat)
        {
            try
            {
                using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                {
                    var workItemBroker = context.GetWorkItemBroker();

                    List<WorkItem> workItems;
                    if (stat)
                        workItems = workItemBroker.GetStatPendingWorkItems(count);
                    else
                        workItems = workItemBroker.GetPendingWorkItems(count);

                    foreach (var item in workItems)
                    {
                        item.Status = WorkItemStatusEnum.InProgress;
                    }

                    context.Commit();
                    return workItems;
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected error querying for {0} {1} priority WorkItems", count, stat ? "Stat" : "Normal");
                return null;
            }
        }


        /// <summary>
        /// Method for getting next <see cref="StudyManagement.Storage.WorkItem"/> entry.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        /// A <see cref="StudyManagement.Storage.WorkItem"/> entry if found, or else null;
        /// </returns>
        private List<WorkItem> GetWorkItemsToDelete(int count)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                var workItems = workItemBroker.GetWorkItemsToDelete(count);

                foreach (var item in workItems)
                {
                    item.Status = WorkItemStatusEnum.DeleteInProgress;
                }

                context.Commit();
                if (workItems.Count > 0)
                    return workItems;
            }

            // Get entries already marked as deleted by the GUI.
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                var workItems = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.Deleted, null);

                if (workItems.Count > count)
                    workItems = workItems.GetRange(0, count);

                foreach (var item in workItems)
                {
                    item.Status = WorkItemStatusEnum.DeleteInProgress;
                }

                context.Commit();

                return workItems;
            }
        }

        /// <summary>
        /// Called on startup to reset InProgress WorkItems back to Pending.
        /// </summary>
        private void ResetInProgressWorkItems()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.DeleteInProgress, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Deleted;
                }

                context.Commit();
            }
        }

    	#endregion
    }
}
