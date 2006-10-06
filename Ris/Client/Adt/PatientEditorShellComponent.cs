using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientEditorShellComponent : NavigatorComponentContainer
    {
        private PatientProfile _subject;
        private PatientEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;


        public PatientEditorShellComponent(PatientProfile subject)
        {
            _subject = subject;

            this.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientEditorComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));
        }

        public override void Start()
        {
            _patientEditor.Subject = _subject;
            _addressesSummary.Subject = _subject;
            _phoneNumbersSummary.Subject = _subject;

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }
    }
}
