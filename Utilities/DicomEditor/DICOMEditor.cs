using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.DicomEditor
{
    [MenuAction("Dump", "global-menus/Utilities/DicomEditor")]
    [ClickHandler("Dump", "Dump")]
    [MenuAction("Dump", "explorerlocal-contextmenu/DumpFiles")]
    [ClickHandler("Dump", "Dump")]
    [Tooltip("Dump", "OpenDicomFilesVerbose")]
    [IconSet("Dump", IconScheme.Colour, "Icons.DumpToolSmall.png", "Icons.DumpToolSmall.png", "Icons.DumpToolSmall.png")]
    [EnabledStateObserver("Dump", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    [ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
    public class DicomEditor : ToolBase
    {
        public DicomEditor()
        {
            _enabled = true;
            _component = null;
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
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
                    //EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
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

        private void Dump()
        {
            if (_component == null)
            {
                _component = new DicomEditorComponent();
                
                DicomEditorFileDumper dumper = DicomEditorFileDumper.New(this.ContextBase, _component);
                dumper.LaunchAsShelf();
                _component.Files = dumper.GetFiles();
            }
            else
            {
                DicomEditorFileDumper dumper = DicomEditorFileDumper.New(this.ContextBase, _component);
                _component.Files = dumper.GetFiles();
            }
        }

        private bool _enabled;
        private event EventHandler _enabledChanged;
        private DicomEditorComponent _component; 
    }

    // Original pass at making this a menu item tool
    //[MenuAction("show", "global-menus/Utilities/Dicom Editor")]
    //[ClickHandler("show", "Show")]

    //[ExtensionOf(typeof(DesktopToolExtensionPoint))]

    //public class DicomEditor : Tool<IDesktopToolContext>
    //{
    //    private DicomEditorComponent _component;

    //    public DICOMEditor()
    //    {
    //    }

    //    public void Show()
    //    {
    //        if (_component == null)
    //        {
    //            _component = new DicomEditorComponent();

    //            ApplicationComponent.LaunchAsShelf(
    //                this.Context.DesktopWindow,
    //                _component,
    //                "DICOM Editor",
    //                ShelfDisplayHint.DockLeft,
    //                delegate(IApplicationComponent component) { _component = null; });
                
    //        }
    //    }

    //}
}
