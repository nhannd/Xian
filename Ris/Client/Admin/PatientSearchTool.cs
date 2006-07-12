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

            _patientAdminComponent.SetSearchCriteria(new PatientSearchCriteria());
        }

        private void ResultsWorkspaceClosed(IApplicationComponent component)
        {
            _patientAdminWorkspace = null;
            _patientAdminComponent = null;
        }
    }
}
