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
	class WorkspaceTab : Crownwood.DotNetMagic.Controls.TabPage
	{
        private IWorkspace _workspace;

		public WorkspaceTab(IWorkspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			_workspace = workspace;

            IWorkspaceView view = (IWorkspaceView)ViewFactory.CreateAssociatedView(_workspace.GetType());
            view.SetWorkspace(_workspace);

			this.Control = view.GuiElement as Control;
			this.PropertyChanged += new PropChangeHandler(OnPropertyChanged);

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

		void OnPropertyChanged(Crownwood.DotNetMagic.Controls.TabPage page, Crownwood.DotNetMagic.Controls.TabPage.Property prop, object oldValue)
		{
			if (prop == Property.Selected)
			{
				if (page.Selected)
					_workspace.DesktopWindow.WorkspaceManager.ActiveWorkspace = _workspace;
			}
		}
	}
}
