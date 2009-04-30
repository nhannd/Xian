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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin
{
    internal class AuthorityGroupAssembler
    {
        internal AuthorityGroupSummary CreateAuthorityGroupSummary(AuthorityGroup authorityGroup)
        {
            return new AuthorityGroupSummary(authorityGroup.GetRef(), authorityGroup.Name);
        }

        internal AuthorityGroupDetail CreateAuthorityGroupDetail(AuthorityGroup authorityGroup)
        {

            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
        	List<AuthorityTokenSummary> tokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary>(
        		authorityGroup.AuthorityTokens,
        		delegate(AuthorityToken token)
        		{
        			return assembler.GetAuthorityTokenSummary(token);
        		});

            return new AuthorityGroupDetail(
                authorityGroup.GetRef(),
                authorityGroup.Name,
                tokens);
        }

        internal void UpdateAuthorityGroup(AuthorityGroup authorityGroup, AuthorityGroupDetail detail, IPersistenceContext persistenceContext)
        {
            authorityGroup.Name = detail.Name;
			authorityGroup.AuthorityTokens.Clear();

			if (detail.AuthorityTokens.Count > 0)
			{
				// process authority tokens
				List<string> tokenNames = CollectionUtils.Map<AuthorityTokenSummary, string>(
					detail.AuthorityTokens,
					delegate(AuthorityTokenSummary token)
					{
						return token.Name;
					});

				AuthorityTokenSearchCriteria where = new AuthorityTokenSearchCriteria();
				where.Name.In(tokenNames);
				IList<AuthorityToken> authTokens = persistenceContext.GetBroker<IAuthorityTokenBroker>().Find(where);

				authorityGroup.AuthorityTokens.AddAll(authTokens);
			}
        }
    }
}
