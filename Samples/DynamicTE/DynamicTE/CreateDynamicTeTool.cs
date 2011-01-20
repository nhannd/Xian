#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[MenuAction("show", "global-menus/MenuTools/MyTools/CreateDynamicTE", "Show")]
	[ButtonAction("show", "global-toolbars/MyTools/CreateDynamicTE", "Show")]
	[Tooltip("show", "CreateDynamicTE")]
	[IconSet("show", IconScheme.Colour, "Icons.CreateDynamicTeToolSmall.png", "Icons.CreateDynamicTeToolMedium.png", "Icons.CreateDynamicTeToolLarge.png")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CreateDynamicTeTool : ImageViewerTool
	{
		private static DynamicTeComponent _cadComponent;

		public CreateDynamicTeTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
		}

		public void Show()
		{
			// check if a layout component is already displayed
			if (_cadComponent == null)
			{
				// create and initialize the layout component
				_cadComponent = new DynamicTeComponent(this.Context.DesktopWindow);

				// launch the layout component in a shelf
				// note that the component is thrown away when the shelf is closed by the user
				ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_cadComponent,
					"TE",
					ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { _cadComponent = null; });
			}
		}

	}
}
