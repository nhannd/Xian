#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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