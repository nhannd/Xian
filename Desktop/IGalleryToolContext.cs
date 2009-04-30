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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for tools that operate on a <see cref="GalleryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public class GalleryToolExtensionPoint : ExtensionPoint<ITool> {}

	/// <summary>
	/// <see cref="IToolContext"/> class for tools that operate on a <see cref="GalleryComponent"/>.
	/// </summary>
	public interface IGalleryToolContext : IToolContext
	{
		/// <summary>
		/// Indicates that the current selection of <see cref="IGalleryItem"/>s in the gallery has changed.
		/// </summary>
		event EventHandler SelectionChanged;

		/// <summary>
		/// Indicates that an <see cref="IGalleryItem"/> in the gallery has been activated
		/// </summary>
		event GalleryItemEventHandler ItemActivated;

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> that the <see cref="GalleryComponent"/> is on.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the underlying <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.
		/// </summary>
		IBindingList DataSource { get; }

		/// <summary>
		/// Gets the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		ISelection Selection { get; }

		/// <summary>
		/// Gets the data objects of the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		ISelection SelectedData { get; }

		/// <summary>
		/// Activates the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <param name="item">The item to activate.</param>
		void Activate(IGalleryItem item);

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>s.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="selection">The items to select.</param>
		void Select(IEnumerable<IGalleryItem> selection);

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="item">The item to select.</param>
		void Select(IGalleryItem item);
	}
}