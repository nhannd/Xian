#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeGroupAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IProcedureTypeGroupAdminService))]
	public class ProcedureTypeGroupAdminService : ApplicationServiceBase, IProcedureTypeGroupAdminService
	{
		#region IProcedureTypeGroupAdminService Members

		[ReadOperation]
		public GetProcedureTypeGroupEditFormDataResponse GetProcedureTypeGroupEditFormData(
			GetProcedureTypeGroupEditFormDataRequest request)
		{
			var response = new GetProcedureTypeGroupEditFormDataResponse();
			var ptgAssembler = new ProcedureTypeGroupAssembler();
			var subClasses = ProcedureTypeGroup.ListSubClasses(PersistenceContext);

			// Category choices
			response.Categories = CollectionUtils.Map(subClasses, (Type t) => ptgAssembler.GetCategoryEnumValueInfo(t));

			// ProcedureType choices
			var assembler = new ProcedureTypeAssembler();
			response.ProcedureTypes = CollectionUtils.Map(
				PersistenceContext.GetBroker<IProcedureTypeBroker>().FindAll(request.IncludeDeactivated),
				(ProcedureType rpt) => assembler.CreateSummary(rpt));

			return response;
		}

		[ReadOperation]
		public GetProcedureTypeGroupSummaryFormDataResponse GetProcedureTypeGroupSummaryFormData(GetProcedureTypeGroupSummaryFormDataRequest request)
		{
			var ptgAssembler = new ProcedureTypeGroupAssembler();
			var subClasses = ProcedureTypeGroup.ListSubClasses(PersistenceContext);

			// Category choices
			return new GetProcedureTypeGroupSummaryFormDataResponse(
				CollectionUtils.Map(subClasses, (Type t) => ptgAssembler.GetCategoryEnumValueInfo(t)));
		}

		[ReadOperation]
		public ListProcedureTypeGroupsResponse ListProcedureTypeGroups(
			ListProcedureTypeGroupsRequest request)
		{
			var response = new ListProcedureTypeGroupsResponse();
			var assembler = new ProcedureTypeGroupAssembler();

			var criteria = new ProcedureTypeGroupSearchCriteria();
			criteria.Name.SortAsc(0);
			var result = request.CategoryFilter == null ?
				PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Find(criteria, request.Page) :
				PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Find(criteria, Type.GetType(request.CategoryFilter.Code), request.Page);

			response.Items = CollectionUtils.Map(result, (ProcedureTypeGroup rptGroup) =>
												 assembler.GetProcedureTypeGroupSummary(rptGroup, this.PersistenceContext));

			return response;
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureTypeGroup)]
		public LoadProcedureTypeGroupForEditResponse LoadProcedureTypeGroupForEdit(
			LoadProcedureTypeGroupForEditRequest request)
		{
			var rptGroup = PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Load(request.EntityRef);
			var assembler = new ProcedureTypeGroupAssembler();
			var detail = assembler.GetProcedureTypeGroupDetail(rptGroup, this.PersistenceContext);
			return new LoadProcedureTypeGroupForEditResponse(rptGroup.GetRef(), detail);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureTypeGroup)]
		public AddProcedureTypeGroupResponse AddProcedureTypeGroup(
			AddProcedureTypeGroupRequest request)
		{
			if (string.IsNullOrEmpty(request.Detail.Name))
			{
				throw new RequestValidationException(SR.ExceptionProcedureTypeGroupNameRequired);
			}

			// create appropriate class of group
			var group = (ProcedureTypeGroup)Activator.CreateInstance(Type.GetType(request.Detail.Category.Code));
			var assembler = new ProcedureTypeGroupAssembler();
			assembler.UpdateProcedureTypeGroup(group, request.Detail, this.PersistenceContext);

			PersistenceContext.Lock(group, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddProcedureTypeGroupResponse(
				assembler.GetProcedureTypeGroupSummary(group, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureTypeGroup)]
		public UpdateProcedureTypeGroupResponse UpdateProcedureTypeGroup(
			UpdateProcedureTypeGroupRequest request)
		{
			var group = PersistenceContext.Load<ProcedureTypeGroup>(request.EntityRef, EntityLoadFlags.CheckVersion);
			var assembler = new ProcedureTypeGroupAssembler();
			assembler.UpdateProcedureTypeGroup(group, request.Detail, this.PersistenceContext);

			return new UpdateProcedureTypeGroupResponse(
				assembler.GetProcedureTypeGroupSummary(group, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureTypeGroup)]
		public DeleteProcedureTypeGroupResponse DeleteProcedureTypeGroup(DeleteProcedureTypeGroupRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IProcedureTypeGroupBroker>();
				var item = broker.Load(request.ProcedureTypeGroupRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteProcedureTypeGroupResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ProcedureTypeGroup))));
			}
		}

		#endregion
	}
}
