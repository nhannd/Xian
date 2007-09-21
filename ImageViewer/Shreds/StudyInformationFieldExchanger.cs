using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services;

namespace ClearCanvas.ImageViewer.Shreds
{
	public class StudyInformationFieldExchanger
	{
		[DicomField(DicomTags.StudyInstanceUid)]
		public string StudyInstanceUid;

		[DicomField(DicomTags.PatientId)]
		public string PatientId;

		[DicomField(DicomTags.PatientsName)]
		public string PatientsName;

		[DicomField(DicomTags.StudyDate)]
		public string StudyDate;

		[DicomField(DicomTags.StudyDescription)]
		public string StudyDescription;

		public static implicit operator StudyInformation(StudyInformationFieldExchanger info)
		{
			ClearCanvas.ImageViewer.Services.StudyInformation clone = new StudyInformation();
			clone.StudyInstanceUid = info.StudyInstanceUid;
			clone.PatientId = info.PatientId;
			clone.PatientsName = info.PatientsName;
			clone.StudyDate = DateParser.Parse(info.StudyDate ?? "");
			clone.StudyDescription = info.StudyDescription;
			return clone;
		}
	}
}
