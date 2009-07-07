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