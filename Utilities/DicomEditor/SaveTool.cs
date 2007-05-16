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
    //[MenuAction("activate", "global-menus/MenuTools/MenuToolsMyTools/SaveTool")]
    [ButtonAction("activate", "dicomeditor-toolbar/ToolbarSave")]
    [Tooltip("activate", "TooltipSave")]
	[IconSet("activate", IconScheme.Colour, "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png")]
    [ClickHandler("activate", "Save")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    public class SaveTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public SaveTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
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
        public void Save()
        {
			if (Platform.ShowMessageBox(SR.MessageConfirmSaveAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                this.Context.DumpManagement.SaveAll();
                this.Context.UpdateDisplay();
            }
        }
    }
}
