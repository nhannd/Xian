#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class LoadStaffGroupEditorFormDataResponse : DataContractBase
    {
		public LoadStaffGroupEditorFormDataResponse(List<StaffSummary> allStaff, List<WorklistSummary> allAdminWorklists)
        {
            this.AllStaff = allStaff;
        	this.AllAdminWorklists = allAdminWorklists;
        }

        [DataMember]
        public List<StaffSummary> AllStaff;

		[DataMember]
		public List<WorklistSummary> AllAdminWorklists;
    }
}
