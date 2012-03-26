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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

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
					GetServerForStudy(base.Context.SelectedStudy) != null;
		}

		private IServerTreeNode GetServerForStudy(StudyItem studyItem)
		{
			if (base.Context.SelectedServerGroup == null || base.Context.SelectedServerGroup.Servers.Count == 0)
			{
				return null;
			}
			else if (base.Context.SelectedServerGroup.Servers.Count == 1)
			{
				return base.Context.SelectedServerGroup.Servers[0];
			}
			else
			{
				return CollectionUtils.SelectFirst(Context.SelectedServerGroup.Servers,
					delegate(IServerTreeNode server)
					{
                        var theServer = server as IServerTreeDicomServer;
						if (theServer != null)
						{
                            var ae = studyItem.Server as IApplicationEntity;
							if (ae != null)
								return theServer.AETitle == ae.AETitle;
						}

						return false;
					});
			}
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
			UpdateEnabled();

			if (!Enabled)
				return;

			try
			{
				SeriesDetailsComponent component =
					new SeriesDetailsComponent(base.Context.SelectedStudy, GetServerForStudy(base.Context.SelectedStudy));
				ApplicationComponent.LaunchAsDialog(base.Context.DesktopWindow, component, SR.TitleSeriesDetails);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
