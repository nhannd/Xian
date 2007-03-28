using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientProfileEditorComponent : NavigatorComponentContainer
    {
        private EntityRef _profileRef;
        private PatientProfileDetail _profile;
        private bool _isNew;

        private PatientProfileDetailsEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;
        private EmailAddressesSummaryComponent _emailAddressesSummary;
        private ContactPersonsSummaryComponent _contactPersonsSummary;
        private PatientProfileAdditionalInfoEditorComponent _additionalPatientInfoSummary;
        private NoteSummaryComponent _notesSummary;

        /// <summary>
        /// Constructs an editor to edit the specified profile
        /// </summary>
        /// <param name="profileRef"></param>
        public PatientProfileEditorComponent(EntityRef profileRef)
        {
            _profileRef = profileRef;
            _isNew = false;
        }

        /// <summary>
        /// Constructs an editor to edit a new profile
        /// </summary>
        public PatientProfileEditorComponent()
        {
            _isNew = true;
        }

        public EntityRef PatientProfile
        {
            get { return _profileRef; }
        }

        public override void Start()
        {

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileEditorFormDataResponse formData = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());
                    
                    this.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientProfileDetailsEditorComponent(formData.SexChoices)));
                    this.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent(formData.AddressTypeChoices)));
                    this.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formData.PhoneTypeChoices)));
                    this.Pages.Add(new NavigatorPage("Patient/Email Addresses", _emailAddressesSummary = new EmailAddressesSummaryComponent()));
                    this.Pages.Add(new NavigatorPage("Patient/Contact Persons", _contactPersonsSummary = new ContactPersonsSummaryComponent(formData.ContactPersonTypeChoices, formData.ContactPersonRelationshipChoices)));
                    this.Pages.Add(new NavigatorPage("Patient/Additional Info", _additionalPatientInfoSummary = new PatientProfileAdditionalInfoEditorComponent(formData.ReligionChoices, formData.PrimaryLanguageChoices)));

                    this.ValidationStrategy = new AllNodesContainerValidationStrategy();

                    if (_isNew)
                    {
                        _profile = new PatientProfileDetail();
                        _profile.MrnAssigningAuthority = "UHN";    // TODO remove this hack
                        _profile.HealthcardAssigningAuthority = "Ontario";    // TODO remove this hack
                        _profile.Sex = formData.SexChoices[0];
                        _profile.Religion = formData.ReligionChoices[0];
                        _profile.PrimaryLanguage = formData.PrimaryLanguageChoices[0];
                        _profile.DateOfBirth = Platform.Time.Date;
                    }
                    else
                    {
                        LoadPatientProfileForAdminEditResponse response = service.LoadPatientProfileForAdminEdit(
                            new LoadPatientProfileForAdminEditRequest(_profileRef));

                        _profileRef = response.PatientProfileRef;
                        _profile = response.PatientDetail;

                        this.Host.SetTitle(
                            string.Format(SR.TitlePatientComponent, Format.Custom(_profile.Name), _profile.Mrn));
                    }
                });

            _patientEditor.Subject = _profile;
            _addressesSummary.Subject = _profile.Addresses;
            _phoneNumbersSummary.Subject = _profile.TelephoneNumbers;
            _emailAddressesSummary.Subject = _profile.EmailAddresses;
            _contactPersonsSummary.Subject = _profile.ContactPersons;
            _additionalPatientInfoSummary.Subject = _profile;

            if (_isNew == false)
            {
                this.Pages.Add(new NavigatorPage("Patient/Notes", _notesSummary = new NoteSummaryComponent()));
                _notesSummary.Subject = _profile.Notes;
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
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
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    this.ExitCode = ApplicationComponentExitCode.Error;
                }
                this.Host.Exit();
            }
        }

        public override void Cancel()
        {
            base.Cancel();
        }

        private void SaveChanges()
        {
            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    if (_isNew)
                    {
                        AdminAddPatientProfileResponse response = service.AdminAddPatientProfile(
                            new AdminAddPatientProfileRequest(_profile));

                        _profileRef = response.PatientProfileRef;
                    }
                    else
                    {
                        service.SaveAdminEditsForPatientProfile(
                            new SaveAdminEditsForPatientProfileRequest(_profileRef, _profile));
                    }
                });

        }

    }
}
