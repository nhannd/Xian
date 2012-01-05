#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin.AuthorityGroupAdmin
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IAuthorityGroupReadService))]
    public class AuthorityGroupReadService : CoreServiceLayer, IAuthorityGroupReadService
    {
        #region IAuthorityGroupAdminService Members

        [ReadOperation]
        public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            // Out of caution, requesting all of the authority tokens in the read service is 
            // treated as a security thread, and we're rejecting the request here.
            if (request.Details.HasValue && request.Details.Value)
                throw new UserAccessDeniedException(); 
        
            var criteria = new AuthorityGroupSearchCriteria();
            criteria.Name.SortAsc(0);
            if (request.DataGroup.HasValue)
                criteria.DataGroup.EqualTo(request.DataGroup.Value);

            var assembler = new AuthorityGroupAssembler();
            var authorityGroups = CollectionUtils.Map(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, request.Page),
                (AuthorityGroup authorityGroup) => assembler.CreateAuthorityGroupSummary(authorityGroup));
            return new ListAuthorityGroupsResponse(authorityGroups);
        }

        #endregion

    }
}
