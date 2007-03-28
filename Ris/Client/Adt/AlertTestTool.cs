using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/AlertTestTool")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/AlertTestTool")]
    [Tooltip("apply", "AlertTest")]
    [IconSet("apply", IconScheme.Colour, "Icons.NoteTestToolSmall.png", "Icons.NoteTestToolMedium.png", "Icons.NoteTestToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class AlertTestTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                _enabled = false;   // disable by default
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                        && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else
            {
                _enabled = true;    // always enabled
            }
        }

        /// <summary>
        /// Called to determine whether this tool is enabled/disabled in the UI.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            protected set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);

                try
                {
                    TestAlert(item.PatientProfileRef);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, context.DesktopWindow);
                }
            }
            else
            {
                IPatientOverviewToolContext context = (IPatientOverviewToolContext)this.ContextBase;
                try
                {
                    TestAlert(context.PatientProfile);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, context.DesktopWindow);
                }
            }
        }

        private void TestAlert(EntityRef patientProfileRef)
        {
            StringBuilder sb = new StringBuilder();

            Platform.GetService<IAlertService>(
                delegate(IAlertService service)
                {
                    GetAlertsByPatientProfileResponse response = service.GetAlertsByPatientProfile(new GetAlertsByPatientProfileRequest(patientProfileRef));

                    foreach (AlertNotificationDetail detail in response.AlertNotifications)
                    {
                        sb.AppendLine(string.Format("Representation: {0} | Severity: {1} | Type: {2}", detail.Representation, detail.Severity, detail.Type));
                    }
                });

            Platform.ShowMessageBox(sb.ToString());        
        }
    }
}
