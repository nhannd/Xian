using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;


namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/Delete")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Delete")]
	[ClickHandler("activate", "DeleteStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Delete Study")]
	[IconSet("activate", IconScheme.Colour, "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
		public DeleteStudyTool()
		{

		}

		public void DeleteStudy()
		{
			if (this.Context.SelectedStudy == null)
				return;

			Uid studyUid = new Uid(this.Context.SelectedStudy.StudyInstanceUID);
			IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(studyUid);

			try
			{
				DataAccessLayer.GetIDataStoreWriter().RemoveStudy(study);
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
				Platform.ShowMessageBox("Unable to delete study");
			}

			this.Context.RefreshStudyList();
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from the local machine, then we don't
			// even care whether a study has been selected or not
			if (this.Context.LastSearchedServer.Host != "localhost")
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnLastSearchedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.
			if (this.Context.SelectedStudy == null)
				return;

			if (this.Context.LastSearchedServer.Host == "localhost")
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
