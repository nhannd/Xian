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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class StudyIntegrityQueueController
	{
        private readonly StudyIntegrityQueueAdaptor _adaptor = new StudyIntegrityQueueAdaptor();

		public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<StudyIntegrityQueue> GetRangeStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetReconicleQueueItemsCount(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteStudyIntegrityQueueItem(StudyIntegrityQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

        private void ReconcileStudy(string command,StudyIntegrityQueue item )
        {
            //Ignore the reconcile command if the item is null.
            if (item == null) return;

			// Preload the change description so its not done during the DB transaction
			XmlDocument changeDescription = new XmlDocument();
			changeDescription.LoadXml(command);

			using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                
                LockStudyParameters lockParms = new LockStudyParameters();
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ReconcileScheduled;
                lockParms.StudyStorageKey = item.StudyStorageKey;
                ILockStudy broker = context.GetBroker<ILockStudy>();
                broker.Execute(lockParms);
                if (!lockParms.Successful)
                {
                    throw new ApplicationException(lockParms.FailureReason);
                }

                
				//Add to Study History
				StudyHistoryeAdaptor historyAdaptor = new StudyHistoryeAdaptor();
				StudyHistoryUpdateColumns parameters = new StudyHistoryUpdateColumns();

				parameters.StudyData = item.StudyData;
				parameters.ChangeDescription = changeDescription;
				parameters.StudyStorageKey = item.StudyStorageKey;
				parameters.StudyHistoryTypeEnum = StudyHistoryTypeEnum.StudyReconciled;

				StudyHistory history = historyAdaptor.Add(context, parameters);

				//Create WorkQueue Entry
				WorkQueueAdaptor workQueueAdaptor = new WorkQueueAdaptor();
				WorkQueueUpdateColumns row = new WorkQueueUpdateColumns();
				row.Data = item.QueueData;
				row.ServerPartitionKey = item.ServerPartitionKey;
				row.StudyStorageKey = item.StudyStorageKey;
				row.StudyHistoryKey = history.GetKey();
				row.WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcileStudy;
				row.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                row.ScheduledTime = Platform.Time;
                row.ExpirationTime = Platform.Time.AddHours(1);
				WorkQueue newWorkQueueItem = workQueueAdaptor.Add(context, row);

				StudyIntegrityQueueUidAdaptor studyIntegrityQueueUidAdaptor = new StudyIntegrityQueueUidAdaptor();
				StudyIntegrityQueueUidSelectCriteria crit = new StudyIntegrityQueueUidSelectCriteria();
				crit.StudyIntegrityQueueKey.EqualTo(item.GetKey());
				IList<StudyIntegrityQueueUid> uidList = studyIntegrityQueueUidAdaptor.Get(context, crit);

				WorkQueueUidAdaptor workQueueUidAdaptor = new WorkQueueUidAdaptor();
				WorkQueueUidUpdateColumns update = new WorkQueueUidUpdateColumns();
				foreach (StudyIntegrityQueueUid uid in uidList)
				{
					update.WorkQueueKey = newWorkQueueItem.GetKey();
					update.SeriesInstanceUid = uid.SeriesInstanceUid;
					update.SopInstanceUid = uid.SopInstanceUid;
					workQueueUidAdaptor.Add(context, update);
				}

				//DeleteStudyIntegrityQueue Item
				StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
				criteria.StudyIntegrityQueueKey.EqualTo(item.GetKey());
				studyIntegrityQueueUidAdaptor.Delete(context, criteria);

				StudyIntegrityQueueAdaptor studyIntegrityQueueAdaptor = new StudyIntegrityQueueAdaptor();
				studyIntegrityQueueAdaptor.Delete(context, item.GetKey());

				context.Commit();
			}

		}

	    public void CreateNewStudy(ServerEntityKey itemKey)
	    {
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            ReconcileCreateStudyDescriptor command = new ReconcileCreateStudyDescriptor();
            command.Automatic = false;
	        command.UserName = SessionManager.Current.User.Identity.Name;
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
            command.Commands.Add(new SetTagCommand(DicomTags.StudyInstanceUid, DicomUid.GenerateUid().UID));
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem);
	    }

	    public void MergeStudy(ServerEntityKey itemKey, Boolean useExistingStudy)
        {
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            ReconcileMergeToExistingStudyDescriptor command = new ReconcileMergeToExistingStudyDescriptor();
            command.UserName = SessionManager.Current.User.Identity.Name;
            command.Automatic = false;
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
  
            if(useExistingStudy)
            {
                command.Description = "Merge using existing study information.";
                String xml = XmlUtils.SerializeAsString(command);
                ReconcileStudy(xml, record.QueueItem);    
            }
            else
            {
                command.Description = "Using study information from the conflicting images.";
                
                command.Commands.Add(
                    new SetTagCommand(DicomTags.PatientsName, record.ConflictingImageDetails.StudyInfo.PatientInfo.Name));

                command.Commands.Add(
                                    new SetTagCommand(DicomTags.PatientId, record.ConflictingImageDetails.StudyInfo.PatientInfo.PatientId));
                command.Commands.Add(
                                    new SetTagCommand(DicomTags.PatientsBirthDate, record.ConflictingImageDetails.StudyInfo.PatientInfo.PatientsBirthdate));
                command.Commands.Add(
                                    new SetTagCommand(DicomTags.PatientsSex, record.ConflictingImageDetails.StudyInfo.PatientInfo.Sex));
                command.Commands.Add(
                                    new SetTagCommand(DicomTags.IssuerOfPatientId, record.ConflictingImageDetails.StudyInfo.PatientInfo.IssuerOfPatientId));
                command.Commands.Add(
                                    new SetTagCommand(DicomTags.AccessionNumber, record.ConflictingImageDetails.StudyInfo.AccessionNumber));

                String xml = XmlUtils.SerializeAsString(command);
                ReconcileStudy(xml, record.QueueItem); 
            }
        }

        public void Discard(ServerEntityKey itemKey)
        {
            ReconcileDiscardImagesDescriptor command = new ReconcileDiscardImagesDescriptor();
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            command.UserName = SessionManager.Current.User.Identity.Name;
            
            command.Automatic = false;
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem); 
        }


        public void IgnoreDifferences(ServerEntityKey key)
        {
            ReconcileProcessAsIsDescriptor command = new ReconcileProcessAsIsDescriptor();
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(key));
            command.UserName = SessionManager.Current.Credentials.UserName;
            command.Automatic = false;
            command.Description = "Ignore the differences";
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
            
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem);
        }

	}

    internal class InconsistentDataSIQRecord
    {
        private readonly ImageSetDescriptor _conflictingImageDescriptor;
        private readonly ImageSetDetails _conflictingImageDetails;
        private readonly StudyInformation _existingStudyInfo;
        private readonly StudyIntegrityQueue _queueItem;

        public InconsistentDataSIQRecord(StudyIntegrityQueue queue)
        {
            _queueItem = queue;
            ReconcileStudyWorkQueueData data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(queue.QueueData);
            _conflictingImageDetails = data.Details;
            _conflictingImageDescriptor = XmlUtils.Deserialize<ImageSetDescriptor>(queue.StudyData);
            StudyStorage storage = StudyStorage.Load(HttpContextData.Current.ReadContext, queue.StudyStorageKey);
            Study study = storage.LoadStudy(HttpContextData.Current.ReadContext);
            _existingStudyInfo = new StudyInformation(new ServerEntityAttributeProvider(study));
        }

        public StudyInformation ExistingStudyInfo
        {
            get { return _existingStudyInfo; }
        }

        public ImageSetDescriptor ConflictingImageDescriptor
        {
            get { return _conflictingImageDescriptor; }
        }

        public StudyIntegrityQueue QueueItem
        {
            get { return _queueItem; }
        }

        public ImageSetDetails ConflictingImageDetails
        {
            get { return _conflictingImageDetails; }
        }
    }
}
