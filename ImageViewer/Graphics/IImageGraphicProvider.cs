#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides access to an <see cref="ImageGraphic"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you have subclassed <see cref="PresentationImage"/> and want to expose
	/// an <see cref="ImageGraphic"/> to <see cref="ImageViewerTool"/> objects, 
	/// do so by implementing this interface in your subclass.
	/// </para>
	/// <para>
	/// In general, avoid accessing members of subclasses of <see cref="PresentationImage"/>
	/// directly.  Prefer instead to use provider interfaces such as this one.  Doing
	/// so prevents <see cref="ImageViewerTool"/> objects from having to know about specific
	/// subclasses of <see cref="PresentationImage"/>, thus allowing them to work with
	/// any type of <see cref="PresentationImage"/> that implements the provider interface.
	/// </para>
	/// </remarks>
	public interface IImageGraphicProvider
	{
		/// <summary>
		/// Gets an <see cref="ImageGraphic"/>.
		/// </summary>
		ImageGraphic ImageGraphic { get; }
	}
}
