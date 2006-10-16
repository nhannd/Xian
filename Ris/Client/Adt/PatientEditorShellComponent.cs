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
    public class PatientEditorShellComponent : StackComponentContainer
    {
        private PatientProfile _subject;

        private PatientOverviewComponent _patientOverview;
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
            this.Push(_patientOverview = new PatientOverviewComponent(_subject));
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
                PatientProfile profile = null;
                if (_subject.IsNew)
                {
                    profile = _subject;
                }
                else
                {
                    // reload the patient for 3 reasons:
                    // 1. the existing copy in memory may be stale
                    // 2. the editor should operate on a copy
                    // 3. it may need patient details that were not loaded when the patients were listed
                    IAdtService service = ApplicationContext.GetService<IAdtService>();
                    profile = service.LoadPatientProfile(_subject.OID, true);
                }

                NavigatorComponentContainer navigator = new NavigatorComponentContainer();
                navigator.Pages.Add(new NavigatorPage("Patient", _patientEditor = new PatientEditorComponent()));
                navigator.Pages.Add(new NavigatorPage("Patient/Addresses", _addressesSummary = new AddressesSummaryComponent()));
                navigator.Pages.Add(new NavigatorPage("Patient/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

                _patientEditor.Subject = profile;
                _addressesSummary.Subject = profile;
                _phoneNumbersSummary.Subject = profile;

                this.Push(navigator, PatientEditorExited);

                _editing = true;
            }
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            if (component.ExitCode == ApplicationComponentExitCode.Normal)
            {
                bool success = false;
                PatientProfile profile = _patientEditor.Subject;
                try
                {
                    IAdtService service = ApplicationContext.GetService<IAdtService>();
                    if (profile.IsNew)
                    {
                        service.CreatePatientForProfile(profile);
                    }
                    else
                    {
                        service.UpdatePatientProfile(profile);
                    }

                    success = true;
                }
                catch (ConcurrentModificationException)
                {
                    this.Host.ShowMessageBox("The patient was modified by another user.  Your changes could not be saved.", MessageBoxActions.Ok);
                }
                catch (Exception e)
                {
                    Platform.Log(e);
                    this.Host.ShowMessageBox("An error occured while attempting to save changes to this item", MessageBoxActions.Ok);
                    this.ExitCode = ApplicationComponentExitCode.Cancelled;
                    this.Host.Exit();
                }

                if (success)
                {
                    // set the subject to the updated profile
                    _subject = profile;
                    _patientOverview.Subject = _subject;

                    // update the title, in case the info has changed
                    this.Host.SetTitle(
                        string.Format(SR.PatientComponentTitle, profile.Name.Format(), profile.MRN.Format()));

                }
            }
        }

    }
}
