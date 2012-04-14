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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ButtonAction("activate", ToolbarActionSite + "/ToolbarRetrieveSeries", "RetrieveSeries")]
	[MenuAction("activate", ContextMenuActionSite + "/MenuRetrieveSeries", "RetrieveSeries")]
	[Tooltip("activate", "TooltipRetrieveSeries")]
    [IconSet("activate", "Icons.RetrieveSeriesToolSmall.png", "Icons.RetrieveSeriesToolSmall.png", "Icons.RetrieveSeriesToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ViewerActionPermission("activate", Common.AuthorityTokens.Study.Retrieve)]
    [ExtensionOf(typeof (SeriesDetailsToolExtensionPoint))]
	public class RetrieveSeriesTool : SeriesDetailsTool
	{
		public void RetrieveSeries()
		{
            throw new NotImplementedException("Marmot - need to restore this.");

            /*

			if (!Enabled || SelectedSeries.Count == 0)
				return;

            var result = EventResult.Success;

			var applicationEntity = Server as IApplicationEntity;
			if (applicationEntity == null || applicationEntity.ScpParameters == null)
				return;

			var studyInformation = new StudyInformation
			                           {
			                               PatientId = Patient.PatientId,
			                               PatientsName = Patient.PatientsName,
			                               StudyDate = ParseDicomDate(Study.StudyDate),
			                               StudyDescription = Study.StudyDescription,
			                               StudyInstanceUid = Study.StudyInstanceUid
			                           };

		    var seriesToRetrieve = new List<string>(CollectionUtils.Map<ISeriesIdentifier, string>(SelectedSeries, s => s.SeriesInstanceUid));

			try
			{
			    Platform.GetService(delegate(IDicomServer service)
			            {
			                var source = new ApplicationEntity
			                                 {
			                                     AETitle = applicationEntity.AETitle,
			                                     ScpParameters = new ScpParameters(applicationEntity.ScpParameters)
			                                 };

			                service.RetrieveSeries(source, studyInformation, seriesToRetrieve);
			            });

			    //TODO (Marmot): Restore - notify the user it's been scheduled.
                //LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				result = EventResult.MajorFailure;
				Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception ex)
			{
				result = EventResult.MajorFailure;
				ExceptionHandler.Report(ex, SR.MessageFailedToRetrieveStudy, Context.DesktopWindow);
			}
			finally
			{
				var requestedInstances = new AuditedInstances();
				requestedInstances.AddInstance(studyInformation.PatientId, studyInformation.PatientsName, studyInformation.StudyInstanceUid);
				AuditHelper.LogBeginReceiveInstances(applicationEntity.AETitle, applicationEntity.ScpParameters.HostName, requestedInstances, EventSource.CurrentUser, result);

			}

			//TODO (CR Sept 2010): put a Close method on the context, or put a property on SeriesDetailsTool
			//that somehow allows a tool to flag that when it is clicked, the component should close.
			if (result == EventResult.Success)
				SeriesDetailsComponent.Close();
            */
		}

		protected override void OnSelectedSeriesChanged()
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = (Context.SelectedSeries != null &&
			           Context.SelectedSeries.Count > 0 &&
			           Server != null &&
			           WorkItemActivityMonitor.IsRunning);
		}

		private static DateTime ParseDicomDate(string dicomDate)
		{
			DateTime value;
			DateParser.Parse(dicomDate, out value);
			return value;
		}
	}
}