using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
    public interface IImageViewerToolContext : IToolContext
    {
		/// <summary>
		/// Gets the <see cref="StudyManager"/>
		/// </summary>
        StudyManager StudyManager { get; }

		/// <summary>
		/// Gets the <see cref="IImageViewer"/>
		/// </summary>
        IImageViewer Viewer { get; }
    }
}
