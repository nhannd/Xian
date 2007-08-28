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
    [ButtonAction("activate", "dicomeditor-toolbar/ToolbarRevert")]
    [MenuAction("activate", "dicomeditor-contextmenu/MenuRevert")]
    [ClickHandler("activate", "Revert")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipRevert")]
	[IconSet("activate", IconScheme.Colour, "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class RevertTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private bool _promptForAll;

        public RevertTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
            this.Context.TagEditted += new EventHandler(OnTagEditted);
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

        private void Revert()
        {
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevert, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                if (_promptForAll)
                {
                    if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevertAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                    {
                        this.Context.DumpManagement.RevertEdits(true);
                        this.Context.UpdateDisplay();
                        this.Enabled = false;
                    }
                    else
                    {
                        this.Context.DumpManagement.RevertEdits(false);
                        this.Context.UpdateDisplay();
                        this.Enabled = false;
                    }
                }
                else
                {
                    this.Context.DumpManagement.RevertEdits(false);
                    this.Context.UpdateDisplay();
                    this.Enabled = false;
                }
            }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            _promptForAll = !e.IsCurrentTheOnly;
            this.Enabled = e.HasCurrentBeenEditted == true ? true : false;
        }

        protected void OnTagEditted(object sender, EventArgs e)
        {
            this.Enabled = true;
        }
    }
}

//    [ButtonAction("activate", "dicomeditor-toolbar/Revert All")]
//    [MenuAction("activate", "dicomeditor-contextmenu/Revert All")]
//    [ClickHandler("activate", "RevertAll")]
//    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
//    [Tooltip("activate", "Revert all files")]
//    [IconSet("activate", IconScheme.Colour, "Icons.RevertSmall.png", "Icons.RevertSmall.png", "Icons.RevertSmall.png")]
//    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
//    class RevertAllTool : Tool<DicomEditorComponent.DicomEditorToolContext>
//    {
//        private bool _enabled;
//        private event EventHandler _enabledChanged;

//        public RevertAllTool()
//        {
//        }

//        public override void Initialize()
//        {
//            base.Initialize();
//            this.Enabled = true;
//            this.Context.DisplayedDumpChanged += new EventHandler<DisplayedDumpChangedEventArgs>(OnDisplayedDumpChanged);
//        }

//        public bool Enabled
//        {
//            get { return _enabled; }
//            protected set
//            {
//                if (_enabled != value)
//                {
//                    _enabled = value;
//                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
//                }
//            }
//        }

//        /// <summary>
//        /// Notifies that the Enabled state of this tool has changed.
//        /// </summary>
//        public event EventHandler EnabledChanged
//        {
//            add { _enabledChanged += value; }
//            remove { _enabledChanged -= value; }
//        }

//        private void RevertAll()
//        {
//            if (Platform.ShowMessageBox("ALL the loaded files will be reverted to their last saved state.  All edits will be lost.  Continue?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
//            {
//                this.Context.DumpManagement.RevertAllEdits();
//                this.Context.UpdateDisplay();
//            }
//        }

//        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
//        {
//            this.Enabled = !e.IsCurrentTheOnly;
//        }
//    }
//}
