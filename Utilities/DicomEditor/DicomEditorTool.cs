using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.Utilities.DicomEditor
{
    [MenuAction("activate", "explorerlocal-contextmenu/MenuDumpFiles")]
    [ClickHandler("activate", "Dump")]
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuDicomEditor")]
    [ClickHandler("activate", "Dump")]
    [Tooltip("activate", "OpenDicomFilesVerbose")]
    [IconSet("activate", IconScheme.Colour, "Icons.DumpToolSmall.png", "Icons.DumpToolSmall.png", "Icons.DumpToolSmall.png")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[GroupHint("activate", "Tools.Dicom.Editor")]

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    [ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
    public class DicomEditorTool : ToolBase
    {
        public DicomEditorTool()
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
            if (this.ContextBase is IImageViewerToolContext)
            {
                IImageViewerToolContext context = this.ContextBase as IImageViewerToolContext;
                StandardPresentationImage image = context.Viewer.SelectedPresentationImage as StandardPresentationImage;
                if (image == null)
                {
                    Platform.ShowMessageBox(SR.MessagePleaseSelectAnImage);
                    return;
                }
                FileDicomImage file = image.ImageSop.NativeDicomObject as FileDicomImage;

                if (_component == null)
                {
                    _component = new DicomEditorComponent();

                    ApplicationComponent.LaunchAsShelf(
                                context.DesktopWindow,
                                _component,
                                "DICOM Editor",
                                ShelfDisplayHint.DockRight,
                                delegate(IApplicationComponent component) { _component = null; });

                    _component.Files = new FileDicomImage[1] { file };
                }
                else
                {
                    _component.Files = new FileDicomImage[1] { file };
                }
            }
            else if (this.ContextBase is ILocalImageExplorerToolContext)
            {
                ILocalImageExplorerToolContext context = this.ContextBase as ILocalImageExplorerToolContext;
                List<FileDicomImage> files = new List<FileDicomImage>();

                foreach (string rawPath in context.SelectedPaths)
                {
                    FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(delegate(string path) { files.Add(new FileDicomImage(path)); });
                    FileProcessor.Process(rawPath, "*.*", process, true);
                }

                if (_component == null)
                {
                    _component = new DicomEditorComponent();

                    ApplicationComponent.LaunchAsShelf(
                                context.DesktopWindow,
                                _component,
                                "DICOM Editor",
                                ShelfDisplayHint.DockRight,
                                delegate(IApplicationComponent component) { _component = null; });

                    _component.Files = files;
                }
                else
                {
                    _component.Files = files;
                }
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
