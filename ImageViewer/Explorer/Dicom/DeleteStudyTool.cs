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
			Uid studyUid = new Uid(this.Context.SelectedStudy.StudyInstanceUID);
			IStudy study = DataAbstractionLayer.GetIDataStore().GetStudy(studyUid);

			try
			{
				DataAbstractionLayer.GetIDataStoreWriteAccessor().RemoveStudy(study);
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
			if (this.Context.LastSearchedServer.Host == "localhost")
				base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnLastSearchedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.LastSearchedServer.Host == "localhost")
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
