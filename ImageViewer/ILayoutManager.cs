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