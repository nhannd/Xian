#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarOpenStudy", "OpenStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuOpenStudy", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipOpenStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png")]
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
			// Okay, the method name is deceptive--it doesn't actually
			// open priors yet
			DiagnosticImageViewerComponent imageViewer = new DiagnosticImageViewerComponent();
			StudyItem item = this.Context.SelectedStudy;
			string studyInstanceUid = item.StudyInstanceUID;

			try
			{
				imageViewer.LoadStudy(studyInstanceUid, "DICOM_LOCAL");
			}
			catch (OpenStudyException e)
			{
				if (e.SuccessfulImages == 0 || e.FailedImages > 0)
					ExceptionHandler.Report(e, this.Context.DesktopWindow);

				if (e.SuccessfulImages == 0)
					return;
			}

			Launch(imageViewer);
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

			Launch(imageViewer);
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
				}
				catch (OpenStudyException e)
				{
					if (e.SuccessfulImages == 0 || e.FailedImages > 0)
						ExceptionHandler.Report(e, this.Context.DesktopWindow);

					if (e.SuccessfulImages == 0)
						continue;
				}

				Launch(imageViewer);
			}
		}

		private void Launch(ImageViewerComponent imageViewer)
		{
			WindowBehaviour windowBehaviour = (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;

			// Open the images in a separate window
			if (windowBehaviour == WindowBehaviour.Separate)
				ImageViewerComponent.LaunchInSeparateWindow(imageViewer);
			// Open the images in the same window
			else
				ImageViewerComponent.LaunchInActiveWindow(imageViewer);
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
