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
    [ButtonAction("activate", "dicomeditor-toolbar/Delete")]
    [MenuAction("activate", "dicomeditor-contextmenu/Delete")]
    [ClickHandler("activate", "Delete")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "Delete Tag")]
    [IconSet("activate", IconScheme.Colour, "Icons.DeleteSmall.png", "Icons.DeleteSmall.png", "Icons.DeleteSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class DeleteTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private bool _promptForAll;

        public DeleteTool()
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

        private void Delete()
        {
			if (Platform.ShowMessageBox(SR.MessageConfirmDeleteSelectedTags, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                if (_promptForAll)
                {
					if (Platform.ShowMessageBox(SR.MessageConfirmDeleteSelectedTagsFromAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                    {
                        foreach (DicomEditorTag tag in this.Context.SelectedTags)
                        {
                            this.Context.DumpManagement.ApplyEdit(tag, EditType.Delete, true);
                        }
                        this.Context.UpdateDisplay();
                    }
                    else
                    {
                        foreach (DicomEditorTag tag in this.Context.SelectedTags)
                        {
                            this.Context.DumpManagement.ApplyEdit(tag, EditType.Delete, false);
                        }
                        this.Context.UpdateDisplay();
                    }
                }
                else
                {
                    foreach (DicomEditorTag tag in this.Context.SelectedTags)
                    {
                        this.Context.DumpManagement.ApplyEdit(tag, EditType.Delete, false);
                    }
                    this.Context.UpdateDisplay();
                }
            }
        }

        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
        {
            _promptForAll = !e.IsCurrentTheOnly;
        }
    }
}


//    [ButtonAction("activate", "dicomeditor-toolbar/Delete in All")]
//    [MenuAction("activate", "dicomeditor-contextmenu/Delete in All")]
//    [ClickHandler("activate", "DeleteAll")]
//    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
//    [Tooltip("activate", "Delete Tag from all loaded files")]
//    [IconSet("activate", IconScheme.Colour, "Icons.DeleteSmall.png", "Icons.DeleteSmall.png", "Icons.DeleteSmall.png")]
//    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
//    class DeleteAllTool : Tool<DicomEditorComponent.DicomEditorToolContext>
//    {
//        private bool _enabled;
//        private event EventHandler _enabledChanged;

//        public DeleteAllTool()
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

//        private void DeleteAll()
//        {
//            if (Platform.ShowMessageBox("The selected tag(s) will be deleted from ALL loaded files.  Continue?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
//            {
//                foreach (DicomEditorTag tag in this.Context.SelectedTags)
//                {
//                    this.Context.DumpManagement.ApplyEdit(tag, EditType.Delete, true);
//                }
//                this.Context.UpdateDisplay();
//            }
//        }

//        protected void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
//        {
//            this.Enabled = !e.IsCurrentTheOnly;
//        }      
//    }
//}
