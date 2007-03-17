using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public enum RetrieveLevel
	{
		Study = 0,
		Series,
		Image
	}

	[DataContract]
	public class DicomSendRequest
	{
		private string _destinationHostName;
		private string _destinationAETitle;
		private int _port;
		private IEnumerable<string> _uids;

		public DicomSendRequest()
		{ }

		[DataMember(IsRequired = true)]
		public string DestinationHostName
		{
			get { return _destinationHostName; }
			set { _destinationHostName = value; }
		}

		[DataMember(IsRequired = true)]
		public string DestinationAETitle
		{
			get { return _destinationAETitle; }
			set { _destinationAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> Uids
		{
			get { return _uids; }
			set { _uids = value; }
		}
	}

	[DataContract]
	public class DicomRetrieveRequest
	{
		private string _sourceHostName;
		private string _sourceAETitle;
		private int _port;
		private IEnumerable<string> _uids;
		private RetrieveLevel _retrieveLevel;

		public DicomRetrieveRequest()
		{ }

		[DataMember(IsRequired = true)]
		public string SourceHostName
		{
			get { return _sourceHostName; }
			set { _sourceHostName = value; }
		}

		[DataMember(IsRequired = true)]
		public string SourceAETitle
		{
			get { return _sourceAETitle; }
			set { _sourceAETitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		[DataMember(IsRequired = true)]
		public IEnumerable<string> Uids
		{
			get { return _uids; }
			set { _uids = value; }
		}

		[DataMember(IsRequired = true)]
		public RetrieveLevel RetrieveLevel
		{
			get { return _retrieveLevel; }
			set { _retrieveLevel = value; }
		}
	}

	[DataContract]
	public class UpdateServerSettingRequest
	{
		private string _hostName;
		private string _AETitle;
		private int _port;
		private string _interimStorageDirectory;

		public UpdateServerSettingRequest(string hostName, string aeTitle, int port, string storageDir)
		{
			_hostName = hostName;
			_AETitle = aeTitle;
			_port = port;
			_interimStorageDirectory = storageDir;
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

	[DataContract]
	public class GetServerSettingResponse
	{
		private string _hostName;
		private string _AETitle;
		private int _port;
		private string _interimStorageDirectory;

		public GetServerSettingResponse(string hostName, string aeTitle, int port, string storageDir)
		{
			_hostName = hostName;
			_AETitle = aeTitle;
			_port = port;
			_interimStorageDirectory = storageDir;
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