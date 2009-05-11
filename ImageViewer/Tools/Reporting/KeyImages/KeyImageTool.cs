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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[MenuAction("create", "imageviewer-contextmenu/MenuCreateKeyImage", "Create")]
	[ButtonAction("create", "global-toolbars/ToolbarStandard/ToolbarCreateKeyImage", "Create")]
	[KeyboardAction("create", "imageviewer-keyboard/CreateKeyImage", "Create", KeyStroke = XKeys.Space)]
	[Tooltip("create", "TooltipCreateKeyImage")]
	[IconSet("create", IconScheme.Colour, "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
	[EnabledStateObserver("create", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("create", Common.AuthorityTokens.Workflow.Study.Modify)]

	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowKeyImages", "Show")]
	[Tooltip("show", "TooltipShowKeyImages")]
	[IconSet("show", IconScheme.Colour, "Icons.ShowKeyImagesToolSmall.png", "Icons.ShowKeyImagesToolMedium.png", "Icons.ShowKeyImagesToolLarge.png")]
	[EnabledStateObserver("show", "ShowEnabled", "ShowEnabledChanged")]
	[ViewerActionPermission("show", Common.AuthorityTokens.Workflow.Study.Modify)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	internal class KeyImageTool : ImageViewerTool
	{
		#region Private Fields

		private readonly FlashOverlayController _flashOverlayController;
		private bool _enabled;
		private event EventHandler _enabledChanged;
		private bool _showEnabled;
		private event EventHandler _showEnabledChanged;
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		#endregion

		public KeyImageTool()
		{
			_flashOverlayController = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ResourceResolver(this.GetType(), false));
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public bool ShowEnabled
		{
			get { return _showEnabled; }
			set
			{
				if (_showEnabled == value)
					return;

				_showEnabled = value;
				EventsHelper.Fire(_showEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler ShowEnabledChanged
		{
			add { _showEnabledChanged += value; }
			remove { _showEnabledChanged -= value; }
		}
	
		#region Overrides

		public override void Initialize()
		{
			base.Initialize();
			KeyImageClipboard.OnViewerOpened(Context.Viewer);

			Enabled = LocalDataStoreActivityMonitor.IsConnected && 
				PermissionsHelper.IsInRole(Common.AuthorityTokens.Workflow.Study.Modify);

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.Connected += OnConnected;
			_localDataStoreEventBroker.LostConnection += OnLostConnection;
		}

		protected override void Dispose(bool disposing)
		{
			_localDataStoreEventBroker.Connected -= OnConnected;
			_localDataStoreEventBroker.LostConnection -= OnLostConnection;
			_localDataStoreEventBroker.Dispose();

			if (Context != null)
				KeyImageClipboard.OnViewerClosed(Context.Viewer);

			base.Dispose(disposing);
		}

		private void UpdateEnabled()
		{
			Enabled = base.SelectedPresentationImage is IImageSopProvider &&
					((IImageSopProvider)base.SelectedPresentationImage).ImageSop.DataSource.IsStored &&
			          LocalDataStoreActivityMonitor.IsConnected &&
					  PermissionsHelper.IsInRole(Common.AuthorityTokens.Workflow.Study.Modify);

			ShowEnabled = LocalDataStoreActivityMonitor.IsConnected;
		}

		private void OnConnected(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void OnLostConnection(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		#endregion

		#region Methods

		public void Show()
		{
			if (ShowEnabled)
				KeyImageClipboard.Show();
		}

		public void Create()
		{
			if (!Enabled)
				return;

			try
			{
				IPresentationImage image = Context.Viewer.SelectedPresentationImage;
				if (image != null)
				{
					KeyImageClipboard.Add(image);
					_flashOverlayController.Flash(image);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to add item to the key image clipboard.");
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}
		
		#endregion
	}
}
