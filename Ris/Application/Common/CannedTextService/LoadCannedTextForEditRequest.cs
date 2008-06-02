using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class LoadCannedTextForEditRequest : DataContractBase
	{
		public LoadCannedTextForEditRequest(EntityRef cannedTextRef)
		{
			this.CannedTextRef = cannedTextRef;
		}

		[DataMember]
		public EntityRef CannedTextRef;
	}
}
