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
using System.Text;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IProcedureTypeAdminService))]
	public class ProcedureTypeAdminService : ApplicationServiceBase, IProcedureTypeAdminService
	{
		#region IProcedureTypeAdminService Members

		[ReadOperation]
		public TextQueryResponse<ProcedureTypeSummary> TextQuery(TextQueryRequest request)
		{
			IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();

			TextQueryHelper<ProcedureType, ProcedureTypeSearchCriteria, ProcedureTypeSummary> helper
				= new TextQueryHelper<ProcedureType, ProcedureTypeSearchCriteria, ProcedureTypeSummary>(
					delegate
					{
						string rawQuery = request.TextQuery;

						IList<string> terms = TextQueryHelper.ParseTerms(rawQuery);
						List<ProcedureTypeSearchCriteria> criteria = new List<ProcedureTypeSearchCriteria>();

						// allow matching on name (assume entire query is a name which may contain spaces)
						ProcedureTypeSearchCriteria nameCriteria = new ProcedureTypeSearchCriteria();
						nameCriteria.Name.StartsWith(rawQuery);
						criteria.Add(nameCriteria);

						// allow matching of any term against ID
						criteria.AddRange(CollectionUtils.Map<string, ProcedureTypeSearchCriteria>(terms,
									 delegate(string term)
									 {
										 ProcedureTypeSearchCriteria c = new ProcedureTypeSearchCriteria();
										 c.Id.StartsWith(term);
										 return c;
									 }));

						return criteria.ToArray();
					},
					delegate(ProcedureType pt)
					{
						return assembler.CreateSummary(pt);
					},
					delegate(ProcedureTypeSearchCriteria[] criteria, int threshold)
					{
						return broker.Count(criteria) <= threshold;
					},
					delegate(ProcedureTypeSearchCriteria[] criteria, SearchResultPage page)
					{
						return broker.Find(criteria, page);
					});

			return helper.Query(request);
		}

		[ReadOperation]
		public ListProcedureTypesResponse ListProcedureTypes(ListProcedureTypesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			if (!string.IsNullOrEmpty(request.Id))
				where.Id.StartsWith(request.Id);
			if (!string.IsNullOrEmpty(request.Name))
				where.Name.Like(string.Format("%{0}%", request.Name));
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

			IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
			IList<ProcedureType> items = broker.Find(where, request.Page);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new ListProcedureTypesResponse(
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(items,
					delegate(ProcedureType item)
					{
						return assembler.CreateSummary(item);
					})
				);
		}

		[ReadOperation]
		public LoadProcedureTypeForEditResponse LoadProcedureTypeForEdit(LoadProcedureTypeForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureTypeRef, "request.ProcedureTypeRef");

			ProcedureType item = PersistenceContext.Load<ProcedureType>(request.ProcedureTypeRef);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new LoadProcedureTypeForEditResponse(assembler.CreateDetail(item));
		}

		[ReadOperation]
		public LoadProcedureTypeEditorFormDataResponse LoadProcedureTypeEditorFormData(LoadProcedureTypeEditorFormDataRequest request)
		{
			ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			where.Deactivated.EqualTo(false);

			IList<ProcedureType> procTypes = PersistenceContext.GetBroker<IProcedureTypeBroker>().Find(where);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new LoadProcedureTypeEditorFormDataResponse(
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(procTypes,
					delegate(ProcedureType pt) { return assembler.CreateSummary(pt); }));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public AddProcedureTypeResponse AddProcedureType(AddProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");

			if (string.IsNullOrEmpty(request.ProcedureType.PlanXml))
				throw new RequestValidationException(SR.ExceptionProcedurePlanXmlRequired);

			ProcedureType item = new ProcedureType();

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.Lock(item, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public UpdateProcedureTypeResponse UpdateProcedureType(UpdateProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");
			Platform.CheckMemberIsSet(request.ProcedureType.ProcedureTypeRef, "request.ProcedureType.ProcedureTypeRef");

			if (string.IsNullOrEmpty(request.ProcedureType.PlanXml))
				throw new RequestValidationException(SR.ExceptionProcedurePlanXmlRequired);

			ProcedureType item = PersistenceContext.Load<ProcedureType>(request.ProcedureType.ProcedureTypeRef);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public DeleteProcedureTypeResponse DeleteProcedureType(DeleteProcedureTypeRequest request)
		{
			try
			{
				IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
				ProcedureType item = broker.Load(request.ProcedureTypeRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteProcedureTypeResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ProcedureType))));
			}
		}

		#endregion
	}
}
