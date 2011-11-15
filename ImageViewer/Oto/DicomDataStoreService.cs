#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Oto;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Oto
{
	[ExtensionOf(typeof(OtoServiceExtensionPoint))]
	[OtoServiceBehaviour(UseSynchronizationContext = true)]
	public class DicomDataStoreService : OtoServiceBase
	{
		[DataContract]
		public class StudyProperties
		{
			[DataMember]
			[Description("DICOM Patient ID")]
			public string PatientId;

			[DataMember]
			[Description("DICOM Accession Number")]
			public string AccessionNumber;

			[DataMember]
			[Description("DICOM Patients Name")]
			public string PatientsName;

			[DataMember]
			[Description("DICOM Study Date")]
			public DateTime? StudyDate;

			[DataMember]
			[Description("DICOM Study Description")]
			public string StudyDescription;

			[DataMember]
			[Description("DICOM Modalities in Study")]
			public string ModalitiesInStudy;

			[DataMember]
			[Description("DICOM Study Instance UID")]
			public string StudyInstanceUID;
		}


		[DataContract]
		public class QueryInput : StudyProperties
		{
		}

		[DataContract]
		public class QueryOutput
		{
			public QueryOutput(List<StudyProperties> results)
			{
				this.Results = results;
			}

			[DataMember]
			[Description("Results of the query.")]
			public List<StudyProperties> Results;
		}

		[Description("Query the local datastore for studies matching the specified properties.")]
		public QueryOutput QueryStudies(QueryInput input)
		{
			var criteria = new DicomAttributeCollection();
			criteria[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			criteria[DicomTags.PatientId].SetStringValue(input.PatientId);
			criteria[DicomTags.AccessionNumber].SetStringValue(input.AccessionNumber);
			criteria[DicomTags.PatientsName].SetStringValue(input.PatientsName);
			criteria[DicomTags.StudyDate].SetDateTime(0, input.StudyDate);
			criteria[DicomTags.StudyDescription].SetStringValue(input.StudyDescription);
			criteria[DicomTags.PatientsBirthDate].SetStringValue("");
			criteria[DicomTags.ModalitiesInStudy].SetStringValue(input.ModalitiesInStudy);
			criteria[DicomTags.SpecificCharacterSet].SetStringValue("");
			criteria[DicomTags.StudyInstanceUid].SetStringValue(input.StudyInstanceUID);

			var reader = DataAccessLayer.GetIDataStoreReader();
			var results = reader.Query(criteria);

			return new QueryOutput(
				CollectionUtils.Map(results,
					delegate(DicomAttributeCollection result)
					{
						var item = new StudyProperties();
						item.PatientId = result[DicomTags.PatientId].ToString();
						item.PatientsName = result[DicomTags.PatientsName].ToString();
						item.StudyDate = result[DicomTags.StudyDate].GetDateTime(0);
						item.StudyDescription = result[DicomTags.StudyDescription].ToString();
						item.ModalitiesInStudy = result[DicomTags.ModalitiesInStudy].ToString();
						item.AccessionNumber = result[DicomTags.AccessionNumber].ToString();
						item.StudyInstanceUID = result[DicomTags.StudyInstanceUid].ToString();
						return item;
					}));
		}

		[DataContract]
		public class OpenStudiesInput
		{
			[DataMember]
			public string[] StudyInstanceUids { get; set; }
		}

		public class OpenStudiesOutput
		{

		}


		[Description("Opens the specified studies in a new workspace.")]
		public OpenStudiesOutput OpenStudies(OpenStudiesInput input)
		{
			Platform.CheckForNullReference(input, "input");
			Platform.CheckMemberIsSet(input.StudyInstanceUids, "StudyInstanceUids");

			if(input.StudyInstanceUids.Length == 0)
				return new OpenStudiesOutput();

			var helper = new OpenStudyHelper();
			foreach (var studyInstanceUid in input.StudyInstanceUids)
				helper.AddStudy(studyInstanceUid, null, "DICOM_LOCAL");

			helper.Title = "imageviewer";
			helper.OpenStudies();

			return new OpenStudiesOutput();
		}
	}
}
