using System;
using System.Drawing;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[MenuAction("show", "global-menus/MenuTools/MenuToolsMyTools/DynamicTE", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarMyTools/DynamicTE", "Show")]
	[Tooltip("show", "DynamicTE")]
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
				_cadComponent = new DynamicTeComponent(this.Context);

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
