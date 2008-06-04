using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class MergeDuplicateContactPointRequest : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="duplicate">The duplicate contact point to remove.</param>
		/// <param name="original">The original contact point to keep.</param>
		public MergeDuplicateContactPointRequest(
			ExternalPractitionerContactPointSummary duplicate, 
			ExternalPractitionerContactPointSummary original)
		{
			this.Duplicate = duplicate;
			this.Original = original;
		}

		[DataMember]
		public ExternalPractitionerContactPointSummary Duplicate;

		[DataMember]
		public ExternalPractitionerContactPointSummary Original;
	}
}
