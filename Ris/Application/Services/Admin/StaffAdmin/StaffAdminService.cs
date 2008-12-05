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

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IStaffAdminService))]
	public class StaffAdminService : ApplicationServiceBase, IStaffAdminService
	{
		#region IStaffAdminService Members

		[ReadOperation]
		// note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin
		// because it is used in non-admin situations - perhaps we need to create a separate operation???
		public ListStaffResponse ListStaff(ListStaffRequest request)
		{

			StaffAssembler assembler = new StaffAssembler();

			StaffSearchCriteria criteria = new StaffSearchCriteria();
			criteria.Name.FamilyName.SortAsc(0);
			if (!string.IsNullOrEmpty(request.FirstName))
				criteria.Name.GivenName.StartsWith(request.FirstName);
			if (!string.IsNullOrEmpty(request.LastName))
				criteria.Name.FamilyName.StartsWith(request.LastName);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			ApplyStaffTypesFilter(request.StaffTypesFilter, new StaffSearchCriteria[] { criteria });

			return new ListStaffResponse(
				CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
					PersistenceContext.GetBroker<IStaffBroker>().Find(criteria, request.Page),
					delegate(Staff s)
					{
						return assembler.CreateStaffSummary(s, PersistenceContext);
					}));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request)
		{
			// note that the version of the StaffRef is intentionally ignored here (default behaviour of ReadOperation)
			Staff s = PersistenceContext.Load<Staff>(request.StaffRef);
			StaffAssembler assembler = new StaffAssembler();

			return new LoadStaffForEditResponse(assembler.CreateStaffDetail(s, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request)
		{
			StaffGroupAssembler groupAssember = new StaffGroupAssembler();

			return new LoadStaffEditorFormDataResponse(
				EnumUtils.GetEnumValueList<StaffTypeEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<SexEnum>(this.PersistenceContext),
				(new SimplifiedPhoneTypeAssembler()).GetPatientPhoneTypeChoices(),
				EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(false),
					delegate(StaffGroup group) { return groupAssember.CreateSummary(group); })
				);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public AddStaffResponse AddStaff(AddStaffRequest request)
		{
			Staff staff = new Staff();

			StaffAssembler assembler = new StaffAssembler();
			assembler.UpdateStaff(request.StaffDetail, staff, Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup), PersistenceContext);

			PersistenceContext.Lock(staff, DirtyState.New);

			// ensure the new staff is assigned an OID before using it in the return value
			PersistenceContext.SynchState();

			return new AddStaffResponse(assembler.CreateStaffSummary(staff, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public UpdateStaffResponse UpdateStaff(UpdateStaffRequest request)
		{
			Staff staff = PersistenceContext.Load<Staff>(request.StaffDetail.StaffRef, EntityLoadFlags.CheckVersion);

			StaffAssembler assembler = new StaffAssembler();
			assembler.UpdateStaff(request.StaffDetail, staff, Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup), PersistenceContext);

			return new UpdateStaffResponse(assembler.CreateStaffSummary(staff, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Staff)]
		public DeleteStaffResponse DeleteStaff(DeleteStaffRequest request)
		{
			try
			{
				IStaffBroker broker = PersistenceContext.GetBroker<IStaffBroker>();
				Staff item = broker.Load(request.StaffRef, EntityLoadFlags.Proxy);

				User affectedUser = FindUserByName(item.UserName);
				if (affectedUser != null)
					affectedUser.DisplayName = null;

				//bug #3324: because StaffGroup owns the collection, need to iterate over each group
				//and manually remove this staff
				List<StaffGroup> groups = new List<StaffGroup>(item.Groups);
				foreach (StaffGroup group in groups)
				{
					group.RemoveMember(item);
				}

				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteStaffResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Staff))));
			}
		}

		[ReadOperation]
		public TextQueryResponse<StaffSummary> TextQuery(StaffTextQueryRequest request)
		{
			IStaffBroker broker = PersistenceContext.GetBroker<IStaffBroker>();
			StaffAssembler assembler = new StaffAssembler();

			TextQueryHelper<Staff, StaffSearchCriteria, StaffSummary> helper
				= new TextQueryHelper<Staff, StaffSearchCriteria, StaffSummary>(
					delegate(string rawQuery)
					{
						// this will hold all criteria
						List<StaffSearchCriteria> criteria = new List<StaffSearchCriteria>();

						// build criteria against names
						PersonName[] names = TextQueryHelper.ParsePersonNames(rawQuery);
						criteria.AddRange(CollectionUtils.Map<PersonName, StaffSearchCriteria>(names,
							delegate(PersonName n)
							{
								StaffSearchCriteria sc = new StaffSearchCriteria();
								sc.Name.FamilyName.StartsWith(n.FamilyName);
								if (n.GivenName != null)
									sc.Name.GivenName.StartsWith(n.GivenName);
								return sc;
							}));

						// build criteria against identifiers
						string[] ids = TextQueryHelper.ParseIdentifiers(rawQuery);
						criteria.AddRange(CollectionUtils.Map<string, StaffSearchCriteria>(ids,
									 delegate(string word)
									 {
										 StaffSearchCriteria c = new StaffSearchCriteria();
										 c.Id.StartsWith(word);
										 return c;
									 }));


						ApplyStaffTypesFilter(request.StaffTypesFilter, criteria);

						return criteria.ToArray();
					},
					delegate(Staff staff)
					{
						return assembler.CreateStaffSummary(staff, PersistenceContext);
					},
					delegate(StaffSearchCriteria[] criteria, int threshold)
					{
						return broker.Count(criteria) <= threshold;
					},
					delegate(StaffSearchCriteria[] criteria, SearchResultPage page)
					{
						return broker.Find(criteria, page);
					});

			return helper.Query(request);
		}

		#endregion

		/// <summary>
		/// Applies the specified staff types filter to the specified set of criteria objects.
		/// </summary>
		/// <param name="staffTypesFilter"></param>
		/// <param name="criteria"></param>
		private void ApplyStaffTypesFilter(IEnumerable<string> staffTypesFilter, IEnumerable<StaffSearchCriteria> criteria)
		{
			IEnumBroker broker = PersistenceContext.GetBroker<IEnumBroker>();
			if (staffTypesFilter != null)
			{
				// parse strings into StaffType 
				List<StaffTypeEnum> typeFilters = CollectionUtils.Map<string, StaffTypeEnum>(staffTypesFilter,
					   delegate(string t) { return broker.Find<StaffTypeEnum>(t); });

				if (typeFilters.Count > 0)
				{
					// apply type filter to each criteria object
					foreach (StaffSearchCriteria criterion in criteria)
					{
						criterion.Type.In(typeFilters);
					}
				}
			}
		}

		private User FindUserByName(string name)
		{
			if (String.IsNullOrEmpty(name))
				return null;

			try
			{
				UserSearchCriteria where = new UserSearchCriteria();
				where.UserName.EqualTo(name);

				return PersistenceContext.GetBroker<IUserBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				return null;
			}
		}
	}
}
