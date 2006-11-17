using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorFileDumper
    {
        protected DicomEditorFileDumper()
        {

        }

        public static DicomEditorFileDumper New(IToolContext ToolContext, DicomEditorComponent Component)
        {
            if (ToolContext is IImageViewerToolContext)
            {
                return new DicomEditorImageViewerFileDumper(ToolContext, Component);
            }
            else if (ToolContext is ILocalImageExplorerToolContext)
            {
                return new DicomEditorLocalImageExplorerFileDumper(ToolContext, Component);
            }
            else
            {
                return null;
            }
        }

        public virtual void LaunchAsShelf()
        {
        }

        public virtual IEnumerable<FileDicomImage> GetFiles()
        {
            return new List<FileDicomImage>();
        }

        protected DicomEditorComponent _component; 
    }

    public class DicomEditorImageViewerFileDumper : DicomEditorFileDumper
    {
        public DicomEditorImageViewerFileDumper(IToolContext ToolContext, DicomEditorComponent Component)
        {
            _toolContext = ToolContext as IImageViewerToolContext;
            base._component = Component;
        }

        public override void LaunchAsShelf()
        {
            ApplicationComponent.LaunchAsShelf(
                            _toolContext.DesktopWindow,
                            _component,
                            "DICOM Editor",
                            ShelfDisplayHint.DockRight,
                            delegate(IApplicationComponent component) { _component = null; });
        }

        public override IEnumerable<FileDicomImage> GetFiles()
        {
            DicomPresentationImage image = _toolContext.Viewer.SelectedPresentationImage as DicomPresentationImage;
            FileDicomImage file = image.ImageSop.NativeDicomObject as FileDicomImage;
            
            return new FileDicomImage[1] { file };
        }

        private IImageViewerToolContext _toolContext;
    }

    public class DicomEditorLocalImageExplorerFileDumper : DicomEditorFileDumper
    {
        public DicomEditorLocalImageExplorerFileDumper(IToolContext ToolContext, DicomEditorComponent Component)
        {
            _toolContext = ToolContext as ILocalImageExplorerToolContext;
            base._component = Component;
        }

        public override void LaunchAsShelf()
        {
            ApplicationComponent.LaunchAsShelf(
                            _toolContext.DesktopWindow,
                            _component,
                            "DICOM Editor",
                            ShelfDisplayHint.DockRight,
                            delegate(IApplicationComponent component) { _component = null; });
        }

        public override IEnumerable<FileDicomImage> GetFiles()
        {
            List<FileDicomImage> files = new List<FileDicomImage>();

            foreach (string rawPath in _toolContext.SelectedPaths)
            {
                FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(delegate(string path) { files.Add(new FileDicomImage(path)); });
                FileProcessor.Process(rawPath, "*.*", process, true);
            }

            return files;
        }

        private ILocalImageExplorerToolContext _toolContext;
    }

    


}
