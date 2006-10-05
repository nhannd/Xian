using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

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
    public class WorklistComponent : ApplicationComponent
    {
        private PatientProfileTableData _searchResults;

        private PatientProfile _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private IAdtService _adtService;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistComponent()
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

        public ITable SearchResults
        {
            get { return _searchResults; }
            set
            {
                _searchResults.Items.Clear();
                if (value != null)
                {
                    _searchResults.Items.AddRange(value.Items);
                }
            }
        }

        public void SetSelection(ISelection selection)
        {
            this.SelectedPatient = (PatientProfile)selection.Item;
        }

        public void DoubleClickItem()
        {
            if (_selectedPatient != null)
            {
                ApplicationComponent.LaunchAsWorkspace(
                    this.Host.DesktopWindow,
                    new PatientComponent(_selectedPatient),
                    string.Format("{0} - {1}", _selectedPatient.Name.Format(), _selectedPatient.MRN.Id),
                    null);
            }
        }


        #endregion

    }
}
