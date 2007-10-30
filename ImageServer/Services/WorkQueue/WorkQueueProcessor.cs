#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.WorkQueue;
using System.Net;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    public class WorkQueueProcessor
    {
        #region Members
        private string _name;
        private ManualResetEvent _threadStop; 
        private Thread _theThread = null;
        private bool _stop = false;
        private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private Dictionary<TypeEnum, IWorkQueueProcessorFactory> _extensions = new Dictionary<TypeEnum, IWorkQueueProcessorFactory>();
        private SimpleBlockingThreadPool _threadPool;
        private string _processorID = null;
        #endregion

        #region Constructor
        public WorkQueueProcessor(String name, int numberThreads)
        {
            _name = name;
            _threadPool = new SimpleBlockingThreadPool(numberThreads);

            WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No WorkQueueFactory Extension found.");
            }

            foreach (object obj in factories)
            {
                IWorkQueueProcessorFactory factory = obj as IWorkQueueProcessorFactory;
                if (factory != null)
                {
                    TypeEnum type = factory.GetWorkQueueType();
                    _extensions.Add(type, factory);
                }
                else 
                    Platform.Log(LogLevel.Error,"Unexpected incorrect type loaded for extension: {0}",obj.GetType());
            }

        }
        #endregion

        #region public members
        /// <summary>
        /// A string representing the ID of the work queue processor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This ID is used to reset the work queue items.
        /// </para>
        /// <para>
        /// For the time being, the machine ID is tied to the IP address. Assumimg the server
        /// will be installed on a machine with DHCP disabled or if the DNS server always assign
        /// the same IP for the machine, this will work fine.
        /// </para>
        /// <para>
        /// Because of this implemenation, all instances of WorkQueueProcessor will have the same ID.
        /// </para>
        /// </remarks>
        protected string ProcessorID
        {
            get { 

                if (_processorID==null)
                {
                    try
                    {
                        String strHostName = Dns.GetHostName();
                        
                        // Find host by name
                        IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

                        // Enumerate IP addresses
                        foreach (IPAddress ipaddress in iphostentry.AddressList)
                        {
                            _processorID = ipaddress.ToString();
                            break;
                        } 
                    }catch(Exception e)
                    {
                        Platform.Log(LogLevel.Error, "Cannot resolve hostname into IP address");
                    }
                }

                if (_processorID == null)
                {
                    Platform.Log(LogLevel.Warn, "Could not determine hostname or IP address of the local machine. Work Queue Processor ID is set to Unknown");
                    _processorID = "Unknown";

                }

                return _processorID;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start the WorkQueue processor
        /// </summary>
        public void Start()
        {       
            if (!_threadPool.Active)
                _threadPool.Start();
            if (_theThread == null)
            {
                _threadStop = new ManualResetEvent(false); 
                _theThread = new Thread(Process);
                _theThread.Name = _name;
                _theThread.Start();
            }
        }

        /// <summary>
        /// Stop the WorkQueue processor
        /// </summary>
        public void Stop()
        {
            _stop = true;
            _threadStop.Set();
            _theThread.Join();
            _theThread = null;
            if (_threadPool.Active)
                _threadPool.Stop();
        }

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        public void FailQueueItem(Model.WorkQueue item)
        {
            using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                parms.ProcessorID = ProcessorID;

                parms.StatusEnum = StatusEnum.GetEnum("Failed");
                parms.WorkQueueKey = item.GetKey();
                parms.FailureCount = item.FailureCount + 1;
                parms.StudyStorageKey = item.StudyStorageKey;

                // change the expiration time so that 
                // the item stays in the queue for a while before somebody clean it up
                parms.ScheduledTime = Platform.Time;
                    // note: we probably don't need to change the scheduled time. Or do we?
                parms.ExpirationTime = Platform.Time.AddDays(1);

                if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.TypeEnum.Name,
                                 item.GetKey().ToString());
                }

                updateContext.Commit();
            }
        }

        /// <summary>
        /// Reset queue items that were unadvertly left in "in progress" state by previous run. 
        /// </summary>
        public void ResetFailedItems()
        {
            ImageServerServicesWorkQueueSettings settings = ImageServerServicesWorkQueueSettings.Default;

            StatusEnum pending = StatusEnum.GetEnum("Pending");
            StatusEnum failed = StatusEnum.GetEnum("Failed");

            using (IUpdateContext ctx = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueReset reset = ctx.GetBroker<IWorkQueueReset>();
                WorkQueueResetParameters parms = new WorkQueueResetParameters();
                parms.ProcessorID = ProcessorID;

                // reschedule to start again now
                parms.RescheduleTime = Platform.Time;
                // retry will expire X minutes from now (so other process MAY NOT remove them)
                parms.RetryExpirationTime = Platform.Time.AddMinutes(settings.WorkQueueMaxFailureCount * settings.WorkQueueFailureDelayMinutes);

                // if an entry has been retried more than WorkQueueMaxFailureCount, it should be failed
                parms.MaxFailureCount = settings.WorkQueueMaxFailureCount;
                // failed item expires now (so other process can remove them if desired)
                parms.FailedExpirationTime = Platform.Time;

                IList<Model.WorkQueue> modifiedList = reset.Execute(parms);

                if (modifiedList != null)
                {
                    // output the list of items that have been reset
                    foreach (Model.WorkQueue queueItem in modifiedList)
                    {
                        if (queueItem.StatusEnum.Equals(pending))
                            Platform.Log(LogLevel.Info, "Cleanup: Reset Queue Item : {0} --> Status={1} Scheduled={2} ExpirationTime={3}",
                                            queueItem.GetKey().Key, 
                                            queueItem.StatusEnum.Description, 
                                            queueItem.ScheduledTime, 
                                            queueItem.ExpirationTime);
                    }

                    // output the list of items that have been failed because it exceeds the max retry count
                    foreach (Model.WorkQueue queueItem in modifiedList)
                    {
                        if (queueItem.StatusEnum.Equals(failed))
                            Platform.Log(LogLevel.Info, "Cleanup: Fail Queue Item  : {0} : FailureCount={1} ExpirationTime={2}",
                                            queueItem.GetKey().Key,
                                            queueItem.FailureCount,
                                            queueItem.ExpirationTime);
                    }                    
                }     
           
                ctx.Commit();
            }
        }


        /// <summary>
        /// The processing thread.
        /// </summary>
        /// <remarks>
        /// This method queries the database for WorkQueue entries to work on, and then uses
        /// a thread pool to process the entries.
        /// </remarks>
        private void Process()
        {
            ResetFailedItems();

            while (true)
            {
                bool foundResult = false;

                if (_threadPool.QueueCount < _threadPool.Concurrency)
                {
                    using (IUpdateContext read = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IQueryWorkQueue select = read.GetBroker<IQueryWorkQueue>();
                        WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
                        parms.ProcessorID = ProcessorID;

                        IList<Model.WorkQueue> list = select.Execute(parms);

                        if (list.Count > 0)
                            foundResult = true;

                        foreach (Model.WorkQueue queueItem in list)
                        {
                            if (!_extensions.ContainsKey(queueItem.TypeEnum))
                            {
                                Platform.Log(LogLevel.Error,
                                             "No extensions loaded for WorkQueue item type: {0}.  Failing item.",
                                             queueItem.TypeEnum.Description);

                                //Just fail the WorkQueue item, not much else we can do
                                FailQueueItem(queueItem);
                            }
                            else
                            {
                                IWorkQueueProcessorFactory factory = _extensions[queueItem.TypeEnum];

                                IWorkQueueItemProcessor processor = factory.GetItemProcessor();

                                // Assign the id to the processor. All sub processors have the same ID as the parent
                                // Note: 
                                // This approach should be sufficient to work queue reset mechanism. The assumptions are:
                                //      1. only one instance of the WorkQueueProcessor will exist on the same machine at one time.
                                //      2. The only time that the sub-processor dies and leaves the item in "In Progress" state
                                //          is when users stop the service. All other general failures will be handled cleanly by the general
                                //          exception handler.
                                //  
                                processor.ProcessorID = ProcessorID;

                                // Enqueue the actual processing of the item to the 
                                // thread pool.  
                                _threadPool.Enqueue(delegate
                                                        {
                                                            try
                                                            {
                                                                processor.Process(queueItem);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Platform.Log(LogLevel.Error, e,
                                                                             "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (GUID: {1})",
                                                                             queueItem.TypeEnum.Description,
                                                                             queueItem.GetKey());

                                                                FailQueueItem(queueItem);
                                                            }

                                                            // Cleanup the processor
                                                            processor.Dispose();
                                                        });
                            }
                        }
                    }

                    if (!foundResult)
                    {
                        _threadStop.WaitOne(ImageServerServicesWorkQueueSettings.Default.WorkQueueQueryDelay, false);
                        _threadStop.Reset();
                    }
                }
                else
                {
                    // Wait for only 1 second when the thread pool is all in use.
                    _threadStop.WaitOne(1000, false);
                    _threadStop.Reset();                    
                }
                if (_stop)
                    return;
            }
        }
        #endregion
    }
}