#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
            SetKey(workQueue.GetKey());
            Data = workQueue.Data;
            ExpirationTime = workQueue.ExpirationTime;
            FailureCount = workQueue.FailureCount;
            FailureDescription = workQueue.FailureDescription;
            InsertTime = workQueue.InsertTime;
            ProcessorID = workQueue.ProcessorID;
            ScheduledTime = workQueue.ScheduledTime;
            ServerPartitionKey = workQueue.ServerPartitionKey;
            StudyHistoryKey = workQueue.StudyHistoryKey;
            StudyStorageKey = workQueue.StudyStorageKey;
            WorkQueuePriorityEnum = workQueue.WorkQueuePriorityEnum;
            WorkQueueStatusEnum = workQueue.WorkQueueStatusEnum;
            WorkQueueTypeEnum = workQueue.WorkQueueTypeEnum;

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

        /// <summary>
        /// NOTE: This location is not updated if the filesystem path is changed.
        /// </summary>
        /// <returns></returns>
        public string GetDuplicateSopFolder()
        {
            return QueueData.DuplicateSopFolder;
        }
        #endregion
    }
}