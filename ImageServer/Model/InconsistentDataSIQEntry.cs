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
        private StudyStorageLocation _location;

        public InconsistentDataSIQEntry()
        {
            
        }

        public InconsistentDataSIQEntry(StudyIntegrityQueue studyIntegrityQueueEntry)
        {
            Platform.CheckTrue(studyIntegrityQueueEntry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.InconsistentData,
                               String.Format("Cannot copy data from StudyIntegrityQueue record of type {0}",
                                             studyIntegrityQueueEntry.StudyIntegrityReasonEnum));

            this.SetKey(studyIntegrityQueueEntry.GetKey());
            this.Description = studyIntegrityQueueEntry.Description;
            this.InsertTime = studyIntegrityQueueEntry.InsertTime;
            this.QueueData = studyIntegrityQueueEntry.QueueData;
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
            Platform.CheckForNullReference(QueueData, "QueueData");
            // TODO: We should use ReconcileStudyWorkQueueData instead here. But that is impossible 
            // because of the Model<--> COmmon dependency.
 
            XmlNode xmlStoragePath = this.QueueData.SelectSingleNode("//StoragePath");
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