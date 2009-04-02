using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class GetConfigurationDocumentResponse : DataContractBase
	{
		public GetConfigurationDocumentResponse(ConfigurationDocumentKey documentKey, string content)
		{
			DocumentKey = documentKey;
			Content = content;
		}

		[DataMember]
		public ConfigurationDocumentKey DocumentKey;

		[DataMember]
		public string Content;
	}
}
