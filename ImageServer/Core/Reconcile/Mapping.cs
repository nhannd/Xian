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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Mapping for study information used for study reconciliation
	/// </summary>
	class StudyInfoMapping
	{
		#region Private Members
		private string _studyId;
		private string _studyInstanceUid;
		private string _studyDescription;
		private string _studyDate;
		private string _studyTime;
		private string _accessionNumber;
		#endregion

		#region Public Properties
		[DicomField(DicomTags.StudyId)]
		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value; }
		}

		[DicomField(DicomTags.StudyInstanceUid)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DicomField(DicomTags.StudyDescription)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DicomField(DicomTags.StudyDate)]
		public string StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
		}

		[DicomField(DicomTags.StudyTime)]
		public string StudyTime
		{
			get { return _studyTime; }
			set { _studyTime = value; }
		}

		[DicomField(DicomTags.AccessionNumber)]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}

		#endregion

	}

	/// <summary>
	/// Mapping for patient information used for study reconciliation
	/// </summary>
	class DemographicInfo
	{
		#region Private Members
		private string _patientsName;
		private string _patientId;
		private string _IssuerOfPatientId;
		private string _patientsBirthdate;
		private string _patientsSex;
		#endregion

		#region Public Properties
		[DicomField(DicomTags.PatientsName)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DicomField(DicomTags.PatientId)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DicomField(DicomTags.IssuerOfPatientId)]
		public string IssuerOfPatientId
		{
			get { return _IssuerOfPatientId; }
			set { _IssuerOfPatientId = value; }
		}

		[DicomField(DicomTags.PatientsBirthDate)]
		public string PatientsBirthDate
		{
			get { return _patientsBirthdate; }
			set { _patientsBirthdate = value; }
		}

		[DicomField(DicomTags.PatientsSex)]
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value; }
		}

		#endregion

	}
}