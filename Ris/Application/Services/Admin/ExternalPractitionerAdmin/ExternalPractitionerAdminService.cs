using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IExternalPractitionerAdminService))]
    public class ExternalPractitionerAdminService : ApplicationServiceBase, IExternalPractitionerAdminService
    {
        #region IExternalPractitionerAdminService Members

        [ReadOperation]
        // note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin
        // because it is used in non-admin situations - perhaps we need to create a separate operation???
        public ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request)
        {
            SearchResultPage page = new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows);

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            ExternalPractitionerSearchCriteria criteria = new ExternalPractitionerSearchCriteria();
            if (!string.IsNullOrEmpty(request.FirstName))
                criteria.Name.GivenName.StartsWith(request.FirstName);
            if (!string.IsNullOrEmpty(request.LastName))
                criteria.Name.FamilyName.StartsWith(request.LastName);

            return new ListExternalPractitionersResponse(
                CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
                    PersistenceContext.GetBroker<IExternalPractitionerBroker>().Find(criteria, page),
                    delegate(ExternalPractitioner s)
                    {
                        return assembler.CreateExternalPractitionerSummary(s, PersistenceContext);
                    }));
        }

        [ReadOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request)
        {
            // note that the version of the ExternalPractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
            ExternalPractitioner s = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            return new LoadExternalPractitionerForEditResponse(s.GetRef(), assembler.CreateExternalPractitionerDetail(s, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request)
        {
            //TODO:  replace "dummy" lists
            List<string> dummyCountries = new List<string>();
            dummyCountries.Add("Canada");

            List<string> dummyProvinces = new List<string>();
            dummyProvinces.Add("Ontario");

            return new LoadExternalPractitionerEditorFormDataResponse(
                EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
                dummyProvinces,
                dummyCountries,
                (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false)
                );

        }

        [UpdateOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = new ExternalPractitioner();

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac);

            PersistenceContext.Lock(prac, DirtyState.New);

            // ensure the new prac is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

        [UpdateOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef, EntityLoadFlags.CheckVersion);

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac);

            return new UpdateExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

        #endregion
    }
}
