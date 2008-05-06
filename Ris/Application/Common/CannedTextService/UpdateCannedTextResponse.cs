using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class UpdateCannedTextResponse : DataContractBase
	{
		public UpdateCannedTextResponse(CannedTextSummary cannedTextSummary)
        {
			this.CannedTextSummary = cannedTextSummary;
        }

        [DataMember]
		public CannedTextSummary CannedTextSummary;
	}
}
