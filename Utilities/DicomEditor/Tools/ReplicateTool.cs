using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarReplicate", "Replicate")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuReplicate", "Replicate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipReplicate")]
	[IconSet("activate", IconScheme.Colour, "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class ReplicateTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public ReplicateTool()
        {
        }
        
        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
        }

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

        private void Replicate()
        {
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmReplicateTagsInAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                foreach (DicomEditorTag tag in this.Context.SelectedTags)
                {
                    this.Context.DumpManagement.EditTag(tag.TagId, tag.Value, true);
                }
                this.Context.UpdateDisplay();
            }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            this.Enabled = !e.IsCurrentTheOnly;
        }
    }
}
