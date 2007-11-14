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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    public class ServiceLockProcessor
    {
        #region Members
        private readonly string _name;
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly Dictionary<ServiceLockTypeEnum, IServiceLockProcessorFactory> _extensions = new Dictionary<ServiceLockTypeEnum, IServiceLockProcessorFactory>();
        private readonly SimpleBlockingThreadPool _threadPool;
        private string _processorID = null;
        private ManualResetEvent _threadStop;
        private Thread _theThread = null;
        private bool _stop = false;
        #endregion

        #region Constructor
        public ServiceLockProcessor(String name, int numberThreads)
        {
            _name = name;
            _threadPool = new SimpleBlockingThreadPool(numberThreads);

            ServiceLockFactoryExtensionPoint ep = new ServiceLockFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No ServiceLockFactory Extension found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    IServiceLockProcessorFactory factory = obj as IServiceLockProcessorFactory;
                    if (factory != null)
                    {
                        ServiceLockTypeEnum type = factory.GetServiceLockType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
        }
        #endregion
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

        public void ResetServiceLock(Model.ServiceLock item)
        {
            
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
            // Reset any queue items related to this system that are in a "In Progress" state.
           //ResetFailedItems();

            while (true)
            {
                bool foundResult = false;

                if (_threadPool.QueueCount < _threadPool.Concurrency)
                {
                    using (IUpdateContext read = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IQueryServiceLock select = read.GetBroker<IQueryServiceLock>();
                        ServiceLockQueryParameters parms = new ServiceLockQueryParameters();
                        parms.ProcessorId = ServiceTools.ProcessorId;

                        IList<Model.ServiceLock> list = select.Execute(parms);

                        if (list.Count > 0)
                            foundResult = true;

                        foreach (Model.ServiceLock queueItem in list)
                        {
                            if (!_extensions.ContainsKey(queueItem.ServiceLockTypeEnum))
                            {
                                Platform.Log(LogLevel.Error,
                                             "No extensions loaded for ServiceLockTypeEnum item type: {0}.  Failing item.",
                                             queueItem.ServiceLockTypeEnum.Description);

                                //Just fail the WorkQueue item, not much else we can do
                                ResetServiceLock(queueItem);
                                continue;
                            }

                            IServiceLockProcessorFactory factory = _extensions[queueItem.ServiceLockTypeEnum];

                            IServiceLockItemProcessor processor;
                            try
                            {
                                processor = factory.GetItemProcessor();
                            }
                            catch (Exception e)
                            {
                                Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkQueue processor.");
                                ResetServiceLock(queueItem);
                                continue;
                            }

                            // Assign the id to the processor. All sub processors have the same ID as the parent
                            // Note: 
                            // This approach should be sufficient to work queue reset mechanism. The assumptions are:
                            //      1. only one instance of the WorkQueueProcessor will exist on the same machine at one time.
                            //      2. The only time that the sub-processor dies and leaves the item in "In Progress" state
                            //          is when users stop the service. All other general failures will be handled cleanly by the general
                            //          exception handler.
                            //  
                            //processor.ProcessorId = ServiceTools.ProcessorId;

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
                                                                         queueItem.ServiceLockTypeEnum.Description,
                                                                         queueItem.GetKey());

                                                            ResetServiceLock(queueItem);
                                                        }

                                                        // Cleanup the processor
                                                        processor.Dispose();
                                                    });
                        }
                    }

                    if (!foundResult)
                    {
                        _threadStop.WaitOne(1000*60, false); // once a minute
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
    }
}