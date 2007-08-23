using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.Samples.Cad
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuCad", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarCad", "Show")]
	[IconSet("show", IconScheme.Colour, "Icons.CadToolSmall.png", "Icons.CadToolMedium.png", "CadToolLarge.png")]
	[Tooltip("show", "TooltipCad")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CadTool : ImageViewerTool
	{
		private static CadApplicationComponent _cadComponent;

        /// <summary>
        /// Constructor
        /// </summary>
		public CadTool()
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
			if (_cadComponent == null)
            {
                // create and initialize the layout component
				_cadComponent = new CadApplicationComponent(this.Context);
				
                // launch the layout component in a shelf
                // note that the component is thrown away when the shelf is closed by the user
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
					_cadComponent,
                    SR.Title,
                    ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { _cadComponent = null; });
            }
        }
	
	}
}
