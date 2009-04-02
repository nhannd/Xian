using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class GetConfigurationDocumentRequest : ConfigurationDocumentRequestBase
	{
		public GetConfigurationDocumentRequest(ConfigurationDocumentKey documentKey)
			:base(documentKey)
		{
		}
	}
}
