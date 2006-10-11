using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientReconciliationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientReconciliationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientReconciliationComponent class
    /// </summary>
    [AssociateView(typeof(PatientReconciliationComponentViewExtensionPoint))]
    public class PatientReconciliationComponent : ApplicationComponent
    {
        private PatientPreviewComponent _targetPreviewComponent;
        private ApplicationComponentHost _targetPreviewHost;

        private PatientPreviewComponent _reconciliationPreviewComponent;
        private ApplicationComponentHost _reconciliationPreviewHost;

        private PatientProfile _selectedTargetProfile;
        private PatientProfile _selectedReconciliationProfile;

        private PatientProfileTable _targetProfiles;
        private ReconciliationCandidateTable _reconciliationProfiles;

        private IAdtService _adtService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientReconciliationComponent(PatientProfile targetProfile)
        {
            _selectedTargetProfile = targetProfile;
        }

        public override void Start()
        {
            // create the preview components
            _targetPreviewHost = new ApplicationComponentHost(_targetPreviewComponent = new PatientPreviewComponent(false), this.Host.DesktopWindow);
            _reconciliationPreviewHost = new ApplicationComponentHost(_reconciliationPreviewComponent = new PatientPreviewComponent(false), this.Host.DesktopWindow);

            // get the ADT service
            _adtService = ApplicationContext.GetService<IAdtService>();

            // ensure all profiles for the patient are loaded
            _adtService.LoadPatientProfiles(_selectedTargetProfile.Patient);

            // add all target profiles - ensure the initially selected one is at the top of the list
            _targetProfiles = new PatientProfileTable();
            _targetProfiles.Items.Add(_selectedTargetProfile);
            foreach (PatientProfile alternateProfile in _selectedTargetProfile.Patient.Profiles)
            {
                if (!alternateProfile.MRN.Equals(_selectedTargetProfile.MRN))
                {
                    _targetProfiles.Items.Add(alternateProfile);
                }
            }

            IList<PatientProfileMatch> matches = _adtService.FindPatientReconciliationMatches(_selectedTargetProfile);
            _reconciliationProfiles = new ReconciliationCandidateTable();

            foreach (PatientProfileMatch match in matches)
            {
                _reconciliationProfiles.Items.Add(new ReconciliationCandidateTableEntry(match));
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public IApplicationComponentView TargetPreviewComponentView
        {
            get { return _targetPreviewHost.ComponentView; }
        }

        public IApplicationComponentView ReconciliationPreviewComponentView
        {
            get { return _reconciliationPreviewHost.ComponentView; }
        }

        #region Presentation Model

        public ITable TargetProfiles
        {
            get { return _targetProfiles; }
        }

        public ITable ReconciliationProfiles
        {
            get { return _reconciliationProfiles; }
        }

        public void SetSelectedTargetProfile(ISelection selection)
        {
            PatientProfile profile = (PatientProfile)selection.Item;
            if (profile != _selectedTargetProfile)
            {
                _selectedTargetProfile = profile;
                _targetPreviewComponent.Subject = _selectedTargetProfile;
            }
        }

        public void SetSelectedReconciliationProfile(ISelection selection)
        {
            ReconciliationCandidateTableEntry entry = (ReconciliationCandidateTableEntry)selection.Item;
            PatientProfile profile = (entry == null) ? null : entry.ProfileMatch.PatientProfile;
            if (profile != _selectedReconciliationProfile)
            {
                _selectedReconciliationProfile = profile;
                _reconciliationPreviewComponent.Subject = _selectedReconciliationProfile;
            }
        }

        public void Reconcile()
        {
            IList<Patient> checkedPatients = new List<Patient>();
            foreach (ReconciliationCandidateTableEntry entry in _reconciliationProfiles.Items)
            {
                if (entry.Checked && !checkedPatients.Contains(entry.ProfileMatch.PatientProfile.Patient))
                {
                    checkedPatients.Add(entry.ProfileMatch.PatientProfile.Patient);
                }
            }

            // confirmation
            ConfirmReconciliationComponent confirmComponent = new ConfirmReconciliationComponent(_selectedTargetProfile.Patient, checkedPatients);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, confirmComponent, "Confirm Reconciliation");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {

                try
                {
                    _adtService.ReconcilePatient(_selectedTargetProfile.Patient, checkedPatients);
                }
                catch (PatientReconciliationException e)
                {
                    Platform.Log(e);
                    this.Host.ShowMessageBox("An error occured while attempting to reconcile the patient profiles", MessageBoxActions.Ok);
                }
            }

            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();

        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }


        #endregion
    }
}
