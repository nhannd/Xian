using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicatePractitionerRequest : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="original">The original record to keep.</param>
		/// <param name="duplicates">All the duplicate practitioners, including the original</param>
		public MergeDuplicatePractitionerRequest(
			ExternalPractitionerSummary original,
			List<ExternalPractitionerSummary> duplicates)
		{
			this.Original = original;
			this.Duplicates = duplicates;
		}

		[DataMember]
		public ExternalPractitionerSummary Original;

		[DataMember]
		public List<ExternalPractitionerSummary> Duplicates;
	}
}
