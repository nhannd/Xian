using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuRoiAnalysis")]
    [ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarRoiAnalysis")]
    [ClickHandler("show", "Show")]
	[IconSet("show", IconScheme.Colour, "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolMedium.png", "Icons.RoiHistogramToolLarge.png")]
	[Tooltip("show", "TooltipROIAnalysis")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RoiAnalysisTool : ImageViewerTool
	{
		private static RoiAnalysisComponentContainer _roiAnalysisComponent;

        /// <summary>
        /// Constructor
        /// </summary>
		public RoiAnalysisTool()
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
			if (_roiAnalysisComponent == null)
            {
                // create and initialize the layout component
				_roiAnalysisComponent = new RoiAnalysisComponentContainer(this.Context);
				
                // launch the layout component in a shelf
                // note that the component is thrown away when the shelf is closed by the user
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
					_roiAnalysisComponent,
                    SR.Title,
                    ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { _roiAnalysisComponent = null; });
            }
        }

	}
}
