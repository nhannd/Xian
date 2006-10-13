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
    public class ReconciliationComponent : ApplicationComponent
    {
        class PreviewHost : ApplicationComponentHost
        {
            private ReconciliationComponent _owner;

            public PreviewHost(ReconciliationComponent owner, PatientProfilePreviewComponent preview)
                :base(preview)
	        {
                _owner = owner;
	        }

            public override IDesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }
        }

        private PatientProfilePreviewComponent _targetPreviewComponent;
        private ApplicationComponentHost _targetPreviewHost;

        private PatientProfilePreviewComponent _reconciliationPreviewComponent;
        private ApplicationComponentHost _reconciliationPreviewHost;

        private PatientProfile _selectedTargetProfile;
        private PatientProfile _selectedReconciliationProfile;

        private PatientProfileTable _targetProfileTable;
        private ReconciliationCandidateTable _reconciliationProfileTable;

        private IList<PatientProfileMatch> _matches;

        private IAdtService _adtService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationComponent(PatientProfile targetProfile, IList<PatientProfileMatch> matches)
        {
            _selectedTargetProfile = targetProfile;
            _matches = matches;
        }

        public override void Start()
        {
            // create the preview components
            _targetPreviewHost = new PreviewHost(this, _targetPreviewComponent = new PatientProfilePreviewComponent(false));
            _targetPreviewHost.StartComponent();

            _reconciliationPreviewHost = new PreviewHost(this, _reconciliationPreviewComponent = new PatientProfilePreviewComponent(false));
            _reconciliationPreviewHost.StartComponent();

            // get the ADT service
            _adtService = ApplicationContext.GetService<IAdtService>();

            // ensure all profiles for the patient are loaded
            _adtService.LoadPatientProfiles(_selectedTargetProfile.Patient);

            // add all target profiles - ensure the initially selected one is at the top of the list
            _targetProfileTable = new PatientProfileTable();
            _targetProfileTable.Items.Add(_selectedTargetProfile);
            foreach (PatientProfile alternateProfile in _selectedTargetProfile.Patient.Profiles)
            {
                if (!alternateProfile.MRN.Equals(_selectedTargetProfile.MRN))
                {
                    _targetProfileTable.Items.Add(alternateProfile);
                }
            }

            _reconciliationProfileTable = new ReconciliationCandidateTable();
            foreach (PatientProfileMatch match in _matches)
            {
                ReconciliationCandidateTableEntry entry = new ReconciliationCandidateTableEntry(match);
                entry.CheckedChanged += new EventHandler(CandidateCheckedChangedEventHandler);
                _reconciliationProfileTable.Items.Add(entry);
            }

            base.Start();
        }

        public override void Stop()
        {
            _targetPreviewHost.StopComponent();
            _reconciliationPreviewHost.StopComponent();
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

        public ITable TargetProfileTable
        {
            get { return _targetProfileTable; }
        }

        public ITable ReconciliationProfileTable
        {
            get { return _reconciliationProfileTable; }
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

        public bool ReconcileEnabled
        {
            get { return AnyCandidatesChecked(); }
        }

        public void Reconcile()
        {
            IList<Patient> checkedPatients = new List<Patient>();
            foreach (ReconciliationCandidateTableEntry entry in _reconciliationProfileTable.Items)
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
                    _adtService.ReconcilePatients(_selectedTargetProfile.Patient, checkedPatients);
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

        private void CandidateCheckedChangedEventHandler(object sender, EventArgs e)
        {
            ReconciliationCandidateTableEntry changedEntry = (ReconciliationCandidateTableEntry)sender;

            foreach (ReconciliationCandidateTableEntry entry in _reconciliationProfileTable.Items)
            {
                if (entry != changedEntry && entry.ProfileMatch.PatientProfile.Patient.Equals(changedEntry.ProfileMatch.PatientProfile.Patient))
                {
                    entry.Checked = changedEntry.Checked;
                    _reconciliationProfileTable.Items.NotifyItemUpdated(entry);
                }
            }

            NotifyPropertyChanged("ReconcileEnabled");
        }

        private bool AnyCandidatesChecked()
        {
            foreach (ReconciliationCandidateTableEntry entry in _reconciliationProfileTable.Items)
            {
                if (entry.Checked)
                    return true;
            }
            return false;
        }

    }
}
