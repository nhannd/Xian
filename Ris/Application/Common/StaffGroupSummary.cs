using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffGroupSummary : DataContractBase, IEquatable<StaffGroupSummary>
    {
        public StaffGroupSummary(EntityRef groupRef, string name, string description, bool isElective, bool deactivated)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
        	this.IsElective = isElective;
        	this.Deactivated = deactivated;
        }

        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        public StaffGroupSummary()
        {
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
		public bool Deactivated;


    	public bool Equals(StaffGroupSummary staffGroupSummary)
    	{
    		if (staffGroupSummary == null) return false;
    		return Equals(StaffGroupRef, staffGroupSummary.StaffGroupRef);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as StaffGroupSummary);
    	}

    	public override int GetHashCode()
    	{
    		return StaffGroupRef != null ? StaffGroupRef.GetHashCode() : 0;
    	}
    }
}
