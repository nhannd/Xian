#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Contains a list of fields that will be updated when processing duplicates. This is the list
	/// in the server partition configuration.
	/// </summary>
	public class StudyMatchingMap
	{
		#region Public Properties

		[DicomField(DicomTags.PatientsName)]
		public string PatientsName { get; set; }

		[DicomField(DicomTags.PatientId)]
		public string PatientId { get; set; }

		[DicomField(DicomTags.IssuerOfPatientId)]
		public string IssuerOfPatientId { get; set; }

		[DicomField(DicomTags.PatientsBirthDate)]
		public string PatientsBirthDate { get; set; }

		[DicomField(DicomTags.PatientsSex)]
		public string PatientsSex { get; set; }

		[DicomField(DicomTags.AccessionNumber)]
		public string AccessionNumber { get; set; }

		[DicomField(DicomTags.StudyInstanceUid)]
		public string StudyInstanceUid { get; set; }

		[DicomField(DicomTags.StudyId)]
		public string StudyId { get; set; }

		[DicomField(DicomTags.StudyDescription)]
		public string StudyDescription { get; set; }

		[DicomField(DicomTags.StudyDate)]
		public string StudyDate { get; set; }

		[DicomField(DicomTags.StudyTime)]
		public string StudyTime { get; set; }

		#endregion
	}
}