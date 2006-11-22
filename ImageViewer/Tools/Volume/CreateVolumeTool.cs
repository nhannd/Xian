using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	[MenuAction("show", "global-menus/MenuTools/MenuToolsMyTools/CreateVolumeTool")]
	[ButtonAction("show", "global-toolbars/ToolbarMyTools/CreateVolumeTool")]
	[Tooltip("show", "Create Volume")]
	[IconSet("show", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolMedium.png", "Icons.CreateVolumeToolLarge.png")]
	[ClickHandler("show", "Show")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class CreateVolumeTool : ImageViewerDesktopTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public CreateVolumeTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		public void Show()
		{
			if (this.ImageViewerToolComponent == null)
			{
				// create and initialize the layout component
				this.ImageViewerToolComponent = new VolumeComponent(GetSubjectImageViewer());

				// launch the layout component in a shelf
				// note that the component is thrown away when the shelf is closed by the user
				ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					this.ImageViewerToolComponent,
					"Volume Controller",
					ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { this.ImageViewerToolComponent = null; });
			}


			//IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			//VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);
			//IDisplaySet displaySet = new DisplaySet();
			//displaySet.Name = String.Format("{0} (3D)", selectedDisplaySet.Name);
			//displaySet.PresentationImages.Add(image);
			//this.ImageViewer.LogicalWorkspace.DisplaySets.Add(displaySet);
			//IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			//imageBox.DisplaySet = displaySet;
			//imageBox.Draw();
		}
	}
}
