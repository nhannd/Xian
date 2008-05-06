using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class ListCannedTextResponse : DataContractBase
	{
		public ListCannedTextResponse(List<CannedTextSummary> cannedTexts)
		{
			this.CannedTexts = cannedTexts;
		}

		[DataMember]
		public List<CannedTextSummary> CannedTexts;
	}
}
