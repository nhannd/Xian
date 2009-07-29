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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

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

            response.AddressTypeChoices = EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext);
            response.ContactPersonRelationshipChoices = EnumUtils.GetEnumValueList<ContactPersonRelationshipEnum>(PersistenceContext);
            response.ContactPersonTypeChoices = EnumUtils.GetEnumValueList<ContactPersonTypeEnum>(PersistenceContext);
            response.HealthcardAssigningAuthorityChoices = EnumUtils.GetEnumValueList<InsuranceAuthorityEnum>(PersistenceContext);
            response.MrnAssigningAuthorityChoices = EnumUtils.GetEnumValueList<InformationAuthorityEnum>(PersistenceContext);

            // Sort the category from High to Low, then sort by name
            IList<PatientNoteCategory> sortedCategoryList = CollectionUtils.Sort(
				PersistenceContext.GetBroker<IPatientNoteCategoryBroker>().FindAll(false),
                delegate(PatientNoteCategory x, PatientNoteCategory y)
                {
                    return string.Compare(x.Name, y.Name);
                });
            
            response.NoteCategoryChoices = new List<PatientNoteCategorySummary>();
            PatientNoteCategoryAssembler categoryAssembler = new PatientNoteCategoryAssembler();
            response.NoteCategoryChoices = CollectionUtils.Map<PatientNoteCategory, PatientNoteCategorySummary, List<PatientNoteCategorySummary>>(
                    sortedCategoryList,
                    delegate(PatientNoteCategory category)
                    {
                        return categoryAssembler.CreateNoteCategorySummary(category, this.PersistenceContext);
                    });

            response.PrimaryLanguageChoices = EnumUtils.GetEnumValueList<SpokenLanguageEnum>(PersistenceContext);
            response.ReligionChoices = EnumUtils.GetEnumValueList<ReligionEnum>(PersistenceContext);
            response.SexChoices = EnumUtils.GetEnumValueList<SexEnum>(PersistenceContext);
            response.PhoneTypeChoices = (new SimplifiedPhoneTypeAssembler()).GetPatientPhoneTypeChoices();

            return response;
        }


        [ReadOperation]
        public LoadPatientProfileForEditResponse LoadPatientProfileForEdit(LoadPatientProfileForEditRequest request)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(request.PatientProfileRef);
            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new LoadPatientProfileForEditResponse(profile.Patient.GetRef(), profile.GetRef(), assembler.CreatePatientProfileDetail(profile, PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Patient.Update)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.PatientProfile.Update)]
        public UpdatePatientProfileResponse UpdatePatientProfile(UpdatePatientProfileRequest request)
        {
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef, EntityLoadFlags.CheckVersion);

        	bool updatePatient = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Patient.Update);
			bool updateProfile = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.PatientProfile.Update);

            UpdateHelper(profile, request.PatientDetail, updatePatient, updateProfile);

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new UpdatePatientProfileResponse(assembler.CreatePatientProfileSummary(profile, PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Patient.Create)]
        public AddPatientResponse AddPatient(AddPatientRequest request)
        {
            PatientProfile profile = new PatientProfile();
            Patient patient = new Patient();
            patient.AddProfile(profile);

            UpdateHelper(profile, request.PatientDetail, true, true);

            PersistenceContext.Lock(patient, DirtyState.New);
            PersistenceContext.SynchState();

            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new AddPatientResponse(assembler.CreatePatientProfileSummary(profile, PersistenceContext));
        }

        #endregion

        private void UpdateHelper(PatientProfile profile, PatientProfileDetail detail, bool updatePatient, bool updateProfile)
        {
            if (updatePatient)
            {
                Patient patient = profile.Patient;

                PatientNoteAssembler noteAssembler = new PatientNoteAssembler();
                noteAssembler.Synchronize(patient, detail.Notes, CurrentUserStaff, PersistenceContext);

                PatientAttachmentAssembler attachmentAssembler = new PatientAttachmentAssembler();
                attachmentAssembler.Synchronize(patient.Attachments, detail.Attachments, this.CurrentUserStaff, PersistenceContext);
            }

            if(updateProfile)
            {
                PatientProfileAssembler assembler = new PatientProfileAssembler();
                assembler.UpdatePatientProfile(profile, detail, PersistenceContext);
            }
        }
    }
}
