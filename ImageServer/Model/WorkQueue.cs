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
        private String _seriesInstanceUid;
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
        private String SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }
        private TypeEnum TypeEnum
        {
            get { return _typeEnum; }
            set { _typeEnum = value; }
        }
        private StatusEnum StatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        private DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }
        private DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }
        private DateTime InsertTime
        {
            get { return _insertTime; }
            set { _insertTime = value; }
        }
        private XmlDocument Data
        {
            get { return _data; }
            set { _data = value; }
        }
        #endregion
    }
}
