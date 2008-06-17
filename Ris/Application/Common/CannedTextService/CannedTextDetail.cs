using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextDetail : DataContractBase
	{
		public CannedTextDetail()
		{
		}

		public CannedTextDetail(string name, string category, StaffSummary staff, StaffGroupSummary staffGroup, string text)
		{
			this.Name = name;
			this.Category = category;
			this.Staff = staff;
			this.StaffGroup = staffGroup;
			this.Text = text;
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffSummary Staff;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string Text;

		public bool IsPersonal
		{
			get { return this.Staff != null; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
