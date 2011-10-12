#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [EnterpriseCoreService]
    [ServiceContract]
    [Authentication(false)]
    public interface IAuthorityGroupReadService
    {
        /// <summary>
        /// Summary list of all authority groups
        /// </summary>
        /// <param name="request"><see cref="ListAuthorityGroupsRequest"/></param>
        /// <returns><see cref="ListAuthorityGroupsResponse"/></returns>
        [OperationContract]
        ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request);
    }
}
