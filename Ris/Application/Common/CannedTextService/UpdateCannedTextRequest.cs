using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class UpdateCannedTextRequest : DataContractBase
	{
		public UpdateCannedTextRequest(EntityRef cannedTextRef, CannedTextDetail detail)
        {
			this.CannedTextRef = cannedTextRef;
            this.Detail = detail;
        }

        [DataMember]
		public EntityRef CannedTextRef;

        [DataMember]
		public CannedTextDetail Detail;
	}
}
