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

            response.AddressTypeChoices = CollectionUtils.Map<AddressTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IAddressTypeEnumBroker>().Load().Items,
                delegate(AddressTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.ContactPersonRelationshipChoices = CollectionUtils.Map<ContactPersonRelationshipEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IContactPersonRelationshipEnumBroker>().Load().Items,
                delegate(ContactPersonRelationshipEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });


            response.ContactPersonTypeChoices = CollectionUtils.Map<ContactPersonTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IContactPersonTypeEnumBroker>().Load().Items,
                delegate(ContactPersonTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });


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
                    if (x.Severity > y.Severity)
                        return 1;
                    else if (x.Severity < y.Severity)
                        return -1;

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

            response.PrimaryLanguageChoices = CollectionUtils.Map<SpokenLanguageEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<ISpokenLanguageEnumBroker>().Load().Items,
                delegate(SpokenLanguageEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.ReligionChoices = CollectionUtils.Map<ReligionEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IReligionEnumBroker>().Load().Items,
                delegate(ReligionEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.SexChoices = CollectionUtils.Map<SexEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<ISexEnumBroker>().Load().Items,
                delegate(SexEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.PhoneTypeChoices = (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false);

            return response;
        }


        [ReadOperation]
        public LoadPatientProfileForAdminEditResponse LoadPatientProfileForAdminEdit(LoadPatientProfileForAdminEditRequest request)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(request.PatientProfileRef);
            broker.LoadAddressesForPatientProfile(profile);
            broker.LoadTelephoneNumbersForPatientProfile(profile);
            broker.LoadEmailAddressesForPatientProfile(profile);
            broker.LoadContactPersonsForPatientProfile(profile);
            broker.LoadPatientForPatientProfile(profile);

            PersistenceContext.GetBroker<IPatientBroker>().LoadNotesForPatient(profile.Patient);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new LoadPatientProfileForAdminEditResponse(profile.Patient.GetRef(), profile.GetRef(), assembler.CreatePatientProfileDetail(profile, PersistenceContext));
        }

        [UpdateOperation]
        public SaveAdminEditsForPatientProfileResponse SaveAdminEditsForPatientProfile(SaveAdminEditsForPatientProfileRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.PatientProfileRef, EntityLoadFlags.CheckVersion);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            return new SaveAdminEditsForPatientProfileResponse();
        }

        [UpdateOperation]
        public AdminAddPatientProfileResponse AdminAddPatientProfile(AdminAddPatientProfileRequest request)
        {
            PatientProfile profile = new PatientProfile();
            Patient patient = new Patient();
            patient.AddProfile(profile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            PersistenceContext.Lock(patient, DirtyState.New);
//            PersistenceContext.Lock(profile, DirtyState.New);
            PersistenceContext.SynchState();

            return new AdminAddPatientProfileResponse(patient.GetRef(), profile.GetRef());
        }

        #endregion

    }
}
