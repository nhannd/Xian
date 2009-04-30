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
using System.Security.Policy;
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Threading;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarOpenStudy", "OpenStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuOpenStudy", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipOpenStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png")]

	[ActionPermission("activate", Common.AuthorityTokens.Workflow.Study.View)]
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
			try
			{
				if (!PermissionsHelper.IsInRole(Common.AuthorityTokens.Workflow.Study.View))
					return;

				if (this.Context.SelectedServerGroup.IsLocalDatastore)
				{
					OpenStudyHelper.OpenStudies(new OpenStudyArgs(GetStudyInstanceUids(), null, "DICOM_LOCAL", ViewerLaunchSettings.WindowBehaviour));
				}
				else
				{
					OpenStudyHelper.OpenStudies(
						new OpenStudyArgs(GetStudyInstanceUids(), this.Context.SelectedStudy.Server, "CC_STREAMING", ViewerLaunchSettings.WindowBehaviour));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private string[] GetStudyInstanceUids()
		{
			string[] uids = new string[this.Context.SelectedStudies.Count];
			for (int i = 0; i < this.Context.SelectedStudies.Count; ++i)
				uids[i] = this.Context.SelectedStudies[i].StudyInstanceUID;

			return uids;
		}

		private void SetDoubleClickHandler()
		{
			if (this.Context.SelectedServerGroup.IsLocalDatastore ||
				this.Context.SelectedServerGroup.IsOnlyStreamingServers())
				this.Context.DefaultActionHandler = OpenStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from the local machine, then we don't
			// even care whether a study has been selected or not
			if (!this.Context.SelectedServerGroup.IsLocalDatastore &&
				!this.Context.SelectedServerGroup.IsOnlyStreamingServers())
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
				if (this.Context.SelectedServerGroup.IsOnlyStreamingServers())
				{
					if (this.Context.SelectedStudy != null)
						this.Enabled = true;
					else
						this.Enabled = false;

					SetDoubleClickHandler();
				}
				else
					this.Enabled = false;
			}
		}

	}
}
