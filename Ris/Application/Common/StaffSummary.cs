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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffSummary : DataContractBase, ICloneable, IEquatable<StaffSummary>
    {
        public StaffSummary(EntityRef staffRef, string staffId, EnumValueInfo staffType, PersonNameDetail personNameDetail, bool deactivated)
        {
            this.StaffRef = staffRef;
            this.StaffId = staffId;
            this.StaffType = staffType;
            this.Name = personNameDetail;
        	this.Deactivated = deactivated;
        }

        public StaffSummary()
        {
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public EnumValueInfo StaffType;

        [DataMember]
        public string StaffId;

        [DataMember]
        public PersonNameDetail Name;

		[DataMember]
		public bool Deactivated;

		public override string ToString()
        {
            return this.Name.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
        	return new StaffSummary(this.StaffRef, this.StaffId, this.StaffType, this.Name, this.Deactivated);
        }

        #endregion

    	public bool Equals(StaffSummary staffSummary)
    	{
    		if (staffSummary == null) return false;
    		return Equals(StaffRef, staffSummary.StaffRef);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as StaffSummary);
    	}

    	public override int GetHashCode()
    	{
    		return StaffRef != null ? StaffRef.GetHashCode() : 0;
    	}
    }
}
