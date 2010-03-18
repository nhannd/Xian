using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeExternalPractitionerRequest : DataContractBase
	{
		[DataMember]
		public EntityRef DuplicatePractitionerRef;

		[DataMember]
		public ExternalPractitionerDetail MergedPractitioner;

		[DataMember]
		public Dictionary<EntityRef, EntityRef> ContactPointReplacements;
	}
}
