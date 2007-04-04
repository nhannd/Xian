using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[MenuAction("show", "global-menus/MenuTools/Standard/LayoutManager")]
    [ButtonAction("show", "global-toolbars/ToolbarStandard/LayoutManager")]
    [ClickHandler("show", "Show")]
    [IconSet("show", IconScheme.Colour, "", "Icons.LayoutMedium.png", "Icons.LayoutLarge.png")]
    [Tooltip("show", "Layout Manager")]
	[GroupHint("show", "Application.Workspace.Layout.Basic")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class LayoutTool : ComponentImageViewerTool
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutTool()
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
        /// Shows the layout component in a shelf.  Only one layout component will ever be shown
        /// at a time, so if there is already a layout component showing, this method does nothing
        /// </summary>
        public void Show()
		{
            // check if a layout component is already displayed
            if (this.ImageViewerToolComponent == null)
            {
                // create and initialize the layout component
				this.ImageViewerToolComponent = new LayoutComponent(GetActiveImageViewer());

                // launch the layout component in a shelf
                // note that the component is thrown away when the shelf is closed by the user
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
					this.ImageViewerToolComponent,
                    SR.TitleLayoutManager,
                    ShelfDisplayHint.DockLeft,// | ShelfDisplayHint.DockAutoHide,
					delegate(IApplicationComponent component) { this.ImageViewerToolComponent = null; });
            }
        }

	}
}
