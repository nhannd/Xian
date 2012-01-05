#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
	public class ListAuthorityGroupsRequest : DataContractBase
    {
		public ListAuthorityGroupsRequest()
		{
			Page = new SearchResultPage();
		}

		[DataMember]
		public SearchResultPage Page;

        [DataMember]
        public bool? DataGroup;

        [DataMember]
        public bool? Details;

	}
}
