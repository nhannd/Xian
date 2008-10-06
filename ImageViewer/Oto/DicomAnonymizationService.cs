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
