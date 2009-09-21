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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuCine", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarCine", "Activate", KeyStroke = XKeys.C)]
	[Tooltip("activate", "TooltipCine")]
	[IconSet("activate", IconScheme.Colour, "Icons.CineToolSmall.png", "Icons.CineToolMedium.png", "Icons.CineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Cine")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CineTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		private IDesktopWindow _desktopWindow;

		public CineTool()
		{
			_desktopWindow = null;
		}

		public void Activate()
		{
			// check if a layout component is already displayed
			if (_shelves.ContainsKey(this.Context.DesktopWindow))
			{
				_shelves[this.Context.DesktopWindow].Activate();
			}
			else
			{
				_desktopWindow = this.Context.DesktopWindow;

				CineApplicationComponent component = new CineApplicationComponent(_desktopWindow);
				IShelf shelf = ApplicationComponent.LaunchAsShelf(
					_desktopWindow,
					component, 
					SR.TitleCine,
					"Cine",
					ShelfDisplayHint.DockFloat);

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