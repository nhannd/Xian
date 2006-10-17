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
        private PatientProfile _subject;

        private PatientProfileDetailsEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        public PatientProfileEditorComponent(PatientProfile subject)
        {
            _subject = subject;
        }

        public PatientProfile Subject
        {
            get { return _subject; }
        }

        public override void Start()
        {
            this.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientProfileDetailsEditorComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

            _patientEditor.Subject = _subject;
            _addressesSummary.Subject = _subject;
            _phoneNumbersSummary.Subject = _subject;

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
            catch (ConcurrentModificationException)
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
            IAdtService service = ApplicationContext.GetService<IAdtService>();
            if (_subject.IsNew)
            {
                service.CreatePatientForProfile(_subject);
            }
            else
            {
                service.UpdatePatientProfile(_subject);
            }
        }

    }
}
