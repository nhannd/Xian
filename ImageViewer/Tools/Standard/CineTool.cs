#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuCine", "Activate")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardCine", "Activate", KeyStroke = XKeys.C)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarCine", "Activate")]
	[Tooltip("activate", "TooltipCine")]
	[IconSet("activate", IconScheme.Colour, "Icons.CineToolSmall.png", "Icons.CineToolMedium.png", "Icons.CineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Cine")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CineTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves;

		static CineTool()
		{
			_shelves = new Dictionary<IDesktopWindow, IShelf>();
		}

		public CineTool()
		{
		}

		public void Activate()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			// check if a layout component is already displayed
			if (_shelves.ContainsKey(desktopWindow))
			{
				_shelves[desktopWindow].Activate();
			}
			else
			{
				CineApplicationComponent component = new CineApplicationComponent(desktopWindow);
				IShelf shelf = ApplicationComponent.LaunchAsShelf(
					desktopWindow,
					component, SR.TitleCine,
					"Cine",
					ShelfDisplayHint.DockFloat);

				_shelves[desktopWindow] = shelf;
				_shelves[desktopWindow].Closed += OnShelfClosed;
			}
		}

		void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			_shelves.Remove(this.Context.DesktopWindow);
		}

		protected override void Dispose(bool disposing)
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			if (_shelves.ContainsKey(desktopWindow))
				_shelves[desktopWindow].Closed -= OnShelfClosed;

			base.Dispose(disposing);
		}
	}
}