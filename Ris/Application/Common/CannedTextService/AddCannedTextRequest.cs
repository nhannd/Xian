using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class AddCannedTextRequest : DataContractBase
	{
		public AddCannedTextRequest(CannedTextDetail detail)
        {
            this.Detail = detail;
        }

        [DataMember]
		public CannedTextDetail Detail;
	}
}
