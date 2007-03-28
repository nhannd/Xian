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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/NoteTestTool")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/NoteTestTool")]
    [Tooltip("apply", "NoteTest")]
    [IconSet("apply", IconScheme.Colour, "Icons.NoteTestToolSmall.png", "Icons.NoteTestToolMedium.png", "Icons.NoteTestToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class NoteTestTool : ToolBase
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
                    AddNote(item.PatientProfileRef);
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
                    AddNote(context.PatientProfile);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, context.DesktopWindow);
                }
            }
        }

        private void AddNote(EntityRef patientProfileRef)
        {
            NoteDetail detail = new NoteDetail();
            detail.TimeStamp = Platform.Time;
            detail.Severity = "High";
            Random rand = new Random();
            detail.Text = "Random note: " + rand.Next();

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    service.SaveNewNoteForPatient(new SaveNewNoteForPatientRequest(patientProfileRef, detail));
                });
        }
    }
}
