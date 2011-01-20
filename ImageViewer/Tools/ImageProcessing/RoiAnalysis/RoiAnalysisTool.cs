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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuRoiAnalysis", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarRoiAnalysis", "Show")]
	[IconSet("show", IconScheme.Colour, "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolMedium.png", "Icons.RoiHistogramToolLarge.png")]
	[Tooltip("show", "TooltipRoiAnalysis")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RoiAnalysisTool : ImageViewerTool
	{
		private static RoiAnalysisComponentContainer _roiAnalysisComponent;
		private static IShelf _roiAnalysisShelf;

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
				_roiAnalysisShelf = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
					_roiAnalysisComponent,
                    SR.Title,
                    ShelfDisplayHint.DockLeft);

				_roiAnalysisShelf.Closed += RoiAnalysisShelf_Closed;
            }
        }

		private static void RoiAnalysisShelf_Closed(object sender, ClosedEventArgs e) {
			// note that the component is thrown away when the shelf is closed by the user
			_roiAnalysisShelf.Closed -= RoiAnalysisShelf_Closed;
			_roiAnalysisShelf = null;
			_roiAnalysisComponent = null;
		}

	}
}
