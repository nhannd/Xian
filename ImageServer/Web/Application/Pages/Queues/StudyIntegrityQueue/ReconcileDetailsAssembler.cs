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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public static class ReconcileDetailsAssembler
    {
        public static ReconcileDetails CreateReconcileDetails(StudyIntegrityQueueSummary item)
        {
            ReconcileDetails details=null;
            if (item.TheStudyIntegrityQueueItem.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.InconsistentData))
                details = new ReconcileDetails(item.TheStudyIntegrityQueueItem);
            else
                details = new DuplicateEntryDetails(item.TheStudyIntegrityQueueItem);

            Study study = item.StudySummary.TheStudy;
            details.StudyInstanceUID = study.StudyInstanceUid;

            //Set the demographic details of the Existing Patient
            details.ExistingStudy = new ReconcileDetails.StudyInfo();
            details.ExistingStudy.StudyInstanceUID = study.StudyInstanceUid;
            details.ExistingStudy.AccessionNumber = study.AccessionNumber;
            details.ExistingStudy.StudyDate = study.StudyDate;
            details.ExistingStudy.Patient.PatientID = study.PatientId;
            details.ExistingStudy.Patient.Name = study.PatientsName;
            details.ExistingStudy.Patient.Sex = study.PatientsSex;
            details.ExistingStudy.Patient.IssuerOfPatientID = study.IssuerOfPatientId;
            details.ExistingStudy.Patient.BirthDate = study.PatientsBirthDate;
            details.ExistingStudy.Series = CollectionUtils.Map<Model.Series, ReconcileDetails.SeriesDetails>(
                study.Series,
                delegate(Model.Series theSeries)
                    {
                        ReconcileDetails.SeriesDetails seriesDetails = new ReconcileDetails.SeriesDetails();
                        seriesDetails.Description = theSeries.SeriesDescription;
                        seriesDetails.SeriesInstanceUid = theSeries.SeriesInstanceUid;
                        seriesDetails.Modality = theSeries.Modality;
                        seriesDetails.NumberOfInstances = theSeries.NumberOfSeriesRelatedInstances;
                        return seriesDetails;
                    });


            details.ConflictingImageSet = item.QueueData.Details;


            details.ConflictingStudyInfo = new ReconcileDetails.StudyInfo();

            if (item.QueueData.Details != null)
            {
                // extract the conflicting study info from QueueData
                details.ConflictingStudyInfo.AccessionNumber = item.QueueData.Details.StudyInfo.AccessionNumber;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;
                details.ConflictingStudyInfo.StudyInstanceUID = item.QueueData.Details.StudyInfo.StudyInstanceUid;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo();
                details.ConflictingStudyInfo.Patient.BirthDate = item.QueueData.Details.StudyInfo.PatientInfo.PatientsBirthdate;
                details.ConflictingStudyInfo.Patient.IssuerOfPatientID = item.QueueData.Details.StudyInfo.PatientInfo.IssuerOfPatientId;
                details.ConflictingStudyInfo.Patient.Name = item.QueueData.Details.StudyInfo.PatientInfo.Name;
                details.ConflictingStudyInfo.Patient.PatientID = item.QueueData.Details.StudyInfo.PatientInfo.PatientId;
                details.ConflictingStudyInfo.Patient.Sex = item.QueueData.Details.StudyInfo.PatientInfo.Sex;

                details.ConflictingStudyInfo.Series =
                    CollectionUtils.Map<SeriesInformation, ReconcileDetails.SeriesDetails>(
                        item.QueueData.Details.StudyInfo.Series,
                        delegate(SeriesInformation input)
                            {
                                ReconcileDetails.SeriesDetails seriesDetails = new ReconcileDetails.SeriesDetails();
                                seriesDetails.Description = input.SeriesDescription;
                                seriesDetails.Modality = input.Modality;
                                seriesDetails.SeriesInstanceUid = input.SeriesInstanceUid;
                                seriesDetails.NumberOfInstances = input.NumberOfInstances;
                                return seriesDetails;
                            });
            }
            else
            {
                // Extract the conflicting study info from StudyData
                // Note: Not all fields are available.
                ImageSetDescriptor desc = ImageSetDescriptor.Parse(item.TheStudyIntegrityQueueItem.StudyData.DocumentElement);

                details.ConflictingStudyInfo.AccessionNumber = desc[DicomTags.AccessionNumber] != null
                                                                   ? desc[DicomTags.AccessionNumber].Value
                                                                   : null;
                details.ConflictingStudyInfo.StudyDate = desc[DicomTags.StudyDate] != null
                                                             ? desc[DicomTags.StudyDate].Value
                                                             : null;
                details.ConflictingStudyInfo.StudyInstanceUID = desc[DicomTags.StudyInstanceUid] != null
                                                                    ? desc[DicomTags.StudyInstanceUid].Value
                                                                    : null;
                details.ConflictingStudyInfo.StudyDate = desc[DicomTags.StudyDate] != null
                                                             ? desc[DicomTags.StudyDate].Value
                                                             : null;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo();
                details.ConflictingStudyInfo.Patient.BirthDate = desc[DicomTags.PatientsBirthDate] != null
                                                                     ? desc[DicomTags.PatientsBirthDate].Value
                                                                     : null;
                details.ConflictingStudyInfo.Patient.IssuerOfPatientID = desc[DicomTags.IssuerOfPatientId] != null
                                                                             ? desc[DicomTags.IssuerOfPatientId].Value
                                                                             : null;
                details.ConflictingStudyInfo.Patient.Name = desc[DicomTags.PatientsName] != null
                                                                ? desc[DicomTags.PatientsName].Value
                                                                : null;
                details.ConflictingStudyInfo.Patient.PatientID = desc[DicomTags.PatientId] != null
                                                                     ? desc[DicomTags.PatientId].Value
                                                                     : null;
                details.ConflictingStudyInfo.Patient.Sex = desc[DicomTags.PatientsSex] != null
                                                               ? desc[DicomTags.PatientsSex].Value
                                                               : null;
                
                
                List<ReconcileDetails.SeriesDetails> series = new List<ReconcileDetails.SeriesDetails>();
                details.ConflictingStudyInfo.Series = series;

                IStudyIntegrityQueueUidEntityBroker uidBroker =
                    HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
                StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
                criteria.StudyIntegrityQueueKey.EqualTo(item.TheStudyIntegrityQueueItem.GetKey());

                IList<StudyIntegrityQueueUid> uids = uidBroker.Find(criteria);

                Dictionary<string, List<StudyIntegrityQueueUid>> seriesGroups = CollectionUtils.GroupBy<StudyIntegrityQueueUid, string>(uids, delegate(StudyIntegrityQueueUid uid) { return uid.SeriesInstanceUid; });

                foreach (string seriesUid in seriesGroups.Keys)
                {
                    ReconcileDetails.SeriesDetails seriesDetails = new ReconcileDetails.SeriesDetails();
                    seriesDetails.SeriesInstanceUid = seriesUid; 
                    seriesDetails.Description = seriesGroups[seriesUid][0].SeriesDescription;
                    seriesDetails.NumberOfInstances = seriesGroups[seriesUid].Count;
                    //seriesDetails.Modality = "N/A";
                    series.Add(seriesDetails);
                }
            }
            

            return details;
        }
    }
}