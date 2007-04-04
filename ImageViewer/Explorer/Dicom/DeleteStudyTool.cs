using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;


namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy")]
	[ClickHandler("activate", "DeleteStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDeleteStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
		public DeleteStudyTool()
		{

		}

		private void DeleteStudy()
		{
			BlockingOperation.Run(this.DeleteStudyInternal);
		}

		private void DeleteStudyInternal()
		{
			if (this.Context.SelectedStudy == null)
				return;

			DialogBoxAction action = Platform.ShowMessageBox(SR.MessageConfirmDeleteStudy, MessageBoxActions.YesNo);

			if (action == DialogBoxAction.No)
				return;

			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				Uid studyInstanceUid = new Uid(item.StudyInstanceUID);
				IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(studyInstanceUid);

				try
				{
					DataAccessLayer.GetIDataStoreWriter().RemoveStudy(study);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.MessageUnableToDeleteStudy, this.Context.DesktopWindow);
				}
			}

			this.Context.RefreshStudyList();
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from the local machine, then we don't
			// even care whether a study has been selected or not
			if (!this.Context.SelectedServerGroup.IsLocalDatastore)
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.
			if (this.Context.SelectedStudy == null)
				return;

			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
