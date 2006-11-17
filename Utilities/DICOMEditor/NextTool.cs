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
    [ButtonAction("activate", "dicomeditor-toolbar/Next")]
    [ClickHandler("activate", "Next")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "View Previous File")]
    [IconSet("activate", IconScheme.Colour, "Icons.NextSmall.png", "Icons.NextSmall.png", "Icons.NextSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class NextTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public NextTool()
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

        private void Next()
        {
            this.Context.DumpManagement.LoadedFileDumpIndex += 1;
            this.Context.UpdateDisplay();
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            this.Enabled = !(e.IsCurrentTheOnly || e.IsCurrentLast);
        }
    }
}
