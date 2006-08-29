using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
    public interface IImageViewerToolContext : IToolContext
    {
        StudyManager StudyManager { get; }
        IImageViewer Viewer { get; }
    }
}
