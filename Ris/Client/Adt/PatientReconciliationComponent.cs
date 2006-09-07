using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientReconciliationComponent"/>
    /// </summary>
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

        private PatientProfileTableData _searchResults;
        private PatientProfileTableData _alternateProfiles;
        private ITableData _reconciliationCandidateProfiles;







        /// <summary>
        /// Constructor
        /// </summary>
        public PatientReconciliationComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            _searchResults = new PatientProfileTableData();
            _alternateProfiles = new PatientProfileTableData();
            _reconciliationCandidateProfiles = new PatientProfileTableData();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
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
        }

        public void SetSelectedSearchResults(ISelection selection)
        {
        }

        #endregion
    }
}
