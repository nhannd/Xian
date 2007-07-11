using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("view", "global-menus/Patient/View Details...")]
    [ButtonAction("view", "global-toolbars/Patient/ViewPatient")]
    [ButtonAction("view", "folderexplorer-items-toolbar/Details")]
    [MenuAction("view", "folderexplorer-items-contextmenu/Details")]
    [MenuAction("view", "RegistrationPreview-menu/Details")]
    [ClickHandler("view", "View")]
    [EnabledStateObserver("view", "Enabled", "EnabledChanged")]
    [Tooltip("view", "Open patient details")]
	[IconSet("view", IconScheme.Colour, "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationPreviewToolExtensionPoint))]
    public class PatientOverviewTool : Tool<IToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        
        public override void Initialize()
        {
            base.Initialize();

            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItemsChanged += delegate(object sender, EventArgs args)
                {
                    this.Enabled = DetermineEnablement();
                };
            }
        }

        private bool DetermineEnablement()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                return (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                    && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
            }
            else if (this.ContextBase is IRegistrationPreviewToolContext)
            {
                IRegistrationPreviewToolContext context = (IRegistrationPreviewToolContext)this.ContextBase;
                return (context.WorklistItem != null && context.WorklistItem.PatientProfileRef != null);
            }

            return false;
        }

        public bool Enabled
        {
            get 
            { 
                this.Enabled = DetermineEnablement();
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void View()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(context.SelectedItems);
                OpenPatient(item.PatientProfileRef, context.DesktopWindow);
            }
            else if (this.ContextBase is IRegistrationPreviewToolContext)
            {
                IRegistrationPreviewToolContext context = (IRegistrationPreviewToolContext)this.ContextBase;
                OpenPatient(context.WorklistItem.PatientProfileRef, context.DesktopWindow);
            }
        }

        protected void OpenPatient(EntityRef profile, IDesktopWindow window)
        {
            Document doc = DocumentManager.Get(profile.ToString());
            if (doc == null)
            {
                doc = new PatientOverviewDocument(profile, window);
                doc.Open();
            }
            else
            {
                doc.Activate();
            }
        }
    }
}
