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
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

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
			if (!string.IsNullOrEmpty(request.FirstName))
				criteria.Name.GivenName.StartsWith(request.FirstName);
			if (!string.IsNullOrEmpty(request.LastName))
				criteria.Name.FamilyName.StartsWith(request.LastName);

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

			return new LoadStaffForEditResponse(s.GetRef(), assembler.CreateStaffDetail(s, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request)
		{
			StaffGroupAssembler groupAssember = new StaffGroupAssembler();


			return new LoadStaffEditorFormDataResponse(
				EnumUtils.GetEnumValueList<StaffTypeEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<SexEnum>(this.PersistenceContext),
				(new SimplifiedPhoneTypeAssembler()).GetPatientPhoneTypeChoices(),
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(),
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
			Staff staff = PersistenceContext.Load<Staff>(request.StaffRef, EntityLoadFlags.CheckVersion);

			StaffAssembler assembler = new StaffAssembler();
			assembler.UpdateStaff(request.StaffDetail, staff, Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.StaffGroup), PersistenceContext);

			return new UpdateStaffResponse(assembler.CreateStaffSummary(staff, PersistenceContext));
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
					delegate(StaffSearchCriteria[] criteria)
					{
						return broker.Count(criteria);
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
			if (staffTypesFilter != null)
			{
				// parse strings into StaffType 
				List<StaffType> typeFilters = CollectionUtils.Map<string, StaffType>(staffTypesFilter,
					   delegate(string t) { return (StaffType)Enum.Parse(typeof(StaffType), t); });

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
	}
}
