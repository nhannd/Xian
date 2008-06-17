using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class LoadCannedTextForEditRequest : DataContractBase
	{
		public LoadCannedTextForEditRequest(EntityRef cannedTextRef)
		{
			this.CannedTextRef = cannedTextRef;
		}

		public LoadCannedTextForEditRequest(
			string name,
			string category,
			string staffId,
			string staffGroupName)
		{
			this.Name = name;
			this.Category = category;
			this.StaffId = staffId;
			this.StaffGroupName = staffGroupName;
		}

		[DataMember]
		public EntityRef CannedTextRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public string StaffId;

		[DataMember]
		public string StaffGroupName;
	}
}
