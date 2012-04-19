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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ButtonAction("activate", ToolbarActionSite + "/ToolbarSendSeries", "SendSeries")]
    [MenuAction("activate", ContextMenuActionSite + "/MenuSendSeries", "SendSeries")]
    [Tooltip("activate", "TooltipSendSeries")]
    [IconSet("activate", "Icons.SendSeriesToolSmall.png", "Icons.SendSeriesToolSmall.png", "Icons.SendSeriesToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Send)]
    [ExtensionOf(typeof (SeriesDetailsToolExtensionPoint))]
    public class SendSeriesTool : SeriesDetailsTool
	{
        public void SendSeries()
		{
            throw new NotImplementedException("Marmot - need to restore this.");
		}

		protected override void OnSelectedSeriesChanged()
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = (Context.SelectedSeries != null &&
			           Context.SelectedSeries.Count > 0 &&
			           //TODO (Marmot): This determines local/remote; will be fixing this shortly.
                       Server == null &&
			           WorkItemActivityMonitor.IsRunning);
		}
	}
}