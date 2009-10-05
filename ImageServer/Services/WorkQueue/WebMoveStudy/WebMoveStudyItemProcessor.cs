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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class WebMoveStudyItemProcessor : AutoRouteItemProcessor
    {
        
        /// <summary>
        /// Gets the list of instances to be sent from the study xml
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<StorageInstance> GetStorageInstanceList()
        {
        	IList<WorkQueueUid> seriesList = WorkQueueUidList;

            Platform.CheckForNullReference(StorageLocation, "StorageLocation");

            List<StorageInstance> list = new List<StorageInstance>(); 
            string studyPath = StorageLocation.GetStudyPath();
            StudyXml studyXml = LoadStudyXml(StorageLocation);
            foreach (SeriesXml seriesXml in studyXml)
            {
                // FOR SERIES LEVEL Move,
                // Check if the series is in the WorkQueueUid list. If it is not in the list then don't include it.
				if (seriesList.Count > 0)
				{
					bool found = false;
					foreach (WorkQueueUid uid in seriesList)
					{
						if (!string.IsNullOrEmpty(uid.SeriesInstanceUid))
							if (uid.SeriesInstanceUid.Equals(seriesXml.SeriesInstanceUid))
							{
								found = true;
								break;
							}
					}
					if (!found) continue; // don't send this series
				}

            	foreach (InstanceXml instanceXml in seriesXml)
                {
                    string seriesPath = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);
                    string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ".dcm");
                    StorageInstance instance = new StorageInstance(instancePath);
                    instance.SopClass = instanceXml.SopClass;
                    instance.TransferSyntax = instanceXml.TransferSyntax;
                    instance.SopInstanceUid = instanceXml.SopInstanceUid;
                    instance.StudyInstanceUid = studyXml.StudyInstanceUid;
                    instance.PatientId = studyXml.PatientId;
                    instance.PatientsName = studyXml.PatientsName;

                    list.Add(instance);
                }
            }
            
            return list;
        }

        protected override void OnComplete()
        {
            // Force the entry to idle and stay for a while
            // Note: the assumption is the code will set ScheduledTime = ExpirationTime = some future time
            // so that the item will be removed when it is processed again.
            PostProcessing(WorkQueueItem,
                           WorkQueueProcessorStatus.CompleteDelayDelete, 
                           WorkQueueProcessorDatabaseUpdate.None);
        
        }

        protected override bool CanStart()
        {
            IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
                new WorkQueueTypeEnum[] { WorkQueueTypeEnum.StudyProcess, WorkQueueTypeEnum.ReconcileStudy },
                new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending });
            
            if (relatedItems != null && relatedItems.Count > 0)
            {
                // can't do it now. Reschedule it for future
                List<Model.WorkQueue> list = CollectionUtils.Sort(relatedItems, delegate(Model.WorkQueue item1, Model.WorkQueue item2)
                                      {
                                          return item1.ScheduledTime.CompareTo(item2.ScheduledTime);
                                      });

                DateTime newScheduledTime = relatedItems[0].ScheduledTime.AddSeconds(WorkQueueProperties.PostponeDelaySeconds);
                if (newScheduledTime < Platform.Time.AddMinutes(1))
                    newScheduledTime = Platform.Time.AddMinutes(1);

                PostponeItem(WorkQueueItem, newScheduledTime, newScheduledTime.AddDays(1), "Study is being reconciled.");
                Platform.Log(LogLevel.Info, "{0} postponed to {1}. Study UID={2}", WorkQueueItem.WorkQueueTypeEnum, newScheduledTime, StorageLocation.StudyInstanceUid);
            }

            return base.CanStart();
        }
    }

    
}
