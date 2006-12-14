using System;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise;
using ClearCanvas.HL7;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("view1", "global-menus/Patient/View Details...")]
    [ButtonAction("view1", "global-toolbars/Patient/ViewPatient")]
    [ClickHandler("view1", "View")]
    [EnabledStateObserver("view1", "Enabled", "EnabledChanged")]
    [Tooltip("view1", "Open patient details")]
    [IconSet("view1", IconScheme.Colour, "PatientOpenToolSmall.png", "PatientOpenToolMedium.png", "PatientOpenToolLarge.png")]

    [MenuAction("view2", "hl7Queue-contextmenu/View Details")]
    [ButtonAction("view2", "hl7Queue-toolbar/Details")]
    [ClickHandler("view2", "View")]
    [EnabledStateObserver("view2", "Enabled", "EnabledChanged")]
    [Tooltip("view2", "Open patient details")]
    [IconSet("view2", IconScheme.Colour, "OpenItemSmall.png", "OpenItemMedium.png", "OpenItemLarge.png")]

    [ExtensionOf(typeof(HL7QueueToolExtensionPoint))]
    public class HL7QueuePatientSearchTool : Tool<IHL7QueueToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            this.Context.DefaultAction = View;
            this.Context.SelectedHL7QueueItemChanged += delegate(object sender, EventArgs args)
            {
                this.Enabled = (this.Context.SelectedHL7QueueItem != null);
            };
        }

        public bool Enabled
        {
            get { return _enabled; }
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
            OpenPatient(this.Context.SelectedHL7QueueItem, this.Context.DesktopWindow);
        }

        protected void OpenPatient(EntityRef<HL7QueueItem> hl7QueueItemRef, IDesktopWindow window)
        {
            IHL7QueueService service = ApplicationContext.GetService<IHL7QueueService>();
            PatientProfile profile = service.GetReferencedPatient(hl7QueueItemRef);

            if (profile != null)
            {
                EntityRef<PatientProfile> profileRef = new EntityRef<PatientProfile>(profile);
                Document doc = DocumentManager.Get(profileRef.ToString());
                if (doc == null)
                {
                    doc = new PatientOverviewDocument(profileRef, window);
                    doc.Open();
                }
                else
                {
                    doc.Activate();
                }
            }
            else
            {
                Platform.ShowMessageBox(SR.MessagePatientNotFound);
            }
        }
    }
}
