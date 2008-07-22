using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffGroupDetail : DataContractBase
    {
        public StaffGroupDetail()
        {
            this.Members = new List<StaffSummary>();
        }

        public StaffGroupDetail(EntityRef groupRef, string name, string description, bool isElective, List<StaffSummary> members, bool deactivated)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
            this.Members = members;
        	this.IsElective = isElective;
        	this.Deactivated = deactivated;
        }

        [DataMember]
        public EntityRef StaffGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

		[DataMember]
		public bool IsElective;

		[DataMember]
        public List<StaffSummary> Members;

		[DataMember]
		public bool Deactivated;
    }
}