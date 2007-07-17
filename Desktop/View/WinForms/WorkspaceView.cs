using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using Crownwood.DotNetMagic.Controls;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class WorkspaceView : DesktopObjectView, IWorkspaceView
    {
        private Crownwood.DotNetMagic.Controls.TabPage _tabPage;
        private DesktopWindowView _desktopView;
        private Control _control;

        protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(workspace.Component.GetType());
            componentView.SetComponent((IApplicationComponent)workspace.Component);

            _tabPage = new Crownwood.DotNetMagic.Controls.TabPage();
            _tabPage.Control = _control = componentView.GuiElement as Control;
            _tabPage.Tag = this;

            _desktopView = desktopView;
        }

        protected internal Crownwood.DotNetMagic.Controls.TabPage TabPage
        {
            get { return _tabPage; }
        }

        #region DesktopObjectView overrides

        public override void SetTitle(string title)
        {
            _tabPage.Title = title;
            _tabPage.ToolTip = title;
        }

        public override void Open()
        {
            _desktopView.AddWorkspaceView(this);
        }

        public override void Activate()
        {
            _tabPage.Selected = true;
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
        }

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
