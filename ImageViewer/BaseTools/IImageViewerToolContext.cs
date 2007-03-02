using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// Defines an image viewer tool context.
	/// </summary>
    public interface IImageViewerToolContext : IToolContext
    {
		/// <summary>
		/// Gets the <see cref="IImageViewer"/>.
		/// </summary>
        IImageViewer Viewer { get; }

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/>.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }
    }
}
