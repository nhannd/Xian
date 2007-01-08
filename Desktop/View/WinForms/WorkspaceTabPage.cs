using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	class WorkspaceTabPage : Crownwood.DotNetMagic.Controls.TabPage
	{
        private IWorkspace _workspace;
        private Control _control;

		public WorkspaceTabPage(IWorkspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			_workspace = workspace;

            IWorkspaceView view = (IWorkspaceView)ViewFactory.CreateAssociatedView(_workspace.GetType());
            view.SetWorkspace(_workspace);

			this.Control = _control = view.GuiElement as Control;

			SetTitle();
			
			_workspace.TitleChanged += new EventHandler(_workspace_TitleChanged);
		}

        public IWorkspace Workspace
        {
            get { return _workspace; }
        }

		private void _workspace_TitleChanged(object sender, EventArgs e)
        {
			SetTitle();
		}

		private void SetTitle()
		{
			this.Title = _workspace.Title;
			this.ToolTip = this.Title;
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _control != null)
            {
                _control.Dispose();
                _control = null;
            }
            base.Dispose(disposing);
        }
	}
}
