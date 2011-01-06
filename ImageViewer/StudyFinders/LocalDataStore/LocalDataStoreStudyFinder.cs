#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.StudyFinders.LocalDataStore
{
    [ExtensionOf(typeof(StudyFinderExtensionPoint))]
    public class LocalDataStoreStudyFinder : StudyFinder
    {
        public LocalDataStoreStudyFinder()
			: base("DICOM_LOCAL")
        {

        }

        public override StudyItemList Query(QueryParameters queryParams, object targetServer)
        {
			Platform.CheckForNullReference(queryParams, "queryParams");

			DicomAttributeCollection collection = new DicomAttributeCollection();
        	collection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
            collection[DicomTags.PatientId].SetStringValue(queryParams["PatientId"]);
            collection[DicomTags.AccessionNumber].SetStringValue(queryParams["AccessionNumber"]);
            collection[DicomTags.PatientsName].SetStringValue(queryParams["PatientsName"]);
			collection[DicomTags.ReferringPhysiciansName].SetStringValue(queryParams["ReferringPhysiciansName"]);
			collection[DicomTags.StudyDate].SetStringValue(queryParams["StudyDate"]);
			collection[DicomTags.StudyTime].SetStringValue("");
			collection[DicomTags.StudyDescription].SetStringValue(queryParams["StudyDescription"]);
        	collection[DicomTags.PatientsBirthDate].SetStringValue("");
            collection[DicomTags.ModalitiesInStudy].SetStringValue(queryParams["ModalitiesInStudy"]);
			collection[DicomTags.StudyInstanceUid].SetStringValue(queryParams["StudyInstanceUid"]);
			collection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");
			collection[DicomTags.InstanceAvailability].SetStringValue("");

            StudyItemList studyItemList = new StudyItemList();
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (DicomAttributeCollection result in reader.Query(collection))
				{
					StudyItem item = new StudyItem(result[DicomTags.StudyInstanceUid].ToString(), null, Name);
					item.SpecificCharacterSet = result.SpecificCharacterSet;
					item.PatientId = result[DicomTags.PatientId].ToString();
					item.PatientsName = new PersonName(result[DicomTags.PatientsName].ToString());
					item.ReferringPhysiciansName = new PersonName(result[DicomTags.ReferringPhysiciansName].GetString(0, ""));
					item.PatientsBirthDate = result[DicomTags.PatientsBirthDate].ToString();
					item.StudyDate = result[DicomTags.StudyDate].ToString();
					item.StudyTime = result[DicomTags.StudyTime].ToString();
					item.StudyDescription = result[DicomTags.StudyDescription].ToString();
					item.ModalitiesInStudy = DicomStringHelper.GetStringArray(result[DicomTags.ModalitiesInStudy].ToString());
					item.AccessionNumber = result[DicomTags.AccessionNumber].ToString();
					item.NumberOfStudyRelatedInstances = result[DicomTags.NumberOfStudyRelatedInstances].GetInt32(0, 0);
					item.InstanceAvailability = result[DicomTags.InstanceAvailability].GetString(0, "");
					if (String.IsNullOrEmpty(item.InstanceAvailability))
						item.InstanceAvailability = "ONLINE";

					studyItemList.Add(item);
				}
			}

        	AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
        	                           SopClass.StudyRootQueryRetrieveInformationModelFindUid, collection);

            return studyItemList;
        }
    }
}
