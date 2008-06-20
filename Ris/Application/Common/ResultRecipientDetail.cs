using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ResultRecipientDetail : DataContractBase
	{
		public ResultRecipientDetail()
		{
		}

		public ResultRecipientDetail(ExternalPractitionerSummary practitioner, ExternalPractitionerContactPointDetail contactPoint, EnumValueInfo preferredCommunicationMode)
		{
			this.Practitioner = practitioner;
			this.ContactPoint = contactPoint;
			this.PreferredCommunicationMode = preferredCommunicationMode;
		}

		[DataMember]
		public ExternalPractitionerSummary Practitioner;

		[DataMember]
		public ExternalPractitionerContactPointDetail ContactPoint;

		[DataMember]
		public EnumValueInfo PreferredCommunicationMode;
	}
}