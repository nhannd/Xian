using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
    public interface IImageViewerToolContext : IToolContext
    {
		/// <summary>
		/// Gets the <see cref="IImageViewer"/>
		/// </summary>
        IImageViewer Viewer { get; }
		IDesktopWindow DesktopWindow { get; }
    }
}
