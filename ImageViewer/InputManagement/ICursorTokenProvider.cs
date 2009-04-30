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

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A provider of a <see cref="CursorToken"/> that is returned based on the current mouse position within an <see cref="ITile"/>.
	/// </summary>
	/// <remarks>
	/// The framework will look for this interface on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>) when the
	/// mouse has moved within the current <see cref="ITile"/>.  If the object returns a <see cref="CursorToken"/>, then the
	/// corresponding cursor will be shown at the current mouse position.
	/// </remarks>
	/// <seealso cref="CursorToken"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.Graphic"/>
	public interface ICursorTokenProvider
	{
		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		CursorToken GetCursorToken(Point point);
	}
}
