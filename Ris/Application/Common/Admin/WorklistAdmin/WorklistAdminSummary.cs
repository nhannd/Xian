#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
	public class WorklistAdminSummary : WorklistSummary
    {
        /// <summary>
        /// No-args constructor needed by Oto scripts.
        /// </summary>
        public WorklistAdminSummary()
        {
        }

        public WorklistAdminSummary(
            EntityRef worklistRef,
            string name,
            string description,
            WorklistClassSummary worklistClass,
            StaffSummary ownerStaff,
            StaffGroupSummary ownerGroup)
			: base(worklistRef, name, description, worklistClass.ClassName, worklistClass.CategoryName, worklistClass.DisplayName, ownerStaff, ownerGroup)
        {
            WorklistClass = worklistClass;
        }


        [DataMember]
        public WorklistClassSummary WorklistClass;
    }
}