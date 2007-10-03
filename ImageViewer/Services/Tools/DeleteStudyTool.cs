using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy", "DeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy", "DeleteStudy")]
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

			List<string> deleteStudies = new List<string>();
			foreach(StudyItem item in this.Context.SelectedStudies)
				deleteStudies.Add(item.StudyInstanceUID);

			DeleteInstancesRequest request = new DeleteInstancesRequest();
			request.DeletePriority = DeletePriority.High;
			request.InstanceLevel = InstanceLevel.Study;
			request.InstanceUids = deleteStudies;

			try
			{
				LocalDataStoreDeletionHelper.DeleteInstancesAndWait(request, 1000,
					delegate(LocalDataStoreDeletionHelper.DeletionProgressInformation progressInformation)
					{
						//wait as long as it takes.
						return true;
					});
			}
			catch (EndpointNotFoundException)
			{
				Platform.ShowMessageBox(SR.MessageDeleteLocalDataStoreServiceNotRunning);
			}
			catch (LocalDataStoreDeletionHelper.ConnectionLostException e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToStartDelete, this.Context.DesktopWindow);
			}
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

			foreach (Workspace workspace in this.Context.DesktopWindow.Workspaces)
			{
                IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (viewer == null)
					continue;

                imageViewers.Add(viewer);
			}

			return imageViewers;
		}
	}
}
