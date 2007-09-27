using System;
using System.Collections.Generic;
using System.Text;
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.NoteAdmin)]
        public ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request)
        {
            NoteCategorySearchCriteria criteria = new NoteCategorySearchCriteria();
            SearchResultPage page = new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows);

            NoteCategoryAssembler assembler = new NoteCategoryAssembler();
            return new ListAllNoteCategoriesResponse(
                CollectionUtils.Map<NoteCategory, NoteCategorySummary, List<NoteCategorySummary>>(
                    PersistenceContext.GetBroker<INoteCategoryBroker>().Find(criteria, page),
                    delegate(NoteCategory category)
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.NoteAdmin)]
        public LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request)
        {
            // note that the version of the NoteCategoryRef is intentionally ignored here (default behaviour of ReadOperation)
            NoteCategory category = PersistenceContext.Load<NoteCategory>(request.NoteCategoryRef);
            NoteCategoryAssembler assembler = new NoteCategoryAssembler();

            return new LoadNoteCategoryForEditResponse(category.GetRef(), assembler.CreateNoteCategoryDetail(category, this.PersistenceContext));
        }

        /// <summary>
        /// Add the specified NoteCategory
        /// </summary>
        /// <param name="NoteCategory"></param>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.NoteAdmin)]
        public AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request)
        {
            NoteCategory noteCategory = new NoteCategory();

            NoteCategoryAssembler assembler = new NoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            PersistenceContext.Lock(noteCategory, DirtyState.New);

            // ensure the new NoteCategory is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }


        /// <summary>
        /// Update the specified NoteCategory
        /// </summary>
        /// <param name="NoteCategory"></param>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.NoteAdmin)]
        public UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request)
        {
            NoteCategory noteCategory = PersistenceContext.Load<NoteCategory>(request.NoteCategoryRef, EntityLoadFlags.CheckVersion);

            NoteCategoryAssembler assembler = new NoteCategoryAssembler();
            assembler.UpdateNoteCategory(request.NoteCategoryDetail, noteCategory);

            return new UpdateNoteCategoryResponse(assembler.CreateNoteCategorySummary(noteCategory, this.PersistenceContext));
        }

        #endregion

    }
}
