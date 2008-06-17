using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextIdentifierDetail : DataContractBase
	{
		public CannedTextIdentifierDetail()
		{
		}

		public CannedTextIdentifierDetail(string name, string category, StaffSummary staff, StaffGroupSummary staffGroup)
		{
			this.Name = name;
			this.Category = category;
			this.Staff = staff;
			this.StaffGroup = staffGroup;
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffSummary Staff;

		[DataMember]
		public StaffGroupSummary StaffGroup;
	}
}
