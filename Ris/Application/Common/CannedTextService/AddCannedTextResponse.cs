using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class AddCannedTextResponse : DataContractBase
	{
		public AddCannedTextResponse(CannedTextSummary cannedTextSummary)
        {
			this.CannedTextSummary = cannedTextSummary;
        }

        [DataMember]
		public CannedTextSummary CannedTextSummary;
	}
}
