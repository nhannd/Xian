using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class DeletePractitionerRequest : DataContractBase
	{
		public DeletePractitionerRequest(ExternalPractitionerSummary practitioner)
		{
			this.Practitioner = practitioner;
		}

		[DataMember]
		public ExternalPractitionerSummary Practitioner;
	}
}
