#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
