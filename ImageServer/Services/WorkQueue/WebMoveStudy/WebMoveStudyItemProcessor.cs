#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.Dicom;
using ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy
{
    public class WebMoveStudyItemProcessor : AutoRouteItemProcessor
    {
        protected override void AddWorkQueueUidsToSendList(Model.WorkQueue item, ImageServerStorageScu scu)
        {
			string studyPath = StorageLocation.GetStudyPath();

            StudyXml studyXml = LoadStudyXml(StorageLocation);

            scu.LoadStudyFromStudyXml(studyPath, studyXml);
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
			if (item.ScheduledTime.Equals(item.ExpirationTime))
			{
				Platform.Log(LogLevel.Debug, "Removing Idle WebMoveStudy WorkQueueEntry: {0}", item.Key);
				base.PostProcessing(item, WorkQueueProcessorStatus.Complete, 
									WorkQueueProcessorNumProcessed.None,
				                    WorkQueueProcessorDatabaseUpdate.None);
				return;
			}

            if (!LoadStorageLocation(item))
            {
            	Platform.Log(LogLevel.Warn,"Unable to find readable location when processing WebMoveStudy WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2),item.ExpirationTime.AddMinutes(2));
            	return;
            }

            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(item.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.StudyProcess, WorkQueueTypeEnum.ReconcileStudy });
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending});

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item, workQueueCriteria);
            if (relatedItems != null && relatedItems.Count > 0)
            {
                // can't do it now. Reschedule it for future
                relatedItems.Sort(delegate(Model.WorkQueue item1, Model.WorkQueue item2)
                                      {
                                          return item1.ScheduledTime.CompareTo(item2.ScheduledTime);
                                      });

                DateTime newScheduledTime = relatedItems[0].ScheduledTime.AddMinutes(1);
                if (newScheduledTime < Platform.Time.AddMinutes(1))
                    newScheduledTime = Platform.Time.AddMinutes(1);

                PostponeItem(item, newScheduledTime, newScheduledTime.AddDays(1));
                Platform.Log(LogLevel.Info, "{0} postponed to {1}. Study UID={2}", item.WorkQueueTypeEnum, newScheduledTime, StorageLocation.StudyInstanceUid);
            }
            else
            {
                base.ProcessItem(item);
            }
        }
    }
}
