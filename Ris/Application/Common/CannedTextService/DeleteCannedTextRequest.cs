using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class DeleteCannedTextRequest : DataContractBase
	{
		public DeleteCannedTextRequest(EntityRef cannedTextRef)
        {
			this.CannedTextRef = cannedTextRef;
        }

        [DataMember]
		public EntityRef CannedTextRef;
	}
}
