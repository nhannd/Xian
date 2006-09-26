using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientSearchResultComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientSearchResultComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientSearchResultComponent class
    /// </summary>
    [AssociateView(typeof(PatientSearchResultComponentViewExtensionPoint))]
    public class PatientSearchResultComponent : ApplicationComponent
    {
        private PatientProfileTableData _searchResults;
        private PatientProfileSearchCriteria _searchCriteria;

        private PatientProfile _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private IAdtService _adtService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientSearchResultComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchResults = new PatientProfileTableData(_adtService);
        }

        public override void Stop()
        {
            base.Stop();
        }

        public PatientProfileSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                DoSearch();
            }
        }

        public PatientProfile SelectedPatient
        {
            get { return _selectedPatient; }
            protected set
            {
                if (value != _selectedPatient)
                {
                    _selectedPatient = value;
                    EventsHelper.Fire(_selectedPatientChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedPatientChanged
        {
            add { _selectedPatientChanged += value; }
            remove { _selectedPatientChanged -= value; }
        }

        #region Presentation Model

        public ITableData SearchResults
        {
            get { return _searchResults; }
        }

        public void SetSelection(ISelection selection)
        {
            this.SelectedPatient = (PatientProfile)selection.Item;
        }

        #endregion


        private void DoSearch()
        {
            _searchResults.Clear();
            if (_searchCriteria != null)
            {
                _searchResults.AddRange(_adtService.ListPatientProfiles(_searchCriteria));
            }
        }
    }
}
