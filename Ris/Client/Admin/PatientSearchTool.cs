using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientSearchToolViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }

    [MenuAction("showHide", "Admin/Patient/Find...")]
    [ButtonAction("showHide", "PatientAdminToolbar/Find")]
    [Tooltip("showHide", "Find Patient")]
    [ClickHandler("showHide", "ShowHide")]
    [CheckedStateObserver("showHide", "IsViewActive", "ViewActivationChanged")]

    [ToolView(typeof(PatientSearchToolViewExtensionPoint), "Find Patient", ToolViewDisplayHint.DockLeft, "IsViewActive", "ViewActivationChanged")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientSearchTool : Tool
    {
        private bool _viewActive;
        private event EventHandler _viewActivationChanged;

        private IWorkspace _patientAdminWorkspace;
        private PatientAdminComponent _patientAdminComponent;

        private string _familyName;
        private string _givenName;

        private bool _searchEnabled;
        private event EventHandler _searchEnabledChanged;

        public bool IsViewActive
        {
            get { return _viewActive; }
            set
            {
                if (value != _viewActive)
                {
                    _viewActive = value;
                    EventsHelper.Fire(_viewActivationChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler ViewActivationChanged
        {
            add { _viewActivationChanged += value; }
            remove { _viewActivationChanged -= value; }
        }

        public void ShowHide()
        {
            this.IsViewActive = !this.IsViewActive;
        }

        public string FamilyName
        {
            get { return _familyName; }
            set
            {
                _familyName = value;
                UpdateDisplay();
            }
        }

        public string GivenName
        {
            get { return _givenName; }
            set
            {
                _givenName = value;
                UpdateDisplay();
            }
        }

        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            protected set
            {
                if (_searchEnabled != value)
                {
                    _searchEnabled = value;
                    EventsHelper.Fire(_searchEnabledChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler SearchEnabledChanged
        {
            add { _searchEnabledChanged += value; }
            remove { _searchEnabledChanged -= value; }
        }

        public void Search()
        {   
            if (_patientAdminWorkspace == null)
            {
                _patientAdminComponent = new PatientAdminComponent();
                _patientAdminWorkspace = ApplicationComponent.LaunchAsWorkspace(
                    _patientAdminComponent,
                    "Patient Search Results",
                    ResultsWorkspaceClosed);
            }

            _patientAdminComponent.SetSearchCriteria(BuildCriteria());
        }

        private void UpdateDisplay()
        {
            PatientSearchCriteria criteria = BuildCriteria();
            this.SearchEnabled = !criteria.IsEmpty;
        }

        private PatientSearchCriteria BuildCriteria()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria();
            if (_familyName != null && _familyName.Length > 0)
                criteria.Name.FamilyName.Like(_familyName + "%");

            if (_givenName != null && _givenName.Length > 0)
                criteria.Name.GivenName.Like(_givenName + "%");

            return criteria;
        }

        private void ResultsWorkspaceClosed(IApplicationComponent component)
        {
            _patientAdminWorkspace = null;
            _patientAdminComponent = null;
        }
    }
}
