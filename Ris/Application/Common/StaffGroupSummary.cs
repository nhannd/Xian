#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
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
