using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointResponse : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="updatedOriginal">The updated data for the original record.</param>
		public MergeDuplicateContactPointResponse(ExternalPractitionerContactPointSummary updatedOriginal)
		{
			this.UpdatedOriginal = updatedOriginal;
		}

		[DataMember]
		public ExternalPractitionerContactPointSummary UpdatedOriginal;
	}
}
