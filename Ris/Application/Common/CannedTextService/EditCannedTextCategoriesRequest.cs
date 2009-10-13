using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class EditCannedTextCategoriesRequest : DataContractBase
	{
		public EditCannedTextCategoriesRequest(List<EntityRef> cannedTextRefs, string category)
		{
			this.CannedTextRefs = cannedTextRefs;
			this.Category = category;
		}

		[DataMember]
		public List<EntityRef> CannedTextRefs;

		[DataMember]
		public string Category;
	}
}