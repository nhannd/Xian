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

            response.AddressTypeChoices = CollectionUtils.Map<AddressTypeEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<IAddressTypeEnumBroker>().Load().Items,
                delegate(AddressTypeEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });

            response.ContactPersonRelationshipChoices = CollectionUtils.Map<ContactPersonRelationshipEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<IContactPersonRelationshipEnumBroker>().Load.Values,
                delegate(ContactPersonRelationshipEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });


            response.ContactPersonTypeChoices = CollectionUtils.Map<ContactPersonTypeEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<IContactPersonTypeEnumBroker>().Load.Values,
                delegate(ContactPersonTypeEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });


            List<string> dummyHealthcardChoices = new List<string>();
            dummyHealthcardChoices.Add("Ontario");
            response.HealthcardAssigningAuthorityChoices = dummyHealthcardChoices;

            List<string> dummyMrnChoices = new List<string>();
            dummyMrnChoices.Add("UHN");
            dummyMrnChoices.Add("MSH");
            dummyMrnChoices.Add("WCH");
            response.MrnAssigningAuthorityChoices = dummyMrnChoices;

            response.NoteSeverityChoices = new List<EnumValueInfo>();
            //TODO: add Note severity when Note code is merged into trunk
            //response.NoteSeverityChoices = CollectionUtils.Map<NoteSeverityEnum, EnumValueInfo>(
            //    PersistenceContext.GetBroker<INoteSeverityEnumBroker>().Load.Values,
            //    delegate(NoteSeverityEnum e)
            //    {
            //        return new EnumValueInfo(e.Code, e.Value);
            //    });
            
            response.PrimaryLanguageChoices = CollectionUtils.Map<SpokenLanguageEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<ISpokenLanguageEnumBroker>().Load().Values,
                delegate(SpokenLanguageEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });

            response.ReligionChoices = CollectionUtils.Map<ReligionEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<IReligionEnumBroker>().Load().Values,
                delegate(ReligionEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });

            response.SexChoices = CollectionUtils.Map<SexEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<ISexEnumBroker>().Load().Values,
                delegate(SexEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });

            //TODO: Map Telephone Equipment and Telephone Use to single list

            response.TelephoneEquipmentChoices = CollectionUtils.Map<TelephoneEquipmentEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<ITelephoneEquipmentEnumBroker>().Load().Values,
                delegate(TelephoneEquipmentEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });
            
            response.TelephoneUseChoices = CollectionUtils.Map<TelephoneUseEnum, EnumValueInfo>(
                PersistenceContext.GetBroker<ITelephoneUseEnumBroker>().Load().Values,
                delegate(TelephoneUseEnum e)
                {
                    return new EnumValueInfo(e.Code, e.Value);
                });

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

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new LoadPatientProfileForAdminEditResponse(profile.GetRef(), assembler.CreatePatientProfileDetail(profile, PersistenceContext));
        }

        [UpdateOperation]
        public SaveAdminEditsForPatientProfileResponse SaveAdminEditsForPatientProfile(SaveAdminEditsForPatientProfileRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.PatientProfileRef, EntityLoadFlags.CheckVersion);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            RegistrationWorkflow.RegistrationWorkflowAssembler registrationAssembler = new ClearCanvas.Ris.Application.Services.RegistrationWorkflow.RegistrationWorkflowAssembler();
            return new SaveAdminEditsForPatientProfileResponse(registrationAssembler.CreateWorklistItem(profile, PersistenceContext));
        }

        [UpdateOperation]
        public AdminAddPatientProfileResponse AdminAddPatientProfile(AdminAddPatientProfileRequest request)
        {
            PatientProfile profile = new PatientProfile();
            Patient patient = new Patient();
            patient.AddProfile(profile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            PersistenceContext.Lock(profile, DirtyState.New);
            PersistenceContext.Lock(patient, DirtyState.New);
            PersistenceContext.SynchState();

            RegistrationWorkflow.RegistrationWorkflowAssembler registrationAssembler = new ClearCanvas.Ris.Application.Services.RegistrationWorkflow.RegistrationWorkflowAssembler();
            return new SaveAdminEditsForPatientProfileResponse(registrationAssembler.CreateWorklistItem(profile, PersistenceContext));
        }

        #endregion

    }
}
