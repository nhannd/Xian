using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class SetConfigurationDocumentRequest : ConfigurationDocumentRequestBase
	{
		public SetConfigurationDocumentRequest(ConfigurationDocumentKey documentKey, string content)
			:base(documentKey)
		{
			Content = content;
		}

		[DataMember]
		public string Content;
	}
}
