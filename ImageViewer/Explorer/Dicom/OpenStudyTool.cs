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
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarOpenStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuOpenStudy")]
	[ClickHandler("activate", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipOpenStudy")]
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
			BlockingOperation.Run(this.OpenStudyInternal);
		}
		
		private void OpenStudyInternal()
		{
			if (this.Context.SelectedStudies == null)
				return;

			if (this.Context.SelectedStudies.Count == 1)
			{
				OpenSingleStudyWithPriors();
			}
			else
			{
				OpenMultipleStudiesInSingleWorkspace();
				//OpenMultipleStudiesInIndividualWorkspaces();
			}
		}

		private void OpenSingleStudyWithPriors()
		{
			DiagnosticImageViewerComponent imageViewer = new DiagnosticImageViewerComponent();
			StudyItem item = this.Context.SelectedStudy;
			string studyInstanceUid = item.StudyInstanceUID;

			try
			{
				imageViewer.LoadStudy(studyInstanceUid, "DICOM_LOCAL");
				LaunchWorkspace(imageViewer);
			}
			catch (OpenStudyException e)
			{
				if (e.SuccessfulImages == 0 || e.FailedImages > 0)
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private void OpenMultipleStudiesInSingleWorkspace()
		{
			DiagnosticImageViewerComponent imageViewer = new DiagnosticImageViewerComponent();
			int completelySuccessfulStudies = 0;
			int successfulImagesInLoadFailure = 0;

			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				string studyInstanceUid = item.StudyInstanceUID;

				try
				{
					imageViewer.LoadStudy(studyInstanceUid, "DICOM_LOCAL");
					completelySuccessfulStudies++;
				}
				catch (OpenStudyException e)
				{
					// Study failed to load completely; keep track of how many
					// images in the study actually did load
					successfulImagesInLoadFailure += e.SuccessfulImages;

					if (e.SuccessfulImages == 0 || e.FailedImages > 0)
						ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}

			// If nothing at all was able to load, then don't bother trying to
			// even open a workspace; just return
			if (completelySuccessfulStudies == 0 && successfulImagesInLoadFailure == 0)
				return;

			LaunchWorkspace(imageViewer);
		}

		private void OpenMultipleStudiesInIndividualWorkspaces()
		{
			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				DiagnosticImageViewerComponent imageViewer = new DiagnosticImageViewerComponent();
				string studyInstanceUid = item.StudyInstanceUID;

				try
				{
					imageViewer.LoadStudy(studyInstanceUid, "DICOM_LOCAL");
					LaunchWorkspace(imageViewer);
				}
				catch (OpenStudyException e)
				{
					if (e.SuccessfulImages == 0 || e.FailedImages > 0)
						ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
		}

		private void LaunchWorkspace(DiagnosticImageViewerComponent imageViewer)
		{
			ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				imageViewer,
				imageViewer.PatientsLoadedLabel,
				delegate
				{
					imageViewer.Dispose();
				});

			imageViewer.Layout();
			imageViewer.PhysicalWorkspace.SelectDefaultImageBox();
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
