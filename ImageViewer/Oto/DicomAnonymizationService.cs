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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.Oto;
using System.Runtime.Serialization;
using System.IO;

namespace ClearCanvas.ImageViewer.Oto
{
	[ExtensionOf(typeof (OtoServiceExtensionPoint))]
	public class DicomAnonymizationService : OtoServiceBase
	{
		[DataContract]
		public class AnonymizeStudyInput
		{
			[DataMember]
			[Required]
			[Description("Study Instance UID of the study to anonymize.")]
			public string StudyInstanceUID;

			[DataMember]
			[Description("Value to replace the Patient ID with.")]
			public string PatientId;

			[DataMember]
			[Description("Value to replace the Patients Name with.")]
			public string PatientsName;

			[DataMember]
			[Description("Value to replace the Patient Sex with.")]
			public string PatientsSex;

			[DataMember]
			[Description("Value to replace the Patient Birth Date with.")]
			public DateTime? PatientsBirthDate;

			[DataMember]
			[Description("Value to replace the Accession Number with.")]
			public string AccessionNumber;

			[DataMember]
			[Description("Value to replace the Study Description with.")]
			public string StudyDescription;

			[DataMember]
			[Description("Value to replace the Study Date with.")]
			public DateTime? StudyDate;

			[DataMember]
			[Required]
			[Description("Directory in which to place the anonymized DICOM files.")]
			public string OutputDirectory;
		}

		[DataContract]
		public class AnonymizeStudyOutput
		{
			public AnonymizeStudyOutput(int processedImageCount)
			{
				this.ProcessedImageCount = processedImageCount;
			}

			[DataMember]
			[Description("The number of images processed.")]
			public int ProcessedImageCount;
		}

		public AnonymizeStudyOutput AnonymizeStudy(AnonymizeStudyInput input)
		{
			// load study to anonymize
			IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader();
			IStudy study = reader.GetStudy(input.StudyInstanceUID);
			List<ISopInstance> sops = new List<ISopInstance>(study.GetSopInstances());

			// ensure there is a valid output location
			if(string.IsNullOrEmpty(input.OutputDirectory))
			{
				// create temp dir
			}

			string fullPath = Path.GetFullPath(input.OutputDirectory);
			if (!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}

			// set up anonymization data
			StudyData studyData = new StudyData();
			studyData.PatientId = input.PatientId;
			studyData.PatientsNameRaw = input.PatientsName;
			studyData.PatientsBirthDate = input.PatientsBirthDate;
			studyData.PatientsSex = input.PatientsSex;
			studyData.AccessionNumber = input.AccessionNumber;
			studyData.StudyDescription = input.StudyDescription;
			studyData.StudyDate = input.StudyDate;

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyData;

			//The default anonymizer removes the series data, so we just clone the original.
			anonymizer.AnonymizeSeriesDataDelegate =
				delegate(SeriesData original) { return original.Clone(); };

			// anonymize each image in the study
			for (int i = 0; i < sops.Count; ++i)
			{
				ISopInstance sop = sops[i];
				DicomFile file = new DicomFile(sop.GetLocationUri().LocalDiskPath);

				anonymizer.Anonymize(file);

				file.Save(string.Format("{0}\\{1}.dcm", fullPath, i));
			}

			return new AnonymizeStudyOutput(sops.Count);
		}
	}
}
