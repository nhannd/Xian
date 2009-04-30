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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// An interface for objects that handle mouse wheel input.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The framework will look for this interface on <see cref="ClearCanvas.Desktop.Tools.ITool"/>s belonging to the current
	/// <see cref="IImageViewer"/> (via <see cref="IViewerShortcutManager.GetMouseWheelHandler"/>) and if an appropriate one 
	/// is found it will be given capture until a short period of time has expired.
	/// </para>
	/// </remarks>
	/// <seealso cref="IImageViewer"/>
	/// <seealso cref="ImageViewerComponent"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="TileController"/>
	public interface IMouseWheelHandler
	{
		/// <summary>
		/// Called by the framework when mouse wheel input has started.
		/// </summary>
		void StartWheel();

		/// <summary>
		/// Called by the framework each time the mouse wheel is moved.
		/// </summary>
		void Wheel(int wheelDelta);

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		void StopWheel();
	}
}
