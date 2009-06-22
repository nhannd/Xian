#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyLocator;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[MenuAction("show", "dicomstudybrowser-contextmenu/MenuShowSeriesDetails", "Show")]
	[ButtonAction("show", "dicomstudybrowser-toolbar/ToolbarShowSeriesDetails", "Show")]

	[Tooltip("show", "TooltipSeriesDetails")]
	[IconSet("show", IconScheme.Colour, "Icons.ShowSeriesDetailsToolSmall.png", "Icons.ShowSeriesDetailsToolMedium.png", "Icons.ShowSeriesDetailsToolLarge.png")]

	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ViewerActionPermission("show", ImageViewer.Common.AuthorityTokens.Workflow.Study.Search)]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ShowSeriesDetailsTool : StudyBrowserTool
	{
		public ShowSeriesDetailsTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, System.EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedStudyChanged(object sender, System.EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			if (base.Context.SelectedServerGroup == null)
				base.Enabled = false;
			else if (base.Context.SelectedServerGroup.IsLocalDatastore)
				base.Enabled = CanUseLocal() && base.Context.SelectedStudy != null && base.Context.SelectedStudies.Count == 1;
			else
				base.Enabled = base.Context.SelectedStudy != null && base.Context.SelectedStudies.Count == 1 &&
					base.Context.SelectedServerGroup.Servers.Count == 1 &&
					base.Context.SelectedServerGroup.Servers[0] != null && base.Context.SelectedServerGroup.Servers[0].IsServer;
		}

		private static bool? _canUseLocal;

		public static bool CanUseLocal()
		{
			if (_canUseLocal.HasValue)
				return _canUseLocal.Value;

			try
			{
				IStudyRootQuery query = (IStudyRootQuery)new LocalStudyRootQueryExtensionPoint().CreateExtension();
				_canUseLocal = true;
			}
			catch(NotSupportedException)
			{
				_canUseLocal = false;
			}

			return _canUseLocal.Value;
		}

		public void Show()
		{
			try
			{
				ShowSeriesDetails();
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private void ShowSeriesDetails()
		{
			UpdateEnabled();

			if (!Enabled)
				return;

			try
			{
				SeriesDetailsComponent component =
					new SeriesDetailsComponent(base.Context.SelectedStudy, base.Context.SelectedServerGroup.Servers[0]);
				ApplicationComponent.LaunchAsDialog(base.Context.DesktopWindow, component, SR.TitleSeriesDetails);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
