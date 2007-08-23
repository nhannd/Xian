using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Queue;

namespace ClearCanvas.ImageServer.Shreds.WorkQueueServer
{
    /// <summary>
    /// Shreds namespace manager of processing threads for the WorkQueue.
    /// </summary>
    public class WorkQueueServerManager
    {
        #region Private Members
        private static WorkQueueServerManager _instance;
        private WorkQueueProcessor _theProcessor;
        #endregion

        #region Constructors
        public WorkQueueServerManager()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// Singleton instance of the class.
        /// </summary>
        public static WorkQueueServerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WorkQueueServerManager();

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion

        #region Public Methods

        public void Start()
        {
            if (_theProcessor == null)
            {
                _theProcessor = new WorkQueueProcessor("WorkQueue Processor",5); // 5 threads for processor
                _theProcessor.Start();
            }
        }

        public void Stop()
        {
            if (_theProcessor != null)
            {
                _theProcessor.Stop();
                _theProcessor = null;
            }
        }
        #endregion
    }
}
