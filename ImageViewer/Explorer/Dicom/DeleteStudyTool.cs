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
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy")]
	[ClickHandler("activate", "DeleteStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDeleteStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
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

			if (AtLeastOneStudyInUse())
				return;

			if (!ConfirmDeletion())
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

			this.Enabled = this.Context.SelectedServerGroup.IsLocalDatastore;
		}

		private bool ConfirmDeletion()
		{
			string message;
			if (this.Context.SelectedStudies.Count == 1)
				message = SR.MessageConfirmDeleteStudy;
			else
				message = String.Format(SR.MessageConfirmDeleteStudies, this.Context.SelectedStudies.Count);

			DialogBoxAction action = this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);

			if (action == DialogBoxAction.Yes)
				return true;
			else
				return false;
		}

		// This is a total hack to prevent a user from deleting a study
		// that is currently in use.  The proper way of doing this is
		// to lock the study when it's in use.  But for now, this will do.
		private bool AtLeastOneStudyInUse()
		{
			IList<StudyItem> studiesInUse = GetStudiesInUse();

			Dictionary<string, string> setStudyUidsInUse = new Dictionary<string, string>();
			foreach (StudyItem item in studiesInUse)
				setStudyUidsInUse[item.StudyInstanceUID] = item.StudyInstanceUID;

			// No studies in use.  Just return.
			if (setStudyUidsInUse.Count == 0)
				return false;

			string message;

			// Notify the user
			if (this.Context.SelectedStudies.Count == 1)
			{
				message = SR.MessageSelectedStudyInUse;
			}
			else
			{
				if (setStudyUidsInUse.Count == 1)
					message = SR.MessageOneOfSelectedStudiesInUse;
				else
					message = String.Format(SR.MessageSomeOfSelectedStudiesInUse, setStudyUidsInUse.Count);
			}

			this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);

			return true;
		}

		private IList<StudyItem> GetStudiesInUse()
		{
			List<StudyItem> studiesInUse = new List<StudyItem>();
			IEnumerable<IImageViewer> imageViewers = GetImageViewers();

			foreach (StudyItem study in this.Context.SelectedStudies)
			{
				foreach (IImageViewer imageViewer in imageViewers)
				{
					if (imageViewer.StudyTree.GetStudy(study.StudyInstanceUID) != null)
						studiesInUse.Add(study);
				}
			}

			return studiesInUse;
		}

		private IEnumerable<IImageViewer> GetImageViewers()
		{
			List<IImageViewer> imageViewers = new List<IImageViewer>();

			foreach (IWorkspace workspace in this.Context.DesktopWindow.WorkspaceManager.Workspaces)
			{
				if (!(workspace is ApplicationComponentHostWorkspace))
					continue;

				IApplicationComponent component = ((ApplicationComponentHostWorkspace)workspace).Component;

				if (component == null)
					continue;

				if (!(component is IImageViewer))
					continue;

				imageViewers.Add(component as IImageViewer);
			}

			return imageViewers;
		}
	}
}
