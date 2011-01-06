#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
                        request.AddToGroups,
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
