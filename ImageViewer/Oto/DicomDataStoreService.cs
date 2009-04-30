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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Oto;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Oto
{
	[ExtensionOf(typeof(OtoServiceExtensionPoint))]
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
			DicomAttributeCollection criteria = new DicomAttributeCollection();
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

			IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader();
			IEnumerable<DicomAttributeCollection> results = reader.Query(criteria);

			return new QueryOutput(
				CollectionUtils.Map<DicomAttributeCollection, StudyProperties>(results,
					delegate(DicomAttributeCollection result)
					{
						StudyProperties item = new StudyProperties();
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


	}
}
