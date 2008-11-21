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
using System.IO;
using System.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{


    public static class ReconcileDetailsAssembler
    {
        private static int ArraySize = 100;

        public static ReconcileDetails CreateReconcileDetails(StudyIntegrityQueueSummary item)
        {
            ReconcileDetails details = new ReconcileDetails();

            details.StudyIntegrityQueueItem = item.TheStudyIntegrityQueueItem;
            Study study = item.StudySummary.TheStudy;
            details.StudyInstanceUID = study.StudyInstanceUid;

            //Set the demographic details of the Existing Patient
            details.ExistingPatient.PatientID = study.PatientId;
            details.ExistingPatient.AccessionNumber = study.AccessionNumber;
            details.ExistingPatient.Sex = study.PatientsSex;
            details.ExistingPatient.IssuerOfPatientID = study.IssuerOfPatientId;
            details.ExistingPatient.BirthDate = study.PatientsBirthDate;

            SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
            SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
            seriesCriteria.StudyKey.EqualTo(study.GetKey());
            seriesCriteria.ServerPartitionKey.EqualTo(item.StudySummary.ThePartition.GetKey());

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
        

            ReconcileStudyQueueDescription description = ParseDescription(item.TheStudyIntegrityQueueItem.Description);

            details.ExistingPatient.Name = description.ExistingPatientName;
            details.ConflictingPatient.Name = description.ConflictingPatientName;

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            item.TheStudyIntegrityQueueItem.StudyData.WriteTo(xw);

            string studyData = sw.ToString();

            details.ConflictingPatient.PatientID = GetConflictingPatientID(studyData);
            details.ConflictingPatient.AccessionNumber = GetConflictingPatientAccessionNumber(studyData);
            details.ConflictingPatient.Sex = GetConflictingPatientSex(studyData);
            details.ConflictingPatient.IssuerOfPatientID = GetConflictingIssuerOfPatientID(studyData);
            details.ConflictingPatient.BirthDate = GetConflictingPatientBirthDate(studyData);

            StudyIntegrityQueueUidAdaptor uidAdaptor = new StudyIntegrityQueueUidAdaptor();
            StudyIntegrityQueueUidSelectCriteria uidCriteria = new StudyIntegrityQueueUidSelectCriteria();
            uidCriteria.StudyIntegrityQueueKey.EqualTo(item.TheStudyIntegrityQueueItem.GetKey());

            IList<StudyIntegrityQueueUid> uidItems = uidAdaptor.Get(uidCriteria);

            //
            // Get the Series information from the ReconcileQueueUid. For each
            // Uid matching the current queue item, count the total number of
            // images for each series description.
            //

            ArrayList seriesInstanceUid = new ArrayList();
            int[] seriesCount = new int[ArraySize];
            string[] seriesDescription = new string[ArraySize];

            foreach (StudyIntegrityQueueUid uidItem in uidItems)
            {
                if (seriesInstanceUid.Contains(uidItem.SeriesInstanceUid))
                {
                    int index = seriesInstanceUid.IndexOf(uidItem.SeriesInstanceUid);
                    seriesCount[index] = seriesCount[index] + 1;
                }
                else
                {
                    int index = seriesInstanceUid.Add(uidItem.SeriesInstanceUid);

                    //Grow the array by ArraySize if there are more unique series.
                    if (index > seriesCount.Length)
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

        private static ReconcileStudyQueueDescription ParseDescription(string description)
        {
            ReconcileStudyQueueDescription desc = new ReconcileStudyQueueDescription();
            desc.Parse(description);
            return desc;
        }


        private static string GetConflictingPatientID(string studyData)
        {
            string patientIDTag = "00100020";
            return parseXmlString(studyData, patientIDTag);
        }

        private static string GetConflictingPatientSex(string studyData)
        {
            string sexTag = "00100040";
            return parseXmlString(studyData, sexTag);
        }

        private static string GetConflictingPatientBirthDate(string studyData)
        {
            string patientBirthDateTag = "00100030";
            return parseXmlString(studyData, patientBirthDateTag);

        }

        private static string GetConflictingIssuerOfPatientID(string studyData)
        {
            string issuerOfPatientIDTag = "00100021";
            return parseXmlString(studyData, issuerOfPatientIDTag);
        }

        private static string GetConflictingPatientAccessionNumber(string studyData)
        {
            string accessionTag = "00080050";
            return parseXmlString(studyData, accessionTag);
        }

        private static string parseXmlString(string xmlString, string tag)
        {
            string VALUE = "Value=";

            string str = xmlString.Substring(xmlString.IndexOf(tag));
            str = str.Substring(str.IndexOf(VALUE) + VALUE.Length + 1);
            str = str.Substring(0, str.IndexOf("\""));

            return str;
        }
    }
}