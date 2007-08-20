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
using ClearCanvas.ImageViewer.StudyManagement;

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
		private IShelf _shelf;
		private DicomEditorComponent _component;
        private bool _enabled;
        private event EventHandler _enabledChanged;

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
                IImageSopProvider image = context.Viewer.SelectedPresentationImage as IImageSopProvider;
                if (image == null)
                {                    
                    context.DesktopWindow.ShowMessageBox(SR.MessagePleaseSelectAnImage, MessageBoxActions.Ok);
                    return;
                }
                DicomFile file = image.ImageSop.NativeDicomObject as DicomFile;

                //Fix for Ticket #623 - HH - It turns out that for memory usage optimization the pixel data tag is stripped from the in memory dataset.  
                //So while there are probably many better ways to address the missing pixel data tag a small hack was introduced because this entire utility will 
                //be completely refactored in the very near future to make use of the methods the pacs uses to parse the tags.
                //Addendum to Comment above - HH 07/27/07 - Turns out that our implementation continues to remove the pixel data for optimization at this time so 
                //the workaround is still needed.
                file = new DicomFile(file.Filename);

				if (_shelf != null)
				{
					_shelf.Activate();
				}
				else
                {
                    _component = new DicomEditorComponent();

                    _shelf = ApplicationComponent.LaunchAsShelf(
                                context.DesktopWindow,
                                _component,
                                SR.TitleDicomEditor,
                                ShelfDisplayHint.DockRight | ShelfDisplayHint.DockAutoHide,
                                delegate(IApplicationComponent component)
								{
									_shelf = null;
									_component = null; 
								});
                }

                _component.Clear();
                _component.Load(file.Filename);
                _component.UpdateComponent();

            }
            else if (this.ContextBase is ILocalImageExplorerToolContext)
            {
                ILocalImageExplorerToolContext context = this.ContextBase as ILocalImageExplorerToolContext;
                List<string> files = new List<string>();
                bool newComponent = false;

                foreach (string rawPath in context.SelectedPaths)
                {
                    FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(delegate(string path) { files.Add(path); });
                    FileProcessor.Process(rawPath, "*.*", process, true);
                }

                if (_component == null)
                {
                    _component = new DicomEditorComponent();
                    newComponent = true;
                }
                else
                {
                    _component.Clear();
                }

                bool userCancelled = false;

                BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext backgroundcontext)
                {
                    int i = 0;

                    foreach (string file in files)
                    {
                        if (backgroundcontext.CancelRequested)
                        {
                            backgroundcontext.Cancel();
                            userCancelled = true;
                            return;
                        }
                        try
                        {
                            _component.Load(file);
                        }
                        catch (DicomException e)
                        {
                            backgroundcontext.Error(e);
                            return;
                        }
                        backgroundcontext.ReportProgress(new BackgroundTaskProgress((int)(((double)(i + 1) / (double)files.Count) * 100.0), SR.MessageDumpProgressBar));
                        i++;
                    }

                    backgroundcontext.Complete(null);
                }, true);

                try
                {
                    ProgressDialog.Show(task, context.DesktopWindow, true);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.MessageFailedDump, context.DesktopWindow);
                    return;
                }

                if (userCancelled == true)
                    return;


                if (newComponent == true)
                {
					_shelf = ApplicationComponent.LaunchAsShelf(
								context.DesktopWindow,
								_component,
								SR.TitleDicomEditor,
								ShelfDisplayHint.DockRight | ShelfDisplayHint.DockAutoHide,
								delegate(IApplicationComponent component)
								{
									_shelf = null;
									_component = null; 
								});
				}

                _component.UpdateComponent();
            }
        }
    }
}
