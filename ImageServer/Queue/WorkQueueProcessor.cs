using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Queue
{
    public class WorkQueueProcessor
    {
        #region Members
        private static WorkQueueProcessor _theProcessor;
        private static ManualResetEvent _threadStop = new ManualResetEvent(false); 
        private Thread _theThread = null;
        private bool _stop = false;
        private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private Dictionary<TypeEnum, IWorkQueueProcessorFactory> _extensions = new Dictionary<TypeEnum, IWorkQueueProcessorFactory>();
        #endregion

        #region Constructor
        private WorkQueueProcessor()
        {
            WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();
            foreach (object obj in factories)
            {
                IWorkQueueProcessorFactory factory = obj as IWorkQueueProcessorFactory;
                TypeEnum type = factory.GetWorkQueueType();
                _extensions.Add(type, factory);
            }
        }
        #endregion

        #region Methods
        private void StartThread()
        {
            _theThread = new Thread(new ThreadStart(Process));
            _theThread.Name = "WorkQueue Processor";
            _theThread.Start();
        }

        private void StopThread()
        {
            _stop = true;
            _threadStop.Set();
            _theThread.Join();
        }

        public void Process()
        {
            while (true)
            {
                IReadContext read = _store.OpenReadContext();
                ISelectWorkQueue select = read.GetBroker<ISelectWorkQueue>();
                WorkQueueSelectParameters parms = new WorkQueueSelectParameters();
                IList<WorkQueue> list = select.Execute(parms);
                read.Dispose();

                foreach (WorkQueue queueItem in list)
                {
                    if (!_extensions.ContainsKey(queueItem.TypeEnum))
                    {
                        Platform.Log(LogLevel.Error, "No extensions loaded for WorkQueue item type: {0}", 
                            queueItem.TypeEnum.Description);
                    }
                    else
                    {
                        IWorkQueueProcessorFactory factory = _extensions[queueItem.TypeEnum];

                        IWorkQueueItemProcessor processor = factory.GetItemProcessor();
                        try
                        {
                            processor.Process(queueItem);
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e, 
                                "Unexpected exception when processing WorkQueue item of type {0}", 
                                queueItem.TypeEnum.Description);
                        }
                    }
                }

                _threadStop.WaitOne(100, false);
                _threadStop.Reset();

                if (_stop == true)
                    return;
            }
        }
        #endregion

        #region Static Methods
        public static void Start()
        {
            if (_theProcessor != null)
            {
                Platform.Log(LogLevel.Error, "Start method of WorkQueueProcessor called when processor already started.");
                return;
            }
            _theProcessor = new WorkQueueProcessor();
            _theProcessor.StartThread();
        }

        public static void Stop()
        {
            if (_theProcessor == null)
            {
                Platform.Log(LogLevel.Error, "Stop method of WorkQueueProcessor called when the processor was not running.");
                return;
            }
            _theProcessor.StopThread();
            _theProcessor = null;
        }
        #endregion
    }
}
