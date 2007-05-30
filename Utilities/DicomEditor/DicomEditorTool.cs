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
	[IconSet("activate", IconScheme.Colour, "Icons.DicomEditorToolSmall.png", "Icons.DicomEditorToolMedium.png", "Icons.DicomEditorToolLarge.png")]
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

                DicomFileAccessor accessor = new DicomFileAccessor();

                if (_component == null)
                {
                    _component = new DicomEditorComponent();

                    ApplicationComponent.LaunchAsShelf(
                                context.DesktopWindow,
                                _component,
                                SR.TitleDicomEditor,
                                ShelfDisplayHint.DockRight,
                                delegate(IApplicationComponent component) { _component = null; });
                }
                
                _component.Dumps = new DicomDump[1] { accessor.LoadDicomDump(file) };
                
            }
            else if (this.ContextBase is ILocalImageExplorerToolContext)
            {
                ILocalImageExplorerToolContext context = this.ContextBase as ILocalImageExplorerToolContext;
                List<FileDicomImage> files = new List<FileDicomImage>();
                List<DicomDump> dumps = new List<DicomDump>();

                foreach (string rawPath in context.SelectedPaths)
                {
                    FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(delegate(string path) { files.Add(new FileDicomImage(path)); });
                    FileProcessor.Process(rawPath, "*.*", process, true);
                }
                    
                DicomFileAccessor accessor = new DicomFileAccessor();

                bool userCancelled = false;
                BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext backgroundcontext)
                {
                    int i = 0;

                    foreach (FileDicomImage file in files)
                    {
                        if (backgroundcontext.CancelRequested)
                        {
                            backgroundcontext.Cancel();
                            userCancelled = true;
                            return;
                        }
                        try
                        {
                            dumps.Add(accessor.LoadDicomDump(file));
                        }
                        catch (GeneralDicomException e)
                        {
                            backgroundcontext.Error(e);
                        }
                        backgroundcontext.ReportProgress(new BackgroundTaskProgress((int)(((double)(i + 1) / (double)files.Count) * 100.0), SR.MessageDumpProgressBar));
                        i++;
                    }

                    backgroundcontext.Complete(null);
                }, true);

                try
                {
                    ProgressDialog.Show(task, context.DesktopWindow, true);
                    if (userCancelled == true)
                        return;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.MessageFailedDump, context.DesktopWindow);                    
                    return;
                }

                if (_component == null)
                {
                    _component = new DicomEditorComponent();

                    ApplicationComponent.LaunchAsShelf(
                                context.DesktopWindow,
                                _component,
                                SR.TitleDicomEditor,
                                ShelfDisplayHint.DockRight,
                                delegate(IApplicationComponent component) { _component = null; });
                }
                
                _component.Dumps = dumps;
            }            
        }

        private bool _enabled;
        private event EventHandler _enabledChanged;
        private DicomEditorComponent _component; 
    }
}
