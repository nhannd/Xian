using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ConfigurationDocumentRequestBase : DataContractBase
	{
		public ConfigurationDocumentRequestBase(ConfigurationDocumentKey documentKey)
		{
			DocumentKey = documentKey;
		}

		[DataMember]
		public ConfigurationDocumentKey DocumentKey;
	}
}