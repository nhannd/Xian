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
			if (studyInstanceUID == "" ||
				ImageWorkspace.StudyManager.StudyTree.GetStudy(studyInstanceUID) == null)
			{
				Platform.ShowMessageBox(SR.ErrorUnableToLoadStudy);
				return;
			}

			ImageWorkspace ws = new ImageWorkspace(studyInstanceUID);
			DesktopApplication.WorkspaceManager.Workspaces.Add(ws);
		}
	}
}
