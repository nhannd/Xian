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

		public WorkspaceTabPage(IWorkspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			_workspace = workspace;

            IWorkspaceView view = (IWorkspaceView)ViewFactory.CreateAssociatedView(_workspace.GetType());
            view.SetWorkspace(_workspace);

			this.Control = view.GuiElement as Control;
            this.Title = _workspace.Title;

			_workspace.TitleChanged += new EventHandler(_workspace_TitleChanged);
		}

        void _workspace_TitleChanged(object sender, EventArgs e)
        {
            this.Title = _workspace.Title;
        }

        public IWorkspace Workspace
		{
			get { return _workspace; }
		}
	}
}
