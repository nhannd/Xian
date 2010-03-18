using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeExternalPractitionerResponse : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="updatedOriginal">The updated data for the original record.</param>
		public MergeExternalPractitionerResponse(ExternalPractitionerSummary updatedOriginal)
		{
			this.UpdatedOriginal = updatedOriginal;
		}

		[DataMember]
		public ExternalPractitionerSummary UpdatedOriginal;
	}
}
