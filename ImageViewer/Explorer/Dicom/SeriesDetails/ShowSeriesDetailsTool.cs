#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[MenuAction("show", "dicomstudybrowser-contextmenu/MenuShowSeriesDetails", "Show")]
	[ButtonAction("show", "dicomstudybrowser-toolbar/ToolbarShowSeriesDetails", "Show")]

	[Tooltip("show", "TooltipSeriesDetails")]
	[IconSet("show", "Icons.ShowSeriesDetailsToolSmall.png", "Icons.ShowSeriesDetailsToolMedium.png", "Icons.ShowSeriesDetailsToolLarge.png")]

	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("show", "Visible", "VisibleChanged")]

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
            Visible = Context.SelectedServers.AnySupport<IStudyRootQuery>();

			if (Context.SelectedServers == null)
			{
			    Enabled = false;
			}
            else if (Context.SelectedStudy == null || Context.SelectedStudies.Count > 1)
            {
                Enabled = false;
            }
            else
            {
                Enabled = Context.SelectedStudy.Server != null &&
                          Context.SelectedStudy.Server.IsSupported<IStudyRootQuery>();
            }
        }

		public void Show()
		{
			UpdateEnabled();

			if (!Enabled)
				return;

			try
			{
				var component = new SeriesDetailsComponent(Context.SelectedStudy);
				ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, component, SR.TitleSeriesDetails);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
