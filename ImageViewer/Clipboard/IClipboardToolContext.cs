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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// The clipboard tool context.
	/// </summary>
	/// <remarks>
	/// Provides clipboard tools access to items in the clipboard.
	/// </remarks>
	public interface IClipboardToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> in which the clipboard is hosted.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets a list of the items in the clipboard.
		/// </summary>
		IList<IClipboardItem> ClipboardItems { get; }

		/// <summary>
		/// Gets a list of the selected items in the clipboard.
		/// </summary>
		ReadOnlyCollection<IClipboardItem> SelectedClipboardItems { get; }

		/// <summary>
		/// Occurs when the list of clipboard items has changed.
		/// </summary>
		event EventHandler ClipboardItemsChanged;

		/// <summary>
		/// Occurs when the list of selected clipboard items has changed.
		/// </summary>
		event EventHandler SelectedClipboardItemsChanged;
	}
}
