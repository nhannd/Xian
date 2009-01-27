#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// A global image clipboard.
	/// </summary>
	/// <remarks>
	/// The clipboard can be thought of as a "holding area" for images the user has deemed to
	/// be of interest. Clipboard tools can then operate on those images.
	/// </remarks>
	public static class Clipboard
	{
		public const string ClipboardSiteToolbar = "clipboard-toolbar";
		public const string ClipboardSiteMenu = "clipboard-contextmenu";
		
		internal static readonly BindingList<IClipboardItem> Items = new BindingList<IClipboardItem>();

		/// <summary>
		/// Adds an <see cref="IPresentationImage"/> to the clipboard.
		/// </summary>
		/// <param name="image"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			Items.Add(ClipboardComponent.CreatePresentationImageItem(image));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IDisplaySet"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IDisplaySet"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");

			Items.Add(ClipboardComponent.CreateDisplaySetItem(displaySet));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="selectionStrategy"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/>s
		/// (as determined by the <paramref name="selectionStrategy"/>) is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			Platform.CheckForNullReference(selectionStrategy, "selectionStrategy");

			Items.Add(ClipboardComponent.CreateDisplaySetItem(displaySet, selectionStrategy));
		}
	}
}
