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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Formatting;
using System.Threading;

namespace ClearCanvas.Ris.Client
{
    public class PatientProfileEditorComponent : NavigatorComponentContainer
    {
        private EntityRef _patientRef;
        private EntityRef _profileRef;
        private PatientProfileDetail _profile;
        private readonly bool _isNew;
        private readonly List<PatientAttachmentSummary> _newAttachments;

        private PatientProfileDetailsEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;
        private EmailAddressesSummaryComponent _emailAddressesSummary;
        private ContactPersonsSummaryComponent _contactPersonsSummary;
        private PatientProfileAdditionalInfoEditorComponent _additionalPatientInfoSummary;
        private PatientNoteSummaryComponent _notesSummary;
        private AttachedDocumentPreviewComponent _documentSummary;

        /// <summary>
        /// Constructs an editor to edit the specified profile
        /// </summary>
        /// <param name="profileRef"></param>
        public PatientProfileEditorComponent(EntityRef profileRef)
        {
            _profileRef = profileRef;
            _isNew = false;
            _newAttachments = new List<PatientAttachmentSummary>();
        }

        /// <summary>
        /// Constructs an editor to edit a new profile
        /// </summary>
        public PatientProfileEditorComponent()
        {
            _isNew = true;
            _newAttachments = new List<PatientAttachmentSummary>();
        }

        /// <summary>
        /// Constructs an editor to edit a new profile with attachments
        /// </summary>
        public PatientProfileEditorComponent(EntityRef profileRef, List<PatientAttachmentSummary> attachments)
        {
            Platform.CheckForNullReference(attachments, "attachments");

            _profileRef = profileRef;
            _isNew = false;
            _newAttachments = attachments;
        }

        /// <summary>
        /// Constructs an editor to edit a new profile with attachments
        /// </summary>
        public PatientProfileEditorComponent(List<PatientAttachmentSummary> attachments)
        {
            Platform.CheckForNullReference(attachments, "attachments");

            _isNew = true;
            _newAttachments = attachments;
        }

        public EntityRef PatientRef
        {
            get { return _patientRef; }
        }

        public EntityRef PatientProfileRef
        {
            get { return _profileRef; }
        }

        public PatientProfileDetail PatientProfile
        {
            get { return _profile; }
        }

        public override void Start()
        {
            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileEditorFormDataResponse formData = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());
                    if (_isNew)
                    {
                        _profile = new PatientProfileDetail();
                        _profile.Mrn.AssigningAuthority = formData.MrnAssigningAuthorityChoices.Count > 0 ? formData.MrnAssigningAuthorityChoices[0] : null;
                        _profile.Healthcard.AssigningAuthority = formData.HealthcardAssigningAuthorityChoices.Count > 0 ? formData.HealthcardAssigningAuthorityChoices[0] : null;
                        _profile.Sex = formData.SexChoices[0];
                        _profile.Religion = formData.ReligionChoices[0];
                        _profile.PrimaryLanguage = formData.PrimaryLanguageChoices[0];
                        _profile.DateOfBirth = Platform.Time.Date;
                    }
                    else
                    {
                        LoadPatientProfileForEditResponse response = service.LoadPatientProfileForEdit(
                            new LoadPatientProfileForEditRequest(_profileRef));

                        _profileRef = response.PatientProfileRef;
                        _profile = response.PatientDetail;

                        this.Host.Title =
                            string.Format(SR.TitlePatientComponent, PersonNameFormat.Format(_profile.Name), MrnFormat.Format(_profile.Mrn));
                    }

                    if (_newAttachments.Count > 0)
                    {
                        _profile.Attachments.AddRange(_newAttachments);
                        this.AcceptEnabled = true;
                    }

                    // if the user has permission to either a) create a new patient, or b) update the patient profile, then 
                    // these pages should be displayed
                    if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientProfile.Update)
                        || Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create))
                    {
                        this.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientProfileDetailsEditorComponent(formData.SexChoices, formData.MrnAssigningAuthorityChoices, formData.HealthcardAssigningAuthorityChoices)));
                        this.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent(formData.AddressTypeChoices)));
                        this.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formData.PhoneTypeChoices)));
                        this.Pages.Add(new NavigatorPage("Patient/Email Addresses", _emailAddressesSummary = new EmailAddressesSummaryComponent()));
                        this.Pages.Add(new NavigatorPage("Patient/Contact Persons", _contactPersonsSummary = new ContactPersonsSummaryComponent(formData.ContactPersonTypeChoices, formData.ContactPersonRelationshipChoices)));
                        this.Pages.Add(new NavigatorPage("Patient/Culture", _additionalPatientInfoSummary = new PatientProfileAdditionalInfoEditorComponent(formData.ReligionChoices, formData.PrimaryLanguageChoices)));

                        _addressesSummary.SetModifiedOnListChange = true;
                        _phoneNumbersSummary.SetModifiedOnListChange = true;
                        _emailAddressesSummary.SetModifiedOnListChange = true;
                        _contactPersonsSummary.SetModifiedOnListChange = true;

                        _patientEditor.Subject = _profile;
                        _addressesSummary.Subject = _profile.Addresses;
                        _phoneNumbersSummary.Subject = _profile.TelephoneNumbers;
                        _emailAddressesSummary.Subject = _profile.EmailAddresses;
                        _contactPersonsSummary.Subject = _profile.ContactPersons;
                        _additionalPatientInfoSummary.Subject = _profile;
                    }

                    // if the user has permission to either a) create a new patient, or b) update a patient, then
                    // these pages should be displayed
                    if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create)
                        || Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Update))
                    {
                        this.Pages.Add(new NavigatorPage("Patient/Notes", _notesSummary = new PatientNoteSummaryComponent(formData.NoteCategoryChoices)));
                        _notesSummary.Subject = _profile.Notes;

                        NavigatorPage patientDocumentsPage = new NavigatorPage("Patient/Documents", 
                            _documentSummary = new AttachedDocumentPreviewComponent(true, true));
                        this.Pages.Add(patientDocumentsPage);
                        _documentSummary.PatientAttachments = _profile.Attachments;

                        if (_newAttachments.Count > 0)
                        {
                            this.MoveTo(this.Pages.IndexOf(patientDocumentsPage));
                            _documentSummary.SetInitialSelection(_newAttachments[0]);
                        }
                    }

                    this.ValidationStrategy = new AllComponentsValidationStrategy();

                });


            base.Start();
        }

        public override void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
                                            delegate
                                            {
                                                this.ExitCode = ApplicationComponentExitCode.Error;
                                                this.Host.Exit();
                                            });
                }
            }
        }

        private void SaveChanges()
        {
            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    if (_isNew)
                    {
                        AddPatientResponse response = service.AddPatient(
                            new AddPatientRequest(_profile));

                        _patientRef = response.PatientProfile.PatientRef;
                        _profileRef = response.PatientProfile.PatientProfileRef;
                    }
                    else
                    {
                        UpdatePatientProfileResponse response = service.UpdatePatientProfile(
                            new UpdatePatientProfileRequest(_profileRef, _profile));

                        _patientRef = response.PatientProfile.PatientRef;
                        _profileRef = response.PatientProfile.PatientProfileRef;
                    }
                });

            if (_documentSummary != null)
                _documentSummary.SaveChanges();
        }

    }
}