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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ReconcileQueue
{
    static public class ReconcileDetailsAssembler
    {
        private static int ArraySize = 100;

        static public ReconcileDetails CreateReconcileDetails(Model.ReconcileQueue item)
        {
            ReconcileDetails details = new ReconcileDetails();

            details.ReconcileQueueItem = item;

            StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);
            StudyAdaptor studyAdaptor = new StudyAdaptor();
            StudySelectCriteria studycriteria = new StudySelectCriteria();
            studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
            IList<Study> studyList = studyAdaptor.Get(studycriteria);

            if (studyList != null && studyList.Count > 0)
            {
                Study study = studyList[0];
                details.StudyInstanceUID = study.StudyInstanceUid;

                SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
                seriesCriteria.StudyKey.EqualTo(study.GetKey());
                seriesCriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);

                IList<Series> series = seriesAdaptor.Get(seriesCriteria);

                List<ReconcileDetails.SeriesDetails> existingSeriesList = new List<ReconcileDetails.SeriesDetails>();
                if (series != null)
                {
                    foreach (Series seriesItem in series)
                    {
                        ReconcileDetails.SeriesDetails seriesDetails = new ReconcileDetails.SeriesDetails();
                        seriesDetails.Description = seriesItem.SeriesDescription;
                        seriesDetails.NumberOfInstances = seriesItem.NumberOfSeriesRelatedInstances;

                        existingSeriesList.Add(seriesDetails);
                    }
                }

                details.ExistingPatient.Series = existingSeriesList.ToArray();
            }

            details.ExistingPatient.Name = GetExistingName(item.Description);
            details.ConflictingPatient.Name = GetConflictingName(item.Description);

            ReconcileQueueUidAdaptor uidAdaptor = new ReconcileQueueUidAdaptor();
            ReconcileQueueUidSelectCriteria uidCriteria = new ReconcileQueueUidSelectCriteria();
            uidCriteria.ReconcileQueueKey.EqualTo(item.GetKey());

            IList<ReconcileQueueUid> uidItems = uidAdaptor.Get(uidCriteria);

            //
            // Get the Series information from the ReconcileQueueUid. For each
            // Uid matching the current queue item, count the total number of
            // images for each series description.
            //
            
            ArrayList seriesInstanceUid= new ArrayList();
            int[] seriesCount = new int[ArraySize];
            string[] seriesDescription = new string[ArraySize];

            foreach (ReconcileQueueUid uidItem in uidItems)
            {
                if(seriesInstanceUid.Contains(uidItem.SeriesInstanceUid))
                {
                    int index = seriesInstanceUid.IndexOf(uidItem.SeriesInstanceUid);
                    seriesCount[index] = seriesCount[index] + 1;
                } 
                else
                {
                    int index = seriesInstanceUid.Add(uidItem.SeriesInstanceUid);

                    //Grow the array by ArraySize if there are more unique series.
                    if(index > seriesCount.Length)
                    {
                        int[] tempSeriesCount = new int[seriesCount.Length + ArraySize];
                        seriesCount.CopyTo(tempSeriesCount, 0);
                        seriesCount = tempSeriesCount;

                        string[] tempDescription = new string[seriesDescription.Length + ArraySize];
                        seriesDescription.CopyTo(tempDescription, 0);
                        seriesDescription = tempDescription;
                    }

                    seriesCount[index] = 1;
                    seriesDescription[index] = uidItem.SeriesDescription;
                }
            }

            List<ReconcileDetails.SeriesDetails> seriesList = new List<ReconcileDetails.SeriesDetails>();
            foreach (string uid in seriesInstanceUid)
            {
                int index = seriesInstanceUid.IndexOf(uid);
                
                ReconcileDetails.SeriesDetails seriesDetails = new ReconcileDetails.SeriesDetails();
                seriesDetails.Description = seriesDescription[index];
                seriesDetails.NumberOfInstances = seriesCount[index];
                seriesList.Add(seriesDetails);   
            }

            details.ConflictingPatient.Series = seriesList.ToArray();

            return details;
        }

        //Extract the Existing Patient name from the description column
        static private string GetExistingName(string description)
        {
            String patientNames = description.Substring(description.IndexOf("=") + 1);
            return patientNames.Substring(0, patientNames.IndexOf("\r\n"));
        }

        //Extract the conflicting patient name from the description column
        static private string GetConflictingName(string description)
        {
            String patientNames = description.Substring(description.IndexOf("=") + 1);
            return patientNames.Substring(patientNames.IndexOf("=") + 1);
        }
    }
}
