#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Authentication.Imex;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Authentication.Admin.AuthorityGroupAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IAuthorityGroupAdminService))]
	public class AuthorityGroupAdminService : CoreServiceLayer, IAuthorityGroupAdminService
	{
		#region IAuthorityGroupAdminService Members

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();
			criteria.Name.SortAsc(0);

            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> authorityGroups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary>(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, request.Page),
                delegate(AuthorityGroup authorityGroup)
                {
                    return assembler.CreateAuthorityGroupSummary(authorityGroup);
                });
            return new ListAuthorityGroupsResponse(authorityGroups);
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
        {
            AuthorityGroup authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            return new LoadAuthorityGroupForEditResponse(assembler.CreateAuthorityGroupDetail(authorityGroup));
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
        {
            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
            List<AuthorityTokenSummary> authorityTokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary>(
                PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindAll(),
                delegate(AuthorityToken authorityToken)
                {
                    return assembler.GetAuthorityTokenSummary(authorityToken);
                });

            return new ListAuthorityTokensResponse(authorityTokens);
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
        {
			// create new group
            AuthorityGroup authorityGroup = new AuthorityGroup();

			// set properties from request
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

			// save
            PersistenceContext.Lock(authorityGroup, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddAuthorityGroupResponse(assembler.CreateAuthorityGroupSummary(authorityGroup));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupDetail.AuthorityGroupRef);

			// set properties from request
			AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

			PersistenceContext.SynchState();

            return new UpdateAuthorityGroupResponse(assembler.CreateAuthorityGroupSummary(authorityGroup));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request)
		{
			IAuthorityGroupBroker broker = PersistenceContext.GetBroker<IAuthorityGroupBroker>();
            AuthorityGroup authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef, EntityLoadFlags.Proxy);

			// before we can delete an authority group, first need to remove all tokens and users
			authorityGroup.AuthorityTokens.Clear();
			authorityGroup.RemoveAllUsers();

			// delete group
			broker.Delete(authorityGroup);

			PersistenceContext.SynchState();

			return new DeleteAuthorityGroupResponse();
		}

    	[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Tokens, "Tokens");

			if(request.Tokens.Count > 0)
			{
				AuthorityTokenImporter importer = new AuthorityTokenImporter();
				importer.Import(
					CollectionUtils.Map<AuthorityTokenSummary, AuthorityTokenDefinition>(request.Tokens,
						delegate(AuthorityTokenSummary s) { return new AuthorityTokenDefinition(s.Name, s.Description); }),
						(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityTokensResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request)
    	{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroups, "AuthorityGroups");

			if (request.AuthorityGroups.Count > 0)
			{
				AuthorityGroupImporter importer = new AuthorityGroupImporter();
				importer.Import(
					CollectionUtils.Map<AuthorityGroupDetail, AuthorityGroupDefinition>(request.AuthorityGroups,
						delegate(AuthorityGroupDetail g)
						{
							return new AuthorityGroupDefinition(g.Name,
								CollectionUtils.Map<AuthorityTokenSummary, string>(g.AuthorityTokens,
								   delegate(AuthorityTokenSummary s) { return s.Name; }).ToArray());
						}),
						(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityGroupsResponse();
		}

    	#endregion
    }
}
