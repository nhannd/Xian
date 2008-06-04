using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class ContactPointTextQueryRequest : DataContractBase
	{
		public ContactPointTextQueryRequest(
			EntityRef practitionerRef,
			TextQueryRequest textQueryRequest)
		{
			this.PractitionerRef = practitionerRef;
			this.TextQueryRequest = textQueryRequest;
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public TextQueryRequest TextQueryRequest;
	}
}
