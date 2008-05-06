using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [DataContract]
    public class CannedTextSummary : DataContractBase
    {
		public CannedTextSummary(EntityRef worklistRef
			, string name
			, string path
			, string text
			, List<StaffSummary> staffSubscribers
			, List<StaffGroupSummary> groupSubscribers)
        {
			this.CannedTextRef = worklistRef;
        	this.Name = name;
        	this.Path = path;
        	this.Text = text;
			this.StaffSubscribers = staffSubscribers;
			this.GroupSubscribers = groupSubscribers;
        }

        [DataMember]
        public EntityRef CannedTextRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Path;

        [DataMember]
		public string Text;

		[DataMember]
		public List<StaffSummary> StaffSubscribers;

		[DataMember]
		public List<StaffGroupSummary> GroupSubscribers;
	}
}
