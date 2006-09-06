using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [MenuAction("show", "global-menus/MenuLayout/MenuLayoutLayoutManager")]
    [ButtonAction("show", "global-toolbars/ToolbarStandard/MenuLayoutLayoutManager")]
    [ClickHandler("show", "Show")]
    [IconSet("show", IconScheme.Colour, "", "Icons.LayoutMedium.png", "Icons.LayoutLarge.png")]
    [Tooltip("show", "MenuLayoutLayoutManager")]

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class LayoutTool : Tool<IDesktopToolContext>
	{
        private LayoutComponent _layoutComponent;

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

            this.Context.DesktopWindow.WorkspaceManager.ActiveWorkspaceChanged += WorkspaceActivatedEventHandler;
        }

        /// <summary>
        /// Shows the layout component in a shelf.  Only one layout component will ever be shown
        /// at a time, so if there is already a layout component showing, this method does nothing
        /// </summary>
        public void Show()
		{
            // check if a layout component is already displayed
            if (_layoutComponent == null)
            {
                // create and initialize the layout component
                _layoutComponent = new LayoutComponent();
                _layoutComponent.Subject = GetSubjectImageViewer();

                // launch the layout component in a shelf
                // note that the component is thrown away when the shelf is closed by the user
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    _layoutComponent,
                    SR.MenuLayoutLayoutManager,
                    ShelfDisplayHint.DockLeft,// | ShelfDisplayHint.DockAutoHide,
                    delegate(IApplicationComponent component) { _layoutComponent = null; });
            }
        }

        /// <summary>
        /// Associate the layout component with the active workspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkspaceActivatedEventHandler(object sender, WorkspaceActivationChangedEventArgs e)
        {
            if (_layoutComponent != null)
            {
                _layoutComponent.Subject = GetSubjectImageViewer();
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="IImageViewer"/> hosted by the active workspace,
        /// if it exists, otherwise null.
        /// </summary>
        /// <returns></returns>
        private IImageViewer GetSubjectImageViewer()
        {
            IWorkspace workspace = this.Context.DesktopWindow.ActiveWorkspace;
            if(workspace is ApplicationComponentHostWorkspace
                && ((ApplicationComponentHostWorkspace)workspace).Component is IImageViewer)
            {
                return (IImageViewer)((ApplicationComponentHostWorkspace)workspace).Component;
            }
            return null;
        }
	}
}
