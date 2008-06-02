using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class DeleteCannedTextRequest : DataContractBase
	{
		public DeleteCannedTextRequest(List<EntityRef> cannedTextRefs)
        {
			this.CannedTextRefs = cannedTextRefs;
        }

        [DataMember]
        public List<EntityRef> CannedTextRefs;
	}
}
