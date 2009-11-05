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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Controllers for updating <see cref="StudyIntegrityQueue"/> record that has <see cref="StudyIntegrityReasonEnum"/> equal to <see cref="StudyIntegrityReasonEnum.Duplicate"/>
    /// </summary>
    public class DuplicateSopEntryController
    {
        /// <summary>
        /// Inserts work queue entry to process the duplicates.
        /// </summary>
        /// <param name="entryKey"><see cref="ServerEntityKey"/> of the <see cref="StudyIntegrityQueue"/> entry  that has <see cref="StudyIntegrityReasonEnum"/> equal to <see cref="StudyIntegrityReasonEnum.Duplicate"/> </param>
        /// <param name="action"></param>
        public void Process(ServerEntityKey entryKey, ProcessDuplicateAction action)
        {
            
            DuplicateSopReceivedQueue entry = DuplicateSopReceivedQueue.Load(HttpContextData.Current.ReadContext, entryKey);
            Platform.CheckTrue(entry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate, "Invalid type of entry");

            IList<StudyIntegrityQueueUid> uids = LoadDuplicateSopUid(entry);

            using(IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ProcessDuplicateQueueEntryQueueData data = new ProcessDuplicateQueueEntryQueueData
                                                           	{
                                                           		Action = action,
                                                           		DuplicateSopFolder = entry.GetFolderPath()
                                                           	};

            	IWorkQueueProcessDuplicateSopBroker broker = context.GetBroker<IWorkQueueProcessDuplicateSopBroker>();
                WorkQueueProcessDuplicateSopUpdateColumns columns = new WorkQueueProcessDuplicateSopUpdateColumns
                                                                    	{
                                                                    		Data = XmlUtils.SerializeAsXmlDoc(data),
                                                                    		GroupID = entry.GroupID,
                                                                    		ScheduledTime = Platform.Time,
                                                                    		ExpirationTime =
                                                                    			Platform.Time.Add(TimeSpan.FromMinutes(15)),
                                                                    		ServerPartitionKey = entry.ServerPartitionKey,
                                                                    		WorkQueuePriorityEnum =
                                                                    			WorkQueuePriorityEnum.Medium,
                                                                    		StudyStorageKey = entry.StudyStorageKey,
                                                                    		WorkQueueStatusEnum = WorkQueueStatusEnum.Pending
                                                                    	};

            	WorkQueueProcessDuplicateSop processDuplicateWorkQueueEntry = broker.Insert(columns);

                IWorkQueueUidEntityBroker workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
                IStudyIntegrityQueueUidEntityBroker duplicateUidBroke = context.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
                foreach (StudyIntegrityQueueUid uid in uids)
                {
                    WorkQueueUidUpdateColumns uidColumns = new WorkQueueUidUpdateColumns
                                                           	{
                                                           		Duplicate = true,
                                                           		Extension = ServerPlatform.DuplicateFileExtension,
                                                           		SeriesInstanceUid = uid.SeriesInstanceUid,
                                                           		SopInstanceUid = uid.SopInstanceUid,
                                                           		RelativePath = uid.RelativePath,
                                                           		WorkQueueKey = processDuplicateWorkQueueEntry.GetKey()
                                                           	};

                	workQueueUidBroker.Insert(uidColumns);

                    duplicateUidBroke.Delete(uid.GetKey());
                }

                IDuplicateSopEntryEntityBroker duplicateEntryBroker =
                    context.GetBroker<IDuplicateSopEntryEntityBroker>();
                duplicateEntryBroker.Delete(entry.GetKey());


                context.Commit();
            }
        }

        private static IList<StudyIntegrityQueueUid> LoadDuplicateSopUid(DuplicateSopReceivedQueue entry)
        {
            IStudyIntegrityQueueUidEntityBroker broker =
                HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueUidEntityBroker>();

            StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
            criteria.StudyIntegrityQueueKey.EqualTo(entry.GetKey());
            return broker.Find(criteria);
        }
    }
}
