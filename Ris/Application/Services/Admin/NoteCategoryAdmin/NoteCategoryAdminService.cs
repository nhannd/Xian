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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;
using System.Security.Permissions;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.NoteCategoryAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(INoteCategoryAdminService))]
    public class NoteCategoryAdminService : ApplicationServiceBase, INoteCategoryAdminService
    {
        #region INoteCategoryAdminService Members

        /// <summary>
        /// Return all NoteCategory options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request)
        {
            PatientNoteCategorySearchCriteria criteria = new PatientNoteCategorySearchCriteria();
			criteria.Name.SortAsc(0);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            return new ListAllNoteCategoriesResponse(
                CollectionUtils.Map<PatientNoteCategory, PatientNoteCategorySummary, List<PatientNoteCategorySummary>>(
                    PersistenceContext.GetBroker<IPatientNoteCategoryBroker>().Find(criteria, request.Page),
                    delegate(PatientNoteCategory category)
                    {
                        return assembler.CreateNoteCategorySummary(category, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetNoteCategoryEditFormDataResponse GetNoteCategoryEditFormData(GetNoteCategoryEditFormDataRequest request)
        {
            List<EnumValueInfo> severityChoices = EnumUtils.GetEnumValueList<NoteSeverityEnum>(PersistenceContext);
            return new GetNoteCategoryEditFormDataResponse(severityChoices);

        }

        [ReadOperation]
        public LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request)
        {
            // note that the version of the NoteCategoryRef is intentionally ignored here (default behaviour of ReadOperation)
            PatientNoteCategory category = PersistenceContext.Load<PatientNoteCategory>(request.NoteCategoryRef);
            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();

            return new LoadNoteCategoryForEditResponse(category.GetRef(), assembler.CreateNoteCategoryDetail(category, this.PersistenceContext));
        }

        /// <summary>
        /// Add the specified NoteCategory
        /// </summary>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
        public AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request)
        {
            PatientNoteCategory noteCategory = new PatientNoteCategory();

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            PersistenceContext.Lock(noteCategory, DirtyState.New);

            // ensure the new NoteCategory is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }


        /// <summary>
        /// Update the specified NoteCategory
        /// </summary>
        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
		public UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request)
        {
            PatientNoteCategory noteCategory = PersistenceContext.Load<PatientNoteCategory>(request.NoteCategoryDetail.NoteCategoryRef, EntityLoadFlags.CheckVersion);

            PatientNoteCategoryAssembler assembler = new PatientNoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            return new UpdateNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }

		/// <summary>
		/// Delete the specified NoteCategory
		/// </summary>
		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.PatientNoteCategory)]
		public DeleteNoteCategoryResponse DeleteNoteCategory(DeleteNoteCategoryRequest request)
		{
			try
			{
				IPatientNoteCategoryBroker broker = PersistenceContext.GetBroker<IPatientNoteCategoryBroker>();
				PatientNoteCategory item = broker.Load(request.NoteCategoryRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteNoteCategoryResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(PatientNoteCategory))));
			}
		}

		#endregion

    }
}
