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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class WorklistToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IWorklistToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        PatientProfile SelectedPatientProfile { get; }
    }


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
        class WorklistToolContext : ToolContext, IWorklistToolContext
        {
            private WorklistComponent _component;

            public WorklistToolContext(WorklistComponent component)
            {
                _component = component;
            }

            #region IWorklistToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public PatientProfile SelectedPatientProfile
            {
                get { return _component._selectedPatient; }
            }

            #endregion
        }


        private PatientProfileTable _searchResults;

        private PatientProfile _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private IAdtService _adtService;
        private ToolSet _toolSet; 

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistComponent()
        {
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchResults = new PatientProfileTable();

            _toolSet = new ToolSet(new WorklistToolExtensionPoint(), new WorklistToolContext(this));

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

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
                    new PatientOverviewComponent(_selectedPatient),
                    string.Format("{0} - {1}", _selectedPatient.Name.Format(), _selectedPatient.MRN.Id),
                    null);
            }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "worklist-contextmenu", _toolSet.Actions);
            }
        }


        #endregion

    }
}
