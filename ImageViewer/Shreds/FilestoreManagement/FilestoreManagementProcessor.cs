#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Shreds.WorkItemService;
using ClearCanvas.ImageViewer.StudyManagement.Storage;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.FilestoreManagement
{
	internal sealed class FilestoreManagementProcessor : QueueProcessor<Study>
	{
		public FilestoreManagementProcessor(int batchSize, TimeSpan sleepTime)
			: base(batchSize, sleepTime)
		{
		}

		protected override IList<Study> GetNextBatch(int batchSize)
		{
			using(var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				return broker.GetStudiesForDeletion(Platform.Time, batchSize);
			}
		}

		protected override void ProcessItem(Study study)
		{
			var result = EventResult.Success;
			try
			{
			    WorkItem item;
				using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
				{
					item = ProcessStudy(study.Oid, context);
					context.Commit();
				}
                if (item != null)
                        WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemHelper.FromWorkItem(item));

			}
			catch (Exception)
			{
				result = EventResult.MajorFailure;
				throw;
			}
			finally
			{
				var instances = new AuditedInstances();
				instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

				AuditHelper.LogDeleteStudies(AuditHelper.LocalAETitle, instances, EventSource.CurrentUser, result);
			}
		}

		private static WorkItem ProcessStudy(long studyOid, DataAccessContext context)
		{
			var studyBroker = context.GetStudyBroker();
			var study = studyBroker.GetStudy(studyOid);

			// on the off chance it was deleted by another thread/process, there's nothing to do here
			if (study.Deleted)
				return null;

			// TODO (Marmot): this code was duplicated from DeleteClient.DeleteStudy
			var now = Platform.Time;
			var item = new WorkItem
			           	{
			           		Request = new DeleteStudyRequest
			           		          	{
			           		          		Study = new WorkItemStudy(study),
			           		          		Patient = new WorkItemPatient(study)
			           		          	},
			           		Type = WorkItemTypeEnum.DeleteStudy,
			           		Priority = WorkItemPriorityEnum.Normal,
			           		InsertTime = now,
			           		ScheduledTime = now.AddSeconds(5),
			           		DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes),
			           		ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds),
			           		Status = WorkItemStatusEnum.Pending,
			           		StudyInstanceUid = study.StudyInstanceUid
			           	};

			var workItemBroker = context.GetWorkItemBroker();
			workItemBroker.AddWorkItem(item);
			study.Deleted = true;
		    return item;
		}
	}
}
