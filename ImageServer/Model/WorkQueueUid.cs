using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class WorkQueueUid : ServerEntity
    {
        #region Constructors
        public WorkQueueUid()
            : base("WorkQueueUid")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _workQueueKey;
        private String _seriesInstanceUid;
        private String _sopInstanceUid;
        #endregion

        #region Public Properties
        public ServerEntityKey WorkQueueKey
        {
            get { return _workQueueKey; }
            set { _workQueueKey = value; }
        }
        public String SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }
        public String SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }
        #endregion
    }
}
