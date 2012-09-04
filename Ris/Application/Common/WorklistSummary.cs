#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class WorklistSummary : DataContractBase
    {
		public WorklistSummary()
		{

		}

        public WorklistSummary(EntityRef worklistRef, string displayName, string description,
			string className, string classCategoryName, string classDisplayName, StaffSummary ownerStaff, StaffGroupSummary ownerGroup)
        {
            WorklistRef = worklistRef;
            DisplayName = displayName;
            Description = description;
            ClassName = className;
			ClassCategoryName = classCategoryName;
			ClassDisplayName = classDisplayName;
			OwnerStaff = ownerStaff;
            OwnerGroup = ownerGroup;
        }

        public bool IsUserWorklist
        {
            get { return IsStaffOwned || IsGroupOwned; }
        }

        public bool IsStaffOwned
        {
            get { return OwnerStaff != null; }
        }

        public bool IsGroupOwned
        {
            get { return OwnerGroup != null; }
        }

        [DataMember]
        public EntityRef WorklistRef;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string Description;

		[DataMember]
        public string ClassName;

		[DataMember]
		public string ClassCategoryName;

		[DataMember]
		public string ClassDisplayName;

        [DataMember]
        public StaffSummary OwnerStaff;

        [DataMember]
        public StaffGroupSummary OwnerGroup;
    }
}
