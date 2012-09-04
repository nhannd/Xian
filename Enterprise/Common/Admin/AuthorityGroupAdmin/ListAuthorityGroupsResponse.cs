#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
    public class ListAuthorityGroupsResponse : DataContractBase
    {
        public ListAuthorityGroupsResponse(List<AuthorityGroupSummary> authorityGroups)
        {
            AuthorityGroups = authorityGroups;
        }

        public ListAuthorityGroupsResponse(List<AuthorityGroupDetail> authorityGroups)
        {
            AuthorityGroupDetails = authorityGroups;
        }

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;

        [DataMember] 
        public List<AuthorityGroupDetail> AuthorityGroupDetails;
    }
}
