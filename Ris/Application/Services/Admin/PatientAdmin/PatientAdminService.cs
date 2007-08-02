using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

using Iesi.Collections;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.PatientAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IPatientAdminService))]
    public class PatientAdminService : ApplicationServiceBase, IPatientAdminService
    {
        #region IPatientAdminService Members

        [ReadOperation]
        public LoadPatientProfileEditorFormDataResponse LoadPatientProfileEditorFormData(LoadPatientProfileEditorFormDataRequest request)
        {
            // ignore request

            LoadPatientProfileEditorFormDataResponse response = new LoadPatientProfileEditorFormDataResponse();

            //TODO:  replace "dummy" lists
            
            List<string> dummyCountries = new List<string>();
            dummyCountries.Add("Canada");
            response.AddressCountryChoices = dummyCountries;

            List<string> dummyProvinces = new List<string>();
            dummyProvinces.Add("Ontario");
            response.AddressProvinceChoices = dummyProvinces;

            response.AddressTypeChoices = EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext);
            response.ContactPersonRelationshipChoices = EnumUtils.GetEnumValueList<ContactPersonRelationshipEnum>(PersistenceContext);
            response.ContactPersonTypeChoices = EnumUtils.GetEnumValueList<ContactPersonTypeEnum>(PersistenceContext);

            List<string> dummyHealthcardChoices = new List<string>();
            dummyHealthcardChoices.Add("Ontario");
            response.HealthcardAssigningAuthorityChoices = dummyHealthcardChoices;

            List<string> dummyMrnChoices = new List<string>();
            dummyMrnChoices.Add("UHN");
            dummyMrnChoices.Add("MSH");
            dummyMrnChoices.Add("WCH");
            response.MrnAssigningAuthorityChoices = dummyMrnChoices;

            // Sort the category from High to Low, then sort by name
            IList<NoteCategory> sortedCategoryList = CollectionUtils.Sort<NoteCategory>(
                PersistenceContext.GetBroker<INoteCategoryBroker>().FindAll(),
                delegate(NoteCategory x, NoteCategory y)
                {
                    return string.Compare(x.Name, y.Name);
                });
            
            response.NoteCategoryChoices = new List<NoteCategorySummary>();
            NoteCategoryAssembler categoryAssembler = new NoteCategoryAssembler();
            response.NoteCategoryChoices = CollectionUtils.Map<NoteCategory, NoteCategorySummary, List<NoteCategorySummary>>(
                    sortedCategoryList,
                    delegate(NoteCategory category)
                    {
                        return categoryAssembler.CreateNoteCategorySummary(category, this.PersistenceContext);
                    });

            response.PrimaryLanguageChoices = EnumUtils.GetEnumValueList<SpokenLanguageEnum>(PersistenceContext);
            response.ReligionChoices = EnumUtils.GetEnumValueList<ReligionEnum>(PersistenceContext);
            response.SexChoices = EnumUtils.GetEnumValueList<SexEnum>(PersistenceContext);
            response.PhoneTypeChoices = (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false);

            return response;
        }


        [ReadOperation]
        public LoadPatientProfileForAdminEditResponse LoadPatientProfileForAdminEdit(LoadPatientProfileForAdminEditRequest request)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(request.PatientProfileRef);
            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new LoadPatientProfileForAdminEditResponse(profile.Patient.GetRef(), profile.GetRef(), assembler.CreatePatientProfileDetail(profile, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.PatientProfileAdmin)]
        public SaveAdminEditsForPatientProfileResponse SaveAdminEditsForPatientProfile(SaveAdminEditsForPatientProfileRequest request)
        {
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef, EntityLoadFlags.CheckVersion);

            CheckForDuplicateMrn(request.PatientDetail.Mrn.Id, request.PatientDetail.Mrn.AssigningAuthority, profile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            return new SaveAdminEditsForPatientProfileResponse();
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.PatientProfileAdmin)]
        public AdminAddPatientProfileResponse AdminAddPatientProfile(AdminAddPatientProfileRequest request)
        {
            PatientProfile profile = new PatientProfile();
            Patient patient = new Patient();
            patient.AddProfile(profile);

            CheckForDuplicateMrn(request.PatientDetail.Mrn.Id, request.PatientDetail.Mrn.AssigningAuthority, profile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            PersistenceContext.Lock(patient, DirtyState.New);
//            PersistenceContext.Lock(profile, DirtyState.New);
            PersistenceContext.SynchState();

            return new AdminAddPatientProfileResponse(patient.GetRef(), profile.GetRef());
        }

        #endregion

        /// <summary>
        /// Helper method to check validate that the MRN does not already exist
        /// </summary>
        /// <param name="mrnId"></param>
        /// <param name="mrnAuthority"></param>
        /// <param name="subject"></param>
        private void CheckForDuplicateMrn(string mrnId, string mrnAuthority, PatientProfile subject)
        {
            try
            {
                PatientProfileSearchCriteria where = new PatientProfileSearchCriteria();
                where.Mrn.Id.EqualTo(mrnId);
                where.Mrn.AssigningAuthority.EqualTo(mrnAuthority);

                IList<PatientProfile> duplicateList = PersistenceContext.GetBroker<IPatientProfileBroker>().Find(where, new SearchResultPage(0, 2));
                foreach (PatientProfile duplicate in duplicateList)
                {
                    if (duplicate != subject)
                        throw new RequestValidationException(string.Format(SR.ExceptionMrnAlreadyExists, mrnAuthority, mrnId));
                }
            }
            catch (EntityNotFoundException)
            {
                // no duplicates
            }
        }

    }
}
