using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
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