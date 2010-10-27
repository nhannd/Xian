#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod
{
	//internal for now b/c the patient root query stuff is
	internal interface IPatientRootData : IPatientData
	{
		[DicomField(DicomTags.NumberOfPatientRelatedStudies)]
		int? NumberOfPatientRelatedStudies { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedSeries)]
		int? NumberOfPatientRelatedSeries { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedInstances)]
		int? NumberOfPatientRelatedInstances { get; }
	}

	public interface IPatientData
	{
		[DicomField(DicomTags.PatientId)]
		string PatientId { get; }

		[DicomField(DicomTags.PatientsName)]
		string PatientsName { get; }

		[DicomField(DicomTags.PatientsBirthDate)]
		string PatientsBirthDate { get; }

		[DicomField(DicomTags.PatientsBirthTime)]
		string PatientsBirthTime { get; }

		[DicomField(DicomTags.PatientsSex)]
		string PatientsSex { get; }
	}
}