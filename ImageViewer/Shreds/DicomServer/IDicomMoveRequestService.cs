using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
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

	[ServiceContract]
	public interface IDicomMoveRequestService
	{
		[OperationContract]
		void Send(DicomSendRequest request);

		[OperationContract]
		void Retrieve(DicomRetrieveRequest request);
	}
}
