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
		/// <param name="duplicate">The duplicate practitioners to remove.</param>
		/// <param name="original">The original record to keep.</param>
		public MergeDuplicatePractitionerRequest(ExternalPractitionerSummary duplicate, ExternalPractitionerSummary original)
		{
			this.Duplicate = duplicate;
			this.Original = original;
		}

		[DataMember]
		public ExternalPractitionerSummary Duplicate;

		[DataMember]
		public ExternalPractitionerSummary Original;
	}
}
