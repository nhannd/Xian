#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// Defines an interface for image layout management.
	/// </summary>
	/// <remarks>
	/// If you want to implement your own hanging protocol engine,
	/// you need to 1) implement this interface and 2) mark your class
	/// with the <code>[ExtensionOf(typeof(LayoutManagerExtensionPoint))]</code>
	/// attribute.
	/// </remarks>
	public interface ILayoutManager : IDisposable
	{
		/// <summary>
		/// Sets the owning <see cref="IImageViewer"/>.
		/// </summary>
		/// <param name="imageViewer"></param>
		void SetImageViewer(IImageViewer imageViewer);

		/// <summary>
		/// Lays out the images on the image viewer specified by <see cref="SetImageViewer"/>.
		/// </summary>
		/// <remarks>
		/// This is invoked by the <see cref="ImageViewerComponent"/> when images are
		/// first displayed, or anytime when <see cref="IImageViewer.Layout"/> is called.
		/// </remarks>
		void Layout();
	}
}