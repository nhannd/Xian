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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	[MenuAction("copyImage", "imageviewer-contextmenu/MenuClipboard/MenuCopyImageToClipboard", "CopyImage")]
	[IconSet("copyImage", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyImage", "CopyImageEnabled", "CopyImageEnabledChanged")]

	[MenuAction("copyDisplaySet", "imageviewer-contextmenu/MenuClipboard/MenuCopyDisplaySetToClipboard", "CopyDisplaySet")]
	[IconSet("copyDisplaySet", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyDisplaySet", "CopyDisplaySetEnabled", "CopyDisplaySetEnabledChanged")]

	[MenuAction("copySubset", "imageviewer-contextmenu/MenuClipboard/MenuCopySubsetToClipboard", "CopySubset")]
	[IconSet("copySubset", IconScheme.Colour, "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copySubset", "CopySubsetEnabled", "CopySubsetEnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CopyToClipboardTool : Tool<IImageViewerToolContext>
	{
		private static IShelf _copySubsetShelf;

		private bool _copyDisplaySetEnabled;
		private bool _copyImageEnabled;
		private bool _copySubsetEnabled;

		private event EventHandler _copyDisplaySetEnabledChanged;
		private event EventHandler _copyImageEnabledChanged;
		private event EventHandler _copySubsetEnabledChanged;

		public CopyToClipboardTool()
		{
			_copyDisplaySetEnabled = false;
			_copyImageEnabled = false;
			_copySubsetEnabled = false;
		}

		public bool CopyDisplaySetEnabled
		{
			get { return _copyDisplaySetEnabled; }	
			set
			{
				if (value == _copyDisplaySetEnabled)
					return;

				_copyDisplaySetEnabled = value;
				EventsHelper.Fire(_copyDisplaySetEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopyDisplaySetEnabledChanged
		{
			add { _copyDisplaySetEnabledChanged += value; }
			remove { _copyDisplaySetEnabledChanged -= value; }
		}

		public bool CopyImageEnabled
		{
			get { return _copyImageEnabled; }
			set
			{
				if (value == _copyImageEnabled)
					return;

				_copyImageEnabled = value;
				EventsHelper.Fire(_copyImageEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopyImageEnabledChanged
		{
			add { _copyImageEnabledChanged += value; }
			remove { _copyImageEnabledChanged -= value; }
		}

		public bool CopySubsetEnabled
		{
			get { return _copySubsetEnabled; }
			set
			{
				if (value == _copySubsetEnabled)
					return;

				_copySubsetEnabled = value;
				EventsHelper.Fire(_copySubsetEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopySubsetEnabledChanged
		{
			add { _copySubsetEnabledChanged += value; }
			remove { _copySubsetEnabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			base.Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void CopyImage()
		{
			try
			{
				BlockingOperation.Run(
					delegate
						{
							Clipboard.Add(this.Context.Viewer.SelectedPresentationImage);
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
							Clipboard.Add(this.Context.Viewer.SelectedPresentationImage.ParentDisplaySet);
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
				CopySubsetToClipboardComponent component;

				if (_copySubsetShelf != null)
				{
					component = (CopySubsetToClipboardComponent)_copySubsetShelf.Component;
					if (component.DesktopWindow != this.Context.DesktopWindow)
					{
						component.Close();
					}
					else
					{
						_copySubsetShelf.Activate();
						return;
					}
				}

				IDesktopWindow desktopWindow = this.Context.DesktopWindow;

				component = new CopySubsetToClipboardComponent(desktopWindow);

				_copySubsetShelf = ApplicationComponent.LaunchAsShelf(
					desktopWindow,
					component,
					SR.TitleCopySubsetToClipboard,
					ShelfDisplayHint.ShowNearMouse);

				_copySubsetShelf.Closed += delegate { _copySubsetShelf = null; };
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			if (e.SelectedImageBox.DisplaySet == null)
				UpdateEnabled(null);
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedDisplaySet);
		}

		private void UpdateEnabled(IDisplaySet selectedDisplaySet)
		{
			if (selectedDisplaySet == null || selectedDisplaySet.PresentationImages.Count < 1)
			{
				CopyDisplaySetEnabled = false;
				CopySubsetEnabled = false;
				CopyImageEnabled = false;
			}
			else if (selectedDisplaySet.PresentationImages.Count == 1)
			{
				CopyDisplaySetEnabled = false;
				CopySubsetEnabled = false;
				CopyImageEnabled = true;
			}
			else
			{
				CopyDisplaySetEnabled = true;
				CopySubsetEnabled = true;
				CopyImageEnabled = true;
			}
		}
	}
}
