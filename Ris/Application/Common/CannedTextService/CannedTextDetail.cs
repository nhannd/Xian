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

		public CannedTextDetail(string name, string path, string text, StaffSummary staff, StaffGroupSummary staffGroup)
        {
        	this.Name = name;
        	this.Path = path;
        	this.Text = text;
			this.Staff = staff;
			this.StaffGroup = staffGroup;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Path;

        [DataMember]
        public string Text;

		[DataMember]
		public StaffSummary Staff;

		[DataMember]
		public StaffGroupSummary StaffGroup;

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
