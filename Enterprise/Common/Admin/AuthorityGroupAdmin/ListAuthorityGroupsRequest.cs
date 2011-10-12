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

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
	public class ListAuthorityGroupsRequest : DataContractBase
    {
		public ListAuthorityGroupsRequest()
		{
			this.Page = new SearchResultPage();
		}

		[DataMember]
		public SearchResultPage Page;

        [DataMember] 
        public bool? DataGroup;
	}
}
