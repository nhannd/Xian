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
        #endregion

        #region Constructor
        public WorkQueueProcessor(String name, int numberThreads)
        {
            _name = name;
            _threadPool = new SimpleBlockingThreadPool(numberThreads);

            WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();
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
            IReadContext read = _store.OpenReadContext();
            IUpdateWorkQueue update = read.GetBroker<IUpdateWorkQueue>();
            WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();

            parms.StatusEnum = StatusEnum.GetEnum("Failed");
            parms.WorkQueueKey = item.GetKey();
            parms.StudyStorageKey = item.StudyStorageKey;
            parms.ScheduledTime = Platform.Time;
            parms.ExpirationTime = Platform.Time.AddDays(1);
            parms.FailureCount = item.FailureCount + 1;
            
            if (false == update.Execute(parms))
            {
                Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.TypeEnum.Name, item.GetKey().ToString());
            }

            read.Dispose();
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
            while (true)
            {
                bool foundResult = false;

                if (_threadPool.QueueCount < _threadPool.Concurrency)
                {
                    IReadContext read = _store.OpenReadContext();
                    IQueryWorkQueue select = read.GetBroker<IQueryWorkQueue>();
                    WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
                    IList<Model.WorkQueue> list = select.Execute(parms);
                    read.Dispose();

                    if (list.Count > 0)
                        foundResult = true;

                    foreach (Model.WorkQueue queueItem in list)
                    {
                        if (!_extensions.ContainsKey(queueItem.TypeEnum))
                        {
                            Platform.Log(LogLevel.Error, "No extensions loaded for WorkQueue item type: {0}.  Failing item.",
                                         queueItem.TypeEnum.Description);

                            //Just fail the WorkQueue item, not much else we can do
                            FailQueueItem(queueItem);
                        }
                        else
                        {
                            IWorkQueueProcessorFactory factory = _extensions[queueItem.TypeEnum];
                            
                            IWorkQueueItemProcessor processor = factory.GetItemProcessor();

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
                                                                         queueItem.TypeEnum.Description,queueItem.GetKey());

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
                if (_stop)
                    return;
            }
        }
        #endregion
    }
}