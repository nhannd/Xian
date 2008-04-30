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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	[MenuAction("copyImage", "imageviewer-contextmenu/MenuCopyImageToClipboard", "CopyImage")]
	[IconSet("copyImage", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyImage", "Enabled", "EnabledChanged")]

	[MenuAction("copyDisplaySet", "imageviewer-contextmenu/MenuCopyDisplaySetToClipboard", "CopyDisplaySet")]
	[IconSet("copyDisplaySet", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyDisplaySet", "Enabled", "EnabledChanged")]

	//[MenuAction("copySubset", "imageviewer-contextmenu/MenuCopySubsetToClipboard", "CopySubset")]
	//[IconSet("copySubset", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	//[EnabledStateObserver("copySubset", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CopyToClipboardTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _copyShelves = new Dictionary<IDesktopWindow, IShelf>();

		public CopyToClipboardTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_copyShelves.ContainsKey(this.Context.DesktopWindow))
					return _copyShelves[this.Context.DesktopWindow];

				return null;
			}	
		}

		public void CopyImage()
		{
			try
			{
				BlockingOperation.Run(
					delegate
						{
							Clipboard.Add(this.SelectedPresentationImage);
						});
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void CopyDisplaySet()
		{
			try
			{
				BlockingOperation.Run(
					delegate
						{
							Clipboard.Add(this.SelectedPresentationImage.ParentDisplaySet);
						});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void CopySubset()
		{
			try
			{
				if (ComponentShelf != null)
				{
					ComponentShelf.Activate();
				}
				else
				{
					IDesktopWindow desktopWindow = this.Context.DesktopWindow;

					CopySubsetToClipboardComponent component =
						new CopySubsetToClipboardComponent(desktopWindow);

					_copyShelves[desktopWindow] = ApplicationComponent.LaunchAsShelf(
						desktopWindow,
						component,
						SR.TitleCopySubsetToClipboard,
						ShelfDisplayHint.DockFloat);

					_copyShelves[desktopWindow].Closed +=
						delegate
							{
								_copyShelves.Remove(desktopWindow);
							};
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			base.OnTileSelected(sender, e);

			if (e.SelectedTile.PresentationImage != null)
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
