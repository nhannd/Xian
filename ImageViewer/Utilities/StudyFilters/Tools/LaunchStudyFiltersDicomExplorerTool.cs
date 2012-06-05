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
		    var studyLoaders = new List<IStudyLoader>();
			int sopCount = 0;
			foreach (var studyItem in base.Context.SelectedStudies)
			{
                if (!studyItem.Server.IsSupported<IStudyLoader>())
                    return;

			    var loader = studyItem.Server.GetService<IStudyLoader>();
                studyLoaders.Add(loader);
			    sopCount += loader.Start(new StudyLoaderArgs(studyItem.StudyInstanceUid, studyItem.Server));
			}

			bool success = false;
		    var component = new StudyFilterComponent {BulkOperationsMode = true};
		    var task = new BackgroundTask(c =>
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
            base.Enabled = Context.SelectedStudies.Count > 0
                //TODO (Marmot):Not sure why it was restricted to local, but I'm leaving it.
                && base.Context.SelectedServers.IsLocalServer
                && base.Context.SelectedServers.AllSupport<IStudyLoader>();

            //TODO (Marmot): Not sure why Enabled is restricted to local. Check here too for consistency.
            Visible = Context.SelectedServers.IsLocalServer && Context.SelectedServers.AllSupport<IStudyLoader>();
		    
		}
	}
}