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

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
	[ServiceImplementsContract(typeof(IWorklistAdminService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class WorklistAdminService : ApplicationServiceBase, IWorklistAdminService
	{
		#region IWorklistAdminService Members

		[ReadOperation]
		public ListWorklistCategoriesResponse ListWorklistCategories(ListWorklistCategoriesRequest request)
		{
			var categories = CollectionUtils.Map<Type, string>(
				WorklistFactory.Instance.ListWorklistClasses(true),
				Worklist.GetCategory);

			// in case some worklist classes did not have assigned categories
			CollectionUtils.Remove(categories, (string s) => s == null);

			return new ListWorklistCategoriesResponse(CollectionUtils.Unique(categories));
		}

		[ReadOperation]
		public ListWorklistClassesResponse ListWorklistClasses(ListWorklistClassesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var worklistClasses =
				ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);

			var assembler = new WorklistAdminAssembler();
			return new ListWorklistClassesResponse(
				CollectionUtils.Map<Type, WorklistClassSummary>(worklistClasses,
					assembler.CreateClassSummary));
		}

		[ReadOperation]
		public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			var worklistClasses = ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);

			// grab the persistent worklists
			var broker = PersistenceContext.GetBroker<IWorklistBroker>();
			var persistentClassNames = CollectionUtils.Select(worklistClasses, t => !Worklist.GetIsStatic(t))
				.ConvertAll<string>(Worklist.GetClassName);

			var worklists = broker.Find(request.WorklistName, request.IncludeUserDefinedWorklists, persistentClassNames, request.Page);

			// optionally include the static ones
			if (request.IncludeStatic)
			{
				foreach (var worklistClass in worklistClasses)
				{
					if (Worklist.GetIsStatic(worklistClass))
						worklists.Add(WorklistFactory.Instance.CreateWorklist(worklistClass));
				}
			}

			var adminAssembler = new WorklistAdminAssembler();
			return new ListWorklistsResponse(
				CollectionUtils.Map<Worklist, WorklistAdminSummary, List<WorklistAdminSummary>>(
				worklists,
				worklist => adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListProcedureTypeGroupsResponse ListProcedureTypeGroups(ListProcedureTypeGroupsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureTypeGroupClass, "request.ProcedureTypeGroupClass");

			var procedureTypeGroupClass = ProcedureTypeGroup.GetSubClass(request.ProcedureTypeGroupClass, PersistenceContext);
			if (procedureTypeGroupClass == null)
				throw new ArgumentException("Invalid ProcedureTypeGroupClass name");

			var response = new ListProcedureTypeGroupsResponse();

			var assembler = new ProcedureTypeGroupAssembler();
			response.ProcedureTypeGroups =
				CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
					PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Find(new ProcedureTypeGroupSearchCriteria(), procedureTypeGroupClass),
					group => assembler.GetProcedureTypeGroupSummary(group, PersistenceContext));

			return response;
		}

		[ReadOperation]
		public GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request)
		{
			var response = new GetWorklistEditFormDataResponse();

			var assembler = new WorklistAdminAssembler();
			response.WorklistClasses = CollectionUtils.Map<Type, WorklistClassSummary>(
				WorklistFactory.Instance.ListWorklistClasses(false),
				assembler.CreateClassSummary);

			var staffAssembler = new StaffAssembler();
			response.StaffChoices = CollectionUtils.Map<Staff, StaffSummary>(
				this.PersistenceContext.GetBroker<IStaffBroker>().FindAll(false),
				item => staffAssembler.CreateStaffSummary(item, PersistenceContext));

			var staffGroupAssembler = new StaffGroupAssembler();
			response.GroupSubscriberChoices = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
				this.PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(false),
				staffGroupAssembler.CreateSummary);

			var facilityAssembler = new FacilityAssembler();
			response.FacilityChoices = CollectionUtils.Map<Facility, FacilitySummary>(
				this.PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
				facilityAssembler.CreateFacilitySummary);

			var locationAssembler = new LocationAssembler();
			response.PatientLocationChoices = CollectionUtils.Map<Location, LocationSummary>(
				this.PersistenceContext.GetBroker<ILocationBroker>().FindAll(false),
				locationAssembler.CreateLocationSummary);

			response.OrderPriorityChoices = EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext);
			response.PatientClassChoices = EnumUtils.GetEnumValueList<PatientClassEnum>(PersistenceContext);

			// add extra data iff editing a user-defined worklist (bug #4871)
			if (request.UserDefinedWorklist)
			{
				response.OwnerGroupChoices = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					this.CurrentUserStaff.ActiveGroups, // only current user's active staff groups should be choosable
					staffGroupAssembler.CreateSummary);
			}

			return response;
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request)
		{
			var worklist = PersistenceContext.Load<Worklist>(request.EntityRef);

			var adminAssembler = new WorklistAdminAssembler();
			var adminDetail = adminAssembler.GetWorklistDetail(worklist, this.PersistenceContext);
			return new LoadWorklistForEditResponse(worklist.GetRef(), adminDetail);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public AddWorklistResponse AddWorklist(AddWorklistRequest request)
		{
			if (string.IsNullOrEmpty(request.Detail.Name))
			{
				throw new RequestValidationException(SR.ExceptionWorklistNameRequired);
			}

			// create instance of worklist owner
			var owner = CreateOwner(request.Detail, request.IsUserWorklist);

			// ensure user has access to create this worklist
			CheckAccess(owner);

			CheckWorklistCountRestriction(owner);

			// create instance of appropriate class
			var worklist = WorklistFactory.Instance.CreateWorklist(request.Detail.WorklistClass.ClassName);

			// set owner
			worklist.Owner = owner;

			// update properties
			UpdateWorklistHelper(request.Detail, worklist);

			PersistenceContext.Lock(worklist, DirtyState.New);
			PersistenceContext.SynchState();

			var adminAssembler = new WorklistAdminAssembler();
			return new AddWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request)
		{
			var worklist = this.PersistenceContext.Load<Worklist>(request.EntityRef);

			// check if user can update
			CheckAccess(worklist.Owner);

			// update
			UpdateWorklistHelper(request.Detail, worklist);

			var adminAssembler = new WorklistAdminAssembler();
			return new UpdateWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Personal)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Worklist.Group)]
		public DeleteWorklistResponse DeleteWorklist(DeleteWorklistRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IWorklistBroker>();
				var item = broker.Load(request.WorklistRef, EntityLoadFlags.Proxy);

				// check if user can delete
				CheckAccess(item.Owner);

				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteWorklistResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Worklist))));
			}
		}

		#endregion

		private WorklistOwner CreateOwner(WorklistAdminDetail detail, bool userWorklist)
		{
			// if not creating a user worklist, the owner is Admin
			if (!userWorklist)
				return WorklistOwner.Admin;

			// if an owner group is specified, assign ownership to the group
			if (detail.IsGroupOwned)
			{
				var group = PersistenceContext.Load<StaffGroup>(detail.OwnerGroup.StaffGroupRef, EntityLoadFlags.Proxy);
				return new WorklistOwner(group);
			}

			// otherwise assign ownership to current user, regardless of whether a different owner staff specified
			return new WorklistOwner(CurrentUserStaff);
		}

		/// <summary>
		/// Checks whether the current user has access to worklists owned by the specified worklist owner.
		/// </summary>
		/// <param name="owner"></param>
		private void CheckAccess(WorklistOwner owner)
		{
			// admin can access any worklist
			if (UserHasToken(AuthorityTokens.Admin.Data.Worklist))
				return;

			// if worklist is staff-owned, and user has personal token, they have access
			if (owner.IsStaffOwner && owner.Staff.Equals(this.CurrentUserStaff)
				 && UserHasToken(AuthorityTokens.Workflow.Worklist.Personal))
				return;

			// if worklist is group-owned, user must have group token and be a member of the group
			if (owner.IsGroupOwner && owner.Group.Members.Contains(this.CurrentUserStaff)
				&& UserHasToken(AuthorityTokens.Workflow.Worklist.Group))
				return;

			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

		private void CheckWorklistCountRestriction(WorklistOwner owner)
		{
			var worklistCount = PersistenceContext.GetBroker<IWorklistBroker>().Count(owner);

			// admin can have unlimited worklists
			if (owner.IsAdminOwner)
				return;

			var settings = new WorklistSettings();
			if (owner.IsStaffOwner)
			{
				if (worklistCount >= settings.MaxPersonalWorklists)
					throw new RequestValidationException(SR.ExceptionMaximumWorklistsReachedForStaff);
			}
			else if (owner.IsGroupOwner)
			{
				if (worklistCount >= settings.MaxWorklistsPerStaffGroup)
					throw new RequestValidationException(SR.ExceptionMaximumWorklistsReachedForStaffGroup);
			}
		}

		private void UpdateWorklistHelper(WorklistAdminDetail detail, Worklist worklist)
		{
			var adminAssembler = new WorklistAdminAssembler();
			adminAssembler.UpdateWorklist(
				worklist,
				detail,
				worklist.Owner.IsAdminOwner,	// only update subscribers iff the worklist is admin owned
				this.PersistenceContext);
		}

		public static List<Type> ListClassesHelper(List<string> classNames, List<string> categories, bool includeStatic)
		{
			var worklistClasses = new List<Type>(WorklistFactory.Instance.ListWorklistClasses(true));

			// optionally filter classes by class name
			if (classNames != null && classNames.Count > 0)
			{
				worklistClasses = CollectionUtils.Select(worklistClasses, t => classNames.Contains(Worklist.GetClassName(t)));
			}

			// optionally filter classes by category
			if (categories != null && categories.Count > 0)
			{
				worklistClasses = CollectionUtils.Select(worklistClasses, t => categories.Contains(Worklist.GetCategory(t)));
			}

			// optionally exclude static
			if (!includeStatic)
			{
				worklistClasses = CollectionUtils.Select(worklistClasses, t => !Worklist.GetIsStatic(t));
			}

			return worklistClasses;
		}


	}
}
