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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuLayoutManager", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarLayoutManager", "Show")]
	[Desktop.Actions.IconSet("show", IconScheme.Colour, "Icons.LayoutToolSmall.png", "Icons.LayoutToolMedium.png", "Icons.LayoutToolLarge.png")]
    [Tooltip("show", "Layout Manager")]
	[GroupHint("show", "Application.Workspace.Layout.Basic")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class LayoutTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		private IDesktopWindow _desktopWindow;

		/// <summary>
        /// Constructor
        /// </summary>
        public LayoutTool()
		{
			_desktopWindow = null;
		}

		public void Show()
		{
			// check if a layout component is already displayed
			if (_shelves.ContainsKey(this.Context.DesktopWindow))
			{
				_shelves[this.Context.DesktopWindow].Activate();
			}
			else
			{
				_desktopWindow = this.Context.DesktopWindow;

				LayoutComponent layoutComponent = new LayoutComponent(_desktopWindow);

            	IShelf shelf = ApplicationComponent.LaunchAsShelf(
					_desktopWindow,
            		layoutComponent,
            		SR.TitleLayoutManager,
            		"Layout",
            		ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);
				_shelves[_desktopWindow] = shelf;

				_shelves[_desktopWindow].Closed += OnShelfClosed;
            }
        }

		private void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			// We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
			// ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
			// exist at the DesktopWindow level and there can only be one of each type of shelf
			// open at the same time per DesktopWindow (otherwise things look funny).  Because of 
			// this, we need to allow this event handling method to be called after this tool has
			// already been disposed (e.g. viewer workspace closed), which is why we store the 
			// _desktopWindow variable.

			_shelves[_desktopWindow].Closed -= OnShelfClosed;
			_shelves.Remove(_desktopWindow);
			_desktopWindow = null;
		}
	}
}
