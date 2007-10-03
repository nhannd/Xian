using System;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// WorkQueue entry entity.
    /// </summary>
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
        private int _failureCount;
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
        public int FailureCount
        {
            get { return _failureCount; }
            set { _failureCount = value; }
        }
        public XmlDocument Data
        {
            get { return _data; }
            set { _data = value; }
        }
        #endregion

        #region Static Methods
        static public WorkQueue Load(ServerEntityKey key)
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            ISelectWorkQueue broker = read.GetBroker<ISelectWorkQueue>();
            WorkQueue theItem = broker.Load(key);
            read.Dispose();
            return theItem;
        }
        #endregion
    }
}
