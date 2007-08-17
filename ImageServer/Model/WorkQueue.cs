using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class WorkQueue : ServerEntity
    {
        #region Constructors
        public WorkQueue()
            : base("WorkQueue")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _studyStorageKey;
        private TypeEnum _typeEnum;
        private StatusEnum _statusEnum;
        private DateTime _expirationTime;
        private DateTime _scheduledTime;
        private DateTime _insertTime;
        private XmlDocument _data;
        #endregion

        #region Public Properties
        public ServerEntityKey StudyStorageKey
        {
            get { return _studyStorageKey; }
            set { _studyStorageKey = value; }
        }
        public TypeEnum TypeEnum
        {
            get { return _typeEnum; }
            set { _typeEnum = value; }
        }
        public StatusEnum StatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }
        public DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }
        public DateTime InsertTime
        {
            get { return _insertTime; }
            set { _insertTime = value; }
        }
        public XmlDocument Data
        {
            get { return _data; }
            set { _data = value; }
        }
        #endregion
    }
}
