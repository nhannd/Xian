#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="StudyIntegrityQueue"/>
    /// </summary>
    public class InconsistentDataSIQEntry : StudyIntegrityQueue
    {

        public InconsistentDataSIQEntry()
        {
            
        }

        public InconsistentDataSIQEntry(StudyIntegrityQueue studyIntegrityQueueEntry)
        {
            Platform.CheckTrue(studyIntegrityQueueEntry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.InconsistentData,
                               String.Format("Cannot copy data from StudyIntegrityQueue record of type {0}",
                                             studyIntegrityQueueEntry.StudyIntegrityReasonEnum));

            this.SetKey(studyIntegrityQueueEntry.Key);
            this.Description = studyIntegrityQueueEntry.Description;
            this.InsertTime = studyIntegrityQueueEntry.InsertTime;
            this.Details = studyIntegrityQueueEntry.Details;
            this.ServerPartitionKey = studyIntegrityQueueEntry.ServerPartitionKey;
            this.StudyData = studyIntegrityQueueEntry.StudyData;
            this.StudyIntegrityReasonEnum = studyIntegrityQueueEntry.StudyIntegrityReasonEnum;
            this.StudyStorageKey = studyIntegrityQueueEntry.StudyStorageKey;
            this.GroupID = studyIntegrityQueueEntry.GroupID;
            
        }


        public new static InconsistentDataSIQEntry Load(IPersistenceContext context, ServerEntityKey key)
        {
            return new InconsistentDataSIQEntry(StudyIntegrityQueue.Load(context, key));
        }

        public string GetFolderPath()
        {
            Platform.CheckForNullReference(Details, "Details");
            // TODO: We should use ReconcileStudyWorkQueueData instead here. But that is impossible 
            // because of the Model<--> COmmon dependency.
 
            XmlNode xmlStoragePath = this.Details.SelectSingleNode("//StoragePath");
            Platform.CheckForNullReference(xmlStoragePath, "xmlStoragePath");
            // TODO: end

            String storagePath = xmlStoragePath.InnerText;
            return storagePath;
        }

        public string GetSopPath(string seriesUid, string instanceUid)
        {
            string path = Path.Combine(GetFolderPath(), instanceUid);
            path += "." + "dcm";
            return path;
        }
    }
}