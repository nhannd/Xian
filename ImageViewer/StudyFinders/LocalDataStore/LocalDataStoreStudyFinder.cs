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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.StudyFinders.LocalDataStore
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyFinderExtensionPoint))]
    public class LocalDataStoreStudyFinder : IStudyFinder
    {
        public LocalDataStoreStudyFinder()
        {

        }

        public string Name
        {
            get
            {
                return "DICOM_LOCAL";
            }
        }

        public StudyItemList Query(QueryParameters queryParams, object targetServer)
        {
			Platform.CheckForNullReference(queryParams, "queryParams");

			DicomAttributeCollection collection = new DicomAttributeCollection();
        	collection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
            collection[DicomTags.PatientId].SetStringValue(queryParams["PatientId"]);
            collection[DicomTags.AccessionNumber].SetStringValue(queryParams["AccessionNumber"]);
            collection[DicomTags.PatientsName].SetStringValue(queryParams["PatientsName"]);
            collection[DicomTags.StudyDate].SetStringValue(queryParams["StudyDate"]);
			collection[DicomTags.StudyTime].SetStringValue("");
			collection[DicomTags.StudyDescription].SetStringValue(queryParams["StudyDescription"]);
        	collection[DicomTags.PatientsBirthDate].SetStringValue("");
            collection[DicomTags.ModalitiesInStudy].SetStringValue(queryParams["ModalitiesInStudy"]);
            collection[DicomTags.SpecificCharacterSet].SetStringValue("");
			collection[DicomTags.StudyInstanceUid].SetStringValue(queryParams["StudyInstanceUid"]);
			collection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");
			collection[DicomTags.InstanceAvailability].SetStringValue("");

            StudyItemList studyItemList = new StudyItemList();
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (DicomAttributeCollection result in reader.Query(collection))
				{
					StudyItem item = new StudyItem(result[DicomTags.StudyInstanceUid].ToString(), null, this.Name);
					item.SpecificCharacterSet = result.SpecificCharacterSet;
					item.PatientId = result[DicomTags.PatientId].ToString();
					item.PatientsName = new PersonName(result[DicomTags.PatientsName].ToString());
					item.PatientsBirthDate = result[DicomTags.PatientsBirthDate].ToString();
					item.StudyDate = result[DicomTags.StudyDate].ToString();
					item.StudyTime = result[DicomTags.StudyTime].ToString();
					item.StudyDescription = result[DicomTags.StudyDescription].ToString();
					item.ModalitiesInStudy = result[DicomTags.ModalitiesInStudy].ToString();
					item.AccessionNumber = result[DicomTags.AccessionNumber].ToString();
					item.NumberOfStudyRelatedInstances = result[DicomTags.NumberOfStudyRelatedInstances].GetUInt32(0, 0);
					item.InstanceAvailability = result[DicomTags.InstanceAvailability].GetString(0, "");
					if (String.IsNullOrEmpty(item.InstanceAvailability))
						item.InstanceAvailability = "ONLINE";

					studyItemList.Add(item);
				}
			}

			AuditedInstances queriedInstances = new AuditedInstances();
			studyItemList.ForEach(delegate(StudyItem study) { queriedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUID); });
			AuditHelper.LogQueryStudies("Find Studies", null, null, queriedInstances, EventSource.CurrentUser, EventResult.Success);

            return studyItemList;
        }
    }
}
