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
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    sealed public class WorkItemProcessor : QueueProcessor
    {
        #region Members

        private readonly Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory> _extensions = new Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory>();
		private readonly ProcessorThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;

        #endregion

		#region Constructor
        private WorkItemProcessor(int numberStatThreads, int numberNormalThreads, string name)
        {
            _threadStop = new ManualResetEvent(false);

			_threadPool = new ProcessorThreadPool(numberStatThreads,numberNormalThreads)
			                  {
			                      ThreadPoolName = name + " Pool"
			                  };

			var ep = new WorkItemFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No WorkItemFactory Extension found.");
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
		/// Stop the WorkQueue processor
		/// </summary>
		public override void RequestStop()
		{
            base.RequestStop();

            // Set both events, just in case.
			_threadStop.Set();

			if (_threadPool.Active)
				_threadPool.Stop();
		}

		/// <summary>
		/// The processing thread.
		/// </summary>
		/// <remarks>
		/// This method queries the database for WorkQueue entries to work on, and then uses
		/// a thread pool to process the entries.
		/// </remarks>
        protected override void RunCore()
        {
            // Reset any in progress WorkItems if we crashed while processing.
		    ResetInProgressWorkItems();

		    if (!_threadPool.Active)
		        _threadPool.Start();

		    Platform.Log(LogLevel.Info, "Work Item Processor running...");

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
                
                if ( list == null)
                {
                    /* No threads available, wait for one to complete. */
                    _threadStop.WaitOne(3000, false);
                    continue;
                }
                if  (list.Count == 0)
                {
                    // Cleanup work items that have aged off
                    DeleteWorkItems();

                    // No result found 
                    _threadStop.WaitOne(2000, false);
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
                            proxy.Fail("No plugin to handle WorkItem type: " + item.Type,WorkItemFailureType.Fatal);
                   
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
             	string failureDescription;
                if (!processor.Initialize(proxy))
                {
                	proxy.Postpone();
                	return;
                }

                if (processor.CanStart(proxy, out failureDescription))
                {
                    processor.Process(proxy);
                }
                else
                {
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
			}
		}

        /// <summary>
        /// Cleanup WorkItems that have been marked for deletion or have aged off.
        /// </summary>
        private void DeleteWorkItems()
        {
            List<WorkItem> itemsToDelete;

            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();

                itemsToDelete = workItemBroker.GetWorkItemsToDelete();
            } 

            foreach (var item in itemsToDelete)
            {
                var proxy = new WorkItemStatusProxy(item);
                if (item.Status == WorkItemStatusEnum.Complete)
                    proxy.Delete();
                else
                {
                    //TODO, need to do something special in case there's files on disk to cleanup
                    // Prob. need to do this in another thread.
                }
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
            using (var context = new DataAccessContext())
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

        private void ResetInProgressWorkItems()
        {
            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }
        }

    	#endregion
    }
}
