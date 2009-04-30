#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
        {
            this.StaffID = staffID;
            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.StaffTypesFilter = staffTypesFilter;
            this.Page = page;
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
    }
}