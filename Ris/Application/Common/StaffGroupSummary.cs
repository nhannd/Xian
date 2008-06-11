using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffGroupSummary : DataContractBase
    {
        public StaffGroupSummary(EntityRef groupRef, string name, string description, bool isElective)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
        	this.IsElective = isElective;
        }

        [DataMember]
        public EntityRef StaffGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

		[DataMember]
		public bool IsElective;
	}
}