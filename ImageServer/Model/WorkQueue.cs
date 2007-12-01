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
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
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
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _studyStorageKey;
        private ServerEntityKey _deviceKey; 
        private WorkQueueTypeEnum _typeEnum;
        private WorkQueueStatusEnum _statusEnum;
        private DateTime _expirationTime;
        private DateTime _scheduledTime;
        private DateTime _insertTime;
        private int _failureCount;
        private XmlDocument _data;
        private string _processor;
        #endregion

        #region Public Properties
        public ServerEntityKey StudyStorageKey
        {
            get { return _studyStorageKey; }
            set { _studyStorageKey = value; }
        }
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerEntityKey DeviceKey
        {
            get { return _deviceKey; }
            set { _deviceKey = value; }
        }
        public WorkQueueTypeEnum WorkQueueTypeEnum
        {
            get { return _typeEnum; }
            set { _typeEnum = value; }
        }
        public WorkQueueStatusEnum WorkQueueStatusEnum
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
        public string ProcessorID
        {
            get { return _processor; }
            set { _processor = value; }
        }
        #endregion

        #region Static Methods
        static public WorkQueue Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }            
        }
        static public WorkQueue Load(IReadContext read, ServerEntityKey key)
        {
            ISelectWorkQueue broker = read.GetBroker<ISelectWorkQueue>();
            WorkQueue theItem = broker.Load(key);
            return theItem;
        }
        #endregion
    }
}
