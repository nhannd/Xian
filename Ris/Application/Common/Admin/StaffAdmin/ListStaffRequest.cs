#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
	public class ListStaffRequest : ListRequestBase
    {
        public ListStaffRequest()
        {
        }

        public ListStaffRequest(string staffID, string familyName, string givenName, string[] staffTypesFilter)
            : this(staffID, familyName, givenName, staffTypesFilter, null)
        {
        }

		public ListStaffRequest(string staffID, string familyName, string givenName, string[] staffTypesFilter, SearchResultPage page)
			: this(staffID, familyName, givenName, staffTypesFilter, page, false)
		{
			
		}

        public ListStaffRequest(string staffID, string familyName, string givenName, string[] staffTypesFilter, SearchResultPage page, bool exactMatch)
        {
            this.StaffID = staffID;
            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.StaffTypesFilter = staffTypesFilter;
            this.Page = page;
			this.ExactMatch = exactMatch;
        }

		/// <summary>
		/// Filter by staff ID.
		/// </summary>
        [DataMember]
        public string StaffID;

		/// <summary>
		/// Filter by given name, or partial given name.
		/// </summary>
        [DataMember]
        public string GivenName;

		/// <summary>
		/// Filter by family name, or partial family name.
		/// </summary>
		[DataMember]
        public string FamilyName;

		/// <summary>
		/// Filter by name of associated user account. Exact match only.  Typically not used in conjunction with any other filters.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Filter by staff type.
		/// </summary>
        [DataMember]
        public string[] StaffTypesFilter;

		/// <summary>
		/// Perform exact match using the filters, except for UserName, which is always used exactly.
		/// </summary>
		[DataMember]
		public bool ExactMatch;
	}
}