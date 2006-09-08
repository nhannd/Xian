using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("open", "dicomstudybrowser-toolbar/Open")]
	[MenuAction("open", "dicomstudybrowser-contextmenu/Open")]
	[ClickHandler("open", "OpenStudy")]
	[EnabledStateObserver("open", "Enabled", "EnabledChanged")]
	[Tooltip("open", "Open Study")]
	[IconSet("open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : StudyBrowserTool
	{
		public OpenStudyTool()
		{

		}

		public override void Initialize()
		{
			this.Context.DefaultActionHandler = OpenStudy;
			base.Initialize();
		}

		public void OpenStudy()
		{
			if (this.Context.SelectedStudy == null)
				return;

			string studyInstanceUid = this.Context.SelectedStudy.StudyInstanceUID;
			string label = String.Format("{0}, {1} | {2}",
				this.Context.SelectedStudy.LastName,
				this.Context.SelectedStudy.FirstName,
				this.Context.SelectedStudy.PatientId);

			try
			{
				this.Context.StudyLoader.LoadStudy(studyInstanceUid);

			}
			catch (OpenStudyException e)
			{
				if (e.StudyCouldNotBeLoaded)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
					return;
				}

				if (e.AtLeastOneImageFailedToLoad)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorAtLeastOneImageFailedToLoad);
					return;
				}

				return;
			}

			ImageViewerComponent imageViewer = new ImageViewerComponent(studyInstanceUid);
			ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, imageViewer, label, null);
		}
	}
}
