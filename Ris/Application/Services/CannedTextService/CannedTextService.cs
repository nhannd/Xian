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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	[ServiceImplementsContract(typeof(ICannedTextService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class CannedTextService : ApplicationServiceBase, ICannedTextService
	{
		#region ICannedTextService Members

		[ReadOperation]
		public ListCannedTextResponse ListCannedText(ListCannedTextRequest request)
		{
			CannedTextAssembler assembler = new CannedTextAssembler();
			List<CannedTextSearchCriteria> criterias = new List<CannedTextSearchCriteria>();

			CannedTextSearchCriteria personalCannedTextCriteria = new CannedTextSearchCriteria();
			personalCannedTextCriteria.Staff.EqualTo(this.CurrentUserStaff);
			criterias.Add(personalCannedTextCriteria);

			if (this.CurrentUserStaff.Groups != null && this.CurrentUserStaff.Groups.Count > 0)
			{
				CannedTextSearchCriteria groupCannedTextCriteria = new CannedTextSearchCriteria();
				groupCannedTextCriteria.StaffGroup.In(this.CurrentUserStaff.Groups);
				criterias.Add(groupCannedTextCriteria);
			}

			IList<CannedText> results = PersistenceContext.GetBroker<ICannedTextBroker>().Find(criterias.ToArray(), request.Page);

			List<CannedTextSummary> staffCannedText = CollectionUtils.Map<CannedText, CannedTextSummary>(results,
				delegate(CannedText cannedText)
					{
						return assembler.GetCannedTextSummary(cannedText, this.PersistenceContext);
					});

			return new ListCannedTextResponse(staffCannedText);
		}

		[ReadOperation]
		public GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request)
		{
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new GetCannedTextEditFormDataResponse(
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					this.CurrentUserStaff.ActiveGroups,	// only active staff groups should be choosable
					delegate(StaffGroup group)
						{
							return groupAssembler.CreateSummary(group);
						}));
		}

		[ReadOperation]
		public LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request)
		{
			ICannedTextBroker broker = PersistenceContext.GetBroker<ICannedTextBroker>();
			CannedText cannedText;
			
			if (request.CannedTextRef != null)
			{
				cannedText = broker.Load(request.CannedTextRef);
			}
			else
			{
				CannedTextSearchCriteria criteria = new CannedTextSearchCriteria();

				if (!string.IsNullOrEmpty(request.Name))
					criteria.Name.EqualTo(request.Name);

				if (!string.IsNullOrEmpty(request.Category))
					criteria.Category.EqualTo(request.Category);

				if (!string.IsNullOrEmpty(request.StaffId))
					criteria.Staff.Id.EqualTo(request.StaffId);

				if (!string.IsNullOrEmpty(request.StaffGroupName))
					criteria.StaffGroup.Name.EqualTo(request.StaffGroupName);

				cannedText = broker.FindOne(criteria);
			}

			CannedTextAssembler assembler = new CannedTextAssembler();
			return new LoadCannedTextForEditResponse(assembler.GetCannedTextDetail(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public AddCannedTextResponse AddCannedText(AddCannedTextRequest request)
		{
			try
			{
				CheckCannedTextWriteAccess(request.Detail);

				if (string.IsNullOrEmpty(request.Detail.Name))
					throw new RequestValidationException(SR.ExceptionCannedTextNameRequired);

				if (string.IsNullOrEmpty(request.Detail.Category))
					throw new RequestValidationException(SR.ExceptionCannedTextCategoryRequired);

				CannedTextAssembler assembler = new CannedTextAssembler();
				CannedText cannedText = assembler.CreateCannedText(request.Detail, this.CurrentUserStaff, this.PersistenceContext);

				PersistenceContext.Lock(cannedText, DirtyState.New);
				PersistenceContext.SynchState();

				return new AddCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
			}
			catch (EntityValidationException)
			{
				string text = request.Detail.IsPersonal ?
					string.Format("staff {0}, {1}", this.CurrentUserStaff.Name.FamilyName, this.CurrentUserStaff.Name.GivenName) :
					string.Format("{0} group", request.Detail.StaffGroup.Name);

				throw new RequestValidationException(string.Format(SR.ExceptionIdenticalCannedTextExist, text));
			}
		}

		[UpdateOperation]
		public UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request)
		{
			CheckCannedTextWriteAccess(request.Detail);

			CannedText cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef);

			CannedTextAssembler assembler = new CannedTextAssembler();
			assembler.UpdateCannedText(cannedText, request.Detail, this.CurrentUserStaff, this.PersistenceContext);

			PersistenceContext.SynchState();
			return new UpdateCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request)
		{
			CannedText cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef, EntityLoadFlags.Proxy);
			CheckCannedTextWriteAccess(cannedText);

			PersistenceContext.GetBroker<ICannedTextBroker>().Delete(cannedText);

			return new DeleteCannedTextResponse();
		}

		#endregion

		private static void CheckCannedTextWriteAccess(CannedText cannedText)
		{
			CheckCannedTextWriteAccess(cannedText.StaffGroup == null);
		}

		private static void CheckCannedTextWriteAccess(CannedTextDetail cannedText)
		{
			CheckCannedTextWriteAccess(cannedText.IsPersonal);
		}

		private static void CheckCannedTextWriteAccess(bool isPersonal)
		{
			if (isPersonal && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.CannedText.Personal) == false || 
				!isPersonal && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.CannedText.Group) == false)
				throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

	}
}
