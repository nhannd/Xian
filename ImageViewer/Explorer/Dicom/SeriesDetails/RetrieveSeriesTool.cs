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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ButtonAction("activate", ToolbarActionSite + "/ToolbarRetrieveSeries", "RetrieveSeries")]
	[MenuAction("activate", ContextMenuActionSite + "/MenuRetrieveSeries", "RetrieveSeries")]
	[Tooltip("activate", "TooltipRetrieveSeries")]
    [IconSet("activate", "Icons.RetrieveSeriesToolSmall.png", "Icons.RetrieveSeriesToolSmall.png", "Icons.RetrieveSeriesToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", ImageViewer.AuthorityTokens.Study.Retrieve)]
    [ExtensionOf(typeof (SeriesDetailsToolExtensionPoint))]
	public class RetrieveSeriesTool : SeriesDetailsTool
	{
		public void RetrieveSeries()
		{
			if (!Enabled || SelectedSeries.Count == 0)
				return;

		    var client = new DicomRetrieveBridge();
            var seriesUids = Context.SelectedSeries.Select(item => item.SeriesInstanceUid).ToList();
		 
			try
			{
                client.RetrieveSeries(Server, Context.Study, seriesUids.ToArray());

                DateTime? studyDate = DateParser.Parse(Context.Study.StudyDate);
                Context.DesktopWindow.ShowAlert(AlertLevel.Info,
                                string.Format(SR.MessageFormatRetrieveSeriesScheduled, seriesUids.Count,
                                              Server.Name, Context.Study.PatientsName.FormattedName, studyDate.HasValue ? Format.Date(studyDate.Value) : string.Empty,
                                              Context.Study.AccessionNumber),
                                SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
                  
			}
			catch (EndpointNotFoundException)
			{
			    Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception ex)
			{
			    ExceptionHandler.Report(ex, SR.MessageFailedToRetrieveSeries, Context.DesktopWindow);
			}

			//TODO (CR Sept 2010): put a Close method on the context, or put a property on SeriesDetailsTool
			//that somehow allows a tool to flag that when it is clicked, the component should close.
			//if (result == EventResult.Success)
			//	SeriesDetailsComponent.Close();
            
		}

		protected override void OnSelectedSeriesChanged()
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
		    Enabled = Context.SelectedSeries.Count > 0
                      && !Context.Server.IsLocal
		              && WorkItemActivityMonitor.IsRunning;
		}
	}
}