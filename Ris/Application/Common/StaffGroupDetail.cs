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

        public StaffGroupDetail(EntityRef groupRef, string name, string description, List<StaffSummary> members)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
            this.Members = members;
        }

        [DataMember]
        public EntityRef StaffGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public List<StaffSummary> Members;
    }
}