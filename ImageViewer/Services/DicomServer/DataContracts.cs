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

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	public enum DeletionBehaviour
	{
		DeleteOnSuccess = 0,
		DeleteAlways
	}

	[DataContract]
	public class SendOperationReference
	{
		private Guid _identifier;

		public SendOperationReference(Guid identifier)
		{
			_identifier = identifier;
		}

		public SendOperationReference()
		{
		}

		[DataMember(IsRequired = true)]
		public Guid Identifier
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		public static implicit operator SendOperationReference(Guid identifier)
		{
			return new SendOperationReference(identifier);
		}

		//TODO: steps remaining, steps completed, failed, warning.
	}

	[DataContract]
	public abstract class SendInstancesRequest
	{
		private AEInformation _destinationAEInformation;

		public SendInstancesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public AEInformation DestinationAEInformation
		{
			get { return _destinationAEInformation; }
			set { _destinationAEInformation = value; }
		}
	}

	[DataContract]
	public class SendStudiesRequest : SendInstancesRequest
	{
		private IEnumerable<string> _studyInstanceUids;

		public SendStudiesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> StudyInstanceUids
		{
			get { return _studyInstanceUids; }
			set { _studyInstanceUids = value; }
		}
	}

	[DataContract]
	public class SendSeriesRequest : SendInstancesRequest
	{
		private string _studyInstanceUid;
		private IEnumerable<string> _seriesInstanceUids;

		public SendSeriesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> SeriesInstanceUids
		{
			get { return _seriesInstanceUids; }
			set { _seriesInstanceUids = value; }
		}
	}

	[DataContract]
	public class SendSopInstancesRequest : SendInstancesRequest
	{
		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private IEnumerable<string> _sopInstanceUids;

		public SendSopInstancesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> SopInstanceUids
		{
			get { return _sopInstanceUids; }
			set { _sopInstanceUids = value; }
		}
	}

	[DataContract]
	public class SendFilesRequest : SendInstancesRequest
	{
		private IEnumerable<string> _fileExtensions;
		private IEnumerable<string> _filePaths;
		private bool _recursive;
		private DeletionBehaviour _deletionBehaviour;

		public SendFilesRequest()
		{
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

		[DataMember(IsRequired = false)]
		public DeletionBehaviour DeletionBehaviour
		{
			get { return _deletionBehaviour; }
			set { _deletionBehaviour  = value; }
		}
	}

	[DataContract]
	public class DicomServerConfiguration
	{
		private string _hostName;
		private string _AETitle;
		private int _port;
		private string _interimStorageDirectory;

		public DicomServerConfiguration(string hostName, string aeTitle, int port, string interimStorageDirectory)
		{
			_hostName = hostName;
			_AETitle = aeTitle;
			_port = port;
			_interimStorageDirectory = interimStorageDirectory;
		}

		public DicomServerConfiguration()
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
			get { return _AETitle; }
			set { _AETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		[DataMember(IsRequired = true)]
		public string InterimStorageDirectory
		{
			get { return _interimStorageDirectory; }
			set { _interimStorageDirectory = value; }
		}
	}
}