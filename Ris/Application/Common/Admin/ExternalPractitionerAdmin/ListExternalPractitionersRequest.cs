#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[Serializable]
	public enum VerifiedState
	{
		All,
		Verified,
		NotVerified
	}

	[DataContract]
	public class ListExternalPractitionersRequest : ListRequestBase
	{
		public ListExternalPractitionersRequest()
			: this(null, null, new SearchResultPage())
		{
		}

		public ListExternalPractitionersRequest(string surname, string givenname)
			: this(surname, givenname, new SearchResultPage())
		{
		}

		public ListExternalPractitionersRequest(string surname, string givenname, SearchResultPage page)
			: this(surname, givenname, VerifiedState.All, null, null, true, false, false, false, false, page)
		{
			this.LastName = surname;
			this.FirstName = givenname;
			this.Page = page;
		}

		public ListExternalPractitionersRequest(
			string surname,
			string givenname,
			VerifiedState verifiedState,
			DateTime? lastVerifiedRangeFrom,
			DateTime? lastVerifiedRangeUntil,
			bool queryItems,
			bool queryCount,
			bool sortByLastVerifiedTime,
			bool sortByLastEditedTime,
			bool sortAscending,
			SearchResultPage page)
		{
			this.LastName = surname;
			this.FirstName = givenname;
			this.VerifiedState = verifiedState;
			this.LastVerifiedRangeFrom = lastVerifiedRangeFrom;
			this.LastVerifiedRangeUntil = lastVerifiedRangeUntil;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.SortByLastVerifiedTime = sortByLastVerifiedTime;
			this.SortByLastEditedTime = sortByLastEditedTime;
			this.SortAscending = sortAscending;
			this.Page = page;
		}

		[DataMember]
		public string FirstName;

		[DataMember]
		public string LastName;

		[DataMember]
		public VerifiedState VerifiedState;

		[DataMember]
		public bool QueryCount;

		[DataMember]
		public bool QueryItems;

		[DataMember]
		public bool SortByLastVerifiedTime;

		[DataMember]
		public bool SortByLastEditedTime;

		[DataMember]
		public bool SortAscending;

		[DataMember]
		public DateTime? LastVerifiedRangeFrom;

		[DataMember]
		public DateTime? LastVerifiedRangeUntil;
	}
}