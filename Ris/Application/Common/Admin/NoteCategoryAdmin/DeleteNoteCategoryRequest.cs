using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
	[DataContract]
	public class DeleteNoteCategoryRequest : DataContractBase
	{
		public DeleteNoteCategoryRequest(EntityRef noteCategoryRef)
		{
			this.NoteCategoryRef = noteCategoryRef;
		}

		[DataMember]
		public EntityRef NoteCategoryRef;
	}
}
