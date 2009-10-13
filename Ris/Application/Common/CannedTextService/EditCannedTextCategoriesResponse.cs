using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class EditCannedTextCategoriesResponse : DataContractBase
	{
		public EditCannedTextCategoriesResponse(List<CannedTextSummary> cannedTexts)
		{
			CannedTexts = cannedTexts;
		}

		[DataMember]
		public List<CannedTextSummary> CannedTexts;
	}
}