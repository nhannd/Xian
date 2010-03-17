using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeExternalPractitionerFormDataRequest : DataContractBase
	{
		public LoadMergeExternalPractitionerFormDataRequest()
		{
			this.DeactivatedContactPointRefs = new List<EntityRef>();
		}

		/// <summary>
		/// Specifies the reference of an external practitioner.
		/// Request to return a list of duplicate external practitioners.
		/// </summary>
		[DataMember]
		public EntityRef PractitionerRef;

		/// <summary>
		/// A list of contact point references that will become deactivated.
		/// Request to return a list of orders affected by these contact points.
		/// </summary>
		[DataMember]
		public List<EntityRef> DeactivatedContactPointRefs;
	}
}
