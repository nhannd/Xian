using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ConfigurationDocumentKey
	{
		private string _documentName;
		private Version _version;
		private string _user;
		private string _instanceKey;

		public ConfigurationDocumentKey(string documentName, Version version, string user, string instanceKey)
		{
			_documentName = documentName;
			_version = version;
			_user = user;
			_instanceKey = instanceKey;
		}

		[DataMember]
		public string DocumentName
		{
			get { return _documentName; }
			private set { _documentName = value; }
		}

		[DataMember]
		public Version Version
		{
			get { return _version; }
			private set { _version = value; }
		}

		[DataMember]
		public string User
		{
			get { return _user; }
			private set { _user = value; }
		}

		[DataMember]
		public string InstanceKey
		{
			get { return _instanceKey; }
			private set { _instanceKey = value; }
		}
	}
}
