#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    // TODO (Marmot): remove this stuff or at least refactor it.

	public enum DeletionBehaviour
	{
		None = 0,
		DeleteOnSuccess,
		DeleteAlways
	}

	[DataContract]
	public class SendOperationReference
	{
		private Guid _identifier;
		private bool _isBackground;

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

		[DataMember(IsRequired = true)]
		public bool IsBackground
		{
			get { return _isBackground; }
			set { _isBackground = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is SendOperationReference)
				return ((SendOperationReference) obj).Identifier == Identifier;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}

		public static bool operator == (SendOperationReference ref1, SendOperationReference ref2)
		{
			return Object.Equals(ref1, ref2);
		}

		public static bool operator !=(SendOperationReference ref1, SendOperationReference ref2)
		{
			return !Object.Equals(ref1, ref2);
		}
	}

	[DataContract]
	public abstract class SendInstancesRequest
	{
		private AEInformation _destinationAEInformation;
		private bool _isBackground;

		public SendInstancesRequest()
		{
		}

		[DataMember(IsRequired = true)]
		public AEInformation DestinationAEInformation
		{
			get { return _destinationAEInformation; }
			set { _destinationAEInformation = value; }
		}

		[DataMember(IsRequired = true)]
		public bool IsBackground
		{
			get { return _isBackground; }
			set { _isBackground = value; }
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
}