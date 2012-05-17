#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("Open", "dicomstudybrowser-toolbar/ToolbarFilterStudy", "Open")]
	[MenuAction("Open", "dicomstudybrowser-contextmenu/MenuFilterStudy", "Open")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("Open", "Visible", "VisibleChanged")]
	[Tooltip("Open", "TooltipFilterStudy")]
	[IconSet("Open", "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (StudyBrowserToolExtensionPoint))]
	public class LaunchStudyFiltersDicomExplorerTool : StudyBrowserTool
	{
		public void Open()
		{
			int sopCount = 0;
			List<IStudyLoader> studyLoaders = new List<IStudyLoader>();
			foreach (StudyManagement.StudyItem studyItem in base.Context.SelectedStudies)
			{
				IStudyLoader localStudyLoader = CreateLoader();
				if (localStudyLoader == null)
					return;

				studyLoaders.Add(localStudyLoader);
				sopCount += localStudyLoader.Start(new StudyLoaderArgs(studyItem.StudyInstanceUid, studyItem.Server));
			}

			bool success = false;
			StudyFilterComponent component = new StudyFilterComponent();
			component.BulkOperationsMode = true;
			BackgroundTask task = new BackgroundTask(c =>
			                                         	{
			                                         		c.ReportProgress(new BackgroundTaskProgress(0, sopCount, SR.MessageLoading));
			                                         		if (c.CancelRequested)
			                                         			c.Cancel();

			                                         		int progress = 0;
			                                         		foreach (IStudyLoader localStudyLoader in studyLoaders)
			                                         		{
			                                         			Sop sop;
			                                         			while ((sop = localStudyLoader.LoadNextSop()) != null)
			                                         			{
			                                         				component.Items.Add(new SopDataSourceStudyItem(sop));
																	c.ReportProgress(new BackgroundTaskProgress(Math.Min(sopCount, ++progress) - 1, sopCount, SR.MessageLoading));
			                                         				if (c.CancelRequested)
			                                         					c.Cancel();
			                                         				sop.Dispose();
			                                         			}
			                                         		}

			                                         		success = true;
			                                         		component.Refresh(true);
			                                         		c.Complete();
			                                         	}, true);
			ProgressDialog.Show(task, this.Context.DesktopWindow, true, ProgressBarStyle.Continuous);

			if (success)
			{
				component.BulkOperationsMode = false;
				base.Context.DesktopWindow.Workspaces.AddNew(component, SR.TitleStudyFilters);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			this.UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.UpdateEnabled();
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			base.OnSelectedStudyChanged(sender, e);
			this.UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			base.Enabled = this.IsLocalStudyLoaderSupported
						   && base.Context.SelectedServerGroup != null && base.Context.SelectedServerGroup.IsLocalDatastore
						   && base.Context.SelectedStudies != null && base.Context.SelectedStudies.Count > 0;
			base.Visible = base.Context.SelectedServerGroup != null && base.Context.SelectedServerGroup.IsLocalDatastore;
		}

		private static IStudyLoader CreateLoader()
		{
			return StudyLoader.Create("DICOM_LOCAL");
		}
	}
}