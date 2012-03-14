#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy", "DeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy", "DeleteStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDeleteStudy")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]

	[ViewerActionPermission("activate", ImageViewer.Services.AuthorityTokens.Study.Delete)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
		public DeleteStudyTool()
		{

		}

		private void DeleteStudy()
		{
			if (!Enabled || this.Context.SelectedStudy == null)
				return;

			if (AtLeastOneStudyInUse())
				return;

			if (!ConfirmDeletion())
				return;

			List<string> deleteStudies = CollectionUtils.Map<StudyItem, string>(this.Context.SelectedStudies,
                                                    delegate(StudyItem study)
                                                    	{
                                                    		return study.StudyInstanceUid;
                                                    	});

			DeleteInstancesRequest request = new DeleteInstancesRequest();
			request.DeletePriority = DeletePriority.High;
			request.InstanceLevel = InstanceLevel.Study;
			request.InstanceUids = deleteStudies;

			try
			{
				BackgroundTask task = new BackgroundTask(TaskMethod, false, deleteStudies);
				ProgressDialog.Show(task, Application.DesktopWindows.ActiveWindow, true);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToDeleteStudy, this.Context.DesktopWindow);
			}
		}

		private void TaskMethod(IBackgroundTaskContext context)
		{
			DeleteInstancesRequest request = new DeleteInstancesRequest();
			request.DeletePriority = DeletePriority.High;
			request.InstanceLevel = InstanceLevel.Study;
			request.InstanceUids = (List<string>) context.UserState;

			context.ReportProgress(new BackgroundTaskProgress(0, SR.MessageDeletingStudies));

			try
			{
				LocalDataStoreDeletionHelper.DeleteInstancesAndWait(request, 500,
							delegate(LocalDataStoreDeletionHelper.DeletionProgressInformation progress)
							{
								int total = progress.NumberDeleted + progress.NumberRemaining;
								int percent = (int)(progress.NumberDeleted / (float)total * 100F);
								context.ReportProgress(new BackgroundTaskProgress(percent, SR.MessageDeletingStudies));
								return true;
							});

				context.Complete(null);

				AuditedInstances deletedInstances = new AuditedInstances();
				((List<string>) context.UserState).ForEach(delegate(string studyInstanceUid) { deletedInstances.AddInstance(studyInstanceUid); });
				AuditHelper.LogDeleteStudies(AuditHelper.LocalAETitle, deletedInstances, EventSource.CurrentUser, EventResult.Success);
			}
			catch(Exception e)
			{
				context.Error(e);
			}
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			this.Enabled = (this.Context.SelectedStudy != null &&
			                this.Context.SelectedServerGroup.IsLocalDatastore &&
			                LocalDataStoreActivityMonitor.IsConnected);
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
				setStudyUidsInUse[item.StudyInstanceUid] = item.StudyInstanceUid;

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
					if (imageViewer.StudyTree.GetStudy(study.StudyInstanceUid) != null)
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
