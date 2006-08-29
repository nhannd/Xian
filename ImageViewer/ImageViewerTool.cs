using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
{
    public class ImageViewerTool : Tool
    {
        /// <summary>
        /// The <see cref="IImageViewerToolContext"/> with which this tool is associated.
        /// </summary>
        protected IImageViewerToolContext Context
        {
            get { return (IImageViewerToolContext)base.ContextBase; }
        }
    }
}
