#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[MenuAction("create", "imageviewer-contextmenu/MenuCreateKeyImage", "Create")]
	[ButtonAction("create", "global-toolbars/ToolbarStandard/ToolbarCreateKeyImage", "Create", KeyStroke = XKeys.Space)]
	[Tooltip("create", "TooltipCreateKeyImage")]
	[IconSet("create", IconScheme.Colour, "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
	[EnabledStateObserver("create", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("create", AuthorityTokens.KeyImages)]

	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowKeyImages", "Show")]
	[Tooltip("show", "TooltipShowKeyImages")]
	[IconSet("show", IconScheme.Colour, "Icons.ShowKeyImagesToolSmall.png", "Icons.ShowKeyImagesToolMedium.png", "Icons.ShowKeyImagesToolLarge.png")]
	[EnabledStateObserver("show", "ShowEnabled", "ShowEnabledChanged")]
	[ViewerActionPermission("show", AuthorityTokens.KeyImages)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	internal class KeyImageTool : ImageViewerTool
	{
		#region Private Fields

		private readonly FlashOverlayController _flashOverlayController;
		private bool _showEnabled;
		private bool _firstKeyImageCreation = true;
		private event EventHandler _showEnabledChanged;
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		#endregion

		public KeyImageTool()
		{
			_flashOverlayController = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ResourceResolver(this.GetType(), false));
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
			KeyImageClipboard.OnViewerOpened(base.Context.Viewer);

			base.Enabled = LocalDataStoreActivityMonitor.IsConnected &&
				PermissionsHelper.IsInRole(AuthorityTokens.KeyImages);

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.Connected += this.OnConnected;
			_localDataStoreEventBroker.LostConnection += this.OnLostConnection;
		}

		protected override void Dispose(bool disposing)
		{
			_localDataStoreEventBroker.Connected -= OnConnected;
			_localDataStoreEventBroker.LostConnection -= OnLostConnection;
			_localDataStoreEventBroker.Dispose();

			if (base.Context != null)
				KeyImageClipboard.OnViewerClosed(base.Context.Viewer);

			base.Dispose(disposing);
		}

		private void UpdateEnabled()
		{
			base.Enabled = KeyImagePublisher.IsSupportedImage(base.SelectedPresentationImage) &&
			          LocalDataStoreActivityMonitor.IsConnected &&
					  PermissionsHelper.IsInRole(AuthorityTokens.KeyImages);

			this.ShowEnabled = LocalDataStoreActivityMonitor.IsConnected &&
					  PermissionsHelper.IsInRole(AuthorityTokens.KeyImages);
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
			if (this.ShowEnabled)
				KeyImageClipboard.Show();
		}

		public void Create()
		{
			if (!base.Enabled)
				return;

			try
			{
				IPresentationImage image = base.Context.Viewer.SelectedPresentationImage;
				if (image != null)
				{
					KeyImageClipboard.Add(image);
					_flashOverlayController.Flash(image);
				}

				if (_firstKeyImageCreation && this.ShowEnabled)
				{
					KeyImageClipboard.Show(ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft);
					_firstKeyImageCreation = false;
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
