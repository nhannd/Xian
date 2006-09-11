using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

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
        private string _mrn;
        private string _healthcard;
        private string _familyName;
        private string _givenName;
        private string _error;

        private PatientProfileTableData _searchResults;
        private PatientProfileTableData _alternateProfiles;
        private ReconciliationCandidateTableData _reconciliationCandidateProfiles;

        private PatientProfile _selectedSearchResult;

        private IAdtService _adtService;

        public event EventHandler ErrorChanged;







        /// <summary>
        /// Constructor
        /// </summary>
        public PatientReconciliationComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            _adtService = ApplicationContext.GetService<IAdtService>();

            _searchResults = new PatientProfileTableData(_adtService);
            _alternateProfiles = new PatientProfileTableData(_adtService);
            _reconciliationCandidateProfiles = new ReconciliationCandidateTableData(_adtService);
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string Mrn
        {
            get { return _mrn; }
            set { _mrn = value; }
        }

        public string Healthcard
        {
            get { return _healthcard; }
            set { _healthcard = value; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }

        public string GivenName
        {
            get { return _givenName; }
            set { _givenName = value; }
        }

        public string Error
        {
            get { return _error; }
            private set { _error = value; }
        }

        public ITableData SearchResults
        {
            get { return _searchResults; }
        }

        public ITableData AlternateProfiles
        {
            get { return _alternateProfiles; }
        }

        public ITableData ReconciliationCandidateProfiles
        {
            get { return _reconciliationCandidateProfiles; }
        }

        public void Search()
        {
            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            if (_mrn != null)
                criteria.MRN.Id.Like(_mrn + "%");
            if (_healthcard != null)
                criteria.Healthcard.Id.Like(_healthcard + "%");
            if (_familyName != null)
                criteria.Name.FamilyName.Like(_familyName + "%");
            if (_givenName != null)
                criteria.Name.GivenName.Like(_givenName + "%");

            IList<PatientProfile> profiles = _adtService.ListPatientProfiles(criteria);

            _searchResults.Clear();
            _searchResults.AddRange(profiles);
        }

        public void SetSelectedSearchResults(ISelection selection)
        {
            _selectedSearchResult = (PatientProfile)selection.Item;

            RefreshAlternateProfiles();
            RefreshReconciliationCandidates();
        }

        public void Reconcile()
        {
            _error = "";
            IList<Patient> checkedPatients = new List<Patient>();
            foreach (ReconciliationCandidateTableEntry entry in _reconciliationCandidateProfiles)
            {
                if (entry.Checked && !checkedPatients.Contains(entry.ProfileMatch.PatientProfile.Patient))
                {
                    checkedPatients.Add(entry.ProfileMatch.PatientProfile.Patient);
                }
            }

            // confirmation
            ConfirmReconciliationComponent confirmComponent = new ConfirmReconciliationComponent(_selectedSearchResult.Patient, checkedPatients);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, confirmComponent, "Confirm Reconciliation");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {

                try
                {
                    _adtService.ReconcilePatient(_selectedSearchResult.Patient, checkedPatients);
                }
                catch (PatientReconciliationException e)
                {
                    Console.WriteLine(e.ToString());
                    Error = e.Message;
                    ErrorChanged(this, new EventArgs());

                }

                RefreshAlternateProfiles();
                RefreshReconciliationCandidates();
            }
        }


        #endregion

        private void RefreshAlternateProfiles()
        {
            _alternateProfiles.Clear();

            if (_selectedSearchResult != null)
            {
                IList<PatientProfile> alternates = _adtService.ListReconciledPatientProfiles(_selectedSearchResult);
                _alternateProfiles.AddRange(alternates);
            }
        }

        private void RefreshReconciliationCandidates()
        {
            _reconciliationCandidateProfiles.Clear();

            if (_selectedSearchResult != null)
            {
                IList<PatientProfileMatch> matches = _adtService.FindPatientReconciliationMatches(_selectedSearchResult);
                foreach (PatientProfileMatch match in matches)
                {
                    _reconciliationCandidateProfiles.Add(new ReconciliationCandidateTableEntry(match));
                }
            }
        }


    }
}
