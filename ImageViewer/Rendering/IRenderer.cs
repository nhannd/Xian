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

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines an <see cref="IPresentationImage"/> renderer.
	/// </summary>
	/// <remarks>
	/// Unless you are implementing your own renderer, you should never
	/// have to interact with this interface.  The two methods on <see cref="IRenderer"/>
	/// should only ever have to be called by the Framework, and thus
	/// should be treated as internal.
	/// </remarks>
	public interface IRenderer : IDisposable
	{
		/// <summary>
		/// Gets an <see cref="IRenderingSurface"/>.
		/// </summary>
		/// <param name="windowID">The window ID.  On Windows systems, this is the window handle
		/// or "hwnd".</param>
		/// <param name="width">The width of the surface.</param>
		/// <param name="height">The height of the surface.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method is called by <b>TileControl</b> (i.e., the <see cref="ITile"/> view)
		/// whenever it is resized, which includes when the control is first created.
		/// Once <b>TileControl</b> has obtained the surface, it just holds onto it.
		/// Your implementation of <see cref="Draw"/> should just render to the
		/// that same surface (passed in via the <see cref="DrawArgs"/>) irrespective 
		/// of the <see cref="IPresentationImage"/> being rendered.
		/// </remarks>
		IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height);

		/// <summary>
		/// Draws the <see cref="IPresentationImage"/> passed in through the 
		/// <see cref="DrawArgs"/>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>
		/// This method is called by the <see cref="PresentationImage"/> whenever
		/// <see cref="IDrawable.Draw"/> is called.  If you are implementing
		/// your own renderer, <see cref="DrawArgs"/> contains all you need to 
		/// know to perform the rendering, such as the <see cref="IRenderingSurface"/>, etc.  
		/// </remarks>
		void Draw(DrawArgs args);
	}
}
