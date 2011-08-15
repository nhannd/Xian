#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin
{
    internal class AuthorityGroupAssembler
    {
        internal AuthorityGroupSummary CreateAuthorityGroupSummary(AuthorityGroup authorityGroup)
        {
            return new AuthorityGroupSummary(authorityGroup.GetRef(), authorityGroup.Name, authorityGroup.Description,
                                             authorityGroup.DataGroup);
        }

        internal AuthorityGroupDetail CreateAuthorityGroupDetail(AuthorityGroup authorityGroup)
        {

            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
        	List<AuthorityTokenSummary> tokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary>(
        		authorityGroup.AuthorityTokens,
        		assembler.GetAuthorityTokenSummary);

            return new AuthorityGroupDetail(
                authorityGroup.GetRef(),
                authorityGroup.Name,
                authorityGroup.Description,
                authorityGroup.DataGroup,
                tokens);
        }

        internal void UpdateAuthorityGroup(AuthorityGroup authorityGroup, AuthorityGroupDetail detail, IPersistenceContext persistenceContext)
        {
            authorityGroup.Name = detail.Name;
            authorityGroup.Description = detail.Description;
            authorityGroup.DataGroup = detail.DataGroup;
			authorityGroup.AuthorityTokens.Clear();

			if (detail.AuthorityTokens.Count > 0)
			{
				// process authority tokens
				List<string> tokenNames = CollectionUtils.Map<AuthorityTokenSummary, string>(
					detail.AuthorityTokens,
					token => token.Name);

				AuthorityTokenSearchCriteria where = new AuthorityTokenSearchCriteria();
				where.Name.In(tokenNames);
				IList<AuthorityToken> authTokens = persistenceContext.GetBroker<IAuthorityTokenBroker>().Find(where);

				authorityGroup.AuthorityTokens.AddAll(authTokens);
			}
        }
    }
}
