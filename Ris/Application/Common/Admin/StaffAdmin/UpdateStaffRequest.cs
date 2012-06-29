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

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class UpdateStaffRequest : DataContractBase
    {
        public UpdateStaffRequest(StaffDetail staffDetail)
        {
            this.StaffDetail = staffDetail;

			// update all staff groups by default (eg assume operating in "admin" mode)
        	this.UpdateNonElectiveGroups = true;
        	this.UpdateElectiveGroups = true;
        }

        [DataMember]
        public StaffDetail StaffDetail;

		/// <summary>
		/// Specifies whether to update non-elective group memberships.  Admin privileges are required.
		/// </summary>
		[DataMember]
		public bool UpdateNonElectiveGroups;

		/// <summary>
		/// Specifies whether to update elective group memberships.
		/// </summary>
		[DataMember]
		public bool UpdateElectiveGroups;
	}
}
