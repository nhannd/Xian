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

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	[MenuAction("show", "global-menus/MenuView/MenuShowClipboard", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowClipboard", "Show")]
	[Tooltip("show", "TooltipShowClipboard")]
	[IconSet("show", IconScheme.Colour, "Icons.ShowClipboardToolSmall.png", "Icons.ShowClipboardToolMedium.png", "Icons.ShowClipboardToolLarge.png")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShowClipboardTool : ImageViewerTool
	{
		private static ClipboardComponent _clipboardComponent;
		private static IShelf _shelf;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
		public ShowClipboardTool()
		{
		}

		public void Show()
		{
			if (_clipboardComponent == null)
			{
				_clipboardComponent = new ClipboardComponent();

				_shelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_clipboardComponent,
					SR.TitleClipboard,
					"Clipboard",
					ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);

				_shelf.Closed += OnShelfClosed;
			}
			else
			{
				_shelf.Show();
			}
		}

		private void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			_shelf = null;
			_clipboardComponent = null;
		}

		/// <summary>
		/// Called when the tool is disposed.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (_shelf != null)
				_shelf.Closed -= OnShelfClosed;

			base.Dispose(disposing);
		}

	}
}
