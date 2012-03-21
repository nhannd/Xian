#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItem
{
    /// <summary>
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    sealed public class WorkItemProcessor
    {
        #region Members

        private readonly Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory> _extensions = new Dictionary<WorkItemTypeEnum, IWorkItemProcessorFactory>();
		private readonly WorkItemThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly ManualResetEvent _terminateEvent;
        private bool _stop;

        #endregion

		#region Constructor
        public WorkItemProcessor(int numberStatThreads, int numberNormalThreads, ManualResetEvent terminateEvent, string name)
        {
            _terminateEvent = terminateEvent;
            _threadStop = new ManualResetEvent(false);

			_threadPool = new WorkItemThreadPool(numberStatThreads,numberNormalThreads)
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

        #region Public Methods
		/// <summary>
		/// Stop the WorkQueue processor
		/// </summary>
		public void Stop()
		{
            // Set both events, just in case.
			_terminateEvent.Set();
		    _threadStop.Set();

			_stop = true;
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
        public void Run()
		{		 
		    if (!_threadPool.Active)
		        _threadPool.Start();

		    Platform.Log(LogLevel.Info, "Work Item Processor running...");

            while (true)
            {
                if (_stop)
                    return;

                List<StudyManagement.Storage.WorkItem> list = null;
                if (_threadPool.StatThreadsAvailable > 0)
                {
                    list = GetWorkItems(_threadPool.StatThreadsAvailable, true);
                }
                else if (_threadPool.NormalThreadsAvailable > 0)
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
                    /* No result found */
                    _terminateEvent.WaitOne(2000, false);
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
                    _terminateEvent.WaitOne(3000, false);
                }
            }
		}

        /// <summary>
        /// Cancel a current running WorkItem
        /// </summary>
        /// <param name="workItemOid"></param>
        public void Cancel(int workItemOid)
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
        private void ExecuteProcessor(IWorkItemProcessor processor, StudyManagement.Storage.WorkItem queueItem)
		{
		    var proxy = new WorkItemStatusProxy(queueItem);

			try
			{
             	string failureDescription;
                if (!processor.Intialize(proxy))
                {
                	proxy.Postpone(string.Empty);
                	return;
                }

                if (processor.CanStart(proxy, out failureDescription))
                {
                    processor.Process(proxy);
                }
                else
                {
                    proxy.Postpone(failureDescription);
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
        /// Method for getting next <see cref="StudyManagement.Storage.WorkItem"/> entry.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="stat">Search for stat items.</param>
		/// <remarks>
		/// </remarks>
		/// <returns>
		/// A <see cref="StudyManagement.Storage.WorkItem"/> entry if found, or else null;
		/// </returns>
        public List<StudyManagement.Storage.WorkItem> GetWorkItems(int count, bool stat)
        {
            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();

                var statItems = stat
                                    ? workItemBroker.GetStatPendingWorkItems(count)
                                    : workItemBroker.GetPendingWorkItems(count);

                foreach (var item in statItems)
                {
                    item.Status = WorkItemStatusEnum.InProgress;
                }

                context.Commit();
                return statItems;
            }
        }

    	#endregion
    }
}
