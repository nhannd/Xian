#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines a rendering surface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Unless you are implementing your own renderering surface, you should never
	/// have to interact with this interface.  The two properties on 
	/// <see cref="IRenderingSurface"/> should only ever have to be set by the 
	/// Framework.
	/// </para>
	/// </remarks>
	public interface IRenderingSurface : IDisposable
	{
		/// <summary>
		/// Gets or sets the window ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the window handle, or "hwnd" 
		/// of the WinForms control you will eventually render to.  This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr WindowID
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the context ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the device context handle, or "hdc"
		/// of the WinForms control you will eventually render to. This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr ContextID
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the rectangle of the surface.
		/// </summary>
		/// <remarks>
		/// This is the rectangle of the view onto the <see cref="ITile"/>.
		/// The top-left corner is always (0,0).  This rectangle changes as the
		/// view (i.e., the hosting window) changes size. Implementor should be
		/// aware that the rectangle can have a width or height of 0, and handle
		/// that boundary case appropriately.
		/// </remarks>
		Rectangle ClientRectangle
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the rectangle that requires repainting.
		/// </summary>
		/// <remarks>
		/// The implementer of <see cref="IRenderer"/> should use this rectangle
		/// to intelligently perform the <see cref="DrawMode.Refresh"/> operation.
		/// This property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		Rectangle ClipRectangle
		{ 
			get; 
			set;
		}

	}
}
