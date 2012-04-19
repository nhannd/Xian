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
    [ButtonAction("activate", ToolbarActionSite + "/ToolbarDeleteSeries", "DeleteSeries")]
    [MenuAction("activate", ContextMenuActionSite + "/MenuDeleteSeries", "DeleteSeries")]
    [Tooltip("activate", "TooltipDeleteSeries")]
    [IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Delete)]
    [ExtensionOf(typeof(SeriesDetailsToolExtensionPoint))]
    public class DeleteSeriesTool : SeriesDetailsTool
    {
        public void DeleteSeries()
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