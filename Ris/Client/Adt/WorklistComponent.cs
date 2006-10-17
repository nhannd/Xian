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
        ClickHandlerDelegate DefaultAction { get; set; }

        PatientProfile SelectedPatientProfile { get; }
        event EventHandler SelectedPatientProfileChanged;
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

            public ClickHandlerDelegate DefaultAction
            {
                get { return _component._defaultAction; }
                set { _component._defaultAction = value; }
            }

            public PatientProfile SelectedPatientProfile
            {
                get { return _component._selectedPatient; }
            }

            public event EventHandler SelectedPatientProfileChanged
            {
                add { _component.SelectedPatientChanged += value; }
                remove { _component.SelectedPatientChanged -= value; }
            }


            #endregion
        }


        private PatientProfileTable _searchResults;

        private PatientProfile _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private IAdtService _adtService;
        private ToolSet _toolSet;
        private ClickHandlerDelegate _defaultAction;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistComponent()
        {
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _adtService.PatientProfileChanged += PatientProfileChangedEventHandler;
            _searchResults = new PatientProfileTable();

            _toolSet = new ToolSet(new WorklistToolExtensionPoint(), new WorklistToolContext(this));

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            // important to unsubscribe from service
            _adtService.PatientProfileChanged -= PatientProfileChangedEventHandler;

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
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
            if (_selectedPatient != null && _defaultAction != null)
            {
                _defaultAction();
            }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "worklist-contextmenu", _toolSet.Actions);
            }
        }

        public ActionModelNode ToolbarModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "worklist-toolbar", _toolSet.Actions);
            }
        }

        #endregion

        private void PatientProfileChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            long oid = e.Change.EntityOID;

            // check if the patient with this oid is in the list
            int index = _searchResults.Items.FindIndex(delegate(PatientProfile p) { return p.OID == oid; });
            if (index > -1)
            {
                if (e.Change.ChangeType == EntityChangeType.Update)
                {
                    // the profile was updated, so we need to update the list to reflect any changes

                    // first need to check whether this item has the current selection
                    bool wasSelected = (_selectedPatient == _searchResults.Items[index]);

                    // update the profile in the list
                    PatientProfile p = _adtService.LoadPatientProfile(oid, false);
                    _searchResults.Items[index] = p;

                    // reset the selected patient
                    if (wasSelected)
                    {
                        this.SelectedPatient = p;
                    }
                }
                else if (e.Change.ChangeType == EntityChangeType.Delete)
                {
                    _searchResults.Items.RemoveAt(index);
                }
            }
        }
    }
}
