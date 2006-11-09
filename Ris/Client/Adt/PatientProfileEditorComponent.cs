using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientProfileEditorComponent : NavigatorComponentContainer
    {
        private EntityRef<PatientProfile> _profileRef;
        private PatientProfile _profile;
        private bool _isNew;
        private IAdtService _adtService;

        private PatientProfileDetailsEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        /// <summary>
        /// Constructs an editor to edit the specified profile
        /// </summary>
        /// <param name="profileRef"></param>
        public PatientProfileEditorComponent(EntityRef<PatientProfile> profileRef)
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

        public EntityRef<PatientProfile> PatientProfile
        {
            get { return _profileRef; }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();

            if (_isNew)
            {
                _profile = new PatientProfile();
                _profile.Mrn.AssigningAuthority = "UHN";    // TODO remove this hack
                _profile.Healthcard.AssigningAuthority = "Ontario";    // TODO remove this hack
            }
            else
            {
                _profile = _adtService.LoadPatientProfile(_profileRef, true);
                this.Host.SetTitle(
                    string.Format(SR.PatientComponentTitle, _profile.Name.Format(), _profile.Mrn.Format()));
            }


            this.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientProfileDetailsEditorComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

            _patientEditor.Subject = _profile;
            _addressesSummary.Subject = _profile;
            _phoneNumbersSummary.Subject = _profile;

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Accept()
        {
            try
            {
                SaveChanges();
                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (ConcurrencyException)
            {
                this.Host.ShowMessageBox("The patient was modified by another user.  Your changes could not be saved.", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            catch (Exception e)
            {
                Platform.Log(e);
                this.Host.ShowMessageBox("An error occured while attempting to save changes to this item", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            this.Host.Exit();
        }

        public override void Cancel()
        {
            base.Cancel();
        }

        private void SaveChanges()
        {
            if (_isNew)
            {
                _adtService.CreatePatientForProfile(_profile);
                _profileRef = new EntityRef<PatientProfile>(_profile);
            }
            else
            {
                _adtService.UpdatePatientProfile(_profile);
            }
        }

    }
}
