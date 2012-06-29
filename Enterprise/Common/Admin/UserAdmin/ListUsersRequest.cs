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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
	public class ListUsersRequest : DataContractBase
    {
        public ListUsersRequest()
        {
			this.Page = new SearchResultPage();
        }

		/// <summary>
		/// Filter by user name.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Filter by display name.
		/// </summary>
		[DataMember]
		public string DisplayName;

		/// <summary>
		/// Specify true to apply filters as exact match only.
		/// </summary>
		[DataMember]
		public bool ExactMatchOnly;

		[DataMember]
		public SearchResultPage Page;
	}
}
