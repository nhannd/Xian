#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Authentication.Imex;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

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
			var criteria = new AuthorityGroupSearchCriteria();
			criteria.Name.SortAsc(0);

			var assembler = new AuthorityGroupAssembler();
			var authorityGroups = CollectionUtils.Map(
				PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, request.Page),
				(AuthorityGroup authorityGroup) => assembler.CreateAuthorityGroupSummary(authorityGroup));
			return new ListAuthorityGroupsResponse(authorityGroups);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
		{
			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef);
			var assembler = new AuthorityGroupAssembler();
			return new LoadAuthorityGroupForEditResponse(assembler.CreateAuthorityGroupDetail(authorityGroup));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
		{
			var assembler = new AuthorityTokenAssembler();
			var authorityTokens = CollectionUtils.Map(
				PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindAll(),
				(AuthorityToken authorityToken) => assembler.GetAuthorityTokenSummary(authorityToken));

			return new ListAuthorityTokensResponse(authorityTokens);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
		{
			// create new group
			var authorityGroup = new AuthorityGroup();

			// set properties from request
			var assembler = new AuthorityGroupAssembler();
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
			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupDetail.AuthorityGroupRef);

			// set properties from request
			var assembler = new AuthorityGroupAssembler();
			assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateAuthorityGroupResponse(assembler.CreateAuthorityGroupSummary(authorityGroup));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request)
		{
			var broker = PersistenceContext.GetBroker<IAuthorityGroupBroker>();
			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef, EntityLoadFlags.Proxy);

			if (request.DeleteOnlyWhenEmpty)
			{
				var count = broker.GetUserCountForGroup(authorityGroup);
				if (count > 0)
					throw new AuthorityGroupIsNotEmptyException(authorityGroup.Name, count);
			}

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

			if (request.Tokens.Count > 0)
			{
				var importer = new AuthorityTokenImporter();
				importer.Import(
					CollectionUtils.Map(request.Tokens, (AuthorityTokenSummary s) => new AuthorityTokenDefinition(s.Name, s.Description)),
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
				var importer = new AuthorityGroupImporter();
				importer.Import(
					CollectionUtils.Map(request.AuthorityGroups,
										(AuthorityGroupDetail g) =>
											new AuthorityGroupDefinition(g.Name,
												CollectionUtils.Map(g.AuthorityTokens, (AuthorityTokenSummary s) => s.Name).ToArray())),
					(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityGroupsResponse();
		}

		#endregion
	}
}
