#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            return new SaveAdminEditsForPatientProfileResponse(profile.Patient.GetRef(), profile.GetRef());
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.PatientProfileAdmin)]
        public AdminAddPatientProfileResponse AdminAddPatientProfile(AdminAddPatientProfileRequest request)
        {
            PatientProfile profile = new PatientProfile();
            Patient patient = new Patient();
            patient.AddProfile(profile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            assembler.UpdatePatientProfile(profile, request.PatientDetail, PersistenceContext);

            PersistenceContext.Lock(patient, DirtyState.New);

            PersistenceContext.SynchState();

            return new AdminAddPatientProfileResponse(patient.GetRef(), profile.GetRef());
        }

        #endregion
    }
}
