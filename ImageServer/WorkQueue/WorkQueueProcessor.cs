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
        private Thread _theThread = null;
        private bool _stop = false;
        private static ManualResetEvent _threadStop = new ManualResetEvent(false);
        private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        #endregion

        #region Constructor
        private WorkQueueProcessor()
        {
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
