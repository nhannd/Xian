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

		public CannedTextDetail(string name, string category, StaffGroupSummary staffGroup, string text)
		{
			this.Name = name;
			this.Category = category;
			this.StaffGroup = staffGroup;
			this.Text = text;
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string Text;

		public bool IsPersonal
		{
			get { return !this.IsGroup; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
