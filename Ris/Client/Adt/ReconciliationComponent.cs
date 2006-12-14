using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

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
        class DiffHost : ApplicationComponentHost
        {
            private ReconciliationComponent _owner;

            public DiffHost(ReconciliationComponent owner, PatientProfileDiffComponent diff)
                : base(diff)
	        {
                _owner = owner;
	        }

            public override IDesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }
        }

        private PatientProfileDiffComponent _diffComponent;
        private ApplicationComponentHost _diffComponentHost;

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
            // create the diff component
            _diffComponentHost = new DiffHost(this, _diffComponent = new PatientProfileDiffComponent());
            _diffComponentHost.StartComponent();

            // get the ADT service
            _adtService = ApplicationContext.GetService<IAdtService>();

            // add all target profiles - ensure the initially selected one is at the top of the list
            _targetProfileTable = new PatientProfileTable();
            _targetProfileTable.Items.Add(_selectedTargetProfile);
            foreach (PatientProfile profile in _selectedTargetProfile.Patient.Profiles)
            {
                if (!profile.Equals(_selectedTargetProfile))
                {
                    _targetProfileTable.Items.Add(profile);
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
            _diffComponentHost.StopComponent();
            base.Stop();
        }

        public IApplicationComponentView DiffComponentView
        {
            get { return _diffComponentHost.ComponentView; }
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
                UpdateDiff();
            }
        }

        public void SetSelectedReconciliationProfile(ISelection selection)
        {
            ReconciliationCandidateTableEntry entry = (ReconciliationCandidateTableEntry)selection.Item;
            PatientProfile profile = (entry == null) ? null : entry.ProfileMatch.PatientProfile;
            if (profile != _selectedReconciliationProfile)
            {
                _selectedReconciliationProfile = profile;
                UpdateDiff();
            }
        }

        public bool ReconcileEnabled
        {
            get { return AnyCandidatesChecked(); }
        }

        public void Reconcile()
        {
            try
            {
                DoReconciliation();
                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (PatientReconciliationException e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToReconcile, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }

            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion

        private void UpdateDiff()
        {
            if (_selectedTargetProfile == null || _selectedReconciliationProfile == null)
            {
                _diffComponent.ProfilesToCompare = null;
            }
            else
            {
                _diffComponent.ProfilesToCompare = new EntityRef<PatientProfile>[] {
                    new EntityRef<PatientProfile>(_selectedTargetProfile),
                    new EntityRef<PatientProfile>(_selectedReconciliationProfile) };
            }
        }

        private void DoReconciliation()
        {
            IList<Patient> checkedPatients = new List<Patient>();
            foreach (ReconciliationCandidateTableEntry entry in _reconciliationProfileTable.Items)
            {
                if (entry.Checked && !checkedPatients.Contains(entry.ProfileMatch.PatientProfile.Patient))
                {
                    // we need to load all the profiles for this patient, for the confirmation stage
                    Patient patient = _adtService.LoadPatientAndAllProfiles(new EntityRef<PatientProfile>(entry.ProfileMatch.PatientProfile));
                    checkedPatients.Add(patient);
                }
            }

            // confirmation
            ReconciliationConfirmComponent confirmComponent = new ReconciliationConfirmComponent(_selectedTargetProfile.Patient, checkedPatients);
            ApplicationComponentExitCode confirmExitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, confirmComponent, SR.TitleConfirmReconciliation);

            if (confirmExitCode == ApplicationComponentExitCode.Normal)
            {
                _adtService.ReconcilePatients(_selectedTargetProfile.Patient, checkedPatients);
            }
        }

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
