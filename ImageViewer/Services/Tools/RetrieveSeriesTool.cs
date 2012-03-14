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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", ToolbarActionSite + "/ToolbarRetrieveSeries", "RetrieveSeries")]
	[MenuAction("activate", ContextMenuActionSite + "/MenuRetrieveSeries", "RetrieveSeries")]
	[Tooltip("activate", "TooltipRetrieveSeries")]
	[IconSet("activate", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("activate", Services.AuthorityTokens.Study.Retrieve)]
	[ExtensionOf(typeof (SeriesDetailsToolExtensionPoint))]
	public class RetrieveSeriesTool : SeriesDetailsTool
	{
		public void RetrieveSeries()
		{
			if (!Enabled || SelectedSeries.Count == 0)
				return;

			var result = EventResult.Success;

			var applicationEntity = Server as ApplicationEntity;
			if (applicationEntity == null)
				return;

			var studyInformation = new StudyInformation();
			studyInformation.PatientId = Patient.PatientId;
			studyInformation.PatientsName = Patient.PatientsName;
			studyInformation.StudyDate = ParseDicomDate(Study.StudyDate);
			studyInformation.StudyDescription = Study.StudyDescription;
			studyInformation.StudyInstanceUid = Study.StudyInstanceUid;

			var seriesToRetrieve = new List<string>(CollectionUtils.Map<ISeriesIdentifier, string>(SelectedSeries, s => s.SeriesInstanceUid));

			var client = new DicomServerServiceClient();
			try
			{
				client.Open();

				var aeInformation = new AEInformation();
				aeInformation.AETitle = applicationEntity.AETitle;
				aeInformation.HostName = applicationEntity.Host;
				aeInformation.Port = applicationEntity.Port;

				client.RetrieveSeries(aeInformation, studyInformation, seriesToRetrieve);

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
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
				AuditHelper.LogBeginReceiveInstances(applicationEntity.AETitle, applicationEntity.Host, requestedInstances, EventSource.CurrentUser, result);

			}

			//TODO (CR Sept 2010): put a Close method on the context, or put a property on SeriesDetailsTool
			//that somehow allows a tool to flag that when it is clicked, the component should close.
			if (result == EventResult.Success)
				SeriesDetailsComponent.Close();
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
			           LocalDataStoreActivityMonitor.IsConnected);
		}

		private static DateTime ParseDicomDate(string dicomDate)
		{
			DateTime value;
			DateParser.Parse(dicomDate, out value);
			return value;
		}
	}
}