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

using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Graphics {
	/// <summary>
	/// Provides access to a <see cref="GraphicCollection"/> containing application-level graphics.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Application-level graphics are rendered after any domain-level graphics and before any
	/// user-level graphics.
	/// </para>
	/// <para>
	/// If you have subclassed <see cref="PresentationImage"/> and want to expose
	/// a <see cref="GraphicCollection"/> to <see cref="ImageViewerTool"/> objects, 
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
	/// <seealso cref="IOverlayGraphicsProvider"/>
	public interface IApplicationGraphicsProvider
	{
		/// <summary>
		/// Gets a <see cref="GraphicCollection"/> of application-level graphics.
		/// </summary>
		GraphicCollection ApplicationGraphics { get; }
	}
}
