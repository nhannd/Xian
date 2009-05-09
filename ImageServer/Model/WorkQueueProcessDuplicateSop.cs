#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="WorkQueue"/> for handling duplicates.
    /// </summary>
    public class WorkQueueProcessDuplicateSop : WorkQueue
    {

        #region Static Members
        static readonly XmlSerializer _serializer = new XmlSerializer(typeof(ProcessDuplicateQueueEntryQueueData));
        #endregion

        #region Private Members
        private ProcessDuplicateQueueEntryQueueData _queueData;
        #endregion

        #region Constructors
        public WorkQueueProcessDuplicateSop()
        {

        }

        public WorkQueueProcessDuplicateSop(WorkQueue workQueue)
        {
            this.SetKey(workQueue.GetKey());
            this.Data = workQueue.Data;
            this.ExpirationTime = workQueue.ExpirationTime;
            this.FailureCount = workQueue.FailureCount;
            this.FailureDescription = workQueue.FailureDescription;
            this.InsertTime = workQueue.InsertTime;
            this.ProcessorID = workQueue.ProcessorID;
            this.ScheduledTime = workQueue.ScheduledTime;
            this.ServerPartitionKey = workQueue.ServerPartitionKey;
            this.StudyHistoryKey = workQueue.StudyHistoryKey;
            this.StudyStorageKey = workQueue.StudyStorageKey;
            this.WorkQueuePriorityEnum = workQueue.WorkQueuePriorityEnum;
            this.WorkQueueStatusEnum = workQueue.WorkQueueStatusEnum;
            this.WorkQueueTypeEnum = workQueue.WorkQueueTypeEnum;

            _queueData = (ProcessDuplicateQueueEntryQueueData)_serializer.Deserialize(new XmlNodeReader(workQueue.Data.DocumentElement));

        }
        #endregion

        #region Public Properties

        public ProcessDuplicateQueueEntryQueueData QueueData
        {
            get { return _queueData; }
            set
            {
                _queueData = value;

                StringWriter sw = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(sw);
                _serializer.Serialize(xmlTextWriter, _queueData);

                Data = new XmlDocument();
                Data.LoadXml(sw.ToString());
            }
        }

        #endregion

        #region Public Methods
        public string GetDuplicateSopFolder()
        {
            return QueueData.DuplicateSopFolder;
        }
        #endregion
    }
}