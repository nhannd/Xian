using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StudyLoader : IStudyLoader
	{
		public StudyLoader()
		{
		}

		public abstract string Name
		{
			get;
		}

		public virtual void LoadStudy(string studyInstanceUID)
		{
            /*
             * 
             * Showing a message box here isn't a good idea
            if (studyInstanceUID == "" ||
                ImageViewerComponent.StudyManager.StudyTree.GetStudy(studyInstanceUID) == null)
			{
				Platform.ShowMessageBox(SR.ErrorUnableToLoadStudy);
				return;
			}

             * 
             * This responsibility should be moved out of here
			ImageWorkspace ws = new ImageWorkspace(studyInstanceUID);
			DesktopApplication.WorkspaceManager.Workspaces.Add(ws);
             */
		}
	}
}
