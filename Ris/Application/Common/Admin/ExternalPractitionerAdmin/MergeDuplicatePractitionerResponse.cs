using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicatePractitionerResponse : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="updatedOriginal">The updated data for the original record.</param>
		public MergeDuplicatePractitionerResponse(ExternalPractitionerSummary updatedOriginal)
		{
			this.UpdatedOriginal = updatedOriginal;
		}

		[DataMember]
		public ExternalPractitionerSummary UpdatedOriginal;
	}
}
