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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's 
	/// default <see cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.IViewerShortcutManager"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseToolButtonAttribute : Attribute
	{
		private readonly XMouseButtons _mouseButton;
		private readonly bool _initiallyActive;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseToolButtonAttribute(XMouseButtons mouseButton, bool initiallyActive)
		{
			_mouseButton = mouseButton;
			_initiallyActive = initiallyActive;
		}

		/// <summary>
		/// Gets the mouse button assigned to the <see cref="MouseImageViewerTool"/>.
		/// </summary>
		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		/// <summary>
		/// Gets whether or not the tool should be initially active upon opening the viewer.
		/// </summary>
		public bool InitiallyActive
		{
			get { return _initiallyActive; }
		}
	}
}