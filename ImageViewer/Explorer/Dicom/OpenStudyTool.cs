using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/Open")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Open")]
	[ClickHandler("activate", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Open Study")]
	[IconSet("activate", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : StudyBrowserTool
	{
		public OpenStudyTool()
		{

		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void OpenStudy()
		{
			if (this.Context.SelectedStudies == null)
				return;

			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				string studyInstanceUid = item.StudyInstanceUID;
				string label = String.Format("{0}, {1} | {2}",
					item.LastName,
					item.FirstName,
					item.PatientId);

				try
				{
					IStudyLoader studyLoader = ImageViewerComponent.StudyManager.StudyLoaders["DICOM_LOCAL"];
					studyLoader.LoadStudy(studyInstanceUid);
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

				ImageViewerComponent imageViewer = new DiagnosticImageViewerComponent(studyInstanceUid);
				ApplicationComponent.LaunchAsWorkspace(
					this.Context.DesktopWindow,
					imageViewer,
					label,
					delegate
					{
						imageViewer.Dispose();
					});
			}
		}

		private void SetDoubleClickHandler()
		{
			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				this.Context.DefaultActionHandler = OpenStudy;
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
			if (this.Context.SelectedServerGroup.IsLocalDatastore)
			{
				if (this.Context.SelectedStudy != null)
					this.Enabled = true;
				else
					this.Enabled = false;

				SetDoubleClickHandler();
			}
			else
			{
				this.Enabled = false;
			}
		}
	}
}
