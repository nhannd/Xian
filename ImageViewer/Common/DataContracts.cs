#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common
{
    // TODO (Marmot): Remove.
	[DataContract]
	public class StudyInformation
	{
		private string _studyInstanceUid;
		private string _patientId;
		private string _patientsName;
		private string _studyDescription;
		private DateTime? _studyDate;

		public StudyInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DataMember(IsRequired = true)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime? StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
		}

		public void CopyTo(StudyInformation studyInformation)
		{
			studyInformation.StudyInstanceUid = this.StudyInstanceUid;
			studyInformation.PatientId = this.PatientId;
			studyInformation.PatientsName = this.PatientsName;
			studyInformation.StudyDescription = this.StudyDescription;
			studyInformation.StudyDate = this.StudyDate;
		}

		public void CopyFrom(StudyInformation studyInformation)
		{
			this.StudyInstanceUid = studyInformation.StudyInstanceUid;
			this.PatientId = studyInformation.PatientId;
			this.PatientsName = studyInformation.PatientsName;
			this.StudyDescription = studyInformation.StudyDescription;
			this.StudyDate = studyInformation.StudyDate;
		}

		public StudyInformation Clone()
		{
			StudyInformation clone = new StudyInformation();
			CopyTo(clone);
			return clone;
		}
	}

	public enum InstanceLevel
	{
		Study = 0,
		Series,
		Sop
	}

    // TODO (Marmot): remove.
	[DataContract]
	public class InstanceInformation
	{
		private InstanceLevel _instanceLevel;
		private string _instanceUid;

		public InstanceInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public InstanceLevel InstanceLevel
		{
			get { return _instanceLevel; }
			set { _instanceLevel = value; }
		}

		[DataMember(IsRequired = true)]
		public string InstanceUid
		{
			get { return _instanceUid; }
			set { _instanceUid = value; }
		}

		public void CopyTo(InstanceInformation other)
		{
			other.InstanceLevel = this.InstanceLevel;
			other.InstanceUid = this.InstanceUid;
		}

		public InstanceInformation Clone()
		{
			InstanceInformation clone = new InstanceInformation();
			CopyTo(clone);
			return clone;
		}
	}
}
