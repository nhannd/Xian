#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
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

	public partial class ImageViewerComponent
	{
		/// <summary>
		/// A basic implementation of <see cref="IImageViewerToolContext"/>.
		/// </summary>
		protected class ImageViewerToolContext : ToolContext, IImageViewerToolContext
		{
			private readonly ImageViewerComponent _component;

			/// <summary>
			/// Constructs a new <see cref="ImageViewerToolContext"/>.
			/// </summary>
			/// <param name="component">The <see cref="ImageViewerComponent"/> that owns the tools.</param>
			public ImageViewerToolContext(ImageViewerComponent component)
			{
				_component = component;
			}

			#region IImageViewerToolContext Members

			/// <summary>
			/// Gets the <see cref="IImageViewer"/>.
			/// </summary>
			public IImageViewer Viewer
			{
				get { return _component; }
			}

			/// <summary>
			/// Gets the <see cref="IDesktopWindow"/>.
			/// </summary>
			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			#endregion
		}
	}
}
