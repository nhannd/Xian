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
		private Workspace _workspace;

		public WorkspaceTab(Workspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			_workspace = workspace;
			this.Control = workspace.View.GuiElement as Control;
			this.PropertyChanged += new PropChangeHandler(OnPropertyChanged);
		}

		public Workspace Workspace
		{
			get { return _workspace; }
		}

		void OnPropertyChanged(Crownwood.DotNetMagic.Controls.TabPage page, Crownwood.DotNetMagic.Controls.TabPage.Property prop, object oldValue)
		{
			if (prop == Property.Selected)
			{
				if (page.Selected)
					_workspace.IsActivated = true;
			}
		}
	}
}
