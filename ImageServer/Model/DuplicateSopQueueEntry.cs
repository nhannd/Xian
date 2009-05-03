using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="StudyIntegrityQueue"/>
    /// </summary>
    public class DuplicateSopReceivedQueue : StudyIntegrityQueue
    {
        private string _duplicateSopFolderPath;
        public DuplicateSopReceivedQueue()
        {
            
        }

        public DuplicateSopReceivedQueue(StudyIntegrityQueue studyIntegrityQueueEntry)
        {
            Platform.CheckTrue(studyIntegrityQueueEntry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate,
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
            
        }

        public string GetDuplicateSopPath(string seriesInstanceUid, string sopInstanceUid)
        {
            String path = Path.Combine(GetDuplicateSopFolder(), sopInstanceUid);
            return path + ".dup";
        }
        
        public string GetDuplicateSopFolder()
        {
            if (_duplicateSopFolderPath==null)
            {
                using(IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    StudyStorage storage = StudyStorage.Load(context, base.StudyStorageKey);
                    IList<StudyStorageLocation> studyLocations = StudyStorageLocation.FindStorageLocations(storage);
                    StudyStorageLocation studyLocation = studyLocations[0];

                    String path = Path.Combine(studyLocation.FilesystemPath, studyLocation.PartitionFolder);
                    path = Path.Combine(path, "Duplicate");
                    path = Path.Combine(path, studyLocation.StudyInstanceUid);
                    path = Path.Combine(path, GetKey().Key.ToString());
                    _duplicateSopFolderPath = path;
                }
                
            }
            return _duplicateSopFolderPath;
            
        }


        public new static DuplicateSopReceivedQueue Load(IPersistenceContext context, ServerEntityKey key)
        {
            return new DuplicateSopReceivedQueue(StudyIntegrityQueue.Load(context, key));
        }

    }
}
