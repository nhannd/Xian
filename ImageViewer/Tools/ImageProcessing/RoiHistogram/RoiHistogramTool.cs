using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuRoiHistogram")]
    [ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarRoiHistogram")]
    [ClickHandler("show", "Show")]
	[IconSet("show", IconScheme.Colour, "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolMedium.png", "Icons.RoiHistogramToolLarge.png")]
	[Tooltip("show", "ROI Histogram")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RoiHistogramTool : ImageViewerTool
	{
		private static RoiHistogramComponent _roiHistogramComponent;

        /// <summary>
        /// Constructor
        /// </summary>
		public RoiHistogramTool()
		{
        }

        /// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Shows the ROI Histogram component in a shelf.  Only one ROI Histogram component will ever be shown
		/// at a time, so if there is already an ROI Histogram component showing, this method does nothing
        /// </summary>
        public void Show()
		{
            // check if a layout component is already displayed
			if (_roiHistogramComponent == null)
            {
                // create and initialize the layout component
				_roiHistogramComponent = new RoiHistogramComponent(this.Context);

                // launch the layout component in a shelf
                // note that the component is thrown away when the shelf is closed by the user
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
					_roiHistogramComponent,
                    SR.Title,
                    ShelfDisplayHint.DockLeft,// | ShelfDisplayHint.DockAutoHide,
					delegate(IApplicationComponent component) { _roiHistogramComponent = null; });
            }
        }

	}
}
