#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services
{
	[DataContract]
	public class PublishFilesRequest
	{
		private IEnumerable<string> _fileExtensions;
		private IEnumerable<string> _filePaths;
		private bool _recursive;
		private bool _publishLocally;

		public PublishFilesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public bool PublishLocally
		{
			get { return _publishLocally; }
			set { _publishLocally = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> FilePaths
		{
			get { return _filePaths; }
			set { _filePaths = value; }
		}

		[DataMember(IsRequired = false)]
		public IEnumerable<string> FileExtensions
		{
			get { return _fileExtensions; }
			set { _fileExtensions = value; }
		}

		[DataMember(IsRequired = false)]
		public bool Recursive
		{
			get { return _recursive; }
			set { _recursive = value; }
		}
	}
	
	[DataContract]
	public class AEInformation
	{
		private string _hostName;
		private string _aeTitle;
		private int _port;

		public AEInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string HostName
		{
			get { return _hostName; }
			set { _hostName = value; }
		}

		[DataMember(IsRequired = true)]
		public string AETitle
		{
			get { return _aeTitle; }
			set { _aeTitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}
	}

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
