using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientEditorShellComponent : StackComponentContainer
    {
        private PatientProfile _subject;
        private PatientEditorComponent _patientEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        private bool _openInEditMode;
        private bool _editing;


        public PatientEditorShellComponent(PatientProfile subject, bool openInEditMode)
        {
            _subject = subject;
            _openInEditMode = openInEditMode;
        }

        public PatientProfile Subject
        {
            get { return _subject; }
        }

        public override void Start()
        {
            this.Push(new PatientOverviewComponent(_subject));
            if (_openInEditMode)
            {
                OpenEditor();
            }
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void OpenEditor()
        {
            if (!_editing)
            {
                // reload the patient for 3 reasons:
                // 1. the existing copy in memory may be stale
                // 2. the editor should operate on a copy
                // 3. it may need patient details that were not loaded when the patients were listed
                IAdtService service = ApplicationContext.GetService<IAdtService>();
                PatientProfile profile = service.LoadPatientProfile(_subject.OID, true);

                NavigatorComponentContainer navigator = new NavigatorComponentContainer();
                navigator.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientEditorComponent()));
                navigator.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent()));
                navigator.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

                _patientEditor.Subject = profile;
                _addressesSummary.Subject = profile;
                _phoneNumbersSummary.Subject = profile;

                this.Push(navigator);

                _editing = true;
            }
        }
    }
}
