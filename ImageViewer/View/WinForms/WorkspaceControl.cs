using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public abstract class WorkspaceControl : UserControl
	{
		private Workspace _workspace;

		public WorkspaceControl(Workspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			_workspace = workspace;
		}

		public Workspace Workspace
		{
			get { return _workspace; }
		}
	}
}
