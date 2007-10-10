using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IWorkspaceView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateWorkspaceView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding <see cref="SetTitle"/> to customize the display of the workspace title.
    /// </para>
    /// </remarks>
    public class WorkspaceView : DesktopObjectView, IWorkspaceView
    {
        private Crownwood.DotNetMagic.Controls.TabPage _tabPage;
        private DesktopWindowView _desktopView;
        private Control _control;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="desktopView"></param>
        protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(workspace.Component.GetType());
            componentView.SetComponent((IApplicationComponent)workspace.Component);

            _tabPage = new Crownwood.DotNetMagic.Controls.TabPage();
            _tabPage.Control = _control = componentView.GuiElement as Control;
            _tabPage.Tag = this;

            _desktopView = desktopView;
        }

        /// <summary>
        /// Gets the tab page that hosts this workspace view.
        /// </summary>
        protected internal Crownwood.DotNetMagic.Controls.TabPage TabPage
        {
            get { return _tabPage; }
        }

        #region DesktopObjectView overrides

        /// <summary>
        /// Sets the title of the workspace.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _tabPage.Title = title;
            _tabPage.ToolTip = title;
        }

        /// <summary>
        /// Opens the workspace, adding the tab to the tab group.
        /// </summary>
        public override void Open()
        {
            _desktopView.AddWorkspaceView(this);
        }

        /// <summary>
        /// Activates the workspace, making the tab the selected tab.
        /// </summary>
        public override void Activate()
        {
            _tabPage.Selected = true;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Show()
        {
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Hide()
        {
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tabPage != null)
                {
                    // Remove the tab
                    _desktopView.RemoveWorkspaceView(this);

                    _control.Dispose();
                    _control = null;
                    _tabPage.Dispose();
                    _tabPage = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}
