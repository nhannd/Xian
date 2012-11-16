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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[MenuAction("create", "imageviewer-contextmenu/MenuCreateKeyImage", "Create")]
	[ButtonAction("create", "global-toolbars/ToolbarStandard/ToolbarCreateKeyImage", "Create", KeyStroke = XKeys.Space)]
	[Tooltip("create", "TooltipCreateKeyImage")]
	[IconSet("create", "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
	[EnabledStateObserver("create", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("create", AuthorityTokens.KeyImages)]

	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowKeyImages", "Show")]
	[Tooltip("show", "TooltipShowKeyImages")]
	[IconSet("show", "Icons.ShowKeyImagesToolSmall.png", "Icons.ShowKeyImagesToolMedium.png", "Icons.ShowKeyImagesToolLarge.png")]
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
		private IWorkItemActivityMonitor _workItemActivityMonitor;

		#endregion

		public KeyImageTool()
		{
			_flashOverlayController = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ApplicationThemeResourceResolver(this.GetType(), false));
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

			UpdateEnabled();

		    _workItemActivityMonitor = WorkItemActivityMonitor.Create();
            _workItemActivityMonitor.IsConnectedChanged += OnIsConnectedChanged;
		}

	    protected override void Dispose(bool disposing)
		{
            _workItemActivityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
			_workItemActivityMonitor.Dispose();

			if (base.Context != null)
				KeyImageClipboard.OnViewerClosed(base.Context.Viewer);

			base.Dispose(disposing);
		}

		private void UpdateEnabled()
		{
            // TODO  Better way to address Webstation usage?
			base.Enabled = KeyImagePublisher.IsSupportedImage(base.SelectedPresentationImage) &&
					  PermissionsHelper.IsInRole(AuthorityTokens.KeyImages) &&
                      !(SelectedPresentationImage.ParentDisplaySet.Descriptor is KeyImageDisplaySetDescriptor) &&
                      (WorkItemActivityMonitor.IsRunning || !KeyImageClipboard.HasViewPlugin());

            this.ShowEnabled = WorkItemActivityMonitor.IsRunning &&
					  PermissionsHelper.IsInRole(AuthorityTokens.KeyImages);
		}

        private void OnIsConnectedChanged(object sender, EventArgs eventArgs)
        {
            UpdateEnabled();
        }

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled();

            /*
            if (!KeyImageClipboard.HasViewPlugin())
            {
                if (SelectedPresentationImage.ParentDisplaySet.Descriptor is KeyImageDisplaySetDescriptor)
                {
                    foreach (ClearCanvas.Desktop.Actions.Action a in this.Actions)
                    {
                        if (a.Path.LocalizedPath.Equals("imageviewer-contextmenu/MenuCreateKeyImage")
                          | a.Path.LocalizedPath.Equals("global-toolbars/ToolbarStandard/Create Key Image"))
                        {
                            a.IconSet = new IconSet("Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png");
                        }
                    }
                }
                else
                {
                    foreach (ClearCanvas.Desktop.Actions.Action a in this.Actions)
                    {
                        if (a.Path.LocalizedPath.Equals("imageviewer-contextmenu/MenuCreateKeyImage")
                         || a.Path.LocalizedPath.Equals("global-toolbars/ToolbarStandard/Create Key Image"))
                        {
                            a.IconSet = new IconSet("Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png");
                        }
                    }
                }
            }
            */
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
				KeyImageClipboard.Show(Context.DesktopWindow);
		}

		public void Create()
		{
			if (!base.Enabled)
				return;

		    if (KeyImageClipboard.HasViewPlugin())
		    {
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
		                KeyImageClipboard.Show(Context.DesktopWindow,
		                                       ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft);
		                _firstKeyImageCreation = false;
		            }
		        }
		        catch (Exception ex)
		        {
		            Platform.Log(LogLevel.Error, ex, "Failed to add item to the key image clipboard.");
		            ExceptionHandler.Report(ex, SR.MessageCreateKeyImageFailed, base.Context.DesktopWindow);
		        }
		    }
		    else
		    {
                try
                {
                    IPresentationImage image = base.Context.Viewer.SelectedPresentationImage;
                    if (image != null)
                    {
                        // New Virtual Display Set
                        KeyImageDisplaySet.AddKeyImage(image);

                        _flashOverlayController.Flash(image);
                    }

                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Failed to create virtual display set for key image.");
                    ExceptionHandler.Report(ex, SR.MessageCreateKeyImageFailed, base.Context.DesktopWindow);
                }
            }
		}
		
		#endregion
	}
}
