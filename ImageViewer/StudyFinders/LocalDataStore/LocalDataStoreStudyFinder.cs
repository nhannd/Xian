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
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Iod.Macros;

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

            //.NET strings are unicode, therefore, so is all the query data.
            const string utf8 = "ISO_IR 192";
            var collection = new DicomAttributeCollection {SpecificCharacterSet = utf8};
            collection[DicomTags.SpecificCharacterSet].SetStringValue(utf8);
            
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

			collection[DicomTags.PatientSpeciesDescription].SetStringValue(GetString(queryParams, "PatientSpeciesDescription"));
			var codeValue = GetString(queryParams, "PatientSpeciesCodeSequenceCodeValue");
			var codeMeaning = GetString(queryParams, "PatientSpeciesCodeSequenceCodeMeaning");
			if (codeValue != null || codeMeaning != null)
			{
				var codeSequenceMacro = new CodeSequenceMacro
					{
						CodingSchemeDesignator = "",
						CodeValue = codeValue,
						CodeMeaning = codeMeaning
					};
				collection[DicomTags.PatientSpeciesCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
            }

			collection[DicomTags.PatientBreedDescription].SetStringValue(GetString(queryParams, "PatientBreedDescription"));
			codeValue = GetString(queryParams, "PatientBreedCodeSequenceCodeValue");
			codeMeaning = GetString(queryParams, "PatientBreedCodeSequenceCodeMeaning");
			if (codeValue != null || codeMeaning != null)
			{
				var codeSequenceMacro = new CodeSequenceMacro
					{
						CodingSchemeDesignator = "",
						CodeValue = codeValue,
						CodeMeaning = codeMeaning
					};
				collection[DicomTags.PatientBreedCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
            }

			collection[DicomTags.ResponsiblePerson].SetStringValue(GetString(queryParams, "ResponsiblePerson"));
			collection[DicomTags.ResponsiblePersonRole].SetStringValue("");
			collection[DicomTags.ResponsibleOrganization].SetStringValue(GetString(queryParams, "ResponsibleOrganization"));

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

					item.PatientSpeciesDescription = result[DicomTags.PatientSpeciesDescription].GetString(0, "");
					var patientSpeciesCodeSequence = result[DicomTags.PatientSpeciesCodeSequence];
					if (!patientSpeciesCodeSequence.IsNull && patientSpeciesCodeSequence.Count > 0)
					{
						var codeSequenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) result[DicomTags.PatientSpeciesCodeSequence].Values)[0]);
						item.PatientSpeciesCodeSequenceCodingSchemeDesignator = codeSequenceMacro.CodingSchemeDesignator;
						item.PatientSpeciesCodeSequenceCodeValue = codeSequenceMacro.CodeValue;
						item.PatientSpeciesCodeSequenceCodeMeaning = codeSequenceMacro.CodeMeaning;
					}

					item.PatientBreedDescription = result[DicomTags.PatientBreedDescription].GetString(0, "");
					var patientBreedCodeSequence = result[DicomTags.PatientBreedCodeSequence];
					if (!patientBreedCodeSequence.IsNull && patientBreedCodeSequence.Count > 0)
					{
						var codeSequenceMacro = new CodeSequenceMacro(((DicomSequenceItem[])result[DicomTags.PatientBreedCodeSequence].Values)[0]);
                        item.PatientBreedCodeSequenceCodingSchemeDesignator = codeSequenceMacro.CodingSchemeDesignator;
						item.PatientBreedCodeSequenceCodeValue = codeSequenceMacro.CodeValue;
						item.PatientBreedCodeSequenceCodeMeaning = codeSequenceMacro.CodeMeaning;
					}

					item.ResponsiblePerson = new PersonName(result[DicomTags.ResponsiblePerson].GetString(0, ""));
					item.ResponsiblePersonRole = result[DicomTags.ResponsiblePersonRole].GetString(0, "");
					item.ResponsibleOrganization = result[DicomTags.ResponsibleOrganization].GetString(0, "");

					studyItemList.Add(item);
				}
			}

			AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
									   SopClass.StudyRootQueryRetrieveInformationModelFindUid, collection);

			return studyItemList;
		}

		private static string GetString(QueryParameters queryParams, string key)
		{
			string sResult;
			if (queryParams.TryGetValue(key, out sResult))
				return sResult;
			return "";
		}
	}
}
