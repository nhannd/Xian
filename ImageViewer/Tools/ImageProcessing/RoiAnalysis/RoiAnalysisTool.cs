#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
