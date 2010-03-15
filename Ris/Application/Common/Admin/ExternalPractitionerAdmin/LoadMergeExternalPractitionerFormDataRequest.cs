using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeExternalPractitionerFormDataRequest : DataContractBase
	{
		public LoadMergeExternalPractitionerFormDataRequest(EntityRef practitionerRef)
		{
			this.PractitionerRef = practitionerRef;
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public bool IncludeDetail;

		[DataMember]
		public bool IncludeDuplicates;
	}
}
