using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.Model.StudyManagement;
using ClearCanvas.Workstation.Dashboard;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Dashboard.Local
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Dashboard.DashboardContentExtensionPoint))]
	public class LocalDashboardContent : DashboardContent
	{
		private MasterViewControl _masterView = new MasterViewControl();
		private DetailViewControl _detailView = new DetailViewControl();
		private bool _firstTimeSelected = true;
		private LocalImageLoader _imageLoader = new LocalImageLoader();

		public LocalDashboardContent()
		{
			_detailView.HeaderText = "Open DICOM Images";
			_detailView.OpenImage += new EventHandler(OnLoadImages);
		}

		public override string Name
		{
			get { return "My Computer"; }
		}

		public override void OnSelected()
		{
			base.MasterView = _masterView;
			base.DetailView = _detailView;

			if (_firstTimeSelected)
			{
				_detailView.Initialize();
				_firstTimeSelected = false;
			}
		}

		private void OnLoadImages(object sender, EventArgs e)
		{
			using (new CursorManager(sender as Control, Cursors.WaitCursor))
			{
				string studyInstanceUID = _imageLoader.Load(_detailView.SelectedPath);

				if (studyInstanceUID == "" ||
					ImageWorkspace.StudyManager.StudyTree.GetStudy(studyInstanceUID) == null)
				{
					Platform.ShowMessageBox(ClearCanvas.Workstation.Model.SR.ErrorUnableToLoadStudy);
					return;
				}

				ImageWorkspace ws = new ImageWorkspace(studyInstanceUID);
				WorkstationModel.WorkspaceManager.Workspaces.Add(ws);
			}
		}
	}
}
