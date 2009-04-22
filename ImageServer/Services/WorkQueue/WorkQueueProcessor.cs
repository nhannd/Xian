#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    public class WorkQueueProcessor
    {
        #region Members

        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory> _extensions = new Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory>();
		private readonly WorkQueueThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly ManualResetEvent _terminateEvent;
        private bool _stop = false;
    	private readonly List<WorkQueueTypeEnum> _nonMemoryLimitedList = new List<WorkQueueTypeEnum>();

        #endregion

		#region Constructor
		public WorkQueueProcessor(int numberThreads, ManualResetEvent terminateEvent, string name)
        {
            _terminateEvent = terminateEvent;
            _threadStop = new ManualResetEvent(false);

        	string memoryLimitedTypes = WorkQueueSettings.Instance.NonMemoryLimitedWorkQueueTypes;
			if (memoryLimitedTypes.Length > 0)
			{
				string[] typeArray = memoryLimitedTypes.Split(',');
				foreach (string type in typeArray)
				{
					WorkQueueTypeEnum val = WorkQueueTypeEnum.GetEnum(type);
					if (val != null)
					{
                        if (!_nonMemoryLimitedList.Contains(val))
                            _nonMemoryLimitedList.Add(val);
					}							
				}
			}

			_threadPool =
				new WorkQueueThreadPool(numberThreads,
				                        WorkQueueSettings.Instance.PriorityWorkQueueThreadCount,
				                        WorkQueueSettings.Instance.MemoryLimitedWorkQueueThreadCount,
				                        _nonMemoryLimitedList);
			_threadPool.ThreadPoolName = name + " Pool";


            WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No WorkQueueFactory Extension found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    IWorkQueueProcessorFactory factory = obj as IWorkQueueProcessorFactory;
                    if (factory != null)
                    {
                        WorkQueueTypeEnum type = factory.GetWorkQueueType();
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
			_terminateEvent.Set(); // make sure it's set
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

			Platform.Log(LogLevel.Info, "Work Queue Processor running...");

			while (true)
			{
				if (_stop)
					return;

				if (_threadPool.CanQueueItem)
				{
					try
					{
						Model.WorkQueue queueListItem = GetWorkQueueItem(ServiceTools.ProcessorId);
						if (queueListItem == null)
						{
							/* No result found, or reach max queue entries for each type */
							_terminateEvent.WaitOne(WorkQueueSettings.Instance.WorkQueueQueryDelay, false);
							continue;
						}
						else if (!_extensions.ContainsKey(queueListItem.WorkQueueTypeEnum))
						{
							Platform.Log(LogLevel.Error,
										 "No extensions loaded for WorkQueue item type: {0}.  Failing item.",
										 queueListItem.WorkQueueTypeEnum);

							//Just fail the WorkQueue item, not much else we can do
							FailQueueItem(queueListItem, "No plugin to handle WorkQueue type: " + queueListItem.WorkQueueTypeEnum);
							continue;
						}

						try
						{
							IWorkQueueProcessorFactory factory = _extensions[queueListItem.WorkQueueTypeEnum];
							IWorkQueueItemProcessor processor = factory.GetItemProcessor();

							// Enqueue the actual processing of the item to the thread pool.  
							_threadPool.Enqueue(processor, queueListItem, ExecuteProcessor);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkQueue processor.");
							FailQueueItem(queueListItem, "Failure getting WorkQueue processor: " + e.Message);
							continue;
						}
					}
					catch (Exception e)
					{
						// Wait for only 1.5 seconds
						Platform.Log(LogLevel.Error, e, "Exception occured when processing WorkQueue item.");
						_terminateEvent.WaitOne(3000, false);
					}
				}
				else
				{
					// wait for new opening in the pool or termination
					WaitHandle.WaitAny(new WaitHandle[] { _threadStop, _terminateEvent }, 3000, false);
					_threadStop.Reset();
				}
			}
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// The actual delegate 
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="queueItem"></param>
		private void ExecuteProcessor(IWorkQueueItemProcessor processor, Model.WorkQueue queueItem)
		{
			try
			{
				processor.Process(queueItem);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
							 "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (GUID: {1})",
							 queueItem.WorkQueueTypeEnum,
							 queueItem.GetKey());
				if (e.InnerException != null)
					FailQueueItem(queueItem, e.InnerException.Message);
				else
					FailQueueItem(queueItem, e.Message);

				ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error,
									 processor.Name, AlertTypeCodes.UnableToProcess,
									 "Work Queue item failed: Type={0}, GUID={1}",
									 queueItem.WorkQueueTypeEnum,
									 queueItem.GetKey());
			}


			// Signal the parent thread, so it can query again
			_threadStop.Set();

			// Cleanup the processor
			processor.Dispose();
		}

		/// <summary>
		/// Get array of enumerated values to query on.
		/// </summary>
		/// <returns></returns>
		private string GetNonMemoryLimitedTypeString()
		{
			string typeString = String.Empty;
			if (_nonMemoryLimitedList.Count > 0)
			{
				foreach (WorkQueueTypeEnum type in _nonMemoryLimitedList)
				{
					if (typeString.Length > 0)
						typeString += "," + type.Enum;
					else
						typeString = type.Enum.ToString();
				}
			}
			return typeString;
		}

    	/// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
		/// <param name="failureDescription">The reason for the failure.</param>
        private void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            // Must retry to reset the status of the entry in case of db error
            // Failure to do so will create stale work queue entry (stuck in "In Progress" state)
            // which can only be recovered by restarting the service.
            while(true) 
            {
                try
                {
                    using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                        UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
                        parms.ProcessorID = ServiceTools.ProcessorId;

                        parms.WorkQueueKey = item.GetKey();
                        parms.StudyStorageKey = item.StudyStorageKey;
                        parms.FailureCount = item.FailureCount + 1;
                        parms.FailureDescription = failureDescription;

                        WorkQueueSettings settings = WorkQueueSettings.Instance;
                        if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                        {
                            Platform.Log(LogLevel.Error,
                                         "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
                                         item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                            parms.ScheduledTime = Platform.Time;
                            parms.ExpirationTime = Platform.Time.AddDays(1);
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
                                         item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                            parms.ScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                            parms.ExpirationTime =
                                Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount) *
                                                         settings.WorkQueueFailureDelayMinutes);
                        }

                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                         item.GetKey().ToString());
                        }
                        else
                        {
                            updateContext.Commit();
                            break; // done
                        }
                    }                    
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, "Error occurred when calling FailQueueItem. Retry later. {0}", ex.Message);
                    _terminateEvent.WaitOne(2000, false);
                    if (_stop)
                        break;
                }                
            }
        }
      
		/// <summary>
		/// Method for getting next <see cref="WorkQueue"/> entry.
		/// </summary>
		/// <param name="processorId">The Id of the processor.</param>
		/// <remarks>
		/// </remarks>
		/// <returns>
		/// A <see cref="WorkQueue"/> entry if found, or else null;
		/// </returns>
		public Model.WorkQueue GetWorkQueueItem(string processorId)
		{
			Model.WorkQueue queueListItem = null;

			// If we don't have the max high priority threads in use,
			// first see if there's any available
			if (_threadPool.HighPriorityThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;
					parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// If we didn't find a high priority work queue item, and we have threads 
			// available for memory limited work queue items, query for the next queue item available.
			if (queueListItem == null
				&& _threadPool.MemoryLimitedThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// This logic only accessed if memory limited and priority threads are used up 
			if (queueListItem == null)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
					parms.ProcessorID = processorId;
					parms.WorkQueueTypeEnumList = GetNonMemoryLimitedTypeString();

					queueListItem = select.FindOne(parms);

					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			return queueListItem;
		}

        #endregion
    }
}
