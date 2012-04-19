#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[MenuAction("show", "dicomstudybrowser-contextmenu/MenuShowSeriesDetails", "Show")]
	[ButtonAction("show", "dicomstudybrowser-toolbar/ToolbarShowSeriesDetails", "Show")]

	[Tooltip("show", "TooltipSeriesDetails")]
	[IconSet("show", "Icons.ShowSeriesDetailsToolSmall.png", "Icons.ShowSeriesDetailsToolMedium.png", "Icons.ShowSeriesDetailsToolLarge.png")]

	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ShowSeriesDetailsTool : StudyBrowserTool
	{
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
			if (base.Context.SelectedServers == null)
				base.Enabled = false;
			else if (base.Context.SelectedServers.IsLocalServer)
				base.Enabled = StudyStore.IsSupported && base.Context.SelectedStudy != null && base.Context.SelectedStudies.Count == 1;
			else
				base.Enabled = base.Context.SelectedStudy != null && base.Context.SelectedStudies.Count == 1 &&
					GetServerForStudy(base.Context.SelectedStudy) != null;
		}

		private IDicomServiceNode GetServerForStudy(StudyItem studyItem)
		{
			if (base.Context.SelectedServers == null || base.Context.SelectedServers.Count == 0)
			{
				return null;
			}
			else if (base.Context.SelectedServers.Count == 1)
			{
				return base.Context.SelectedServers[0];
			}
			else
			{
			    //TODO (Marmot):
                var ae = studyItem.Server as IApplicationEntity;
                if (ae == null)
                    return null;

			    return Context.SelectedServers.FirstOrDefault(s => s.Name == ae.Name);
			}
		}

		public void Show()
		{
			UpdateEnabled();

			if (!Enabled)
				return;

			try
			{
				var component = new SeriesDetailsComponent(base.Context.SelectedStudy, GetServerForStudy(base.Context.SelectedStudy));
				ApplicationComponent.LaunchAsDialog(base.Context.DesktopWindow, component, SR.TitleSeriesDetails);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
