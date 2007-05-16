using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor
{
    [ButtonAction("activate", "dicomeditor-toolbar/ToolbarPrevious")]
    [ClickHandler("activate", "Previous")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipPrevious")]
	[IconSet("activate", IconScheme.Colour, "Icons.PreviousToolSmall.png", "Icons.PreviousToolSmall.png", "Icons.PreviousToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class PreviousTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public PreviousTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = false;
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

        private void Previous()
        {
            this.Context.DumpManagement.LoadedFileDumpIndex -= 1;
            this.Context.UpdateDisplay();
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            this.Enabled = !(e.IsCurrentTheOnly || e.IsCurrentFirst);
        }
    }
}
