using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Queue;

namespace ClearCanvas.ImageServer.Shreds.WorkQueueServer
{
    public class WorkQueueServerManager
    {
        #region Private Members
        private static WorkQueueServerManager _instance;
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
            WorkQueueProcessor.Start();
        }

        public void Stop()
        {
            WorkQueueProcessor.Stop();
        }
        #endregion
    }
}
