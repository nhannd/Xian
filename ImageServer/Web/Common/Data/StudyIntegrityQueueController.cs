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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

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

        private void ReconcileStudy(string command, ServerEntityKey itemKey)
        {
            StudyIntegrityQueueAdaptor queueAdaptor = new StudyIntegrityQueueAdaptor();
            Model.StudyIntegrityQueue item = queueAdaptor.Get(itemKey);

            //Add to Study History
            StudyHistoryeAdaptor historyAdaptor = new StudyHistoryeAdaptor();
            StudyHistoryUpdateColumns parameters = new StudyHistoryUpdateColumns();

            XmlDocument changeDescription = new XmlDocument();
            changeDescription.LoadXml(command);

            parameters.StudyData = item.StudyData;
            parameters.ChangeDescription = changeDescription;
            parameters.StudyStorageKey = item.StudyStorageKey;
            parameters.StudyHistoryTypeEnum = StudyHistoryTypeEnum.StudyReconciled;

            StudyHistory history = historyAdaptor.Add(parameters);

            //Create WorkQueue Entry
            WorkQueueAdaptor workQueueAdaptor = new WorkQueueAdaptor();
            WorkQueueUpdateColumns row = new WorkQueueUpdateColumns();
            row.Data = item.QueueData;
            row.ServerPartitionKey = item.ServerPartitionKey;
            row.StudyStorageKey = item.StudyStorageKey;
            row.StudyHistoryKey = history.GetKey();
            row.WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcileStudy;
            row.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
            row.ScheduledTime = DateTime.Now;
            row.ExpirationTime = DateTime.Now.AddHours(1);
            WorkQueue newWorkQueueItem = workQueueAdaptor.Add(row);

            StudyIntegrityQueueUidAdaptor studyIntegrityQueueUidAdaptor = new StudyIntegrityQueueUidAdaptor();
            StudyIntegrityQueueUidSelectCriteria crit = new StudyIntegrityQueueUidSelectCriteria();
            crit.StudyIntegrityQueueKey.EqualTo(item.GetKey());
            IList<StudyIntegrityQueueUid> uidList = studyIntegrityQueueUidAdaptor.Get(crit);

            WorkQueueUidAdaptor workQueueUidAdaptor = new WorkQueueUidAdaptor();
            WorkQueueUidUpdateColumns update = new WorkQueueUidUpdateColumns();
            foreach (StudyIntegrityQueueUid uid in uidList)
            {
                update.WorkQueueKey = newWorkQueueItem.GetKey();
                update.SeriesInstanceUid = uid.SeriesInstanceUid;
                update.SopInstanceUid = uid.SopInstanceUid;
                workQueueUidAdaptor.Add(update);
            }

            //DeleteStudyIntegrityQueue Item
            StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
            criteria.StudyIntegrityQueueKey.EqualTo(item.GetKey());
            studyIntegrityQueueUidAdaptor.Delete(criteria);

            StudyIntegrityQueueAdaptor studyIntegrityQueueAdaptor = new StudyIntegrityQueueAdaptor();
            studyIntegrityQueueAdaptor.Delete(item.GetKey());

            UpdateStudyState(item.StudyStorageKey);
        }

	    private void UpdateStudyState(ServerEntityKey key)
	    {
	        StudyController studyController = new StudyController();
            if (!studyController.UpdateStudyState(StudyStorage.Load(key)))
                throw new ApplicationException("An error occurred while updating the study state");
	    }


	    public void CreateNewStudy(ServerEntityKey itemKey)
        {
            ReconcileStudy(String.Format("<CreateStudy><SetTag TagPath=\"0020000D\" Value=\"{0}\" /></CreateStudy>", DicomUid.GenerateUid().UID), itemKey);
        }

        public void MergeStudy(ServerEntityKey itemKey, Boolean useExistingStudy)
        {
            StudyIntegrityQueueAdaptor queueAdaptor = new StudyIntegrityQueueAdaptor();
            Model.StudyIntegrityQueue item = queueAdaptor.Get(itemKey);

            string PatientName = string.Empty;
            string PatientID = string.Empty;;
            string IssuerOfPatientID = string.Empty;
            string Birthdate = string.Empty; 
            string AccessionNumber = string.Empty;
            string PatientSex = string.Empty;

            if(useExistingStudy)
            {
                StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
                StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);
                StudyAdaptor studyAdaptor = new StudyAdaptor();
                StudySelectCriteria studycriteria = new StudySelectCriteria();
                studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
                IList<Study> studyList = studyAdaptor.Get(studycriteria);

                if (studyList != null && studyList.Count > 0)
                {
                    Study study = studyList[0];

                    //Set the demographic details using the Existing Patient
                    PatientName = string.Format("<SetTag TagPath=\"00100010\" Value=\"{0}\"/>", study.PatientsName);
                    PatientID = string.Format("<SetTag TagPath=\"00100020\" Value=\"{0}\"/>", study.PatientId);
                    AccessionNumber = string.Format("<SetTag TagPath=\"00080050\" Value=\"{0}\"/>", study.AccessionNumber);
                    PatientSex = string.Format("<SetTag TagPath=\"00100040\" Value=\"{0}\"/>", study.PatientsSex);
                    IssuerOfPatientID = string.Format("<SetTag TagPath=\"00100021\" Value=\"{0}\"/>", study.IssuerOfPatientId);
                    Birthdate = string.Format("<SetTag TagPath=\"00100030\" Value=\"{0}\"/>", study.PatientsBirthDate); 
                }               
            }
            else
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                item.StudyData.WriteTo(xw);

                string studyData = sw.ToString();

                //Set the demographic details using the Conflicting Patient
                PatientName = string.Format("<SetTag TagPath=\"00100010\" Value=\"{0}\"/>", GetConflictingName(studyData));
                PatientID = string.Format("<SetTag TagPath=\"00100020\" Value=\"{0}\"/>", GetConflictingPatientID(studyData));
                AccessionNumber = string.Format("<SetTag TagPath=\"00080050\" Value=\"{0}\"/>", GetConflictingPatientAccessionNumber(studyData));

                string sex = GetConflictingPatientSex(studyData).ToLower();                
                if (sex.Equals("male") || sex.Equals("m"))
                {
                    sex = "M";
                }
                else if (sex.Equals("female") || sex.Equals("f"))
                {
                    sex = "F";
                }
                else
                {
                    sex = "O";
                }

                PatientSex = string.Format("<SetTag TagPath=\"00100040\" Value=\"{0}\"/>", sex);
                IssuerOfPatientID = string.Format("<SetTag TagPath=\"00100021\" Value=\"{0}\"/>", GetConflictingIssuerOfPatientID(studyData));
                Birthdate = string.Format("<SetTag TagPath=\"00100030\" Value=\"{0}\"/>", GetConflictingPatientBirthDate(studyData));                 
            }
            
            ReconcileStudy(String.Format("<MergeStudy>{0}{1}{2}{3}{4}{5}</MergeStudy>", PatientName, PatientID, AccessionNumber, PatientSex, IssuerOfPatientID, Birthdate), itemKey);
        }

        public void Discard(ServerEntityKey itemKey)
        {
            ReconcileStudy("<Discard/>", itemKey);
        }

        private static string GetConflictingName(string studyData)
        {
            string patientNameTag = "00100010";
            return parseXmlString(studyData, patientNameTag);
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
