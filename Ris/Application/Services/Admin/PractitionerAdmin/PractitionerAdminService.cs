using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.PractitionerAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IPractitionerAdminService))]
    public class PractitionerAdminService : ApplicationServiceBase, IPractitionerAdminService
    {
        #region IPractitionerAdminService Members

        [ReadOperation]
        public FindPractitionersResponse FindPractitioners(FindPractitionersRequest request)
        {
            //Note: FamilyName should never be null because the null reference check is done by WCF using the DataMember IsRequired attribute
            PractitionerSearchCriteria criteria = new PractitionerSearchCriteria();
            criteria.Name.FamilyName.Like(StringCriteriaWithWildcardAppendedTo(request.FamilyName));
            if (request.GivenName != null)
            {
                criteria.Name.GivenName.Like(StringCriteriaWithWildcardAppendedTo(request.GivenName));
            }

            PractitionerAssembler assembler = new PractitionerAssembler();
            return new FindPractitionersResponse(
                CollectionUtils.Map<Practitioner, PractitionerSummary, List<PractitionerSummary>>(
                    PersistenceContext.GetBroker<IPractitionerBroker>().Find(criteria),
                    delegate(Practitioner p)
                    {
                        return assembler.CreatePractitionerSummary(p);
                    }));
        }

        private string StringCriteriaWithWildcardAppendedTo(string surname)
        {
            return surname.IndexOf('%') < 0 ? surname + "%" : surname;
        }

        [ReadOperation]
        public ListAllPractitionersResponse ListAllPractitioners(ListAllPractitionersRequest request)
        {
            PractitionerAssembler assembler = new PractitionerAssembler();
            return new ListAllPractitionersResponse(
                CollectionUtils.Map<Practitioner, PractitionerSummary, List<PractitionerSummary>>(
                    PersistenceContext.GetBroker<IPractitionerBroker>().FindAll(),
                    delegate(Practitioner p)
                    {
                        return assembler.CreatePractitionerSummary(p);
                    }));
        }

        [ReadOperation]
        public LoadPractitionerForEditResponse LoadPractitionerForEdit(LoadPractitionerForEditRequest request)
        {
            // note that the version of the PractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
            Practitioner p = (Practitioner)PersistenceContext.Load(request.PractitionerRef);
            PractitionerAssembler assembler = new PractitionerAssembler();

            return new LoadPractitionerForEditResponse(p.GetRef(), assembler.CreatePractitionerDetail(p, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadPractitionerEditorFormDataResponse LoadPractitionerEditorFormData(LoadPractitionerEditorFormDataRequest request)
        {
            //TODO:  replace "dummy" lists
            List<string> dummyCountries = new List<string>();
            dummyCountries.Add("Canada");

            List<string> dummyProvinces = new List<string>();
            dummyProvinces.Add("Ontario");

            return new LoadPractitionerEditorFormDataResponse(
                CollectionUtils.Map<AddressTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                    PersistenceContext.GetBroker<IAddressTypeEnumBroker>().Load().Items,
                    delegate(AddressTypeEnum e)
                    {
                        return new EnumValueInfo(e.Code.ToString(), e.Value);
                    }),
                dummyProvinces,
                dummyCountries,
                (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false));

        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.PractitionerAdmin)]
        public AddPractitionerResponse AddPractitioner(AddPractitionerRequest request)
        {
            Practitioner practitioner = new Practitioner();
            PractitionerAssembler assembler = new PractitionerAssembler();
            assembler.UpdatePractitioner(request.PractitionerDetail, practitioner);

            CheckForDuplicatePractitioner(request.PractitionerDetail.PersonNameDetail.FamilyName,
                request.PractitionerDetail.PersonNameDetail.GivenName,
                practitioner);

            PersistenceContext.Lock(practitioner, DirtyState.New);

            // ensure the new practitioner is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddPractitionerResponse(assembler.CreatePractitionerSummary(practitioner));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.PractitionerAdmin)]
        public UpdatePractitionerResponse UpdatePractitioner(UpdatePractitionerRequest request)
        {
            Practitioner practitioner = (Practitioner)PersistenceContext.Load(request.PractitionerRef, EntityLoadFlags.CheckVersion);

            PractitionerAssembler assembler = new PractitionerAssembler();
            assembler.UpdatePractitioner(request.PractitionerDetail, practitioner);

            CheckForDuplicatePractitioner(request.PractitionerDetail.PersonNameDetail.FamilyName,
                request.PractitionerDetail.PersonNameDetail.GivenName,
                practitioner);

            return new UpdatePractitionerResponse(assembler.CreatePractitionerSummary(practitioner));
        }

        #endregion

        /// <summary>
        /// Helper method to check that the practitioner with the same name does not already exist
        /// </summary>
        /// <param name="familiyName"></param>
        /// <param name="givenName"></param>
        /// <param name="subject"></param>
        private void CheckForDuplicatePractitioner(string familiyName, string givenName, Practitioner subject)
        {
            try
            {
                PractitionerSearchCriteria where = new PractitionerSearchCriteria();
                where.Name.FamilyName.EqualTo(familiyName);
                where.Name.GivenName.EqualTo(givenName);

                Practitioner duplicate = PersistenceContext.GetBroker<IPractitionerBroker>().FindOne(where);
                if (duplicate != subject)
                    throw new RequestValidationException(string.Format(SR.ExceptionPractitionerAlreadyExist, familiyName, givenName));
            }
            catch (EntityNotFoundException)
            {
                // no duplicates
            }
        }
    }
}
