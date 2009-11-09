#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarOpenStudy", "OpenStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuOpenStudy", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipOpenStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png")]

	[ViewerActionPermission("activate", ImageViewer.AuthorityTokens.Study.Open)]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : StudyBrowserTool
	{
		public OpenStudyTool()
		{

		}

		public override void Initialize()
		{
			base.Initialize();

			SetDoubleClickHandler();
		}

		public void OpenStudy()
		{
			try
			{
				int numberOfSelectedStudies = GetNumberOfSelectedStudies();
				if (numberOfSelectedStudies == 0)
					return;

				if (!PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.Study.Open))
				{
					Context.DesktopWindow.ShowMessageBox(SR.MessageOpenStudyPermissionDenied, MessageBoxActions.Ok);
					return;
				}

				int numberOfLoadableStudies = GetNumberOfLoadableStudies();
				if (numberOfLoadableStudies != numberOfSelectedStudies)
				{
					int numberOfNonLoadableStudies = numberOfSelectedStudies - numberOfLoadableStudies;
					string message;
					if (numberOfSelectedStudies == 1)
					{
						message = SR.MessageCannotOpenNonStreamingStudy;
					}
					else
					{
						if (numberOfNonLoadableStudies == 1)
							message = SR.MessageOneNonStreamingStudyCannotBeOpened;
						else 
							message = String.Format(SR.MessageFormatXNonStreamingStudiesCannotBeOpened, numberOfNonLoadableStudies);
					}

					Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
					return;
				}

				OpenStudyHelper helper = new OpenStudyHelper();
				helper.WindowBehaviour = ViewerLaunchSettings.WindowBehaviour;
				helper.AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer;

				if (Context.SelectedServerGroup.IsLocalDatastore)
				{
					foreach (StudyItem study in Context.SelectedStudies)
						helper.AddStudy(study.StudyInstanceUid, study.Server, LocalStudyLoaderName);
				}
				else
				{
					foreach (StudyItem study in Context.SelectedStudies)
					{
						ApplicationEntity server = study.Server as ApplicationEntity;
						if (server != null)
						{
							if (server.IsStreaming)
								helper.AddStudy(study.StudyInstanceUid, study.Server, StreamingStudyLoaderName);
							else
								helper.AddStudy(study.StudyInstanceUid, study.Server, RemoteStudyLoaderName);
						}
					}
				}

				helper.Title = ImageViewerComponent.CreateTitle(GetSelectedPatients());
				helper.OpenStudies();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private void SetDoubleClickHandler()
		{
			if (GetAtLeastOneServerSupportsLoading() || base.Context.SelectedServerGroup.Servers.Count == 0)
				Context.DefaultActionHandler = OpenStudy;
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
			if (Context.SelectedServerGroup.IsLocalDatastore)
			{
				Enabled = Context.SelectedStudy != null;
			}
			else
			{
				if (Context.SelectedStudy != null)
					Enabled = GetAtLeastOneServerSupportsLoading();
				else
					Enabled = false;
			}

			SetDoubleClickHandler();
		}

		private int GetNumberOfSelectedStudies()
		{
			if (Context.SelectedStudy == null)
				return 0;

			return Context.SelectedStudies.Count;
		}

		private bool GetAtLeastOneServerSupportsLoading()
		{
			if (Context.SelectedServerGroup.IsLocalDatastore && base.IsLocalStudyLoaderSupported)
				return true;

			foreach (Server server in base.Context.SelectedServerGroup.Servers)
			{
				if (server.IsStreaming && base.IsStreamingStudyLoaderSupported)
					return true;
				else if (!server.IsStreaming && base.IsRemoteStudyLoaderSupported)
					return true;
			}

			return false;
		}

		private int GetNumberOfLoadableStudies()
		{
			int number = 0;

			if (Context.SelectedStudy != null)
			{
				if (Context.SelectedServerGroup.IsLocalDatastore && IsLocalStudyLoaderSupported)
					return Context.SelectedStudies.Count;

				foreach (StudyItem study in Context.SelectedStudies)
				{
					ApplicationEntity server = study.Server as ApplicationEntity;
					if (server != null)
					{
						if (server.IsStreaming && IsStreamingStudyLoaderSupported)
							++number;
						else if (!server.IsStreaming && IsRemoteStudyLoaderSupported)
							++number;
					}
				}
			}

			return number;
		}

		private IEnumerable<IPatientData> GetSelectedPatients()
		{
			if (base.Context.SelectedStudy != null)
			{
				foreach (StudyItem studyItem in base.Context.SelectedStudies)
					yield return studyItem;
			}
		}
	}
}
