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
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy", "DeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy", "DeleteStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.DeleteStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDeleteStudy")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]

    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Delete)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
        public void DeleteStudy()
        {
            if (!Enabled || Context.SelectedStudy == null)
                return;

            if (AtLeastOneStudyInUse())
                return;

            if (!ConfirmDeletion())
                return;

            //TODO (Marmot):Restore.
            try
            {
                var client = new DeleteClient();
                foreach (StudyItem study in Context.SelectedStudies)
                {
                    client.DeleteStudy(study);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageFailedToDeleteStudy, Context.DesktopWindow);
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
		    Enabled = (Context.SelectedStudy != null &&
		               Context.SelectedServerGroup.IsLocalServer &&
		               WorkItemActivityMonitor.IsRunning);
		}

		private bool ConfirmDeletion()
		{
		    string message = Context.SelectedStudies.Count == 1 
		                         ? SR.MessageConfirmDeleteStudy 
		                         : String.Format(SR.MessageConfirmDeleteStudies, Context.SelectedStudies.Count);

			DialogBoxAction action = Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);

			if (action == DialogBoxAction.Yes)
				return true;
		    return false;
		}

		// This is a total hack to prevent a user from deleting a study
		// that is currently in use.  The proper way of doing this is
		// to lock the study when it's in use.  But for now, this will do.
		private bool AtLeastOneStudyInUse()
		{
			IEnumerable<StudyItem> studiesInUse = GetStudiesInUse();

			var setStudyUidsInUse = new Dictionary<string, string>();
			foreach (StudyItem item in studiesInUse)
				setStudyUidsInUse[item.StudyInstanceUid] = item.StudyInstanceUid;

			// No studies in use.  Just return.
			if (setStudyUidsInUse.Count == 0)
				return false;

			string message;

			// Notify the user
			if (Context.SelectedStudies.Count == 1)
			{
				message = SR.MessageSelectedStudyInUse;
			}
			else
			{
				message = setStudyUidsInUse.Count == 1 
                    ? SR.MessageOneOfSelectedStudiesInUse 
                    : String.Format(SR.MessageSomeOfSelectedStudiesInUse, setStudyUidsInUse.Count);
			}

			Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);

			return true;
		}

		private IEnumerable<StudyItem> GetStudiesInUse()
		{
			var studiesInUse = new List<StudyItem>();
			IEnumerable<IImageViewer> imageViewers = GetImageViewers();

			foreach (StudyItem study in Context.SelectedStudies)
			{
				foreach (IImageViewer imageViewer in imageViewers)
				{
					if (imageViewer.StudyTree.GetStudy(study.StudyInstanceUid) != null)
						studiesInUse.Add(study);
				}
			}

			return studiesInUse;
		}

		private List<IImageViewer> GetImageViewers()
		{
			var imageViewers = new List<IImageViewer>();

			foreach (Workspace workspace in Context.DesktopWindow.Workspaces)
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
