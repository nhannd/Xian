using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Queue
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
        public void FailQueueItem(WorkQueue item)
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
                    IList<WorkQueue> list = select.Execute(parms);
                    read.Dispose();

                    if (list.Count > 0)
                        foundResult = true;

                    foreach (WorkQueue queueItem in list)
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
                    _threadStop.WaitOne(ImageServerQueueSettings.Default.WorkQueueQueryDelay, false);
                    _threadStop.Reset();
                }
                if (_stop)
                    return;
            }
        }
        #endregion
    }
}
